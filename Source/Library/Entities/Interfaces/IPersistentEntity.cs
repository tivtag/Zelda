// <copyright file="IPersistentEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IPersistentEntity interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Defines an object that gets saved to the <see cref="Zelda.Saving.SaveFile"/>.
    /// </summary>
    public interface IPersistentEntity
    {
        /// <summary>
        /// Gets the (unique) name of this <see cref="IPersistentEntity"/>.
        /// </summary>
        string Name
        { 
            get;
        }
    }
}
