// <copyright file="IAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.IAttackLimiter interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Limiter
{
    using Zelda.Status;

    /// <summary>
    /// Provides a mechanism that limits the usage of an <see cref="Attack"/>.
    /// </summary>
    public interface IAttackLimiter : IZeldaUpdateable
    {    
        /// <summary>
        /// Gets a value indicating whether attacking in general is allowed.
        /// </summary>
        /// <value>
        /// true if it is allowed;
        /// otherwise false.
        /// </value>
        bool IsAllowed
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether attacking the specified <paramref name="target"/>
        /// is allowed.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <returns>
        /// true if it is allowed;
        /// otherwise false.
        /// </returns>
        bool IsAllowedOn( Statable target );

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used.
        /// </summary>
        void OnAttackFired();
        
        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used on a specific target.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        void OnAttackHit( Statable target );        

        /// <summary>
        /// Resets this IAttackLimiter to its initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Returns a clone of this IAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        IAttackLimiter Clone();
    }
}
