// <copyright file="QuickStrikeTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.QuickStrikeTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    using Zelda.Talents.Support;

    /// <summary>
    /// The QuickStrikeTalent learns the QuickStrikeSkill.
    /// <para>
    /// Requires a Dagger to be used!
    /// </para>
    /// <para>
    /// Swiftly strikes at the enemy dealing MeleeDamage. 
    /// Can proc Double Attack. Cooldown of {0} seconds.
    /// </para>
    /// </summary>
    internal sealed class QuickStrikeTalent : SkillTalent<QuickStrikeSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.05f;

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
                TalentResources.TD_QuickStrike,
                GetCooldown( level ).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the damage multiplier the talent provides to the Bash Attack.
        /// </summary>
        public float Cooldown
        {
            get 
            {
                return GetCooldown( this.Level ) - this.swiftFightStyleTalent.QuickStrikeCooldownReduction;
            }
        }

        /// <summary>
        /// Gets the cooldown of the Quick Strike attack.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cooldown in seconds.
        /// </returns>
        private static float GetCooldown( int level )
        {
            return 5.0f - level;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickStrikeTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new QuickStrikeTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal QuickStrikeTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_QuickStrike,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_QuickStrike" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this BashTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.swiftFightStyleTalent = this.Tree.GetTalent<SwiftFightStyleTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] { 
                new TalentRequirement( this.Tree.GetTalent( typeof( DoubleAttackTalent ) ), 1 ) 
            };

            Talent[] followingTalents = null;
            SetTreeStructure( requirements, followingTalents );
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
            return new QuickStrikeSkill( this, this.ServiceProvider );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the SwiftFightStyleTalent that improves this QuickStrikeTalent.
        /// </summary>
        private SwiftFightStyleTalent swiftFightStyleTalent;

        #endregion
    }
}
