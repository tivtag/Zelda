// <copyright file="ResizeToCurrentSpriteBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.ResizeToCurrentSpriteBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Defines an IEntityBehaviour that despawns the entity that it controls
    /// after a fixed amount of time.
    /// </summary>
    public sealed class ResizeToCurrentSpriteBehaviour : IEntityBehaviour
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
        /// Initializes a new instance of the ResizeToCurrentSpriteBehaviour class.
        /// </summary>
        internal ResizeToCurrentSpriteBehaviour()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResizeToCurrentSpriteBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be despawned.
        /// </param>
        public ResizeToCurrentSpriteBehaviour( ZeldaEntity entity )
        {
            this.entity = entity;
        }

        /// <summary>
        /// Updates this ResizeToCurrentSpriteBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            var animatedStrategy = this.entity.DrawDataAndStrategy as IAnimatedDrawDataAndStrategy;

            if( animatedStrategy != null )
            {
                var currentAnimation = animatedStrategy.CurrentAnimation;

                if( currentAnimation != null )
                {
                    var currentFrame = currentAnimation.Frame;

                    if( currentFrame != null )
                    {
                        this.Set( currentFrame.Offset, currentFrame.Sprite.Size );
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the collision rectangle of the entity.
        /// </summary>
        /// <param name="offset">
        /// The offset from the entity position to the upper left corner of the collision rectangle.
        /// </param>
        /// <param name="size">
        /// The size of the collision rectangle.
        /// </param>
        private void Set( Atom.Math.Vector2 offset, Atom.Math.Vector2 size )
        {
            this.entity.Collision.Set( offset, size );
        }

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;

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
        }

        /// <summary>
        /// Returns a clone of this ResizeToCurrentSpriteBehaviour
        /// </summary>
        /// <param name="newOwner">
        /// The ZeldaEntity that should be controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            return new ResizeToCurrentSpriteBehaviour( newOwner );
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
        /// The ZeldaEntity that should be despawned by this ResizeToCurrentSpriteBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;
    }
}
