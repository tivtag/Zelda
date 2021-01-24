// <copyright file="NullManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.NullManaCost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Defines an <see cref="IManaCost"/> that costs absolutely nothing.
    /// </summary>
    internal sealed class NullManaCost : IManaCost
    {
        /// <summary>
        /// Represents an instance of the NullManaCost class.
        /// </summary>
        public static readonly IManaCost Instance = new NullManaCost();

        /// <summary>
        /// Gets the actual mana cost.
        /// </summary>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// The fixed mana cost.
        /// </returns>
        public int Get( Statable user )
        {
            return 0;
        }

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
        public bool Has( Statable user )
        {
            return true;
        }

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
        public bool Apply( Statable user )
        {
            return true;
        }
    }
}
