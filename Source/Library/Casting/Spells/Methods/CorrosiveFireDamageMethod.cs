// <copyright file="CorrosiveFireDamageMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.CorrosiveFireDamageMethod class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting.Spells
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Defines an AttackDamageMethod used to calculate the
    /// damage done by the CorrosiveFire damage over time effect.
    /// </summary>
    internal sealed class CorrosiveFireDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the CorrosiveFireDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.Create(
            DamageSchool.Magical,
            DamageSource.Spell,
            ElementalSchool.Fire,
            SpecialDamageType.DamagerOverTime
        );

        /// <summary>
        /// Calculates the damage done of the specified user on the specified target
        /// using this AttackDamageMethod.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the attack.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            var userEx = (ExtendedStatable)user;
            if( target.Resistances.TryResist( userEx, ElementalSchool.Fire ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );

            int damage = userEx.SpellPower.GetDamage( ElementalSchool.Fire );

            // Apply fixed modifiers:
            damage = userEx.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
            damage = userEx.DamageDone.WithSpecial.ApplyFixed( damage, SpecialDamageType.DamagerOverTime );
            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = (int)(damage * this.GetMultiplier()); // 1. Damage is reduced.
            damage = userEx.DamageDone.Apply( damage, target, DamageTypeInfo );
            damage = userEx.DamageDone.WithSpecial.Apply( damage, SpecialDamageType.DamagerOverTime );
            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );

            return AttackDamageResult.CreateDamageOverTime( damage, DamageTypeInfo );
        }

        /// <summary>
        /// Gets the multplier value used for the next Corrosive Fire.
        /// </summary>
        /// <returns>
        /// The damage multiplier for the next Corrosive Fire dot.
        /// </returns>
        private float GetMultiplier()
        {
            return this.rand.RandomRange( this.minimumMultiplier, this.maximumMultiplier );
        }

        /// <summary>
        /// Sets the values of this CorrosiveFireDamageMethod.
        /// </summary>
        /// <param name="minimumMultiplier">
        /// The minimum multiplier applied to the damage of the attack.
        /// </param>
        /// <param name="maximumMultiplier">
        /// The maximum multiplier applied to the damage of the attack.
        /// </param>
        public void SetValues( float minimumMultiplier, float maximumMultiplier )
        {
            this.minimumMultiplier = minimumMultiplier;
            this.maximumMultiplier = maximumMultiplier;
        }

        /// <summary>
        /// The multiplier applied to the damage of the Corrosive Fire.
        /// </summary>
        private float minimumMultiplier, maximumMultiplier;
    }
}
