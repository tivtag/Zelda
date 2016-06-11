using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Calculates the damage done by the Headshot attack.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class HeadshotDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the HeadshotDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.PhysicalRanged;

        #region [ Methods ]

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
            int damage = rand.RandomRange( player.DamageRangedNormalizedMin, player.DamageRangedNormalizedMax );

            // Apply fixed modifiers:
            damage = player.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = player.DamageDone.Apply( damage, target, DamageTypeInfo );
            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );

            // Critical:
            bool isCrit = user.TryCrit( target, this.extraCritChance );
            if( isCrit )
                damage = (int)(damage * user.CritModifierRanged);
            
            // Handle Block:
            bool isBlocked = target.TryBlock( user, rand );
            if( isBlocked )
            {
                damage = target.HandleBlock( damage, user, rand );
            }

            // Mitigate damage:
            int targetArmor = (int)(target.Armor * this.armorPiercingModifier);
            damage = (int)(damage * player.GetPhysicalMitigationOf( targetArmor, target.Level ));

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

        /// <summary>
        /// Sets the additional values of thie HeadshotDamageMethod.
        /// </summary>
        /// <param name="critChanceIncrease">
        /// The crit chance increase the Headshot attack has compared to a normal attack.
        /// </param>
        /// <param name="armorPiercingModifier">
        /// The modifier that is applied to the armor of the enemy.
        /// </param>
        public void SetValues( float critChanceIncrease, float armorPiercingModifier )
        {
            this.extraCritChance = critChanceIncrease;
            this.armorPiercingModifier = armorPiercingModifier;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The crit chance increase the Headshot attack has compared to a normal attack.
        /// </summary>
        private float extraCritChance;

        /// <summary>
        /// The modifier that is applied to the armor of the enemy.
        /// </summary>
        private float armorPiercingModifier;

        #endregion
    }
}
