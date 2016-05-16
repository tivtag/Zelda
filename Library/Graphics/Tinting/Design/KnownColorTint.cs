// <copyright file="KnownColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.Design.KnownColorTint class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Tinting.Design
{
    using System;

    /// <summary>
    /// Provides access to all known IColorTints.
    /// </summary>
    public static class KnownColorTint
    { 
        /// <summary>
        /// Gets the types of all known IColorTints..
        /// </summary>
        /// <remarks>
        /// The returned array should not be modified.
        /// </remarks>
        public static Type[] Types
        {
            get
            {
                return KnownColorTint.types;
            }
        }

        /// <summary>
        /// The color tints that are known to the design-time system.
        /// </summary>
        private static readonly Type[] types = new Type[7] {
            typeof( CombineColorTint ),
            typeof( ReplacementTint ),
            typeof( LinearColorTint ),
            typeof( LinearColorReplacementTint ),
            typeof( RandomFromListColorTint ),
            typeof( BlendInColorTint ),
            typeof( BlendOutColorTint )
        };
    }
}
