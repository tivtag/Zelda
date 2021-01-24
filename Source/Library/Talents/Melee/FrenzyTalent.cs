// <copyright file="FrenzyTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.FrenzyTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Melee
{
    using System;
    using Zelda.Status;
    
    /// <summary>
    /// The FrenzyTalent provides the player to gain the Frenzy effect 
    /// when they crit with any melee attack.
    /// </summary>
    internal sealed class FrenzyTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The duration of the frenzy effect.
        /// </summary>
        private const float FrenzyEffectDuration = 10.0f;

        /// <summary>
        /// Gets the speed increase provided by the Frenzy effect.
        /// </summary>
        /// <param name="level">The level of the talent.</param>
        /// <returns>The speed increase.</returns>
        private static float GetSpeed( int level )
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
                TalentResources.TD_Frenzy,
                GetSpeed( level ).ToString( culture ),
                FrenzyEffectDuration.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FrenzyTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FrenzyTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FrenzyTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_Frenzy,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Frenzy" ),
                5,
                tree,
                serviceProvider
            )
        {
        }        

        /// <summary>
        /// Setups the connections of this FrenzyTalent with other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( BattleShoutTalent ) ), 2 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( FurorTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this FrenzyTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new AttackSpeedEffect( Zelda.Attacks.AttackType.Melee, 0.0f, StatusManipType.Percental );
            this.aura = new TimedAura( FrenzyEffectDuration, effect ) {
                Name                = this.LocalizedName,
                DescriptionProvider = this,
                IsVisible           = true,
                Symbol              = this.Symbol 
            };

            this.Statable.MeleeCrit += this.OnCriticalMeleeStrike;
        }

        /// <summary>
        /// Uninitializes this FrenzyTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.Statable.MeleeCrit -= this.OnCriticalMeleeStrike;

            this.effect = null;
            this.aura = null;
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this FrenzyTalent.
        /// </summary>
        protected override void Refresh()
        {
            bool wasRemoved = this.AuraList.Remove( aura );

            effect.Value = -GetSpeed( this.Level );

            if( wasRemoved )
            {
                this.AuraList.Add( aura );
            }
        }

        /// <summary>
        /// Gets called when the player crits with a melee attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCriticalMeleeStrike( Statable sender )
        {
            aura.ResetDuration();

            if( aura.AuraList == null )
            {
                this.AuraList.Add( aura );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The AttackSpeedEffect that gets applied by Frenzy.
        /// </summary>
        private AttackSpeedEffect effect;

        /// <summary>
        /// The TimedAura that holds the AttackSpeedEffect that gets applied by Frenzy.
        /// </summary>
        private TimedAura aura;

        #endregion
    }
}
