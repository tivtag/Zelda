// <copyright file="IProjectileSprites.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Drawing.IProjectileSprites interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Provides access to the sprites used to visualize a Projectile.
    /// </summary>
    public interface IProjectileSprites
    {
        /// <summary>
        /// Gets the ISprite associated with the given <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">
        /// The direction the projectile is heading.
        /// </param>
        /// <returns>
        /// The Sprite associated with the given direction.
        /// </returns>
        ISprite Get( Direction4 direction );

        /// <summary>
        /// Returns the given ISprite to this IProjectileSprites instance.
        /// </summary>
        /// <param name="sprite">
        /// The sprite to return. Must have been requested with <see cref="Get"/>.
        /// </param>
        void Return( ISprite sprite );

        /// <summary>
        /// Gets the size of the ISprite for the given Direction.
        /// </summary>
        /// <param name="direction">
        /// The direction of the projectile.
        /// </param>
        /// <returns>
        /// The size in pixels.
        /// </returns>
        Point2 GetSize( Direction4 direction );
    }
}
