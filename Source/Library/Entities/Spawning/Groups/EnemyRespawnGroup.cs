// <copyright file="EnemyRespawnGroup.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.EnemyRespawnGroup class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a group of enemies (of the same template) that can respawn.
    /// </summary>
    public class EnemyRespawnGroup : IEnemyRespawnGroup
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the enemy-template that is
        /// spawned in the <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        public string EnemyTemplateName
        {
            get
            {
                return this.enemyTemplateName; 
            }

            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                this.enemyTemplateName = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of enemies that are spawned 
        /// in the <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        public int Size
        {
            get 
            { 
                return this.count;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value", value, Atom.ErrorStrings.SpecifiedValueIsNegative );

                this.count = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Cooldown"/> it takes for a died enemy
        /// of the <see cref="EnemyRespawnGroup"/> to respawn.
        /// </summary>
        public Cooldown RespawnCooldown
        {
            get 
            {
                return this.respawnCooldown;
            }

            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                this.respawnCooldown = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISpawnPoint"/> this EnemyRespawnGroup is spawning enemies at.
        /// </summary>
        protected ISpawnPoint SpawnPoint
        {
            get
            {
                return this._spawnPoint;
            }

            private set
            {
                ISpawnPoint old = this._spawnPoint;
                this._spawnPoint = value;

                this.OnSpawnPointChanged( old, value );
            }
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
        protected virtual void OnSpawnPointChanged( ISpawnPoint oldSpawnPoint, ISpawnPoint newSpawnPoint )
        {
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyRespawnGroup"/> class.
        /// </summary>
        public EnemyRespawnGroup()
        {
            this.respawnCooldown = new Cooldown( 300.0f );
        }

        /// <summary>
        /// Creates all enemies in the <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        /// <param name="spawnPoint">
        /// The ISpawnPoint in which enemies are spawned.
        /// </param>
        /// <param name="templateManager">
        /// The <see cref="EntityTemplateManager"/> object.
        /// </param>
        public void Create( ISpawnPoint spawnPoint, EntityTemplateManager templateManager )
        {
            #region - Verify -

            if( spawnPoint == null )
                throw new ArgumentNullException( "spawnPoint" );

            if( templateManager == null )
                throw new ArgumentNullException( "templateManager" );

            if( this.respawnCooldown == null )
                throw new InvalidOperationException( Resources.Error_RespawnCooldownIsNull );

            #endregion

            if( string.IsNullOrEmpty( this.enemyTemplateName ) )
            {
                this.enemyContainers = new EnemyContainer[0];
            }
            else
            {
                this.enemyContainers = new EnemyContainer[count];

                // All enemies are created from the same template.
                IEntityTemplate enemyTemplate = templateManager.GetTemplate( enemyTemplateName );

                for( int i = 0; i < count; ++i )
                {
                    this.enemyContainers[i] = this.CreateEnemyContainer( enemyTemplate );
                }
            }

            this.SpawnPoint = spawnPoint;
        }

        /// <summary>
        /// Creates and returns a new EnemyContainer.
        /// </summary>
        /// <param name="enemyTemplate">
        /// The IEntityTemplate that should be used to create instances of the enemy.
        /// </param>
        /// <returns>
        /// The new EnemyContainer.
        /// </returns>
        private EnemyContainer CreateEnemyContainer( IEntityTemplate enemyTemplate )
        {
            Enemy enemy = (Enemy)enemyTemplate.CreateInstance();
            enemy.IsSaved    = false;
            enemy.IsEditable = false;

            var cooldown = this.respawnCooldown.IsShared ? this.respawnCooldown : new Cooldown( this.respawnCooldown.TotalTime );
           
            var container = new EnemyContainer( enemy, cooldown );
            this.SetupEnemyContainer( container );
            return container;
        }

        /// <summary>
        /// Includes additional setup logic for a new EnemyContainer.
        /// </summary>
        /// <param name="container">
        /// The new EnemyContainer that should be setup.
        /// </param>
        protected virtual void SetupEnemyContainer( EnemyContainer container )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.enemyContainers == null )
                return;

            for( int i = 0; i < this.enemyContainers.Length; ++i )
            {
                this.UpdateContainer( this.enemyContainers[i], updateContext );
            }
        }

        /// <summary>
        /// Updates the given EnemyContainer.
        /// </summary>
        /// <param name="container">
        /// The EnemyContainer to update.
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateContainer( EnemyContainer container, ZeldaUpdateContext updateContext )
        {
            if( container == null || !container.IsRespawning )
                return;

            container.RespawnCooldown.Update( updateContext.FrameTime );
            this.TrySpawn( container );
        }

        /// <summary>
        /// Tries to spawn the Enemy in the given EnemyContainer.
        /// </summary>
        /// <param name="container">
        /// The EnemyContainer whose .
        /// </param>
        private void TrySpawn( EnemyContainer container )
        {
            if( this.ShouldSpawn( container ) )
            {
                container.Spawn( this.SpawnPoint );
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
        protected virtual bool ShouldSpawn( EnemyContainer container )
        {
            return container.RespawnCooldown.IsReady;
        }

        /// <summary>
        /// Spawns the <see cref="Enemy"/> entities of this IEnemyRespawnGroup.
        /// </summary>
        protected void Spawn()
        {
            for( int i = 0; i < this.enemyContainers.Length; ++i )
            {
                var container = this.enemyContainers[i];
                container.Spawn( this.SpawnPoint );
            }
        }

        /// <summary>
        /// Despawns the <see cref="Enemy"/> entities of this IEnemyRespawnGroup.
        /// </summary>
        protected void Despawn()
        {
            for( int i = 0; i < this.enemyContainers.Length; ++i )
            {
                var container = this.enemyContainers[i];
                container.Despawn();
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( this.Size );
            context.Write( this.EnemyTemplateName ?? string.Empty );
            context.Write( this.RespawnCooldown.TotalTime );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, typeof( EnemyRespawnGroup ) );

            this.Size                         = context.ReadInt32();
            this.EnemyTemplateName            = context.ReadString();
            this.RespawnCooldown.TotalTime    = context.ReadSingle();

            if( version == 1 )
            {
                // depracted. Used to read the AllowSpawningWhenPlayerInSight property.
                context.ReadBoolean();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The name of the enemy-template that is spawned in the <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        private string enemyTemplateName;

        /// <summary>
        /// The number of enemies that are spawned in the <see cref="EnemyRespawnGroup"/>.
        /// </summary>
        private int count;

        /// <summary>
        /// The <see cref="Cooldown"/> it takes for a died enemy of the <see cref="EnemyRespawnGroup"/> to respawn.
        /// </summary>
        private Cooldown respawnCooldown;

        /// <summary>
        /// The list of enemies.
        /// </summary>
        private EnemyContainer[] enemyContainers;

        /// <summary>
        /// The storage field of the <see cref="SpawnPoint"/> property.
        /// </summary>
        private ISpawnPoint _spawnPoint;

        #endregion

        #region [ class EnemyContainer ]

        /// <summary>
        /// Stores the data about an Enemy.
        /// </summary>
        protected sealed class EnemyContainer
        {
            /// <summary>
            /// The <see cref="Enemy"/> stored in the <see cref="EnemyContainer"/>. 
            /// </summary>
            public readonly Enemy Enemy;

            /// <summary>
            /// Stores the cooldown
            /// </summary>
            public readonly Cooldown RespawnCooldown;

            /// <summary>
            /// States whether the Enemy is currently respawning.
            /// </summary>
            public bool IsRespawning = true;

            /// <summary>
            /// Specifies whether the next respawn
            /// is the initial spawn of the enemy.
            /// </summary>
            public bool IsInitialSpawn = true;

            /// <summary>
            /// Initializes a new instance of the <see cref="EnemyContainer"/> class.
            /// </summary>
            /// <param name="enemy">
            /// The Enemy that is stored by the new EnemyContainer.
            /// </param>
            /// <param name="respawnCooldown">
            /// Stores the time the enemy takes to respawn once he has been killed.
            /// </param>
            public EnemyContainer( Enemy enemy, Cooldown respawnCooldown )
            {
                Debug.Assert( enemy != null );
                Debug.Assert( respawnCooldown != null );

                this.Enemy           = enemy;
                this.RespawnCooldown = respawnCooldown;
                this.RespawnCooldown.TimeLeft = 0.0f;

                this.Enemy.Killable.Killed += this.OnEnemyKilled;
            }

            /// <summary>
            /// Spawns the Enemy.
            /// </summary>
            /// <param name="spawnPoint">
            /// The ISpawnPoint to spawn the enemy at.
            /// </param>
            public void Spawn( ISpawnPoint spawnPoint )
            {
                if( this.Enemy == null || this.Enemy.Scene != null )
                    return;

                this.ResetStateForSpawn();

                if( this.IsInitialSpawn )
                {
                    this.IsInitialSpawn = false;
                }
                else
                {
                    SpawnHelper.AddBlendInColorTint( this.Enemy );
                }

                spawnPoint.Spawn( this.Enemy );
                this.ResetBehaviour();
                this.IsRespawning = false;
            }

            /// <summary>
            /// Resets the state of this Enemy to make it spawnable.
            /// </summary>
            private void ResetStateForSpawn()
            {
                this.Enemy.Statable.AuraList.Clear();
                this.Enemy.Statable.Life = this.Enemy.Statable.MaximumLife;
                this.Enemy.Statable.Mana = this.Enemy.Statable.MaximumMana;
                this.Enemy.Moveable.ResetPush();
            }

            private void ResetBehaviour()
            {
                var behaviour = this.Enemy.Behaveable.Behaviour;

                if( behaviour != null )
                {
                    behaviour.Reset();
                    behaviour.Enter();
                }
            }

            /// <summary>
            /// Despawn the Enemy of this EnemyRespawnGroup.
            /// </summary>
            internal void Despawn()
            {
                var enemy = this.Enemy;

                if( enemy != null )
                {
                    SpawnHelper.Despawn( enemy );
                }
            }

            /// <summary>
            /// Gets called when the Enemy has been killed.
            /// </summary>
            /// <param name="sender">
            /// The sender of the event.
            /// </param>
            private void OnEnemyKilled( Zelda.Entities.Components.Killable sender )
            {
                this.IsRespawning = true;
                this.RespawnCooldown.Reset();
            }
        }

        #endregion
    }
}