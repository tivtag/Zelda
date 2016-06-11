// <copyright file="TileMapDataLayerViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.TileMapDataLayerViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile.ViewModels
{
    using System;
    using Atom.Scene.Tiles;
    using Atom.Wpf;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="TileMapDataLayer"/> to provide them to the View (WPF).
    /// </summary>
    /// <remarks>
    /// Maps in TLoZ - BC only consist of <see cref="TileMapSpriteDataLayer"/>s.
    /// This class encapsulates everything that is related too ALL TileMapDataLayers.
    /// </remarks>
    public abstract class TileMapDataLayerViewModel : ViewModel<TileMapDataLayer>
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapDataLayerViewModel"/> class.
        /// </summary>
        /// <param name="layer">
        /// The layer the new ViewModel wraps around.
        /// </param>
        /// <param name="floor">
        /// The TileMapFloorViewModel that owns the new TileMapDataLayerViewModel.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="layer"/>, <paramref name="floor"/> or <paramref name="serviceProvider"/> is null.
        /// </exception>
        public TileMapDataLayerViewModel( TileMapDataLayer layer, TileMapFloorViewModel floor, IZeldaServiceProvider serviceProvider )
            : base( layer )
        {
            #region [ Verify Arguments ]

            if( floor == null )
                throw new ArgumentNullException( "floor" );

            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            #endregion

            this.Floor = floor;
            this.ServiceProvider = serviceProvider;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the (bindable) Name of the TileMapDataLayer.
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
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets or sets a (bindable) value that indicates whether the TileMapDataLayer is visible.
        /// </summary>
        public bool IsVisible
        {
            get 
            { 
                return this.Model.IsVisible; 
            }

            set
            {
                if( value == this.IsVisible )
                    return;

                this.Model.IsVisible = value;
                this.OnPropertyChanged( "IsVisible" );
            }
        }

        /// <summary>
        /// Receives an object which provides fast access to game services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the TileMapFloorViewModel that owns this TileMapDataLayerViewModel.
        /// </summary>
        protected TileMapFloorViewModel Floor
        {
            get;
            private set;
        }

        #endregion

        #region [ Methods ]

        #endregion

        #region [ Fields ]

        #endregion
    }
}
