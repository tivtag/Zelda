// <copyright file="NaturePolarity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.NaturePolarity class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using Atom.Math;
    using Zelda.Attacks.Methods;
    using Zelda.Casting.Spells;
    using Zelda.Entities.Spawning;
    using Zelda.Status;

    /// <summary>
    /// During the NaturePolarity the hell-chicken boss takes increased fire damage
    /// and randomly spawns poison clouds around him.
    /// </summary>
    internal sealed class NaturePolarity : MagicalPolarity
    {
        /// <summary>
        /// The number of poison clouds that should spawn.
        /// </summary>
        private static readonly IntegerRange SpawnCount = new IntegerRange( 2, 4 );

        /// <summary>
        /// The range, starting at the boss at which the poison clouds should spawn.
        /// </summary>
        private static readonly FloatRange SpawnRange = new FloatRange( 16.0f, 160.0f );

        /// <summary>
        /// The damage the poison clouds are doing.
        /// </summary>
        private static readonly IntegerRange PoisonDamageRange = new IntegerRange( 350, 525 );

        /// <summary>
        /// The time between additional Poison Cloud spawns.
        /// </summary>
        private static readonly FloatRange TimeBetweenAdditionalPoisonClouds = new FloatRange( 2.5f, 6.0f );

        /// <summary>
        /// Initializes a new instance of the NaturePolarity class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <param name="boss">
        /// Identifies the hell-chicken boss object that owns the new NaturePolarity.
        /// </param>
        public NaturePolarity( IZeldaServiceProvider serviceProvider, Enemy boss )
            : base( new Vector4( 0.0f, 1.0f, 0.0f, 1.0f ), ElementalSchool.Nature, boss )
        {
            var method = new FixedPoisonOverTimeSpellDamageMethod() {
                 DamageRange = PoisonDamageRange
            };

            method.Setup( serviceProvider );

            this.poisonCloudSpell = new PoisonCloudSpell( method, boss.Statable, serviceProvider );
            this.rand = serviceProvider.Rand;
            this.timeLeftUntilPoisonCloud = TimeBetweenAdditionalPoisonClouds.GetRandomValue( this.rand );
        }

        /// <summary>
        /// Called when this NaturePolarity gets enabled.
        /// </summary>
        public override void Enable()
        {
            int poisonCloudsToSpawn = SpawnCount.GetRandomValue( this.rand );

            for( int i = 0; i < poisonCloudsToSpawn; ++i )
            {
                this.SpawnPoisonCloud();
            }

            base.Enable();
        }

        /// <summary>
        /// Updates this NaturePolarity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.timeLeftUntilPoisonCloud -= updateContext.FrameTime;

            if( this.timeLeftUntilPoisonCloud <= 0.0f )
            {
                this.SpawnPoisonCloud();
                this.timeLeftUntilPoisonCloud = TimeBetweenAdditionalPoisonClouds.GetRandomValue( this.rand );
            }
        }

        /// <summary>
        /// Spawns a poison cloud somewhere around the boss.
        /// </summary>
        private void SpawnPoisonCloud()
        {
            Vector2 position = GetRandomPoisonCloudSpawnPosition();
            var cloud = this.poisonCloudSpell.CreatePoisonCloud( position, this.Boss.FloorNumber );

            SpawnHelper.AddBlendInColorTint( cloud );
            cloud.AddToScene( this.Boss.Scene );
        }

        /// <summary>
        /// Gets a position somewhere around the boss, at which the poison cloud should be spawned.
        /// </summary>
        /// <returns>
        /// A random position.
        /// </returns>
        private Vector2 GetRandomPoisonCloudSpawnPosition()
        {
            float angle = this.rand.RandomRange( 0.0f, Constants.TwoPi );
            float range = SpawnRange.GetRandomValue( this.rand );

            Vector2 offset = new Vector2(
                range * (float)System.Math.Cos( angle ),
                range * -(float)System.Math.Sin( angle )
            );

            return this.Boss.Collision.Center + offset;
        }

        /// <summary>
        /// The time left in seconds until an additional Poison Cloud is spawned.
        /// </summary>
        private float timeLeftUntilPoisonCloud;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly IRand rand;

        /// <summary>
        /// The spell responsible for creating Poison Clouds.
        /// </summary>
        private readonly PoisonCloudSpell poisonCloudSpell;
    }
}
