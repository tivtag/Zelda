// <copyright file="SwitchableEntitySelectionEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Design.SwitchableEntitySelectionEditor class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Design
{
    using Atom;

    /// <summary>
    /// Represents an <see cref="EntitySelectionEditor"/> that filters
    /// out all entities that don't implement the <see cref="Atom.ISwitchable"/> interface.
    /// </summary>
    public sealed class SwitchableEntitySelectionEditor : EntitySelectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the SwitchableEntitySelectionEditor class.
        /// </summary>
        public SwitchableEntitySelectionEditor()
            : base( entity => entity != null && entity.GetType().Implements( typeof( ISwitchable ) ) )
        {
        }
    }
}
