// <copyright file="FilesToCopyFinder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.FilesToCopyFinder class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager.Copy
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implements a mechanism that finds the files that should be copied
    /// from a specific location to another location.
    /// </summary>
    public sealed class FilesToCopyFinder : IFilesToCopyFinder
    {
        public FilesToCopyFinder( string originalDirectory, IFileCopyDecider fileCopyDecider )
        {
            this.originalDirectory = originalDirectory;
            this.fileCopyDecider = fileCopyDecider;
        }

        /// <summary>
        /// Gets the files that should be copied.
        /// </summary>
        /// <returns>
        /// The names of the files that should be copied.
        /// </returns>
        public IEnumerable<string> GetFilesToCopy()
        {
            var originalFiles = this.GetOriginalFiles();
            var filesToCopy = new List<string>();

            foreach( string file in originalFiles )
            {
                if( this.ShouldCopyFile( file ) )
                {
                    filesToCopy.Add( file );
                }
            }

            return filesToCopy;
        }

        /// <summary>
        /// Gets all files that are in the original source directory.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetOriginalFiles()
        {
            return Directory.GetFiles( this.originalDirectory, "*", SearchOption.AllDirectories );
        }

        /// <summary>
        /// Gets a value indicating whether the specified file should be copied.
        /// </summary>
        /// <param name="file">
        /// The file to check.
        /// </param>
        /// <returns>
        /// true if it should be copied;
        /// otherwise false.
        /// </returns>
        private bool ShouldCopyFile( string file )
        {
            string fileName = Path.GetFileName( file );
            return this.fileCopyDecider.ShouldCopy( fileName );
        }

        private readonly string originalDirectory;
        private readonly IFileCopyDecider fileCopyDecider;
    }
}
