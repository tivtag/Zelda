// <copyright file="ExternalEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ExternalEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using Atom.Events;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents an <see cref="Event"/> that is not saved and
    /// also not removeable from the <see cref="EventManager"/> by the user.
    /// </summary>
    public abstract class ExternalEvent : Event, ISavedState, IRemoveableState
    {
        /// <summary>
        /// Gets a value indicating whether 
        /// this Event is allowed be removed manually by the user.
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
        /// this Event should be saved.
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
