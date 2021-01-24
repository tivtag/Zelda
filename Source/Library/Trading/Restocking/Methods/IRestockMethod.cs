// <copyright file="IRestockMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Restocking.IRestockMethod interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Trading.Restocking
{
    using System.ComponentModel;

    /// <summary>
    /// Provides a mechanism that restocks a <see cref="MerchantItem"/>
    /// after a certain event occurred.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IRestockMethod : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Hooks this IRestockMode up with the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The IMerchant this IRestockMode should hook up with.
        /// </param>
        void Hook( MerchantItem merchantItem );

        /// <summary>
        /// Unhooks this IRestockMode.
        /// </summary>
        void Unhook();
    }
}
