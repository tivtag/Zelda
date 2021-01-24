// <copyright file="WorldStatusProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary> Defines the Zelda.Saving.WorldStatusProvider class. </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Saving
{
    using System.Diagnostics;
    using Zelda.GameStates;
    using Atom.Diagnostics.Contracts;
    using System;
    
    /// <summary>
    /// Implements a mechanism to receive the current status of the world.
    /// </summary>
    internal sealed class WorldStatusProvider : IWorldStatusProvider
    {
        /// <summary>
        /// Gets the <see cref="WorldStatus"/> object;
        /// responsible for holding the overal state of the game world.
        /// </summary>
        public WorldStatus WorldStatus
        {
            get
            {
                var profile = ingameState.Profile;
                if( profile == null )
                    return null;

                return profile.WorldStatus;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WorldStatusProvider class.
        /// </summary>
        /// <param name="ingameState">
        /// The GameState that is running the actual game logic.
        /// </param>
        public WorldStatusProvider( IngameState ingameState )
        {
            Contract.Requires<ArgumentNullException>( ingameState != null );

            this.ingameState = ingameState;
        }

        /// <summary>
        /// Identifies the GameState that is running the actual game logic.
        /// </summary>
        private readonly IngameState ingameState;
    }
}
