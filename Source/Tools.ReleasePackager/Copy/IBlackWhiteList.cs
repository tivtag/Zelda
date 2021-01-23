// <copyright file="IBlackWhiteList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.IBlackWhiteList interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager.Copy
{
    /// <summary>
    /// Provides a mechanism to filter a file from inclusion or declusion.
    /// </summary>
    public interface IBlackWhiteList
    {
        /// <summary>
        /// Gets a value indicating whether the specified extension
        /// is black or white listed.
        /// </summary>
        /// <param name="extension">
        /// The file extension to check.
        /// </param>
        /// <returns>
        /// true if the extension is included in this IBlackWhiteList;
        /// otherwise false.
        /// </returns>
        bool HasExtension( string extension );

        /// <summary>
        /// Gets a value indicating whether the specified fileName
        /// is black or white listed.
        /// </summary>
        /// <param name="fileName">
        /// The relative full path of the file to check.
        /// </param>
        /// <returns>
        /// true if the file with the specified fileName is included in this IBlackWhiteList;
        /// otherwise false.
        /// </returns>
        bool HasFile( string fileName );

        /// <summary>
        /// Gets a value indicating whether the specified fullPath
        /// is black or white listed because of its directory of sub directory.
        /// </summary>
        /// <param name="fullPath">
        /// The relative full path of the file to check.
        /// </param>
        /// <returns>
        /// true if the directory with the specified fullPath is included in this IBlackWhiteList;
        /// otherwise false.
        /// </returns>
        bool HasDirectory( string fullPath );
    }
}
