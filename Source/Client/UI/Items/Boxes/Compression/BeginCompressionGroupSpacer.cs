// <copyright file="BeginCompressionGroupSpacer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.BeginCompressionGroupSpacer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items.Boxes
{
    /// <summary>
    /// Represents a marker box that is used when compression is enabled
    /// to seperate two differently related groups of IBoxes.
    /// </summary>
    internal sealed class BeginCompressionGroupSpacer : NoOpBox
    {
        /// <summary>
        /// Represents an instance of the <see cref="BeginCompressionGroupSpacer"/> class.
        /// </summary>
        public static readonly IBox Instance = new BeginCompressionGroupSpacer();

        /// <summary>
        /// Prevents the default creation of instances of the <see cref="BeginCompressionGroupSpacer"/> class.
        /// </summary>
        private BeginCompressionGroupSpacer()
        {
        }
    }
}
