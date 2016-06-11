// <copyright file="TileWorkspaceControl.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.TileWorkspaceControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile
{
    using System.Windows.Controls;
    using Zelda.Editor.Tile.ViewModels;

    /// <summary>
    /// Contains the WPF user interface responsible for editing the TileWorkspace.
    /// </summary>
    public partial class TileWorkspaceControl : UserControl
    {
        /// <summary>
        /// Gets or sets the <see cref="TileWorkspace"/> this TileWorkspaceControl manipulates.
        /// </summary>
        public TileWorkspace Workspace 
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the TileWorkspaceControl class.
        /// </summary>
        public TileWorkspaceControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets called when the currently selected floor has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedFloorChanged( object sender, SelectionChangedEventArgs e )
        {
            var scene = this.Workspace.Scene;
            if( scene == null )
                return;

            var floor = (TileMapFloorViewModel)this.listBoxFloors.SelectedItem;
            scene.Map.SelectedFloor = floor;

            if( floor != null )
            {
                this.listBoxLayers.SelectedItem = floor.SelectedLayer;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Gets called when the currently selected layer has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SelectionChangedEventArgs"/> that contain the event data.
        /// </param>
        private void OnSelectedLayerChanged( object sender, SelectionChangedEventArgs e )
        {
            var scene = this.Workspace.Scene;
            if( scene == null )
                return;

            var floor = scene.Map.SelectedFloor;
            if( floor == null )
                return;

            var layer = (TileMapSpriteDataLayerViewModel)this.listBoxLayers.SelectedItem;
            if( layer == null )
                return; // SelectedItem is null when the binding changes whilen switching from one Floor to another.

            // We don't want to lose the currently selected layer of the floor tho.
            floor.SelectedLayer = layer;
        }
    }
}