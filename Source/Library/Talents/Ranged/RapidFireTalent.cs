// <copyright file="RapidFireTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.RapidFireTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;
    
    /// <summary>
    /// The RapidFireTalent provides the player with the Rapid Fire skill.
    /// <para>
    /// Rapid Fire increases chance to pierce by 10%/20%/30% and ranged attack speed by 15%/30%/45% for 24 seconds.
    /// </para>
    /// </summary>
    internal sealed class RapidFireTalent : SkillTalent<RapidFireSkill>
    {
        /// <summary>
        /// The ranged speed increase provided by Rapid Fire for each talent level.
        /// </summary>
        private const float SpeedIncreasePerLevel = 15.0f;
        
        /// <summary>
        /// The ranged pierce increase provided by Rapid Fire for each talent level.
        /// </summary>
        private const float PierceIncreasePerLevel = 10.0f;

        /// <summary>
        /// The duration of Rapid Fire in seconds.
        /// </summary>
        public const float Duration = 24.0f;

        /// <summary>
        /// The cooldown of the Rapid Fire skill.
        /// </summary>
        public const float Cooldown = 120.0f;
        
        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.3f;
        
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
                TalentResources.TD_RapidFire,
                (level * SpeedIncreasePerLevel).ToString(culture),
                (level * PierceIncreasePerLevel).ToString(culture),
                Duration.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the speed increase provided by the Rapid Fire skill.
        /// </summary>
        public float SpeedChanceIncrease
        {
            get
            {
                return this.Level * SpeedIncreasePerLevel;
            }
        }    
        
        /// <summary>
        /// Gets the piercing increase provided by the Rapid Fire skill.
        /// </summary>
        public float PiercingChanceIncrease
        {
            get
            {
                return this.Level * PierceIncreasePerLevel;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RapidFireTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new RapidFireTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RapidFireTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_RapidFire,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Pierce" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this RapidFireTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( PiercingArrowsTalent ) ), 3 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( ArrowShowerTalent ) )                
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Creates the Skill object of this RapidFireTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new RapidFireSkill( this );
        }
    }
}
