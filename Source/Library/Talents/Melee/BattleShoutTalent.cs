// <copyright file="BattleShoutTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.BattleShoutTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The BattleShoutTalent provides the player access to the BattleShoutSkill.
    /// <para>
    /// Battle Shout increases strength by (at level 5) 25% + 5 for  60 seconds.
    /// Cooldown 120 seconds.
    /// </para>
    /// </summary>
    internal sealed class BattleShoutTalent : SkillTalent<BattleShoutSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The fixed strength increase provided by Battle Shout based on the level of the talent.
        /// </summary>
        private const int FixedStrengthIncreasePerLevel = 1;

        /// <summary>
        /// The multiplicative strength increase provided by Battle Shout based on the level of the talent.
        /// </summary>
        private const float MultiplicativeStrengthIncreasePerLevel = 5.0f;

        /// <summary>
        /// The cooldown of Battle Shout.
        /// </summary>
        public const float Cooldown = 120.0f;

        /// <summary>
        /// The duration of Battle Shout.
        /// </summary>
        public const float Duration = 60.0f;

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
                TalentResources.TD_BattleShout,
                (FixedStrengthIncreasePerLevel * level).ToString( culture ),
                (100.0f + (MultiplicativeStrengthIncreasePerLevel * level)).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the strength multiplier the talent provides to the Bash Attack.
        /// </summary>
        public float StrengthMultiplier
        {
            get { return MultiplicativeStrengthIncreasePerLevel * Level; }
        }

        /// <summary>
        /// Gets the fixed damage increase the talent provides to the Bash Attack.
        /// </summary>
        public int FixedStrengthIncrease
        {
            get { return FixedStrengthIncreasePerLevel * Level; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleShoutTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new BattleShoutTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BattleShoutTalent(  TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_BattleShout,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_BattleShout" ),
                5,
                tree, 
                serviceProvider 
            )
        {
        }

        /// <summary>
        /// Setups the connections of this BattleShoutTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1]
            { 
                new TalentRequirement( Tree.GetTalent( typeof( MeleeTrainingTalent ) ), 4 ) 
            };

            Talent[] followingTalents = new Talent[2]
            {
                this.Tree.GetTalent( typeof( FrenzyTalent ) ),
                this.Tree.GetTalent( typeof( EnrageTalent ) )
            };

            SetTreeStructure( requirements, followingTalents );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this BattleShoutTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new BattleShoutSkill( this );
        }

        #endregion
    }
}
