// <copyright file="RangedPlayerAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.RangedPlayerAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Ranged
{
    using System;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    
    /// <summary>
    /// Defines a <see cref="RangedAttack"/> that is used by the Player.
    /// </summary>
    public class RangedPlayerAttack : RangedAttack
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this <see cref="RangedPlayerAttack"/> can be used
        /// depending on the state of the PlayerEntity that owns it.
        /// </summary>
        /// <remarks>
        /// Ranged Attacks require a ranged weapon to be equiped.
        /// </remarks>
        public override bool IsUseable
        {
            get
            {
                // The player can't use any ranged attack if:
                return this.player.Equipment.CanUseRanged && !this.player.Moveable.IsSwimming;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Entities.PlayerEntity"/> that owns this RangedPlayerAttack.
        /// </summary>
        protected Zelda.Entities.PlayerEntity Player
        {
            get
            {
                return player;
            }
        }

        /// <summary>
        /// Gets the <see cref="PlayerDrawDataAndStrategy"/> of the PlayerEntity
        /// that owns this PlayerRangedAttack.
        /// </summary>
        protected PlayerDrawDataAndStrategy DrawStrategy
        {
            get
            {
                return this.drawStrategy;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedPlayerAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The entity that owns the new RangedPlayerAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new RangedPlayerAttack does. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="player"/> or <paramref name="method"/> is null.
        /// </exception>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="PlayerEntity"/> contains no <see cref="Zelda.Status.Statable"/> component.
        /// </exception>
        public RangedPlayerAttack( PlayerEntity player, AttackDamageMethod method )
            : base( player, method )
        {
            this.player = player;
            this.drawStrategy = player.DrawDataAndStrategy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedPlayerAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The entity that owns the new RangedPlayerAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new RangedPlayerAttack does. 
        /// </param>
        /// <param name="cooldown">
        /// The Cooldown that is used to create a new TimedAttackLimiter for the new RangedPlayerAttack.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="player"/>, <paramref name="method"/> or <paramref name="cooldown"/> is null.
        /// </exception>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="PlayerEntity"/> contains no <see cref="Zelda.Status.Statable"/> component.
        /// </exception>
        public RangedPlayerAttack( PlayerEntity player, AttackDamageMethod method, Cooldown cooldown )
            : base( player, method )
        {
            this.player = player;
            this.drawStrategy = player.DrawDataAndStrategy;
            this.Limiter = new Zelda.Attacks.Limiter.TimedAttackLimiter( cooldown );
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Fires this <see cref="RangedPlayerAttack"/>,
        /// launching a projectile.
        /// </summary>
        /// <param name="target">This paramter is not used.</param>
        /// <returns>
        /// true if the attack has been used;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.SpawnProjectile();
                this.drawStrategy.ShowSpecialAnimation( PlayerSpecialAnimation.AttackRanged );
                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this PlayerRangedAttack should fire.
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
            var specialAnimation = drawStrategy.SpecialAnimation;

            if( specialAnimation != PlayerSpecialAnimation.None &&
                specialAnimation != PlayerSpecialAnimation.AttackRanged )
            {
                return false;
            }

            return this.IsReady;
        }

        /// <summary>
        /// Spawns a new Projectile heading into the right direction.
        /// </summary>
        private void SpawnProjectile()
        {
            var direction = player.Transform.Direction;
            Vector2 objCenter = player.Collision.Center;

            switch( direction )
            {
                case Direction4.Up:
                    SpawnProjectile(
                        new Vector2( objCenter.X - 7, player.Transform.Y + 2 ),
                        new Vector2( 0, -GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Down:
                    SpawnProjectile(
                        new Vector2( objCenter.X + 2, objCenter.Y - 2 ),
                        new Vector2( 0, GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Left:
                    SpawnProjectile(
                        new Vector2( player.Transform.X, objCenter.Y - 5 ),
                        new Vector2( -GetProjectileMovementSpeed(), 0 )
                    );
                    break;

                case Direction4.Right:
                    SpawnProjectile(
                        new Vector2( objCenter.X, objCenter.Y - 5 ),
                        new Vector2( GetProjectileMovementSpeed(), 0 )
                    );
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The PlayerEntity that owns this <see cref="RangedPlayerAttack"/>.
        /// </summary>
        private readonly PlayerEntity player;

        /// <summary>
        /// The PlayerDrawDataAndStrategy associated with the PlayerEntity.
        /// </summary>
        private readonly PlayerDrawDataAndStrategy drawStrategy;

        #endregion
    }
}
