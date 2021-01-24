// <copyright file="PotionMasteryTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Support.PotionMasteryTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Status;

    /// <summary>
    /// The <see cref="PotionMasteryTalent"/> provides the Player
    /// with a passive effect that increases the effectiviness
    /// of healing and mana potions by 10%/20%/30%.
    /// </summary>
    internal sealed class PotionMasteryTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the potion effectivines increase provided
        /// by this PotionMasteryTalent.
        /// </summary>
        /// <param name="talentLevel">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The effectivines increase in percent.
        /// </returns>
        private static float GetEffectiviness( int talentLevel )
        {
            return (talentLevel * 10.0f);
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
                TalentResources.TD_PotionMastery,
                GetEffectiviness( level ).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PotionMasteryTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the PotionMasteryTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PotionMasteryTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_PotionMastery,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_PotionMastery" ),
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
                new TalentRequirement( this.Tree.GetTalent( typeof( MeditationTalent ) ), 1 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( PoisonMasteryTalent ) ),
                this.Tree.GetTalent( typeof( RackingPainsTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this PotionMasteryTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effectLife = new LifeManaPotionEffectivenessEffect( 0.0f, LifeMana.Life );
            this.effectMana = new LifeManaPotionEffectivenessEffect( 0.0f, LifeMana.Mana );

            this.aura = new PermanentAura( new StatusEffect[2] { effectLife, effectMana } )  {
                Name = "PotionMastery_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this PotionMasteryTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.effectLife = null;
            this.effectMana = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this PotionMasteryTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effectMana.Value = this.effectLife.Value = GetEffectiviness( this.Level );
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive StatusEffects this PotionMasteryTalent provides.
        /// </summary>
        private LifeManaPotionEffectivenessEffect effectLife, effectMana;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this PotionMasteryTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
