// <copyright file="PooledDamageOverTimeAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Pooling.PooledDamageOverTimeAura class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Pooling
{
    using Atom.Collections.Pooling;
    using Zelda.Status.Auras;
    
    /// <summary>
    /// Defines a wrapper around <see cref="TimedAura"/> that can be pooled within
    /// an <see cref="AuraPool{PooledTimedAura}"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PooledDamageOverTimeAura : PooledObjectWrapper<DamageOverTimeAura>
    {
        /// <summary>
        /// Initializes a new instance of the PooledDamageOverTimeAura class.
        /// </summary>
        /// <param name="aura">
        /// The DamageOverTimeAura that actually gets pooled.
        /// </param>
        public PooledDamageOverTimeAura( DamageOverTimeAura aura )
            : base( aura )
        {
        }
    }
}
