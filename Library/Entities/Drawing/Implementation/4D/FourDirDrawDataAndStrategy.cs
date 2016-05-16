// <copyright file="FourDirDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.FourDirDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/>
    /// which contains data for standing LEFT, RIGHT, UP and DOWN.
    /// </summary>
    /// <remarks>
    /// This IDrawDataAndStrategy's sprite group layout is, X is the SpriteGroup:
    /// X_Left, X_Right, X_Up, X_Down
    /// They are required to be normal Sprites.
    /// </remarks>
    public sealed class FourDirDrawDataAndStrategy : IDrawDataAndStrategy
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

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FourDirDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">The entity to visualize.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public FourDirDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the FourDirDrawDataAndStrategy class.
        /// </summary>
        internal FourDirDrawDataAndStrategy()
        {
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
                this.spriteLeft = this.spriteRight = this.spriteUp = this.spriteDown = null;
            }
            else
            {
                var spriteLoader = serviceProvider.SpriteLoader;

                this.spriteLeft  = spriteLoader.LoadSprite( this.SpriteGroup + "_Left" );
                this.spriteRight = spriteLoader.LoadSprite( this.SpriteGroup + "_Right" );
                this.spriteUp    = spriteLoader.LoadSprite( this.SpriteGroup + "_Up" );
                this.spriteDown  = spriteLoader.LoadSprite( this.SpriteGroup + "_Down" );
            }
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
            switch( entity.Transform.Direction )
            {
                case Direction4.Left:
                    current = spriteLeft;
                    break;
                case Direction4.Right:
                    current = spriteRight;
                    break;
                case Direction4.Up:
                    current = spriteUp;
                    break;
                case Direction4.Down:
                    current = spriteDown;
                    break;

                default:
                    break;
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
            if( current != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                current.Draw( drawPosition, drawContext.Batch );
            }
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="FourDirDrawDataAndStrategy"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        public IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            return new FourDirDrawDataAndStrategy( newOwner )
            {
                SpriteGroup   = this.SpriteGroup,

                spriteLeft  = this.spriteLeft,
                spriteRight = this.spriteRight,
                spriteUp    = this.spriteUp,
                spriteDown  = this.spriteDown
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
        /// The sprites.
        /// </summary>
        private Sprite spriteLeft, spriteRight, spriteUp, spriteDown;

        /// <summary>
        /// The current <see cref="Sprite"/>.
        /// </summary>
        private Sprite current;

        /// <summary>
        /// The object that gets visualized by thise <see cref="FourDirDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        #endregion
    }
}
