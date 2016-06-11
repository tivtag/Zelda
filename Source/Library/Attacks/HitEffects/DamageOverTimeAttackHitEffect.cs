// <copyright file="DamageOverTimeAttackHitEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.DamageOverTimeAttackHitEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Attacks.HitEffects
{
    using System;
    using System.Diagnostics.Contracts;
    using Zelda.Status;
    using Zelda.Status.Auras;

    // TODO: Optimize cloning of the DamageOverTimeAura.

    /// <summary>
    /// An <see cref="IAttackHitEffect"/> that applies a damage over time(DOT)
    /// effect on the target.
    /// This class can't be inherited.
    /// </summary>
    /// <seealso cref="DamageOverTimeAura"/>
    public sealed class DamageOverTimeAttackHitEffect : IAttackHitEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageOverTimeAttackHitEffect"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="aura"/> or <paramref name="method"/> is null.
        /// </exception>
        /// <param name="aura">
        /// The aura that is applied on Hit
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that is used to calculate damage done.
        /// </param>
        public DamageOverTimeAttackHitEffect( DamageOverTimeAura aura, AttackDamageMethod method )
        {
            Contract.Requires<ArgumentNullException>( aura != null );
            Contract.Requires<ArgumentNullException>( method != null );

            this.aura = aura;
            this.method = method;
        }

        /// <summary>
        /// Called when the attack hits.
        /// </summary>
        /// <param name="user">
        /// The user of the attack.
        /// </param>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        public void OnHit( Statable user, Statable target )
        {
            AttackDamageResult results = this.method.GetDamageDone( user, target );
            if( HasAttackMissed( ref results ) )
                return;

            // !!!! rethink following code! !!!!
            // creating new instances mid-game is bad.
            var dot = (DamageOverTimeAura)this.aura.Clone();
            dot.DamageEachTick = results.Damage / aura.TickCount;
            dot.ResetDuration();

            target.AuraList.Add( dot );
        }

        /// <summary>
        /// Gets a value indcating whether the specified attakc has resisted, missed or dodged.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static bool HasAttackMissed( ref AttackDamageResult results )
        {
            return results.AttackReceiveType == AttackReceiveType.Resisted ||
                results.AttackReceiveType == AttackReceiveType.Miss ||
                results.AttackReceiveType == AttackReceiveType.Dodge;
        }

        /// <summary>
        /// The AttackDamageMethod that is used to calculate damage done.
        /// </summary>
        private readonly AttackDamageMethod method;

        /// <summary>
        /// The aura that is applied on Hit
        /// </summary>
        private readonly DamageOverTimeAura aura;
    }
}
