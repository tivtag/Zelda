// <copyright file="ISet.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.ISet interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Sets
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom;

    /// <summary>
    /// Represents a set of <see cref="ISetItem"/>s that when equipped
    /// together provide an <see cref="ISetBonus"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface ISet : IReadOnlyNameable, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Gets the localized name of this ISet.
        /// </summary>
        string LocalizedName
        {
            get;
        }

        /// <summary>
        /// Gets the list of <see cref="ISetItem"/> that are part of this ISet.
        /// </summary>
        IList<ISetItem> Items
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="ISetBonus"/> this ISet provides when all Items of this ISet
        /// are equipped.
        /// </summary>
        ISetBonus Bonus
        {
            get;
        }
    }
}
