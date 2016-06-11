// <copyright file="PlayerSpawnPoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.PlayerSpawnPoint class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using Atom.Math;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents a <see cref="SpawnPoint"/> that only
    /// allows the <see cref="PlayerEntity"/> to respawn.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PlayerSpawnPoint : SpawnPoint
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the direction the PlayerEntity is turned into after spawning. 
        /// </summary>
        /// <remarks>
        /// If set to <see cref="Direction4.None"/> the PlayerEntity will maintain his old direction.
        /// </remarks>
        /// <value>The default value is <see cref="Direction4.None"/>.</value>
        public Direction4 SpawnDirection
        {
            get 
            {
                return this.Transform.Direction;
            }

            set
            {
                this.Transform.Direction = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this PlayerSpawnPoint
        /// will be set as the spawn location when the player saves
        /// his game.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool SaveLocationAtSpawn
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSpawnPoint"/> class.
        /// </summary>
        public PlayerSpawnPoint()
        {
            this.SpawnDirection = Direction4.None;
            this.SaveLocationAtSpawn = true;
            this.Collision.Size = new Vector2( 1.0f, 1.0f );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Spawns the given <see cref="PlayerEntity"/> at this <see cref="PlayerSpawnPoint"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If <paramref name="entity"/> is not of type <see cref="PlayerEntity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <param name="entity">
        /// The entity to spawn.
        /// </param>
        public override void Spawn( ZeldaEntity entity )
        {
            // Spawn
            base.Spawn( entity );

            // Apply spawn direction
            if( this.SpawnDirection != Direction4.None )
                entity.Transform.Direction = this.SpawnDirection;

            // Set this Spawn Point as the last Save Point
            if( this.SaveLocationAtSpawn )
            {
                var player = entity as PlayerEntity;

                if( player != null )
                {
                    player.Profile.LastSavePoint = new Zelda.Saving.SavePoint( this.Scene.Name, this.Name );
                }
            }

            this.Scene.Camera.Update();
        }

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="PlayerSpawnPoint"/>
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            PlayerSpawnPoint clone = new PlayerSpawnPoint();

            SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="PlayerSpawnPoint"/> to be a clone of this PlayerSpawnPoint.
        /// </summary>
        /// <param name="clone">
        /// The PlayerSpawnPoint to setup.
        /// </param>
        private void SetupClone( PlayerSpawnPoint clone )
        {
            base.SetupClone( clone );

            clone.SpawnDirection = this.SpawnDirection;
        }

        #endregion

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="PlayerSpawnPoint"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<PlayerSpawnPoint>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>   
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( PlayerSpawnPoint entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                const int CurrentVersion = 2;
                context.WriteHeader( CurrentVersion );

                // Write Spawn Point Data:
                context.Write( entity.Name );
                context.Write( entity.FloorNumber );

                context.Write( entity.Transform.X );
                context.Write( entity.Transform.Y );

                context.Write( entity.Collision.Size.X );
                context.Write( entity.Collision.Size.Y );

                context.Write( (int)entity.SpawnDirection );
                context.Write( entity.SaveLocationAtSpawn );
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( PlayerSpawnPoint entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                const int CurrentVersion = 2;
                int version = context.ReadHeader( 0, CurrentVersion, this.GetType() );

                // Read Name
                entity.Name        = context.ReadString();
                entity.FloorNumber = context.ReadInt32();

                // Read Transform
                Vector2 position;
                position.X = context.ReadSingle();
                position.Y = context.ReadSingle();
                entity.Transform.Position = position;

                Vector2 size;
                size.X = context.ReadSingle();
                size.Y = context.ReadSingle();
                entity.Collision.Size = size;

                entity.SpawnDirection = (Direction4)context.ReadInt32();

                if( version >= 2 )
                {
                    entity.SaveLocationAtSpawn = context.ReadBoolean();
                }
            }
        }

        #endregion
    }
}
