// <copyright file="AppliedImpactResearchTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.AppliedImpactResearchTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Magic
{
    /// <summary>
    /// Increases the maximum number of new Firewhirls
    /// a Firewhirls can split into upon impact by 1/2/3.
    /// </summary>
    /// <seealso cref="ImpactTheoryTalent"/>
    internal sealed class AppliedImpactResearchTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the number of maximum additional times the AppliedImpactResearchTalent
        /// allows Whirlwind to split into new Whirlwinds.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The number of additional splits.
        /// </returns>
        private static int GetAdditionalSplits( int level )
        {
            return level;
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
                TalentResources.TD_AppliedImpactResearch,
                GetAdditionalSplits( level ).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the number of additional new Whirlwinds a Whirlwind
        /// can split if the player has invested into AppliedImpactResearchTalent.
        /// </summary>
        /// <value>
        /// The chance to proc in %.
        /// </value>
        public int AdditionalSplits
        {
            get
            {
                return GetAdditionalSplits( this.Level );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedImpactResearchTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the AppliedImpactResearchTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal AppliedImpactResearchTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_AppliedImpactResearch,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_AppliedImpactResearch" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this AppliedImpactResearchTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.impactTheoryTalent = this.Tree.GetTalent<ImpactTheoryTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ImpactTheoryTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this AppliedImpactResearchTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this AppliedImpactResearchTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this AppliedImpactResearchTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.impactTheoryTalent.RefreshMaximumSplitCount();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ImpactTheoryTalent which gets improved by this AppliedImpactResearchTalent.
        /// </summary>
        private ImpactTheoryTalent impactTheoryTalent;

        #endregion
    }
}
