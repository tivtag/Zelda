// <copyright file="PercentageOfTotalManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.PercentageOfTotalManaCost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Defines an <see cref="IManaCost"/> that costs a percentage of the total mana
    /// of the <see cref="Zelda.Status.ExtendedStatable"/> that uses the mana.
    /// </summary>
    internal sealed class PercentageOfTotalManaCost : ManaCostBase
    {
        /// <summary>
        /// Initializes a new instance of the PercentageOfTotalManaCost class.
        /// </summary>
        /// <param name="percentage">
        /// The percentage the new PercentageOfTotalManaCost encapsulates.
        /// </param>
        public PercentageOfTotalManaCost( float percentage )
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
            return (int)(user.MaximumMana * percentage);
        }

        /// <summary>
        /// Stores the percentage encapsulated by this PercentageOfTotalManaCost.
        /// </summary>
        private readonly float percentage;
    }
}