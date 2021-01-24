// <copyright file="RevitalizingStrikesTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.RevitalizingStrikesTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Atom.Math;
    using Zelda.Status.Damage;

    /// <summary>
    /// The RevitalizingStrikesTalent gives a 15% chance on physical melee or 
    /// ranged attack to heal self for 15/20/25% of damage done.
    /// </summary>
    internal sealed class RevitalizingStrikesTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// States the chance for the Revitalizing Strikes effect
        /// to proc on a melee or ranged attack in percent.
        /// </summary>
        private const float FixedProcChance = 15.0f;

        /// <summary>
        /// Gets the multiplier that is applied to damage done to
        /// get healing done by the Revitalizing Strikes.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The multiplier that convert damage -> healing.
        /// </returns>
        private static float GetDamageToHealingMultiplier( int level )
        {
            return 0.1f + (level * 0.05f);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the description of this Talent for
        /// the specified talent level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent to get the description for.
        /// </param>
        /// <returns>
        /// The localized description of this Talent for the specified talent level.
        /// </returns>
        protected override string GetDescription( int level )
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                TalentResources.TD_RevitalizingStrikes,
                FixedProcChance.ToString( culture ),
                (100.0f * GetDamageToHealingMultiplier(level)). ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RevitalizingStrikesTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new RevitalizingStrikesTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RevitalizingStrikesTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_RevitalizingStrikes,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Frenzy" ),
                3,
                tree,
                serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Setups the Talent's TalentRequirements, following Talents and any additional components.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( RecoverWoundsTalent ) ), 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this RevitalizingStrikesTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.Owner.Attackable.AttackHit += this.OnAttackUsed;
        }

        /// <summary>
        /// Initializes this RevitalizingStrikesTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.Owner.Attackable.AttackHit -= this.OnAttackUsed;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this RevitalizingStrikesTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.damageToHealingMultiplier = GetDamageToHealingMultiplier( this.Level );
        }

        /// <summary>
        /// Called when the player has used any attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The AttackEventArgs that contain the event data.
        /// </param>
        private void OnAttackUsed( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            if( this.TryProc( e.DamageResult ) )
            {
                this.Proc( e.DamageResult.Damage );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Revitalizing Strikes
        /// effect has procced.
        /// </summary>
        /// <param name="damageResult">
        /// The result of the attack which might proc the effect.
        /// </param>
        /// <returns>
        /// true if it has procced;
        /// otherwise false.
        /// </returns>
        private bool TryProc( Zelda.Attacks.AttackDamageResult damageResult )
        {
            if( CanProc( ref damageResult ) )
            {
                float roll = this.rand.RandomRange( 0.0f, 100.0f );
                return roll <= FixedProcChance;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Procs the Revitalizing Strikes effect; healing the player.
        /// </summary>
        /// <param name="damageDone">
        /// The amount of damage that has been done.
        /// </param>
        private void Proc( int damageDone )
        {
            int amount = GetAmountHealed( damageDone );
            this.ApplyHealing( amount );
        }

        /// <summary>
        /// Converts damage done of the player into self healing
        /// given by the Revitalizing Strikes effect.
        /// </summary>
        /// <param name="damageDone">
        /// The damage that has been done.
        /// </param>
        /// <returns>
        /// The self healing the player should receive.
        /// </returns>
        private int GetAmountHealed( int damageDone )
        {
            return (int)(damageDone * this.damageToHealingMultiplier);
        }

        /// <summary>
        /// Heals the player that owns this RevitalizingStrikesTalent
        /// for the specified <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">
        /// The amount to heal.
        /// </param>
        private void ApplyHealing( int amount )
        {
            this.Owner.Statable.RestoreLife(
                Zelda.Attacks.AttackDamageResult.CreateHealed( amount )
            );
        }

        /// <summary>
        /// Gets a value indicating whether damage descriped by the specified
        /// <see cref="DamageTypeInfo"/> instance can proc the Revitalizing Strikes
        /// effect.
        /// </summary>
        /// <param name="damageResult">
        /// The result of the attack which might proc the effect.
        /// </param>
        /// <returns>
        /// true if theoretically the Revitalizing Strikes effect could proc;
        /// otherwise false.
        /// </returns>
        private static bool CanProc( ref Zelda.Attacks.AttackDamageResult damageResult )
        {
            if( damageResult.Damage <= 0 )
                return false;

            var damageType = damageResult.DamageTypeInfo;

            if( damageType != null )
            {
                return damageType.School == DamageSchool.Physical &&
                       damageType.SpecialType == SpecialDamageType.None;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The multiplier value that converts a damage done into self healing received.
        /// </summary>
        private float damageToHealingMultiplier;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        #endregion
    }
}
