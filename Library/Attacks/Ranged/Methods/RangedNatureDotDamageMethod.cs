using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Defines an AttackDamageMethod that is used
    /// to calculate the damage done by ranged nature damage over time attack.
    /// (aka. Poisoned Shot)
    /// </summary>
    internal sealed class RangedNatureDotDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the FireBombDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.Create(
            DamageSchool.Magical,
            DamageSource.Ranged,
            ElementalSchool.Nature,
            SpecialDamageType.DamagerOverTime | SpecialDamageType.Poison
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
            var player = (ExtendedStatable)user;
            if( target.Resistances.TryResist( player, ElementalSchool.Nature ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );

            int damage = rand.RandomRange( player.DamageRangedNormalizedMin, player.DamageRangedNormalizedMax );

            // Apply fixed modifiers:
            damage = player.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
            damage = player.DamageDone.WithSpecial.ApplyFixed( damage, SpecialDamageType.DamagerOverTime );
            damage = player.DamageDone.WithSpecial.ApplyFixed( damage, SpecialDamageType.Poison );

            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = (int)(damage * multiplier);
            damage = player.DamageDone.Apply( damage, target, DamageTypeInfo );
            damage = player.DamageDone.WithSpecial.Apply( damage, SpecialDamageType.DamagerOverTime );
            damage = player.DamageDone.WithSpecial.Apply( damage, SpecialDamageType.Poison );

            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );
     
            return AttackDamageResult.CreateDamageOverTime( damage, DamageTypeInfo );
        }

        /// <summary>
        /// Sets the values of this RangedNatureDotDamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The multiplier applied to the damage of the attack.
        /// </param>
        public void SetValues( float multiplier )
        {
            this.multiplier = multiplier;
        }

        /// <summary>
        /// The multiplier applied to the damage of the attack.
        /// </summary>
        private float multiplier = 1.0f;
    }
}
