// <copyright file="DamageDoneWithSchoolContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageDoneWithSchoolContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage done properties for all <see cref="DamageSchool"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageDoneWithSchoolContainer : DamageSchoolContainer
    {
        /// <summary>
        /// Initializes a new instance of the DamageDoneWithSchoolContainer class.
        /// </summary>
        internal DamageDoneWithSchoolContainer()
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
            return DamageDoneWithSchoolEffect.GetIdentifier( school );
        }
    }
}
