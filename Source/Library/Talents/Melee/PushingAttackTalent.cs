// <copyright file="PushingAttackTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.PushingAttackTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The PushingAttackTalent provides the player the PushingAttackSkill.
    /// <para>
    /// Pushing Attack pushes the enemy away with full power dealing MeleeDamage,
    /// increasing pushing power by X to Y.
    /// </para>
    /// </summary>
    internal sealed class PushingAttackTalent : SkillTalent<PushingAttackSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The cooldown of the attack in seconds.
        /// </summary>
        public const float Cooldown = 15.0f;

        /// <summary>
        /// The base pushing power of the PushingAttack,
        /// not considering in the level of the talent.
        /// </summary>
        private const float BasePushingPower = 65.0f;

        /// <summary>
        /// The minimum pushing power increase per level.
        /// </summary>
        private const float PushingPowerMinimumPerLevel = 5.0f;

        /// <summary>
        /// The maximum pushing power increase per level.
        /// </summary>
        private const float PushingPowerMaximumPerLevel = 16.0f;

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
                TalentResources.TD_PushingAttack,
                (BasePushingPower + (level * PushingPowerMinimumPerLevel)).ToString( culture ),
                (BasePushingPower + (level * PushingPowerMaximumPerLevel)).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the minimum pushing power of the Pushing Attack.
        /// </summary>
        public float PushingPowerMinimum
        {
            get
            {
                return BasePushingPower + (this.Level * PushingPowerMinimumPerLevel);
            }
        }

        /// <summary>
        /// Gets the maximum pushing power of the Pushing Attack.
        /// </summary>
        public float PushingPowerMaximum
        {
            get
            {
                return BasePushingPower + (Level * PushingPowerMaximumPerLevel);
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PushingAttackTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new PushingAttackTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PushingAttackTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_PushingAttack,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_PushingAttack" ),
                3,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this PushingAttackTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( BashTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this PushingAttackTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new PushingAttackSkill( this, ServiceProvider );
        }

        #endregion
    }
}
