// <copyright file="PiercingFireTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Magic.PiercingFireTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Magic
{    
    /// <summary>
    /// 'Piercing Fire' increases chance to crit with Firewall and
    /// Flames of Phlegethon by 3% / 7% / 10%.
    /// </summary>
    internal sealed class PiercingFireTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static float GetCritChanceIncrease( int level )
        {
            switch( level )
            {
                case 1:
                    return 3.0f;

                case 2:
                    return 7.0f;

                case 3:
                    return 10.0f;

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
                TalentResources.TD_PiercingFire,
                GetCritChanceIncrease( level ).ToString( culture )
            );
        }

        /// <summary>
        /// Gets the crit increase this PiercingFireTalent provides to the FirewallSkill and 
        /// FlamesOfPhlegethonSkill.
        /// </summary>
        public float CritChanceIncrease
        {
            get
            {
                return GetCritChanceIncrease( this.Level );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PiercingFireTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the PiercingFireTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PiercingFireTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_PiercingFire,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_PiercingFire" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this PiercingFireTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.firewallTalent = this.Tree.GetTalent<FirewallTalent>();

            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.firewallTalent, 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this PiercingFireTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this PiercingFireTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes this PiercingFireTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.firewallTalent.Skill.RefreshDataFromTalents();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the FirewallTalent which gets improved by this PiercingFireTalent.
        /// </summary>
        private FirewallTalent firewallTalent;

        #endregion
    }
}