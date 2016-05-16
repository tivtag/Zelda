// <copyright file="SpawnPoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.SpawnPoint class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Saving;

    /// <summary>
    /// Defines a basic implementation of the <see cref="ISpawnPoint"/> interface.
    /// </summary>
    public class SpawnPoint : ZeldaEntity, ISpawnPoint, IEditModeDrawable, IZeldaSetupable
    {
        #region [ Properties ]
        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPoint"/> class.
        /// </summary>
        public SpawnPoint()
        {
            if( !ZeldaScene.EditorMode )
            {
                // Make SpawnPoints invisible in the normal game. 
                // This improves runtime perfomance.
                this.IsVisible = false;
            }

            this.Collision.Size = new Vector2( 16.0f, 16.0f );
        }

        /// <summary>
        /// Setups this <see cref="EnemySpawnPoint"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Spawns the given <see cref="ZeldaEntity"/> at this <see cref="SpawnPoint"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is null.</exception>
        /// <param name="entity">
        /// The entity to spawn.
        /// </param>
        public virtual void Spawn( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            if( entity.Scene != null )
                entity.RemoveFromScene();

            entity.Transform.Position = GetSpawnPosition();
            entity.FloorNumber        = this.FloorNumber;
            entity.AddToScene( this.Scene );

            this.OnSpawned( entity );
        }

        private void OnSpawned( ZeldaEntity entity )
        {
            var spawnable = entity as ISpawnableEntity;

            if( spawnable != null )
            {
                spawnable.Spawnable.NotifySpawnedAt( this );
            }
        }

        /// <summary>
        /// Receives a position within the spawn area of this SpawnPoint.
        /// </summary>
        /// <returns>The new spawn position.</returns>
        private Vector2 GetSpawnPosition()
        {
            if( rand == null )
                return this.Transform.Position;

            var rectangle = this.Collision.Rectangle;

            Vector2 position;
            position.X = rectangle.X + (rand.RandomSingle * rectangle.Width);
            position.Y = rectangle.Y + (rand.RandomSingle * rectangle.Height);

            return position;
        }

        #region > Drawing <

        /// <summary>
        /// Draws this <see cref="SpawnPoint"/> in edit-mode.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public virtual void DrawEditMode( ZeldaDrawContext drawContext )
        {
            if( !IsVisible )
                return;

            var rectangle = (Rectangle)this.Collision.Rectangle;

            if( rectangle.Width <= 1 )
            {
                rectangle.Width = 16;
                rectangle.X    -= 8;
            }

            if( rectangle.Height <= 1 )
            {
                rectangle.Height = 16;
                rectangle.Y     -= 8;
            }

            drawContext.Batch.DrawRect( 
                rectangle, 
                new Microsoft.Xna.Framework.Color( 0, 0, 180, 150 ) 
            );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="SpawnPoint"/>
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            SpawnPoint clone = new SpawnPoint();
            SetupClone( clone );
            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="SpawnPoint"/> to be a clone of this SpawnPoint.
        /// </summary>
        /// <param name="clone">
        /// The SpawnPoint to setup.
        /// </param>
        protected void SetupClone( SpawnPoint clone )
        {
            clone.rand = this.rand;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="SpawnPoint"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<SpawnPoint>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReaderWriter"/> class.
            /// </summary>   
            /// <exception cref="ArgumentNullException">
            /// If <paramref name="serviceProvider"/> is null.
            /// </exception>
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
            public override void Serialize( SpawnPoint entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Write Spawn Point Data:
                context.Write( entity.Name );
                context.Write( entity.FloorNumber );

                context.Write( entity.Transform.Position );
                context.Write( entity.Collision.Size );
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
            public override void Deserialize( SpawnPoint entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );
                context.ReadDefaultHeader( this.GetType() );

                // Read Name
                entity.Name        = context.ReadString();
                entity.FloorNumber = context.ReadInt32();

                entity.Transform.Position = context.ReadVector2();
                entity.Collision.Size = context.ReadVector2();
            }
        }

        #endregion
    }
}
