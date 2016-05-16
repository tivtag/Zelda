// <copyright file="ItemInfoColors.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfoColors class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items
{
    using Microsoft.Xna.Framework;
    using Atom.Xna;

    /// <summary>
    /// Provides access to the colors used to visualize the properties
    /// of items.
    /// </summary>
    internal static class ItemInfoColors
    {
        /// <summary>
        /// The color of the item information background.
        /// </summary>
        public static readonly Color Background = Color.Black.WithAlpha( Settings.Instance.ItemDescriptionBoxAlpha );

        /// <summary>
        /// The color of the description of an item.
        /// </summary>
        public static readonly Color Description = Color.LightGreen;
        
        /// <summary>
        /// The color of a not fulfilled requirement.
        /// </summary>
        public static readonly Color RequirementNotFulfilled = Color.Red;

        /// <summary>
        /// The color of a bad Status Effect.
        /// </summary>
        public static readonly Color BadEffect = Color.Red;

        /// <summary>
        /// The color of an additonal effect.
        /// </summary>
        public static readonly Color Effect = Color.White;

        /// <summary>
        /// The color of an additonal effect gained by gems.
        /// </summary>
        public static readonly Color GemEffect = Color.LightSkyBlue;

        /// <summary>
        /// The color of a set.
        /// </summary>
        public static readonly Color Set = Color.Green;

        /// <summary>
        /// The color of an effect of a set.
        /// </summary>
        public static readonly Color SetEffect = Color.White; // new Color( 100, 250, 100 );

        /// <summary>
        /// The color of a set item that has not beed equipped.
        /// </summary>
        public static readonly Color SetItemNotEquipped = Color.Gray;
    }
}
