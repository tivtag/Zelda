// <copyright file="DualSwitchableDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.DualSwitchableDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws a <see cref="ZeldaEntity"/>
    /// that implements the <see cref="ISwitchable"/> interface. A different <see cref="ISprite"/> 
    /// is drawn depending on the current switch state of the ZeldaEntity.
    /// </summary>
    public sealed class DualSwitchableDrawDataAndStrategy : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the sprite that is
        /// used when the ISwitchable ZeldaEntity is switched on.
        /// </summary>
        public string SpriteOnName
        {
            get 
            {
                return ((IDrawDataAndStrategy)this).SpriteGroup; 
            }

            set
            { 
                ((IDrawDataAndStrategy)this).SpriteGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite that is used when
        /// the ISwitchable ZeldaEntity is switched on.
        /// </summary>
        public ISprite SpriteOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the sprite that is
        /// used when the ISwitchable ZeldaEntity is switched off.
        /// </summary>
        public string SpriteOffName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sprite that is used when
        /// the ISwitchable ZeldaEntity is switched off.
        /// </summary>
        public ISprite SpriteOff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <remarks>We store the SpriteOnName in this.</remarks>
        string IDrawDataAndStrategy.SpriteGroup
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DualSwitchableDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="DualSwitchableDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <see cref="ZeldaEntity"/> doesn't implement the required <see cref="ISwitchable"/> interface.
        /// </exception>
        public DualSwitchableDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity     = entity;
            this.switchable = entity as ISwitchable;

            if( this.switchable == null )
            {
                throw new ArgumentException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_EntityIsRequiredToImplementInterfaceX,
                        "ISwitchable"
                    ),
                    "entity"
                );
            }
        }

        /// <summary>
        /// Initializes a new instance of the DualSwitchableDrawDataAndStrategy class.
        /// </summary>
        internal DualSwitchableDrawDataAndStrategy()
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
            var spriteLoader = serviceProvider.SpriteLoader;

            // Load SpriteOn:
            if( string.IsNullOrEmpty( this.SpriteOnName ) )
            {
                this.SpriteOn = null;
            }
            else
            {
                this.SpriteOn = spriteLoader.Load( this.SpriteOnName );
            }

            // Load SpriteOff:
            if( string.IsNullOrEmpty( this.SpriteOffName ) )
            {
                this.SpriteOff = null;
            }
            else
            {
                this.SpriteOff = spriteLoader.Load( this.SpriteOffName );
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
            if( switchable.IsSwitched )
            {
                currentSprite = this.SpriteOn;
            }
            else
            {
                currentSprite = this.SpriteOff;
            }

            var updateable = currentSprite as IUpdateable;

            if( updateable != null )
                updateable.Update( updateContext );
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.currentSprite != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                currentSprite.Draw( drawPosition, drawContext.Batch );
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
        public IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            return new DualSwitchableDrawDataAndStrategy( newOwner ) {
                SpriteOnName  = this.SpriteOnName,
                SpriteOn      = this.SpriteOn,
                SpriteOffName = this.SpriteOffName,
                SpriteOff     = this.SpriteOff
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
            context.Write( this.SpriteOnName ?? string.Empty );
            context.Write( this.SpriteOffName ?? string.Empty );
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
            this.SpriteOnName = context.ReadString();
            this.SpriteOffName = context.ReadString();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the currently drawn sprite.
        /// </summary>
        private ISprite currentSprite;

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// The ZeldaEntity cast to an <see cref="ISwitchable"/>, cached.
        /// </summary>
        private readonly ISwitchable switchable;

        #endregion
    }
}
