// <copyright file="FourDirAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.FourDirAnimDrawDataAndStrategy class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains data for moving LEFT, RIGHT, UP and DOWN.
    /// </summary>
    /// <remarks>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X_Left, X_Right, X_Up, X_Down
    /// They are tequired to be AnimatedSprites.
    /// </remarks>
    public sealed class FourDirAnimDrawDataAndStrategy : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>-
        /// </summary>
        public string SpriteGroup
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FourDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <see cref="ZeldaEntity"/> doesn't own the <see cref="Zelda.Entities.Components.Moveable"/> component.
        /// </exception>
        public FourDirAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.moveable = entity.Components.Get<Components.Moveable>();

            if( this.moveable == null )
            {
                throw new ArgumentException( 
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                         Atom.ErrorStrings.EntityIsRequiredToHaveComponentType,
                        "Zelda.Entities.Components.Moveable" 
                    )
                );
            }

            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FourDirAnimDrawDataAndStrategy"/> class.
        /// </summary>
        internal FourDirAnimDrawDataAndStrategy()
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
        public void Update( ZeldaUpdateContext updateContext )
        {
            switch( this.entity.Transform.Direction )
            {
                case Direction4.Left:
                    this.current = animMoveLeft;
                    break;
                case Direction4.Right:
                    this.current = animMoveRight;
                    break;
                case Direction4.Up:
                    this.current = animMoveUp;
                    break;
                case Direction4.Down:
                    this.current = animMoveDown;
                    break;

                default:
                    break;
            }

            if( this.current != null )
            {
                if( this.moveable.IsStanding )
                {
                    this.current.Reset();
                }
                else
                {
                    this.current.Animate( updateContext.FrameTime );
                }
            }
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.current != null )
            {
                var drawPosition = this.entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.current.Draw( drawPosition, drawContext.Batch );
            }
        }
        
        /// <summary>
        /// Loads the assets needed by this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Load( IZeldaServiceProvider serviceProvider )
        {
            if( string.IsNullOrEmpty( this.SpriteGroup ) )
            {
                this.animMoveLeft = this.animMoveRight = this.animMoveUp = this.animMoveDown  = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                this.animMoveLeft  = new SpriteAnimation( spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Left" ) );
                this.animMoveRight = new SpriteAnimation( spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Right" ) );
                this.animMoveUp    = new SpriteAnimation( spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Up" ) );
                this.animMoveDown  = new SpriteAnimation( spriteLoader.LoadAnimatedSprite( this.SpriteGroup + "_Down" ) );
            }
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="FourDirAnimDrawDataAndStrategy"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            return new FourDirAnimDrawDataAndStrategy( newOwner ) {
                SpriteGroup   = this.SpriteGroup,

                animMoveLeft  = this.animMoveLeft  == null ? null : this.animMoveLeft.Clone(),
                animMoveRight = this.animMoveRight == null ? null : this.animMoveRight.Clone(),
                animMoveUp    = this.animMoveUp    == null ? null : this.animMoveUp.Clone(),
                animMoveDown  = this.animMoveDown  == null ? null : this.animMoveDown.Clone()
            };
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
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.SpriteGroup ?? string.Empty );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.SpriteGroup = context.ReadString();
        }

        #endregion
        
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
        /// The object that gets visualized by thise <see cref="FourDirAnimDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        #endregion
    }
}
