// <copyright file="IColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.IColorTint interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting
{
    using System.ComponentModel;
    using Atom.Math;

    /// <summary>
    /// Provides a mechanism that allows one to apply a tinting
    /// effect to a color.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IColorTint : Zelda.Saving.ISaveable, IZeldaUpdateable
    {
        /// <summary>
        /// Applies this IColorTint to the given color.
        /// </summary>
        /// <param name="color">
        /// The input color.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        Vector4 Apply( Vector4 color );
    }
}
