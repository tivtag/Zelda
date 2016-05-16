// <copyright file="IAffixedItemInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.IAffixedItemInstance interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes
{
    /// <summary>
    /// The public shared interface of the ItemInstances that
    /// are pased on <see cref="AffixedItem"/>s.
    /// </summary>
    public interface IAffixedItemInstance
    {
        /// <summary>
        /// Gets the <see cref="AffixedItem"/> the AffixedItemInstance is based on.
        /// </summary>
        AffixedItem AffixedItem
        {
            get;
        }
    }
}
