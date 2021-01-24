// <copyright file="SmoothedEmblazonmentTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.SmoothedEmblazonmentTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Items;
    using Zelda.Status;
    
    /// <summary>
    /// The <see cref="SmoothedEmblazonmentTalent"/> increases the amount
    /// of stats Rings and Necklaces give to the Player
    /// by 4%/8%/12%.
    /// </summary>
    internal sealed class SmoothedEmblazonmentTalent : Talent
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the stat increase provided
        /// by the SmoothedEmblazonmentTalent.
        /// </summary>
        /// <param name="talentLevel">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The increase in percent.
        /// </returns>
        private static float GetEffectiviness( int talentLevel )
        {
            return 4.0f * talentLevel;
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
            return string.Format(
                CultureInfo.CurrentCulture,
                TalentResources.TD_SmoothedEmblazonment,
                GetEffectiviness( level ).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothedEmblazonmentTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the SmoothedEmblazonmentTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal SmoothedEmblazonmentTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_SmoothedEmblazonment,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_SmoothedEmblazonment" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the talent and its connections with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( CriticalBalanceTalent ) ), 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this SmoothedEmblazonmentTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effectRings = new EquipmentSlotStatModifierEffect( EquipmentSlot.Ring, 0.0f );
            this.effectNecklaces = new EquipmentSlotStatModifierEffect( EquipmentSlot.Necklace, 0.0f );

            this.aura = new PermanentAura( new StatusEffect[2] { this.effectRings, this.effectNecklaces } ) {
                Name = "SmoothedEmblazonment_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this SmoothedEmblazonmentTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.effectRings = null;
            this.effectNecklaces = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes this SmoothedEmblazonmentTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );

            float effectValue = GetEffectiviness( this.Level );
            this.effectRings.Value     = effectValue;
            this.effectNecklaces.Value = effectValue;

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive StatusEffects this SmoothedEmblazonmentTalent provides.
        /// </summary>
        private EquipmentSlotStatModifierEffect effectRings, effectNecklaces;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this SmoothedEmblazonmentTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
