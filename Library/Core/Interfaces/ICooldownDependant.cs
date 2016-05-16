// <copyright file="ICooldownDependant.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ICooldownDependant interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda
{
    /// <summary>
    /// Represents an object that is dependant on a <see cref="Cooldown"/>.
    /// </summary>
    public interface ICooldownDependant
    {
        /// <summary>
        /// Refreshes the cooldown of this ICooldownDependant object.
        /// </summary>
        void RefreshCooldown();
    }
}
