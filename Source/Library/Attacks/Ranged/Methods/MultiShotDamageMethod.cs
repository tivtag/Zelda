using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Defines the AttackDamageMethod for the MultiShotAttack.
    /// Multi Shot does reduced damage compared to a normal ranged attack.
    /// </summary>
    internal sealed class MultiShotDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the MultiShotDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalRanged;

        /// <summary>
        /// Calculates the damage done of the specified user on the specified target
        /// using this AttackDamageMethod.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the attack.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            var player = (ExtendedStatable)user;
            int damage = rand.RandomRange( player.DamageRangedNormalizedMin, player.DamageRangedNormalizedMax );
            
            // Apply fixed modifiers:
            damage = player.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = (int)(damage * Talents.Ranged.MultiShotTalent.DamageReduction); // 1. Multishot damage is reduced.
            damage = player.DamageDone.Apply( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );
            
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
            damage = (int)(damage * player.GetPhysicalMitigationOf( target ));

            // Multi Shot often does 0 damage at the beginning. This fixes this.
            if( damage == 0 )
                damage = 1;

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
