// <copyright file="UniqueNameHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.UniqueNameHelper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Atom;

    /// <summary>
    /// Implements mechanism for creating unique names.
    /// </summary>
    public static class UniqueNameHelper
    {
        /// <summary>
        /// Gets an unique name that starts with the specified <paramref name="baseName"/> and ends with a number.
        /// </summary>
        /// <param name="baseName">
        /// The base name.
        /// </param>
        /// <param name="existingItems">
        /// The already existing items.
        /// </param>
        /// <returns>
        /// The new unique name that has been created.
        /// </returns>
        public static string Get( string baseName, IEnumerable<INameable> existingItems )
        {
            int nr = -1;
            string name;

            do
            {
                ++nr;
                name = baseName + nr.ToString( CultureInfo.InvariantCulture );
            }
            while( existingItems.Any( item => name.Equals( item.Name, StringComparison.Ordinal ) ) );

            return name;
        }
    }
}
