// <copyright file="FreelyFireNtimesThenTimedAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.FreelyFireNtimesThenTimedAttackLimiter class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Limiter
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// Defins an IAttackLimiter that freely allows attacks to be used
    /// N times before it goes on a cooldown.
    /// </summary>
    public sealed class FreelyFireNtimesThenTimedAttackLimiter : IAttackLimiter
    {
        /// <summary>
        /// Gets a value indicating whether attacks limited by this FreelyFireNtimesThenTimedAttackLimiter can be used freely. 
        /// </summary>
        private bool CanAttackFreely
        {
            get
            {
                return this.timesAttackUsed < this.maximumTimesAttackUsedBeforeCooldown;
            }
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
                if( this.CanAttackFreely )
                    return true;

                return this.cooldown.IsReady;
            }
        }

        /// <summary>
        /// Initializes a new instance of the FreelyFireNtimesThenTimedAttackLimiter class.
        /// </summary>
        /// <param name="maximumTimesAttackCanBeUsedBeforeCooldown">
        /// The number of times the attack can be freely used before it goes on cooldown.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown to apply after the attack has been used.
        /// </param>
        public FreelyFireNtimesThenTimedAttackLimiter( int maximumTimesAttackCanBeUsedBeforeCooldown, Cooldown cooldown )
        {
            this.maximumTimesAttackUsedBeforeCooldown = maximumTimesAttackCanBeUsedBeforeCooldown;
            this.cooldown = cooldown;
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
        public bool IsAllowedOn( Statable target )
        {
            return true;
        }

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used.
        /// </summary>
        public void OnAttackFired()
        {
            if( this.CanAttackFreely )
            {
                this.timesAttackUsed += 1;

                if( !this.CanAttackFreely )
                {
                    this.cooldown.Reset();
                }
            }
        }

        /// <summary>
        /// Called when an attack that might be limited by this IAttackLimiter
        /// has been used on a specific target.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        public void OnAttackHit( Statable target )
        {
        }

        /// <summary>
        /// Resets this FreelyFireNtimesThenTimedAttackLimiter.
        /// </summary>
        public void Reset()
        {
            this.timesAttackUsed = 0;
            this.cooldown.Reset();
        }

        /// <summary>
        /// Returns a clone of this FreelyFireNtimesThenTimedAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public IAttackLimiter Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates this FreelyFireNtimesThenTimedAttackLimiter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.CanAttackFreely )
            {
                this.cooldown.Update( updateContext.FrameTime );

                if( this.cooldown.IsReady )
                {
                    this.timesAttackUsed = 0;
                }
            }
        }

        private int timesAttackUsed;
        private int maximumTimesAttackUsedBeforeCooldown;
        private readonly Cooldown cooldown;
    }
}
