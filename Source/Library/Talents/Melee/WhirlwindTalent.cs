// <copyright file="WhirlwindTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.WhirlwindTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Melee
{
    using System;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    using Zelda.Skills.Melee;

    /// <summary>
    /// The WhirlwindTalent provides the player with the WhirlwindSkill.
    /// <para>
    /// Whirlwind is a powerful attack that needs to be charged up.
    /// After the charge is complete the player turns like a Whirlwind,
    /// hitting and pushing all enemies extremly hard. (40% increased damage)
    /// </para>
    /// </summary>
    internal sealed class WhirlwindTalent : SkillTalent<WhirlwindSkill>
    {
        /// <summary>
        /// The damage increase in % per talent level.
        /// </summary>
        private const float DamageIncreaseInPercentPerLevel = 8.0f;

        /// <summary>
        /// The base cooldown of the Skill.
        /// </summary>
        private const int BaseCooldown = 21;

        /// <summary>
        /// The cooldown reduction per talent level.
        /// </summary>
        private const int CooldownReductionPerLevel = 1;

        /// <summary>
        /// The mana needed per point of base mana to use the Skill.
        /// </summary>
        public const float ManaNeededPoBM = 0.25f;
                
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

            float totalTime = this.Owner.DrawDataAndStrategy.GetSpecialAnimationTotalTime(PlayerSpecialAnimation.AttackWhirlwind);
            float animationSpeed = GetAnimationSpeed(this.Owner);
            float time =  totalTime / animationSpeed;

            return string.Format(
                culture,
                TalentResources.TD_Whirlwind,
                (100.0f + (DamageIncreaseInPercentPerLevel * level)).ToString( culture ),
                (BaseCooldown - (CooldownReductionPerLevel * level)).ToString( culture ),
                Math.Round( time, 2 )
            );
        }

        /// <summary>
        /// Gets the damage multiplier of the Whirlwind Attack.
        /// </summary>
        public float DamageMultiplier
        {
            get
            {
                return 1.0f + (DamageIncreaseInPercentPerLevel * Level / 100.0f);
            }
        }

        /// <summary>
        /// Gets the cooldown of the Whirlwind Skill.
        /// </summary>
        public float Cooldown
        {
            get
            {
                return BaseCooldown - (CooldownReductionPerLevel * Level);
            }
        }
                
        /// <summary>
        /// Initializes a new instance of the <see cref="WhirlwindTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new WhirlwindTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal WhirlwindTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_Whirlwind,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Whirlwind" ),
                5,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this WhirlwindTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[2] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ImprovedBashTalent ) ), 2 ),
                new TalentRequirement( this.Tree.GetTalent( typeof( EnrageTalent       ) ), 3 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( BladestormTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }
        
        /// <summary>
        /// Refreshes the power of the Skill that is related to this SkillTalent.
        /// </summary>
        public override void RefreshSkill()
        {
            // The power of Whirlwind partially moves to Bladestorm.
            this.Tree.GetTalent<BladestormTalent>().RefreshSkill();

            base.RefreshSkill();
        }

        /// <summary>
        /// Creates the Skill object of this WhirlwindTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new WhirlwindSkill( this, this.ServiceProvider );
        }

        public static float GetAnimationSpeed(PlayerEntity player)
        {
            float factor = 2.0f - player.Statable.CastTimeModifier;
            float animationSpeed = 1000.0f * factor;
            return animationSpeed;
        }
    }
}
