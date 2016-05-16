// <copyright file="ImpactTheoryTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.ImpactTheoryTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Magic
{
    using Atom.Math;

    /// <summary>
    /// Impact Theory gives Firewhirl a 10/20/30% chance to split
    /// into atleast 4 new Firewhirls.
    /// </summary>
    internal sealed class ImpactTheoryTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The minimum number of Firewhirls an existing Firewhirl that undergoes the
        /// Impact Theory effect has to split into.
        /// </summary>
        private const int MinimumSplitCount = 2;

        /// <summary>
        /// The base maximum number of Firewhirls an existing Firewhirl that undergoes the
        /// Impact Theory effect can split into.
        /// </summary>
        private const int MaximumBaseSplitCount = 4;

        /// <summary>
        /// The offset applied before an offspring Firewhirl is spawned.
        /// </summary>
        public const float Offset = 8.0f;

        /// <summary>
        /// The distance a Firewhirl projectile must have travelled atleast
        /// before it can proc another Firewhirl. (squared, pixels)
        /// </summary>
        public const float RequiredTravelDistanceSquared = 1.0f;

        /// <summary>
        /// The maximum number of times a firewhirl can continuously split.
        /// </summary>
        public const float MaximumNumberOfContinuousSplits = 2;

        /// <summary>
        /// Gets the chance Whirlwind can proc the Impact Theory effect.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The chance to proc in %.
        /// </returns>
        private static float GetProcChance( int level )
        {
            return 10.0f + (level * 10.0f);
        }

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the chance Whirlwind can proc the Impact Theory efect.
        /// </summary>
        /// <value>
        /// The chance to proc in %.
        /// </value>
        public float ProcChance
        {
            get
            {
                return GetProcChance( this.Level );
            }
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
                TalentResources.TD_ImpactTheory,
                GetProcChance( level ).ToString( culture ),
                MinimumSplitCount.ToString( culture ),
                MaximumBaseSplitCount.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpactTheoryTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the ImpactTheoryTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ImpactTheoryTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ImpactTheory,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ImpactTheory" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ImpactTheoryTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.firewhirlTalent = this.Tree.GetTalent<FirewhirlTalent>();
            this.appliedImpactResearchTalent = this.Tree.GetTalent<AppliedImpactResearchTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( firewhirlTalent, 1 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( AppliedImpactResearchTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this ImpactTheoryTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this ImpactTheoryTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the maximum number of new Whirlwinds a Whirlwind can
        /// split upon impact.
        /// </summary>
        internal void RefreshMaximumSplitCount()
        {
            this.maximumSplitCount = MaximumBaseSplitCount + this.appliedImpactResearchTalent.AdditionalSplits;
        }

        /// <summary>
        /// Gets the number of Firewhirls the next Firewhirl should split.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// The number of splits.
        /// </returns>
        internal int GetNumberOfSplits( RandMT rand )
        {
            return rand.RandomRange( MinimumSplitCount, this.maximumSplitCount );
        }

        /// <summary>
        /// Refreshes the strength of this ImpactTheoryTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.firewhirlTalent.Skill.RefreshDataFromTalents();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The cached maximum number of sub-Whirlwinds a Whirlwind can split into.
        /// </summary>
        private int maximumSplitCount = MaximumBaseSplitCount;

        /// <summary>
        /// Identifies the FirewhirlTalent which gets improved by this ImpactTheoryTalent.
        /// </summary>
        private FirewhirlTalent firewhirlTalent;

        /// <summary>
        /// Identifies the AppliedImpactResearchTalent which improves this ImpactTheoryTalent.
        /// </summary>
        private AppliedImpactResearchTalent appliedImpactResearchTalent;

        #endregion
    }
}
