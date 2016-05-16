// <copyright file="FurorTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.FurorTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The FurorTalent provides the player with the Furor skill.
    /// <para>
    /// Furor increases melee attack speed by 15%/30%/45% for 10 seconds.
    /// </para>
    /// </summary>
    internal sealed class FurorTalent : SkillTalent<FurorSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The ranged speed increase provided by Furor for each talent level.
        /// </summary>
        private const float SpeedIncreasePerLevel = 15.0f;

        /// <summary>
        /// The duration of Furor in seconds.
        /// </summary>
        public const float Duration = 10.0f;

        /// <summary>
        /// The cooldown of the Furor skill.
        /// </summary>
        public const float Cooldown = 60.0f;

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
                TalentResources.TD_Furor,
                (level * SpeedIncreasePerLevel).ToString( culture ),
                Duration.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the speed increase provided by the Furor skill.
        /// </summary>
        public float SpeedIncrease
        {
            get
            {
                return this.Level * SpeedIncreasePerLevel;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FurorTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FurorTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FurorTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Furor,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Furor" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FurorTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( FrenzyTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this FurorTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new FurorSkill( this );
        }

        #endregion
    }
}
