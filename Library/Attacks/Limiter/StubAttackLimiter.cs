// <copyright file="StubAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.StubAttackLimiter class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Limiter
{
    /// <summary>
    /// Implemetns an <see cref="IAttackLimiter"/> that does nothing and allows 
    /// all attacks to be fired.
    /// </summary>
    public sealed class StubAttackLimiter : IAttackLimiter
    {
        /// <summary>
        /// A cached instance of this StubAttackLimiter that is meant for re-use.
        /// </summary>
        public static readonly StubAttackLimiter Instance = new StubAttackLimiter();

        /// <summary>
        /// Gets a value indicating whether attacking in general is allowed.
        /// </summary>
        /// <value>
        /// true if it is allowed;
        /// otherwise false.
        /// </value>
        public bool IsAllowed
        {
            get 
            {
                return true;
            }
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
        public bool IsAllowedOn( Zelda.Status.Statable target )
        {
            return true;
        }

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used.
        /// </summary>
        public void OnAttackFired()
        {      
        }

        /// <summary>
        /// Resets this IAttackLimiter to its initial state.
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Updates this IAttackLimiter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used on a specific target.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        public void OnAttackHit( Zelda.Status.Statable target )
        {
        }

        /// <summary>
        /// Returns a clone of this StubAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public IAttackLimiter Clone()
        {
            return this;
        }
    }
}
