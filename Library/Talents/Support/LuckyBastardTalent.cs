// <copyright file="LuckyBastardTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.LuckyBastardTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Status;
    
    /// <summary>
    /// The <see cref="LuckyBastardTalent"/> provides the Player
    /// with a passive effect that increases the chance to find
    /// rare items by 5%/10%/15%. (MF)
    /// </summary>
    internal sealed class LuckyBastardTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The percentage mana regeneration increase provided by the Talent per talent level.
        /// </summary>
        private const float PercentalMagicFindIncreasePerLevel = 5.0f;

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
                TalentResources.TD_LuckyBastard,
                (PercentalMagicFindIncreasePerLevel * level).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LuckyBastardTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new LuckyBastardTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal LuckyBastardTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_LuckyBastard,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_LuckyBastard" ),
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
                new TalentRequirement( this.Tree.GetTalent( typeof( ConcentrateOnTheFactsTalent ) ), 3 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( AngelEmbracementTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this LuckyBastardTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new MagicFindEffect( 0.0f, StatusManipType.Percental );
            this.aura = new PermanentAura( effect ) {
                Name = "LuckyBastard_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this LuckyBastardTalent.
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
        /// Refreshes the strength of this LuckyBastardTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = this.Level * PercentalMagicFindIncreasePerLevel;
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive effect this LuckyBastardTalent provides.
        /// </summary>
        private MagicFindEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this LuckyBastardTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
