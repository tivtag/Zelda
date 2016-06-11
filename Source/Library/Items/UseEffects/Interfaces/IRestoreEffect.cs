// <copyright file="IRestoreEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.IRestoreEffect interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.UseEffects
{
    using Zelda.Status;

    /// <summary>
    /// Represents the base interface shared by all power restoring
    /// IUseEffects.
    /// </summary>
    public interface IRestoreEffect : IItemUseEffect
    {
        /// <summary>
        /// Gets the average amount restored of the given power type.
        /// </summary>
        /// <param name="powerType">
        /// The power type.
        /// </param>
        /// <param name="user">
        /// The statable component of the Entity that wants to use this IRestoreEffect.
        /// </param>
        /// <returns>
        /// The average amount restored.
        /// </returns>
        int GetAverageAmountRestored( LifeMana powerType, Statable user );
    }
}
