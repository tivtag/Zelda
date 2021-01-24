// <copyright file="IValueStorage.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.IValueStorage{T} interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving.Storage
{
    /// <summary>
    /// Provides a place to store a single value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value.
    /// </typeparam>
    public interface IValueStorage<T> : IStorage
    {
        /// <summary>
        /// Gets or sets the value stored in this IValueStorage{T}.
        /// </summary>
        T Value
        {
            get;
            set;
        }
    }
}
