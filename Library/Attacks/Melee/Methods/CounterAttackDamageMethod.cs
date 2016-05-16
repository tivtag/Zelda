using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Defines the AttackDamageMethod responsible for calculating the damage done
    /// by the CounterAttackSkill.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class CounterAttackDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the CounterAttackDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalMelee;

        /// <summary>
        /// Calculates the damage done of the given user on the given target
        /// using the <see cref="CounterAttackDamageMethod"/>.
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

            // 1. Counter Attack is unparryable.
            int damage = rand.RandomRange( userEx.DamageMeleeNormalizedMin, userEx.DamageMeleeNormalizedMax );

            // Fixed Modifiers:
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );

            // Multipliers:
            damage = (int)(damage * this.multiplier); // 2. Multiplicative increase
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );
            
            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);
                        
            // Mitigate damage:
            damage = (int)(damage * userEx.GetPhysicalMitigationOf( target ));

            // 3. And can't be blocked.
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
        /// Sets the parameters of this CounterAttackDamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The damage multiplier provided by Counter Attack.
        /// </param>
        public void SetValues( float multiplier )
        {
            this.multiplier    = multiplier;
        }

        /// <summary>
        /// The damage multiplier provided by Counter Attack.
        /// </summary>
        private float multiplier = 1.0f;
    }
}
