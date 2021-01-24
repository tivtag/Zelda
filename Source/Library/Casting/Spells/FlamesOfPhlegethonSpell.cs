// <copyright file="FlamesOfPhlegethonSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.FlamesOfPhlegethonSpell class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting.Spells
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities;
    using Zelda.Entities.Behaviours;

    /// <summary>
    /// Summons {0} wave(s) of fire directly from the underworld.
    /// Targets that are hit take {0}% to {1}% fire damage.
    /// {1} secs cooldown. {2} secs cast time.
    /// </summary>
    internal sealed class FlamesOfPhlegethonSpell : PlayerSpell
    {
        #region [ Constants ]

        /// <summary>
        /// The time between two pillar spawns within a single wave.
        /// </summary>
        private const float TimeBetweenPillars = 0.35f;

        /// <summary>
        /// The time between two waves of flames.
        /// </summary>
        private const float TimeBetweenWaves = 3.0f;

        /// <summary>
        /// The time any Wave is active for.
        /// </summary>
        private const float WaveTime = 15.0f;

        #endregion
        
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the maximum number of waves the FlamesOfPhlegethonSpell should spawn.
        /// </summary>
        public int MaximumWaveCount
        {
            get
            {
                return this.maximumWaveCount;
            }

            set
            {
                this.maximumWaveCount = value;
                this.EnsureWavesAreAllocated();
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the FlamesOfPhlegethonSpell class.
        /// </summary>
        /// <param name="owner">
        /// The PlayerEntity that owns the new FlamesOfPhlegethonSpell.
        /// </param>
        /// <param name="castTime">
        /// The time it takes to cast the new FlamesOfPhlegethonSpell.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that is used to calculate the damage done of the new FlamesOfPhlegethonSpell.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public FlamesOfPhlegethonSpell( PlayerEntity owner, float castTime, AttackDamageMethod method, IZeldaServiceProvider serviceProvider )
            : base( owner, castTime, method )
        {
            this.pillarSpell = new FlamesOfPhlegethonPillar( owner, method, serviceProvider );
            owner.Statable.Died += this.OnPlayerDied;
        }

        /// <summary>
        /// Called when the player that owns this FlamesOfPhlegethonSpell has died
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        private void OnPlayerDied( Zelda.Status.Statable sender )
        {
            this.Reset();
            this.isActive = false;
        }

        /// <summary>
        /// Ensures that enough <see cref="Wave"/> instances have been allocated.
        /// </summary>
        private void EnsureWavesAreAllocated()
        {
            while( this.waves.Count < this.maximumWaveCount )
            {
                this.AllocateWave();
            }
        }

        /// <summary>
        /// Allocates a new Wave instance.
        /// </summary>
        private void AllocateWave()
        {
            var wave = new Wave( this );
            wave.Ended += this.OnWaveEnded;

            this.waves.Add( wave );
        }
        
        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Fires this FlamesOfPhlegethonSpell.
        /// </summary>
        /// <param name="target">
        /// This paramter is not used.
        /// </param>
        /// <returns>
        /// Always true; spells are cast instead of fired.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            this.Reset();
            this.isActive = true;
            return true;
        }

        /// <summary>
        /// Resets the state of this FlamesOfPhlegethonSpell.
        /// </summary>
        private void Reset()
        {
            foreach( var wave in this.waves )
            {
                wave.Reset();
            }

            this.waveCount = 0;
            this.timeLeftUntilNextWave = 0.0f;
        }

        /// <summary>
        /// Updates this FlamesOfPhlegethonSpell.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( !this.isActive )
                return;

            if( this.CanSpawnMoreWaves() )
            {
                this.UpdateWaveSpawning( updateContext );
            }

            this.UpdateWaves( updateContext );
        }

        /// <summary>
        /// Gets a value indicating whether more an additional
        /// <see cref="Wave"/> of flames can be spawned.
        /// </summary>
        private bool CanSpawnMoreWaves()
        {
            return this.waveCount < this.MaximumWaveCount;
        }

        /// <summary>
        /// Updates the logic of spawning the next Wave.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateWaveSpawning( ZeldaUpdateContext updateContext )
        {
            this.timeLeftUntilNextWave -= updateContext.FrameTime;

            if( this.timeLeftUntilNextWave <= 0.0f )
            {
                this.StartNextWave();
                this.timeLeftUntilNextWave = TimeBetweenWaves;
            }
        }

        /// <summary>
        /// Starts/Spawns the next wave.
        /// </summary>
        private void StartNextWave()
        {
            Wave wave = this.GetInactiveWave();
            
            if( wave != null )
            {
                wave.Start();
            }

            ++this.waveCount;
        }

        /// <summary>
        /// Gets a currently non-active <see cref="Wave"/> object.
        /// </summary>
        /// <returns></returns>
        private Wave GetInactiveWave()
        {
            return this.waves.FirstOrDefault( ( wave ) => !wave.IsActive );
        }

        /// <summary>
        /// Updates the currently active <see cref="Wave"/>s.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateWaves( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.waves.Count; ++i )
            {
                this.waves[i].Update( updateContext );
            }
        }

        /// <summary>
        /// Called when a Wave of this FlamesOfPhlegethonSpell has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnWaveEnded( Wave sender )
        {
            if( this.AreAllWavesInactive() )
            {
                this.isActive = this.CanSpawnMoreWaves();
            }
        }

        /// <summary>
        /// Gets a value indicating whether all Waves of this FlamesOfPhlegethonSpell
        /// are indicating.
        /// </summary>
        /// <returns>
        /// true if all waves are inactive;
        /// otherwise false.
        /// </returns>
        private bool AreAllWavesInactive()
        {
            foreach( var wave in this.waves )
            {
                if( wave.IsActive )
                    return false;
            }

            return true;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whther this FlamesOfPhlegethonSpell is currently active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The number of waves that have been spawned.
        /// </summary>
        private int waveCount;

        /// <summary>
        /// The time left in seconds until the next wave spawns.
        /// </summary>
        private float timeLeftUntilNextWave;

        /// <summary>
        /// The maximum number of waves that should be spawned.
        /// </summary>
        private int maximumWaveCount = 1;

        /// <summary>
        /// The spell that is responsible for creating the individual pillars
        /// of the Waves.
        /// </summary>
        private readonly FlamesOfPhlegethonPillar pillarSpell;

        /// <summary>
        /// The list of all active and non-active Waves.
        /// </summary>
        private readonly List<Wave> waves = new List<Wave>();

        #endregion

        #region [ Classes ]

        /// <summary>
        /// Represents a single wave of fire pillars of the FlamesOfPhlegethonSpell.
        /// </summary>
        private sealed class Wave
        {
            #region [ Events ]

            /// <summary>
            /// Raised when this Wave has ended.
            /// </summary>
            public event SimpleEventHandler<Wave> Ended;

            #endregion

            #region [ Properties ]

            /// <summary>
            /// Gets a value indicating whether this Wave is currently active.
            /// </summary>
            public bool IsActive
            {
                get
                {
                    return this.isActive;
                }
            }

            #endregion

            #region [ Initialization ]

            /// <summary>
            /// Initializes a new instance of the Wave class.
            /// </summary>
            /// <param name="spell">
            /// Identifies the FlamesOfPhlegethonSpell that owns the new Wave.
            /// </param>
            public Wave( FlamesOfPhlegethonSpell spell )
            {
                this.spell = spell;
            }
            
            /// <summary>
            /// Resets this Wave.
            /// </summary>
            public void Reset()
            {
                this.range = 0.0f;
                this.angle = 0.0f;
                this.isActive = false;
            }

            /// <summary>
            /// Starts this Wave.
            /// </summary>
            internal void Start()
            {
                this.range = 8.0f;
                this.timeLeft = WaveTime;
                this.rangeSpeed = 5.0f;
                this.angleSpeed = 1.5f;
                this.timeLeftUntilNextPillar = 0.0f;

                this.isActive = true;
            }

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Updates this Wave.
            /// </summary>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            internal void Update( ZeldaUpdateContext updateContext )
            {
                if( !this.isActive )
                    return;
                
                this.timeLeft -= updateContext.FrameTime;
                
                if( this.timeLeft <= 0.0f )
                {
                    this.isActive = false;
                    this.Ended.Raise( this );
                    return;
                }

                this.UpdateSpawnPosition( updateContext );
                this.UpdatePillarSpawning( updateContext );
            }

            /// <summary>
            /// Updates the position at which this Wave currently spawns pillars.
            /// </summary>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            private void UpdateSpawnPosition( ZeldaUpdateContext updateContext )
            {
                this.angle += (this.angleSpeed * updateContext.FrameTime);

                if( this.range <= 54.0f )
                {
                    this.range += (this.rangeSpeed * updateContext.FrameTime);
                }
            }

            /// <summary>
            /// Updates the spawning of the Fire Pillars of this Wave.
            /// </summary>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            private void UpdatePillarSpawning( ZeldaUpdateContext updateContext )
            {
                this.timeLeftUntilNextPillar -= updateContext.FrameTime;

                if( this.timeLeftUntilNextPillar <= 0.0f )
                {
                    this.SpawnPillar();
                    this.timeLeftUntilNextPillar = TimeBetweenPillars;
                }
            }

            /// <summary>
            /// Spawns a new Fire Pillar at the current position.
            /// </summary>
            private void SpawnPillar()
            {
                var player = this.spell.Owner;

                this.spell.pillarSpell.SpawnFirePillarAt( 
                    this.GetPillarSpawnPosition(),
                    player.FloorNumber,
                    player.Scene
                );
            }

            /// <summary>
            /// Gets the position at which the next pillar should be spawned.
            /// </summary>
            /// <returns></returns>
            private Vector2 GetPillarSpawnPosition()
            {
                Vector2 playerPosition = this.spell.Owner.Transform.Position;
                Vector2 direction = new Vector2( 
                    (float)Math.Cos( this.angle ), 
                    -(float)Math.Sin( this.angle )
                );

                return playerPosition + (direction * this.range);
            }

            #endregion

            #region [ Fields ]

            /// <summary>
            /// The time left this Wave is active for.
            /// </summary>
            private float timeLeft;

            /// <summary>
            /// The time left until the next pillar of this Wave spawns.
            /// </summary>
            private float timeLeftUntilNextPillar;
            
            /// <summary>
            /// The current angle of this Wave.
            /// </summary>
            private float angle;

            /// <summary>
            /// The speed at which the angle of this Wave changes.
            /// </summary>
            private float angleSpeed;

            /// <summary>
            /// The current range of this Wave.
            /// </summary>
            private float range;

            /// <summary>
            /// The speed at which the range of this Wave changes.
            /// </summary>
            private float rangeSpeed;

            /// <summary>
            /// States whether this Wave is currently active.
            /// </summary>
            private bool isActive;
            
            /// <summary>
            /// Identifies the FlamesOfPhlegethonSpell that owns the new Wave.
            /// </summary>
            private readonly FlamesOfPhlegethonSpell spell;

            #endregion
        }
        
        /// <summary>
        /// Represents the spell responsible for creating the individual pillars
        /// of the Flames of Phlegethon spell.
        /// </summary>
        private sealed class FlamesOfPhlegethonPillar : FirePillarSpell
        {
            /// <summary>
            /// Initializes a new instance of the FlamesOfPhlegethonPillar class.
            /// </summary>
            /// <param name="player">
            /// The player that owns the new FirewallPillarSpell.
            /// </param>
            /// <param name="damageMethod">
            /// The damage method used to calculate the damage done by each individual pillar.
            /// </param>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related servives.
            /// </param>
            public FlamesOfPhlegethonPillar( PlayerEntity player, AttackDamageMethod damageMethod, IZeldaServiceProvider serviceProvider )
                : base( player, player.Statable, 0.0f, damageMethod, serviceProvider )
            {
            }

            /// <summary>
            /// Allows custom initialization logic for Fire Pillars created by this FirePillarSpell
            /// to be inserted.
            /// </summary>
            /// <param name="firePillar">
            /// The Fire Pillar that was created created.
            /// </param>
            protected override void SetupFirePillarEntity( DamageEffectEntity firePillar )
            {
                firePillar.Behaveable.Behaviour = new RemoveAfterAnimationEndedBehaviour( firePillar );
            }
        }

        #endregion
    }
}