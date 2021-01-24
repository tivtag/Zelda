// <copyright file="DamageTakenFromSourceContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageTakenFromSourceContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage taken properties that relate to a <see cref="DamageSource"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageTakenFromSourceContainer : DamageSourceContainer
    {
        /// <summary>
        /// Initializes a new instance of the DamageTakenFromSourceContainer class.
        /// </summary>
        internal DamageTakenFromSourceContainer()
        {
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given DamageSource.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected override string GetEffectIdentifier( DamageSource source )
        {
            return DamageTakenFromSourceEffect.GetIdentifier( source );
        }
    }
}
