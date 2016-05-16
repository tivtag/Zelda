// <copyright file="IProperty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.IProperty interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Core.Properties
{
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Represents an arbitary property.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IProperty : ISaveable
    {
    }
}
