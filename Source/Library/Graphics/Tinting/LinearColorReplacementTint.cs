// <copyright file="LinearColorReplacementTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.LinearColorReplacementTint class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Tinting
{
    using Atom.Math;

    /// <summary>
    /// Defines a TimedColorTint that linearly interpolates between a specified InitialColor
    /// and a specified FinalColor.
    /// </summary>
    /// <remarks>
    /// The input color is completly discarded.
    /// </remarks>
    public sealed class LinearColorReplacementTint : TimedColorTint
    {
        /// <summary>
        /// Gets or sets the initial color this LinearColorReplacementTint interpolates from.
        /// </summary>
        public Vector4 InitialColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the final color this LinearColorReplacementTint interpolates to.
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
        /// The input color. Unused.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        public override Atom.Math.Vector4 Apply( Atom.Math.Vector4 color )
        {
            return Vector4.Lerp( this.InitialColor, this.FinalColor, (1.0f - this.Factor) );
        }
    }
}
