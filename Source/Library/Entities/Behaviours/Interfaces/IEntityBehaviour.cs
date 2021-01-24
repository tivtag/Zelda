// <copyright file="IEntityBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.IEntityBehaviour interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours
{
    using System.ComponentModel;

    /// <summary>
    /// An <see cref="IEntityBehaviour"/> controls how
    /// an <see cref="ZeldaEntity"/> reacts to the world.
    /// </summary>
    /// <remarks>
    /// There are at first Behaviours that do one specialized thing,
    /// and then there are Behaviours that take these specialized Behaviours
    /// and mix them/use them internally.
    /// </remarks>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IEntityBehaviour : Zelda.Saving.ISaveable, IZeldaUpdateable
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IEntityBehaviour"/> is currently active.
        /// </summary>
        bool IsActive
        {
            get;
        }

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> 
        /// If this <see cref="IEntityBehaviour"/> is currently not active.
        /// </exception>
        void Leave();

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Returns a clone of this <see cref="IEntityBehaviour"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">
        /// The owner of the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        IEntityBehaviour Clone( ZeldaEntity newOwner );
    }
}
