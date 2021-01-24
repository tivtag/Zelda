// <copyright file="ProjectileDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Drawing.ProjectileDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using System;
    using Atom.Xna;
    using Zelda.Entities.Drawing;
    
    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> is used
    /// to draw a <see cref="Projectile"/>.
    /// </summary>
    internal sealed class ProjectileDrawDataAndStrategy : IDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ISprite"/> used to draw the Projectile.
        /// </summary>
        public ISprite Sprite
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectileDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="projectile">
        /// The Projectile to visualize with the new <see cref="ProjectileDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="projectile"/> is null.
        /// </exception>
        public ProjectileDrawDataAndStrategy( Projectile projectile )
        {
            if( projectile == null )
                throw new ArgumentNullException( "projectile" );

            this.transform = projectile.Transform;
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
            var updateable = this.Sprite as Atom.IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            var drawPosition = transform.Position;
            drawPosition.X = (int)drawPosition.X;
            drawPosition.Y = (int)drawPosition.Y;

            this.Sprite.Draw( drawPosition, 0.0f, transform.Origin, transform.Scale, drawContext.Batch );
        }

        #region > Not Supported Operation <

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This operation is not supported.
        /// </exception>
        string IDrawDataAndStrategy.SpriteGroup
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Loads the assets needed by this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        void IDrawDataAndStrategy.Load( IZeldaServiceProvider serviceProvider )
        {
            throw new NotSupportedException();
        }

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
        IDrawDataAndStrategy IDrawDataAndStrategy.Clone( ZeldaEntity newOwner )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            throw new NotSupportedException();
        }     
        
        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ZeldaTransform component of the Projectile.
        /// </summary>
        private readonly Zelda.Entities.Components.ZeldaTransform transform;

        #endregion
    }
}