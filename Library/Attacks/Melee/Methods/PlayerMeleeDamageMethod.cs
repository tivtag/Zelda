using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Calculates the damage of a melee attack of the Player.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class PlayerMeleeDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the PlayerMeleeDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalMelee;

        /// <summary>
        /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
        /// using this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The calculated result.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            var player = (ExtendedStatable)user;
            
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            // Did he parry?
            if( target.TryParry( user ) )
                return AttackDamageResult.CreateParried( DamageTypeInfo );

            int damage = rand.RandomRange( player.DamageMeleeNormalizedMin, player.DamageMeleeNormalizedMax );

            // Fixed Modifiers:
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );

            // Multipliers:
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );
            
            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);

            // Handle Block.
            bool isBlocked = target.TryBlock( user, rand );
            if( isBlocked )
            {
                damage = target.HandleBlock( damage, user, rand );
            }
            
            // Mitigate damage
            damage = (int)(damage * player.GetPhysicalMitigationOf( target ));

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

            return AttackDamageResult.Create( damage, isCrit, isBlocked, DamageTypeInfo );
        }
    }
}
