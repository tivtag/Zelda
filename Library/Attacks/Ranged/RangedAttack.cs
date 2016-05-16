// <copyright file="RangedAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.RangedAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Ranged
{
    using System;
    using Atom;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Projectiles;
    using Zelda.Status;

    /// <summary>
    /// Defines an <see cref="Attack"/> that is used in Ranged Combat.
    /// </summary>
    /// <remarks>
    /// By default the attack's attack delay is the same as the object that uses it.
    /// </remarks>
    public class RangedAttack : Attack, IProjectileAttack
    {
        /// <summary>
        /// Fired when a Projectile has been fired by this RangedAttack.
        /// </summary>
        public event RelaxedEventHandler<Projectile> ProjectileFired;
        
        /// <summary>
        /// Gets the <see cref="ProjectileSettings"/> that control the
        /// Projectiles fired by this RangedAttack.
        /// </summary>
        public ProjectileSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// Gets or sets the ProjectileHitSettings that is used for all Projectiles
        /// fired by this RangedAttack.
        /// </summary>
        /// <value>
        /// Is null by default.
        /// </value>
        public ProjectileHitSettings HitSettings
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RangedAttack"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity that owns the new RangedAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new RangedAttack does. 
        /// </param>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="ZeldaEntity"/> contains no <see cref="Statable"/> component.
        /// </exception>
        public RangedAttack( ZeldaEntity entity, AttackDamageMethod method )
            : base( entity, method )
        {
        }
                
        /// <summary> 
        /// Fires this <see cref="RangedAttack"/>. 
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// Whether this attack has been fired.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.SpawnProjectile();
                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spawns the next Projectile.
        /// </summary>
        private void SpawnProjectile()
        {
            Vector2 center = this.Owner.Collision.Center;
            Direction4 direction = this.Transform.Direction;
            Point2 size = this.settings.Sprites.GetSize( direction );

            switch( direction )
            {
                case Direction4.Up:
                    SpawnProjectile(
                        new Vector2( center.X - (size.X / 2), this.Owner.Transform.Y - 1 ),
                        new Vector2( 0.0f, -GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Down:
                    SpawnProjectile(
                        new Vector2( center.X - (size.Y / 2), center.Y ),
                        new Vector2( 0.0f, GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Left:
                    SpawnProjectile(
                        new Vector2( this.Owner.Transform.X - 2, center.Y - 5 ),
                        new Vector2( -GetProjectileMovementSpeed(), 0.0f )
                    );
                    break;

                case Direction4.Right:
                    SpawnProjectile(
                        new Vector2( center.X + 2, center.Y - 5 ),
                        new Vector2( GetProjectileMovementSpeed(), 0.0f )
                    );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the movement speed of the next Projectile.
        /// </summary>
        /// <returns>
        /// The movement speed of the next Projectile.
        /// </returns>
        protected float GetProjectileMovementSpeed()
        {
            return this.settings.Speed.GetRandomValue( this.Rand );
        }
        
        /// <summary>
        /// Creates a new projectile object with the given settings
        /// and spawns it in the scene.
        /// </summary>
        /// <param name="creator">
        /// The object that fires the projectile.
        /// </param>
        /// <param name="position">
        /// The starting position of the projectile.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor the projectile should spawn at.
        /// </param>
        /// <param name="direction">
        /// The direction the projectile is travelling in.
        /// </param>
        /// <param name="speed">
        /// The traveling speed of the projectile to spawn.
        /// </param>
        /// <returns>
        /// The ProjectileObject; taken from a pool of inactive projectiles.
        /// </returns>
        protected virtual Projectile SpawnProjectile(
            Statable creator,
            Vector2 position,
            int floorNumber,
            Direction4 direction,
            Vector2 speed
        )
        {
            var projectile = Projectile.Spawn(
                creator, 
                position,                
                floorNumber,
                direction,
                speed,
                this.settings,
                this.HitSettings,
                this.DamageMethod
            );

            if( this.ProjectileFired != null )
            {
                this.ProjectileFired( this, projectile );
            }

            return projectile;
        }

        /// <summary>
        /// Spawns a new projectile.
        /// </summary>
        /// <param name="position">The position to spawn a projectile at.</param>
        /// <param name="speed">The traveling speed of the projectile to spawn.</param>
        protected void SpawnProjectile( Vector2 position, Vector2 speed )
        {
            this.SpawnProjectile(
                this.Statable,
                position,
                this.Owner.FloorNumber,
                this.Transform.Direction,
                speed
            );
        }

        /// <summary>
        /// The settings applies to the Projectiles spawned by this RangedAttack.
        /// </summary>
        private readonly ProjectileSettings settings = new ProjectileSettings();
    }
}
