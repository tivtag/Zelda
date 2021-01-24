// <copyright file="PiercingArrowsTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.PiercingArrowsTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Status;

    /// <summary>
    /// The PiercingArrowsTalent increases the chance 
    /// projectiles pierce through enemies.
    /// </summary>
    /// <remarks>
    /// The chance is increased by 3% per talent level.
    /// </remarks>
    internal sealed class PiercingArrowsTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The chance to pierce with ranged attacks increase the Talent provides per talent level.
        /// </summary>
        private const float ChanceToPiereIncreasePerLevel = 3.0f;

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
                TalentResources.TD_PiercingArrows,
                (level * ChanceToPiereIncreasePerLevel).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PiercingArrowsTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new PiercingArrowsTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PiercingArrowsTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_PiercingArrows,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Pierce" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this PiercingArrowsTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( RangedTrainingTalent ) ), 4 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( PoisonedShotTalent ) ),
                this.Tree.GetTalent( typeof( RapidFireTalent    ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this PiercingArrowsTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Pierce );
            this.aura = new PermanentAura( effect ) {
                Name = "PiercingArrows"
            };
        }

        /// <summary>
        /// Uninitializes this PiercingArrowsTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.effect = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this PiercingArrowsTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = ChanceToPiereIncreasePerLevel * this.Level;
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ChanceToStatusEffect this PiercingArrowsTalent provides.
        /// </summary>
        private ChanceToStatusEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the Status Effects this PiercingArrowsTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}