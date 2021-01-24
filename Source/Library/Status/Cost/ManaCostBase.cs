// <copyright file="Cost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.Cost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Provides a base implementation of the IManaCost interface.
    /// </summary>
    internal abstract class ManaCostBase : IManaCost
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
        public abstract int Get( Statable user );

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
            return Has( this.Get( user ), user );
        }

        /// <summary>
        /// Gets a value indicating whether the given user has the given mana.
        /// </summary>
        /// <param name="cost">
        /// The mana the user is required to have.
        /// </param>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// Returns true if the given user has the required mana;
        /// otherwise false.
        /// </returns>
        private static bool Has( int cost, Statable user )
        {
            return user.Mana >= cost;
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
            int cost = this.Get( user );

            if( Has( cost, user ) )
            {
                user.Mana -= cost;
                return true;
            }

            return false;
        }
    }
}
