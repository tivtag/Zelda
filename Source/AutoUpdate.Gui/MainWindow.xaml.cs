// <copyright file="MainWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Updater.MainWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Updater
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using Atom.AutoUpdate;
    using Atom.AutoUpdate.Manifest;

    /// <summary>
    /// Defines the Main Window of the Zelda Updater application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.statusBar = new StatusBarInfoController( this.statusBarItemInfo );

            Execute.InitializeWithDispatcher();
            ThreadPool.QueueUserWorkItem( state => this.BeginDownloadingPatchNotes() );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if( (e.Key == System.Windows.Input.Key.Escape) || (this.hasCompleted && e.Key == System.Windows.Input.Key.Enter) )
            {
                this.Shutdown();
            }
        }

        private void Shutdown()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginDownloadingPatchNotes()
        {
            var patchNotesDownloader = new PatchNotesDownloader( this.downloader );

            patchNotesDownloader.Errored += ( sender, e ) => { this.ShowError( "Patch Note Download Error: ", e.GetException() ); };
            patchNotesDownloader.Completed += ( sender, e ) => {
                Execute.OnUIThread( () => {
                    this.textBoxPatchNotes.Text = patchNotesDownloader.PatchNotes;
                } );

                this.BeginDownloadingManifest();
            };

            this.ShowInformation( "Downloading Patch Notes.." );
            patchNotesDownloader.DownloadPatchNotesAsync();
        }        

        /// <summary>
        /// 
        /// </summary>
        private void BeginDownloadingManifest()
        {            
            var manifestDownloader = new ManifestDownloader( this.downloader );
            
            manifestDownloader.Errored += (sender, e) => { this.ShowError( "Manifest Download Error: ", e.GetException() );  };
            manifestDownloader.Completed += (sender, e) => {
                this.BeginDownloadingFiles( manifestDownloader.Manifest );
            };

            this.ShowInformation( "Downloading Manifest.." );
            manifestDownloader.DownloadManifestAync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifest"></param>
        private void BeginDownloadingFiles( IManifest manifest )
        {
            this.ShowInformation( "Finding changed Files .." );
            var filesToDownload = new FilesToDownloadFinder( manifest ).Find();

            if( filesToDownload.Count > 0 )
            {
                var gameFilesDownloader = new AsyncGameFilesDownloader( this.downloader );

                gameFilesDownloader.Completed += ( sender, e ) => {
                    this.BeginCleanUnrequiredFiles( manifest );
                    this.Completed();

                    Execute.OnUIThread( () => {
                        string downloadCountText = gameFilesDownloader.SucessfulDownloadCount == filesToDownload.Count ? 
                            filesToDownload.Count.ToString( CultureInfo.CurrentCulture ) :
                            gameFilesDownloader.SucessfulDownloadCount.ToString( CultureInfo.CurrentCulture ) + "/" +
                            filesToDownload.Count.ToString( CultureInfo.CurrentCulture );

                        string messageText = string.Format( 
                            CultureInfo.CurrentCulture,
                            "{0} {1} been updated.",
                            downloadCountText,
                            gameFilesDownloader.SucessfulDownloadCount == 1 ? "file has" : "files have"
                        );

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat( messageText );
                        sb.AppendLine();

                        foreach( var pair in filesToDownload )
                        {
                            sb.Append( "\n  " );
                            switch( pair.Item1 )
                            {
                                case UpdateReason.InvalidHash:
                                    sb.Append( "MD5 mismatch - " );
                                    break;

                                case UpdateReason.FileMissing:
                                    sb.Append( "missing - " );
                                    break;

                                default:
                                    break;
                            }
                            sb.Append( pair.Item2.FileName );
                        }

                        string text = sb.ToString();
                        this.ShowInformation( "Done :)\n" + text );

                        MessageBox.Show(
                            messageText,
                            string.Empty,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    } );
                };

                gameFilesDownloader.Errored += ( sender, e ) => { this.ShowError( "Download Error: ", e.GetException() ); };
                gameFilesDownloader.DownloadProgressChanged += this.OnDownloadProgressChanged;
                
                this.ShowInformation( "Downloading Files .." );
                gameFilesDownloader.DownloadAsync( filesToDownload.Select( x => x.Item2 ).ToArray() );
            }
            else
            {
                this.BeginCleanUnrequiredFiles( manifest );
                this.Completed();
            }
        }

        private void BeginCleanUnrequiredFiles( IManifest manifest )
        {
            this.ShowInformation( "Cleaning Unrequired Files .." );

            var cleaner = new UnrequiredFilesCleaner();
            cleaner.RunAsync( manifest );
        }

        private void OnDownloadProgressChanged( object sender, GameFilesDownloadProgressChangedEventArgs e )
        {
            Execute.OnUIThread( () => {
                this.textFile.Text = e.File.FileName;
                this.textDownloadProgress.Text = e.BytesReceived.ToString() + " / " + e.TotalBytesToReceive.ToString() + " bytes";
                this.progressBarCurrentFile.Value = (e.BytesReceived / (double)e.TotalBytesToReceive) * 100.0;

                if( fileIndex != e.FileIndex )
                {
                    this.progressBar.Value = (e.FileIndex / (double)e.FileCount) * 100.0;
                    this.progressBar.ToolTip = e.FileIndex.ToString() + " / " + e.FileCount.ToString() + " files";
                    this.fileIndex = e.FileIndex;
                }
            } );
        }

        private int fileIndex = -1;

        private void Completed()
        {
            Execute.OnUIThread( () => {
                this.progressBar.Value = 100.0;
                this.progressBarCurrentFile.Value = 100.0;
                this.progressBar.ToolTip = null;
                this.textFile.Text = string.Empty;
                this.textDownloadProgress.Text = string.Empty;
                this.hasCompleted = true;

                this.progressBarCurrentFile.Visibility = Visibility.Collapsed;
                this.buttonStartGame.Visibility = Visibility.Visible;
                this.ShowInformation( "Done :)" );
            } );
        }

        private void OnStartGameButtonClicked( object sender, RoutedEventArgs e )
        {
            this.Shutdown();
        }
        
        private void ShowError( string additionalMessage, Exception exc )
        {
            this.statusBar.ShowError( additionalMessage + exc.Message + "\n\n" + exc.ToString() );
        }

        private void ShowInformation( string info )
        {
            this.statusBar.ShowInformation( info );
        }

        private bool hasCompleted;
        private readonly IDownloader downloader = new Downloader();
        private readonly StatusBarInfoController statusBar;
    }
}
