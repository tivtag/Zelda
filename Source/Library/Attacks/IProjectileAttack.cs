// <copyright file="IProjectileAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.IProjectileAttack interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks
{
    using Atom;
    using Zelda.Entities.Projectiles;

    /// <summary>
    /// Represents an attack that fires <see cref="Projectile"/>s.
    /// </summary>
    public interface IProjectileAttack
    {
        /// <summary>
        /// Fired when a Projectile has been fired by this IProjectileAttack.
        /// </summary>
        event RelaxedEventHandler<Projectile> ProjectileFired;

        /// <summary>
        /// Gets the <see cref="ProjectileSettings"/> that control the
        /// Projectiles fired by this IProjectileAttack.
        /// </summary>
        ProjectileSettings Settings
        {
            get;
        }
            
        /// <summary>
        /// Gets or sets the ProjectileHitSettings that is used for all Projectiles
        /// fired by this IProjectileAttack.
        /// </summary>        
        ProjectileHitSettings HitSettings
        {
            get;
            set;
        }
    }
}
