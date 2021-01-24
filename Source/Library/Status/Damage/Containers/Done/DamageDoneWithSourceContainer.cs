// <copyright file="DamageDoneWithSourceContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageDoneWithSourceContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage done properties for all <see cref="DamageSource"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageDoneWithSourceContainer : DamageSourceContainer
    {
        /// <summary>
        /// Initializes a new instance of the DamageDoneWithSourceContainer class.
        /// </summary>
        internal DamageDoneWithSourceContainer()
        {
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given DamageSource.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected override string GetEffectIdentifier( DamageSource school )
        {
            return DamageDoneWithSourceEffect.GetIdentifier( school );
        }
    }
}
