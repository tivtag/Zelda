// <copyright file="INotifyKilledEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.INotifyKilledEntity interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities
{
    /// <summary>
    /// Provides a mechanism to notify the object
    /// that it managed to kill a <see cref="Components.Killable"/> <see cref="ZeldaEntity"/>.
    /// </summary>
    public interface INotifyKilledEntity
    {
        /// <summary>
        /// Fired when this object has managed to kill the specified <see cref="Zelda.Entities.ZeldaEntity"/>.
        /// </summary>
        event Atom.RelaxedEventHandler<Zelda.Entities.ZeldaEntity> EntityKilled;

        /// <summary>
        /// Notifies this object that it managed to kill a <see cref="Components.Killable"/> <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="killable">
        /// The Components.Killable component of the ZeldaEntity that has been killed.
        /// </param>
        void NotifyKilled( Components.Killable killable );
    }
}
