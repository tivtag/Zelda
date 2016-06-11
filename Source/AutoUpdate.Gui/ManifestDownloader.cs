// <copyright file="ManifestDownloader.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.ManifestDownloader class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;
    using System.IO;
    using Atom.AutoUpdate;
    using Atom.AutoUpdate.Manifest;
    using Atom.AutoUpdate.Manifest.Serialization;
    using Zelda.Updater.Properties;

    /// <summary>
    /// Implements a mechanism that downloads the
    /// current updater <see cref="IManifest"/>.
    /// </summary>
    public sealed class ManifestDownloader : IAsyncProcess
    {
        /// <summary>
        /// Raised when this ManifestDownloader has completed downloading
        /// the manifest.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Raised when an error has occurred during the download.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Errored;

        /// <summary>
        /// Gets the IManifest that has been downloaded.
        /// </summary>
        public IManifest Manifest
        {
            get
            {
                return this.manifest;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ManifestDownloader class.
        /// </summary>
        /// <param name="downloader">
        /// The IDownloader that should be used to download the IManifest data.
        /// </param
        public ManifestDownloader( IDownloader downloader )
            : this( downloader, new BinaryManifestSerializer() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the ManifestDownloader class.
        /// </summary>
        /// <param name="downloader">
        /// The IDownloader that should be used to download the IManifest data.
        /// </param>
        /// <param name="manifestSerializer">
        /// The IManifestSerializer that should be used to derserialize the downloaded data.
        /// </param>
        public ManifestDownloader( IDownloader downloader, IManifestSerializer manifestSerializer )
        {
            this.downloader = downloader;
            this.manifestSerializer = manifestSerializer;
        }

        /// <summary>
        /// Downloads the current IManifest.
        /// </summary>
        /// <returns>
        /// The IManifest that has been downloaded.
        /// </returns>
        public void DownloadManifestAync()
        {
            this.downloader.AsyncDataDownloadCompleted += this.OnAsyncManifestDownloadCompleted;
            this.downloader.DownloadDataAsync( GetDownloadAddress() );
        }

        /// <summary>
        /// Called when the async manifest dowload has completed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The System.Net.DownloadDataCompletedEventArgs that contain event data.
        /// </param>
        private void OnAsyncManifestDownloadCompleted( object sender, System.Net.DownloadDataCompletedEventArgs e )
        {
            if( e.Error != null )
            {
                this.OnError( e.Error );
                return;
            }

            this.downloader.AsyncDataDownloadCompleted -= this.OnAsyncManifestDownloadCompleted;
            this.ProcessDownloadedData( e.Result );
        }

        /// <summary>
        /// Processes the data that has been successfully downloaded.
        /// </summary>
        /// <param name="data">
        /// The data that has been downloaded.
        /// </param>
        private void ProcessDownloadedData( byte[] data )
        {
            try
            {
                this.manifest = this.manifestSerializer.Deserialize( new MemoryStream( data ) );
            }
            catch( Exception exc )
            {
                this.OnError( exc );
                return;
            }

            if( this.Completed != null )
            {
                this.Completed( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the Errored event.
        /// </summary>
        /// <param name="e">
        /// The exception that has occurred.
        /// </param>
        private void OnError( Exception e )
        {
            if( this.Errored != null )
            {
                this.Errored( this, new ErrorEventArgs( e ) );
            }
        }

        /// <summary>
        /// Gets the address at which the manifest file is downloaded from.
        /// </summary>
        /// <returns></returns>
        private static string GetDownloadAddress()
        {
            return Path.Combine( Settings.Default.DownloadAddress, "manifest.txt" );
        }

        /// <summary>
        /// The IManifest that has been downloaded.
        /// </summary>
        private IManifest manifest;

        /// <summary>
        /// The IDownloader that should be used to download the IManifest data.
        /// </summary>
        private readonly IDownloader downloader;

        /// <summary>
        /// The IManifestSerializer that should be used to derserialize the downloaded data.
        /// </summary>
        private readonly IManifestSerializer manifestSerializer;
    }
}
