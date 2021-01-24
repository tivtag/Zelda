// <copyright file="TimedAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.TimedAttackLimiter class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Limiter
{
    using System;
    using Atom.Diagnostics.Contracts;

    /// <summary>
    /// Limits the usage of an Attack by not allowing the Attack to go
    /// off for a fixed amount of time after it fired.
    /// </summary>
    public class TimedAttackLimiter : IAttackLimiter
    {
        /// <summary>
        /// Initializes a new instance of the TimedAttackLimiter class.
        /// </summary>
        /// <param name="cooldown">
        /// The cooldown of the attack.
        /// </param>
        public TimedAttackLimiter( Cooldown cooldown )
        {
            Contract.Requires<ArgumentNullException>( cooldown != null );

            this.cooldown = cooldown;
        }

        /// <summary>
        /// Initializes a new instance of the TimedAttackLimiter class.
        /// </summary>
        /// <param name="cooldown">
        /// The cooldown of the attack.
        /// </param>
        public TimedAttackLimiter( float cooldown )
        {
            this.cooldown = new Cooldown( cooldown );
        }

        /// <summary>
        /// Initializes a new instance of the TimedAttackLimiter class.
        /// </summary>
        public TimedAttackLimiter()
        {
            this.cooldown = new Cooldown();
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
                return this.cooldown.IsReady;
            }
        }

        /// <summary>
        /// Gets or sets the time until the Attack can be used again.
        /// </summary>
        public float AttackTick
        {
            get 
            {
                return this.cooldown.TimeLeft;
            }

            set 
            {
                this.cooldown.TimeLeft = value; 
            }
        }

        /// <summary>
        /// Gets or sets the time it takes until the Attack can go off again.
        /// </summary>
        public virtual float AttackDelay
        {
            get
            {
                return cooldown.TotalTime;
            }

            set
            {
                this.cooldown.TotalTime = value;
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
            this.cooldown.TimeLeft = this.AttackDelay;
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
        /// Resets this IAttackLimiter to its initial state.
        /// </summary>
        public void Reset()
        {
            this.cooldown.TimeLeft = 0.0f;
        }

        /// <summary>
        /// Updates this TimedAttackLimiter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.cooldown.Update( updateContext.FrameTime );
        }
        
        /// <summary>
        /// Returns a clone of this TimedAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public virtual IAttackLimiter Clone()
        {
            return new TimedAttackLimiter( cooldown.Clone() );
        }

        /// <summary>
        /// The cooldown of the Attack.
        /// </summary>
        private readonly Cooldown cooldown;
    }
}
