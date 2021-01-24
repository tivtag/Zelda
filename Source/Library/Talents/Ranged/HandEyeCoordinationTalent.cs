// <copyright file="HandEyeCoordinationTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.HandEyeCoordinationTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using System;
    using Zelda.Status;
    
    /// <summary>
    /// Increases dexterity by 5%/10% of total agility.
    /// </summary>
    internal sealed class HandEyeCoordinationTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The amount of agility that is converted into dexterity per talent level.
        /// </summary>
        private const float PercentageOfAgilityToDexterityPerLevel = 0.05f;

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
                TalentResources.TD_HandEyeCoordination,
                (level * PercentageOfAgilityToDexterityPerLevel * 100.0f).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HandEyeCoordinationTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new HandEyeCoordinationTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal HandEyeCoordinationTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_HandEyeCoordination,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_HandEyeCoordination" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this HandEyeCoordinationTalent with the other Talents of the TalentTree.
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
        /// Initializes this HandEyeCoordinationTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new StatEffect( 0.0f, StatusManipType.Fixed, Stat.Dexterity );
            this.aura = new PermanentAura( this.effect );

            this.AuraList.Add( this.aura );
            this.Statable.AgilityUpdated += this.OnPlayerAgilityUpdated;
        }

        /// <summary>
        /// Uninitializes this HandEyeCoordinationTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.effect = null;
            this.aura = null;

            this.Statable.AgilityUpdated -= this.OnPlayerAgilityUpdated;
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Refreshes the strength of this HandEyeCoordinationTalent.
        /// </summary>
        protected override void Refresh()
        {
            float agilityToDexterity = PercentageOfAgilityToDexterityPerLevel * this.Level;

            // Apply new value.
            effect.Value = this.Owner.Statable.Agility * agilityToDexterity;

            // Refresh the value.
            this.Owner.Statable.Refresh_Dexterity();
        }

        /// <summary>
        /// Called when the total agility of the player has been updated.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnPlayerAgilityUpdated( object sender, EventArgs e )
        {
            this.Refresh();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The aura that contains the passive effect of the Talent.
        /// </summary>
        private PermanentAura aura;

        /// <summary>
        /// The effect of the Talent.
        /// </summary>
        private StatEffect effect;

        #endregion
    }
}
