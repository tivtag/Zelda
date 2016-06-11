// <copyright file="SharedCooldownMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.SharedCooldownMap class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System.Diagnostics;

    /// <summary>
    /// The <see cref="SharedCooldownMap"/> stores <see cref="Cooldown"/>s that are shared
    /// over multiple objects.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The Cooldowns are sorted by the cooldown's unique ID.
    /// </remarks>
    public sealed class SharedCooldownMap : System.Collections.Generic.Dictionary<int, Cooldown>
    {
        /// <summary>
        /// Gets the <see cref="Status.ExtendedStatable"/> that owns this SharedCooldownMap.
        /// </summary>
        public Status.ExtendedStatable Owner
        {
            get 
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedCooldownMap"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Status.ExtendedStatable"/> that owns the new SharedCooldownMap.
        /// </param>
        public SharedCooldownMap( Status.ExtendedStatable owner )
        {
            Debug.Assert( owner != null );

            this.owner = owner;
        }

        /// <summary>
        /// The <see cref="Status.ExtendedStatable"/> that owns this SharedCooldownMap.
        /// </summary>
        private readonly Status.ExtendedStatable owner;
    }
}
