// <copyright file="CorrosiveFireTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.CorrosiveFireTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Magic
{
    /// <summary>
    /// Firewhirl also burns the target for 20%/30% to 30%/40%
    /// fire damage over 6 seconds.
    /// </summary>
    internal sealed class CorrosiveFireTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The duration of the burn effect.
        /// </summary>
        public const float Duration = 6.0f;

        /// <summary>
        /// The number of damage ticks.
        /// </summary>
        public const int TickCount = 3;

        /// <summary>
        /// Gets the minimum damage modifier of the corrosive fire effect.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The minimum damage modifier.
        /// </returns>
        private static float GetMinimumDamageModifier( int level )
        {
            return 0.1f + (level * 0.1f);
        }

        /// <summary>
        /// Gets the maximum damage modifier of the corrosive fire effect.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The maximum damage modifier.
        /// </returns>
        private static float GetMaximumDamageModifier( int level )
        {
            return 0.2f + (level * 0.1f);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the minimum damage modifier of the corrosive fire effect.
        /// </summary>
        /// <returns>
        /// The minimum damage modifier.
        /// </returns>
        public float MinimumDamageModifier
        {
            get
            {
                return GetMinimumDamageModifier( this.Level );
            }
        }

        /// <summary>
        /// Gets the maximum damage modifier of the corrosive fire effect.
        /// </summary>
        /// <returns>
        /// The maximum damage modifier.
        /// </returns>
        public float MaximumDamageModifier
        {
            get
            {
                return GetMaximumDamageModifier( this.Level );
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
                TalentResources.TD_CorrosiveFire,
                (GetMinimumDamageModifier( level ) * 100).ToString( culture ),
                (GetMaximumDamageModifier( level ) * 100).ToString( culture ),
                Duration.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrosiveFireTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the CorrosiveFireTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal CorrosiveFireTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_CorrosiveFire,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_CorrosiveFire" ),
                2,
                tree,
                serviceProvider
            )
        {
        }
        
        /// <summary>
        /// Setups the connections of this CorrosiveFireTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.firewhirlTalent = this.Tree.GetTalent<FirewhirlTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( firewhirlTalent, 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this CorrosiveFireTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this CorrosiveFireTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this CorrosiveFireTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.firewhirlTalent.Skill.RefreshDataFromTalents();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the FirewhirlTalent which gets improved by this CorrosiveFireTalent.
        /// </summary>
        private FirewhirlTalent firewhirlTalent;

        #endregion
    }
}
