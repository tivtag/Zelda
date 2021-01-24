// <copyright file="IZeldaTileHandler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IZeldaTileHandler interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Atom.Scene.Tiles;
    using Zelda.Entities.Components;

    /// <summary>
    /// Descripes the interface of an object that 
    /// handles interaction with a TileMapFloor's ActionLayer.
    /// </summary>
    public interface IZeldaTileHandler : ITileHandler<Moveable>
    {
    }
}
