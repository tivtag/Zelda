// <copyright file="IStatusSetBonus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.IStatusSetBonus interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Zelda.Status;
    
    /// <summary>
    /// Represents an <see cref="ISetBonus"/> that provides an arabitary list
    /// of <see cref="StatusEffect"/>s.
    /// </summary>
    public interface IStatusSetBonus : ISetBonus
    {
        /// <summary>
        /// Gets the list of <see cref="StatusEffect"/> this IStatusSetBonus provides.
        /// </summary>
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        IList<StatusEffect> Effects
        {
            get;
        }
    }
}
