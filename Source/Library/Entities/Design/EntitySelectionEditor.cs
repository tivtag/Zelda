// <copyright file="EntitySelectionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Design.EntitySelectionEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Design
{
    using System;
    using System.Linq;
    using Atom;

    /// <summary>
    /// Defines an <see cref="System.Drawing.Design.UITypeEditor"/> that
    /// enables one to select an existing entity.
    /// </summary>
    public class EntitySelectionEditor : Atom.Design.BaseItemSelectionEditor<ZeldaEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySelectionEditor"/> class.
        /// </summary>
        public EntitySelectionEditor()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySelectionEditor"/> class.
        /// </summary>
        /// <param name="predicate">
        /// The predicate an entity must fulfill to be included in the selection list.
        /// </param>
        protected EntitySelectionEditor( Func<ZeldaEntity, bool> predicate )
        {
            if( predicate == null )
            {
                this.predicate = entity => true;
            }
            else
            {
                this.predicate = predicate;
            }
        }

        /// <summary>
        /// Gets the items that can be selected in this BaseItemSelectionEditor{TItem}.
        /// </summary>
        /// <returns>
        /// The list of items.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<ZeldaEntity> GetSelectableItems()
        {
            var scene = GlobalServices.GetService<ZeldaScene>();
            return scene.Entities.Where( predicate );
        }

        /// <summary>
        /// The predicate an entity must fulfill to be included in the selection list.
        /// </summary>
        private readonly Func<ZeldaEntity, bool> predicate;
    }
}