// <copyright file="RespawnableEntitySpawn.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.RespawnableEntitySpawn class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using System.Diagnostics;
    using Atom.Math;
    using Zelda.Entities.Components;
    using Zelda.Saving;

    /// <summary>
    /// Defines an <see cref="EntitySpawn"/> that allows the spawning and respawning 
    /// of a single <see cref="Respawnable"/> <see cref="ZeldaEntity"/>.
    /// </summary>
    public class RespawnableEntitySpawn : EntitySpawn
    {
        /// <summary>
        /// Gets the ZeldaEntity that gets spawned by this RespawnableEntitySpawn.
        /// </summary>
        public override ZeldaEntity Entity
        {
            get
            {
                return base.Entity;
            }

            protected set
            {
                this.isInitialSpawn = true;
                base.Entity         = value;
            }
        }

        /// <summary>
        /// Fires up the respawning process.
        /// </summary>
        public void StartRespawn()
        {
            Debug.Assert( respawnable != null );

            this.isRespawning    = true;
            this.respawnTimeLeft = respawnable.RespawnTime;
        }

        /// <summary>
        /// Updates this RespawnableEntitySpawn.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( isRespawning )
            {
                if( respawnTimeLeft > 0.0 )
                {
                    respawnTimeLeft -= updateContext.FrameTime;
                }
                else
                {
                    if( Respawn() )
                    {
                        isRespawning   = false;
                        isInitialSpawn = false;
                    }
                    else
                    {
                        // try again later:
                        respawnTimeLeft = 25.0f;
                    }
                }
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Tries to respawn the ZeldaEntity.
        /// </summary>
        /// <returns>
        /// true if the entity has been respawned;
        /// otherwise false.
        /// </returns>
        private bool Respawn()
        {
            if( respawnable == null )
                return false;

            if( respawnable.CanRespawnAt( this.Scene, this.Transform.Position, this.FloorNumber, this.isInitialSpawn ) )
            {
                respawnable.Respawn( this.Scene, this.Transform.Position, this.FloorNumber );
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Setups the given ZeldaEntity for usage in this EntitySpawn.
        /// </summary>
        /// <param name="entity">
        /// The related ZeldaEntity.
        /// </param>
        protected override void RegisterEntity( ZeldaEntity entity )
        {
            this.respawnable = entity.Components.Get<Respawnable>();
            if( this.respawnable == null )
                throw new Atom.Components.ComponentNotFoundException( typeof( Respawnable ) );

            base.RegisterEntity( entity );

            // Hock events:
            this.respawnable.RespawnNeeded += this.OnEntityRespawnNeeded;
        }

        /// <summary>
        /// Unregisters the given ZeldaEntity from usage in this EntitySpawn.
        /// </summary>
        /// <param name="entity">
        /// The related ZeldaEntity.
        /// </param>
        protected override void UnregisterEntity( ZeldaEntity entity )
        {
            this.respawnable.RespawnNeeded -= this.OnEntityRespawnNeeded;
            this.respawnable = null;

            base.UnregisterEntity( entity );
        }

        /// <summary>
        /// Gets called when the entity wants to be respawned.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnEntityRespawnNeeded( Respawnable sender )
        {
            this.StartRespawn();
        }
        
        /// <summary>
        /// Returns a clone of this RespawnableEntitySpawn.
        /// </summary>
        /// <returns>
        /// The cloned ZeldaEntity.
        /// </returns>
        public override ZeldaEntity Clone()
        {
            var clone = new RespawnableEntitySpawn();
            clone.Setup( serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Specifies whether the ZeldaEntity is currently respawning.
        /// </summary>
        private bool isRespawning = true;

        /// <summary>
        /// States the time (in seconds) left until the ZeldaEntity respawns.
        /// </summary>
        private float respawnTimeLeft;

        /// <summary>
        /// States whether the current respawn is the very first spawn.
        /// </summary>
        private bool isInitialSpawn = true;

        /// <summary>
        /// Identifies the <see cref="Respawnable"/> component of the ZeldaEntity.
        /// </summary>
        private Respawnable respawnable;

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="RespawnableEntitySpawn"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<RespawnableEntitySpawn>
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
            public override void Serialize( RespawnableEntitySpawn entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Data:
                context.Write( entity.Name );
                context.Write( entity.TemplateName ?? string.Empty );

                context.Write( entity.Transform.Position );
                context.Write( (int)entity.Transform.Direction );
                context.Write( entity.FloorNumber );
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
            public override void Deserialize( RespawnableEntitySpawn entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );
                context.ReadDefaultHeader( this.GetType() );

                // Read
                entity.Name         = context.ReadString();
                entity.TemplateName = context.ReadString();

                entity.Transform.Position  = context.ReadVector2();
                entity.Transform.Direction = (Direction4)context.ReadInt32();

                entity.FloorNumber = context.ReadInt32();
            }
        }
    }
}