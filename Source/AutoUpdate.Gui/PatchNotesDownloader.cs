// <copyright file="PatchNotesDownloader.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.PatchNotesDownloader class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Updater
{
    using System;
    using System.IO;
    using System.Text;
    using Atom.AutoUpdate;
    using Zelda.Updater.Properties;

    /// <summary>
    /// Implements a method to download the patch notes.
    /// </summary>
    public sealed class PatchNotesDownloader : IAsyncProcess
    {
        /// <summary>
        /// Raised when this PatchNotesDownloader has completed downloading
        /// the patch notes.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Raised when an error has occurred during the download.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Errored;

        /// <summary>
        /// Gets the patch notes that have been downloaded.
        /// </summary>
        public string PatchNotes
        {
            get
            {
                return this.patchNotes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PatchNotesDownloader class.
        /// </summary>
        /// <param name="downloader">
        /// The IDownloader that should be used to download the IManifest data.
        /// </param>
        public PatchNotesDownloader( IDownloader downloader )
        {
            this.downloader = downloader;
        }

        /// <summary>
        /// Downloads the last known patch notes.
        /// </summary>
        /// <returns></returns>
        public void DownloadPatchNotesAsync()
        {
            this.downloader.AsyncDataDownloadCompleted += this.OnAsyncPatchNoteDownloadCompleted;
            this.downloader.DownloadDataAsync( this.GetDownloadAddress() );
        }

        /// <summary>
        /// Called when the async patch notes dowload has completed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The System.Net.DownloadDataCompletedEventArgs that contain event data.
        /// </param>
        private void OnAsyncPatchNoteDownloadCompleted( object sender, System.Net.DownloadDataCompletedEventArgs e )
        {
            if( e.Error != null )
            {
                this.OnError( e.Error );
                return;
            }

            this.downloader.AsyncDataDownloadCompleted -= this.OnAsyncPatchNoteDownloadCompleted;
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
                using( var streamReader = new StreamReader( new MemoryStream( data ), Encoding.Unicode ) )
                {
                    this.patchNotes = streamReader.ReadToEnd();

                    if( this.patchNotes.Length >= 3 )
                    {
                        this.patchNotes = this.patchNotes.Remove( 0, 3 );
                    }
                }
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
        /// Gets the download address for the patch notes.
        /// </summary>
        /// <returns></returns>
        private string GetDownloadAddress()
        {
            return Path.Combine( Settings.Default.DownloadAddress, "PatchNotes.txt" );
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
        /// The patch notes that have been downloaded.
        /// </summary>
        private string patchNotes = string.Empty;

        /// <summary>
        /// The IDownloader that should be used to download the IManifest data.
        /// </summary>
        private readonly IDownloader downloader;
    }
}
