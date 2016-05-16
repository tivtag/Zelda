// <copyright file="UnrequiredFilesCleaner.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.UnrequiredFilesCleaner class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Atom.AutoUpdate.Manifest;

    /// <summary>
    /// Cleans/Deletes all the files the game doesn't require to run.
    /// </summary>
    public sealed class UnrequiredFilesCleaner : IAsyncProcess
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this UnrequiredFilesCleaner has completed.
        /// </summary>
        public event EventHandler Completed;
        
        /// <summary>
        /// Raised when this UnrequiredFilesCleaner has raised an error.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Errored;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Runs this UnrequiredFilesCleaner asynchronous.
        /// </summary>
        /// <param name="manifest">
        /// The IManifest that contains all the files
        /// the game depends on.
        /// </param>
        public void RunAsync( IManifest manifest )
        {
            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += ( sender, e ) => {
                this.files = Directory.GetFiles( contentPath, "*", SearchOption.AllDirectories );
                this.manifestFiles = manifest.Files.ToArray();

                this.CheckNextFile();
            };

            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Checks the next file in the list of files.
        /// </summary>
        private void CheckNextFile()
        {
            ++this.fileIndex;

            if( this.fileIndex >= this.files.Length )
            {                
                this.Completed( this, EventArgs.Empty );
            }
            else
            {
                this.TryCheckFile( this.files[this.fileIndex] );
                this.CheckNextFile();
            }
        }

        /// <summary>
        /// Checks whether the given file should be cleaned/deleted.
        /// </summary>
        /// <remarks>
        /// This method won't throw.
        /// </remarks>
        /// <param name="file">
        /// The full file path of the file to check.
        /// </param>
        private void TryCheckFile( string file )
        {
            try
            {
                CheckFile( file );
            }
            catch( Exception exc )
            {
                this.Errored( this, new ErrorEventArgs( exc ) );
            }
        }
        
        /// <summary>
        /// Checks whether the given file should be cleaned/deleted.
        /// </summary>
        /// <remarks>
        /// This method might throw.
        /// </remarks>
        /// <param name="file">
        /// The full file path of the file to check.
        /// </param>
        private void CheckFile( string file )
        {
            if( this.ShouldCleanFile( file ) )
            {
                this.CleanFile( file );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified file should be cleaned.
        /// </summary>
        /// <param name="file">
        /// The full file path of the file to check.
        /// </param>
        /// <returns>
        /// true if it should be cleaned; 
        /// otherwise false.
        /// </returns>
        private bool ShouldCleanFile( string file )
        {
            string relativeFile = file.Replace( applicationPath, string.Empty );

            return this.manifestFiles.FirstOrDefault(
                manifestFile => manifestFile.FileName.Equals( relativeFile, StringComparison.InvariantCultureIgnoreCase )
            ) == null;
        }

        /// <summary>
        /// Cleanes/Deletes the specified file.
        /// </summary>
        /// <param name="file">
        /// The full file path of the file to clean.
        /// </param>
        private void CleanFile( string file )
        {
            File.Delete( file );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The index of the current file.
        /// </summary>
        private int fileIndex = -1;

        /// <summary>
        /// The actual files on the hard-disc.
        /// </summary>
        private string[] files;
        
        /// <summary>
        /// The files the game depends on.
        /// </summary>
        private IManifestFile[] manifestFiles;

        /// <summary>
        /// The path of the application.
        /// </summary>
        private readonly string applicationPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// The path that contains the content of the game.
        /// </summary>
        private readonly string contentPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Content" );

        #endregion
    }
}
