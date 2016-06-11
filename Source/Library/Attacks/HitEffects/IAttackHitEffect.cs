// <copyright file="IAttackHitEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.IAttackHitEffect interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks
{
    /// <summary>
    /// An effect that is applied to an object which got hit by an attack.
    /// </summary>
    public interface IAttackHitEffect
    {
        /// <summary>
        /// Called when the effect is to be applied.
        /// </summary>
        /// <param name="user">
        /// The user of the attack.
        /// </param>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        void OnHit( Zelda.Status.Statable user, Zelda.Status.Statable target );
    }
}
