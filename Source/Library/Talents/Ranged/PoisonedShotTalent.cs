// <copyright file="PoisonedShotTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.PoisonedShotTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using System;
    using Zelda.Skills.Ranged;
    
    /// <summary>
    /// The PoisonedShotTalent provides the Player with the PoisonedShotSkill.
    /// <para>
    /// PoisonedShot is a ranged attack that does 
    /// (RangedDamage * InstantDamagePenality) plus (RangedDamage*DamageOverTimeMultiplier)
    /// nature damage over time.
    /// The effect also slows down the enemy by X% per TalentLevel.
    /// </para>
    /// </summary>
    internal sealed class PoisonedShotTalent : SkillTalent<PoisonedShotSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The damage penality of the instant damage part of the PoisonedShot attack.
        /// </summary>
        public const float InstantDamagePenality = 0.1f;

        /// <summary>
        /// The slowing effect applied to a poisoned target.
        /// </summary>
        private const float MovementSlowingEffectPerLevel = -10.0f;

        /// <summary>
        /// The amount of 'ticks' the effect lasts.
        /// </summary>
        public const int TickCount = 4;

        /// <summary>
        /// The time in seconds between two 'ticks'.
        /// </summary>
        public const float TickTime = 2.0f;

        /// <summary>
        /// The duration of the effect in seconds.
        /// </summary>
        public const float Duration = (TickCount * TickTime) + 1.0f;

        /// <summary>
        /// The time in seconds it takes for the attack to cool down
        /// and be reuseable again.
        /// </summary>
        public const float Cooldown = 15.0f;

        /// <summary>
        /// The mana needed per point of base mana to use the Skill.
        /// </summary>
        public const float ManaNeededPoBM = 0.2f;

        /// <summary>
        /// Gets the multiplier value that is applied to ranged damage to calculate
        /// the total Damage Over Time nature damage inflincted by Poisoned Shot.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The multiplier value.
        /// </returns>
        private static float GetDamageOverTimeMultiplier( int level )
        {
            return 1.0f + (level * 0.5f);
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
                TalentResources.TD_PoisonedShot,
                Math.Round( InstantDamagePenality, 2 ).ToString( culture ),
                GetDamageOverTimeMultiplier( level ).ToString( culture ),
                Duration.ToString( culture ),
                (MovementSlowingEffectPerLevel * level).ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the damage multiplier that is used to calculate 
        /// the DoneOverTime damage partof the PoisonedShot attack.
        /// </summary>
        public float DamageOverTimeMultiplier
        {
            get
            { 
                return GetDamageOverTimeMultiplier( this.Level ); 
            }
        }

        /// <summary>
        /// Gets the movement slowing effect applied to a poisoned target.
        /// </summary>
        public float MovementSlowingEffect
        {
            get 
            { 
                return MovementSlowingEffectPerLevel * this.Level;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PoisonedShotTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new PoisonedShotTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PoisonedShotTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_PoisonedShot,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_PoisonedShot" ),
                3,
                tree,
                serviceProvider
            )
        {
        }
        
        /// <summary>
        /// Setups the connections of this PoisonedShotTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( PiercingArrowsTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this PoisonedShotTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new PoisonedShotSkill( this, this.ServiceProvider );
        }

        #endregion
    }
}
