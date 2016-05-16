// <copyright file="PyromaniaTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Magic.PyromaniaTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Magic
{
    using System.Collections.Generic;
    using Zelda.Skills;
    using Zelda.Skills.Magic;

    /// <summary>
    /// Reduces the cooldown of all offensive Fire spells
    /// by 10/20/30/40/50% for 15 seconds.
    /// </summary>
    internal sealed class PyromaniaTalent : SkillTalent<PyromaniaSkill>
    {
        /// <summary>
        /// The duration of the Pyromania effect.
        /// </summary>
        public const float Duration = 15.0f;

        /// <summary>
        /// The cooldown of the Pyromania skill.
        /// </summary>
        public const float Cooldown = 180.0f;

        /// <summary>
        /// Gets the cooldown reduction in % provided by the Pyromana effect
        /// for the given talent level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The reduction in percent.
        /// </returns>
        private float GetCooldownReductionInPercent( int level )
        {
            return level * 10.0f;
        }

        /// <summary>
        /// Gets the SkillTalents that are affected by the PyromaniaTalent.
        /// </summary>
        /// <returns>
        /// An list of SkillTalent whose skill cooldowns are affected
        /// by the PyromaniaTalent.
        /// </returns>
        public IEnumerable<SkillTalent> GetAffectedSkillTalents()
        {
            return new SkillTalent[4] {
                this.Tree.GetTalent<FirewhirlTalent>(),                
                this.Tree.GetTalent<FirevortexTalent>(),           
                this.Tree.GetTalent<FirewallTalent>(),           
                this.Tree.GetTalent<FlamesOfPhlegethonTalent>()
            };
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
                TalentResources.TD_Pyromania,
                GetCooldownReductionInPercent( level ).ToString( culture ),
                Duration.ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the cooldown reduction provided in percent by the Pyromania effect.
        /// </summary>
        public float CooldownReductionInPercent
        {
            get
            {
                return GetCooldownReductionInPercent( this.Level );
            }
        }
                               
        /// <summary>
        /// Initializes a new instance of the <see cref="PyromaniaTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the RazorWindsTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PyromaniaTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Pyromania,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Pyromania" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections between this PyromaniaTalent and the other talents
        /// in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( FirevortexTalent ) ), 2 )
            };

            Talent[] following = new Talent[1]{
                this.Tree.GetTalent( typeof( FirewallTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Creates the <see cref="Skill"/> object of this SkillTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new PyromaniaSkill( this );
        }
    }
}