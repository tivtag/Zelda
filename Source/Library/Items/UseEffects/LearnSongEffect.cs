// <copyright file="LearnSongEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.LearnSongEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.UseEffects
{
    using System;
    using System.ComponentModel;
    using Atom;
    using Zelda.Ocarina;
    
    /// <summary>
    /// Represents an ItemUseEffect that learns the player
    /// a new <see cref="Song"/>.
    /// </summary>
    public sealed class LearnSongEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the song that is learned by this LearnSongEffect.
        /// </summary>
        [Editor( typeof( Zelda.Ocarina.Songs.Design.SongTypeNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SongTypeName
        {
            get 
            { 
                return this.Song == null ? string.Empty : this.Song.GetType().GetTypeName(); 
            }

            set
            {
                var songType = Type.GetType( value );

                if( songType == null || !typeof( Song ).IsAssignableFrom( songType ) )
                {
                    throw new ArgumentException( Zelda.Resources.Error_TheGivenSongNameIsInvalid );
                }
                
                var song = (Song)Activator.CreateInstance( songType );

                var setupable = song as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( this.serviceProvider );

                this.Song = song;
            }
        }

        /// <summary>
        /// Gets the Song that is learnt by this LearnSongEffect.
        /// </summary>
        public Song Song
        {
            get;
            private set;
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
        public override string GetDescription( Zelda.Status.Statable statable )
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                ItemResources.IUE_LearnSong,
                this.Song.DescriptionData.LocalizedName
            );
        }

        /// <summary>
        /// Gets the item budget used by this ItemUseEffect.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get { return 10.0; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the LearnSongEffect class.
        /// </summary>
        public LearnSongEffect()
        {
            this.DestroyItemOnUse = true;
        }

        /// <summary>
        /// Setups this LearnSongEffect.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this LearnSongEffect.
        /// </summary>
        /// <param name="user">
        /// The player that used the item.
        /// </param>
        /// <returns>
        /// True if the item has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Zelda.Entities.PlayerEntity user )
        {
            if( this.Song == null )
                return false;

            var ocarinaBox = user.OcarinaBox;

            if( !ocarinaBox.HasSong( this.Song.GetType() ) )
            {
                ocarinaBox.AddSong( this.Song );
                this.PlaySample();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Plays the sample sounds associated with learning a new song.
        /// </summary>
        private void PlaySample()
        {
            var sample = this.serviceProvider.AudioSystem.GetSample( "Bling_1.ogg" );

            if( sample != null )
            {
                sample.LoadAsSample( false );
                sample.Play();
            }
        }
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.SongTypeName );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.SongTypeName = context.ReadString();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
