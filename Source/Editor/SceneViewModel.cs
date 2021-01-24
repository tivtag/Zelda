// <copyright file="SceneViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.SceneViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor
{
    using System;
    using System.Diagnostics;
    using Atom.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Acid.Wpf.Collections;
    using Atom.Design;
    using Atom.Wpf;
    using Zelda.Editor.Event.ViewModels;
    using Zelda.Editor.Object;
    using Zelda.Editor.Story.ViewModels;
    using Zelda.Editor.Tile.ViewModels;
    using Zelda.Editor.Waypoint;
    using Zelda.Entities;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="ZeldaScene"/> to provide them to the View (WPF).
    /// </summary>
    public sealed class SceneViewModel : ViewModel<ZeldaScene>
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneViewModel"/> class.
        /// </summary>
        /// <param name="scene">
        /// The scene the new ViewModel wraps around.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public SceneViewModel( ZeldaScene scene, IZeldaServiceProvider serviceProvider )
            : base( scene )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.serviceProvider = serviceProvider;

            // Create ViewModels
            this.map          = new TileMapViewModel( scene.Map, this, serviceProvider );
            this.camera       = new CameraViewModel( scene.Camera );
            this.eventManager = new EventManagerViewModel( scene.EventManager );
            this.waypointMap = new WaypointMapViewModel( scene.WaypointMap );
            this.storyboard = new StoryboardViewModel( scene.Storyboard );

            this.objectsView = new CollectionView( this.objects );
            this.objectsView.CurrentChanged += this.OnCurrentObjectChanged;

            foreach( var entity in scene.Entities )
            {
                if( entity.IsEditable )
                {
                    var wrapper = EntityPropertyWrapperFactory.ReceiveWrapper( entity );
                    objects.Add( wrapper );
                }
            }

            // Create commands:
            this.NewObject            = new NewObjectCommand( this );
            this.RemoveObject         = new RemoveObjectCommand( this );
            this.MoveToSelectedObject = new MoveToSelectedObjectCommand( this );
            this.CloneSelectedObject  = new CloneSelectedObjectCommand( this );

            // Misc setup:
            scene.Settings.IsLightingEnabled = false;
            scene.IngameDateTime.TickSpeed = 500.0f;

            // Hook events:
            scene.EntityAdded   += this.OnSceneEntityAdded;
            scene.EntityRemoved += this.OnSceneEntityRemoved;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the view model that wraps around the TileMap of the Scene.
        /// </summary>
        public TileMapViewModel Map
        {
            get
            { 
                return this.map; 
            }
        }

        /// <summary>
        /// Gets the view model that wraps around the ZeldaCamera of the Scene.
        /// </summary>
        public CameraViewModel Camera
        {
            get 
            {
                return this.camera; 
            }
        }

        /// <summary>
        /// Gets the view model that wraps around the EventManager of the Scene.
        /// </summary>
        public EventManagerViewModel EventManager
        {
            get 
            { 
                return this.eventManager; 
            }
        }

        /// <summary>
        /// Gets the ViewModel that wraps around the WaypointMap of the scene.
        /// </summary>
        public WaypointMapViewModel WaypointMap
        {
            get
            {
                return this.waypointMap;
            }
        }

        /// <summary>
        /// Gets the ViewModel that wraps around the Storyboard of the scene.
        /// </summary>
        public StoryboardViewModel Storyboard
        {
            get
            {
                return this.storyboard;
            }
        }

        /// <summary>
        /// Gets the (bindable) enumeration of objects
        /// that are visible to the user.
        /// </summary>
        public CollectionView Objects
        {
            get 
            { 
                return this.objectsView;
            }
        }

        /// <summary>
        /// Gets or sets the object which is currently selected by the user.
        /// </summary>
        public object SelectedObject
        {
            get
            {
                var obj = objectsView.CurrentItem;

                IObjectPropertyWrapper wrapper = obj as IObjectPropertyWrapper;
                if( wrapper != null )
                    return wrapper.WrappedObject;

                return obj;
            }

            set
            {
                foreach( var obj in objects )
                {
                    object actualObj = null;

                    IObjectPropertyWrapper wrapper = obj as IObjectPropertyWrapper;
                    if( wrapper != null )
                        actualObj = wrapper.WrappedObject;
                    else
                        actualObj = obj;

                    if( value == actualObj )
                    {
                        objectsView.MoveCurrentTo( obj );
                        break;
                    }
                }

                OnPropertyChanged( "SelectedObject" );
            }
        }

        /// <summary>
        /// Receives the <see cref="ObjectPropertyWrapperFactory"/> object.
        /// </summary>
        private static EntityPropertyWrapperFactory EntityPropertyWrapperFactory 
        {
            get { return EditorApp.Current.ObjectPropertyWrapperFactory; }
        }

        /// <summary>
        /// Gets the size of the scene in pixels.
        /// </summary>
        public Atom.Math.Vector2 SizeInPixels
        {
            get
            {
                return this.Model.Map.SizeInPixels;
            }
        }

        /// <summary>
        /// Gets the IWorkspace responsible for editing the Waypoint model of the Scene.
        /// </summary>
        public WaypointWorkspace WaypointWorkspace
        {
            get
            {
                return EditorApp.Current.AppObject.WaypointWorkspace;
            }
        }

        #region > Commands <

        /// <summary>
        /// Receives the command which when executed
        /// asks the user to select an object to add to the Scene.
        /// </summary>
        public ICommand NewObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the command which when executed
        /// asks the user whether he wants to remove 
        /// the currently selected object.
        /// </summary>
        public ICommand RemoveObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the command which when executed
        /// moves the camera to the selected object.
        /// </summary>
        public ICommand MoveToSelectedObject
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Receives the command which when executed
        /// clones the selected object.
        /// </summary>
        public ICommand CloneSelectedObject
        {
            get;
            private set;
        }
        
        #endregion

        #region > Wrapped Properties <

        /// <summary>
        /// Gets or sets the (bindable) name of the <see cref="ZeldaScene"/>.
        /// </summary>
        public string Name
        {
            get 
            {
                return this.Model.Name;
            }

            set 
            {
                if( value == this.Name )
                    return;

                this.Model.Name = value;
                OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets the settings of the scene.
        /// </summary>
        public SceneSettings Settings
        {
            get
            {
                return this.Model.Settings;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether lightning is currently enabled for the Scene.
        /// </summary>
        public bool IsLightingEnabled
        {
            get
            {
                return this.Settings.IsLightingEnabled;
            }

            set
            {
                if( value == this.IsLightingEnabled )
                    return;

                this.Settings.IsLightingEnabled = value;
                this.OnPropertyChanged( "IsLightingEnabled" );
            }
        }

        #endregion

        #endregion

        #region [ Commands ]
        
        /// <summary>
        /// Defines the <see cref="ICommand"/> which
        /// asks the user to select an object-type and then adds
        /// a new object of that type to the Scene.
        /// </summary>
        sealed class NewObjectCommand : ViewModelCommand<SceneViewModel, ZeldaScene>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NewObjectCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The SceneViewModel that owns this ICommand.
            /// </param>
            public NewObjectCommand( SceneViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes the <see cref="NewObjectCommand"/>.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var dialog = new Zelda.Editor.Dialogs.EntityTypeSelectionDialog( EntityPropertyWrapperFactory.GetObjectTypes() );

                if( dialog.ShowDialog() == true )
                {
                    Type type = dialog.SelectedEntityType;

                    ZeldaEntity obj = (ZeldaEntity)Activator.CreateInstance( type );
                    var setupable = obj as IZeldaSetupable;
                    if( setupable != null )
                        setupable.Setup( this.ViewModel.serviceProvider );

                    this.ViewModel.AddNew( obj );
                }
            }
        }

        /// <summary>
        /// Defines the <see cref="ICommand"/> which
        /// asks the user whether he wants to remove
        /// the currently selected object from the Scene.
        /// </summary>
        sealed class RemoveObjectCommand : ViewModelCommand<SceneViewModel, ZeldaScene>
        {         
            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveObjectCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The SceneViewModel that owns this ICommand.
            /// </param>
            public RemoveObjectCommand( SceneViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Gets a value whether this ICommand can currently be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedObject != null;
            }

            /// <summary>
            /// Executes the <see cref="RemoveObjectCommand"/>.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                ZeldaEntity entity = this.GetSelectedEntity();

                if( !entity.IsRemoveable )
                {
                    MessageBox.Show(
                        string.Format( CultureInfo.CurrentCulture,
                            Properties.Resources.Info_TheEntityXCantBeRemoved,
                            entity != null ? entity.Name ?? string.Empty : string.Empty
                        ),
                        Atom.ErrorStrings.Information, 
                        MessageBoxButton.OK,
                        MessageBoxImage.Question
                    );
                    return;
                }
                
                if( AskUserShouldRemove( entity ) )
                {
                    entity.RemoveFromScene();
                }
            }

            /// <summary>
            /// Gets the currently selected ZeldaEntity.
            /// </summary>
            /// <returns></returns>
            private ZeldaEntity GetSelectedEntity()
            {
                object obj  = this.ViewModel.objectsView.CurrentItem;
                var wrapper = obj as IObjectPropertyWrapper;

                if( wrapper != null )
                    return wrapper.WrappedObject as ZeldaEntity;
                else
                    return obj as ZeldaEntity;
            }

            /// <summary>
            /// Asks the user whether he really wants to remove the given ZeldaEntity from
            /// the Scene.
            /// </summary>
            /// <param name="entity">The ZeldaEntity to remove.</param>
            /// <returns>
            /// Returns true if the ZeldaEntity should be removed;
            /// otherwise false.
            /// </returns>
            private static bool AskUserShouldRemove( ZeldaEntity entity )
            {
                var result = MessageBox.Show(
                    string.Format( CultureInfo.CurrentCulture,
                        Properties.Resources.Question_ReallyRemoveSelectedObjectX,
                        entity != null ? entity.Name ?? string.Empty : string.Empty
                    ),
                    string.Empty, 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question,
                    MessageBoxResult.No
                );

                return result == MessageBoxResult.Yes;
            }
        }

        /// <summary>
        /// Defines the <see cref="ICommand"/> which
        /// when executed moves the camera
        /// to the selected object.
        /// </summary>
        sealed class MoveToSelectedObjectCommand : ViewModelCommand<SceneViewModel, ZeldaScene>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MoveToSelectedObjectCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public MoveToSelectedObjectCommand( SceneViewModel viewModel )
                : base( viewModel )
            {
                viewModel.objectsView.CurrentChanged += OjectsView_CurrentChanged;
            }

            /// <summary>
            /// Returns a value that indicates whether this ICommand can be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                var obj = this.ViewModel.SelectedObject;
                if( obj == null )
                    return false;

                return obj is ZeldaEntity;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                ZeldaEntity entity = (ZeldaEntity)this.ViewModel.SelectedObject;
                this.ViewModel.Camera.MoveToCentered( entity.Transform.Position );
                this.Model.NotifyVisabilityUpdateNeeded();
            }

            /// <summary>
            /// Gets called when the currently selected object has changed.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OjectsView_CurrentChanged( object sender, EventArgs e )
            {
                OnCanExecuteChanged();
            }
        }
                
        /// <summary>
        /// Defines the <see cref="ICommand"/> which
        /// when executed clones the selected object.
        /// </summary>
        sealed class CloneSelectedObjectCommand : ViewModelCommand<SceneViewModel, ZeldaScene>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CloneSelectedObjectCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public CloneSelectedObjectCommand( SceneViewModel viewModel )
                : base( viewModel )
            {
                viewModel.objectsView.CurrentChanged += OjectsView_CurrentChanged;
            }

            /// <summary>
            /// Returns a value that indicates whether this ICommand can be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                var obj = this.ViewModel.SelectedObject;
                if( obj == null )
                    return false;

                return obj is ZeldaEntity;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !this.CanExecute( parameter ) )
                    return;

                ZeldaEntity entity = (ZeldaEntity)this.ViewModel.SelectedObject;
                ZeldaEntity clone = entity.Clone();
                this.ViewModel.AddNew( clone );
            }

            /// <summary>
            /// Gets called when the currently selected object has changed.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OjectsView_CurrentChanged( object sender, EventArgs e )
            {
                OnCanExecuteChanged();
            }
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Adds a new entity (yet unnamed) entity to the scene.
        /// </summary>
        /// <param name="entity">
        /// The entity to add.
        /// </param>
        internal void AddNew( ZeldaEntity entity )
        {
            entity.Name = this.GetNameForNewEntity( entity.GetType() );

            this.Model.Add( entity );
            this.SelectedObject = entity;
        }

        /// <summary>
        /// Saves this Scene.
        /// </summary>
        public void Save()
        {
            try
            {
                Verify();
                this.Model.Save();

                // Inform of Success
                MessageBox.Show( string.Format( CultureInfo.CurrentCulture,
                    Properties.Resources.Info_SceneXSaved, this.Name ),
                    Atom.ErrorStrings.Information, MessageBoxButton.OK, MessageBoxImage.None
                 );
            }
            catch( Exception exc )
            {
                // Inform of Error
                MessageBox.Show( exc.Message, Atom.ErrorStrings.Error,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
            }
        }

        private void Verify()
        {
            var audioSystem = serviceProvider.AudioSystem;

            foreach( var music in Model.Settings.MusicList )
            {
                if( audioSystem.GetMusic( music.FileName ) == null )
                {
                    throw new ApplicationException( "Missing background music: " + music.FileName );
                }
            }
        }

        /// <summary>
        /// Utility method that gets a name for a new Entity of the given Type.
        /// </summary>
        /// <param name="type">
        /// The type of the new Entity.
        /// </param>
        /// <returns>
        /// The new name.
        /// </returns>
        private string GetNameForNewEntity( Type type )
        {
            if( type == null )
                throw new ArgumentNullException( "type" );

            Debug.Assert( typeof( Zelda.Entities.ZeldaEntity ).IsAssignableFrom( type ) );
            CultureInfo culture = CultureInfo.CurrentCulture;

            string baseName = type.Name;
            string name     = baseName + "_0";
            int   counter   = 0;

            while( this.Model.HasEntity( name ) )
            {
                name = baseName + '_' + (++counter).ToString( culture );
            }

            return name;
        }

        /// <summary>
        /// Helpers method that gets a value indicating whether
        /// the observable object list of this SceneViewModel
        /// contains the given ZeldaEntity directly or via an IObjectPropertyWrapper.
        /// </summary>
        /// <param name="entity">
        /// The entity to check for.
        /// </param>
        /// <returns>
        /// Returns true if the entity exists directly or via an IObjectPropertyWrapper in the objects list;
        /// or otherwise false.
        /// </returns>
        private bool HasEntityOrWrapper( ZeldaEntity entity )
        {
            foreach( var obj in this.objects )
            {
                if( obj == entity )
                    return true;

                var wrapper = obj as IObjectPropertyWrapper;
                if( wrapper != null && wrapper.WrappedObject == entity )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helpers method that tries to get the IObjectPropertyWrapper that wraps
        /// around the given ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity to check for.
        /// </param>
        /// <returns>
        /// The wrapper or null.
        /// </returns>
        public IObjectPropertyWrapper GetExistingWrapperFor( ZeldaEntity entity )
        {
            foreach( var obj in this.objects )
            {
                var wrapper = obj as IObjectPropertyWrapper;
                if( wrapper != null && wrapper.WrappedObject == entity )
                    return wrapper;
            }

            return null;
        }

        #region > Events <

        /// <summary>
        /// Called when an entity has been added to the scene model.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="entity">The ZeldaEntity that has been added.</param>
        private void OnSceneEntityAdded( object sender, ZeldaEntity entity )
        {
            if( !entity.IsEditable )
                return;

            if( !HasEntityOrWrapper( entity ) )
            {
                var wrapper = EntityPropertyWrapperFactory.ReceiveWrapper( entity );

                if( wrapper != null )
                {
                    this.objects.Add( wrapper );
                }
                else
                {
                    this.objects.Add( entity );
                }
            }
        }

        /// <summary>
        /// Called when an entity has been removed from the scene model.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="entity">The ZeldaEntity that has been removed.</param>
        private void OnSceneEntityRemoved( object sender, ZeldaEntity entity )
        {
            if( !this.objects.Remove( entity ) )
            {
                var wrapper = GetExistingWrapperFor( entity );

                if( wrapper != null )
                {
                    this.objects.Remove( wrapper );
                }
            }
        }

        /// <summary>
        /// Gets called when the user selects a different object.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnCurrentObjectChanged( object sender, EventArgs e )
        {
            OnPropertyChanged( "SelectedObject" );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The view model that wraps around the TileMap of the Scene.
        /// </summary>
        private readonly TileMapViewModel map;

        /// <summary>
        /// The view model that wraps around the ZeldaCamera of the Scene.
        /// </summary>
        private readonly CameraViewModel camera;

        /// <summary>
        /// The view model that wraps around the WaypointMap of the Scene.
        /// </summary>
        private readonly WaypointMapViewModel waypointMap;

        /// <summary>
        /// The view model that wraps around the Storyboard of the Scene.
        /// </summary>
        private readonly StoryboardViewModel storyboard;

        /// <summary>
        /// Stores the objects of the Scene which are exposed to the user.
        /// </summary>
        private readonly DispatchingObservableCollection<object> objects = new DispatchingObservableCollection<object>( Dispatcher.CurrentDispatcher );

        /// <summary>
        /// The view over the objects collection.
        /// </summary>
        private readonly CollectionView objectsView;

        /// <summary>
        /// The view model that wraps around the EventManager of the Scene.
        /// </summary>
        private readonly EventManagerViewModel eventManager;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}