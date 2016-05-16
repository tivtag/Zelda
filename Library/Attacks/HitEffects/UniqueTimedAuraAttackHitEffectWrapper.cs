// <copyright file="UniqueTimedAuraAttackHitEffectWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.UniqueTimedAuraAttackHitEffectWrapper class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.HitEffects
{
    using System;
    using System.Diagnostics.Contracts;
    using Zelda.Status;

    /// <summary>
    /// Defines an IAttackHitEffect that wraps around another IAttackHitEffect;
    /// redirecting calls to IAttackHitEffect.OnHit to the wrapped IAttackEffect
    /// if and only if a specific TimedAura isn't yet present on the target statable.
    /// If this is not the case the duration of the TimedAura is reset.
    /// This class can't be inherited.
    /// </summary>
    public sealed class UniqueTimedAuraAttackHitEffectWrapper : IAttackHitEffect
    {
        /// <summary>
        /// Initializes a new instance of the UniqueTimedAuraAttackHitEffectWrapper class.
        /// </summary>
        /// <param name="aura">
        /// The aura that may not be present on the target aura for the wrappedEffect to be executed.
        /// </param>
        /// <param name="wrappedEffect">
        /// The IAttackHitEffect the new UniqueTimedAuraAttackHitEffectWrapper wraps around.
        /// </param>
        public UniqueTimedAuraAttackHitEffectWrapper( TimedAura aura, IAttackHitEffect wrappedEffect )
        {
            Contract.Requires<ArgumentNullException>( aura != null );
            Contract.Requires<ArgumentNullException>( wrappedEffect != null );

            this.aura = aura;
            this.wrappedEffect = wrappedEffect;
        }

        /// <summary>
        /// Called when the effect is to be applied.
        /// </summary>
        /// <param name="user">
        /// The user of the attack.
        /// </param>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        public void OnHit( Statable user, Statable target )
        {
            var aura = target.AuraList.FindTimedAura( this.aura.Name );

            if( aura != null )
            {
                aura.ResetDuration();
            }
            else
            {
                this.wrappedEffect.OnHit( user, target );
            }
        }

        /// <summary>
        /// The aura that may not be present on the target aura for the wrappedEffect to be execute
        /// </summary>
        private readonly TimedAura aura;

        /// <summary>
        /// The IAttackHitEffect this UniqueTimedAuraAttackHitEffectWrapper wraps around.
        /// </summary>
        private readonly IAttackHitEffect wrappedEffect;
    }
}
