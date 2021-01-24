// <copyright file="PlayerMeleeAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Melee.PlayerMeleeAttack class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Attacks.Melee
{
    using System;
    using System.Diagnostics;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Status;

    /// <summary>
    /// Defines a <see cref="MeleeAttack"/> that is used by the Player.
    /// </summary>
    public class PlayerMeleeAttack : MeleeAttack
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Attack"/> is useable depending on the state of its owner.
        /// E.g. one usually can't use an attack while swimming, or if there is not enough mana to use it.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                // The player can't use any melee attack if:
                return !this.player.Moveable.IsSwimming;
            }
        }

        /// <summary>
        /// Gets the <see cref="PlayerDrawDataAndStrategy"/> of the PlayerEntity
        /// that owns this PlayerMeleeAttack.
        /// </summary>
        protected PlayerDrawDataAndStrategy DrawStrategy
        {
            get
            {
                return this.drawStrategy;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerMeleeAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The entity that owns the new PlayerMeleeAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new PlayerMeleeAttack does. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="player"/> or <paramref name="method"/> is null.
        /// </exception>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="PlayerEntity"/> contains no <see cref="Statable"/> component.
        /// </exception>
        public PlayerMeleeAttack( PlayerEntity player, AttackDamageMethod method )
            : base( player, method )
        {
            this.player = player;
            this.drawStrategy = player.DrawDataAndStrategy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerMeleeAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The entity that owns the new PlayerMeleeAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new PlayerMeleeAttack does. 
        /// </param>
        /// <param name="cooldown">
        /// The Cooldown that is used to create a new TimedAttackLimiter for the new PlayerMeleeAttack.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="player"/>, <paramref name="method"/> or <paramref name="cooldown"/> is null.
        /// </exception>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="PlayerEntity"/> contains no <see cref="Statable"/> component.
        /// </exception>
        public PlayerMeleeAttack( PlayerEntity player, AttackDamageMethod method, Cooldown cooldown )
            : base( player, method )
        {
            this.player = player;
            this.drawStrategy = player.DrawDataAndStrategy;
            this.Limiter = new Zelda.Attacks.Limiter.TimedAttackLimiter( cooldown );
        }
                
        /// <summary>
        /// Fires this <see cref="PlayerMeleeAttack"/>.
        /// </summary>
        /// <param name="target">
        /// This parameter is not used. - 
        /// A player melee attack always hits all enemies in its range.
        /// </param>
        /// <returns>
        /// true if the Attack was executed, otherwise false.
        /// </returns>
        public override bool Fire( Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.OnFiring();
                this.HandleAttack();
                this.drawStrategy.ShowSpecialAnimation( PlayerSpecialAnimation.AttackMelee );
                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this PlayerMeleeAttack should fire.
        /// </summary>
        /// <returns>
        /// true if it can and should fire;
        /// otherwise false.
        /// </returns>
        protected override bool ShouldFire()
        {
            if( !this.IsUseable || this.player.IsCasting )
                return false;

            // Don't allow attacking if any other special animation is currently shown.
            if( this.drawStrategy.SpecialAnimation != PlayerSpecialAnimation.None &&
                this.drawStrategy.SpecialAnimation != PlayerSpecialAnimation.AttackMelee )
            {
                return false;
            }

            return this.IsReady;
        }

        /// <summary>
        /// Handles the MeleeAttack by computing the current Attack Region.
        /// </summary>
        private void HandleAttack()
        {
            RectangleF area = GetAttackArea( player.Transform.Direction );
            this.HandleAttack(ref area);
        }

        private RectangleF GetAttackArea( Direction4 direction )
        {
            Vector2 position = this.player.Transform.Position;
            Vector2 collisionPosition = this.player.Collision.Position;

            switch(direction)
            {
                case Direction4.Left:
                    return new RectangleF(
                        collisionPosition.X - 17.0f,
                        position.Y - 2.0f,
                        21.0f,
                        30.0f
                    );

                case Direction4.Right:
                    return new RectangleF(
                        collisionPosition.X + player.Collision.Width - 5.0f,
                        position.Y - 2.0f,
                        21.0f,
                        30.0f
                    );

                case Direction4.Up:
                    return new RectangleF(
                        collisionPosition.X - 10.0f,
                        position.Y - 12.0f,
                        36.0f,
                        21.0f
                    );

                case Direction4.Down:
                    return new RectangleF(
                        collisionPosition.X - 10.0f,
                        position.Y + player.Collision.Height + 4.0f,
                        36.0f,
                        20.0f
                    );

                default:
                    return new RectangleF();
            }
        }

        /// <summary>
        /// Handles the MeleeAttack for the given <paramref name="attackRegion"/>.
        /// </summary>
        /// <param name="attackRegion">
        /// The region the attack should land.
        /// </param>
        public void HandleAttack( ref RectangleF attackRegion )
        {
            Debug.Assert( this.DamageMethod != null, "DamageMethod of type is null. " + this.GetType().ToString() );

            int playerFloor  = player.FloorNumber;
            ZeldaScene scene = player.Scene;

            var entities = scene.VisibleEntities;

            for( int i = 0; i < entities.Count; ++i )
            {
                var target = entities[i];
                if( playerFloor != target.FloorNumber )
                    continue;

                if( attackRegion.Intersects( target.Collision.Rectangle ) )
                {
                    if( target == player )
                        continue;

                    this.HandleActualTarget( target );
                }
            }
        }

        /// <summary>
        /// Handles the actual attacking of the given ZeldaEntity.
        /// </summary>
        /// <param name="target">
        /// A ZeldaEntity that actually got hit by this attack.
        /// </param>
        private void HandleActualTarget( ZeldaEntity target )
        {
            var attackable = target.Components.Get<Attackable>();
            if( attackable == null )
                return;

            var statable = attackable.Statable;

            if( statable != null && !statable.IsInvincible )
            {
                AttackDamageResult result = this.DamageMethod.GetDamageDone( player.Statable, statable );
                attackable.Attack( this, result );

                if( statable.Life <= 0 ||
                    result.AttackReceiveType == AttackReceiveType.Miss ||
                    result.AttackReceiveType == AttackReceiveType.Dodge )
                {
                    return;
                }
            }
            else
            {
                attackable.Attack( this, new AttackDamageResult() );
            }

            // Push the enemy
            if( this.IsPushing )
            {
                var moveable = target.Components.Get<Moveable>();

                if( moveable != null )
                {
                    float power = this.ServiceProvider.Rand.RandomRange( this.PushingPowerMinimum, this.PushingPowerMaximum );

                    power += player.Statable.PushingForceAdditive;
                    power *= player.Statable.PushingForceMultiplicative;

                    moveable.Push( power, player.Transform.Direction );
                }
            }
        }
        
        /// <summary>
        /// The PlayerEntity that owns this <see cref="PlayerMeleeAttack"/>.
        /// </summary>
        protected readonly PlayerEntity player;

        /// <summary>
        /// The PlayerDrawDataAndStrategy associated with the PlayerEntity.
        /// </summary>
        private readonly PlayerDrawDataAndStrategy drawStrategy;
    }
}
