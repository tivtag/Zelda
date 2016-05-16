// <copyright file="PropertyListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.Design.PropertyListEditor class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties.Design
{
    using System;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="PropertyList"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PropertyListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyListEditor"/> class.
        /// </summary>
        public PropertyListEditor()
            : base( typeof( PropertyList ) )
        {
        }

        /// <summary>
        /// Gets the type created by this PropertyListEditor.
        /// </summary>
        /// <returns>The data type of the items in the collection.</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof( IProperty );
        }

        /// <summary>
        /// Gets the data types that this collection editor can contain.
        /// </summary>
        /// <returns>
        /// An array of data types that this collection can contain.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return PropertyListEditor.types;
        }

        /// <summary>
        /// The types the PropertyListEditor can create.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( Scene.RedirectZoneResetProperty ),
            typeof( Scene.AlsoResetAnotherZoneOnZoneResetProperty ),
            typeof( Scene.DungeonProperty )
        };
    }
}
