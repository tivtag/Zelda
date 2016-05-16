// <copyright file="SpawnConstants.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Entities.Spawning.SpawnConstants class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    /// <summary>
    /// Contains tweakable spawning constants.
    /// </summary>
    internal static class SpawnConstants
    {
        /// <summary>
        /// The time (in seconds) it takes for a respawning entity to fully fade-in.
        /// </summary>
        public const float SpawnFadeInTime = 1.5f;

        /// <summary>
        /// The time (in seconds) it takes for a despawning entity to fully fade-out.
        /// </summary>
        public const float DespawnFadeOutTime = 2.5f;
    }
}
