using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Calculates the damage done by the Shield Breaker skill.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class ShieldBreakerDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the ShieldBreakerDamageMethod.
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
            damage += (int)(player.BlockValue * this.blockValueToDamageMultiplier);
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );

            // Multipliers:
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);

            // Shield Breaker ignores armor
            
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
        /// Sets the values of this ShieldBreakerDamageMethod.
        /// </summary>
        /// <param name="blockValueToDamageMultiplier">
        /// The BlockValue to Damage multiplier value.
        /// </param>
        public void SetValues( float blockValueToDamageMultiplier )
        {
            this.blockValueToDamageMultiplier = blockValueToDamageMultiplier;
        }

        /// <summary>
        /// The BlockValue to Damage multiplier value.
        /// </summary>
        private float blockValueToDamageMultiplier = 1.0f;
    }
}
