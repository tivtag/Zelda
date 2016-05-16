// <copyright file="IDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.IDrawDataAndStrategy interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Drawing
{
    using System;
    using System.ComponentModel;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Defines the interface of an object which contains the 
    /// data and strategy required to draw a <see cref="ZeldaEntity"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IDrawDataAndStrategy : Zelda.Saving.ISaveable, IZeldaUpdateable
    {
        /// <summary>
        /// Gets or sets the name of the Sprite Group of this <see cref="IDrawDataAndStrategy"/>-
        /// </summary>
        /// <remarks>
        /// Changing the Sprite Group won't have any effect until <see cref="Load( IZeldaServiceProvider )"/> is called.
        /// </remarks>
        string SpriteGroup { get; set; }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void Draw( ZeldaDrawContext drawContext );
        
        /// <summary>
        /// Loads the assets this IDrawDataAndStrategy requires.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        void Load( IZeldaServiceProvider serviceProvider );

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
        IDrawDataAndStrategy Clone( ZeldaEntity newOwner );
    }
}
