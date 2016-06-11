// <copyright file="UniquePropertyAttribute.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.UniquePropertyAttribute class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties
{
    using System;

    /// <summary>
    /// When applied on an <see cref="IProperty"/> states that a PropertyList
    /// can only contain one of those IProperties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface )]
    public sealed class UniquePropertyAttribute : Attribute
    {
    }
}
