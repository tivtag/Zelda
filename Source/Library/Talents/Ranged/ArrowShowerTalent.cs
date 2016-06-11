// <copyright file="ArrowShowerTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.ArrowShowerTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;

    /// <summary>
    /// The ArrowShowerTalent learns the player the ArrowShowerSkill.
    /// <para>
    /// 'You fire one Multi Shot every X seconds for Y seconds.'
    /// </para>
    /// </summary>
    internal sealed class ArrowShowerTalent : SkillTalent<ArrowShowerSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The time in seconds it takes for the attack to cooldown
        /// and be reuseable again.
        /// </summary>
        public const float Cooldown = 60.0f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.175f;

        /// <summary>
        /// The time in seconds between two Multi Shots.
        /// </summary>
        public const float TimeBetweenMultiShots = 0.5f;

        /// <summary>
        /// The base number of multi shots.
        /// </summary>
        private const int BaseAttackCount = 6;

        /// <summary>
        /// The number of
        /// </summary>
        private const int AttackCountPerLevel = 1;

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
                TalentResources.TD_ArrowShower,
                TimeBetweenMultiShots.ToString( culture ),
                ((BaseAttackCount + (AttackCountPerLevel * level)) * TimeBetweenMultiShots ).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the number of Multi Shots the Arrow Shower unleashes.
        /// </summary>
        public int AttackCount
        {
            get { return BaseAttackCount + (AttackCountPerLevel * this.Level); }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowShowerTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ArrowShowerTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ArrowShowerTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_ArrowShower,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ArrowShower" ),
                3,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ArrowShowerTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[2] {
                new TalentRequirement( this.Tree.GetTalent( typeof( MultiShotTalent ) ), 2 ),                
                new TalentRequirement( this.Tree.GetTalent( typeof( RapidFireTalent ) ), 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this ArrowShowerTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            var multiShotTalent = this.Tree.GetTalent<MultiShotTalent>();
            return new ArrowShowerSkill( this, multiShotTalent, this.ServiceProvider );
        }

        #endregion
    }
}
