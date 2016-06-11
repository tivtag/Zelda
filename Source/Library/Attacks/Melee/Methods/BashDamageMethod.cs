using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Defines the AttackDamageMethod responsible for calculating the damage done
    /// by the BashSkill.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class BashDamageMethod : AttackDamageMethod
    {
        #region [ Damage TypeInfo ]

        /// <summary>
        /// Stores type information about the damage inflicted by the BashDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalMelee;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Calculates the damage done of the given user on the given target
        /// using the <see cref="BashDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the calculation.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            var userEx = (ExtendedStatable)user;

            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            // Did he parry?
            if( target.TryParry( user ) )
                return AttackDamageResult.CreateParried( DamageTypeInfo );

            int damage = rand.RandomRange( userEx.DamageMeleeNormalizedMin, userEx.DamageMeleeNormalizedMax );
            
            // Fixed Modifiers:
            damage += this.fixedValue; // 1. Fixed increase
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );
                        
            // Multipliers:
            damage = (int)(damage * this.multiplier); // 2. Multiplicative increase
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );

            // 3. Increased crit chance
            bool isCrit = user.TryCrit( target, increasedCrit );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);

            // Mitigate damage
            damage = (int)(damage * userEx.GetPhysicalMitigationOf( target ));

            if( isCrit )
            {
                user.OnMeleeCrit();
                target.OnGotMeleeCrit();
            }
            else
            {
                user.OnMeleeHit();
                target.OnGotMeleeHit();
            }

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Sets the parameters of this BashDamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The damage multiplier provided by Bash.
        /// </param>
        /// <param name="fixedValue">
        /// The fixed damage increase provided by Bash.
        /// </param>
        /// <param name="increasedCrit">
        /// The crit chance increase provided by Improved Bash.
        /// </param>
        public void SetValues( float multiplier, int fixedValue, float increasedCrit )
        {
            this.multiplier    = multiplier;
            this.fixedValue    = fixedValue;
            this.increasedCrit = increasedCrit;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The damage multiplier provided by Bash.
        /// </summary>
        private float multiplier = 1.0f;

        /// <summary>
        /// The fixed damage increase provided by Bash.
        /// </summary>
        private int fixedValue;

        /// <summary>
        /// The crit chance increase provided by Improved Bash.
        /// </summary>
        private float increasedCrit;

        #endregion
    }
}
