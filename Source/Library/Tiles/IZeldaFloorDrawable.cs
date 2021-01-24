// <copyright file="IZeldaFloorDrawable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IZeldaFloorDrawable interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Specifies the interface of a drawable object which is part of a floor, 
    /// such as a TileMapFloor.
    /// </summary>
    public interface IZeldaFloorDrawable : Atom.IFloorDrawable
    {
        /// <summary>
        /// Gets the secondary draw order value of this IZeldaFloorDrawable.
        /// </summary>
        /// <value>
        /// This value is used as a secondary sorting-value that is
        /// used when the RelativeDrawOrder of two IZeldaFloorDrawable is equal.
        /// </value>
        float SecondaryDrawOrder
        {
            get;
        }

        /// <summary>
        /// Called before drawing anything is drawn.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void PreDraw( ZeldaDrawContext drawContext );
    }
}
