// <copyright file="CounterAttackTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.CounterAttackTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The CounterAttackTalent provides the player the CounterAttackSkill.
    /// <para>
    /// Counter Attack gets useable for 5 seconds after parrying an attack.
    /// It hits the enemy for MeleeDamage + 11%/22%/33% damage.
    /// </para>
    /// </summary>
    internal sealed class CounterAttackTalent : SkillTalent<CounterAttackSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The cooldown of the CounterAttackSkill in seconds.
        /// </summary>
        public const float Cooldown = 10.0f;

        /// <summary>
        /// The time the CounterAttackSkill is useable for after parrying (in seconds).
        /// </summary>
        public const float UseableTime = 6.0f;        

        /// <summary>
        /// The damage increase in percent per talent level.
        /// </summary>
        private const float DamageIncreasePerLevel = 11.0f;

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the damage increase provided by the CounterAttackTalent.
        /// </summary>
        public float DamageIncreaseMultiplier
        {
            get
            {
                return 1.0f + ((this.Level * DamageIncreasePerLevel) / 100.0f);
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
                TalentResources.TD_CounterAttack,
                (100.0 + (level * DamageIncreasePerLevel)).ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterAttackTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new CounterAttackTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal CounterAttackTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_CounterAttack,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_CounterAttack" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this CounterAttackTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( BattleAwarenessTalent ) ), 3 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this ParryStrikeTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new CounterAttackSkill( this, this.ServiceProvider );
        }

        #endregion
    }
}