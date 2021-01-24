// <copyright file="DamageTakenFromSchoolContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageTakenFromSchoolContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage taken properties for all <see cref="DamageSchool"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageTakenFromSchoolContainer : DamageSchoolContainer
    {
        /// <summary>
        /// Initializes a new instance of the DamageTakenFromSchoolContainer class.
        /// </summary>
        internal DamageTakenFromSchoolContainer()
        {
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given DamageSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected override string GetEffectIdentifier( DamageSchool school )
        {
            return DamageTakenFromSchoolEffect.GetIdentifier( school );
        }
    }
}
