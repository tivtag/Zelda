// <copyright file="ProjectileSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.ProjectileSettings class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Projectiles
{
    using Atom.Math;
    using Zelda.Audio;
    using Zelda.Entities.Projectiles.Drawing;
    
    /// <summary>
    /// Encapsulates settings related to a Projectile.
    /// </summary>
    /// <remarks>
    /// Projectiles can pierce their target and continue traveling.
    /// </remarks>
    public sealed class ProjectileSettings
    {
        /// <summary>
        /// The default movement speed of Projectiles.
        /// </summary>
        public static readonly IntegerRange DefaultSpeed = new IntegerRange( 99, 103 );
        
        /// <summary>
        /// Gets or sets the <see cref="IProjectileSprites"/> used to visualize the Projectiles.
        /// </summary>
        public IProjectileSprites Sprites
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the movement speed of the Projectiles releases by this RangedAttack.
        /// </summary>
        public IntegerRange Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        /// <summary>
        /// Gets or sets the range the Projectiles can travel before they get destroyed; squared.
        /// </summary>
        public float TravelRange
        {
            get
            {
                return (float)System.Math.Sqrt( travelRangeSquared );
            }

            set
            {
                this.travelRangeSquared = value * value;
            }
        }

        /// <summary>
        /// Gets the range the Projectiles can travel before they get destroyed.
        /// </summary>
        public float TravelRangeSquared
        {
            get
            {
                return this.travelRangeSquared;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ProjectilePiercingChanceMode"/> that states
        /// how the chance a Projectile can pierce its target is.
        /// </summary>
        public ProjectilePiercingChanceMode PiercingMode
        {
            get { return this.piercingMode; }
            set { this.piercingMode = value; }
        }

        /// <summary>
        /// Gets or sets the additional chance of the Projectiles 
        /// to pierce through its target.
        /// </summary>
        /// <value>
        /// The default value is 0.
        /// </value>
        public float AdditionalPiercingChance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the Projectiles
        /// have a chance to pierce its target.
        /// </summary>
        public bool CanPierce
        {
            get { return this.piercingMode != ProjectilePiercingChanceMode.None; }
        }

        /// <summary>
        /// Gets or sets the SoundSampleSettings that encapsulates what sound 
        /// is played when the Projectile is travelling.
        /// </summary>
        /// <value>The default value is null.</value>
        public SoundSampleSettings TravellingSound
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IStatefulProjectileUpdateLogic"/> that
        /// is used as a template for the custom update logic of the Projectiles.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public IStatefulProjectileUpdateLogic StatefulUpdateLogicTemplate
        {
            get;
            set;
        }
        
        /// <summary>
        /// Setups this ProjectileSettings.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( this.TravellingSound != null )
            {
                this.TravellingSound.Setup( serviceProvider );
            }
        }
        
        /// <summary>
        /// Gets the IStatefulProjectileUpdateLogic used for a new Projectile.
        /// </summary>
        /// <returns>
        /// A new IStatefulProjectileUpdateLogic instance;
        /// or null.
        /// </returns>
        internal IStatefulProjectileUpdateLogic GetStatefulUpdateLogicInstance()
        {
            if( this.StatefulUpdateLogicTemplate == null )
            {
                return null;
            }
            else
            {
                return (IStatefulProjectileUpdateLogic)this.StatefulUpdateLogicTemplate.Clone();
            }
        }

        /// <summary>
        /// Gets the chance a Projectile can pierce its targets.
        /// </summary>
        /// <param name="creator">
        /// The statable component of the entity that has fired the Projectile.
        /// </param>
        /// <returns>
        /// The chance to pierce in %.
        /// </returns>
        public float GetChanceToPierce( Zelda.Status.Statable creator )
        {
            switch( this.piercingMode )
            {
                case ProjectilePiercingChanceMode.Combined:
                    return creator.ChanceTo.Pierce + this.AdditionalPiercingChance;

                case ProjectilePiercingChanceMode.OnlyEntity:
                    return creator.ChanceTo.Pierce;

                case ProjectilePiercingChanceMode.OnlyAdditional:
                    return this.AdditionalPiercingChance;

                default:
                case ProjectilePiercingChanceMode.None:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Sets the sprites that should be used to visualize the Projectile.
        /// </summary>
        /// <param name="sprite">
        /// The AnimatedSprite that should be used as a template for all Projectiles.
        /// </param>
        public void SetSprites( Atom.Xna.AnimatedSprite sprite )
        {
            this.Sprites = new SingleAnimatedProjectileSprites( sprite );
        }
        
        /// <summary>
        /// Sets the sprites that should be used to visualize the Projectile.
        /// </summary>
        /// <param name="spriteProjectileUp">
        /// The sprite that represents a projectile that is traveling up.
        /// </param>
        /// <param name="spriteProjectileDown">
        /// The sprite that represents a projectile that is traveling down.
        /// </param>
        /// <param name="spriteProjectileLeft">
        /// The sprite that represents a projectile that is traveling left.
        /// </param>
        /// <param name="spriteProjectileRight">
        /// The sprite that represents a projectile that is traveling right.
        /// </param>
        public void SetSprites(
            Atom.Xna.Sprite spriteProjectileUp,
            Atom.Xna.Sprite spriteProjectileDown,
            Atom.Xna.Sprite spriteProjectileLeft,
            Atom.Xna.Sprite spriteProjectileRight )
        {
            this.Sprites = new ProjectileSprites(
                spriteProjectileLeft,
                spriteProjectileRight,
                spriteProjectileDown,
                spriteProjectileUp
            );
        }

        /// <summary>
        /// The backend storage field of the PiercingMode property.
        /// </summary>
        private ProjectilePiercingChanceMode piercingMode = ProjectilePiercingChanceMode.Combined;

        /// <summary>
        /// The movement speed of the Projectiles.
        /// </summary>
        private IntegerRange speed = DefaultSpeed;

        /// <summary>
        /// The range the Projectiles can travel before they get destroyed; squared.
        /// </summary>
        private float travelRangeSquared = 1000.0f;
    }
}
