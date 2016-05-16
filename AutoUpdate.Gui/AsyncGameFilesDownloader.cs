// <copyright file="AsyncGameFilesDownloader.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.AsyncGameFilesDownloader class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Atom.AutoUpdate;
    using Atom.AutoUpdate.Manifest;
    using Zelda.Updater.Properties;

    /// <summary>
    /// Downloads and updates game files asynchronously.
    /// </summary>
    public sealed class AsyncGameFilesDownloader : IAsyncProcess
    {
        /// <summary>
        /// Raised when this AsyncGameFileDownloader has completed downloading all
        /// required files.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Raised when an error has occurred during the download.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Errored;

        /// <summary>
        /// Raised when a file download progress has occurred.
        /// </summary>
        public event EventHandler<GameFilesDownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Gets the number of files that have bee sucessfully downloaded.
        /// </summary>
        public int SucessfulDownloadCount
        {
            get
            {
                return this.sucessfulDownloadCount;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AsyncGameFilesDownloader class.
        /// </summary>
        /// <param name="downloader">
        /// The IDownloader that should be used for downloading the game files.
        /// </param>
        public AsyncGameFilesDownloader( IDownloader downloader )
        {
            this.downloader = downloader;

            this.downloader.AsyncDownloadCompleted += this.OnAsyncDownloadCompleted;
            this.downloader.AsyncDownloadProgressChanged += this.OnAsyncDownloadProgressChanged;
        }

        /// <summary>
        /// Starts downloading the specified files.
        /// </summary>
        /// <param name="files">
        /// The list of files to download.
        /// </param>
        public void DownloadAsync( IList<IManifestFile> files )
        {
            this.file = null;
            this.fileIndex = -1;
            this.files = files;

            this.DownloadNext();
        }

        /// <summary>
        /// Downloads the next file in the list.
        /// </summary>
        private void DownloadNext()
        {
            ++this.fileIndex;

            if( fileIndex >= files.Count )
            {
                if( this.Completed != null )
                {
                    this.Completed( this, EventArgs.Empty );
                }

                return;
            }

            this.file = files[fileIndex];
            this.Download();
        }

        /// <summary>
        /// Downloads the current file.
        /// </summary>
        private void Download()
        {
            try
            {
                string localFileName = Path.Combine( applicationPath, file.FileName );
                string localDirectory = Path.GetDirectoryName( localFileName );
                Directory.CreateDirectory( localDirectory );

                this.downloader.DownloadFileAsync(
                    Path.Combine( Settings.Default.DownloadAddress, this.file.FileName ),
                    localFileName
                );
            }
            catch( Exception exc )
            {
                if( this.Errored != null )
                {
                    this.Errored( this, new ErrorEventArgs( exc ) );
                }
            }
        }

        /// <summary>
        /// Called when the asynchronous download has completed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The System.ComponentModel.AsyncCompletedEventArgs that contain the event data.
        /// </param>
        private void OnAsyncDownloadCompleted( object sender, System.ComponentModel.AsyncCompletedEventArgs e )
        {
            if( e.Error != null )
            {
                if( this.Errored != null )
                {
                    this.Errored( this, new ErrorEventArgs( e.Error ) );
                }
            }
            else
            {
                ++this.sucessfulDownloadCount;
            }

            this.DownloadNext();
        }

        /// <summary>
        /// Called when the current asynchronous download progress has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The System.Net.DownloadProgressChangedEventArgs that contain the event data.
        /// </param>
        private void OnAsyncDownloadProgressChanged( object sender, System.Net.DownloadProgressChangedEventArgs e )
        {
            if( this.DownloadProgressChanged != null )
            {
                this.DownloadProgressChanged(
                    this,
                    new GameFilesDownloadProgressChangedEventArgs(
                        e.BytesReceived,
                        e.TotalBytesToReceive,
                        this.file,
                        this.fileIndex,
                        this.files.Count
                    )
                );
            }
        }

        /// <summary>
        /// The number of files that have been sucessfully downloaded.
        /// </summary>
        private int sucessfulDownloadCount;

        /// <summary>
        /// The infex of the current file beeing downloaded.
        /// </summary>
        private int fileIndex;

        /// <summary>
        /// The current file beeing downloaded.
        /// </summary>
        private IManifestFile file;

        /// <summary>
        /// The files to download.
        /// </summary>
        private IList<IManifestFile> files;

        /// <summary>
        /// The IDownloader that should be used for downloading the game files.
        /// </summary>
        private readonly IDownloader downloader;

        /// <summary>
        /// The base address from which files are downloaded.
        /// </summary>
        private readonly string downloadAddress = Settings.Default.DownloadAddress;

        /// <summary>
        /// The base path of the application.
        /// </summary>
        private readonly string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
    }
}
