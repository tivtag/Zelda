// <copyright file="QuickHandsTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.QuickHandsTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Status;

    /// <summary>
    /// Increases attack speed and spell haste by 2.5%/5%.
    /// </summary>
    internal sealed class QuickHandsTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The amount of attack speed increased per talent level.
        /// </summary>
        private const float AttackSpeedIncreasePerLevel = 2.5f;

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
                TalentResources.TD_QuickHands,
                (level * AttackSpeedIncreasePerLevel).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickHandsTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new QuickHandsTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal QuickHandsTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_QuickHands,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_QuickHands" ),
                2,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this QuickHandsTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( DodgeTrainingTalent ) ), 1 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( FireBombTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this QuickHandsTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.attackSpeedEffect = new AttackSpeedEffect( Zelda.Attacks.AttackType.All, 0.0f, StatusManipType.Percental );
            this.spellHasteEffect = new SpellHasteEffect( 0.0f, StatusManipType.Percental );

            this.aura = new PermanentAura( new StatusEffect[2] { this.attackSpeedEffect, this.spellHasteEffect } );
        }

        /// <summary>
        /// Uninitializes this QuickHandsTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.attackSpeedEffect = null;
            this.spellHasteEffect = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the power of the QuickHandsTalent effect.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            
            // Apply new value.
            float effect = this.Level * AttackSpeedIncreasePerLevel;
            attackSpeedEffect.Value  = -effect;
            spellHasteEffect.Value = effect;

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The aura that contains the passive effect of the Talent.
        /// </summary>
        private PermanentAura aura;

        /// <summary>
        /// The AttackSpeedEffect this QuickHandsTalent provides.
        /// </summary>
        private AttackSpeedEffect attackSpeedEffect;

        /// <summary>
        /// The SpellHasteEffect this QuickHandsTalent provides.
        /// </summary>
        private SpellHasteEffect spellHasteEffect;

        #endregion
    }
}
