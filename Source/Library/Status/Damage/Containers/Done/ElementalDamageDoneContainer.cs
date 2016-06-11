// <copyright file="ElementalDamageDoneContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.ElementalDamageDoneContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage taken from magical attacks of a specific <see cref="ElementalSchool"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ElementalDamageDoneContainer : ElementalSchoolContainer
    {
        /// <summary>
        /// Initializes a new instance of the ElementalDamageDoneContainer class.
        /// </summary>
        internal ElementalDamageDoneContainer()
        {
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected override string GetEffectIdentifier( ElementalSchool school )
        {
            return ElementalDamageDoneEffect.GetIdentifier( school );
        }
    }
}
