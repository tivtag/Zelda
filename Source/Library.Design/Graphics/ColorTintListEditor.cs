// <copyright file="ColorTintListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.Design.ColorTintListEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="List&lt;IColorTint&gt;"/> objects.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ColorTintListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTintListEditor"/> class.
        /// </summary>
        public ColorTintListEditor()
            : base( typeof( List<IColorTint> ) )
        {
        }

        /// <summary>
        /// Receives the list of types this CollectionEditor can create.
        /// </summary>
        /// <returns>
        /// The list of types the CollectionEditor can create.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return KnownColorTint.Types;
        }
    }
}
