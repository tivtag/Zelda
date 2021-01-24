// <copyright file="TimedForEachTargetAttackLimiter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Limiter.TimedForEachTargetAttackLimiter class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Limiter
{
    using System.Collections.Generic;
    using Zelda.Status;

    /// <summary>
    /// Represents an <see cref="IAttackLimiter"/> that limits attacks for a specific amount of time
    /// for each indiviudual attack target.
    /// </summary>
    public class TimedForEachTargetAttackLimiter : IAttackLimiter
    {
        /// <summary>
        /// Gets or sets the time in seconds attacks are not allowed for
        /// on a specific target.
        /// </summary>
        public float AttackDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether attacking in general is allowed.
        /// </summary>
        /// <value>
        /// true if it is allowed;
        /// otherwise false.
        /// </value>
        public virtual bool IsAllowed
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
        public bool IsAllowedOn( Statable target )
        {
            int index = this.limitedTargets.FindIndex( x => x.Target == target );

            if( index == -1 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Called when an attack that might be limited by this TimedForEachTargetAttackLimiter
        /// has been used.
        /// </summary>
        public void OnAttackFired()
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
            this.limitedTargets.Add( new LimitedTarget( target, this.AttackDelay ) );
        }

        /// <summary>
        /// Resets this TimedForEachTargetAttackLimiter to its initial state.
        /// </summary>
        public void Reset()
        {
            this.limitedTargets.Clear();
        }

        /// <summary>
        /// Returns a clone of this TimedForEachTargetAttackLimiter.
        /// </summary>
        /// <returns>
        /// The cloned IAttackLimiter.
        /// </returns>
        public IAttackLimiter Clone()
        {
            return new TimedForEachTargetAttackLimiter() {
                AttackDelay = this.AttackDelay
            };
        }

        /// <summary>
        /// Updates this TimedForEachTargetAttackLimiter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            float frameTime = updateContext.FrameTime;

            for( int i = 0; i < this.limitedTargets.Count; ++i )
            {
                LimitedTarget target = this.limitedTargets[i];
                
                target.TimeLeft -= frameTime;
                if( target.TimeLeft <= 0.0f )
                {
                    this.limitedTargets.RemoveAt( i );
                    --i;
                }
                else
                {
                    this.limitedTargets[i] = target;
                }
            }            
        }
        
        /// <summary>
        /// Represents information about a target that is limited by this TimedForEachTargetAttackLimiter.
        /// </summary>
        private struct LimitedTarget
        {
            /// <summary>
            /// Initializes a new instance of the LimitedTarget structure.
            /// </summary>
            /// <param name="target">
            /// The target the attack is limited against.
            /// </param>
            /// <param name="timeLeft">
            /// The time the target is limited for in seconds.
            /// </param>
            public LimitedTarget( Statable target, float timeLeft )
            {
                this.Target = target;
                this.TimeLeft = timeLeft;
            }

            /// <summary>
            /// The target this TimedForEachTargetAttackLimiter is limiting attacks against.
            /// </summary>
            public Statable Target;
        
            /// <summary>
            /// The time left the target is limited for.
            /// </summary>
            public float TimeLeft;
        }

        /// <summary>
        /// The list of currently limited 
        /// </summary>
        private readonly List<LimitedTarget> limitedTargets = new List<LimitedTarget>();
    }
}
