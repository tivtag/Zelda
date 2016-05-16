// <copyright file="HeadshotTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.HeadshotTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;

    /// <summary>
    /// The HeadshotTalent provides the player with the HeadshotSkill.
    /// <para>
    /// Headshot is a swift ranged instant attack that can only be used
    /// after getting a critical ranged attack.
    /// </para>
    /// <para>
    /// You aim for the head after getting a critical attack,
    /// fireing a swift partially armor-piercing arrow that 
    /// has an improved chance to crit and pierce of 10/20/30%.
    /// </para>
    /// </summary>
    internal sealed class HeadshotTalent : SkillTalent<HeadshotSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The chance to crit and pierce the Headshot attack has over
        /// a normal ranged attack per talent level.
        /// </summary>
        private const float CritAndPierceChanceIncreasePerLevel = 10.0f;

        /// <summary>
        /// The time the Headshot attack can be used for after getting a crit.
        /// </summary>
        public const float TimeUseableAfterCrit = 4.0f;

        /// <summary>
        /// The cooldown of the Headshot attack.
        /// </summary>
        public const float Cooldown = 10.0f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.15f;

        /// <summary>
        /// The multiplier used to calculate the speed of Headshot Arrow.
        /// </summary>
        /// <remarks>
        /// 30% faster.
        /// </remarks>
        public const float ArrowSpeedMultiplier = 1.30f;

        /// <summary>
        /// The armor piercing modifier value of a Headshot.
        /// </summary>
        public const float ArmorPiercingModifier = 0.50f;

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the crit chance increase of a Headshot,
        /// compared to a normal ranged attack.
        /// </summary>
        public float CritChanceIncrease
        {
            get
            {
                return GetCritAndPierceChanceIncrease( this.Level );
            }
        }

        /// <summary>
        /// Gets the pierce chance increase of a Headshot,
        /// compared to a normal ranged attack.
        /// </summary>
        public float AdditionalPiercingChance
        {
            get
            {
                return GetCritAndPierceChanceIncrease( this.Level );
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
                TalentResources.TD_Headshot,
                GetCritAndPierceChanceIncrease( level ).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadshotTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new HeadshotTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal HeadshotTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Headshot,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_GftH" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this HeadshotTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( GoForTheHeadTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Calculates the crit chance increase of an Headshot, compared to a normal ranged attack.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The crit chance increase in %.
        /// </returns>
        private static float GetCritAndPierceChanceIncrease( int level )
        {
            return level * CritAndPierceChanceIncreasePerLevel;
        }

        /// <summary>
        /// Creates the Skill object of this HeadshotTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new HeadshotSkill( this, this.ServiceProvider );
        }

        #endregion
    }
}
