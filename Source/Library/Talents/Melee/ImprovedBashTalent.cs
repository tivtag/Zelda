// <copyright file="ImprovedBashTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.ImprovedBashTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    /// <summary>
    /// 'Improved Bash' is a passive talent that increases
    /// the chance to crit with Bash by 5% per point invested
    /// for a total of 15%.
    /// </summary>
    internal sealed class ImprovedBashTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The crit increase 'Improved Bash' provides to Bash per talent point.
        /// </summary>
        private const float CritIncreasePerLevel = 5.0f;

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
                TalentResources.TD_ImprovedBash,
                (level * CritIncreasePerLevel).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the crit increase this ImprovedBashTalent provides for the BashSkill.
        /// </summary>
        public float BashCritIncrease
        {
            get { return CritIncreasePerLevel * Level; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ImprovedBashTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the ImprovedBashTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ImprovedBashTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_ImprovedBash,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ImprovedBash" ),
                3,
                tree,
                serviceProvider
            )
        {
        }
        
        /// <summary>
        /// Setups the connections of this ImprovedBashTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.bashTalent = this.Tree.GetTalent<BashTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( bashTalent, 4 )
            };

            Talent[] following = new Talent[1] {                
                this.Tree.GetTalent( typeof( WhirlwindTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this ImprovedBashTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this ImprovedBashTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes this ImprovedBashTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.bashTalent.Skill.RefreshDataFromTalents();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the BashTalent which gets improved by this ImprovedBashTalent.
        /// </summary>
        private BashTalent bashTalent;

        #endregion
    }
}
