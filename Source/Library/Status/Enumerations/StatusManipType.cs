// <copyright file="StatusManipType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.StatusManipType enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// The type of a <see cref="StatusValueEffect"/>. 
    /// </summary>
    /// <remarks>
    /// (base value+additive effect+additive effect+...) * (multiplicative effect+multiplicative effect+...)
    /// </remarks>
    public enum StatusManipType
    {
        /// <summary>
        /// A fixed status value.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// A percentual increase of the total <see cref="Fixed"/> value.
        /// </summary>
        Percental = 1,

        /// <summary>
        /// A rating value is converted into a <see cref="Fixed"/> value by
        /// scaling it up or down related to the <see cref="Statable.Level"/>.
        /// </summary>
        /// <seealso cref="StatusCalc"/>
        Rating = 2
    }
}
