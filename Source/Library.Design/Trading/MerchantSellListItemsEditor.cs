// <copyright file="MerchantSellListItemsEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Design.MerchantSellListItemsEditor class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="MerchantSellList"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class MerchantSellListItemsEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantSellListItemsEditor"/> class.
        /// </summary>
        public MerchantSellListItemsEditor()
            : base( typeof( List<MerchantItem> ) )
        {
        }
        
        /// <summary>
        /// Edits the value of the specified object using the specified service provider and context.
        /// </summary>
        /// <param name="context">
        /// An System.ComponentModel.ITypeDescriptorContext that can be used to gain
        /// additional context information.
        /// </param>
        /// <param name="provider">
        /// A service provider object through which editing services can be obtained.
        /// </param>
        /// <param name="value">
        /// The object to edit the value of.
        /// </param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, 
        /// this should return the same object it was passed.
        /// </returns>
        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            this.sellList = (IMerchantSellList)context.Instance;
            return base.EditValue( context, provider, value );
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">
        ///  The type of item to create.
        ///  </param>
        /// <returns>
        /// A new instance of the specified object.
        /// </returns>
        protected override object CreateInstance( Type itemType )
        {
            return new MerchantItem( this.sellList.Merchant );
        }

        /// <summary>
        /// Identifies the IMerchantSellList currently beeing edited.
        /// </summary>
        private IMerchantSellList sellList;
    }
}
