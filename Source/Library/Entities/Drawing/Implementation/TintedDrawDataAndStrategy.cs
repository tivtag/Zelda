// <copyright file="TintedDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.TintedDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using Atom.Xna;
    using Microsoft.Xna.Framework;
    using Zelda.Graphics.Tinting;

    /// <summary>
    /// Defines a base implementation of the <see cref="ITintedDrawDataAndStrategy"/> interface.
    /// </summary>
    public abstract class TintedDrawDataAndStrategy : ITintedDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>-
        /// </summary>
        /// <remarks>
        /// Changing the Sprite Group won't have any effect until <see cref="Load"/> is called.
        /// </remarks>
        public string SpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="ColorTintList"/> this TintedDrawDataAndStrategy is associated with.
        /// </summary>
        public ColorTintList TintList
        {
            get
            {
                return this.tintList;
            }
        }

        /// <summary>
        /// Gets or sets the base tinting color used while drawing with this TintedDrawDataAndStrategy.
        /// </summary>
        /// <value>The default value is Color.White.</value>
        public Color BaseColor
        {
            get
            {
                return this.baseColor;
            }

            set
            {
                this.baseColor = value;
            }
        }

        /// <summary>
        /// Gets the final tinting color used while drawing with this TintedDrawDataAndStrategy.
        /// </summary>
        public Color FinalColor
        {
            get
            {
                return this.tintList.ApplyTinting( this.baseColor );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this TintedDrawDataAndStrategy.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            this.tintList.Update( updateContext );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( this.SpriteGroup ?? string.Empty );
            context.Write( this.baseColor );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.SpriteGroup = context.ReadString();
            this.baseColor = context.ReadColor();
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public abstract void Draw( ZeldaDrawContext drawContext );

        /// <summary>
        /// Loads the assets this IDrawDataAndStrategy requires.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public abstract void Load( IZeldaServiceProvider serviceProvider );

        /// <summary>
        /// Clones this <see cref="IDrawDataAndStrategy"/> for use by the specified object.
        /// </summary>
        /// <param name="newOwner">
        /// The new owner of the cloned <see cref="IDrawDataAndStrategy"/>.
        /// </param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If the <paramref name="newOwner"/> is null.
        /// </exception>
        public abstract IDrawDataAndStrategy Clone( ZeldaEntity newOwner );

        /// <summary>
        /// Setups a clone of this TintedDrawDataAndStrategy.
        /// </summary>
        /// <param name="clone">
        /// The TintedDrawDataAndStrategy to setup as a clone of this TintedDrawDataAndStrategy.
        /// </param>
        protected void SetupClone( TintedDrawDataAndStrategy clone )
        {
            clone.baseColor   = this.baseColor;
            clone.SpriteGroup = this.SpriteGroup;
        }

        /// <summary>
        /// Helper function that creates a clone of the given SpriteAnimation.
        /// </summary>
        /// <param name="animation">
        /// The SpriteAnimation to clone. Can be null.
        /// </param>
        /// <returns>
        /// The cloned SpriteAnimation. Might be null.
        /// </returns>
        protected static Atom.Xna.SpriteAnimation GetSpriteAnimationClone( Atom.Xna.SpriteAnimation animation )
        {
            if( animation == null )
                return null;

            return animation.Clone();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The base color that is used for tinting; before any IColorTints are applied.
        /// </summary>
        private Color baseColor = Color.White;

        /// <summary>
        /// The list of IColorTints applied to the BaseColor to make the FinalColor.
        /// </summary>
        private readonly ColorTintList tintList = new ColorTintList();

        #endregion
    }
}