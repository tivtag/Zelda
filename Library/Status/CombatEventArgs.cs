// <copyright file="CombatEventArgs.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.CombatEventArgs structure.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines the event arguments used by the Statable.
    /// </summary>
    public struct CombatEventArgs
    {
        /// <summary>
        /// The object that initiated the combat event.
        /// </summary>
        public readonly Statable User;

        /// <summary>
        /// The target of the combat event.
        /// </summary>
        public readonly Statable Target;

        /// <summary>
        /// Initializes a new instance of the CombatEventArgs structure.
        /// </summary>
        /// <param name="user">
        /// The user of the combat event.
        /// </param>
        /// <param name="target">
        /// The target of the combat event.
        /// </param>
        internal CombatEventArgs( Statable user, Statable target )
        {
            this.User   = user;
            this.Target = target;
        }
    }
}
