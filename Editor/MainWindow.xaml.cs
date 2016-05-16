// <copyright file="MainWindow.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.MainWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines the main Window of the Editor.
    /// </summary>
    sealed partial class MainWindow : Window, Atom.Events.IEventManagerService
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
            this.xnaThread.Priority = System.Threading.ThreadPriority.AboveNormal;
            this.xnaThread.Start( xnaControl.Handle );

            // Setup services:
            Atom.GlobalServices.Container.AddService( typeof( Atom.Events.IEventManagerService ), this );
            EditorApp.Current.Initialize();
        }

        /// <summary>
        /// Starts up and initializes the XNA game loop.
        /// </summary>
        /// <param name="obj">The handle of the control xna should draw into.</param>
        private void RunXna( object obj )
        {
            IntPtr handle = (IntPtr)obj;
            var    editor = EditorApp.Current;

            xnaApp = new XnaEditorApp( editor, handle );
            editor.AppObject = xnaApp;

            this.InitializeWorkspaceControls();
            xnaApp.Run();
        }

        /// <summary>
        /// Initializes the various workspace controls that are hooked up
        /// with the main window.
        /// </summary>
        private void InitializeWorkspaceControls()
        {
            this.tileWorkspaceControl.Workspace = (Zelda.Editor.Tile.TileWorkspace)xnaApp.GetWorkspace( WorkspaceType.Tile );
            this.eventWorkspaceControl.Workspace = (Zelda.Editor.Event.EventWorkspace)xnaApp.GetWorkspace( WorkspaceType.Event );
            this.objectWorkspaceControl.Workspace = (Zelda.Editor.Object.ObjectWorkspace)xnaApp.GetWorkspace( WorkspaceType.Object );
            this.storyWorkspaceControl.Workspace = (Zelda.Editor.Story.StoryWorkspace)xnaApp.GetWorkspace( WorkspaceType.Story );
            this.waypointWorkspaceControl.Workspace = (Zelda.Editor.Waypoint.WaypointWorkspace)xnaApp.GetWorkspace( WorkspaceType.Waypoint );
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Receives the currently open Scene.
        /// </summary>
        private SceneViewModel Scene
        {
            get { return scene; }
        }

        /// <summary>
        /// Receives an object which provides fast access to zelda-game related services.
        /// </summary>
        public static IZeldaServiceProvider ZeldaServiceProvider
        {
            get { return EditorApp.Current; }
        }
        
        /// <summary>
        /// Receives the EventManager of the currently open Scene.
        /// </summary>
        Atom.Events.EventManager Atom.Events.IEventManagerService.EventManager
        {
            get
            {
                var scene = this.Scene;
                if( scene == null )
                    return null;

                return scene.EventManager.Model;
            }
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Changes the current Scene the user is editing.
        /// </summary>
        /// <param name="sceneViewModel">
        /// The scene object the user is going to edit.
        /// </param>
        private void ChangeScene( SceneViewModel scene )
        {
            if( this.scene != null )
            {
                this.scene.Model.NotifySceneChange( ChangeType.Away );
            }

            this.scene = scene;
            this.DataContext = scene; // Notifies UI
            this.xnaApp.Scene = scene; // Notifies XNA-Workspaces
            this.propertyGridSettings.SelectedObject = this.scene.Settings;

            if( this.scene != null )
            {
                this.scene.Model.NotifySceneChange( ChangeType.To );
            }
        }
        
        /// <summary>
        /// Opens the Scene with the given name.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the Scene to open.
        /// </param>
        private void OpenScene( string sceneName )
        {
            try
            {
                // Load Scene & Create ViewModel
                var scene          = ZeldaScene.Load( sceneName, ZeldaServiceProvider );
                var sceneViewModel = new SceneViewModel( scene, ZeldaServiceProvider );

                ChangeScene( sceneViewModel );
            }
            catch( Exception exc )
            {
                // Inform of Error
                MessageBox.Show( exc.ToString(), Atom.ErrorStrings.Error,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
            }
        }
        
        #region > Events <

        #region - Menu -

        /// <summary>
        /// Gets called when the user clicks on the 'New' button of the File menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs"/> that contain the event data.
        /// </param>
        private void MainMenu_FileNew_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new Dialogs.NewSceneDialog();

            if( dialog.ShowDialog() == true )
            {
                var floorCount = dialog.SelectedFloorCount;
                var sceneName  = dialog.SelectedName;
                int width      = dialog.SelectedWidth;
                int height     = dialog.SelectedHeight;

                // Create and fully initialize the Scene.
                // All initialization must be done
                // before we create the ViewModel.
                var scene = ZeldaScene.CreateManual( width, height, floorCount, ZeldaServiceProvider );
                scene.Name = sceneName;

                // Create and assign SceneViewModel:
                var sceneViewModel = new SceneViewModel( scene, ZeldaServiceProvider );

                for( int i = 0; i < floorCount; ++i )
                    sceneViewModel.Map.AddFloor.Execute( null );
                ChangeScene( sceneViewModel );
            }
        }
        
        /// <summary>
        /// Gets called when the user clicks on the 'Save' button of the File menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs"/> that contain the event data.
        /// </param>
        private void MainMenu_Save_Click( object sender, RoutedEventArgs e )
        {
            if( this.Scene == null )
                return;

            this.Scene.Save();
        }

        /// <summary>
        /// Gets called when the user clicks on the 'Open' button of the File menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs"/> that contain the event data.
        /// </param>
        private void MainMenu_Open_Click( object sender, RoutedEventArgs e )
        {
            if( this.Scene != null )
            {
                var result = MessageBox.Show( 
                    Properties.Resources.Question_SaveTheOpenScene, 
                    Zelda.Resources.Question,
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes 
                );

                switch( result )
                { 
                    case MessageBoxResult.Yes:
                        this.Scene.Save();
                        break;

                    case MessageBoxResult.No:
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                    
                    default:
                        throw new NotImplementedException();
                }
            }

            var dialog = new Microsoft.Win32.OpenFileDialog() {
                Title            = Properties.Resources.DialogTitle_OpenScene,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Scenes\" ),
                Filter           = Properties.Resources.Filter_Scene,
                CheckFileExists  = true,
                RestoreDirectory = true,
                ValidateNames    = true
            };

            System.IO.Directory.CreateDirectory( dialog.InitialDirectory );

            if( dialog.ShowDialog() == true )
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension( dialog.FileName );
                OpenScene( sceneName );
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the 'Exit' button of the File menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs"/> that contain the event data.
        /// </param>
        private void MainMenu_FileExit_Click( object sender, RoutedEventArgs e )
        {
            EditorApp.Current.Shutdown();
        }

        #endregion

        #region - Input -

        /// <summary>
        /// Gets called when the user presses a key while
        /// the window has focus but not any of its controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            xnaApp.HandleKeyDown( e );
        }

        /// <summary>
        /// Gets called when the user releases a key while
        /// the window has focus but not any of its controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyUp( object sender, KeyEventArgs e )
        {
            xnaApp.HandleKeyUp( e );
        }

        /// <summary>
        /// Gets called when the user clicks on the Xna Control of the Editor.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        private void xnaControl_MouseClick( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            xnaApp.HandleMouseClick( e );
        }

        /// <summary>
        /// Gets called when the user moves the mouse on the Xna Control of the Editor.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        private void xnaControl_MouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            xnaApp.HandleMouseMove( e );
        }

        /// <summary>
        /// Gets called when the user presses the mouse on the Xna Control of the Editor.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        private void xnaControl_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            xnaApp.HandleMouseDown( e );
        }

        /// <summary>
        /// Gets called when the user releases the mouse on the Xna Control of the Editor.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        private void xnaControl_MouseUp( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            xnaApp.HandleMouseUp( e );
        }
      
        #endregion

        #region - Status Bar -

        /// <summary>
        /// Gets called when the user clicks 'FloorEditMode' StatusBarItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.
        /// </param>
        private void StatusBarItem_FloorEditMode_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            if( e.LeftButton == MouseButtonState.Pressed )
            {
                var scene = this.Scene;
                if( scene == null )
                    return;

                scene.Map.ToggleFloorEditMode();
            }
        }

        /// <summary>
        /// Gets called when the user clicks 'ActionLayerVisability' StatusBarItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.
        /// </param>
        private void StatusBarItem_ActionLayerVisability_MouseDown( object sender, MouseButtonEventArgs e )
        {
            if( e.LeftButton == MouseButtonState.Pressed )
            {
                var scene = this.Scene;
                if( scene == null )
                    return;

                scene.Map.ToggleActionLayerVisability();
            }
        }

        /// <summary>
        /// Gets called when the user clicks 'LightingState' StatusBarItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.
        /// </param>
        private void StatusBarItem_LightingState_MouseDown( object sender, MouseButtonEventArgs e )
        {
            if( e.LeftButton == MouseButtonState.Pressed )
            {
                var scene = this.Scene;
                if( scene == null )
                    return;

                scene.Settings.IsLightingEnabled = !scene.Settings.IsLightingEnabled;
            }
        }

        #endregion
        
        /// <summary>
        /// Gets called when the user selects a different Workspace.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedTabChanged( object sender, SelectionChangedEventArgs e )
        {
            if( xnaApp == null )
                return;

            var tab = tabControl.SelectedItem as TabItem;

            if( tab != null )
            {
                if( tab.Tag != null )
                {
                    xnaApp.CurrentWorkspace = (WorkspaceType)tab.Tag;
                }
            }
            else
            {
                xnaApp.CurrentWorkspace = WorkspaceType.None;
            }
        }
        
        /// <summary>
        /// Gets called when the MainWindow is closing.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The CancelEventArgs that contain the event data.
        /// </param>
        private void OnClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            lock( xnaApp )
            {
                this.xnaApp.Exit();
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the currently open scene.
        /// </summary>
        private SceneViewModel scene;

        /// <summary>
        /// The Xna application object which is rendering the scene.
        /// </summary>
        private XnaEditorApp xnaApp;

        /// <summary>
        /// The thread the XNA drawing and updating logic runs in.
        /// </summary>
        private readonly System.Threading.Thread xnaThread;

        #endregion
    }
}