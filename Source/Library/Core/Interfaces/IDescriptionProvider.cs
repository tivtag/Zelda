// <copyright file="IDescriptionProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IDescriptionProvider interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Defines an interface which provides a description.
    /// </summary>
    public interface IDescriptionProvider
    {
        /// <summary>
        /// Gets the description provided by this IDescriptionProvider.
        /// </summary>
        string Description 
        { 
            get;
        }
    }
}
