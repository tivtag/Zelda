// <copyright file="FirePolarity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.FirePolarity class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using Atom.Math;
    using Zelda.Entities.Spawning;
    using Zelda.Status;

    /// <summary>
    /// During the FirePolarity the hell-chicken boss takes increased fire damage
    /// and randomly spawns fire below him.
    /// </summary>
    internal sealed class FirePolarity : MagicalPolarity
    {
        /// <summary>
        /// The time in seconds between two fires are spawned under the boss.
        /// </summary>
        private static readonly FloatRange DurationBetweenFires = new FloatRange( 0.25f, 2.45f );

        /// <summary>
        /// The name of the entity template that is used to create the Fire entities.
        /// </summary>
        private const string FireTemplateName = "Fire_Static_50";

        /// <summary>
        /// Initializes a new instance of the FirePolarity class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <param name="boss">
        /// Identifies the hell-chicken boss object that owns the new FirePolarity.
        /// </param>
        public FirePolarity( IZeldaServiceProvider serviceProvider, Enemy boss )
            : base( new Vector4( 1.0f, 0.0f, 0.0f, 1.0f ), ElementalSchool.Fire, boss )
        {
            this.rand = serviceProvider.Rand;
            this.fireTemplate = serviceProvider.EntityTemplateManager.GetTemplate( FireTemplateName );
            this.offsetFromBossCenter = new Vector2( 6.0f, 8.0f );
        }

        /// <summary>
        /// Updates this FirePolarity
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.timeLeftUntilNewFireSpawned -= updateContext.FrameTime;

            if( this.timeLeftUntilNewFireSpawned <= 0.0f )
            {
                this.SpawnFire();
                this.timeLeftUntilNewFireSpawned = DurationBetweenFires.GetRandomValue( this.rand );
            }
        }

        /// <summary>
        /// Spawns a new fire under the boss.
        /// </summary>
        private void SpawnFire()
        {
            var fire = this.fireTemplate.CreateInstance();

            fire.Transform.Position = this.Boss.Collision.Center - offsetFromBossCenter;
            SpawnHelper.AddBlendInColorTint( fire );

            fire.AddToScene( this.Boss.Scene );
        }

        /// <summary>
        /// The time left until a fire is spawned under the boss.
        /// </summary>
        private float timeLeftUntilNewFireSpawned;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly IRand rand;

        /// <summary>
        /// The offset that is applied to the boss center to get the spawn position of the fire.
        /// </summary>
        private readonly Vector2 offsetFromBossCenter;

        /// <summary>
        /// The template from which the fire entities are created.
        /// </summary>
        private readonly IEntityTemplate fireTemplate;
    }
}
