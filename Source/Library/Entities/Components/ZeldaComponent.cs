// <copyright file="ZeldaComponent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.ZeldaComponent class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Components
{
    using Atom.Components;

    /// <summary>
    /// Represents a <see cref="Component"/> in a <see cref="ZeldaEntity"/>.
    /// </summary>
    /// <remarks>
    /// Not all components must inherit from this class.
    /// </remarks>
    public class ZeldaComponent : Component, IEntityOwned
    {
        /// <summary>
        /// Gets the <see cref="ZeldaEntity"/> that owns this ZeldaComponent.
        /// </summary>
        public new ZeldaEntity Owner
        {
            get 
            {
                return this.owner; 
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> that owns the <see cref="ZeldaEntity"/> of this ZeldaComponent.
        /// </summary>
        public ZeldaScene Scene
        {
            get 
            {
                return this.owner.Scene;
            }
        }

        /// <summary>
        /// Called when this ZeldaComponent has been successfully attached to an Atom.Components.IEntity.
        /// </summary>
        public override void Initialize()
        {
            this.owner = (ZeldaEntity)base.Owner;
        }

        /// <summary>
        /// Identifies the ZeldaEntity that owns this ZeldaComponent.
        /// </summary>
        private ZeldaEntity owner;
    }
}
