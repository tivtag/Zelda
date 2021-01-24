// <copyright file="BladestormTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.BladestormTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using System;
    using Zelda.Entities.Drawing;
    using Zelda.Skills.Melee;

    /// <summary>
    /// The BladestormTalent provides the player with the BladestormSkill.
    /// <para>
    /// The player goes nuts after using Whirlwind, 
    /// turning for another X times, dealing (MeleeDamage x Y%) 
    /// non-parry nor dodgeable damage.
    /// Compared to Whirlwind movement is allowed with a speed penality of Z%.
    /// </para>
    /// </summary>
    internal sealed class BladestormTalent : SkillTalent<BladestormSkill>
    {
        /// <summary>
        /// The base cooldown of the Skill.
        /// </summary>
        public const int Cooldown = 26;

        /// <summary>
        /// The timeframe after using Whirlwind in which Bladestorm can be used.
        /// </summary>
        public const float TimeUseableAfterWhirlwind = 5.0f;

        /// <summary>
        /// The movement speed reduction applied to the player while Bladestorm is active.
        /// </summary>
        public const float MovementSpeedReduction = -20.0f;

        /// <summary>
        /// The mana needed per point of base mana to use the Skill.
        /// </summary>
        public const float ManaNeededPoBM = 0.1f;
        
        /// <summary>
        /// Gets the duration in seconds the Bladestorm is active.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The duration in seconds.
        /// </returns>
        private static int GetTurns( int level )
        {
            return 3 + level;
        }

        /// <summary>
        /// Gets the number of 'whirlwind' turns the Bladestorm consists of.
        /// </summary>
        public int Turns
        {
            get
            {
                return GetTurns( this.Level );
            }
        }

        /// <summary>
        /// Gets the damage multiplier of the Bladestorm attack.
        /// </summary>
        public float DamageMultiplier
        {
            get
            {
                return 1.0f + ((this.whirlwindTalent.DamageMultiplier - 1.0f) * 0.5f);
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

            float totalTime = this.Owner.DrawDataAndStrategy.GetSpecialAnimationTotalTime(PlayerSpecialAnimation.AttackBladestorm);
            float animationSpeed = GetAnimationSpeed(this.Owner);
            float timePerRound = totalTime / animationSpeed;
            float time = GetTurns(level) * timePerRound;

            return string.Format(
                culture,
                TalentResources.TD_Bladestorm,
                GetTurns( level ).ToString( culture ),
                (DamageMultiplier * 100.0f).ToString( culture ),
                Math.Round( time, 2 ) 
            );
        }
                
        /// <summary>
        /// Initializes a new instance of the <see cref="BladestormTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new BladestormTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BladestormTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Bladestorm,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Bladestorm" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this BladestormTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {            
            this.whirlwindTalent = this.Tree.GetTalent<WhirlwindTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( whirlwindTalent, 3 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }
        
        /// <summary>
        /// Creates the Skill object of this BladestormTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new BladestormSkill( this, this.whirlwindTalent.Skill, this.ServiceProvider );
        }

        public static float GetAnimationSpeed(Zelda.Entities.PlayerEntity player)
        {
            float factor = 2.0f - player.Statable.CastTimeModifier;
            float animationSpeed = 900.0f * factor;
            return animationSpeed;
        }

        /// <summary>
        /// Identifies the WhirlwindTalent that modifies the power of the Bladestorm.
        /// </summary>
        private WhirlwindTalent whirlwindTalent;
    }
}
