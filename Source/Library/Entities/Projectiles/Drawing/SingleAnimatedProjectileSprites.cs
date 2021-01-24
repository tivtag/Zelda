// <copyright file="SingleAnimatedProjectileSprites.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Drawing.SingleAnimatedProjectileSprites class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using System;
    using Atom.Collections.Pooling;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Provides access to the Sprites used to visualize a Projectile
    /// by using a single AnimatedSprite for all Projectile directions.
    /// This is a sealed class.
    /// </summary>
    public sealed class SingleAnimatedProjectileSprites : IProjectileSprites
    {
        /// <summary>
        /// Initializes a new instance of the SingleAnimatedProjectileSprites class.
        /// </summary>
        /// <param name="spriteTemplate">
        /// The AnimatedSprite used as a template for the SpriteAnimations.
        /// </param>
        public SingleAnimatedProjectileSprites( AnimatedSprite spriteTemplate )
        {
            if( spriteTemplate == null )
                throw new ArgumentNullException( "spriteTemplate" );

            this.size = spriteTemplate.Size;
            this.spriteTemplate = spriteTemplate;

            this.pool = new WrappingPool<ISprite>(
                5,
                () => {
                    return new PooledSpriteWrapper( this.spriteTemplate.CreateInstance() );
                }
            );

            this.pool.AddNodes( 5 );
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
            var poolNode = this.pool.Get();
            var wrapper = poolNode.Item;
            return (ISprite)wrapper;
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
            return this.size;
        }

        /// <summary>
        /// Returns the given ISprite to this IProjectileSprites instance.
        /// </summary>
        /// <param name="sprite">
        /// The sprite to return. Must have been requested with <see cref="Get"/>.
        /// </param>
        public void Return( ISprite sprite )
        {
            var wrapper = (PooledSpriteWrapper)sprite;
            var node = wrapper.PoolNode;
            this.pool.Return( node );
        }

        /// <summary>
        /// The AnimatedSprite used as a template for the Projectiles.
        /// </summary>
        private readonly AnimatedSprite spriteTemplate;

        /// <summary>
        /// The cached size of the spriteTemplate.
        /// </summary>
        private readonly Point2 size;

        /// <summary>
        /// The pool from which sprites are taken.
        /// </summary>
        private readonly WrappingPool<ISprite> pool;
    }
}
