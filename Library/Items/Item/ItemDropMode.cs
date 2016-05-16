// <copyright file="ItemDropMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemDropMode enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the different ItemDropModes an Enemy can have.
    /// </summary>
    public enum ItemDropMode
    {
        /// <summary>
        /// In Normal mode enemies drop 0 to 3 items at once.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// In Special mode enemies drop 1 to 4 items at once.
        /// </summary>
        Special,

        /// <summary>
        /// In Boss mode enemies drop 3 to 6 items at once.
        /// </summary>
        Boss,

        /// <summary>
        /// In Bad mode enemies drop 0 to 1 items at once. More often 0 than 1.
        /// </summary
        Bad,

        /// <summary>
        /// In Better mode enemies drop 1 to 3 items at once.
        /// </summary>
        Better
    }
}
