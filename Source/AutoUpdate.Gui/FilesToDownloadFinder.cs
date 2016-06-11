// <copyright file="FilesToDownloadFinder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.FilesToDownloadFinder class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Atom.AutoUpdate;
    using Atom.AutoUpdate.Manifest;

    public enum UpdateReason
    {
        None,
        FileMissing,
        InvalidHash
    }

    /// <summary>
    /// Responsible for finding the files that must be downloaded and updated.
    /// </summary>
    public sealed class FilesToDownloadFinder
    {
        /// <summary>
        /// Initializes a new instance of the FilesToDownloadFinder class.
        /// </summary>
        /// <param name="manifest">
        /// The IManifest that has been downloaded from the server.
        /// </param>
        public FilesToDownloadFinder( IManifest manifest )
        {
            this.hasher = new MD5FileHasher();
            this.manifest = manifest;
        }

        /// <summary>
        /// Finds the files that should be downloaded.
        /// </summary>
        /// <returns></returns>
        public IList<Tuple<UpdateReason, IManifestFile>> Find()
        {
            var list = new List<Tuple<UpdateReason, IManifestFile>>();

            foreach( var file in manifest.Files )
            {
                UpdateReason reason = this.ShouldUpdateFile( file );

                if( reason != UpdateReason.None )
                {
                    list.Add( Tuple.Create( reason, file ) );
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a value indicating whether the specified file
        /// should be downloaded.
        /// </summary>
        /// <param name="file">
        /// The file to check.
        /// </param>
        /// <returns></returns>
        private UpdateReason ShouldUpdateFile( IManifestFile file )
        {
            string filePath = this.GetFilePath( file );
            if( !File.Exists( filePath ) )
                return UpdateReason.FileMissing;

            if( IsUpdaterStubFile( file ) )
                return UpdateReason.None;

            if( HasDownloadHash( file ) )
            {
                if( IsValidHash( file, filePath ) )
                {
                    return UpdateReason.None;
                }
                else
                {
                    return UpdateReason.InvalidHash;
                }
            }
            else
            {
                return UpdateReason.InvalidHash;
            }
        }



        private static bool HasDownloadHash( IManifestFile file )
        {
            return !string.IsNullOrEmpty( file.FileHash );
        }

        private static bool IsFileOutdated( IManifestFile file, string filePath )
        {
            return file.FileModificationTime > File.GetLastWriteTime( filePath );
        }

        private bool IsValidHash( IManifestFile file, string filePath )
        {
            if( string.IsNullOrEmpty( file.FileHash ) )
            {
                return true;
            }

            try
            {
                using( var stream = File.OpenRead( filePath ) )
                {
                    string hash = this.hasher.GetHash( stream );
                    return file.FileHash.Equals( hash, StringComparison.OrdinalIgnoreCase );
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified IManifestFile
        /// is actually the stub file that is running the Updater.
        /// This file can't be updated using the updater.
        /// </summary>
        /// <param name="file">
        /// The file to check.
        /// </param>
        /// <returns></returns>
        private bool IsUpdaterStubFile( IManifestFile file )
        {
            return file.FileName.Equals( "Zelda Updater.exe", StringComparison.InvariantCulture );
        }

        /// <summary>
        /// Gets the full path on the filesystem for the given IManifestFile.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetFilePath( IManifestFile file )
        {
            return Path.Combine( applicationPath, file.FileName );
        }

        private readonly IFileHasher hasher;
        private readonly IManifest manifest;
        private readonly string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
    }
}
