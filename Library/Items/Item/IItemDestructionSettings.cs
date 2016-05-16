// <copyright file="IItemDestructionSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.IItemDestructionSettings interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using Zelda.Entities;

    /// <summary>
    /// Provides a mechanism for deciding whether an ItemInstance
    /// is allowed to be possibly destructed.
    /// </summary>
    /// <remarks>
    /// Some items are required to progress in the game. As such it shouldn't
    /// be possible to destroy them using the Crafting Bottle or
    /// by dropping them onto the floor.
    /// </remarks>
    public interface IItemDestructionSettings
    {
        /// <summary>
        /// Gets a value indicating whether the specified ItemInstance is allowed to
        /// be possibly deleted.
        /// </summary>
        /// <param name="itemInstance">
        /// The item that might be deleted/destroyed.
        /// </param>
        /// <param name="owner">
        /// The entity that owns the given ItemInstance.
        /// </param>
        /// <returns>
        /// true if possible deletion is allowed;
        /// otherwise false.
        /// </returns>
        bool IsPossibleDeletionAllowed( ItemInstance itemInstance, PlayerEntity owner );
    }
}
