// <copyright file="FirevortexTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.FirevortexTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Magic
{
    using Zelda.Skills.Magic;

    /// <summary>
    /// Casts a vortex of fire that gains 5/7/10% in size and 
    /// strength every 0.5 seconds. Sucks nearby enemies into it.
    /// 0.8 second cast time. 8 seconds cooldown.
    /// </summary>
    internal sealed class FirevortexTalent : SkillTalent<FirevortexSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The cooldown of Fire Vortex (in seconds).
        /// </summary>
        public const float Cooldown = 8.0f;

        /// <summary>
        /// The cast time of Fire Vortex.
        /// </summary>
        public const float CastTime = 0.8f;

        /// <summary>
        /// The mana cost of Fire Vortex, based on base mana.
        /// </summary>
        public const float ManaCostOfBaseMana = 0.1f;

        /// <summary>
        /// The mana cost of Fire Vortex.
        /// </summary>
        public const float ManaCostOfTotalMana = 0.10f;

        /// <summary>
        /// The time (in seconds) it takes before a Firevortex increases
        /// in strength.
        /// </summary>
        public const float TimeBetweenVortexPowerups = 0.5f;

        /// <summary>
        /// Gets the size and power increase a Fire Vortex receives
        /// every second.
        /// </summary>
        /// <param name="level">
        /// The level of the vortex.
        /// </param>
        /// <returns>
        /// The increase in percent per second.
        /// </returns>
        private float GetPowerAndSizeModifierIncreasePerSecond( int level )
        {
            switch( level )
            {
                case 1:
                    return 0.05f;

                case 2:
                    return 0.07f;

                case 3:
                    return 0.10f;

                default:
                    return 0.0f;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the scaling factor applied when a FireVortex powers-up.
        /// </summary>
        public float ScalingFactorOnPowerUp
        {
            get
            {
                return 1.0f + this.GetPowerAndSizeModifierIncreasePerSecond( this.Level );
            }
        }

        /// <summary>
        /// Gets the damage modifier increase applied when a FireVortex powers-up.
        /// </summary>
        public float DamageIncreaseOnPowerUp
        {
            get
            {
                return this.GetPowerAndSizeModifierIncreasePerSecond( this.Level );
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
                TalentResources.TD_Firevortex,
                (100.0f * GetPowerAndSizeModifierIncreasePerSecond( level )).ToString( culture ),
                CastTime.ToString( culture ),
                Cooldown.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FirevortexTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FirevortexTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirevortexTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Firevortex,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Firevortex" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FireVortexTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( FirewhirlTalent ) ), 3 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( RazorWindsTalent ) ),
                this.Tree.GetTalent( typeof( PyromaniaTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this FirevortexTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            var razorWindsTalent = this.Tree.GetTalent<RazorWindsTalent>();

            return new FirevortexSkill(
                this,
                razorWindsTalent,
                this.ServiceProvider
            );
        }

        #endregion
    }
}
