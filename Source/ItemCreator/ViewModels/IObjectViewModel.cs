// <copyright file="IObjectViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.IObjectViewModel interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.ItemCreator
{
    /// <summary>
    /// Represents the interface that all object view-models
    /// the ItemCreator supports implement.
    /// </summary>
    public interface IObjectViewModel : Atom.Design.IObjectPropertyWrapper, Atom.INameable
    {
        /// <summary>
        /// Saves this IItemViewModel.
        /// </summary>
        void Save();
    }
}
