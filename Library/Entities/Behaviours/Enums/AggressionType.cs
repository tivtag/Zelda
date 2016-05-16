// <copyright file="AggressionType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.AggressionType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    /// <summary>
    /// Enumerates the different aggression types of an entity. 
    /// </summary>
    public enum AggressionType
    {
        /// <summary>
        /// No specific aggression type.
        /// </summary>
        None,

        /// <summary>
        /// The entity is aggressive (towards the player).
        /// </summary>
        AlwaysAggressive,

        /// <summary>
        /// The entity is aggressive (towards the player).
        /// </summary>
        Aggressive,

        /// <summary>
        /// The entity is not aggressive.
        /// </summary>
        Neutral
    }
}
