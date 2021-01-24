// <copyright file="IReadOnlyLocalizedNameable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IReadOnlyLocalizedNameable interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Represents an object that provides a mechanism for receiving the localized name of it.
    /// </summary>
    public interface IReadOnlyLocalizedNameable
    {
        /// <summary>
        /// Gets the localized name of this object.
        /// </summary>
        string LocalizedName 
        {
            get;
        }
    }
}
