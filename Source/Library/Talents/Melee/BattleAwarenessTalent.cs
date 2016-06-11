// <copyright file="BattleAwarenessTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.BattleAwarenessTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Melee
{
    using Zelda.Status;

    /// <summary>
    /// The BattleAwarenessTalent increases the chance for the player to parry enemy attacks
    /// by 1/2/3/4/5% + 2/4/6/8/10% total.
    /// </summary>
    internal sealed class BattleAwarenessTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The fixed parry chance increase per talent level.
        /// </summary>
        private const float FixedParryPerLevel = 1.0f;

        /// <summary>
        /// The percentual parry chance increase per talent level.
        /// </summary>
        private const float PercentualParryPerLevel = 2.0f;

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
                TalentResources.TD_BattleAwareness,
                (level * FixedParryPerLevel).ToString( culture ),
                (100.0f + (level * PercentualParryPerLevel)).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleAwarenessTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new BattleAwarenessTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BattleAwarenessTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_BattleAwareness,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_BattleAwareness" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this BattleAwarenessTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ToughnessTalent ) ), 3 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( CounterAttackTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this BattleAwarenessTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.fixedEffect      = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Parry );
            this.percentualEffect = new ChanceToStatusEffect( 0.0f, StatusManipType.Percental, ChanceToStatus.Parry );

            this.aura = new PermanentAura( new StatusEffect[] { fixedEffect, percentualEffect } ) {
                Name = this.GetType().ToString() + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this BattleAwarenessTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.fixedEffect = null;
            this.percentualEffect = null;
            this.aura = null;
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strenght of this BattleAwarenessTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );

            this.fixedEffect.Value      = FixedParryPerLevel * this.Level;
            this.percentualEffect.Value = PercentualParryPerLevel * this.Level;

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ChanceToStatusEffect this BattleAwarenessTalent provides.
        /// </summary>
        private ChanceToStatusEffect fixedEffect, percentualEffect;

        /// <summary>
        /// Identifies the PermanentAura that holds the StatusValueEffect this BattleAwarenessTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
