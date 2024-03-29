﻿using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Calculates the damage of a normal melee attack. 
    /// This class can't be inherited.
    /// </summary>
    public sealed class DefaultMeleeDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the DefaultMeleeDamageMethod.
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
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            // Did he dodge?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            // Did he parry?
            if( target.TryDodge( user ) )
                return AttackDamageResult.CreateParried( DamageTypeInfo );

            int damage = rand.RandomRange( user.DamageMeleeMin, user.DamageMeleeMax );

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

            // Mitigate damage.
            bool isBlocked = target.TryBlock( user, rand );
            if( isBlocked )
            {
                damage = target.HandleBlock( damage, user, rand );
            }

            damage = (int)(damage * user.GetPhysicalMitigationOf( target ));

            if( damage == 0 )
                damage = 1;
            
            if( isCrit )
            {
                user.OnNormalMeleeCrit( target );
                target.OnGotMeleeCrit();
            }
            else
            {
                user.OnNormalMeleeHit( target );
                target.OnGotMeleeHit();
            }

            return AttackDamageResult.Create( damage, isCrit, isBlocked, DamageTypeInfo );
        }
    }
}
