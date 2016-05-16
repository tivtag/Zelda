// <copyright file="EntityFloorRelativity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityFloorRelativity enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    /// <summary>
    /// States how an Entity is handled relative to the other Entity on the same Floor.
    /// </summary>
    public enum EntityFloorRelativity
    {
        /// <summary>
        /// The entity is below all other Entities.
        /// Usually used for graphical StatusEffects that are part of the map
        /// and must be drawn below entities.
        /// </summary>
        IsStrongBelow = 0,

        /// <summary>
        /// Represents the layer above IsStrongBelow and below Normal, IsAbove, IsStrongAbove.
        /// Usually used for 'real' objects that are below normal entities such as Items.
        /// </summary>
        IsBelow,

        /// <summary>
        /// Represents the layer above IsStrongBelow, IsBelow and below IsAbove, IsStrongAbove.
        /// Used for normal Entities. This is the default setting.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents the layer above IsStrongBelow, IsBelow, Normal and below IsStrongAbove.
        /// Used for overlay objects.
        /// </summary>
        IsAbove,

        /// <summary>
        /// Represents the layer above IsStrongBelow, IsBelow, Normal, IsStrongAbove.
        /// Used for overlay objects.
        /// </summary>
        IsStrongAbove
    }
}
