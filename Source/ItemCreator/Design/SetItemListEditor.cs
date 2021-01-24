// <copyright file="SetItemListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.Design.SetItemListEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.ItemCreator.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using Zelda.Items.Sets;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="MerchantSellList"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SetItemListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetItemListEditor"/> class.
        /// </summary>
        public SetItemListEditor()
            : base( typeof( List<ISetItem> ) )
        {
        }
        
        /// <summary>
        /// Gets the data types that this collection editor can contain.
        /// </summary>
        /// <returns></returns>
        protected override Type[] CreateNewItemTypes()
        {
            return SetItemListEditor.types;
        }

        /// <summary>
        /// The types the list can contain.
        /// </summary>
        private static readonly Type[] types = new Type[1]{
            typeof( SetItem )
        };
    }
}
