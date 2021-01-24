// <copyright file="Attackable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.AttackEventArgs structure and
//     the Zelda.Entities.Components.Attackable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using Atom;
    using System.Diagnostics;
    using Zelda.Attacks;
using Zelda.Status;

    /// <summary>
    /// Represents the data of the <see cref="Attackable.Attacked"/> event.
    /// </summary>
    public struct AttackEventArgs
    {
        /// <summary>
        /// Identifies the entity that has attacked.
        /// </summary>
        public readonly ZeldaEntity Attacker;

        /// <summary>
        /// Identifies the entity that has been hit by the attack.
        /// </summary>
        public readonly ZeldaEntity Target;

        /// <summary>
        /// Identifies the statable component of the entity that has been hit by the attack.
        /// </summary>
        public readonly Statable TargetStatable;

        /// <summary>
        /// Identifies the attack that has been used.
        /// </summary>
        public readonly Attack Attack;

        /// <summary>
        /// Stores the damage results of the attack.
        /// </summary>
        public readonly AttackDamageResult DamageResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackEventArgs"/> structure.
        /// </summary>
        /// <param name="attacker">
        /// The entity that has attacked.
        /// </param>
        /// <param name="target">
        /// The entity that has been hit by the attack.
        /// </param>
        /// <param name="attack">
        /// The attack that has been used.
        /// </param>
        /// <param name="damageResult">
        /// The damage results of the attack.
        /// </param>
        public AttackEventArgs(
            ZeldaEntity attacker,
            ZeldaEntity target,
            Statable targetStatable,
            Attack attack,
            AttackDamageResult damageResult )
        {
            this.Attacker = attacker;
            this.Target = target;
            this.TargetStatable = targetStatable;
            this.Attack = attack;
            this.DamageResult = damageResult;
        }
    }

    /// <summary>
    /// Defines a Component that makes its owning <see cref="ZeldaEntity"/>
    /// able to attack and be attacked. This class can't be inherited.
    /// </summary>
    public sealed class Attackable : ZeldaComponent
    {
        /// <summary>
        /// Fires when the attackable <see cref="ZeldaEntity"/> has been attacked.
        /// </summary>
        public event RelaxedEventHandler<AttackEventArgs> Attacked;

        /// <summary>
        /// Fires when the attackable <see cref="ZeldaEntity"/> has used an Attack and hit a target.
        /// </summary>
        public event RelaxedEventHandler<AttackEventArgs> AttackHit;

        /// <summary>
        /// Fires when the attackable <see cref="ZeldaEntity"/> has just started using an Attack.
        /// </summary>
        public event RelaxedEventHandler<Attack> AttackFiring;

        /// <summary>
        /// Fires when the attackable <see cref="ZeldaEntity"/> has used an Attack.
        /// </summary>
        public event RelaxedEventHandler<Attack> AttackFired;

        /// <summary>
        /// Gets the <see cref="Status.Statable"/> component of the <see cref="ZeldaEntity"/>
        /// that owns this <see cref="Attackable"/> component. Can be null.
        /// </summary>
        public Status.Statable Statable
        {
            get
            {
                return this.statable;
            }
        }

        /// <summary>
        /// Called when an IComponent has been removed or added to the IEntity that owns this IComponent.
        /// </summary>
        public override void InitializeBindings()
        {
            this.statable = this.Owner.Components.Find<Status.Statable>();
        }

        /// <summary>
        /// Called when this attackable ZeldaEntity has just started using an attack.
        /// </summary>
        /// <param name="attack">
        /// The attack that was used.
        /// </param>
        public void NotifyFiring( Attack attack )
        {
            this.AttackFiring.Raise( this, attack );
        }

        /// <summary>
        /// Called when this attackable ZeldaEntity has used an attack.
        /// </summary>
        /// <param name="attack">
        /// The attack that was used.
        /// </param>
        public void NotifyFired( Attack attack )
        {
            this.AttackFired.Raise( this, attack );
        }

        /// <summary>
        /// Called when this attackable ZeldaEntity has attacked another ZeldaEntity.
        /// </summary>
        /// <param name="attackEventArgs">
        /// The AttackEventArgs.
        /// </param>
        private void NotifyHit( AttackEventArgs attackEventArgs )
        {
            this.AttackHit.Raise( this, attackEventArgs );
        }

        /// <summary>
        /// Attacks this attackable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="usedAttack">
        /// The attack that has been used against the attackable ZeldaEntity.
        /// </param>
        /// <param name="damageResult">
        /// The result of the attack.
        /// </param>
        public void Attack( Attack usedAttack, AttackDamageResult damageResult )
        {
            Debug.Assert( usedAttack != null );
            Debug.Assert( usedAttack.Owner != null );

            if( this.statable != null )
                this.statable.LoseLife( damageResult );

            // Notify.
            var eventArgs = new AttackEventArgs(
                usedAttack.Owner, // Source
                this.Owner,       // Target
                this.statable,    // TargetStatable
                usedAttack,
                damageResult
            );

            if( this.Attacked != null )
                this.Attacked( this, eventArgs );

            usedAttack.OwnerAttackable.NotifyHit( eventArgs );
        }

        /// <summary>
        /// Attacks this attackable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <remarks>
        /// This version of the Attack method doesn't provide information about the used Attack.
        /// </remarks>
        /// <param name="attacker">The entity that has attacked this attackable entity.</param>
        /// <param name="damageResult">The result of the attack.</param>
        public void Attack( ZeldaEntity attacker, AttackDamageResult damageResult )
        {
            this.Attack( attacker as IAttackableEntity, damageResult );
        }

        /// <summary>
        /// Attacks this attackable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <remarks>
        /// This version of the Attack method doesn't provide information about the used Attack.
        /// </remarks>
        /// <param name="attacker">The entity that has attacked this attackable entity.</param>
        /// <param name="damageResult">The result of the attack.</param>
        public void Attack( IAttackableEntity attacker, AttackDamageResult damageResult )
        {
            Debug.Assert( attacker != null );

            if( this.statable != null )
                this.statable.LoseLife( damageResult );

            // Notify.
            var eventArgs = new AttackEventArgs( attacker as ZeldaEntity, this.Owner, statable, null, damageResult );

            if( this.Attacked != null )
                this.Attacked( this, eventArgs );

            if( attacker != null )
                attacker.Attackable.NotifyHit( eventArgs );
        }

        /// <summary>
        /// Identifies the <see cref="Status.Statable"/> component of the Entity that owns this component.
        /// Can be null.
        /// </summary>
        private Status.Statable statable;
    }
}
