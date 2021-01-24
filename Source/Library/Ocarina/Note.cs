// <copyright file="Note.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Ocarina.Note enumeration.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Ocarina
{
    using System;

    /// <summary>
    /// Enumerates the different notes the player can play on an <see cref="Instrument"/>.
    /// </summary>
    public enum Note
    {
        /// <summary>
        /// No specific note.
        /// </summary>
        None,

        /// <summary>
        /// The note that occurs when the player uses the Left key.
        /// </summary>
        Left,

        /// <summary>
        /// The note that occurs when the player uses the Right key.
        /// </summary>
        Right,

        /// <summary>
        /// The note that occurs when the player uses the Up key.
        /// </summary>
        Up,

        /// <summary>
        /// The note that occurs when the player uses the Down key.
        /// </summary>
        Down
    }
}
