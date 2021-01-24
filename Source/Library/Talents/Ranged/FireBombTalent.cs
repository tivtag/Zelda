// <copyright file="FireBombTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.FireBombTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;

    /// <summary>
    /// The FireBombTalent provides the player access to the FireBombSkill.
    /// <para>
    /// Plants a bomb that deals (35/55/75% Ranged + 60% Magic) damage on explosion
    /// and knocks all hit targets away.
    /// </para>
    /// </summary>
    internal sealed class FireBombTalent : SkillTalent<FireBombSkill>
    {
        /// <summary>
        /// The time in seconds it takes for the attack to cooldown
        /// and be reuseable again.
        /// </summary>
        public const float Cooldown = 12.5f;
        
        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.225f;

        /// <summary>
        /// The radius of the FireBomb's explosion.
        /// </summary>
        public const float ExplosionRadius = 35.0f;

        /// <summary>
        /// The pushing power of the FireBomb's explosion.
        /// </summary>
        public const float ExplosionPushingPower = 180.0f;

        /// <summary>
        /// The amount of damage that comes from ranged damage in %.
        /// </summary>
        private const float RangedDamageContributionPerLevel = 0.20f;

        /// <summary>
        /// The amount of damage that comes from magic damage in %.
        /// </summary>
        private const float BaseMagicDamageContribution = 0.55f;
        
        /// <summary>
        /// Gets the amount of damage that comes from ranged damage in %.
        /// </summary>
        /// <param name="level">The level of the talent.</param>
        /// <returns>
        /// The damage contribution from ranged damage.
        /// </returns>
        private static float GetRangedDamageContribution( int level )
        {
            return 0.2f + (RangedDamageContributionPerLevel * level);
        }

        /// <summary>
        /// Gets the amount of damage that comes from ranged damage in %.
        /// </summary>
        public float RangedMagicDamageContribution
        {
            get
            {
                return GetRangedDamageContribution( this.Level );
            }
        }

        /// <summary>
        /// Gets the amount of damage that comes from magic damage in %.
        /// </summary>
        public float MagicDamageContribution
        {
            get
            {
                return BaseMagicDamageContribution; 
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
                TalentResources.TD_FireBomb,
                (100.0f * GetRangedDamageContribution( level ) ).ToString( culture ),
                (100.0f * BaseMagicDamageContribution).ToString( culture )
            );
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FireBombTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FireBombTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FireBombTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_FireBomb,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_FireBomb" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FireBombTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[2] {
                new TalentRequirement( this.Tree.GetTalent<QuickHandsTalent>(), 2 ),
                new TalentRequirement( this.Tree.GetTalent<HandEyeCoordinationTalent>(), 2 ),
            };

            Talent[] following = new Talent[] {
               this.Tree.GetTalent<FireBombChainTalent>()
            };
            this.SetTreeStructure( requirements, following );
        }
                
        /// <summary>
        /// Creates the Skill object of the FireBombTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new FireBombSkill( this, this.Tree.GetTalent<FireBombChainTalent>(), this.ServiceProvider );
        }
    }
}
