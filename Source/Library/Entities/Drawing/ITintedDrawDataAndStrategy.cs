// <copyright file="ITintedDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.ITintedDrawDataAndStrategy interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using Microsoft.Xna.Framework;
    using Zelda.Graphics.Tinting;
    
    /// <summary>
    /// Defines the interface of an object which contains the
    /// data and strategy required to draw a tinted <see cref="ZeldaEntity"/>.
    /// </summary>
    public interface ITintedDrawDataAndStrategy : IDrawDataAndStrategy
    {
        /// <summary>
        /// Gets the <see cref="ColorTintList"/> this ITintedDrawDataAndStrategy is associated with.
        /// </summary>
        ColorTintList TintList
        {
            get;
        }

        /// <summary>
        /// Gets or sets the base tinting color used while drawing with this ITintedDrawDataAndStrategy.
        /// </summary>
        /// <value>The default value is Color.White.</value>
        Color BaseColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the final tinting color used while drawing with this ITintedDrawDataAndStrategy.
        /// </summary>
        Color FinalColor
        {
            get;
        }
    }
}
