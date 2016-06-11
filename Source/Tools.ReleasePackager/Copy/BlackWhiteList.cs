// <copyright file="BlackWhiteList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.ReleasePackager.Copy.BlackWhiteList class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.ReleasePackager.Copy
{
    using System;
    using System.Linq;

    /// <summary>
    /// Implements a mechanism that filters a file from inclusion or declusion.
    /// </summary>
    public sealed class BlackWhiteList : IBlackWhiteList
    {
        /// <summary>
        /// Initializes a new instance of the BlackWhiteList class.
        /// </summary>
        /// <param name="directories">
        /// The directory parts that are filtered.
        /// </param>
        /// <param name="files">
        /// The files that are directly filtered.
        /// </param>
        /// <param name="extensions">
        /// The file extensions that are filtered.
        /// </param>
        internal BlackWhiteList( string[] directories, string[] files, string[] extensions )
        {
            this.directories = directories;
            this.files = files;
            this.extensions = extensions;
        }

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
        public bool HasExtension( string extension )
        {
            return this.extensions.Contains( 
                extension,
                StringComparer.InvariantCultureIgnoreCase
            );
        }

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
        public bool HasFile( string fileName )
        {
            return this.files.Contains( fileName );
        }

        /// <summary>
        /// Gets a value indicating whether the specified fullPath
        /// is black or white listed because of its directory of sub directory.
        /// </summary>
        /// <param name="directory">
        /// The relative full path of the file to check.
        /// </param>
        /// <returns>
        /// true if the directory with the specified fullPath is included in this IBlackWhiteList;
        /// otherwise false.
        /// </returns>
        public bool HasDirectory( string fullPath )
        {
            return this.directories.Any( directory => fullPath.Contains( directory ) );
        }
        
        /// <summary>
        /// The directory parts that are filtered.
        /// </summary>
        private string[] directories;

        /// <summary>
        /// The files that are directly filtered.
        /// </summary>
        private string[] files;

        /// <summary>
        /// The file extensions that are filtered.
        /// </summary>
        private string[] extensions;
    }
}
