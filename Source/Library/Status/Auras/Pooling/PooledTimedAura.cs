// <copyright file="PooledTimedAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Pooling.PooledTimedAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Pooling
{
    using Atom.Collections.Pooling;
    
    /// <summary>
    /// Defines a wrapper around <see cref="TimedAura"/> that can be pooled within
    /// an <see cref="AuraPool{PooledTimedAura}"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PooledTimedAura : PooledObjectWrapper<TimedAura>
    {
        /// <summary>
        /// Initializes a new instance of the PooledTimedAura class.
        /// </summary>
        public PooledTimedAura()
            : base( new TimedAura() )
        {
        }
    }
}
