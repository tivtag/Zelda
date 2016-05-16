using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Calculates the damage of a LightArrow attack of the Player.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class LightArrowDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the LightArrowDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
            DamageSource.Ranged,
            ElementalSchool.Light 
        );

        #region [ Methods ]

        /// <summary>
        /// Calculates the damage done of the given user on the given target
        /// using the <see cref="LightArrowDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the calculation.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            // Did he resist?
            var player = (ExtendedStatable)user;
            if( target.Resistances.TryResist( player, ElementalSchool.Light ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );
            
            int damage = rand.RandomRange( player.DamageRangedNormalizedMin, player.DamageRangedNormalizedMax );

            // Apply modifiers:
            damage = this.ApplyFixedModifiers( damage, player, target );
            damage = this.ApplyMultipliers( damage, player, target );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierRanged);
            
            // 3. Magical attacks pierce through armor!
            if( isCrit )
            {
                user.OnRangedCrit();
                target.OnGotRangedCrit();
            }
            else
            {
                user.OnRangedHit();
                target.OnGotRangedHit();
            }

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Applies the fixed damage modifiers to the input damage.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="user">
        /// The user of the LightArrow.
        /// </param>
        /// <param name="target">
        /// The target of the LightArrow.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        private int ApplyFixedModifiers( int damage, ExtendedStatable user, Statable target )
        {
            damage += this.fixedValue; // 1. Fixed increase.
            damage += user.SpellPower.GetDamage( ElementalSchool.Light );

            damage = user.DamageDone.WithSchool.ApplyFixedMagical( damage );
            damage = user.DamageDone.WithSource.ApplyFixedRanged( damage );
            damage = user.DamageDone.AgainstRace.ApplyFixed( damage, target.Race );

            damage = target.DamageTaken.FromSchool.ApplyFixedMagical( damage );
            damage = target.DamageTaken.FromSource.ApplyFixedRanged( damage );

            return damage;
        }

        /// <summary>
        /// Applies the damage multipliers to the input damage.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="user">
        /// The user of the LightArrow.
        /// </param>
        /// <param name="target">
        /// The target of the LightArrow.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        private int ApplyMultipliers( int damage, ExtendedStatable user, Statable target )
        {
            damage = (int)(damage * multiplier); // 2. Multiplicative increase.

            damage = user.DamageDone.WithSchool.ApplyMagical( damage );
            damage = user.DamageDone.WithSource.ApplyRanged( damage );
            damage = user.DamageDone.WithElement.Apply( ElementalSchool.Light, damage );
            damage = user.DamageDone.AgainstRace.Apply( damage, target.Race );

            damage = target.DamageTaken.FromSchool.ApplyMagical( damage );
            damage = target.DamageTaken.FromSource.ApplyRanged( damage );
            damage = target.DamageTaken.FromElement.Apply( ElementalSchool.Light, damage );

            return damage;
        }

        /// <summary>
        /// Sets the parameters of this LightArrowDamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The damage multiplier value.
        /// </param>
        /// <param name="fixedValue">
        /// The fixed damage increase.
        /// </param>
        public void SetValues( float multiplier, int fixedValue )
        {
            this.multiplier = multiplier;
            this.fixedValue = fixedValue;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The damage multiplier applied by this LightArrowDamageMethod.
        /// </summary>
        private float multiplier = 1.0f;

        /// <summary>
        /// The fixed damage increase applied by this LightArrowDamageMethod.
        /// </summary>
        private int fixedValue;

        #endregion
    }
}
