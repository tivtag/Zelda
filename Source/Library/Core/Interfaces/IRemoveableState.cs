// <copyright file="IRemoveableState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IRemoveableState interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda
{
    /// <summary>
    /// Provides a mechanism to receive a value
    /// indicating whether the object can be
    /// removed manually by the user.
    /// </summary>
    /// <remarks>
    /// Mostly used to restrict removement of the object
    /// in the editor.
    /// </remarks>
    public interface IRemoveableState
    {
        /// <summary>
        /// Gets a value indicating whether the object that
        /// implements this interface is allowed be removed
        /// manually by the user.
        /// </summary>
        bool IsRemoveAllowed 
        {
            get;
        }
    }
}
