// <copyright file="ColorTintEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.Design.ColorTintEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting.Design
{
    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism
    /// that allows the user to create <see cref="IColorTint"/> instances.
    /// </summary>
    internal sealed class ColorTintEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this ColorTintEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<System.Type> GetTypes()
        {
            return KnownColorTint.Types;
        }
    }
}
