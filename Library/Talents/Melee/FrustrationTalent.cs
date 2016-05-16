// <copyright file="FrustrationTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.FrustrationTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;
    
    /// <summary>
    /// The player turns his frustration into anger;
    /// <para>
    /// increasing chance to get a Double Attack by 20/40/60%.
    /// In his anger the player has a 25% chance to hit himself.
    /// Can't proc off of itself.
    /// 1 minute duration, 5 minutes cooldown.
    /// </para>
    /// </summary>
    internal sealed class FrustrationTalent : SkillTalent<FrustrationSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The chance to get a double attack.
        /// </summary>
        public const float ProcChancePerLevel = 15.0f;

        /// <summary>
        /// The chance for the player to hit himself.
        /// </summary>
        public const float SelfHitChance = 25.0f;

        /// <summary>
        /// The damage multiplier applied to self-inflicted damage while under the Frustration effect.
        /// </summary>
        public const float SelfHitDamageMultiplier = 0.25f;

        /// <summary>
        /// The duration of the Frustration effect.
        /// </summary>
        public const float Duration = 60.0f;

        /// <summary>
        /// The cooldown of the Frustration skill.
        /// </summary>
        public const float Cooldown = 300.0f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.15f;

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
                TalentResources.TD_Frustration,
                (ProcChancePerLevel * level).ToString( culture ),
                Duration.ToString( culture ),
                SelfHitChance.ToString( culture ),
                (SelfHitDamageMultiplier * 100.0f).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FrustrationTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FrustrationTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FrustrationTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Frustration,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Frustration" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FrustrationTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] { 
                new TalentRequirement( this.Tree.GetTalent( typeof( DoubleAttackTalent ) ), 3 ) 
            };

            Talent[] followingTalents = null;
            this.SetTreeStructure( requirements, followingTalents );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of the SkillTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            return new FrustrationSkill( this, this.ServiceProvider );
        }

        #endregion
    }
}
