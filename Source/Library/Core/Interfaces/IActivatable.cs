// <copyright file="IActivatable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IActivatable interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    /// <summary>
    /// Represents an object that can be either active or inactive.
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the object is currently active.
        /// </summary>
        bool IsActive
        {
            get;
            set;
        }
    }
}
