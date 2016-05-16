// <copyright file="ExternalTileAreaEventTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ExternalTileAreaEventTrigger class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using Atom.Events;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="TileAreaEventTrigger"/> that is not saved and
    /// also not removeable from the <see cref="EventManager"/> by the user.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ExternalTileAreaEventTrigger : ZeldaTileAreaEventTrigger, ISavedState, IRemoveableState
    {
        /// <summary>
        /// Gets a value indicating whether 
        /// this TileAreaEventTrigger is allowed be removed manually by the user.
        /// </summary>
        /// <value>Always returns <see langword="false"/>.</value>
        public bool IsRemoveAllowed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether 
        /// this TileAreaEventTrigger should be saved.
        /// </summary>
        /// <value>Always returns <see langword="false"/>.</value>
        public bool IsSaved
        {
            get
            {
                return false;
            }
        }
    }
}
