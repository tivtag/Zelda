// <copyright file="RemoveAfterAnimationEndedBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RemoveAfterAnimationEndedBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using Atom.Xna;
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Defines an IEntityBehaviour that removes an entity from its current scene
    /// afters its animation has ended and reset N times.
    /// </summary>
    /// <seealso cref="IAnimatedDrawDataAndStrategy"/>
    public sealed class RemoveAfterAnimationEndedBehaviour : IEntityBehaviour
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
        /// Initializes a new instance of the RemoveAfterAnimationEndedBehaviour class.
        /// </summary>
        internal RemoveAfterAnimationEndedBehaviour()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RemoveAfterAnimationEndedBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be despawned.
        /// </param>
        public RemoveAfterAnimationEndedBehaviour( ZeldaEntity entity )
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

                if( this.animation != null )
                {
                    this.animation.ReachedEnd += this.OnAnimation_ReachedEnd;
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
            this.entity.RemoveFromScene();
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
        }

        /// <summary>
        /// Returns a clone of this RemoveAfterAnimationEndedBehaviour.
        /// </summary>
        /// <param name="newOwner">
        /// The ZeldaEntity that should be controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            return new RemoveAfterAnimationEndedBehaviour( newOwner );
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
        }

        /// <summary>
        /// The animation that currently is followed.
        /// </summary>
        private SpriteAnimation animation;

        /// <summary>
        /// The ZeldaEntity that should be despawned by this RemoveAfterAnimationEndedBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
