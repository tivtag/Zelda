// <copyright file="MainWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuestCreator.MainWindow class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.QuestCreator
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Zelda.Quests;

    /// <summary>
    /// Defines the main window of the Quest Creator tool.
    /// </summary>
    /// <remarks>
    /// The Quest Creator allows the creation and edition of <see cref="Zelda.Quests.Quest"/>s.
    /// </remarks>
    public sealed partial class MainWindow : Window
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // Startup Xna:
            this.xnaThread = new System.Threading.Thread( RunXna );
            this.xnaThread.Priority = System.Threading.ThreadPriority.Lowest;
            this.xnaThread.Start();

            // Setup events:
            this.Closing += OnMainWindowClosing;
        }

        /// <summary>
        /// Starts up and initializes the XNA game loop.
        /// </summary>
        private void RunXna()
        {
            this.xnaApp = new XnaApp();
            App.Current.XnaApp = this.xnaApp;

            this.xnaApp.Run();
        }

        /// <summary>
        /// Gets called when the MainWindow is closing.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The CancelEventArgs that contains the event data.</param>
        private void OnMainWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            lock( xnaApp )
            {
                this.xnaApp.Exit();
            }
        }

        #endregion        

        #region [ Properties ]

        /// <summary>
        /// Gets the IZeldaServiceProvider that provides fast
        /// access to game-related services.
        /// </summary>
        private IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return App.Current;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens the quest at the given path.
        /// </summary>
        /// <param name="path">
        /// The full path to the quest file.
        /// </param>
        private void OpenQuest( string path )
        {
            try
            {
                using( var reader = new System.IO.BinaryReader( System.IO.File.OpenRead( path ) ) )
                {
                    Quest quest = new Quest();

                    var context = new Zelda.Saving.DeserializationContext( reader, this.ServiceProvider );
                    quest.Deserialize( context );

                    AddQuest( quest );
                }
            }
            catch( Exception exc )
            {
                MessageBox.Show( exc.ToString(), Atom.ErrorStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error );
            }           
        }

        /// <summary>
        /// Adds the given Quest to the tab control.
        /// </summary>
        /// <param name="quest">
        /// The quest to add.
        /// </param>
        private void AddQuest( Quest quest )
        {
            var questViewModel = new QuestViewModel( quest );
            var tabItem = new TabItem();

            // Bind the name of the quest
            // onto the Header of the new TabItem
            Binding nameBinding = new Binding() {
                Source = questViewModel,
                Path   = new PropertyPath( "Name" )
            };

            tabItem.SetBinding( TabItem.HeaderProperty, nameBinding );

            // Create, setup and add QuestViewModelControl
            Grid grid = new Grid();
            tabItem.Content = grid;
            grid.Children.Add( new QuestViewModelControl( questViewModel ) );

            // Add the new tab
            tabControl.Items.Add( tabItem );
        }

        /// <summary>
        /// Gets called when the user has clicked on the "New Quest" menu item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnNewMenuItemClicked( object sender, RoutedEventArgs e )
        {
            var quest = new Quest() { 
                Name = "NewQuest" 
            };
            AddQuest( quest );
        }

        /// <summary>
        /// Gets called when the user has clicked on the "Open" menu item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnOpenMenuItemClicked( object sender, RoutedEventArgs e )
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {                
                Title  = Properties.Resources.DialogTitle_OpenQuest,
                Filter = Properties.Resources.Filter_QuestFiles,
                InitialDirectory = System.IO.Path.Combine( System.AppDomain.CurrentDomain.BaseDirectory, "Content\\Quests\\" ),
                RestoreDirectory = true
            };

            if( dialog.ShowDialog() == true )
            {
                OpenQuest( dialog.FileName );
            }
        }

        /// <summary>
        /// Gets called when the user has clicked on the "Save" menu item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnSaveMenuItemClicked( object sender, RoutedEventArgs e )
        {
            if( this.tabControl.SelectedItem == null )
                return;

            var tabItem = (TabItem)tabControl.SelectedItem;
            var grid    = (Grid)tabItem.Content;
            var control = (QuestViewModelControl)grid.Children[0];

            var viewModel = control.BoundQuestViewModel;

            if( viewModel.Save( this.ServiceProvider) )
            {
                string message = string.Format( 
                    System.Globalization.CultureInfo.CurrentCulture,
                    Properties.Resources.Info_QuestXSavedSuccessfully,
                    viewModel.Name
                );

                MessageBox.Show(
                    message,
                    Atom.ErrorStrings.Information,
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information
               );
            }
        }

        /// <summary>
        /// Gets called when the user has clicked on the "Exit" menu item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnExitMenuItemClicked( object sender, RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The Xna application object.
        /// </summary>
        private XnaApp xnaApp;

        /// <summary>
        /// The thread the XNA logic runs in.
        /// </summary>
        private readonly System.Threading.Thread xnaThread;

        #endregion
    }
}
