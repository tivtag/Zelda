using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Defines the <see cref="AttackDamageMethod"/> of the <see cref="WhirlwindAttack"/>
    /// </summary>
    internal sealed class WhirlwindDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the WhirlwindDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalMelee;

        /// <summary>
        /// Calculates the damage done of the given user on the given target
        /// using this WhirlwindDamageMethod.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the calculation.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            // Did it miss?
            if( user.TryHit( target ) )
                return AttackDamageResult.CreateMissed( DamageTypeInfo );

            var player = (ExtendedStatable)user;

            // 1. Whirlwind can't be dodged.
            // 2. Nor parried.
            int damage = rand.RandomRange( player.DamageMeleeNormalizedMin, player.DamageMeleeNormalizedMax );

            // Fixed Modifiers:
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );

            // Multipliers:
            damage = (int)(damage * this.multiplier); // 3. Increased damage.
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);

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

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Sets the values of the WhirlwindDamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The damage multiplier.
        /// </param>
        public void SetValues( float multiplier )
        {
            this.multiplier = multiplier;
        }

        /// <summary>
        /// The damage multiplier of the Whirlwind attack.
        /// </summary>
        private float multiplier;
    }
}
