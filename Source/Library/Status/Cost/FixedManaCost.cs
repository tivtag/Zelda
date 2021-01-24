// <copyright file="FixedManaCost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Cost.FixedManaCost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Cost
{
    /// <summary>
    /// Defines an <see cref="IManaCost"/> that costs a fixed amount of mana. 
    /// </summary>
    internal sealed class FixedManaCost : ManaCostBase
    {
        /// <summary>
        /// Initializes a new instance of the FixedManaCost class.
        /// </summary>
        /// <param name="fixedCost">
        /// The fixed mana cost the new FixedManaCost encapsulates.
        /// </param>
        public FixedManaCost( int fixedCost )
        {
            this.fixedCost = fixedCost;
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
            return this.fixedCost;
        }

        /// <summary>
        /// Stores the fixed mana cost.
        /// </summary>
        private readonly int fixedCost;
    }
}
