using Atom.Math;
using Zelda.Status;
using Zelda.Status.Damage;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Calculates the damage of a FireBomb.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class FireBombDamageMethod : AttackDamageMethod
    {
        #region [ Damage Type Info ]

        /// <summary>
        /// Stores type information about the damage inflicted by the FireBombDamageMethod.
        /// </summary>
        private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
            DamageSource.Spell,
            ElementalSchool.Fire
        );

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Calculates the damage done of the given user on the given target
        /// using the <see cref="FireBombDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The results of the calculation.</returns>
        public override AttackDamageResult GetDamageDone( Statable user, Statable target )
        {
            var userEx = (ExtendedStatable)user;

            // Did it resist?
            if( target.Resistances.TryResist( userEx, ElementalSchool.Fire ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );

            // Calculate individual damage.
            int magicDamage = this.GetMagicDamage( userEx );
            int rangedDamage = this.GetRangedDamage( userEx );

            // Calculate overall damage.
            int damage = magicDamage + rangedDamage;

            // Apply fixed modifiers:
            damage = userEx.DamageDone.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyFixedMagicalSpell( damage, ElementalSchool.Fire );

            // Apply multipliers:
            damage = userEx.DamageDone.ApplyMagicalSpell( damage, ElementalSchool.Fire, target.Race );
            damage = target.DamageTaken.ApplyMagicalSpell( damage, ElementalSchool.Fire );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * GetCritDamageModifier( user ));
            
            // 3. Magical attacks pierce through armor!
            if( isCrit )
            {
                user.OnMagicCrit();
                target.OnGotMagicCrit();
            }
            else
            {
                user.OnMagicHit();
                target.OnGotMagicHit();
            }

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Gets the magical damage done by the FireBomb.
        /// </summary>
        /// <param name="user">
        /// The planter of the FireBomb.
        /// </param>
        /// <returns>
        /// The magical damage done by the FireBomb; before modifiers.
        /// </returns>
        private int GetMagicDamage( ExtendedStatable user )
        {
            int magicDamage = user.SpellPower.GetDamage( ElementalSchool.Fire );

            magicDamage = (int)(this.magicDamageContribution * magicDamage);

            return magicDamage;
        }

        /// <summary>
        /// Gets the ranged damage done by the FireBomb.
        /// </summary>
        /// <param name="user">
        /// The planter of the FireBomb.
        /// </param>
        /// <returns>
        /// The ranged damage done by the FireBomb; before modifiers.
        /// </returns>
        private int GetRangedDamage( ExtendedStatable user )
        {
            int rangedDamage = rand.RandomRange( user.DamageRangedNormalizedMin, user.DamageRangedNormalizedMax );

            rangedDamage = (int)(this.rangedDamageContribution * rangedDamage);

            return rangedDamage;
        }

        /// <summary>
        /// Gets the critical damage modifier used by this FireBombDamageMethod.
        /// </summary>
        /// <param name="user">
        /// The user of the FireBomb.
        /// </param>
        /// <returns>
        /// The critical damage modifier that should be used
        /// when the FireBomb crits.
        /// </returns>
        private static float GetCritDamageModifier( Statable user )
        {
            return 0.5f * (user.CritModifierSpell + user.CritModifierRanged);
        }

        /// <summary>
        /// Sets the parameters of this FireBombDamageMethod.
        /// </summary>
        /// <param name="rangedDamageContribution">
        /// The amount of damage that comes from the ranged damage of the user.
        /// </param>
        /// <param name="magicDamageContribution">
        /// The amount of damage that comes from the magic damage of the user.
        /// </param>
        public void SetValues( float rangedDamageContribution, float magicDamageContribution )
        {
            this.rangedDamageContribution = rangedDamageContribution;
            this.magicDamageContribution  = magicDamageContribution;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The amount of damage that comes from the ranged damage of the user.
        /// </summary>
        private float rangedDamageContribution;

        /// <summary>
        /// The amount of damage that comes from the magic damage of the user.
        /// </summary>
        private float magicDamageContribution;

        #endregion
    }
}
