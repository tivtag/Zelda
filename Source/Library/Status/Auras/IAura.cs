// <copyright file="IAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.IAura interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using Atom;

    /// <summary>
    /// Specifies the common interface of all Auras; 
    /// which are compositions of StatusEffects.
    /// </summary>
    public interface IAura : IDescriptionProvider
    {
        /// <summary>
        /// Fired when this Aura has been enabled.
        /// </summary>
        event RelaxedEventHandler<Statable> Enabled;

        /// <summary>
        /// Fired when this Aura has been disabled.
        /// </summary>
        event RelaxedEventHandler<Statable> Disabled;

        /// <summary>        
        /// Updates this <see cref="IAura"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void Update( ZeldaUpdateContext updateContext );
    }
}
