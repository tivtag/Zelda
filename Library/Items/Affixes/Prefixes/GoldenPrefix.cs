// <copyright file="GoldenPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.GoldenPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    /// <summary>
    /// The Golden prefix increasing the possible power difference by 20%.
    /// </summary>
    internal sealed class GoldenPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Golden;
            }
        }

        /// <summary>
        /// Applies this IAffix to an Item.
        /// </summary>
        /// <param name="item">
        /// The Item that gets directly modified by this IAffix.
        /// </param>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        public void Apply( Item item, Item baseItem )
        {
            const float Difference = 0.20f;
            item.PossiblePowerDifference = new Atom.Math.FloatRange( item.PossiblePowerDifference.Minimum + Difference, item.PossiblePowerDifference.Maximum + Difference );

                // Make the item more golden:
            item.MultiplyColor( 0.85f, 0.75f, 0.15f );
        }

        /// <summary>
        /// Gets a value indicating whether this IAffix could
        /// possibly applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The item this IAffix is supposed to be applied to.
        /// </param>
        /// <returns>
        /// True if this IAffix could possible applied to the given <paramref name="baseItem"/>;
        /// otherwise false.
        /// </returns>
        public bool IsApplyable( Item baseItem )
        {
            return baseItem.AllowedToVaryInPower;
        }
    }
}
