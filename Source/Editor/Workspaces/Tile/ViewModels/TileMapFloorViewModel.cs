// <copyright file="TileMapFloorViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.TileMapFloorViewModel class.
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
    
    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="TileMapFloor"/> to provide them to the View (WPF).
    /// </summary>
    public sealed class TileMapFloorViewModel : ViewModel<TileMapFloor>
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapFloorViewModel"/> class.
        /// </summary>
        /// <param name="floor">
        /// The floor the new ViewModel wraps around.
        /// </param>
        /// <param name="scene">
        /// The Scene that owns the new TileMapFloorViewModel.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="floor"/>, <paramref name="map"/> or <paramref name="serviceProvider"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the ActionLayer of the given <paramref name="floor"/> is null.
        /// </exception>
        public TileMapFloorViewModel( TileMapFloor floor, TileMapViewModel map, IZeldaServiceProvider serviceProvider )
            : base( floor )
        {
            #region Verify Arguments

            if( floor == null )
                throw new ArgumentNullException( "floor" );

            if( map == null )
                throw new ArgumentNullException( "map" );

            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            if( floor.ActionLayer == null )
                throw new ArgumentException( Properties.Resources.Error_ActionLayerOfFloorIsNull, "floor" );

            #endregion

            this.Map             = map;
            this.ServiceProvider = serviceProvider;

            layers = new ObservableCollection<TileMapSpriteDataLayerViewModel>();
            foreach( var layer in floor.Layers )
                layers.Add( new TileMapSpriteDataLayerViewModel( (TileMapSpriteDataLayer)layer, this, serviceProvider ) );

            actionLayer = new ActionLayerViewModel( (TileMapSpriteDataLayer)floor.ActionLayer, this, serviceProvider );

            // Setup Commands:
            this.AddLayer    = new AddLayerCommand( this );
            this.RemoveLayer = new RemoveLayerCommand( this );
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when the currently <see cref="SelectedLayer"/> has changed. 
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<TileMapSpriteDataLayerViewModel>> SelectedLayerChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Receives (a bindable) collection that
        /// contains the (bindable) <see cref="TileMapSpriteDataLayer"/>s of the TileMap.
        /// </summary>
        public IEnumerable<TileMapSpriteDataLayerViewModel> Layers
        {
            get
            { 
                return this.layers;
            }
        }

        /// <summary>
        /// Receives the (bindable) action-layer of the TileMapFlor.
        /// </summary>
        public ActionLayerViewModel ActionLayer
        {
            get
            {
                return this.actionLayer;
            }
        }

        /// <summary>
        /// Receives the (bindable) name of the TileMapFloor.
        /// </summary>
        public string Name
        {
            get 
            {
                return string.Format( CultureInfo.CurrentCulture,
                    Properties.Resources.Format_FloorNameIndexX,
                    this.Model.FloorNumber.ToString( CultureInfo.CurrentCulture )
                );
            }
        }

        /// <summary>
        /// Gets or sets the currently selected TileMapSpriteDataLayer.
        /// </summary>
        public TileMapSpriteDataLayerViewModel SelectedLayer
        {
            get 
            {
                return this._selectedLayer; 
            }

            set
            {
                if( value == this._selectedLayer )
                    return;

                var oldValue = this._selectedLayer;

                if( value != null )
                {
                    if( !layers.Contains( value ) )
                        throw new ArgumentException( Properties.Resources.Error_GivenLayerIsNotPartOfFloor, "value" );

                    this._selectedLayer = value;
                }
                else
                {
                    this._selectedLayer = null;
                }

                OnPropertyChanged( "SelectedLayer" );
                if( SelectedLayerChanged != null )
                    SelectedLayerChanged( this, new ChangedValue<TileMapSpriteDataLayerViewModel>( oldValue, value ) );
            }
        }

        /// <summary>
        /// Receives the TileMapViewModel that owns this TileMapFloorViewModel.
        /// </summary>
        public TileMapViewModel Map
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Receives an object which provides fast access to game services.
        /// </summary>
        private IZeldaServiceProvider ServiceProvider
        {
            get;
            set;
        }

        #region > Commands <

        /// <summary>
        /// Receives the <see cref="ICommand"/> that when executed
        /// adds a new TileMapDataLayer to the TileMap.
        /// </summary>
        public ICommand AddLayer
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the <see cref="ICommand"/> that when executed
        /// removes the currently <see cref="SelectedLayer"/>.
        /// </summary>
        public ICommand RemoveLayer
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region [ Methods ]

        #endregion

        #region [ Commands ]

        /// <summary>
        /// Defines the <see cref="ICommand"/> that when executed
        /// adds a new TileMapSpriteDataLayer to the TileMap.
        /// </summary>
        private sealed class AddLayerCommand : ViewModelCommand<TileMapFloorViewModel, TileMapFloor>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AddLayerCommand"/> class.
            /// </summary>
            /// <param name="viewModel">The ViewModel that owns this ICommand.</param>
            public AddLayerCommand( TileMapFloorViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var floor = this.Model;
                var layer = new TileMapSpriteDataLayer( "New Layer", floor, floor.Map.Width, floor.Map.Height );
                
                // Setup layer:
                layer.TypeId = (int)TileMapDataLayerType.Normal;
                
                // Add Layer-Model to Floor-Model:  
                floor.AddLayer( layer );

                // Add Layer-ViewModel to Floor-ViewModel
                this.ViewModel.layers.Add( new TileMapSpriteDataLayerViewModel( layer, this.ViewModel, this.ViewModel.ServiceProvider ) );
            }
        }

        /// <summary>
        /// Defines the <see cref="ICommand"/> that when executed
        /// renives the currently selected TileMapSpriteDataLayer from the TileMap.
        /// </summary>
        private sealed class RemoveLayerCommand : ViewModelCommand<TileMapFloorViewModel, TileMapFloor>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveLayerCommand"/> class.
            /// </summary>
            /// <param name="viewModel">The ViewModel that owns this ICommand.</param>
            public RemoveLayerCommand( TileMapFloorViewModel viewModel )
                : base( viewModel )
            {
                viewModel.SelectedLayerChanged += ViewModel_SelectedLayerChanged;
            }

            /// <summary>
            /// Gets a value that indicates whether this ICommand can be executed.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.SelectedLayer != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !CanExecute( parameter ) )
                    return;

                var message = string.Format( CultureInfo.CurrentCulture,
                    Properties.Resources.Qeestion_ReallyRemoveLayerXOfFloorY,
                    this.ViewModel.SelectedLayer.Name, this.ViewModel.Name 
                );

                if( QuestionMessageBox.Show( message ) )
                {
                    var layer = this.ViewModel.SelectedLayer;

                    this.Model.RemoveLayer( layer.Model );
                    this.ViewModel.layers.Remove( layer );
                }
            }

            /// <summary>
            /// Gets called when the currently selected layer has changed.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void ViewModel_SelectedLayerChanged( object sender, ChangedValue<TileMapSpriteDataLayerViewModel> e )
            {
                OnCanExecuteChanged();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the currently selected TileMapSpriteDataLayer.
        /// </summary>
        private TileMapSpriteDataLayerViewModel _selectedLayer;

        /// <summary>
        /// Bindable list of <see cref="TileMapDataLayerViewModel"/>s that
        /// wrap around of the <see cref="TileMapDataLayer"/>s of the TileMap.
        /// </summary>
        private readonly ObservableCollection<TileMapSpriteDataLayerViewModel> layers;

        /// <summary>
        /// The (bindable) action layer of the TileMapFloor.
        /// </summary>
        private readonly ActionLayerViewModel actionLayer;

        #endregion
    }
}
