// <copyright file="EnrageTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.EnrageTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Atom.Math;
    using Zelda.Status;
    using Zelda.Status.Damage;
    
    /// <summary>
    /// The Enrage talent gives the player a 
    /// X% chance to enrage after getting hit or crit by an attack.
    /// Increases damage dealt by 20% for 10 seconds. Can't trigger while active.
    /// </summary>
    internal sealed class EnrageTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The chance to trigger the effect by level.
        /// </summary>
        private const float ChanceToTriggerByLevel = 5.0f;

        /// <summary>
        /// The damage increase
        /// </summary>
        private const float MeleeDamageIncrease = 20.0f;

        /// <summary>
        /// The duration of the effect once active.
        /// </summary>
        private const float Duration = 10.0f;

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
                TalentResources.TD_Enrage,
                (level * ChanceToTriggerByLevel).ToString( culture ),
                MeleeDamageIncrease.ToString( culture ),
                Duration.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrageTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new EnrageTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal EnrageTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Enrage,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Enrage" ),
                5,
                tree,
                serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Setups the connections of this EnrageTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( BattleShoutTalent ) ), 3 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( WhirlwindTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this EnrageTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new DamageDoneWithSourceEffect( 
                MeleeDamageIncrease,
                StatusManipType.Percental,
                DamageSource.Melee
            );

            this.aura = new TimedAura( Duration, this.effect ) {
                Name                = "Enrage_Aura",
                DescriptionProvider = this,
                IsVisible           = true,
                Symbol              = this.Symbol
            };

            this.Owner.Attackable.Attacked += this.OnAttacked;
        }

        /// <summary>
        /// Uninitializes this EnrageTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.effect = null;
            this.aura = null;

            this.Owner.Attackable.Attacked -= this.OnAttacked;
        }

        /// <summary>
        /// Refreshes the strength of this EnrageTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.chanceToTrigger = ChanceToTriggerByLevel * this.Level;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when the player gets hit or crit by any attack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackEventArgs that contains the event data.</param>
        private void OnAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            if( this.ShouldProc( e.DamageResult.DamageTypeInfo ) )
            {
                this.aura.ResetDuration();
                this.AuraList.Add( this.aura );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Enrage effect
        /// should proc.
        /// </summary>
        /// <param name="damageTypeInfo">
        /// An DamageTypeInfo that descripes the damage inflicted onto the player.
        /// </param>
        /// <returns>
        /// true if the effect should proc;
        /// otherwise false.
        /// </returns>
        private bool ShouldProc( DamageTypeInfo damageTypeInfo )
        {
            if( this.aura.AuraList != null )
                return false; // is already active

            if( damageTypeInfo == null || damageTypeInfo.SpecialType != SpecialDamageType.None )
                return false;

            float roll = this.rand.RandomRange( 0.0f, 100.0f );
            return roll <= this.chanceToTrigger;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the chance to trigger the effect.
        /// </summary>
        private float chanceToTrigger;

        /// <summary>
        /// The effect that is applied by the EnrageTalent.
        /// </summary>
        private DamageDoneWithSourceEffect effect;

        /// <summary>
        /// The aura that holds the effect that is applied by the EnrageTalent.
        /// </summary>
        private TimedAura aura;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        #endregion
    }
}
