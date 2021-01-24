// <copyright file="SwiftFightStyleTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.SwiftFightStyleTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Support
{
    using Zelda.Talents.Magic;
    using Zelda.Talents.Melee;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// Reduces the cooldown of Bash and Light Arrow by 0.25/0.5 seconds
    /// and the cooldown of Quick Strike and Firewhirl by 0.1/0.2 seconds.
    /// </summary>
    internal sealed class SwiftFightStyleTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the reduction in cooldown this SwiftFightStyleTalent provides
        /// to Bash and Light Arrow for the given level in seconds.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cooldown reduction in seconds.
        /// </returns>
        private static float GetBashAndLightArrowCooldownReduction( int level )
        {
            return level * 0.25f;
        }

        /// <summary>
        /// Gets the reduction in cooldown this SwiftFightStyleTalent provides
        /// to Quick Strike for the given level in seconds.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cooldown reduction in seconds.
        /// </returns>    
        private static float GetQuickStrikeAndFirewhirlCooldownReduction( int level )
        {
            return level * 0.1f;
        }

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the cooldown reduction this SwiftFightStyleTalent provides for the BashSkill.
        /// </summary>
        public float BashCooldownReduction
        {
            get { return GetBashAndLightArrowCooldownReduction( this.Level ); }
        }

        /// <summary>
        /// Gets the cooldown reduction this SwiftFightStyleTalent provides for the LightArrowSkill.
        /// </summary>
        public float LightArrowCooldownReduction
        {
            get { return GetBashAndLightArrowCooldownReduction( this.Level ); }
        }

        /// <summary>
        /// Gets the cooldown reduction this SwiftFightStyleTalent provides for the QuickStrikeSkill.
        /// </summary>
        public float QuickStrikeCooldownReduction
        {
            get { return GetQuickStrikeAndFirewhirlCooldownReduction( this.Level ); }
        }

        /// <summary>
        /// Gets the cooldown reduction this SwiftFightStyleTalent provides for the FirewhirlSkill.
        /// </summary>
        public float FirewhirlCooldownReduction
        {
            get { return GetQuickStrikeAndFirewhirlCooldownReduction( this.Level ); }
        }

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
                TalentResources.TD_SwiftFightStyle,
                GetBashAndLightArrowCooldownReduction( level ).ToString( culture ),
                GetQuickStrikeAndFirewhirlCooldownReduction( level ).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SwiftFightStyleTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the ImprovedBashTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal SwiftFightStyleTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_SwiftFightStyle,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_SwiftFightStyle" ),
                2,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this SwiftFightStyleTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.bashTalent        = this.Tree.GetTalent<BashTalent>();
            this.lightArrowTalent  = this.Tree.GetTalent<LightArrowTalent>();
            this.quickStrikeTalent = this.Tree.GetTalent<QuickStrikeTalent>();
            this.firewhirlTalent = this.Tree.GetTalent<FirewhirlTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( CriticalBalanceTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this SwiftFightStyleTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this SwiftFightStyleTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this SwiftFightStyleTalent.
        /// </summary>
        protected override void Refresh()
        {
            RefreshCooldown( this.bashTalent );
            RefreshCooldown( this.lightArrowTalent );
            RefreshCooldown( this.quickStrikeTalent );
            RefreshCooldown( this.firewhirlTalent );
        }

        /// <summary>
        /// Refreshes the cooldown of the given SkillTalent.
        /// </summary>
        /// <param name="talent">
        /// The talent to refresh.
        /// </param>
        private static void RefreshCooldown( SkillTalent talent )
        {
            var skill = talent.Skill;

            if( skill != null )
            {
                skill.RefreshDataFromTalents();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the BashTalent which gets improved by this SwiftFightStyleTalent.
        /// </summary>
        private BashTalent bashTalent;

        /// <summary>
        /// Identifies the LightArrowTalent which gets improved by this SwiftFightStyleTalent.
        /// </summary>
        private LightArrowTalent lightArrowTalent;

        /// <summary>
        /// Identifies the QuickStrikeTalent which gets improved by this SwiftFightStyleTalent.
        /// </summary>
        private QuickStrikeTalent quickStrikeTalent;

        /// <summary>
        /// Identifies the FirewhirlTalent which gets improved by this SwiftFightStyleTalent.
        /// </summary>
        private FirewhirlTalent firewhirlTalent;

        #endregion
    }
}
