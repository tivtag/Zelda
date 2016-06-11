using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Melee
{
    /// <summary>
    /// Calculates the damage of a self-inflicted attack caused by the Frustration skill.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class FrustationDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the FrustationDamageMethod.
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

            // 1. The Frustration self-attack can't be dodged.
            // 2. Nor parried.
            int damage = rand.RandomRange( player.DamageMeleeNormalizedMin, player.DamageMeleeNormalizedMax );
            
            // Fixed Modifiers:
            damage = user.DamageDone.ApplyFixedPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyFixedPhysicalMelee( damage );

            // Multipliers:
            damage = (int)(damage * this.multiplier);
            damage = user.DamageDone.ApplyPhysicalMelee( damage );
            damage = target.DamageTaken.ApplyPhysicalMelee( damage );
            
            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierMelee);
            
            // Mitigate damage:
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
        /// Sets the values of this DamageMethod.
        /// </summary>
        /// <param name="multiplier">
        /// The damage multiplier value.
        /// </param>
        public void SetValues( float multiplier )
        {
            this.multiplier = multiplier;
        }

        /// <summary>
        /// The damage multiplier value.
        /// </summary>
        private float multiplier = 1.0f;
    }
}
