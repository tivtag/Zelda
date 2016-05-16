// <copyright file="Cost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.Cost class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using Zelda.Status.Cost;

    /// <summary>
    /// Provices factory methods that create instances of objects that implement the <see cref="IManaCost"/> interface.
    /// </summary>
    public static class ManaCost
    {
        /// <summary>
        /// Gets an instance of an object that implements the IManaCost interface;
        /// and costs absolutely nothing.
        /// </summary>
        public static IManaCost None
        {
            get
            {
                return NullManaCost.Instance;
            }
        }

        /// <summary>
        /// Gets an IManaCost that requires a fixed amount of mana.
        /// </summary>
        /// <param name="amount">
        /// The fixed amount of mana that is required.
        /// </param>
        /// <returns>
        /// A newly created instance of an object that implements the IManaCost interface.
        /// </returns>
        public static IManaCost Fixed( int amount )
        {
            return new FixedManaCost( amount );
        }

        /// <summary>
        /// Gets an IManaCost that requires a percentage of the base mana of the user.
        /// </summary>
        /// <param name="percentage">
        /// The percentage that is required; where 1.0f = 100% of base mana.
        /// </param>
        /// <returns>
        /// A newly created instance of an object that implements the IManaCost interface.
        /// </returns>
        public static IManaCost PercentageOfBase( float percentage )
        {
            return new PercentageOfBaseManaCost( percentage );
        }

        /// <summary>
        /// Gets an IManaCost that requires a percentage of the total mana of the user.
        /// </summary>
        /// <param name="percentage">
        /// The percentage that is required; where 1.0f = 100% of total mana.
        /// </param>
        /// <returns>
        /// A newly created instance of an object that implements the IManaCost interface.
        /// </returns>
        public static IManaCost PercentageOfTotal( float percentage )
        {
            return new PercentageOfTotalManaCost( percentage );
        }

        /// <summary>
        /// Gets an IManaCost that requires a percentage of the base mana plus a percentage of the total mana of the user.
        /// </summary>
        /// <param name="ofBasePercentage">
        /// The percentage of base mana that is required; where 1.0f = 100% of base mana.
        /// </param>
        /// <param name="ofTotalPercentage">
        /// The percentage of total mana that is required; where 1.0f = 100% of total mana.
        /// </param>
        /// <returns>
        /// A newly created instance of an object that implements the IManaCost interface.
        /// </returns>
        public static IManaCost PercentageOfBaseAndTotal( float ofBasePercentage, float ofTotalPercentage )
        {
            return new PercentageOfBaseAndTotalManaCost( ofBasePercentage, ofTotalPercentage );
        }
    }
}
