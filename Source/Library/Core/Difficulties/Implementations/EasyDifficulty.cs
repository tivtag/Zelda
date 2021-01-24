// <copyright file="EasyDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.EasyDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

using Zelda.Status;

namespace Zelda.Difficulties
{
    /// <summary>
    /// Represents the easiest IDifficulty of the game.
    /// </summary>
    public sealed class EasyDifficulty : BaseDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public override DifficultyId Id 
        {
            get
            {
                return DifficultyId.Easy; 
            }
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public override string Name
        {
            get 
            {
                return Resources.Easy;
            }
        }
        
        /// <summary>
        /// Creates the StatusValueEffects that are applied to enemies.
        /// </summary>
        /// <returns>
        /// The newly created StatusValueEffects.
        /// </returns>
        protected override StatusValueEffect[] CreateStatusEffects()
        {
            return new StatusValueEffect[] {
                new MovementSpeedEffect( 10.0f, StatusManipType.Percental )
            };
        }
    }
}
