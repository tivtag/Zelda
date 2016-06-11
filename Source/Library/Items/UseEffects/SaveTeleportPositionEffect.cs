// <copyright file="SaveTeleportPositionEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.SaveTeleportPositionEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.UseEffects
{
    using Atom.Math;
    using Zelda.Core.Properties.Scene;
    using Zelda.Ocarina.Songs.Teleportation;

    /// <summary>
    /// When used, stores the current location within the world data store of the current player.
    /// </summary>
    public sealed class SaveTeleportPositionEffect : ItemUseEffect
    {
        /// <summary>
        /// Setups this SaveTeleportPositionEffect.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;

            base.Setup( serviceProvider );
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Status.Statable statable )
        {
            return ItemResources.IUE_SaveTeleportPosition;
        }

        /// <summary>
        /// Uses this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Entities.PlayerEntity user )
        {
            this.LearnSong( user );

            if( IsSavingAllowed( user ) )
            {
                Save( user );
                this.PlaySound();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the current location of the player in the world-wide data store.
        /// </summary>
        /// <param name="user">
        /// The player entity that executed this ItemUseEffect.
        /// </param>
        private static void Save( Entities.PlayerEntity user )
        {
            user.WorldStatus.DataStore
                .AddOrReplace(
                    CrossTeleportLocationStorage.Identifier,
                    new CrossTeleportLocationStorage() {
                        SceneName = user.Scene.Name,
                        Position = user.Transform.Position,
                        FloorNumber = user.FloorNumber
                    }
                );
        }

        /// <summary>
        /// Gets a value indicating whether the specified player is currently
        /// allowed to save the current location.
        /// </summary>
        /// <param name="user">
        /// The player entity that executed this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if it is allowed -or- otherwise false.
        /// </returns>
        private static bool IsSavingAllowed( Entities.PlayerEntity user )
        {
            var sceneProperties = user.Scene.Settings.Properties;
            var dungeonProperty = sceneProperties.TryGet<DungeonProperty>();

            return dungeonProperty == null;
        }

        /// <summary>
        /// Learns the player the MusicCrossSong, if required.
        /// </summary>
        /// <param name="user">
        /// The player entity that executed this ItemUseEffect.
        /// </param>
        private void LearnSong( Entities.PlayerEntity user )
        {
            if( !user.OcarinaBox.HasSong( typeof( MusicCrossSong ) ) )
            {
                var song = new MusicCrossSong();
                song.Setup( this.serviceProvider );

                user.OcarinaBox.AddSong( song );
            }
        }

        /// <summary>
        /// Plays a simple 'success' sound sample.
        /// </summary>
        private void PlaySound()
        {
            var sample = this.serviceProvider.AudioSystem.GetSample( "PickUpPutDown_MetalLight.wav" );

            sample.LoadAsSample();
            sample.Play();
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }
}
