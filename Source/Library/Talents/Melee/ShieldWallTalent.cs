// <copyright file="ShieldWallTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.ShieldWallTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Melee
{
    using Zelda.Skills.Melee;

    /// <summary>
    /// The ShieldWallTalent learns the player the ShieldWallSkill.
    /// <para>
    /// Reduces physical damage taken by 15/20/25%.
    /// As a penalty also reduces damage done
    /// by 10% and movement speed by 40%.
    /// </para>
    /// <para>
    /// Shield Wall doesn't cost Mana and is active until deactivated.
    /// </para>
    /// </summary>
    internal sealed class ShieldWallTalent : SkillTalent<ShieldWallSkill>
    {
        #region [ Constants ]

        /// <summary>
        /// The damage done penality that is applied to the player when he uses the ShieldWall skill.
        /// </summary>
        public const float DamageDoneReducement = -10.0f;

        /// <summary>
        /// The movement speed  penality that is applied to the player when he uses the ShieldWall skill.
        /// </summary>
        public const float MovementSpeedReducement = -40.0f;

        /// <summary>
        /// The cooldown of the skill.
        /// </summary>
        public const float Cooldown = 5.0f;

        /// <summary>
        /// Gets the damage taken reducement in % provided by the Shield Wall ability.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The damage reduction in %.
        /// </returns>
        private static float GetDamageTakenReducement( int level )
        {
            switch( level )
            {
                case 1:
                    return 15.0f;

                case 2:
                    return 20.0f;

                case 3:
                    return 25.0f;

                case 0:
                default:
                    return 0.0f;
            }
        }

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
                TalentResources.TD_ShieldWall,
                GetDamageTakenReducement( level ).ToString( culture ),
                DamageDoneReducement.ToString( culture ),
                MovementSpeedReducement.ToString( culture )
            );
        }

        /// <summary>
        /// Gets the damage taken reducement provided by the Shield Wall ability.
        /// </summary>
        public float DamageTakenReducement
        {
            get 
            {
                return -GetDamageTakenReducement( this.Level ); 
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldWallTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ShieldWallTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ShieldWallTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_ShieldWall,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ShieldWall" ),
                3,
                tree, 
                serviceProvider 
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ShieldWallTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] { 
                new TalentRequirement( Tree.GetTalent( typeof( VitalityTalent ) ), 3 ) 
            };

            Talent[] followingTalents = null;
            this.SetTreeStructure( requirements, followingTalents );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the Skill object of this ShieldWallTalent.
        /// </summary>
        /// <remarks>
        /// The actual skill objects are only created when really required.
        /// </remarks>
        /// <returns>
        /// The newly created instance of the Skill.
        /// </returns>
        protected override Zelda.Skills.Skill CreateSkill()
        {
            return new ShieldWallSkill( this );
        }

        #endregion
    }
}
