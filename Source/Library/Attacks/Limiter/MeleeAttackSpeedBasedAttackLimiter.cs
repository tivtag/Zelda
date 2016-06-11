// <copyright file="MeleeAttackSpeedBasedAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.MeleeAttackSpeedBasedAttackLimiter class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Limiter
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// Limits the usage of an Attack for <see cref="Statable.AttackSpeedMelee"/> seconds
    /// after the Attack has fired.
    /// </summary>
    public sealed class MeleeAttackSpeedBasedAttackLimiter : TimedAttackLimiter
    {
        /// <summary>
        /// Initializes a new instance of the MeleeAttackSpeedBasedAttackLimiter clas.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that owns the attack
        /// that is limited by the new MeleeAttackSpeedBasedAttackLimiter.
        /// </param>
        public MeleeAttackSpeedBasedAttackLimiter( Statable statable )
        {
            this.statable = statable;
        }

        /// <summary>
        /// Gets the time it takes until the Attack can go off again.
        /// </summary>
        public override float AttackDelay
        {
            get
            {
                return this.statable.AttackSpeedMelee;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public override IAttackLimiter Clone()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The statable component of the entity that owns the attack
        /// that is limited by this MeleeAttackSpeedBasedAttackLimiter.
        /// </summary>
        private readonly Statable statable;
    }
}
