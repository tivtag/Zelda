// <copyright file="PoisonMasteryTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.PoisonMasteryTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// The <see cref="PotionMasteryTalent"/> provides the Player
    /// with a passive effect that increases poison damage done by 15%/25%/35%.
    /// </summary>
    internal sealed class PoisonMasteryTalent : Talent
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
            return 5.0f + (talentLevel * 10.0f);
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
                TalentResources.TD_PoisonMastery,
                GetEffectiviness( level ).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PoisonMasteryTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the PotionMasteryTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PoisonMasteryTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_PoisonMastery,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_PoisonMastery" ),
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
                new TalentRequirement( this.Tree.GetTalent( typeof( PotionMasteryTalent ) ), 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this PoisonMasteryTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new SpecialDamageDoneEffect( 0.0f, StatusManipType.Percental, SpecialDamageType.Poison );
            this.aura = new PermanentAura( this.effect ) {
                Name = "PoisonMastery_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this PoisonMasteryTalent.
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
        /// Refreshes the strength of this PoisonMasteryTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = GetEffectiviness( this.Level );
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive effect this PoisonMasteryTalent provides.
        /// </summary>
        private SpecialDamageDoneEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this PoisonMasteryTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
