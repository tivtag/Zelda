// <copyright file="MultiShotTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.MultiShotTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Skills.Ranged;
    
    /// <summary>
    /// The MultiShotTalent provides the player the MultiShotSkill.
    /// MultiShot is an instant attack that releases (3 + 1 per level) arrows at the same time.
    /// </summary>
    internal sealed class MultiShotTalent : SkillTalent<MultiShotSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The damage reduction applied compared to a normal ranged attacks.
        /// </summary>
        public const float DamageReduction = 1.0f / 3.0f;

        /// <summary>
        /// The time in seconds it takes for the attack to cooldown
        /// and be reuseable again.
        /// </summary>
        public const float Cooldown = 13.0f;

        /// <summary>
        /// The mana needed per point of base mana.
        /// </summary>
        public const float ManaNeededPoBM = 0.01f;

        /// <summary>
        /// The number of arrows released as a base-line.
        /// </summary>
        private const int BaseArrowCount = 3;

        /// <summary>
        /// The number of arrows added to the attack for each talent-level.
        /// </summary>
        private const int ArrowIncreasePerLevel = 1;

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
                TalentResources.TD_MultiShot,
                (BaseArrowCount + (ArrowIncreasePerLevel * level)).ToString( culture ),
                System.Math.Round(100.0f * DamageReduction, 1).ToString( culture ) 
            );
        }

        /// <summary>
        /// Gets a value that represents the number of arrows released by the MultiShot attack.
        /// </summary>
        public int ArrowCount
        {
            get { return BaseArrowCount + (ArrowIncreasePerLevel * this.Level); }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiShotTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new MultiShotTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MultiShotTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_MultiShot,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_MultiShot" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this MultiShotTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[] {
                new TalentRequirement( this.Tree.GetTalent( typeof( LightArrowTalent ) ), 2 )
            };

            Talent[] following = new Talent[] {                                
                this.Tree.GetTalent( typeof( ArrowShowerTalent ) ),
                this.Tree.GetTalent( typeof( ImprovedMultiShotTalent ) ),
                this.Tree.GetTalent( typeof( ArrowRushTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this MultiShotTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Skills.Skill CreateSkill()
        {
            var improvedTalent = (ImprovedMultiShotTalent)this.Tree.GetTalent( typeof( ImprovedMultiShotTalent ) );
            return new MultiShotSkill( this, improvedTalent, this.ServiceProvider );
        }

        #endregion
    }
}
