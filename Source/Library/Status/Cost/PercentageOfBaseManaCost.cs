// <copyright file="PercentageOfBaseManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.PercentageOfBaseManaCost class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Defines an <see cref="IManaCost"/> that costs a percentage of the base mana
    /// of the <see cref="Zelda.Status.ExtendedStatable"/> that uses the mana.
    /// </summary>
    internal sealed class PercentageOfBaseManaCost : ManaCostBase
    {
        /// <summary>
        /// Initializes a new instance of the PercentageOfBaseManaCost class.
        /// </summary>
        /// <param name="percentage">
        /// The percentage the new PercentageOfBaseManaCost encapsulates.
        /// </param>
        public PercentageOfBaseManaCost( float percentage )
        {
            this.percentage = percentage;
        }
        
        /// <summary>
        /// Gets the actual mana cost.
        /// </summary>
        /// <param name="user">
        /// The user that wishes to use mana.
        /// </param>
        /// <returns>
        /// The fixed mana cost.
        /// </returns>
        public override int Get( Statable user )
        {
            var userEx = (ExtendedStatable)user;
            return userEx.GetPercentageOfBaseMana( percentage );
        }

        /// <summary>
        /// Stores the percentage encapsulated by this PercentageOfBaseManaCost.
        /// </summary>
        private readonly float percentage;
    }
}