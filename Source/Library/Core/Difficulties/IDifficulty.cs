// <copyright file="IDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.IDifficulty interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    using Atom;
    using Zelda.Entities;

    /// <summary>
    /// Represents a difficulty mode of the game.
    /// </summary>
    public interface IDifficulty : IReadOnlyNameable
    {
        /// <summary>
        /// Gets the <see cref="DifficultyId"/> that uniquely identifies this IDifficulty.
        /// </summary>
        DifficultyId Id { get; }

        /// <summary>
        /// Applies this IDifficulty to the specified Enemy.
        /// </summary>
        /// <param name="enemy">
        /// The enemy to apply this IDifficulty on.
        /// </param>
        void ApplyOn( Enemy enemy );

        /// <summary>
        /// Called when this IDifficulty has been choosen or given up.
        /// </summary>
        /// <param name="changeType">
        /// States whether this IDifficulty has been choosen or given up.
        /// </param>
        void OnChange( ChangeType changeType );
    }
}
