// <copyright file="RemoveAfterAnimationEndedNtimesBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RemoveAfterAnimationEndedNtimesBehaviour class.
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
    public sealed class RemoveAfterAnimationEndedNtimesBehaviour : IEntityBehaviour
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
        /// Gets or sets the number of times the animation has to end
        /// before the entity is removed from its current scene.
        /// </summary>
        public int TimesAnimationHasToEnded
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the RemoveAfterAnimationEndedNtimesBehaviour class.
        /// </summary>
        internal RemoveAfterAnimationEndedNtimesBehaviour()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RemoveAfterAnimationEndedNtimesBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be despawned.
        /// </param>
        public RemoveAfterAnimationEndedNtimesBehaviour( ZeldaEntity entity )
        {
            this.entity = entity;
        }

        /// <summary>
        /// Updates this RemoveAfterAnimationEndedNtimesBehaviour.
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
            this.timesAnimationEnded = 0;
        }

        /// <summary>
        /// Called when the current hooked-up animation has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnAnimation_ReachedEnd( SpriteAnimation sender )
        {
            ++this.timesAnimationEnded;
            this.animation.Reset();

            if( this.timesAnimationEnded >= this.TimesAnimationHasToEnded )
            {
                this.entity.RemoveFromScene();
                this.Leave();
            }
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

            this.timesAnimationEnded = 0;
        }

        /// <summary>
        /// Returns a clone of this RemoveAfterAnimationEndedNtimesBehaviour.
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
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.TimesAnimationHasToEnded );
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

            this.TimesAnimationHasToEnded = context.ReadInt32();
        }

        /// <summary>
        /// The number of times the animation has ended.
        /// </summary>
        private int timesAnimationEnded;

        /// <summary>
        /// The animation that currently is followed.
        /// </summary>
        private SpriteAnimation animation;

        /// <summary>
        /// The ZeldaEntity that should be despawned by this RemoveAfterAnimationEndedNtimesBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
