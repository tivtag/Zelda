using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Calculates the damage of a normal ranged attack of the Player. This is a sealed class.
    /// </summary>
    internal sealed class NormalPlayerRangedDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the NormalPlayerRangedDamageMethod.
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

            var player = (ExtendedStatable)user;
            int damage = rand.RandomRange( user.DamageRangedMin, user.DamageRangedMax );

            // Apply fixed modifiers:
            damage = player.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = player.DamageDone.Apply( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierRanged);

            // Mitigate damage:
            damage = (int)(damage * player.GetPhysicalMitigationOf( target ));

            if( isCrit )
            {
                user.OnRangedCrit();
                target.OnGotRangedCrit();
            }
            else
            {
                user.OnNormalRangedHit( target );
                target.OnGotRangedHit();
            }

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }
    }
}
