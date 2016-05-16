// <copyright file="AffixType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.AffixType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes
{
    /// <summary>
    /// Enumerates the different types of IAffixes configurations an item might have.
    /// </summary>
    public enum AffixType
    {
        /// <summary>
        /// The item doesn't have any affix.
        /// </summary>
        None = 0,

        /// <summary>
        /// An <see cref="IPrefix"/> stands before the base item.
        /// </summary>
        Prefix = 1,

        /// <summary>
        /// An <see cref="ISuffix"/> stands behind the base item.
        /// </summary>
        Suffix = 2,

        /// <summary>
        /// Both affixes are allowed: An <see cref="IPrefix"/> and an <see cref="ISuffix"/>.
        /// </summary>
        Both =  3,

        /// <summary>
        /// The item must have both affixes: An <see cref="IPrefix"/> and an <see cref="ISuffix"/>.
        /// </summary>
        AlwaysBoth = 4,

        /// <summary>
        /// The item must have one or both affixes: An <see cref="IPrefix"/> and/or an <see cref="ISuffix"/>.
        /// </summary>
        AlwaysOneOrBoth = 5
    }
}
