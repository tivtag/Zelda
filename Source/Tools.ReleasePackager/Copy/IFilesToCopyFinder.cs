// <copyright file="IFilesToCopyFinder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.IFilesToCopyFinder interface.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a mechanism that receives the files that
    /// are supposed to be copied.
    /// </summary>
    public interface IFilesToCopyFinder
    {
        /// <summary>
        /// Gets the files that should be copied.
        /// </summary>
        /// <returns>
        /// The names of the files that should be copied.
        /// </returns>
        IEnumerable<string> GetFilesToCopy();
    }
}
