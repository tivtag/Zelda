// <copyright file="ProjectileSprites.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Drawing.ProjectileSprites class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Provides access to the Sprites used to visualize a Projectile.
    /// This is a sealed class.
    /// </summary>
    public sealed class ProjectileSprites : IProjectileSprites
    {
        /// <summary>
        /// Initializes a new instance of the ProjectileSprites class.
        /// </summary>
        /// <param name="spriteLeft">
        /// The Sprite used to visualize a Projectile heading left.
        /// </param>
        /// <param name="spriteRight">
        /// The Sprite used to visualize a Projectile heading right.
        /// </param>
        /// <param name="spriteDown">
        /// The Sprite used to visualize a Projectile heading Down.
        /// </param>
        /// <param name="spriteUp">
        /// The Sprite used to visualize a Projectile heading up.
        /// </param>
        public ProjectileSprites(
            Sprite spriteLeft,
            Sprite spriteRight,
            Sprite spriteDown,
            Sprite spriteUp )
        {
            if( spriteLeft == null )
                throw new ArgumentNullException( "spriteLeft" );
            if( spriteRight == null )
                throw new ArgumentNullException( "spriteRight" );
            if( spriteDown == null )
                throw new ArgumentNullException( "spriteDown" );
            if( spriteUp == null )
                throw new ArgumentNullException( "spriteUp" );

            this.spriteLeft = spriteLeft;
            this.spriteRight = spriteRight;
            this.spriteDown = spriteDown;
            this.spriteUp = spriteUp;
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
                    return this.spriteLeft;

                case Direction4.Right:
                    return this.spriteRight;

                case Direction4.Up:
                    return this.spriteUp;

                case Direction4.Down:
                    return this.spriteDown;

                default:
                    return null;
            }
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
                    return this.spriteLeft.Size;

                case Direction4.Right:
                    return this.spriteRight.Size;

                case Direction4.Up:
                    return this.spriteUp.Size;

                case Direction4.Down:
                    return this.spriteDown.Size;

                default:
                case Direction4.None:
                    return Point2.Zero;
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
        /// The Sprite used to visualize the Projectiles.
        /// </summary>
        private readonly Sprite spriteLeft, spriteRight, spriteUp, spriteDown;
    }
}
