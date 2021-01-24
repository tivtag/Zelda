// <copyright file="BlendOutColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.BlendOutColorTint class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting
{
    using Atom.Math;

    /// <summary>
    /// Defines a <see cref="TimedColorTint"/> that blends
    /// the alpha channel of the color out.
    /// </summary>
    public class BlendOutColorTint : TimedColorTint
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
        public override Vector4 Apply( Vector4 color )
        {
            color *= this.Factor;
            return color;
        }
    }
}
