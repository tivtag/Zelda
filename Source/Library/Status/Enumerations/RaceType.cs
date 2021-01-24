// <copyright file="RaceType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.RaceType enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Enumerates the different 'races' in the world.
    /// </summary>
    public enum RaceType
    {
        /// <summary>
        /// A normal humanoide.
        /// </summary>
        Human = 0,

        /// <summary>
        /// A demon-like humanoide.
        /// </summary>
        DemiHuman = 1,

        /// <summary>
        /// Undead enemies usually take high damage from Holy attacks.
        /// </summary>
        Undead = 2,

        /// <summary> 
        /// Normal beasts. 
        /// Usually have very high armor but low health.
        /// </summary>
        Plant = 3,

        /// <summary>
        /// The Fairy race type includes Fairy and Ghost type entities.
        /// </summary>
        Fairy = 4,

        /// <summary> 
        /// Normal beasts.
        /// </summary>
        Beast = 5,

        /// <summary>
        /// Demon-like beasts. Usually holy magic deals increased damage.
        /// </summary>
        DemiBeast = 6,

        /// <summary>
        /// Slime-like enemis usually do nature damage.
        /// </summary>
        Slime = 7,

        /// <summary>
        /// Machine type monster usually can't get poisoned.
        /// </summary>
        Machine = 8,

        /// <summary>
        /// Demon-type monsters.
        /// </summary>
        Demon
    }
}
