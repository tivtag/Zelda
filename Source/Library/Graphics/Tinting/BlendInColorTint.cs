// <copyright file="BlendInColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.BlendInColorTint class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting
{
    using Atom.Math;

    /// <summary>
    /// Defines a <see cref="TimedColorTint"/> that blends
    /// the alpha channel of the color in.
    /// </summary>
    public class BlendInColorTint : TimedColorTint
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
            color.W *= (1.0f - this.Factor);
            return color;
        }
    }
}
