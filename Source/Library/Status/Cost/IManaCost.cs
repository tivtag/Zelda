// <copyright file="IManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.IManaCost interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Provides an abstraction of the mana cost concept of an attack/spell/skill.
    /// </summary>
    public interface IManaCost
    {
        /// <summary>
        /// Gets the actual mana cost.
        /// </summary>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// The fixed mana cost.
        /// </returns>
        int Get( Statable user );

        /// <summary>
        /// Gets a value indicating whether the given user has the required mana.
        /// </summary>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// Returns true if the given user has the required mana;
        /// otherwise false.
        /// </returns>
        bool Has( Statable user );

        /// <summary>
        /// Tries to apply this IManaCost to the given user.
        /// </summary>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// Returns true if the mana cost has been applied to the user;
        /// otherwise false.
        /// </returns>
        bool Apply( Statable user );
    }
}
