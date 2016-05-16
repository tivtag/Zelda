// <copyright file="PolarityBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.PolarityBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    using Zelda.Graphics.Tinting;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Encapsulates the polarity behaviour of the Hell Chicken Boss.
    /// </summary>
    public sealed class PolarityBehaviour : IZeldaUpdateable
    {
        #region [ Constants ]

        /// <summary>
        /// The time it takes for the tinting effect to fully interpolate.
        /// </summary>
        private const float TintInterpolationDuration = 2.0f;

        /// <summary>
        /// The time between two polarity changes.
        /// </summary>
        private static readonly FloatRange TimeBetweenPolarityChanges = new FloatRange( 10.0f, 12.0f );

        private const float PhysicalDamageTakenIncrease = 40.0f;
        private const float ChanceToBeHitAndResistDuringGhost = 35.5f;

        #endregion

        #region [ Events ]

        /// <summary>
        /// Raised when the current BossPolarity has changed.
        /// </summary>
        public event EventHandler PolarityChanged;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the PolarityBehaviour class.
        /// </summary>
        /// <param name="boss">
        /// The boss whose polarity is modifies by the new PolarityBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PolarityBehaviour( Enemy boss, IZeldaServiceProvider serviceProvider )
        {
            this.boss = boss;
            this.rand = serviceProvider.Rand;

            this.polarities = CreatePolarities( serviceProvider );
            this.polarityChangeTint = new LinearColorReplacementTint() { 
                TotalTime = TintInterpolationDuration
            };

            this.ChangeToPolarity( this.GetDefaultPolarity() );
        }

        /// <summary>
        /// Creates all polarities the chicken boss might change to.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// A new array of polarities.
        /// </returns>
        private BossPolarity[] CreatePolarities( IZeldaServiceProvider serviceProvider )
        {
            return new BossPolarity[] {
                new BossPolarity( Vector4.One, CreatePhysicalPolarityEffect(), this.boss ),
                new FirePolarity( serviceProvider, this.boss ),
                new NaturePolarity( serviceProvider, this.boss ),
                this.CreateGhostPolarity(),
                // new LightPolarity( serviceProvider, this.boss )
            };
        }

        /// <summary>
        /// Creates the ghost BossPolarity.
        /// </summary>
        /// <returns>
        /// A newly created BossPolarity instance.
        /// </returns>
        private BossPolarity CreateGhostPolarity()
        {
            return new BossPolarity( new Vector4( 1.0f, 1.0f, 1.0f, 0.5f ), CreateGhostPolarityEffect(), this.boss );
        }

        /// <summary>
        /// Creates the effect the Ghost BossPolarity applies.
        /// </summary>
        /// <returns>
        /// A newly created PermanentAura that gets applied to
        /// the boss when he changes to the Ghost Polarity.
        /// </returns>
        private static PermanentAura CreateGhostPolarityEffect()
        {
            var missEffect = new ChanceToBeStatusEffect( 
                -ChanceToBeHitAndResistDuringGhost,
                StatusManipType.Fixed, 
                ChanceToStatus.Miss 
            );

            var resistEffect = new ChanceToResistEffect( 
                ChanceToBeHitAndResistDuringGhost,
                StatusManipType.Fixed, 
                ElementalSchool.All
            );

            var movementSpeedEffect = new MovementSpeedEffect(
                42.0f,
                StatusManipType.Percental
             );

            return new PermanentAura( new StatusEffect[] { missEffect, resistEffect, movementSpeedEffect } ) {
                Name = "Ghost_Polarity"
            };
        }

        /// <summary>
        /// Creates the effect that is applied when the boss changes
        /// into a physical polarity.
        /// </summary>
        /// <returns>
        /// A new PermanentAura that encapsulates the effect.
        /// </returns>
        private static Zelda.Status.PermanentAura CreatePhysicalPolarityEffect()
        {
            var effect = new DamageTakenFromSchoolEffect( PhysicalDamageTakenIncrease, StatusManipType.Percental, DamageSchool.Physical );

            return new PermanentAura( effect ) {
                Name = "Physical_Polarity"
            };
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this PolarityBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.timeLeftUntilPolarityChange -= updateContext.FrameTime;
            
            if( this.timeLeftUntilPolarityChange <= 0.0f )
            {
                this.ChangePolarity();
                this.timeLeftUntilPolarityChange = TimeBetweenPolarityChanges.GetRandomValue( this.rand );
            }

            this.currentPolarity.Update( updateContext );
        }

        /// <summary>
        /// Randomly changes to a -different- polarity.
        /// </summary>
        private void ChangePolarity()
        {
            BossPolarity polarity = this.GetRandomDifferentPolarity();
            this.ChangeToPolarity( polarity );
        }

        /// <summary>
        /// Randomly returns one of the BossPolarities; that is different
        /// compared to the current BossPolarity.
        /// </summary>
        /// <returns>
        /// A random, but different, BossPolarity.
        /// </returns>
        private BossPolarity GetRandomDifferentPolarity()
        {
            int index = rand.RandomRange( 0, this.polarities.Length - 1 );
            BossPolarity polarity = this.polarities[index];

            if( polarity == this.currentPolarity )
            {
                return this.GetRandomDifferentPolarity();
            }
            else
            {
                return polarity;
            }
        }

        /// <summary>
        /// Changes to the specified BossPolarity.
        /// </summary>
        /// <param name="polarity">
        /// The polarity to change to. Might not be null.
        /// </param>
        private void ChangeToPolarity( BossPolarity polarity )
        {
            if( this.currentPolarity == polarity )
                return;

            this.ModifyColorTintOnPolarityChange( polarity );
            this.DisableOldAndEnableNewPolarityEffect( polarity );

            this.PolarityChanged.Raise( this );
        }

        /// <summary>
        /// Modifies the IColorTint that is applied to visually show the
        /// new polarity.
        /// </summary>
        /// <param name="polarity">
        /// The new BossPolarity.
        /// </param>
        private void ModifyColorTintOnPolarityChange( BossPolarity polarity )
        {
            if( !this.tintHasBeenAdded )
            {
                this.AddColorTintToBoss();
            }

            this.polarityChangeTint.InitialColor = this.GetInitialPolarityTintColor();
            this.polarityChangeTint.FinalColor = polarity.Color;
            this.polarityChangeTint.Reset();
        }

        /// <summary>
        /// Adds the IColorTint that visualuzes the current polarity to the boss.
        /// </summary>
        private void AddColorTintToBoss()
        {
            var drawData = this.boss.DrawDataAndStrategy as Zelda.Entities.Drawing.TintedDrawDataAndStrategy;

            if( drawData != null )
            {
                drawData.TintList.Add( this.polarityChangeTint );
                this.tintHasBeenAdded = true;
            }
        }

        /// <summary>
        /// Gets the initial polarity of the polarityChangeTint; taken
        /// into account when changing to a new BossPolarity.
        /// </summary>
        /// <returns>
        /// The initial polarity.
        /// </returns>
        private Vector4 GetInitialPolarityTintColor()
        {
            if( this.currentPolarity == null )
            {
                return Vector4.One;
            }
            else
            {
                return this.currentPolarity.Color;
            }
        }

        /// <summary>
        /// Disables the effect of the current polarity and applies the effect
        /// of the new polarity.
        /// </summary>
        /// <param name="polarity">
        /// The new BossPolarity.
        /// </param>
        private void DisableOldAndEnableNewPolarityEffect( BossPolarity polarity )
        {
            Debug.Assert( polarity != null );

            if( this.currentPolarity != null )
            {
                this.currentPolarity.Disable();
            }

            this.currentPolarity = polarity;
            this.currentPolarity.Enable();
        }
        
        /// <summary>
        /// Resets the current polarity of the boss to the default.
        /// </summary>
        public void ResetPolarity()
        {
            this.ChangeToPolarity( this.GetDefaultPolarity() );
        }

        /// <summary>
        /// Gets the default BossPolarity.
        /// </summary>
        /// <returns>
        /// The default BossPolarity instance.
        /// </returns>
        private BossPolarity GetDefaultPolarity()
        {
            return this.polarities[0];
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The current polarity of the boss.
        /// </summary>
        private BossPolarity currentPolarity;
        
        /// <summary>
        /// Stores the time that is left until the new polarity change.
        /// </summary>
        private float timeLeftUntilPolarityChange = 10.0f;

        /// <summary>
        /// States whether the IColorTint that visualuzes the polarity of this boss has been added.
        /// </summary>
        private bool tintHasBeenAdded;

        /// <summary>
        /// The list of all polarities the boss can possibly change to.
        /// </summary>
        private readonly BossPolarity[] polarities;

        /// <summary>
        /// The IColorTint that is used to tint the boss for the current polarity.
        /// </summary>
        private readonly LinearColorReplacementTint polarityChangeTint;

        /// <summary>
        /// Identifies the boss whose polarity is controlled by this PolarityBehaviour.
        /// </summary>
        private readonly Enemy boss;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        #endregion
    }
}