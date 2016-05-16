// <copyright file="IEditModeDrawable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IEditModeDrawable interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda
{
    using Atom;

    /// <summary>
    /// Defines the interface of an drawable object that supports 
    /// special drawing if the application/game is in edit-mode.
    /// </summary>
    public interface IEditModeDrawable : IFloorDrawable
    {
        /// <summary>
        /// Draws this <see cref="IEditModeDrawable"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void DrawEditMode( ZeldaDrawContext drawContext );
    }
}
