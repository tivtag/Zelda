// <copyright file="InitiallyLimitThenRedirectToOtherAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.InitiallyLimitThenRedirectToOtherAttackLimiter class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Limiter
{
    /// <summary>
    /// Defines an IAttackLimiter that initially limits attacks for a specified amount
    /// of time and then redirects calls to another <see cref="IAttackLimiter"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class InitiallyLimitThenRedirectToOtherAttackLimiter : IAttackLimiter
    {
        /// <summary>
        /// Initializes a new instance of the InitiallyLimitThenRedirectToOtherAttackLimiter class.
        /// </summary>
        /// <param name="totalTimeLimited">
        /// The time attacks are limited for initially.
        /// </param>
        /// <param name="otherLimiter">
        /// The IAttackLimiter calls to thew new InitiallyLimitThenRedirectToOtherAttackLimiter are redirected
        /// to after the initial limitation.
        /// </param>
        public InitiallyLimitThenRedirectToOtherAttackLimiter( float totalTimeLimited, IAttackLimiter otherLimiter )
        {
            this.timeLeftLimited = totalTimeLimited;
            this.totalTimeLimited = totalTimeLimited;
            this.otherLimiter = otherLimiter;
        }

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
                return this.IsInitiallyLimited ? false : this.otherLimiter.IsAllowed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this InitiallyLimitThenRedirectToOtherAttackLimiter is in the initial
        /// phase of limiting all attacks.
        /// </summary>
        private bool IsInitiallyLimited
        {
            get
            {
                return this.timeLeftLimited > 0.0f;
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
            if( !this.IsInitiallyLimited )
            {
                return this.otherLimiter.IsAllowedOn( target );
            }

            return true;
        }

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used.
        /// </summary>
        public void OnAttackFired()
        {
            if( !this.IsInitiallyLimited )
            {
                this.otherLimiter.OnAttackFired();
            }
        }

        /// <summary>
        /// Resets this IAttackLimiter to its initial state.
        /// </summary>
        public void Reset()
        {
            this.timeLeftLimited = this.totalTimeLimited;
            this.otherLimiter.Reset();
        }

        /// <summary>
        /// Updates this TimedAttackLimiter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.IsInitiallyLimited )
            {
                this.timeLeftLimited -= updateContext.FrameTime;
            }
            else
            {
                this.otherLimiter.Update( updateContext );
            }
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
        /// Returns a clone of this TimedAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public IAttackLimiter Clone()
        {
            return new InitiallyLimitThenRedirectToOtherAttackLimiter( this.totalTimeLimited, this.otherLimiter.Clone() );
        }

        /// <summary>
        /// The time left this IAttackLimiter initially limits all attacks.
        /// </summary>
        private float timeLeftLimited;

        /// <summary>
        /// The total time this IAttackLimiter initially limits.
        /// </summary>
        private float totalTimeLimited;

        /// <summary>
        /// The other IAttackLimiter this IAttackLimiter redirects to once the initial limitation time has passed.
        /// </summary>
        private readonly IAttackLimiter otherLimiter;
    }
}
