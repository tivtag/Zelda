// <copyright file="LightPolarity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.LightPolarity class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using Atom.Math;
    using Zelda.Attacks.Methods;
    using Zelda.Casting.Spells.Lightning;
    using Zelda.Status;
    using Zelda.Timing;

    /// <summary>
    /// During the LightPolarity the hell-chicken boss takes increased fire damage
    /// and randomly spawns fire below him.
    /// </summary>
    internal sealed class LightPolarity : MagicalPolarity
    {
        /// <summary>
        /// States the maximum length the lightning bolts reach.
        /// </summary>
        private readonly FloatRange MaximumBoltLength = new FloatRange( 50.0f, 100.0f );

        /// <summary>
        /// The damage the lightning bolts are doing.
        /// </summary>
        private static readonly IntegerRange LightDamageRange = new IntegerRange( 1, 300 );

        /// <summary>
        /// The time it takes for the lightning sparks to fully extract.
        /// </summary>
        private const float TimeUntilSparkFullySized = 8.0f;

        /// <summary>
        /// The time the player has before the spark spawns after entering this polarity.
        /// </summary>
        private const float TimeToReactForSpawn = 1.5f;

        /// <summary>
        /// Enumerates the various rotation modes of the spark.
        /// </summary>
        private enum SparkRotationMode 
        {
            Straight,
            Rotating,
            RotatingBackwards,
            Rotated,
            _ModeCount
        }

        /// <summary>
        /// Sets the current rotation factor.
        /// </summary>
        private float SparkRotation
        {
            // unused
            //get
            //{
            //    return this.spark.Transform.Rotation;
            //}

            set
            {
                this.spark.Transform.Rotation = value;
            }
        }

        /// <summary>
        /// Changes to the next SparkRotationMode.
        /// </summary>
        private void RotateMode()
        {
            this.mode = (SparkRotationMode)(((int)mode + 1) % (int)SparkRotationMode._ModeCount);

            switch( this.mode )
            {
                case SparkRotationMode.Rotated:
                    this.SparkRotation = Constants.Pi / 3;
                    break;

                case SparkRotationMode.Straight:
                    this.SparkRotation = Constants.Pi;
                    break;
                
                default:
                    this.SparkRotation = rand.UncheckedRandomRange( 0.0f, Constants.Pi );
                    break;                    
            }
        }

        /// <summary>
        /// Initializes a new instance of the LightPolarity class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <param name="boss">
        /// Identifies the hell-chicken boss object that owns the new LightPolarity.
        /// </param>
        public LightPolarity( IZeldaServiceProvider serviceProvider, Enemy boss )
            : base( new Vector4( 1.0f, 1.0f, 0.0f, 1.0f ), ElementalSchool.Light, boss )
        {
            this.rand = serviceProvider.Rand;

            var method = new FixedLightSpellDamageMethod() {
                DamageRange = LightDamageRange
            };

            method.Setup( serviceProvider );
            
            this.spark = new LightningSparkEntity() {
                Creator = boss.Statable,
                DamageMethod = method,
                Offset = 0.0f,
                MaximumLength = MaximumBoltLength.GetRandomValue(this.rand),
                IsAutomaticallyRebuild = false
            };

            this.spark.Setup( serviceProvider );
            boss.Transform.AddChild( this.spark.Transform );
        }

        /// <summary>
        /// Updates this LightPolarity
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.spawnTimer.Update( updateContext );

            if( this.spawnTimer.HasEnded )
            {
                if( this.spark.Scene == null )
                {
                    this.SpawnSpark();
                }

                if( this.mode == SparkRotationMode.Rotating )
                {
                    this.spark.Transform.Rotation += updateContext.FrameTime / 3.0f;
                }
                else if( this.mode == SparkRotationMode.RotatingBackwards )
                {
                    this.spark.Transform.Rotation -= updateContext.FrameTime / 3.0f;
                }

                this.lengthTimer.Update( updateContext );
                this.UpdateSparkTransform();
            }
        }

        /// <summary>
        /// Refreshes the transform of the spark that is attached to the boss.
        /// </summary>
        private void UpdateSparkTransform()
        {
            this.spark.MaximumLength = (1.0f - this.lengthTimer.Ratio) * this.currentMaximumBoltLength;
            this.spark.Transform.RelativePosition = new Vector2(
                this.Boss.Collision.Rectangle.Width / 2.0f,
                this.Boss.Collision.Rectangle.Height / 2.0f + 1
            );

            this.spark.Rebuild();
        }

        /// <summary>
        /// Enables this BossPolarity.
        /// </summary>
        public override void Enable()
        {
            this.lengthTimer.Reset();
            this.spark.MaximumLength = 0.0f;
            this.currentMaximumBoltLength = MaximumBoltLength.GetRandomValue( this.rand );
            this.RotateMode();
            this.spawnTimer.Reset();

            base.Enable();
        }

        private void SpawnSpark()
        {
            this.UpdateSparkTransform();
            this.spark.AddToScene( this.Boss.Scene );
        }

        /// <summary>
        /// Disables this BossPolarity.
        /// </summary>
        public override void Disable()
        {
            this.spark.Reset();
            this.spark.RemoveFromScene();
            base.Disable();
        }

        /// <summary>
        /// The current rotation mode of the spark.
        /// </summary>
        private SparkRotationMode mode = SparkRotationMode.Straight;

        /// <summary>
        /// Stores the current maximum bolt length. Changes on every activation.
        /// </summary>
        private float currentMaximumBoltLength;

        /// <summary>
        /// The timer that is used to allow the player to back-off the lightning.
        /// </summary>
        private readonly ResetableTimer spawnTimer = new ResetableTimer( TimeToReactForSpawn );

        /// <summary>
        /// The timer that is used to interpolate the maximum length of the spark.
        /// </summary>
        private readonly ResetableTimer lengthTimer = new ResetableTimer( TimeUntilSparkFullySized );

        /// <summary>
        /// The spark that is spawned in this phase.
        /// </summary>
        private readonly LightningSparkEntity spark;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly IRand rand;
    }
}
