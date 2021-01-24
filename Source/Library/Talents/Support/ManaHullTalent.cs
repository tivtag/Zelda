// <copyright file="ManaHullTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Support.ManaHullTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Atom.Math;
    using Zelda.Status.Auras;

    /// <summary>
    /// You gain a {0}% chance when attacked to
    /// surround yourself with a perfect hull of mana;
    /// regenerating {1}% of base mana over {2} seconds.
    /// </summary>
    internal sealed class ManaHullTalent : Talent
    {
        #region [ Constants ]
        
        /// <summary>
        /// The chance the Mana Hull effect procs when the player gets attacked.
        /// </summary>
        private const float ChanceToProcWhenAttacked = 10.0f;

        /// <summary>
        /// The time the Mana Hull takes to apply its full effect.
        /// </summary>
        private const float RegenerationTime = 4.0f;

        /// <summary>
        /// States how often the Mana Hull ticks to fully regenerate.
        /// </summary>
        private const int TickCount = 3;

        /// <summary>
        /// Gets the base mana regenerated in % by the Mana Hull effect.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The amount of base mana regenerated.
        /// </returns>
        private static float GetPercentageOfBaseManaRegenerated( int level )
        {
            switch( level )
            {
                case 1:
                    return 0.07f;

                case 2:
                    return 0.15f;

                case 3:
                    return 0.20f;

                case 0:
                default:
                    return 0.0f;
            }
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
                TalentResources.TD_ManaHull,
                ChanceToProcWhenAttacked.ToString( CultureInfo.CurrentCulture ),
                (GetPercentageOfBaseManaRegenerated( level ) * 100).ToString( CultureInfo.CurrentCulture ),
                RegenerationTime.ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ManaHullTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the ManaHullTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ManaHullTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ManaHull,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Meditation" ),
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
                new TalentRequirement( this.Tree.GetTalent( typeof( MeditationTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this ManaHullTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.aura = new FixedManaRegenerationAura( RegenerationTime, RegenerationTime / (float)TickCount ) {
                Name = "ManaHull_Aura",
                Symbol = this.Symbol,
                IsVisible = true
            };

            // Hook events.
            this.Owner.Attackable.Attacked += this.OnAttacked;
        }

        /// <summary>
        /// Uninitializes this ManaHullTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.aura = null;
            
            // Unhook events.
            this.Owner.Attackable.Attacked -= this.OnAttacked;
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Called when the player got attacked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contain the event data.</param>
        private void OnAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            float roll = this.ServiceProvider.Rand.RandomRange( 0.0f, 100.0f );

            if( roll <= ChanceToProcWhenAttacked )
            {
                this.ResetAndApplyAura();
            }
        }

        /// <summary>
        /// Resets the ManaRegenerationAura.
        /// </summary>
        private void ResetAndApplyAura()
        {
            this.aura.ResetDuration();

            if( !this.aura.IsEnabled )
            {
                this.aura.ResetTick();

                this.RefreshAura();
                this.Statable.AuraList.Add( this.aura );
            }
        }

        /// <summary>
        /// Refreshes the effect the ManaRegenerationAura has.
        /// </summary>
        private void RefreshAura()
        {
            float percentage = GetPercentageOfBaseManaRegenerated( this.Level );
            int totalManaRegenerated = this.Statable.GetPercentageOfBaseMana( percentage );

            this.aura.ManaEachTick = totalManaRegenerated / TickCount;
        }

        /// <summary>
        /// Refreshes the strength of this ManaHullTalent.
        /// </summary>
        protected override void Refresh()
        {
            // no op.
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The FixedManaRegenerationAura this ManaHullTalent uses to regeneretate
        /// the mana of the player.
        /// </summary>
        private FixedManaRegenerationAura aura;

        #endregion
    }
}
