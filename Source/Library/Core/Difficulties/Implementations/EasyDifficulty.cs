// <copyright file="EasyDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.EasyDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    /// <summary>
    /// Represents the easiest IDifficulty of the game.
    /// </summary>
    public sealed class EasyDifficulty : IDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public DifficultyId Id 
        {
            get
            {
                return DifficultyId.Easy; 
            }
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public string Name
        {
            get 
            {
                return Resources.Easy;
            }
        }

        /// <summary>
        /// Applies this IDifficulty to the specified Enemy.
        /// </summary>
        /// <param name="enemy">
        /// The enemy to apply this IDifficulty on.
        /// </param>
        public void ApplyOn( Entities.Enemy enemy )
        {
            // no op.
        }

        /// <summary>
        /// Called when this IDifficulty has been choosen or given up.
        /// </summary>
        /// <param name="changeType">
        /// States whether this IDifficulty has been choosen or given up.
        /// </param>
        public void OnChange( ChangeType changeType )
        {
            // no op.
        }
    }
}
