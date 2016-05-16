// <copyright file="LinearColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.LinearColorTint class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Tinting
{
    using Atom.Math;

    /// <summary>
    /// Defines a TimedColorTint that linearly interpolates between the input color
    /// and a specified FinalColor.
    /// </summary>
    public sealed class LinearColorTint : TimedColorTint
    {
        /// <summary>
        /// Gets or sets the final color this LinearColorTint interpolates to.
        /// </summary>
        public Vector4 FinalColor
        {
            get;
            set;
        }

        /// <summary>
        /// Applies this IColorTint to the given color.
        /// </summary>
        /// <param name="color">
        /// The input color.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        public override Atom.Math.Vector4 Apply( Atom.Math.Vector4 color )
        {
            return Vector4.Lerp( color, this.FinalColor, (1.0f - this.Factor) );
        }
    }
}
