// <copyright file="ShieldBreakerTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.ShieldBreakerTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;

    /// <summary>
    /// The <see cref="ShieldBreakerTalent"/> provides the player
    /// the ShieldBreakerSkill.
    /// <para>
    /// You build up strength in your swords arm after blocking an attack.
    /// Unleashes MeleeDmg + BlockValue * ({0} * Block Points)% damage. 
    /// 1 Block Point is awarded for each block; lasting 15 seconds. 
    /// 5 Block Points maximum.
    /// </para>
    /// </summary>
    internal sealed class ShieldBreakerTalent : SkillTalent<ShieldBreakerSkill>
    {
        /// <summary>
        /// The time in seconds block points last until the vanish completely.
        /// </summary>
        public const float BlockPointDuration = 15.0f;

        /// <summary>
        /// The maximum number of block points the player can have at a time.
        /// </summary>
        public const int MaximumBlockPoints = 5;

        /// <summary>
        /// The cooldown of the ShieldBreakerSkill.
        /// </summary>
        public const float Cooldown = 4.0f;

        /// <summary>
        /// The mana needed to use this skill per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.05f;

        /// <summary>
        /// Gets the BlockValue multiplier applied per block point.
        /// </summary>
        public float BlockValueMultiplierPerBlockPoint
        {
            get
            {
                return GetBlockValueMultiplierPerBlockPoint( this.Level );
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
                TalentResources.TD_ShieldBreaker,
                (GetBlockValueMultiplierPerBlockPoint( level ) * 100.0f).ToString( culture ),
                BlockPointDuration.ToString( culture ),
                MaximumBlockPoints.ToString( culture )
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldBreakerTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ShieldBreakerTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ShieldBreakerTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ShieldBreaker,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ShieldBreaker" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ShieldBreakerTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( VitalityTalent ) ), 1 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( ShieldMasteryTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Gets the BlockValue multiplier used to convert BlockValue into actual damage
        /// for the given talent level.
        /// </summary>
        /// <param name="level">The level of the talent.</param>
        /// <returns>
        /// The multiplier value.
        /// </returns>
        private static float GetBlockValueMultiplierPerBlockPoint( int level )
        {
            return 0.2f + level * 0.1f;
        }

        /// <summary>
        /// Creates the Skill object of the ShieldBreakerTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new ShieldBreakerSkill( this, this.ServiceProvider );
        }
    }
}
