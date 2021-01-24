// <copyright file="RazorWindsTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.RazorWindsTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Magic
{
    /// <summary>
    /// You learn to control the winds around a Firevortex.
    /// Increases the chance of the vortex
    /// to pierce through a target by 30/40/50/60/70%.
    /// </summary>
    internal sealed class RazorWindsTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the chance of Firevortex to pierce its target.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The chance ot pierce in %.
        /// </returns>
        private static float GetPiercingChance( int level )
        {
            switch( level )
            {
                case 1:
                    return 30.0f;
                case 2:
                    return 40.0f;
                case 3:
                    return 50.0f;
                case 4:
                    return 60.0f;
                case 5:
                    return 70.0f;

                default:
                case 0:
                    return 0.0f;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the chance of Firevortex that it pierces its target and continues travelling.
        /// </summary>
        public float FirevortexPiercingChance
        {
            get
            {
                return GetPiercingChance( this.Level );
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
                TalentResources.TD_RazorWinds,
                GetPiercingChance( level ).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorWindsTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the RazorWindsTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RazorWindsTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_RazorWinds,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_RazorWinds" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this RazorWindsTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.firevortexTalent = this.Tree.GetTalent<FirevortexTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( firevortexTalent, 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this RazorWindsTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this RazorWindsTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this RazorWindsTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.firevortexTalent.Skill.RefreshDataFromTalents();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the FirevortexTalent which gets improved by this RazorWindsTalent.
        /// </summary>
        private FirevortexTalent firevortexTalent;

        #endregion
    }
}
