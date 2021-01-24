// <copyright file="AngelEmbracementTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Support.AngelEmbracementTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Atom.Math;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Gives a 5% chance when attacked to increase all Damage Done by 5/10%,
    /// Armor by 7.5/15% and Magic Find by 15/30% 
    /// for 12 seconds.
    /// </summary>
    internal sealed class AngelEmbracementTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The damage increase the Angel Embracement gives when procced.
        /// </summary>
        private const float DamageIncreaseInPercent = 5.0f;

        /// <summary>
        /// The armor increase the Angel Embracement gives when procced.
        /// </summary>
        private const float ArmorIncreaseInPercent = 7.5f;

        /// <summary>
        /// The magic find increase the Angel Embracement gives when procced.
        /// </summary>
        private const float MagicFindIncreaseInPercent = 15.0f;

        /// <summary>
        /// The duration of the Angel Embracement effect.
        /// </summary>
        private const float Duration = 12.0f;

        /// <summary>
        /// The chance for the Angel Embracement effect to get applied.
        /// </summary>
        private const float ProcChance = 5.0f;

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
                TalentResources.TD_AngelsEmbracement,
                ProcChance.ToString( CultureInfo.CurrentCulture ),
                Duration.ToString( CultureInfo.CurrentCulture ),
                (DamageIncreaseInPercent * level).ToString( CultureInfo.CurrentCulture ),
                (ArmorIncreaseInPercent * level).ToString( CultureInfo.CurrentCulture ),
                (MagicFindIncreaseInPercent * level).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AngelEmbracementTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new AngelEmbracementTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal AngelEmbracementTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_AngelEmbracement,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Angel" ),
                2,
                tree,
                serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Setups the talent and its connections with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( LuckyBastardTalent ) ), 3 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this LuckyBastardTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.damageEffect = new DamageDoneWithSourceEffect( 
                0.0f, 
                StatusManipType.Percental,
                DamageSource.All
            );

            this.armorEffect = new ArmorEffect( 0.0f, StatusManipType.Percental );
            this.magicFindEffect = new MagicFindEffect( 0.0f, StatusManipType.Percental );

            this.aura = new TimedAura( Duration, new StatusValueEffect[3] { damageEffect, armorEffect, magicFindEffect } ) {
                Name      = "AngelEmbracement_Aura",
                Symbol    = this.Symbol,
                IsVisible = true
            };
             
            // Hook events.
            this.Owner.Attackable.Attacked += this.OnAttacked;
        }

        /// <summary>
        /// Uninitializes this LuckyBastardTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.damageEffect = null;
            this.armorEffect = null;
            this.magicFindEffect = null;
            this.aura = null;

            // Unhook events.
            this.Owner.Attackable.Attacked -= this.OnAttacked;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this AngelEmbracementTalent talent.
        /// </summary>
        protected override void Refresh()
        {
            bool wasRemoved = this.AuraList.Remove( this.aura );

            this.armorEffect.Value = ArmorIncreaseInPercent * this.Level;
            this.damageEffect.Value = DamageIncreaseInPercent * this.Level;
            this.magicFindEffect.Value = MagicFindIncreaseInPercent * this.Level;

            if( wasRemoved )
                this.AuraList.Add( this.aura );
        }

        /// <summary>
        /// Called when the player gets hit or crit by any attack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contains the event data.</param>
        private void OnAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            if( this.aura.IsEnabled )
                return;

            if( this.rand.RandomRange( 0.0f, 100.0f ) <= ProcChance )
            {
                this.aura.ResetDuration();
                this.AuraList.Add( this.aura );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this AngelEmbracementTalent provides.
        /// </summary>
        private TimedAura aura;

        /// <summary>
        /// The passive damage done effect the Angel Embracement provides.
        /// </summary>
        private DamageDoneWithSourceEffect damageEffect;

        /// <summary>
        /// The passive magic find effect the Angel Embracement provides.
        /// </summary>
        private MagicFindEffect magicFindEffect;

        /// <summary>
        /// The passive armor effect the Angel Embracement provides.
        /// </summary>
        private ArmorEffect armorEffect;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly Atom.Math.RandMT rand;

        #endregion
    }
}
