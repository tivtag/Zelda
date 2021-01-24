// <copyright file="IProcChance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.IProcChance interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Procs
{
    using System.ComponentModel;
    using Atom.Math;

    /// <summary>
    /// Encapsulates the logic that decides 
    /// whether a proc has occured.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IProcChance : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether a proc has occured
        /// using the rules of this IProcChance by throwing the dice.
        /// </summary>
        /// <param name="caller">
        /// The statable component of the entity that
        /// tries to proc something.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// True if a proc has occured;
        /// otherwise false.
        /// </returns>
        bool TryProc( Statable caller, RandMT rand );
    }
}
