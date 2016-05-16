// <copyright file="ShieldBlockTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.ShieldBlockTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The ShieldBlockTalent provides the player access to the ShieldBlockSkill.
    /// <para>
    /// Shield Block increases Chance to Block by 10/20/30 for 15 seconds. 
    /// 60 seconds cooldown.
    /// </para>
    /// </summary>
    internal sealed class ShieldBlockTalent : SkillTalent<ShieldBlockSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The cooldown of the Shield Block skill.
        /// </summary>
        public const float Cooldown = 60.0f;

        /// <summary>
        /// The duration of the Battle Shout skill.
        /// </summary>
        public const float Duration = 15.0f;
        
        /// <summary>
        /// Gets the fixed Block Chance increase the Shield Block skill provides
        /// at the given level.
        /// </summary>
        /// <param name="level">The level of the Talent.</param>
        /// <returns>
        /// The increased block chance.
        /// </returns>
        private static float GetBlockChanceIncrease( int level )
        {
            return level * 10.0f;
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
                TalentResources.TD_ShieldBlock,
                GetBlockChanceIncrease( level ).ToString( culture ),
                Duration.ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the fixed Block Chance increase the Shield Block skill provides.
        /// </summary>
        public float BlockChanceIncrease
        {
            get
            {
                return GetBlockChanceIncrease( this.Level );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldBlockTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ShieldBlockTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ShieldBlockTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ShieldBlock,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ShieldMastery" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ShieldBlockTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] { 
                new TalentRequirement( Tree.GetTalent( typeof( ShieldMasteryTalent ) ), 2 ) 
            };

            Talent[] followingTalents = null;
            this.SetTreeStructure( requirements, followingTalents );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this ShieldBlockTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new ShieldBlockSkill( this );
        }

        #endregion
    }
}
