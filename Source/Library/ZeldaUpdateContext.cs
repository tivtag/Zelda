// <copyright file="ZeldaUpdateContext.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaUpdateContext class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    /// <summary>
    /// Defines the <see cref="Atom.IUpdateContext"/> used by the game.
    /// </summary>
    public sealed class ZeldaUpdateContext : Atom.Xna.XnaUpdateContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the game is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the current update is a main update;
        /// or a silent behind the scene update.
        /// </summary>
        /// <remarks>
        /// The last inactive scene is updated using non-main updates for a specific time before is really 'expire'.
        /// This is used to simulate a continous world.
        /// </remarks>
        public bool IsMainUpdate
        {
            get
            {
                return this.isMainUpdate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the game is currently running slowly.
        /// </summary>
        public bool IsRunningSlowly
        {
            get
            {
                return this.GameTime.IsRunningSlowly;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaUpdateContext"/> class.
        /// </summary>
        /// <param name="isMainUpdate">
        /// States whether updates done under the new ZeldaUpdateContext are main updates;
        /// or a silent behind the scene updates.
        /// </param>
        public ZeldaUpdateContext( bool isMainUpdate = true )
        {
            this.isMainUpdate = isMainUpdate;
        }

        /// <summary>
        /// States whether updates done under this ZeldaUpdateContext are main updates;
        /// or a silent behind the scene updates.
        /// </summary>
        private readonly bool isMainUpdate;
    }
}
