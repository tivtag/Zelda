// <copyright file="TileMapViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.TileMapViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Atom;
    using Atom.Scene.Tiles;
    using Atom.Wpf;
    using Atom.Xna;
    
    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="TileMap"/> to provide them to the View (WPF).
    /// </summary>
    public sealed class TileMapViewModel : ViewModel<TileMap>
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapViewModel"/> class.
        /// </summary>
        /// <param name="map">
        /// The map the new ViewModel wraps around.
        /// </param>
        /// <param name="scene">
        /// The SceneViewModel that owns the new TileMapViewModel.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public TileMapViewModel( TileMap map, SceneViewModel scene, IZeldaServiceProvider serviceProvider )
            : base( map )
        {
            #region Verify Arguments

            if( scene == null )
                throw new ArgumentNullException( "scene" );

            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            #endregion

            this.Scene           = scene;
            this.ServiceProvider = serviceProvider;

            floors = new ObservableCollection<TileMapFloorViewModel>();
            foreach( var floor in map.Floors )
                floors.Add( new TileMapFloorViewModel( floor, this, serviceProvider ) );

            // Setup commands:
            this.AddFloor    = new AddFloorCommand( this );
            this.RemoveFloor = new RemoveFloorCommand( this );
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Gets called when the currently <see cref="SelectedFloor"/> has changed. 
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<TileMapFloorViewModel>> SelectedFloorChanged;
        
        /// <summary>
        /// Gets called when the <see cref="FloorEditMode"/> has changed. 
        /// </summary>
        public event EventHandler FloorEditModeChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Receives (a bindable) collection that
        /// contains the (bindable) TileMapFloors of the TileMap.
        /// </summary>
        public IEnumerable<TileMapFloorViewModel> Floors
        {
            get 
            { 
                return this.floors;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected (bindable) TileMapFloor.
        /// </summary>
        public TileMapFloorViewModel SelectedFloor
        {
            get
            {
                return this._selectedFloor; 
            }

            set
            {
                if( value == this.SelectedFloor )
                    return;

                if( value != null )
                {
                    if( !this.floors.Contains( value ) )
                    {
                        throw new ArgumentException( Properties.Resources.Error_GivenFloorIsNotPartOfTileMap, "value" );
                    }
                }

                var oldValue = this._selectedFloor;
                this._selectedFloor = value;

                this.OnPropertyChanged( "SelectedFloor" );
                this.OnSelectedFloorChanged( oldValue, value );
            }
        }

        /// <summary>
        /// States whether the user is currently editing the
        /// selected TileMapSpriteDataLayer of the selected TileMapFloor
        /// or the Action Layer of the selected TileMapFloor.
        /// </summary>
        public TileMapFloorEditMode FloorEditMode
        {
            get 
            {
                return _floorEditMode;
            }

            set
            {
                if( value == _floorEditMode )
                    return;

                this._floorEditMode = value;

                OnPropertyChanged( "FloorEditMode" );
                if( FloorEditModeChanged != null )
                    FloorEditModeChanged( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Gets or sets a (bindable) value which ActionLayers are when visible. 
        /// </summary>
        public ActionLayerVisabilityMode ActionLayerVisability
        {
            get
            {
                return _actionLayerVisability;
            }

            set
            {
                if( value == _actionLayerVisability )
                    return;

                _actionLayerVisability = value;
                // Apply change

                switch( _actionLayerVisability )
                {
                    case ActionLayerVisabilityMode.All:
                        foreach( var floor in this.floors )
                            floor.ActionLayer.IsVisible = true;
                        break;

                    case ActionLayerVisabilityMode.None:
                        foreach( var floor in this.floors )
                            floor.ActionLayer.IsVisible = false;
                        break;

                    case ActionLayerVisabilityMode.OnlyCurrent:
                        foreach( var floor in this.floors )
                            floor.ActionLayer.IsVisible = false;

                        if( SelectedFloor != null )
                            SelectedFloor.ActionLayer.IsVisible = true;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                // Notify change
                OnPropertyChanged( "ActionLayerVisability" );
            }
        }

        /// <summary>
        /// Receives the SceneViewModel that owns this TileMapViewModel.
        /// </summary>
        public SceneViewModel Scene
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives an object which provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider ServiceProvider
        {
            get;
            set;
        }
        
        #region > Commands <
        
        /// <summary>
        /// Receives the ICommand that when executed
        /// adds a new TileMapFloor to the TileMap.
        /// </summary>
        public ICommand AddFloor
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the ICommand that when executed
        /// asks the user whether he wishes the remove
        /// the currently selected TileMapFloor.
        /// </summary>
        public ICommand RemoveFloor
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Toggles the <see cref="TileMapFloorEditMode"/> of the <see cref="TileMapViewModel"/>.
        /// </summary>
        public void ToggleFloorEditMode()
        {
            switch( this.FloorEditMode )
            {
                case TileMapFloorEditMode.EditActionLayer:
                    this.FloorEditMode = TileMapFloorEditMode.EditSelectedLayer;
                    break;

                case TileMapFloorEditMode.EditSelectedLayer:
                    this.FloorEditMode = TileMapFloorEditMode.EditActionLayer;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Toggles between the <see cref="ActionLayerVisabilityMode"/>s of the <see cref="TileMapViewModel"/>.
        /// </summary>
        public void ToggleActionLayerVisability()
        {
            switch( this.ActionLayerVisability )
            {
                case ActionLayerVisabilityMode.All:
                    this.ActionLayerVisability = ActionLayerVisabilityMode.None;
                    break;

                case ActionLayerVisabilityMode.None:
                    this.ActionLayerVisability = ActionLayerVisabilityMode.OnlyCurrent;
                    break;

                case ActionLayerVisabilityMode.OnlyCurrent:
                    this.ActionLayerVisability = ActionLayerVisabilityMode.All;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets called when the currently selected TileMapFloor has changed.
        /// </summary>
        /// <param name="oldvalue">The old floor.</param>
        /// <param name="newValue">The new floor.</param>
        private void OnSelectedFloorChanged( TileMapFloorViewModel oldValue, TileMapFloorViewModel newValue )
        {
            if( this.ActionLayerVisability == ActionLayerVisabilityMode.OnlyCurrent )
            {
                if( oldValue != null )
                    oldValue.ActionLayer.IsVisible = false;

                if( newValue != null )
                    newValue.ActionLayer.IsVisible = true;
            }

            if( SelectedFloorChanged != null )
                SelectedFloorChanged( this, new ChangedValue<TileMapFloorViewModel>( oldValue, newValue ) );
        }

        #endregion

        #region [ Commands ]

        /// <summary>
        /// Defines the <see cref="ICommand"/> that when executed
        /// adds a new TileMapFloor to the TileMap.
        /// </summary>
        sealed class AddFloorCommand : ViewModelCommand<TileMapViewModel, TileMap>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AddFloorCommand"/> class.
            /// </summary>
            /// <param name="viewModel">The ViewModel that owns this ICommand.</param>
            public AddFloorCommand( TileMapViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                // Add Floor-Model to TileMap-Model:
                var floor = this.Model.AddFloor( 3 );

                // Every floor has an independent action layer:
                var actionLayer = new TileMapSpriteDataLayer( "ActionLayer_" + floor.FloorNumber.ToString( CultureInfo.CurrentCulture ), 
                    floor, floor.Map.Width, floor.Map.Height );

                SetupActionLayer( actionLayer );
                floor.ActionLayer = actionLayer;

                // Add Floor-ViewModel to TileMap-ViewModel:
                this.ViewModel.floors.Add( new TileMapFloorViewModel( floor, this.ViewModel, this.ViewModel.ServiceProvider ) );
            }

            /// <summary>
            /// Setups the given ActionLayer.
            /// </summary>
            /// <param name="actionLayer">The action layer to lose.</param>
            private void SetupActionLayer( TileMapSpriteDataLayer actionLayer )
            {
                // Set SpriteSheet
                var sheetLoader = this.ViewModel.ServiceProvider.SpriteSheetLoader;
                actionLayer.Sheet = (SpriteSheet)sheetLoader.LoadSpriteSheet( Properties.Resources.AssetPath_ActionSheet );

                actionLayer.TypeId    = (int)TileMapDataLayerType.Action;
                actionLayer.IsVisible = (ViewModel.ActionLayerVisability == ActionLayerVisabilityMode.All);

                // fill outer border of action layer
                // with solid tiles
                int width  = actionLayer.Width;
                int height = actionLayer.Height;
                int bottom = height - 1;
                int right  = width  - 1;

                for( int x = 0; x < width; ++x )
                {
                    actionLayer.SetTile( x, 0, (int)ActionTileId.SolidForAll );      // up
                    actionLayer.SetTile( x, bottom, (int)ActionTileId.SolidForAll ); // bottom
                }

                for( int y = 1; y < bottom; ++y )
                {
                    actionLayer.SetTile(     0, y, (int)ActionTileId.SolidForAll ); // left
                    actionLayer.SetTile( right, y, (int)ActionTileId.SolidForAll ); // right
                }
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ICommand"/> that when executed
        /// asks whether the user wants to remove the currently selected TileMapFloor from the TileMap.
        /// </summary>
        sealed class RemoveFloorCommand : ViewModelCommand<TileMapViewModel, TileMap>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveFloorCommand"/> class.
            /// </summary>
            /// <param name="viewModel">The ViewModel that owns this ICommand.</param>
            public RemoveFloorCommand( TileMapViewModel viewModel )
                : base( viewModel )
            {
                viewModel.SelectedFloorChanged += SelectedFloorChanged;
            }

            /// <summary>
            /// Receives a value indicating whether this RemoveFloorCommand can currently be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns>
            /// true if it can be executed;
            /// otherwise false.
            /// </returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedFloor != null;
            }

            /// <summary>
            /// Executes this RemoveFloorCommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !CanExecute( parameter ) )
                    return;

                // Ask the user
                string question = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resources.Question_ReallyRemoveFloorX,
                    this.ViewModel.SelectedFloor.Name ?? string.Empty
                );

                if( QuestionMessageBox.Show( question ) )
                {
                    var selectedFloor = this.ViewModel.SelectedFloor;

                    // Remove from Model
                    if( this.Model.RemoveFloor( selectedFloor.Model ) )
                    {
                        // Remove from ViewModel
                        this.ViewModel.floors.Remove( selectedFloor );

                        // Update selection
                        this.ViewModel.SelectedFloor = null;
                    }
                }
            }

            /// <summary>
            /// Gets called when the currently selected TileMapFloor has changed.
            /// </summary>
            /// <param name="sender">
            /// The sender of the event.
            /// </param>
            /// <param name="e">
            /// The ChangedValue that contains the event data.
            /// </param>
            void SelectedFloorChanged( object sender, ChangedValue<TileMapFloorViewModel> e )
            {
                OnCanExecuteChanged();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the currently selected TileMapFloor.
        /// </summary>
        private TileMapFloorViewModel _selectedFloor;

        /// <summary>
        /// States whether the user is currently editing the
        /// selected TileMapSpriteDataLayer of the selected TileMapFloor
        /// or the Action Layer of the selected TileMapFloor.
        /// </summary>
        private TileMapFloorEditMode _floorEditMode;

        /// <summary>
        /// Specifies which ActionLayers are when visible. 
        /// </summary>
        private ActionLayerVisabilityMode _actionLayerVisability;
        
        /// <summary>
        /// Bindable list of <see cref="TileMapFloorViewModel"/>s that
        /// wrap around of the <see cref="TileMapFloor"/>s of the TileMap.
        /// </summary>
        private readonly ObservableCollection<TileMapFloorViewModel> floors;

        #endregion
    }
}
