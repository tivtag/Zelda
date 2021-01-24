// <copyright file="ISetBonus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.ISetBonus interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Sets
{
    using System.ComponentModel;

    /// <summary>
    /// Represents the bonus that is given when all <see cref="ISetItem"/>s
    /// of an <see cref="ISet"/> are equipped together.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface ISetBonus : Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Gets a value indicating whether this ISetBonus is currently applied.
        /// </summary>
        [Browsable(false)]
        bool IsApplied
        {
            get;
        }

        /// <summary>
        /// Applies this ISetBonus to the specified ExtendedStatable component.
        /// </summary>
        /// <param name="statable">
        /// The statable component the bonus should be enabled for.
        /// </param>
        void Enable( Zelda.Status.ExtendedStatable statable );

        /// <summary>
        /// Removes this ISetBonus from the ExtendedStatable 
        /// it was previously enabled for.
        /// </summary>
        void Disable();
    }
}
