// <copyright file="ShieldMasteryTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.ShieldMasteryTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Zelda.Status;

    /// <summary>
    /// The ShieldMasteryTalent increases Block Chance by 5/10/15% and Block Value by 15/25/35%.
    /// </summary>
    internal sealed class ShieldMasteryTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the %-Block Value increase given by the ShieldMasteryTalent of the given level.
        /// </summary>
        /// <param name="level">The level of the talent.</param>
        /// <returns>The block chance in %.</returns>
        private static float GetPercentualBlockValue( int level )
        {
            return 5.0f + (level * 10.0f);
        }

        /// <summary>
        /// Gets the increased Chance to Block given by the ShieldMasteryTalent of the given level.
        /// </summary>
        /// <param name="level">The level of the talent.</param>
        /// <returns>The block chance in %.</returns>
        private static float GetFixedBlockChance( int level )
        {
            return level * 5.0f;
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
                TalentResources.TD_ShieldMastery,
                GetFixedBlockChance( level ).ToString( culture ),
                GetPercentualBlockValue( level ).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldMasteryTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ShieldMasteryTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ShieldMasteryTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ShieldMastery,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ShieldMastery" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this ShieldMasteryTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ShieldBreakerTalent ) ), 1 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( ShieldBlockTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this ShieldMasteryTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.fixedBlockChanceEffect = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Block );
            this.percentualBlockValueEffect = new BlockValueEffect( 0.0f, StatusManipType.Percental );

            this.aura = new PermanentAura( new StatusEffect[] { this.fixedBlockChanceEffect, this.percentualBlockValueEffect } ) {
                Name = this.GetType().ToString() + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this ShieldMasteryTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.fixedBlockChanceEffect = null;
            this.percentualBlockValueEffect = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this ShieldMasteryTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );

            this.fixedBlockChanceEffect.Value = GetFixedBlockChance( this.Level );
            this.percentualBlockValueEffect.Value = GetPercentualBlockValue( this.Level );

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ChanceToStatusEffect this ShieldMasteryTalent provides.
        /// </summary>
        private ChanceToStatusEffect fixedBlockChanceEffect;

        /// <summary>
        /// Identifies the BlockValueEffect this ShieldMasteryTalent provides.
        /// </summary>
        private BlockValueEffect percentualBlockValueEffect;

        /// <summary>
        /// Identifies the PermanentAura that holds the StatusValueEffects this ShieldMasteryTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
