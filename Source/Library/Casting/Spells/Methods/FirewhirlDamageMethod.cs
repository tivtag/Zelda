// <copyright file="FirewhirlDamageMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.FirewhirlDamageMethod class.
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
    /// damage done by the FirewhirlDamageMethod attack.
    /// </summary>
    internal sealed class FirewhirlDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the LightArrowDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
            DamageSource.Spell,
            ElementalSchool.Fire
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
            damage = userEx.DamageDone.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire );

            // Apply multipliers:
            damage = (int)(damage * this.GetMultiplier()); // 1. Damage Increase.
            damage = userEx.DamageDone.ApplyMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyMagicalSpell( damage, ElementalSchool.Fire );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierSpell);

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Gets the multplier value used for the next Firewhirl.
        /// </summary>
        /// <returns>
        /// The damage multiplier for the next Firewhirl.
        /// </returns>
        private float GetMultiplier()
        {
            return this.rand.RandomRange( this.minimumMultiplier, this.maximumMultiplier );
        }

        /// <summary>
        /// Sets the values of this FirewhirlDamageMethod.
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
        /// The multiplier applied to the damage of the attack.
        /// </summary>
        private float minimumMultiplier, maximumMultiplier;
    }
}
