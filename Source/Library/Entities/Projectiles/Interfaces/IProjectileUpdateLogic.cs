// <copyright file="IProjectileUpdateLogic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.IProjectileUpdateLogic interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Projectiles
{    
    /// <summary>
    /// Provides a mechanism that updates a <see cref="Projectile"/>.
    /// </summary>
    public interface IProjectileUpdateLogic : IEntityUpdateLogic<Projectile>
    {
    }
}
