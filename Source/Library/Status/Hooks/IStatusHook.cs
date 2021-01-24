// <copyright file="IStatusHook.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Hooks.IStatusHook interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Hooks
{
    using System.ComponentModel;
    using Atom;
    
    /// <summary>
    /// Provides a mechanism that allows one to hook up
    /// with a statable Entity and listen to its events.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IStatusHook : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Fired when this IStatusHook has been invoked.
        /// </summary>
        event RelaxedEventHandler<Statable> Invoked;

        /// <summary>
        /// Hooks this IStatusHook up with the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        void Hook( Statable statable );

        /// <summary>
        /// Unhooks this IStatusHook from the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to unhook from.
        /// </param>
        void Unhook( Statable statable );

        /// <summary>
        /// Gets a short description of this IStatusHook.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that hooks up with this IStatusHook.
        /// </param>
        /// <returns>
        /// A short string that is descriping with what this IStatusHook is hooking up.
        /// </returns>
        string GetShortDescription( Statable statable );

        /// <summary>
        /// Returns a clone of this IStatusHook.
        /// </summary>
        /// <returns>
        /// The cloned IStatusHook.
        /// </returns>
        IStatusHook Clone();
    }
}
