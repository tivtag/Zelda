// <copyright file="DespawnAfterBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.DespawnAfterBehaviour class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours
{
    using Atom.Math;
    using Zelda.Entities.Spawning;
    using Zelda.Saving;

    /// <summary>
    /// Defines an IEntityBehaviour that despawns the entity that it controls
    /// after a fixed amount of time.
    /// </summary>
    public sealed class DespawnAfterBehaviour : IEntityBehaviour
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IEntityBehaviour"/> is currently active.
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the duration for the entity to despawn after entering this behaviour.
        /// </summary>
        public FloatRange Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the DespawnAfterBehaviour class.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        internal DespawnAfterBehaviour( IRand rand )
        {
            this.rand = rand;
        }

        /// <summary>
        /// Initializes a new instance of the DespawnAfterBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be despawned.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        public DespawnAfterBehaviour( ZeldaEntity entity, IRand rand )
        {
            this.entity = entity;
            this.rand = rand;
        }

        /// <summary>
        /// Updates this DespawnAfterBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            this.timeLeftUntilDespawn -= updateContext.FrameTime;

            if( this.timeLeftUntilDespawn <= 0.0f )
            {
                this.DespawnEntity();
                this.Leave();
            }
        }

        /// <summary>
        /// Despawn the entity that is controlled by this DespawnAfterBehaviour.
        /// </summary>
        private void DespawnEntity()
        {
            SpawnHelper.Despawn( this.entity );
        }

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;

            this.Reset();
            this.IsActive = true;
        }

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Leave()
        {
            this.IsActive = false;
        }

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        public void Reset()
        {
            this.timeLeftUntilDespawn = this.Duration.GetRandomValue( this.rand );
        }

        /// <summary>
        /// Returns a clone of this DespawnAfterBehaviour
        /// </summary>
        /// <param name="newOwner">
        /// The ZeldaEntity that should be controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            return new DespawnAfterBehaviour( newOwner, this.rand ) {
                Duration = this.Duration
            };
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Duration );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Duration = context.ReadFloatRange();
        }

        /// <summary>
        /// The time left until the ZeldaEntity gets despawned.
        /// </summary>
        private float timeLeftUntilDespawn;

        /// <summary>
        /// The ZeldaEntity that should be despawned by this DespawnAfterBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly IRand rand;
    }
}
