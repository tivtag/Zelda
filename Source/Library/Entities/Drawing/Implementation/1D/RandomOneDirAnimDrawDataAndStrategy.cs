// <copyright file="RandomOneDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.RandomOneDirAnimDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws
    /// the same Animated Sprite for all actions and directions.
    /// The animation is only animated once in a while.
    /// </summary>
    /// <remarks>
    /// SpriteGroup format, where X is the SpriteGroup:
    /// 'X'
    /// </remarks>
    public sealed class RandomOneDirAnimDrawDataAndStrategy : TintedDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="OneDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        public SpriteAnimation Animation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range of time that can be between animating the Animation.
        /// </summary>
        public Atom.Math.FloatRange TimeBetweenAnimations
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Animation
        /// is currently animating.
        /// </summary>
        private bool IsAnimating
        {
            get
            {
                return this.Animation.IsAnimatingEnabled;
            }

            set
            {
                this.Animation.IsAnimatingEnabled = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomOneDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="RandomOneDirAnimDrawDataAndStrategy"/>.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public RandomOneDirAnimDrawDataAndStrategy( ZeldaEntity entity, Atom.Math.RandMT rand )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity;
            this.rand = rand;
        }

        /// <summary>
        /// Initializes a new instance of the RandomOneDirAnimDrawDataAndStrategy class.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        internal RandomOneDirAnimDrawDataAndStrategy( Atom.Math.RandMT rand )
        {
            this.rand = rand;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.IsAnimating )
            {
                this.Animation.Animate( updateContext.FrameTime );

                if( this.Animation.Time >= this.Animation.TotalTime )
                {
                    this.IsAnimating = false;
                    this.Animation.Reset(); 
                    this.RandomizeTimeLeftUntilAnimation();
                }
            }
            else
            {
                this.timeLeftUntilAnimation -= updateContext.FrameTime;

                if( this.timeLeftUntilAnimation <= 0.0f )
                {
                    this.IsAnimating = true;
                }
            }
        }

        /// <summary>
        /// Randomizes the value of the timeLeftUntilAnimation field
        /// to a value within <see cref="TimeBetweenAnimations"/>.
        /// </summary>
        private void RandomizeTimeLeftUntilAnimation()
        {
            this.timeLeftUntilAnimation = this.TimeBetweenAnimations.GetRandomValue( this.rand );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( this.Animation != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.Animation.Draw( drawPosition, this.FinalColor, drawContext.Batch );
            }
        }

        /// <summary>
        /// Loads the assets needed by this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Load( IZeldaServiceProvider serviceProvider )
        {
            if( string.IsNullOrEmpty( this.SpriteGroup ) )
            {
                this.Animation = null;
            }
            else
            {
                this.Animation = serviceProvider.SpriteLoader.LoadAnimatedSprite( this.SpriteGroup ).CreateInstance();
                
                this.Animation.IsLooping = false;
                this.Animation.IsAnimatingEnabled = false;
            }
        }

        #region > Cloning <

        /// <summary>
        /// Clones this <see cref="IDrawDataAndStrategy"/> for use by the specified object.
        /// </summary>
        /// <param name="newOwner">
        /// The new owner of the cloned <see cref="IDrawDataAndStrategy"/>.
        /// </param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="newOwner"/> is null.
        /// </exception>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new RandomOneDirAnimDrawDataAndStrategy( newOwner, this.rand ) {
                TimeBetweenAnimations = this.TimeBetweenAnimations,
                Animation = this.Animation != null ? this.Animation.Clone() : null
            };

            if( clone.Animation != null )
            {
                clone.Animation.IsAnimatingEnabled = false;
            }

            clone.RandomizeTimeLeftUntilAnimation();
            this.SetupClone( clone );

            return clone;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.TimeBetweenAnimations );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.TimeBetweenAnimations = context.ReadFloatRange();
            this.RandomizeTimeLeftUntilAnimation();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The time left in seconds until the Animation
        /// is animated for one time.
        /// </summary>
        private float timeLeftUntilAnimation;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly Atom.Math.RandMT rand;

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        #endregion
    }
}
