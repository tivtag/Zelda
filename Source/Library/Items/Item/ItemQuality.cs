// <copyright file="ItemQuality.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemQuality enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the different qualities of <see cref="Item"/>s.
    /// </summary>
    public enum ItemQuality
    {
        /// <summary>
        /// No specific quality.
        /// </summary>
        None = 0,

        /// <summary>
        /// A quest-only item, these can't be sold. Color=Silver 
        /// </summary>
        Quest,

        /// <summary> Their only purpose is to be selled to vendors. Color=Grey </summary>
        Rubbish,

        /// <summary> Items that provide no stat-boni. Color=White </summary>
        Common,

        /// <summary> Magic items that provide stat-boni. Color=Green </summary>
        Magic,

        /// <summary> Rare items. Color=Blue  Better than Magic items.</summary>
        Rare,

        /// <summary> Epic items. Better than Magic items. Color=Purple </summary>
        Epic,

        /// <summary> Legendary items. Better than Epic items. Color=Orange </summary>
        Legendary,

        /// <summary>
        /// Only extremely rare items can be artefacts - these are the best items in the game.
        /// </summary>
        /// <remarks>
        /// Better than Legendary items. Color=Red
        /// </remarks>
        Artefact
    }
}
