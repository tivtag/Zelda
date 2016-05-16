// <copyright file="LocalizedDescriptionAttribute.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Editor.LocalizedDescriptionAttribute class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor
{
    using System.ComponentModel;

    /// <summary>
    /// Defines a localized DescriptionAttribute
    /// that uses the <see cref="Zelda.Editor.Properties.Resources"/> resource manager
    /// to lock-up resource information. This is a sealed class.
    /// </summary>
    internal sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="resourceName">The name of the string resource that is locked-up.</param>
        public LocalizedDescriptionAttribute( string resourceName )
            : base( Properties.Resources.ResourceManager.GetString( resourceName ) )
        {
        }
    }
}
