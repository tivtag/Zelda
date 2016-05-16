// <copyright file="MeleeAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Melee.MeleeAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks
{
    using System;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.Status;

    /// <summary>
    /// Represents an Attack that applies damage
    /// with direct contact in a specific area.
    /// </summary>
    public class MeleeAttack : Attack
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Attack"/> is pushing hit enemies away.
        /// </summary>
        public bool IsPushing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum pushing power of this <see cref="Attack"/>.
        /// </summary>
        public float PushingPowerMinimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum pushing power of this <see cref="Attack"/>.
        /// </summary>
        public float PushingPowerMaximum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAttackHitEffect"/> applied by this IAttackHitEffect.
        /// </summary>
        public IAttackHitEffect HitEffect
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MeleeAttack"/> class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new MeleeAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new MeleeAttack does. 
        /// </param>
        public MeleeAttack( ZeldaEntity owner, AttackDamageMethod method )
            : base( owner, method )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Fires the attack at the given <paramref name="target"/>,
        /// if possible.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <returns>
        /// true if the Attack was executed, otherwise false.
        /// </returns>
        public override bool Fire( Attackable target )
        {
            if( target != null && this.ShouldFireAgainst( target ) )
            {
                this.AttackTarget( target );

                this.OnFiredAgainst( target );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attacks the specified target attackable entity.
        /// </summary>
        /// <param name="target">
        /// The target of this attack.
        /// </param>
        private void AttackTarget( Attackable target )
        {
            AttackDamageResult result = new AttackDamageResult();
            var statable = target.Statable;

            if( statable != null )
            {
                if( ShouldFireAttackAt( statable ) )
                {
                    // Calculate damage
                    if( this.DamageMethod != null )
                    {
                        this.DamageMethod.NotifyCallerChanged( this );
                        result = this.DamageMethod.GetDamageDone( this.Statable, statable );

                        // Inflict damage
                        target.Attack( this, result );
                    }
                }
                else
                {
                    return;
                }

                if( this.HitEffect != null )
                {
                    this.ApplyHitEffect( statable, result );
                }
            }
            else
            {
                target.Attack( this, result );
            }

            if( this.IsPushing )
            {
                this.ApplyPushing( target, result );
            }
        }

        /// <summary>
        /// Applies the pushing force of this MeleeAttack.
        /// </summary>
        /// <param name="target">
        /// The target that has been attacked.
        /// </param>
        /// <param name="result">
        /// The result of the attack.
        /// </param>
        private void ApplyPushing( Attackable target, AttackDamageResult result )
        {
            // Don't allow pushing to happen if the attack was dodged or missed.
            // Parried attacks still will push back tho.
            if( result.AttackReceiveType == AttackReceiveType.Dodge || 
                result.AttackReceiveType == AttackReceiveType.Miss )
                return;

            var targetMoveable = target.Owner.Components.Get<Moveable>();

            if( targetMoveable != null )
            {
                float power = this.ServiceProvider.Rand.RandomRange( PushingPowerMinimum, PushingPowerMaximum );

                // Push the target into the direction the owner was looking
                targetMoveable.Push( power, this.Owner.Transform.Direction );
            }
        }

        /// <summary>
        /// Applies the <see cref="HitEffect"/> of this MeleeAttack.
        /// </summary>
        /// <param name="target">
        /// The target that has been attacked.
        /// </param>
        /// <param name="result">
        /// The result of the attack.
        /// </param>
        private void ApplyHitEffect( Statable target, AttackDamageResult result )
        {
            if( this.HitEffect != null )
            {
                if( result.AttackReceiveType == AttackReceiveType.Resisted ||
                    result.AttackReceiveType == AttackReceiveType.Dodge ||
                    result.AttackReceiveType == AttackReceiveType.Parry ||
                    result.AttackReceiveType == AttackReceiveType.Miss )
                {
                    return;
                }

                this.HitEffect.OnHit( this.Statable, target );
            }
        }

        /// <summary>
        /// Gets a value indicating whether this MeleeAttack
        /// should be fired at the given target.
        /// </summary>
        /// <param name="target">
        /// The target of the attack. May not be null.
        /// </param>
        /// <returns>
        /// Returns true if the attack should be executed;
        /// otherwise false.
        /// </returns>
        private static bool ShouldFireAttackAt( Statable target )
        {
            return !target.IsInvincible && !target.IsDead;
        }

        /// <summary>
        /// Setups the specified MeleeAttack to be a clone of this MeleeAttack.
        /// </summary>
        /// <param name="clone">
        /// The MeleeAttack to setup as a clone of this MeleeAttack.
        /// </param>
        public void SetupClone( MeleeAttack clone )
        {
            clone.HitEffect = this.HitEffect;
            clone.DamageMethod = this.DamageMethod;
            clone.Limiter = this.Limiter.Clone();

            clone.IsPushing = this.IsPushing;
            clone.PushingPowerMinimum = this.PushingPowerMinimum;
            clone.PushingPowerMaximum = this.PushingPowerMaximum;
        }

        #endregion
    }
}