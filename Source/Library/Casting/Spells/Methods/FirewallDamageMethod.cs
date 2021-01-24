// <copyright file="FirewallDamageMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.FirewallDamageMethod class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting.Spells
{
    using Atom.Math;
    using Zelda.Status;
    using Zelda.Status.Damage;
    using Zelda.Attacks;

    /// <summary>
    /// Defines an AttackDamageMethod that calculates the damage dealt by the Firewall spell.
    /// </summary>
    internal sealed class FirepillarDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the FirewallDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
            DamageSource.Spell,
            ElementalSchool.Fire
        );

        /// <summary>
        /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
        /// using this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The calculated result.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            var userEx = (ExtendedStatable)user;

            // Did it resist?
            if( target.Resistances.TryResist( userEx, ElementalSchool.Fire ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );

            int damage = userEx.SpellPower.GetDamage( ElementalSchool.Fire );

            // Apply fixed modifiers:
            damage = userEx.DamageDone.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire );

            // Apply multipliers:
            damage = (int)(damage * this.GetDamageModifier());
            damage = userEx.DamageDone.ApplyMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyMagicalSpell( damage, ElementalSchool.Fire );

            // Critical:
            bool isCrit = user.TryCrit( target, this.extraCritChance );
            if( isCrit )
                damage = (int)(damage * user.CritModifierSpell);

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Gets the damage multiplier of the Firewall.
        /// </summary>
        /// <returns>
        /// The current damage modification value of the FireVortex.
        /// </returns>
        private float GetDamageModifier()
        {
            return damageMultiplierRange.GetRandomValue( this.rand );
        }

        /// <summary>
        /// Sets the value sof this AttackDamageMethod.
        /// </summary>
        /// <param name="damageMultiplierRange">
        /// The multiplier that is applied to the damage done.
        /// </param>
        /// <param name="extraCritChance">
        /// The increased crit chance of Firewall.
        /// </param>
        public void SetValues( FloatRange damageMultiplierRange, float extraCritChance )
        {
            this.damageMultiplierRange = damageMultiplierRange;
            this.extraCritChance = extraCritChance;
        }

        /// <summary>
        /// The multiplier that is applied to the damage done.
        /// </summary>
        private FloatRange damageMultiplierRange;

        /// <summary>
        /// The increased crit chance of Firewall.
        /// </summary>
        private float extraCritChance;
    }
}
