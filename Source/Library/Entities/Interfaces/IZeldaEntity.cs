// <copyright file="IZeldaEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.IZeldaEntity interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom;
    using Atom.Scene;
    using Zelda.Entities.Components;

    /// <summary>
    /// Defines the base interface the <see cref="ZeldaEntity"/> class
    /// implements.
    /// </summary>
    public interface IZeldaEntity : ISceneEntity, IReadOnlyNameable, IZeldaFloorDrawable, Zelda.Saving.ISavedState
    {
        /// <summary>
        /// Fired when the FloorNumber of this IZeldaEntity has changed.
        /// </summary>
        event RelaxedEventHandler<ChangedValue<int>> FloorNumberChanged;

        /// <summary>
        /// Fired when this IZeldaEntity has been added to a <see cref="ZeldaScene"/>.
        /// </summary>
        event RelaxedEventHandler<ZeldaScene> Added;

        /// <summary>
        /// Fired when this IZeldaEntity has been removed from a <see cref="ZeldaScene"/>.
        /// </summary>
        event RelaxedEventHandler<ZeldaScene> Removed;

        /// <summary>
        /// Gets the ZeldaScene this IZeldaEntity is part of.
        /// </summary>
        new ZeldaScene Scene
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="ZeldaTransform"/> component of this IZeldaEntity.
        /// </summary>
        ZeldaTransform Transform
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="ZeldaCollision"/> component of this IZeldaEntity.
        /// </summary>
        ZeldaCollision Collision
        {
            get;
        }
    }
}
