// <copyright file="IRestockRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.IRestockRequirement interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Trading.Restocking
{
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Provides a mechanism for receiving a value that indicates whether a MerchantItem
    /// is allowed to be restocked.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IRestockRequirement : ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether this IRestockRequirement has been fulfilled.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem that is about to be restock.
        /// </param>
        /// <returns>
        /// true if the restocking process is allowed to continue;
        /// otherwise false.
        /// </returns>
        bool IsFulfilled( MerchantItem item );
    }
}
