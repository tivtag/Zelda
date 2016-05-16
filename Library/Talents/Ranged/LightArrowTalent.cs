// <copyright file="LightArrowTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.LightArrowTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;
    using Zelda.Talents.Support;

    /// <summary>
    /// The LightArrowTalent provides the player access to the LightArrowSkill.
    /// <para>
    /// The LightArrowSkill is an instant attack that does Light damage
    /// equal to (Ranged Damage + Fixed)*Multiplier.
    /// Each use takes up 10% of base mana.
    /// </para>
    /// </summary>
    internal sealed class LightArrowTalent : SkillTalent<LightArrowSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The time in seconds it takes for the attack to cooldown
        /// and be reuseable again.
        /// </summary>
        private const float BaseCooldown = 9.0f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.18f;

        /// <summary>
        /// The fixed damage increase provided over a normal ranged attack.
        /// </summary>
        private const int FixedDamageIncreasePerLevel = 1;

        /// <summary>
        /// The multiplicative damage increase provided over a normal ranged attack.
        /// </summary>
        private const float MultiplicativeDamageIncreasePerLevel = 0.01f;

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
                TalentResources.TD_LightArrow,
                (FixedDamageIncreasePerLevel * level).ToString( culture ),
                (100.0f + (100.0f * MultiplicativeDamageIncreasePerLevel * level)).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the cooldown of the Light Arrow attack.
        /// </summary>
        public float Cooldown
        {
            get
            {
                return BaseCooldown - this.swiftFightStyleTalent.LightArrowCooldownReduction;
            }
        }

        /// <summary>
        /// Gets the damage multiplier the talent provides to the Light Arrow Attack.
        /// </summary>
        public float DamageMultiplier
        {
            get { return 1.0f + (MultiplicativeDamageIncreasePerLevel * this.Level); }
        }

        /// <summary>
        /// Gets the fixed damage increase the talent provides to the Light Arrow Attack.
        /// </summary>
        public int FixedDamageIncrease
        {
            get { return FixedDamageIncreasePerLevel * Level; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LightArrowTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new LightArrowTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal LightArrowTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_LightArrow,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_LightArrow" ),
                5,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this LightArrowTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.swiftFightStyleTalent = this.Tree.GetTalent<SwiftFightStyleTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( RangedTrainingTalent ) ), 2 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( MultiShotTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of the LightArrowTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new LightArrowSkill( this, this.ServiceProvider );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the SwiftFightStyleTalent that improves this Light Arrow Talent.
        /// </summary>
        private SwiftFightStyleTalent swiftFightStyleTalent;

        #endregion
    }
}
