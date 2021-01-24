// <copyright file="ImprovedMultiShotTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.ImprovedMultiShotTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    /// <summary>
    /// The ImprovedMultiShotTalent reduces the cooldown
    /// of the MultiShotSkill by 2/4/6 seconds.
    /// </summary>
    internal sealed class ImprovedMultiShotTalent : Talent
    { 
        /// <summary>
        /// The cooldown reduction the ImprovedMultiShotTalent provides per level to the MultiShotSkill.
        /// </summary>
        private const float CooldownReductionPerLevel = 2.0f;
        
        /// <summary>
        /// Gets the cooldown reduction this ImprovedMultiShotTalent provides to the MultiShotSkill.
        /// </summary>
        public float CooldownReduction
        {
            get
            {
                return this.Level * CooldownReductionPerLevel;
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
                TalentResources.TD_ImprovedMultiShot,
                (level * CooldownReductionPerLevel).ToString( culture )
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImprovedMultiShotTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ImprovedMultiShotTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ImprovedMultiShotTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ImprovedMultiShot,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ImprovedMultiShot" ),
                3,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ImprovedMultiShotTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( MultiShotTalent ) ), 3 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this ImprovedMultiShotTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this ImprovedMultiShotTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        /// <summary>
        /// Refreshes the strength of this ImprovedMultiShotTalent.
        /// </summary>
        protected override void Refresh()
        {
            var multiShotTalent = (MultiShotTalent)this.Tree.GetTalent( typeof( MultiShotTalent ) );
            multiShotTalent.Skill.RefreshDataFromTalents();
        }
    }
}
