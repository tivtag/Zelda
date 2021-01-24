// <copyright file="FactionList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the static Zelda.Factions.FactionList class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Factions
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Provides static access to all <see cref="Faction"/>s
    /// in the game.
    /// </summary>
    public static class FactionList
    {
        /// <summary>
        /// Gets an enumeration that contains all known <see cref="Faction"/>s.
        /// </summary>
        public static IEnumerable<Faction> Known
        {
            get 
            {
                return FactionList.factions; 
            }
        }

        /// <summary>
        /// Tries to get the <see cref="Faction"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is null.
        /// </exception>
        /// <param name="name">
        /// The name that uniquely identifies the Faction to get.
        /// </param>
        /// <returns>
        /// The requested Faction;
        /// or null if there exists no such Faction.
        /// </returns>
        public static Faction Get( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            foreach( var faction in FactionList.factions )
            {
                if( name.Equals( faction.Name, StringComparison.Ordinal ) )
                {
                    return faction;
                }
            }

            return null;
        }

        /// <summary>
        /// Stores all <see cref="Faction"/>s of the Zelda game.
        /// </summary>
        private static readonly Faction[] factions = new Faction[3] {
            new Faction( "BrotherhoodOfTheTemple", Resources.FD_BrotherhoodOfTheTemple, 0 ),                
            new Faction( "TownFolk", Resources.FD_TownFolk, 0 ),                
            new Faction( "Goronia", Resources.FD_Goronia, 0 )
        };
    }
}
