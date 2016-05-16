// <copyright file="ISavedState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ISavedState interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides a mechanism that allows one to receive whether 
    /// the object that implements the interface should be saved.
    /// </summary>
    public interface ISavedState
    {
        /// <summary>
        /// Gets a value indicating whether the object
        /// that implements this interface should be saved.
        /// </summary>
        bool IsSaved 
        {
            get;
        }
    }
}
