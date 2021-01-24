// <copyright file="CopyProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.CopyProcedure class.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents an IProcedure that copies files form one directory
    /// into another directory.
    /// </summary>
    public sealed class CopyProcedure : IProcedure
    {        
        public CopyProcedure( string originalDirectory, string destinationDirectory, IFilesToCopyFinder filesToCopyFinder )
        {
            this.originalDirectory = originalDirectory;
            this.destinationDirectory = destinationDirectory;
            this.filesToCopyFinder = filesToCopyFinder;
        }

        /// <summary>
        /// Runs this CopyProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucesfully run;
        /// otherwise false.
        /// </returns>
        public bool Run()
        {
            this.CleanDesitination();
            this.CopyFiles();
            return true;
        }

        /// <summary>
        /// Cleans the files in the destination directory and makes sure
        /// that the destination actually exists.
        /// </summary>
        private void CleanDesitination()
        {
            if( Directory.Exists( this.destinationDirectory ) )
            {
                Directory.Delete( this.destinationDirectory, true );
            }

            Directory.CreateDirectory( this.destinationDirectory );
        }

        /// <summary>
        /// Copies the required files over from the source directory to the destination directory.
        /// </summary>
        private void CopyFiles()
        {
            var filesToCopy = this.filesToCopyFinder.GetFilesToCopy();

            foreach( string file in filesToCopy )
            {
                string destinationFile = GetDestinationFileName( file );
                string destinationDirectory = Path.GetDirectoryName( destinationFile );

                Directory.CreateDirectory( destinationDirectory );
                File.Copy( file, destinationFile );
            }
        }

        /// <summary>
        /// Gets the full name for the destination file a the source file.
        /// </summary>
        /// <param name="file">
        /// The name of the file in the source directory.
        /// </param>
        /// <returns>
        /// The name of the file in the destination directory.
        /// </returns>
        private string GetDestinationFileName( string file )
        {
            return file.Replace( this.originalDirectory, this.destinationDirectory );
        }

        private readonly string originalDirectory;
        private readonly string destinationDirectory;
        private readonly IFilesToCopyFinder filesToCopyFinder;
    }
}
