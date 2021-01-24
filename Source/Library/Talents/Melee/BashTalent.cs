// <copyright file="BashTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.BashTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    using Zelda.Talents.Support;
    
    /// <summary>
    /// The BashTalent provides the player access to the BashSkill.
    /// <para>
    /// Bash is a massive single strike which does increased damage.
    /// It costs mana and has a cooldown.
    /// </para>
    /// </summary>
    internal sealed class BashTalent : SkillTalent<BashSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The fixed damage increase provided by Bash based on the level of the talent.
        /// </summary>
        private const int FixedDamageIncreasePerLevel = 1;

        /// <summary>
        /// The multiplicative damage increase provided by Bash based on the level of the talent.
        /// </summary>
        private const float MultiplicativeDamageIncreasePerLevel = 0.05f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.15f;

        /// <summary>
        /// The base cooldown of the Bash Attack.
        /// </summary>
        private const float BaseCooldown = 3.5f;

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
                TalentResources.TD_Bash,
                (FixedDamageIncreasePerLevel * level).ToString( culture ),
                (100.0f + (100.0f * MultiplicativeDamageIncreasePerLevel * level)).ToString( culture ) 
            );            
        }

        /// <summary>
        /// Gets the damage multiplier the talent provides to the Bash Attack.
        /// </summary>
        public float DamageMultiplier
        {
            get { return 1.0f + (MultiplicativeDamageIncreasePerLevel * this.Level); }
        }

        /// <summary>
        /// Gets the fixed damage increase the talent provides to the Bash Attack.
        /// </summary>
        public int FixedDamageIncrease
        {
            get { return FixedDamageIncreasePerLevel * Level; }
        }

        /// <summary>
        /// Gets the cooldown of the Bash attack in seconds.
        /// </summary>
        public float Cooldown
        {
            get
            {
                return BaseCooldown - this.swiftFightStyleTalent.BashCooldownReduction;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BashTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new BashTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BashTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_Bash,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Bash" ),
                5,
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
                new TalentRequirement( this.Tree.GetTalent( typeof( MeleeTrainingTalent ) ), 2 ) 
            };

            Talent[] followingTalents = new Talent[2] {
                Tree.GetTalent( typeof( ImprovedBashTalent  ) ),                
                Tree.GetTalent( typeof( PushingAttackTalent ) )
            };

            this.SetTreeStructure( requirements, followingTalents );
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
            return new BashSkill(
               this,
               this.Tree.GetTalent<ImprovedBashTalent>(),
               this.ServiceProvider
            );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the SwiftFightStyleTalent that improvides this BashTalent.
        /// </summary>
        private SwiftFightStyleTalent swiftFightStyleTalent;

        #endregion
    }
}
