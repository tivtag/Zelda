// <copyright file="TintedFourDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.TintedFourDirAnimDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains data for moving LEFT, RIGHT, UP and DOWN;
    /// and also tints the sprites in a specific color.
    /// </summary>
    /// <remarks>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X_Left, X_Right, X_Up, X_Down
    /// They are tequired to be AnimatedSprites.
    /// </remarks>
    public sealed class TintedFourDirAnimDrawDataAndStrategy : TintedDrawDataAndStrategy
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TintedFourDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <see cref="ZeldaEntity"/> doesn't own the <see cref="Zelda.Entities.Components.Moveable"/> component.
        /// </exception>
        public TintedFourDirAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.moveable = entity.Components.Get<Components.Moveable>();
            Atom.ThrowHelper.IfComponentNull( this.moveable );

            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the TintedFourDirAnimDrawDataAndStrategy class.
        /// </summary>
        internal TintedFourDirAnimDrawDataAndStrategy()
        {
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
            switch( this.entity.Transform.Direction )
            {
                case Direction4.Left:
                    current = animMoveLeft;
                    break;
                case Direction4.Right:
                    current = animMoveRight;
                    break;
                case Direction4.Up:
                    current = animMoveUp;
                    break;
                case Direction4.Down:
                    current = animMoveDown;
                    break;

                default:
                    break;
            }

            if( this.current != null )
            {
                if( this.moveable.IsStanding )
                    this.current.Reset();
                else
                    this.current.Animate( updateContext.FrameTime );
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( current != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( drawPosition, this.FinalColor, drawContext.Batch );
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
                animMoveLeft = animMoveRight =
                animMoveUp   = animMoveDown  = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                animMoveLeft  = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Left" ).CreateInstance();
                animMoveRight = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Right" ).CreateInstance();
                animMoveUp    = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Up" ).CreateInstance();
                animMoveDown  = spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Down" ).CreateInstance();
            }
        }

        /// <summary>
        /// Creates a clone of this <see cref="TintedFourDirAnimDrawDataAndStrategy"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new TintedFourDirAnimDrawDataAndStrategy( newOwner ) {
                animMoveLeft  = this.animMoveLeft  == null ? null : this.animMoveLeft.Clone(),
                animMoveRight = this.animMoveRight == null ? null : this.animMoveRight.Clone(),
                animMoveUp    = this.animMoveUp    == null ? null : this.animMoveUp.Clone(),
                animMoveDown  = this.animMoveDown  == null ? null : this.animMoveDown.Clone()
            };

            this.SetupClone( clone );
            return clone;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The movement animations.
        /// </summary>
        private SpriteAnimation animMoveLeft, animMoveRight, animMoveUp, animMoveDown;

        /// <summary>
        /// The current <see cref="SpriteAnimation"/>.
        /// </summary>
        private SpriteAnimation current;

        /// <summary>
        /// The object that gets visualized by thise <see cref="TintedFourDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        #endregion
    }
}
