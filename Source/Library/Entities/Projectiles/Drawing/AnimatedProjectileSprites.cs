// <copyright file="AnimatedProjectileSprites.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Drawing.AnimatedProjectileSprites class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Provides access to the Sprites used to visualize a Projectile.
    /// This is a sealed class.
    /// </summary>
    public sealed class AnimatedProjectileSprites : IProjectileSprites
    {
        /// <summary>
        /// Initializes a new instance of the AnimatedProjectileSprites class.
        /// </summary>
        /// <param name="spriteTemplateLeft">
        /// The AnimatedSprite used as a template for the SpriteAnimations visualizing a Projectile heading left.
        /// </param>
        /// <param name="spriteTemplateRight">
        /// The AnimatedSprite used as a template for the SpriteAnimations visualizing a Projectile heading right.
        /// </param>
        /// <param name="spriteTemplateDown">
        /// The AnimatedSprite used as a template for the SpriteAnimations visualizing a Projectile heading Down.
        /// </param>
        /// <param name="spriteTemplateUp">
        /// The AnimatedSprite used as a template for the SpriteAnimations visualizing a Projectile heading up.
        /// </param>
        public AnimatedProjectileSprites(
            AnimatedSprite spriteTemplateLeft,
            AnimatedSprite spriteTemplateRight,
            AnimatedSprite spriteTemplateDown,
            AnimatedSprite spriteTemplateUp )
        {
            if( spriteTemplateLeft == null )
                throw new ArgumentNullException( "spriteTemplateLeft" );
            if( spriteTemplateRight == null )
                throw new ArgumentNullException( "spriteTemplateRight" );
            if( spriteTemplateDown == null )
                throw new ArgumentNullException( "spriteTemplateDown" );
            if( spriteTemplateUp == null )
                throw new ArgumentNullException( "spriteTemplateUp" );

            this.spriteTemplateLeft = spriteTemplateLeft;
            this.spriteTemplateRight = spriteTemplateRight;
            this.spriteTemplateDown = spriteTemplateDown;
            this.spriteTemplateUp = spriteTemplateUp;
        }

        /// <summary>
        /// Gets the Projectile Sprite associated with the given <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">
        /// The direction the projectile is heading.
        /// </param>
        /// <returns>
        /// The Sprite associated with the given direction.
        /// </returns>
        public ISprite Get( Direction4 direction )
        {
            switch( direction )
            {
                case Direction4.Left:
                    return this.spriteTemplateLeft.CreateInstance();

                case Direction4.Right:
                    return this.spriteTemplateRight.CreateInstance();

                case Direction4.Up:
                    return this.spriteTemplateUp.CreateInstance();

                case Direction4.Down:
                    return this.spriteTemplateDown.CreateInstance();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the given ISprite to this IProjectileSprites instance.
        /// </summary>
        /// <param name="sprite">
        /// The sprite to return. Must have been requested with <see cref="Get"/>.
        /// </param>
        public void Return( ISprite sprite )
        {
        }

        /// <summary>
        /// Gets the size of the ISprite for the given Direction.
        /// </summary>
        /// <param name="direction">
        /// The direction of the projectile.
        /// </param>
        /// <returns>
        /// The size in pixels.
        /// </returns>
        public Point2 GetSize( Direction4 direction )
        {
            switch( direction )
            {
                case Direction4.Left:
                    return this.spriteTemplateLeft.Size;

                case Direction4.Right:
                    return this.spriteTemplateRight.Size;

                case Direction4.Up:
                    return this.spriteTemplateUp.Size;

                case Direction4.Down:
                    return this.spriteTemplateDown.Size;

                default:
                case Direction4.None:
                    return Point2.Zero;
            }
        }

        /// <summary>
        /// The AnimatedSprite used as a template for the Projectiles.
        /// </summary>
        private AnimatedSprite spriteTemplateLeft, spriteTemplateRight, spriteTemplateUp, spriteTemplateDown;
    }
}
