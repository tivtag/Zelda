// <copyright file="SprintTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.SprintTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;

    /// <summary>
    /// The SprintTalent provides the Player with the SprintSkill.
    /// Sprint increases the movement speed of the Player for a fixed amount of time.
    /// </summary>
    internal sealed class SprintTalent : SkillTalent<SprintSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The movement speed increase the Talent provides per talent-level.
        /// </summary>
        private const float MovementSpeedIncreasePerLevel = 8.0f;

        /// <summary>
        /// The time in seconds it takes for the Skill to cool down
        /// and be reuseable again.
        /// </summary>
        public const float Cooldown = 120.0f;

        /// <summary>
        /// The duration of the effect.
        /// </summary>
        public const float Duration = 25.0f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the movement speed increase (in %) of Sprint provided by the Sprint Talent.
        /// </summary>
        public float MovementSpeedIncrease
        {
            get { return MovementSpeedIncreasePerLevel * Level; }
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
                TalentResources.TD_Sprint,
                (MovementSpeedIncreasePerLevel * level).ToString( culture ),
                Duration.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SprintTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new SprintTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal SprintTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Sprint,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Sprint" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the talent and its connections with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( AgilityTrainingTalent ) ), 3 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of the SkillTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new SprintSkill( this );
        }

        #endregion
    }
}
