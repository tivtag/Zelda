// <copyright file="AllowAllFileCopyDecider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.AllowAllFileCopyDecider class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager.Copy
{
    /// <summary>
    /// Implements an IFileCopyDecider that decides to copy -all- files.
    /// </summary>
    public sealed class AllowAllFileCopyDecider : IFileCopyDecider
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
        public bool ShouldCopy( string fileName ) => true;
    }
}
