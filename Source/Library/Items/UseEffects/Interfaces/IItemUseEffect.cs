// <copyright file="IItemUseEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.IItemUseEffect interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.UseEffects
{
    using Zelda.Core.Requirements;

    /// <summary>
    /// Represents an effect that is applied when an item is used.
    /// </summary>
    public interface IItemUseEffect : IUseable, IRequirement
    {
    }
}
