// <copyright file="IFileCopyDecider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.IFileCopyDecider interface.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    /// <summary>
    /// Provides a mechanism that decides whether a file should be copied.
    /// </summary>
    public interface IFileCopyDecider
    {
        /// <summary>
        /// Gets a value indicating whether the file with the 
        /// specified fileName should be copied.
        /// </summary>
        /// <param name="fileName">
        /// The relative full path of the file.
        /// </param>
        /// <returns>
        /// true if it should be copied;
        /// otherwise false.
        /// </returns>
        bool ShouldCopy( string fileName );
    }
}
