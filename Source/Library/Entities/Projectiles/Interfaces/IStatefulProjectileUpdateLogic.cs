// <copyright file="IStatefulProjectileUpdateLogic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.IStatefulProjectileUpdateLogic interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Projectiles
{
    /// <summary>
    /// Provides a mechanism that updates a <see cref="Projectile"/>;
    /// each entity requires its own instance of the update logic.
    /// </summary>
    public interface IStatefulProjectileUpdateLogic : IProjectileUpdateLogic, IStatefulEntityUpdateLogic<Projectile>
    {
    }
}
