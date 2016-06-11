// <copyright file="FrustrationSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.FrustrationSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Atom.Math;
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary>
    /// Defines the Frustration skill which turns the frustration of the player into anger;
    /// <para>
    /// increasing chance to get a Double Attack by 15/30/45%.
    /// In his anger the player has a 15% chance to hit himself.
    /// 1 minute duration, 5 minutes cooldown.
    /// </para>
    /// </summary>
    internal sealed class FrustrationSkill : PlayerBuffSkill<FrustrationTalent>
    {
        /// <summary>
        /// Gets or sets the FrustrationAura this FrustrationSkill applies.
        /// </summary>
        private new FrustrationAura Aura
        {
            get
            {
                return (FrustrationAura)base.Aura;
            }

            set
            {
                base.Aura = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrustrationSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new FrustrationSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides access to game-related services.
        /// </param>
        internal FrustrationSkill( FrustrationTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, FrustrationTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( FrustrationTalent.ManaNeededPoBM );

            this.Aura = new FrustrationAura( serviceProvider ) {
                Name                = this.LocalizedName,
                IsVisible           = true,
                Symbol              = talent.Symbol,
                DescriptionProvider = this
            };
        }
        
        /// <summary>
        /// Refreshes the strength of this FrustrationSkill based on the talents the player has choosen.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Aura.RefreshData( this.Talent );
        }
                
        /// <summary>
        /// Defines the TimedAura that enables the effect of the FrustrationSkill.
        /// </summary>
        private sealed class FrustrationAura : TimedAura
        {
            #region [ Initialization ]

            /// <summary>
            /// Initializes a new instance of the FrustrationAura class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public FrustrationAura( IZeldaServiceProvider serviceProvider )
                : base( FrustrationTalent.Duration )
            {
                this.rand         = serviceProvider.Rand;
                this.damageMethod = new Zelda.Attacks.Melee.FrustationDamageMethod();
                
                this.damageMethod.SetValues( FrustrationTalent.SelfHitDamageMultiplier );
                this.damageMethod.Setup( serviceProvider );
            }

            /// <summary>
            /// Refreshes the strength of the FrustrationAura.
            /// </summary>
            /// <param name="talent">
            /// The related FrustrationTalent.
            /// </param>
            public void RefreshData( FrustrationTalent talent )
            {
                this.procChance = talent.Level * FrustrationTalent.ProcChancePerLevel;
            }

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Called when the player has managed to hit an enemy with his default attack.
            /// </summary>
            /// <param name="sender">
            /// The sender of the event.
            /// </param>
            /// <param name="e">
            /// The CombatEventArgs that contains the event data.
            /// </param>
            private void OnDefaultMeleeHit( Statable sender, CombatEventArgs e )
            {
                // Does the player get a double attack?
                if( rand.RandomRange( 0.0f, 100.0f ) <= procChance )
                {
                    var attackable = e.Target.Owner.Components.Get<Entities.Components.Attackable>();

                    if( attackable != null )
                    {
                        // Attack again.
                        attackable.Attack( e.User.Owner, damageMethod.GetDamageDone( e.User, e.Target ) );
                    }
                }

                // Does the player hit himself?
                if( rand.RandomRange( 0.0f, 100.0f ) <= FrustrationTalent.SelfHitChance )
                {
                    var attackable = this.player.Attackable;

                    if( attackable != null )
                    {
                        attackable.Attack( 
                            (Zelda.Entities.IAttackableEntity)player, 
                            damageMethod.GetDamageDone( player.Statable, player.Statable )
                        );
                    }
                }
            }

            /// <summary>
            /// Called when this <see cref="Aura"/> has got enabled.
            /// </summary>
            /// <param name="owner">
            /// The Statable that now owns this Aura.
            /// </param>
            protected override void OnEnabled( Statable owner )
            {
                owner.DefaultMeleeHit += this.OnDefaultMeleeHit;

                this.player = (Entities.PlayerEntity)owner.Owner;
            }

            /// <summary>
            /// Called when this <see cref="Aura"/> got disabled.
            /// </summary>
            /// <param name="owner">
            /// The Statable that previously owned this Aura.
            /// </param>
            protected override void OnDisabled( Statable owner )
            {
                owner.DefaultMeleeHit -= this.OnDefaultMeleeHit;
            }

            #endregion

            #region [ Fields ]

            /// <summary>
            /// Stores the chance the Double Attack effect can proc.
            /// </summary>
            private float procChance;

            /// <summary>
            /// Idenfities the PlayerEntity.
            /// </summary>
            private Entities.PlayerEntity player;

            /// <summary>
            /// The damage method used to calculate the damage inflicted by the Double Attack.
            /// </summary>
            private readonly Zelda.Attacks.Melee.FrustationDamageMethod damageMethod;

            /// <summary>
            /// A random number generator.
            /// </summary>
            private readonly RandMT rand;

            #endregion
        }
    }
}
