// <copyright file="DespawnAfterAnimationEndedBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.DespawnAfterAnimationEndedBehaviour class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours
{
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Entities.Drawing;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines an IEntityBehaviour that despawns the entity that it controls
    /// after a fixed amount of time.
    /// </summary>
    /// <seealso cref="IAnimatedDrawDataAndStrategy"/>
    public sealed class DespawnAfterAnimationEndedBehaviour : IEntityBehaviour
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
        /// Gets or sets the time the entity is fading out before beeing removed
        /// from the scene while despawning.
        /// </summary>
        public float FadeOutTime
        {
            get
            {
                return this.fadeOutTime;
            }

            set
            {
                this.fadeOutTime = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DespawnAfterAnimationEndedBehaviour class.
        /// </summary>
        internal DespawnAfterAnimationEndedBehaviour( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the DespawnAfterAnimationEndedBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be despawned.
        /// </param>
        public DespawnAfterAnimationEndedBehaviour( ZeldaEntity entity )
        {
            this.entity = entity;
        }

        /// <summary>
        /// Updates this DespawnAfterAnimationEndedBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            // no op.
            //
            // Could implement for watching of the CurrentAnimation here.
            //
        }

        /// <summary>
        /// Despawn the entity that is controlled by this DespawnAfterAnimationEndedBehaviour.
        /// </summary>
        private void DespawnEntity()
        {
            if( !this.isDespawning && this.IsActive )
            {
                SpawnHelper.Despawn( this.entity, this.fadeOutTime );
                this.isDespawning = true;
            }
        }

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;

            var animatedDrawStrategy = this.entity.DrawDataAndStrategy as IAnimatedDrawDataAndStrategy;

            if( animatedDrawStrategy != null )
            {
                this.animation = animatedDrawStrategy.CurrentAnimation;

                if( animation != null )
                {
                    animation.ReachedEnd += this.OnAnimation_ReachedEnd;
                }
            }

            this.IsActive = true;
        }

        /// <summary>
        /// Called when the current hooked-up animation has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnAnimation_ReachedEnd( SpriteAnimation sender )
        {
            this.DespawnEntity();
            this.Leave();
        }

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Leave()
        {
            if( !this.IsActive )
                return;

            this.IsActive = false;
            this.Reset();
        }

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        public void Reset()
        {
            if( this.animation != null )
            {
                this.animation.ReachedEnd -= this.OnAnimation_ReachedEnd;
                this.animation = null;
            }

            this.isDespawning = false;
        }

        /// <summary>
        /// Returns a clone of this DespawnAfterAnimationEndedBehaviour.
        /// </summary>
        /// <param name="newOwner">
        /// The ZeldaEntity that should be controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            return new DespawnAfterAnimationEndedBehaviour( newOwner );
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
            const int CurrentVersion = 3;
            context.Write( CurrentVersion );

            context.Write( this.fadeOutTime );
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
            const int CurrentVersion = 3;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 2, CurrentVersion, this.GetType() );

            this.fadeOutTime = context.ReadSingle();

            if( version <= 2 )
            {
                /* this.Duration = */ context.ReadFloatRange();
            }
        }

        /// <summary>
        /// The animation that currently is followed.
        /// </summary>
        private SpriteAnimation animation;

        /// <summary>
        /// States whether this DespawnAfterAnimationEndedBehaviour is despawning the entity currently.
        /// </summary>
        private bool isDespawning;
        
        /// <summary>
        /// The time it should take for the entity to fade out when despawning.
        /// </summary>
        private float fadeOutTime = SpawnConstants.DespawnFadeOutTime;

        /// <summary>
        /// The ZeldaEntity that should be despawned by this DespawnAfterAnimationEndedBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
