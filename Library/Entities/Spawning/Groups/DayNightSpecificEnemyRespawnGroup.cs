// <copyright file="DayNightSpecificEnemyRespawnGroup.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.DayNightSpecificEnemyRespawnGroup class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System;

    /// <summary>
    /// Represents an <see cref="EnemyRespawnGroup"/> that spawns
    /// a group of enemies at a specified time of the day.
    /// </summary>
    /// <seealso cref="DayNightState"/>
    public sealed class DayNightSpecificEnemyRespawnGroup : EnemyRespawnGroup
    {
        /// <summary>
        /// Gets or sets the <see cref="DayNightState"/> in which Enemies of this
        /// DayNightSpecificEnemyRespawnGroup are active.
        /// </summary>
        public DayNightState SpawnOnlyAt
        {
            get 
            {
                return this.spawnOnlyAtState; 
            }

            set
            {
                if( value == DayNightState.None )
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "value" );

                this.spawnOnlyAtState = value;
            }
        }

        /// <summary>
        /// Gets called when Day has changed to Evening; or Evening to Night
        /// or Night to Day.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The DayNightEvent that has occurred.
        /// </param>
        private void OnDayNightCycleEvent( object sender, DayNightEvent e )
        {
            if( this.ShouldSpawnEnemies( e ) )
            {
                this.Spawn();
            }

            else if( this.ShouldDespawnEnemies( e ) )
            {
                this.Despawn();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the enemy stored in the given EnemyContainer
        /// should be respawned now.
        /// </summary>
        /// <param name="container">
        /// The EnemyContainer that should be spawned.
        /// </param>
        /// <returns>
        /// Returns true if the enemy should respawned;
        /// otherwise false.
        /// </returns>
        protected override bool ShouldSpawn( EnemyContainer container )
        {
            var dayNightCycle = this.SpawnPoint.Scene.DayNightCycle;

            if( dayNightCycle.State == this.SpawnOnlyAt )
            {
                return base.ShouldSpawn( container );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the enemies of this IEnemyRespawnGroup
        /// should be spawned when the given DayNightEvent occurs.
        /// </summary>
        /// <param name="e">
        /// The DayNightEvent that has occurred.
        /// </param>
        /// <returns>
        /// true if enemies should be spawned;
        /// otherwise false.
        /// </returns>
        private bool ShouldSpawnEnemies( DayNightEvent e )
        {
            return e == DayNightCycle.GetBeginEvent( this.SpawnOnlyAt );
        }

        /// <summary>
        /// Gets a value indicating whether the enemies of this IEnemyRespawnGroup
        /// should be de-spawned when the given DayNightEvent occurs.
        /// </summary>
        /// <param name="e">
        /// The DayNightEvent that has occurred.
        /// </param>
        /// <returns>
        /// true if enemies should be de-spawned;
        /// otherwise false.
        /// </returns>
        private bool ShouldDespawnEnemies( DayNightEvent e )
        {
            return e == DayNightCycle.GetEndEvent( this.SpawnOnlyAt );
        }

        /// <summary>
        /// Called when the <see cref="SpawnPoint"/> property has changed.
        /// </summary>
        /// <param name="oldSpawnPoint">
        /// The old value of the SpawnPoint property.
        /// </param>
        /// <param name="newSpawnPoint">
        /// The new value of the SpawnPoint property.
        /// </param>
        protected override void OnSpawnPointChanged( ISpawnPoint oldSpawnPoint, ISpawnPoint newSpawnPoint )
        {
            if( oldSpawnPoint != null )
            {
                this.UnhookEvents( oldSpawnPoint );
            }

            if( newSpawnPoint != null )
            {
                this.HookEvents( newSpawnPoint );
            }
        }

        /// <summary>
        /// Hooks up events with the given ISpawnPoint.
        /// </summary>
        /// <param name="spawnPoint">
        /// The related ISpawnPoint.
        /// </param>
        private void HookEvents( ISpawnPoint spawnPoint )
        {
            spawnPoint.Added += this.OnSpawnPointAddedToScene;
            spawnPoint.Removed += this.OnSpawnPointRemovedFromScene;

            if( spawnPoint.Scene != null )
            {
                this.HookSceneEvents( spawnPoint.Scene );
            }
        }

        /// <summary>
        /// Unhooks up events from the given ISpawnPoint.
        /// </summary>
        /// <param name="spawnPoint">
        /// The related ISpawnPoint.
        /// </param>
        private void UnhookEvents( ISpawnPoint spawnPoint )
        {
            spawnPoint.Added -= this.OnSpawnPointAddedToScene;
            spawnPoint.Removed -= this.OnSpawnPointRemovedFromScene;

            if( spawnPoint.Scene != null )
            {
                this.UnhookSceneEvents( spawnPoint.Scene );
            }
        }

        /// <summary>
        /// Gets called when the ISpawnPoint this DayNightSpecificEnemyRespawnGroup is
        /// using has been added to a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnSpawnPointAddedToScene( object sender, ZeldaScene scene )
        {
            this.HookSceneEvents( scene );
        }

        /// <summary>
        /// Gets called when the ISpawnPoint this DayNightSpecificEnemyRespawnGroup is
        /// using has been removed from a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnSpawnPointRemovedFromScene( object sender, ZeldaScene scene )
        {
            this.UnhookSceneEvents( scene );
        }

        /// <summary>
        /// Hooks up the EventsHandlers with the given ZeldaScene.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void HookSceneEvents( ZeldaScene scene )
        {
            scene.DayNightCycle.Event += this.OnDayNightCycleEvent;
        }

        /// <summary>
        /// Unhooks up the EventsHandlers with the given ZeldaScene.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void UnhookSceneEvents( ZeldaScene scene )
        {
            scene.DayNightCycle.Event -= this.OnDayNightCycleEvent;
        }

        /// <summary>
        /// Includes additional setup logic for a new EnemyContainer.
        /// </summary>
        /// <param name="container">
        /// The new EnemyContainer that should be setup.
        /// </param>
        protected override void SetupEnemyContainer( EnemyContainer container )
        {
            // DayNight specific enemies might spawn 
            // much later. Because of this we want
            // the enemy to be 'blended' into the scene
            // instead of spawned Deferredly without
            // blending.
            container.IsInitialSpawn = false;
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

            const int Version = 1;
            context.Write( Version );

            context.Write( (byte)this.spawnOnlyAtState );
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

            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, typeof( EnemyRespawnGroup ) );

            this.spawnOnlyAtState = (DayNightState)context.ReadByte();            
        }

        /// <summary>
        /// The storage field of the <see cref="SpawnOnlyAt"/> property.
        /// </summary>
        private DayNightState spawnOnlyAtState = DayNightState.Day;
    }
}
