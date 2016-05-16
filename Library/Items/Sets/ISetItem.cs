// <copyright file="ISetItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.ISetItem interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets
{
    using System.ComponentModel;
    using Atom;

    /// <summary>
    /// Represents an item that is part of an <see cref="ISet"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface ISetItem : IReadOnlyNameable, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Gets the localized name of this ISetItem.
        /// </summary>
        string LocalizedName
        {
            get;
        }
    }
}
