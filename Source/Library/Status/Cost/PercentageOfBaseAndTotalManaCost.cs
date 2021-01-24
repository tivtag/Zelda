// <copyright file="PercentageOfBaseAndTotalManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.PercentageOfBaseAndTotalManaCost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Defines an <see cref="IManaCost"/> that costs a percentage of base mana
    /// and a percentage of total mana added together.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class PercentageOfBaseAndTotalManaCost : ManaCostBase
    {
        /// <summary>
        /// Initializes a new instance of the PercentageOfBaseAndTotalManaCost class.
        /// </summary>
        /// <param name="ofBasePercentage">
        /// The percentage of base mana the new PercentageOfBaseAndTotalManaCost encapsulates.
        /// </param>
        /// <param name="ofTotalPercentage">
        /// The percentage of total mana the new PercentageOfBaseAndTotalManaCost encapsulates.
        /// </param>
        public PercentageOfBaseAndTotalManaCost( float ofBasePercentage, float ofTotalPercentage )
        {
            this.ofBaseManaCost = new PercentageOfBaseManaCost( ofBasePercentage );
            this.ofTotalManaCost = new PercentageOfTotalManaCost( ofTotalPercentage );
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
        public override int Get( Zelda.Status.Statable user )
        {
            return this.ofBaseManaCost.Get( user ) + this.ofTotalManaCost.Get( user );
        }

        /// <summary>
        /// The mana cost of base mana that is part of this PercentageOfBaseAndTotalManaCost.
        /// </summary>
        private readonly PercentageOfBaseManaCost ofBaseManaCost;

        /// <summary>
        /// The mana cost of total mana that is part of this PercentageOfBaseAndTotalManaCost.
        /// </summary>
        private readonly PercentageOfTotalManaCost ofTotalManaCost;
    }
}