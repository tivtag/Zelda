// <copyright file="RecoverWoundsTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.RecoverWoundsTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The <see cref="RecoverWoundsTalent"/> provides the player
    /// the RecoverWoundsSkill.
    /// <para>
    /// Recover Wounds increases Life Regeneration 
    /// by 100% per talent-level for a total of 300%.
    /// </para>
    /// </summary>
    internal sealed class RecoverWoundsTalent : SkillTalent<RecoverWoundsSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The duration of the Increased Life Regneration effect
        /// provided by the skill.
        /// </summary>
        public const float Duration = 45.0f;

        /// <summary>
        /// The cooldown of the RecoverWoundsSkill.
        /// </summary>
        public const float Cooldown = 300.0f;

        /// <summary>
        /// The life regeneration increase in % provided by the Talent.
        /// </summary>
        private const float LifeRegenIncreasePerLevel = 100.0f;

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
                TalentResources.TD_RecoverWounds,
                (level * LifeRegenIncreasePerLevel).ToString( culture ),
                Duration.ToString( culture ) 
            );
        }

        /// <summary>
        /// Gets the Life Regeneration increase provided by the effect of the RecoverWoundsSkill.
        /// </summary>
        public float LifeRegenerationIncrease
        {
            get
            {
                return this.Level * LifeRegenIncreasePerLevel;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoverWoundsTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new RecoverWoundsTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RecoverWoundsTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_RecoverWounds,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Recover" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this RecoverWoundsTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( VitalityTalent ) ), 2 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( RevitalizingStrikesTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of the RecoverWoundsTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new RecoverWoundsSkill( this );
        }

        #endregion
    }
}
