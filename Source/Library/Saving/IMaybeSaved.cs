// <copyright file="IMaybeSaved.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.IMaybeSaved interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    /// <summary>
    /// Provides a mechanism that allows to control
    /// whether an object should be saved/serialized or not.
    /// </summary>
    public interface IMaybeSaved
    {
        /// <summary>
        /// Gets a value indicating whether an object
        /// should be saved.
        /// </summary>
        /// <returns>
        /// true if the object should be serialized;
        /// otherwise false.
        /// </returns>
        bool ShouldSerialize();
    }
}
