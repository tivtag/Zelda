// <copyright file="DrawingPipeline.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.DrawingPipeline class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics
{
    /// <summary>
    /// Enumerates the <see cref="IDrawingPipeline"/>s that have been implemented.
    /// </summary>
    public enum DrawingPipeline
    {
        /// <summary>
        /// Represents the NormalDrawingPipeline.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents the BloomDrawingPipeline.
        /// </summary>
        Bloom,

        /// <summary>
        /// Represents the HighDynamicRangeDrawingPipeline.
        /// </summary>
        HighDynamicRange
    }
}
