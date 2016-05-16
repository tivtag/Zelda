// <copyright file="HitCritHookMode.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Hooks.HitCritHookMode enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Hooks
{
    /// <summary>
    /// Enumerates the different ways an IStatusHook might
    /// hook up with an attack.
    /// </summary>
    public enum HitCritHookMode
    {
        /// <summary>
        /// The IStatusHook hooks up with both hits and crits.
        /// </summary>
        HitAndCrit = 0,

        /// <summary>
        /// The IStatusHook only hooks up with hits.
        /// </summary>
        OnlyHit,

        /// <summary>
        /// The IStatusHook only hooks up with crits.
        /// </summary>
        OnlyCrit
    }
}
