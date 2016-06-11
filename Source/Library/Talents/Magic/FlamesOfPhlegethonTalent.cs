// <copyright file="FlamesOfPhlegethonTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.FlamesOfPhlegethonTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Magic
{
    using Zelda.Skills.Magic;

    /// <summary>
    /// Summons {0} wave(s) of fire directly from the underworld.
    /// Targets that are hit take {0}% to {1}% fire damage.
    /// {1} secs cooldown. {2} secs cast time.
    /// </summary>
    internal sealed class FlamesOfPhlegethonTalent : SkillTalent<FlamesOfPhlegethonSkill>
    {
        /// <summary>
        /// The base cooldown of Flames of Phlegethon (in seconds).
        /// </summary>
        public const float Cooldown = 60.0f;

        /// <summary>
        /// The mana cost of the Flames of Phlegethon talent.
        /// </summary>
        public const float ManaCostOfBaseMana = 0.50f;

        /// <summary>
        /// The additional mana cost of the Flames of Phlegethon talent.
        /// </summary>
        public const float ManaCostOfTotalMana = 0.25f;

        /// <summary>
        /// The cast time of FlamesOfPhlegethon
        /// </summary>
        public const float CastTime = 3.3f;

        /// <summary>
        /// Gets the number of waves that spawn.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The number of waves.
        /// </returns>
        private static int GetWaveCount( int level )
        {
            return 1 + level;
        }

        /// <summary>
        /// Gets the minimum damage modifier of Flames of Phlegethon.
        /// </summary>
        public float MinumumDamageModifier
        {
            get
            {
                return 0.25f;
            }
        }

        /// <summary>
        /// Gets the maximum damage modifier of Flames of Phlegethon.
        /// </summary>
        public float MaximumDamageModifier
        {
            get
            {
                return 0.5f;
            }
        }

        /// <summary>
        /// Gets the number of waves the Flames of Phlegethon consists of.
        /// </summary>
        public int WaveCount
        {
            get
            {
                return GetWaveCount( this.Level );
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
                TalentResources.TD_FlamesOfPhlegethon,
                GetWaveCount( level ).ToString( culture ),
                (100.0f * this.MinumumDamageModifier).ToString( culture ),
                (100.0f * this.MaximumDamageModifier).ToString( culture ),
                Cooldown.ToString( culture ),
                CastTime.ToString( culture )
            );
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FlamesOfPhlegethonTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new Flames of PhlegethonTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FlamesOfPhlegethonTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_FlamesOfPhlegethon,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Firebolt" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this Flames of PhlegethonTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( FirewallTalent ) ), 2 )
            };

            Talent[] following = null;
            SetTreeStructure( requirements, following );
        }
        
        /// <summary>
        /// Creates the Skill object of this Flames of PhlegethonTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new FlamesOfPhlegethonSkill(
                this,
                this.Tree.GetTalent<PiercingFireTalent>(),
                this.ServiceProvider
            );
        }
    }
}