// <copyright file="MeditationTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.MeditationTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Status;
    
    /// <summary>
    /// The <see cref="MeditationTalent"/> provides the Player
    /// with a passive effect that increases Mana Regeneration
    /// by 2/4/6/8/10 and 3/6/9/12/15%.
    /// </summary>
    internal sealed class MeditationTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The fixed mana regeneration increase provided by the Talent per talent level.
        /// </summary>
        private const float FixedManaRegenIncreasePerLevel     = 2.0f;

        /// <summary>
        /// The percentage mana regeneration increase provided by the Talent per talent level.
        /// </summary>
        private const float PercentalManaRegenIncreasePerLevel = 3.0f;

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
                TalentResources.TD_Meditation,
                (FixedManaRegenIncreasePerLevel * level).ToString( CultureInfo.CurrentCulture ),
                (PercentalManaRegenIncreasePerLevel * level).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MeditationTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the MeditationTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MeditationTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_Meditation,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Meditation" ),
                5,
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
                new TalentRequirement( this.Tree.GetTalent( typeof( ConcentrateOnTheFactsTalent ) ), 2 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( PotionMasteryTalent ) ),
                this.Tree.GetTalent( typeof( ManaHullTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this MeditationTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effectFixed     = new LifeManaRegenEffect( 0.0f, StatusManipType.Fixed, LifeMana.Mana );
            this.effectPercental = new LifeManaRegenEffect( 0.0f, StatusManipType.Percental, LifeMana.Mana );

            this.aura = new PermanentAura( new StatusEffect[2] { effectFixed, effectPercental } ) {
                Name = "Meditation_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this MeditationTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.effectFixed = null;
            this.effectPercental = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this MeditationTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );

            this.effectFixed.Value     = this.Level * FixedManaRegenIncreasePerLevel;
            this.effectPercental.Value = this.Level * PercentalManaRegenIncreasePerLevel;

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive StatusEffects this MedidationTalent provides.
        /// </summary>
        private LifeManaRegenEffect effectFixed, effectPercental;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this MeditationTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
