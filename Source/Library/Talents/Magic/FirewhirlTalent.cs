// <copyright file="FirewhirlTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.FirewhirlTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Magic
{
    using Zelda.Skills.Magic;

    /// <summary>
    /// Firewhirl deals 103/106/109/112/115% to 105/110/115/120/125% fire damage.
    /// 0.9/0.8/0.7/0.6/0.5 seconds cast time.
    /// 4 seconds cooldown.
    /// </summary>
    /// <remarks>
    /// Firewhirl can be enhanced using the <see cref="CorrosiveFireTalent"/>.
    /// </remarks>
    internal sealed class FirewhirlTalent : SkillTalent<FirewhirlSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The base cooldown of Firewhirl (in seconds).
        /// </summary>
        private const float BaseCooldown = 4.0f;

        /// <summary>
        /// The mana cost of the Firewhirl talent.
        /// </summary>
        public const float ManaCostOfBaseMana = 0.2f;

        /// <summary>
        /// The additional mana cost of the Firewhirl talent.
        /// </summary>
        public const float ManaCostOfTotalMana = 0.05f;

        /// <summary>
        /// Gets the cast time of Firewhirl of the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cast time for the given talent level.
        /// </returns>
        private static float GetCastTime( int level )
        {
            return 1.0f - (level * 0.1f);
        }

        /// <summary>
        /// Gets the minimum damage modifier of Firewhirl of the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cast time for the given talent level.
        /// </returns>
        private static float GetMinimumDamageModifier( int level )
        {
            return 1.0f + (level * 0.03f);
        }

        /// <summary>
        /// Gets the maximum damage modifier of Firewhirl of the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The cast time for the given talent level.
        /// </returns>
        private static float GetMaximumDamageModifier( int level )
        {
            return 1.0f + (level * 0.05f);
        }

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the cast time of Firewhirl.
        /// </summary>
        public float CastTime
        {
            get
            {
                return GetCastTime( this.Level );
            }
        }

        /// <summary>
        /// Gets the cooldown of the Firewhirl skill.
        /// </summary>
        public float Cooldown
        {
            get
            {
                return BaseCooldown - this.swiftFightStyleTalent.FirewhirlCooldownReduction;
            }
        }

        /// <summary>
        /// Gets the minimum damage modifier of Firewhirl.
        /// </summary>
        public float MinumumDamageModifier
        {
            get
            {
                return GetMinimumDamageModifier( this.Level );
            }
        }

        /// <summary>
        /// Gets the maximum damage modifier of Firewhirl.
        /// </summary>
        public float MaximumDamageModifier
        {
            get
            {
                return GetMaximumDamageModifier( this.Level );
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
                TalentResources.TD_Firewhirl,
                (GetMinimumDamageModifier( level ) * 100).ToString( culture ),
                (GetMaximumDamageModifier( level ) * 100).ToString( culture ),
                GetCastTime( level ).ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewhirlTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FirewhirlTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirewhirlTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Firewhirl,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Firewhirl" ),
                5,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FirewhirlTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.swiftFightStyleTalent = this.Tree.GetTalent<Zelda.Talents.Support.SwiftFightStyleTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( MagicTrainingTalent ) ), 2 )
            };

            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( FirevortexTalent ) ),
                this.Tree.GetTalent( typeof( CorrosiveFireTalent ) ),
                this.Tree.GetTalent( typeof( ImpactTheoryTalent ) )
            };

            SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this FirewhirlTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            var impactTheoryTalent = this.Tree.GetTalent<ImpactTheoryTalent>();
            var corrosiveFireTalent = this.Tree.GetTalent<CorrosiveFireTalent>();
            
            return new FirewhirlSkill(
                this, 
                impactTheoryTalent, 
                corrosiveFireTalent, 
                this.ServiceProvider
            );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the SwiftFightStyleTalent which reduces the cooldown of this FirewhirlTalent.
        /// </summary>
        private Zelda.Talents.Support.SwiftFightStyleTalent swiftFightStyleTalent;

        #endregion
    }
}
