// <copyright file="IRestocker.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.IRestocker interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Trading.Restocking
{
    using System.ComponentModel;
    
    /// <summary>
    /// Provides a merchism to restock a <see cref="MerchantItem"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IRestocker : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Restocks the given MerchantItem.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem to restock.
        /// </param>
        void Restock( MerchantItem item );
    }
}
