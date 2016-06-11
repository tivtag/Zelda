// <copyright file="ProjectilePiercingChanceMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.ProjectilePiercingChanceMode enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Projectiles
{
    /// <summary>
    /// Enumerates the different modes that define how the
    /// final piercing chance of a Projectile is calculated.
    /// </summary>
    public enum ProjectilePiercingChanceMode
    {
        /// <summary>
        /// The projectile can't pierce its target.
        /// </summary>
        None,

        /// <summary>
        /// The projectile can pierce its target based on the
        /// piercing chance of the entity.
        /// </summary>
        OnlyEntity,

        /// <summary>
        /// The projectile can pierce its target based on the
        /// additional piercing chance.
        /// </summary>
        OnlyAdditional,

        /// <summary>
        /// The projectile can pierce its target based on the
        /// piercing chance of the entity combined with the
        /// additional piercing chance.
        /// </summary>
        Combined
    }
}
