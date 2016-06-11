using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Calculates the damage of a ranged attack.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class RangedDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the RangedDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalRanged;

        /// <summary>
        /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
        /// using this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The calculated result.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            int damage = rand.RandomRange( user.DamageRangedMin, user.DamageRangedMax );

            // Fixed Modifiers:
            damage = user.DamageDone.ApplyFixedPhysicalRanged( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalRanged( damage );

            // Multipliers:
            damage = user.DamageDone.ApplyPhysicalRanged( damage );
            damage = target.DamageTaken.ApplyPhysicalRanged( damage );
            
            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierRanged);

            // Handle Block.
            bool isBlocked = target.TryBlock( user, rand );
            if( isBlocked )
            {
                damage = target.HandleBlock( damage, user, rand );
            }

            // Mitigate damage
            damage = (int)(damage * user.GetPhysicalMitigationOf( target ));

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

            return AttackDamageResult.Create( damage, isCrit, isBlocked, DamageTypeInfo );
        }
    }
}
