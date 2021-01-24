// <copyright file="FirevortexTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Magic.FirevortexTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Magic
{
    using Zelda.Skills.Magic;

    /// <summary>
    /// Each Firewall consists of 3 individual pillars that deal minor damage
    /// but push back the enemy.
    /// </summary>
    internal sealed class FirewallTalent : SkillTalent<FirewallSkill>
    {
        /// <summary>
        /// The base cooldown of Firewall (in seconds).
        /// </summary>
        public const float Cooldown = 20.0f;

        /// <summary>
        /// The mana cost of the Firewall talent.
        /// </summary>
        public const float ManaCostOfBaseMana = 0.3f;

        /// <summary>
        /// The additional mana cost of the Firewall talent.
        /// </summary>
        public const float ManaCostOfTotalMana = 0.06f;

        /// <summary>
        /// The cast time of Firewall
        /// </summary>
        public const float CastTime = 1.06f;

        /// <summary>
        /// States how often Firewall can be cast before it goes on cooldown.
        /// </summary>
        public const int TimesCastableBeforeCooldown = 3;

        /// <summary>
        /// The factor applied to enemy movement speed to calculate the push force.
        /// </summary>
        public const float PushFactor = 0.6f;

        /// <summary>
        /// Gets the minimum damage modifier of Firewall of the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cast time for the given talent level.
        /// </returns>
        private static float GetMinimumDamageModifier( int level )
        {
            return 0.20f + (level * 0.02f);
        }

        /// <summary>
        /// Gets the maximum damage modifier of Firewall of the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cast time for the given talent level.
        /// </returns>
        private static float GetMaximumDamageModifier( int level )
        {
            return 0.2f + (level * 0.05f);
        }
             
        /// <summary>
        /// Gets the minimum damage modifier of Firewall.
        /// </summary>
        public float MinumumDamageModifier
        {
            get
            {
                return GetMinimumDamageModifier( this.Level );
            }
        }

        /// <summary>
        /// Gets the maximum damage modifier of Firewall.
        /// </summary>
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
                TalentResources.TD_Firewall,
                (100.0f * GetMinimumDamageModifier( level )).ToString( culture ),
                (100.0f * GetMaximumDamageModifier( level )).ToString( culture ),
                Cooldown.ToString( culture ),
                TimesCastableBeforeCooldown.ToString( culture ),
                CastTime.ToString( culture )
            );
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FirewallTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirewallTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Firewall,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Firewall" ),
                3,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FirewallTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( PyromaniaTalent ) ), 2 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( PiercingFireTalent ) ),
                this.Tree.GetTalent( typeof( FlamesOfPhlegethonTalent ) )
            };

            SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Creates the Skill object of this FirewallTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {            
            return new FirewallSkill(
                this,
                this.Tree.GetTalent<PiercingFireTalent>(),
                this.ServiceProvider
            );
        }
    }
}
