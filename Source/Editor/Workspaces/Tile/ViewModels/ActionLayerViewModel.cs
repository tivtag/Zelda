// <copyright file="ActionLayerViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.ActionLayerViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Tile.ViewModels
{
    using System;
    using Atom.Scene.Tiles;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="TileMapSpriteDataLayer"/> that represents an action layer
    /// to provide them to the View (WPF).
    /// </summary>
    public sealed class ActionLayerViewModel : TileMapSpriteDataLayerViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLayerViewModel"/> class.
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
        public ActionLayerViewModel( TileMapSpriteDataLayer layer, TileMapFloorViewModel floor, IZeldaServiceProvider serviceProvider )
            : base( layer, floor, serviceProvider )
        {
            if( ((TileMapDataLayerType)layer.TypeId) != TileMapDataLayerType.Action )
            {
                throw new ArgumentException( Properties.Resources.Error_GivenLayerIsNoActionLayer, "layer" );
            }
        }
    }
}
