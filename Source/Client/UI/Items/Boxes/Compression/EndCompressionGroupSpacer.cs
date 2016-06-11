// <copyright file="EndCompressionGroupSpacer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.EndCompressionGroupSpacer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items.Boxes
{
    /// <summary>
    /// Represents a marker box that indicates that a group of related IBoxes which should be compressed has ended.
    /// </summary>
    /// <seealso cref="BeginCompressionGroupSpacer"/>
    internal sealed class EndCompressionGroupSpacer : NoOpBox
    {
        /// <summary>
        /// Represents an instance of the <see cref="EndCompressionGroupSpacer"/> class.
        /// </summary>
        public static readonly IBox Instance = new EndCompressionGroupSpacer();

        /// <summary>
        /// Prevents the default creation of instances of the <see cref="EndCompressionGroupSpacer"/> class.
        /// </summary>
        private EndCompressionGroupSpacer()
        {
        }
    }
}
