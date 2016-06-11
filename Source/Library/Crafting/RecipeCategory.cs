// <copyright file="RecipeCategory.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.RecipeCategory enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Crafting
{
    /// <summary>
    /// Enumerates the various categories into which <see cref="Recipe"/>s
    /// are grouped.
    /// </summary>
    public enum RecipeCategory : byte
    {
        /// <summary>
        /// Represents all categories together.
        /// </summary>
        All,

        /// <summary>
        /// All buff food; includes potions.
        /// </summary>
        Food,

        /// <summary>
        /// All weapons; including melee, ranged and offhand.
        /// </summary>
        Weapons,

        /// <summary>
        /// All defensive offhand items.
        /// </summary>
        Shields,

        /// <summary>
        /// All armor items; including chests, belts, heads and boots.
        /// </summary>
        Armor,

        /// <summary>
        /// All items that don't git in any of the other categories.
        /// </summary>
        Miscellaneous,

        /// <summary>
        /// All jewelry; including rings, trinkets, necklaces and relics.
        /// </summary>
        Jewelry
    }
}
