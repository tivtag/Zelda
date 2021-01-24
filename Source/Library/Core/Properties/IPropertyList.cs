// <copyright file="IPropertyList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.IPropertyList interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Core.Properties
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of IProperties.
    /// </summary>
    public interface IPropertyList : IList<IProperty>, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Tries to get the IProperty of the specified type.
        /// </summary>
        /// <typeparam name="TProperty">
        /// The exact type of the property to get.
        /// </typeparam>
        /// <returns>
        /// The requested property; or null.
        /// </returns>
        TProperty TryGet<TProperty>()
            where TProperty : class, IProperty;
    }
}
