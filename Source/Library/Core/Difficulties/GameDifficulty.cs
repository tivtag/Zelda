// <copyright file="GameDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Difficulties.GameDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    using System.Collections.Generic;
    using System.Linq;
    using Zelda.Entities;

    /// <summary>
    /// Provides access to the currently selected <see cref="IDifficulty"/> of the game.
    /// </summary>
    public static class GameDifficulty
    {
        /// <summary>
        /// Gets the different difficulties the game has to offer.
        /// </summary>
        public static IEnumerable<IDifficulty> All
        {
            get
            {
                return difficulties;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected <see cref="IDifficulty"/>.
        /// </summary>
        public static DifficultyId Current
        {
            get
            {
                return current.Id;
            }

            set
            {
                if( current != null )
                {
                    if( value == Current )
                        return;

                    current.OnChange( ChangeType.Away );
                }

                current = difficulties.First( difficulty => difficulty.Id == value );
                current.OnChange( ChangeType.To );
            }
        }
        
        /// <summary>
        /// Applies the current <see cref="IDifficulty"/> on the specified <see cref="Enemy"/>.
        /// </summary>
        /// <param name="enemy">
        /// The enemy to apply the current IDifficulty on.
        /// </param>
        public static void ApplyOn( Enemy enemy )
        {
            current.ApplyOn( enemy );
        }

        /// <summary>
        /// Applies the current <see cref="IDifficulty"/> on the specified <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="entity">
        /// The ZeldaEntity to apply the current IDifficulty on.
        /// </param>
        public static void ApplyOn( ZeldaEntity entity )
        {
            var enemy = entity as Enemy;

            if( enemy != null )
            {
                current.ApplyOn( enemy );
            }
        }

        /// <summary>
        /// The currently selected difficulty.
        /// </summary>
        private static IDifficulty current;

        /// <summary>
        /// Enumerates the different difficulties the game has to offer.
        /// </summary>
        private static readonly IDifficulty[] difficulties = new IDifficulty[] { 
            new EasyDifficulty(),
            new NormalDifficulty(),
            new NightmareDifficulty(),
            new HellDifficulty(),
            new InsaneDifficulty()
        };
    }
}
