// <copyright file="StatusEffectListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Design.StatusEffectListEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="List&lt;StatusEffect&gt;"/> objects.
    /// This class can't be inherited.
    /// </summary>
    public sealed class StatusEffectListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffectListEditor"/> class.
        /// </summary>
        public StatusEffectListEditor()
            : base( typeof( List<StatusEffect> ) )
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
            return KnownStatusEffect.Types;
        }
    }
}
