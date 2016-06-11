// <copyright file="IRangedEnemyBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.IRangedEnemyBehaviour interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using Zelda.Attacks.Ranged;

    /// <summary>
    /// An <see cref="IRangedEnemyBehaviour"/> controls the behaviour of a ranged enemy.
    /// </summary>
    public interface IRangedEnemyBehaviour : IEntityBehaviour
    {
        /// <summary>
        /// Gets the <see cref="RangedAttack"/> that controls the attack logic
        /// of the ranged enemy.
        /// </summary>
        RangedAttack RangedAttack
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="RangedEnemyAttackSettings"/> that stores
        /// the ranged attack settings of the ranged enemy.
        /// </summary>
        RangedEnemyAttackSettings AttackSettings
        {
            get;
        }
    }
}
