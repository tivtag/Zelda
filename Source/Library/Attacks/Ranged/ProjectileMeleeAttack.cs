// <copyright file="ProjectileMeleeAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.ProjectileMeleeAttack class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Ranged
{
    using Zelda.Entities.Projectiles;
    using Zelda.Attacks.Limiter;

    /// <summary>
    /// Defines the MeleeAttack that is internally used by a <see cref="Projectile"/>
    /// to launch a strike against a hit enemy.
    /// </summary>
    public sealed class ProjectileMeleeAttack : MeleeAttack
    {
        /// <summary>
        /// The time until a Projectile that has pierced a target can hit a new target.
        /// </summary>
        private const float TimeUntilPiercedProjectileCanHitAgain = 0.4f;

        /// <summary>
        /// Gets the <see cref="Projectile"/> that owns this ProjectileMeleeAttack.
        /// </summary>
        public Projectile Projectile
        {
            get
            {
                return this.projectile;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ProjectileMeleeAttack class.
        /// </summary>
        /// <param name="projectile">
        /// The Projectile that owns the new ProjectileMeleeAttack.
        /// </param>
        internal ProjectileMeleeAttack( Projectile projectile )
            : base( null, null )
        {
            this.Limiter = new TimedAttackLimiter( TimeUntilPiercedProjectileCanHitAgain );
            this.projectile = projectile;
        }

        /// <summary>
        /// The Projectile that owns this ProjectileMeleeAttack.
        /// </summary>
        private readonly Projectile projectile;
    }
}
