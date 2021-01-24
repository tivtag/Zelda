// <copyright file="EnemySpawnPoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.EnemySpawnPoint class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom;
    using Atom.Diagnostics;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines a <see cref="SpawnPoint"/> that spawns <see cref="Enemy"/> entities.
    /// </summary>
    /// <remarks>
    /// Enemies that got killed will respawn after a set time frame.
    /// </remarks>
    public class EnemySpawnPoint : SpawnPoint
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the list of <see cref="EnemyRespawnGroup"/>s this EnemySpawnPoint consists of.
        /// </summary>
        [Editor( "Zelda.Entities.Spawning.Design.EnemyRespawnGroupListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public List<IEnemyRespawnGroup> RespawnGroups
        {
            get 
            {
                return this.respawnGroups;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="EnemySpawnPoint"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < respawnGroups.Count; ++i )
                this.respawnGroups[i].Update( updateContext );

            base.Update( updateContext );
        }

        #region > Drawing <

        /// <summary>
        /// Draws this <see cref="EnemySpawnPoint"/> in edit-mode.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void DrawEditMode( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
                return;

            var rectangle = (Rectangle)this.Collision.Rectangle;

            if( rectangle.Width == 0 )
            {
                rectangle.Width = 16;
                rectangle.X    -= 8;
            }

            if( rectangle.Height == 0 )
            {
                rectangle.Height = 16;
                rectangle.Y     -= 8;
            }

            drawContext.Batch.DrawRect( 
                rectangle,
                new Microsoft.Xna.Framework.Color( 0, 100, 100, 150 )
            );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="EnemySpawnPoint"/>
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new EnemySpawnPoint();
            this.SetupClone( clone );
            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stroes the <see cref="EnemyRespawnGroup"/>s spawning in this EnemySpawnPoint.
        /// </summary>
        private readonly List<IEnemyRespawnGroup> respawnGroups = new List<IEnemyRespawnGroup>();

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="EnemySpawnPoint"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<EnemySpawnPoint>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
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
            public override void Serialize( EnemySpawnPoint entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                // Write Spawn Point Data:
                context.Write( entity.Name );
                context.Write( entity.FloorNumber );

                context.Write( entity.Transform.X );
                context.Write( entity.Transform.Y );

                context.Write( entity.Collision.Size.X );
                context.Write( entity.Collision.Size.Y );

                // Write EnemyRespawnGroups:
                context.Write( entity.respawnGroups.Count );

                foreach( var group in entity.respawnGroups )
                {
                    context.Write( group.GetType().GetTypeName() );
                    group.Serialize( context );
                }
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
            public override void Deserialize( EnemySpawnPoint entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );

                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

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

                // Read respawn groups:
                int groupCount = context.ReadInt32();

                entity.respawnGroups.Clear();
                entity.respawnGroups.Capacity = groupCount;

                for( int i = 0; i < groupCount; ++i )
                {
                    IEnemyRespawnGroup group = null;

                    if( version > 1 )
                    {
                        string typeName = context.ReadString();
                        Type type = Type.GetType( typeName );
                        group = (IEnemyRespawnGroup)Activator.CreateInstance( type );
                    }
                    else
                    {
                        group = new EnemyRespawnGroup();                        
                    }

                    group.Deserialize( context );

                    try {
                        group.Create( entity, serviceProvider.EntityTemplateManager );
                    }
                    catch( Exception exc )
                    {
                        serviceProvider.Log.WriteLine();
                        serviceProvider.Log.WriteLine( Atom.Diagnostics.LogSeverities.Error, exc.ToString() );
                        serviceProvider.Log.WriteLine();
                    }

                    entity.respawnGroups.Add( group );
                }
            }
        }

        #endregion
    }
}
