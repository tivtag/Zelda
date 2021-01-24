// <copyright file="ProjectilePlayerSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.ProjectilePlayerSpell class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting.Spells
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities;
    using Zelda.Entities.Projectiles;
    using Zelda.Status;
    
    /// <summary>
    /// Defines a PlayerSpell that fires a Projectile.
    /// </summary>
    public class ProjectilePlayerSpell : PlayerSpell, IProjectileAttack
    {
        /// <summary>
        /// Fired when a Projectile has been fired by this ProjectilePlayerSpell.
        /// </summary>
        public event Atom.RelaxedEventHandler<Projectile> ProjectileFired;

        /// <summary>
        /// Fired when a Projectile that has been fired by this ProjectilePlayerSpell got destroyed.
        /// </summary>
        public event Atom.RelaxedEventHandler<Projectile> ProjectileDestroyed;

        /// <summary>
        /// Gets the <see cref="ProjectileSettings"/> that control the
        /// Projectiles fired by this ProjectilePlayerSpell.
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
        /// fired by this ProjectilePlayerSpell.
        /// </summary>
        public ProjectileHitSettings HitSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePlayerSpell"/> class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new ProjectilePlayerSpell.
        /// </param>
        /// <param name="castTime">
        /// The time it takes for the new ProjectilePlayerSpell to cast.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new ProjectilePlayerSpell does. 
        /// </param>
        public ProjectilePlayerSpell( PlayerEntity owner, float castTime, AttackDamageMethod method )
            : base( owner, castTime, method )
        {
        }

        /// <summary>
        /// Setups this ProjectilePlayerSpell.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( this.HitSettings != null )
                this.HitSettings.Setup( serviceProvider );

            this.settings.Setup( serviceProvider );
            base.Setup( serviceProvider );
        }
        
        /// <summary> 
        /// Fires this <see cref="ProjectilePlayerSpell"/>. 
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// Whether this attack has been fired.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            this.SpawnProjectile();
            return true;
        }

        /// <summary>
        /// Gets a spawn position for a Projectile of this ProjectilePlayerSpell
        /// that travels into the given direction.
        /// </summary>
        /// <param name="position">
        /// The spawning position.
        /// </param>
        /// <param name="direction">
        /// The direction the projectile should head to visually.
        /// </param>
        /// <returns>
        /// The new and centered position.
        /// </returns>
        public Vector2 GetCenteredSpawnPosition( Vector2 position, Direction4 direction )
        {
            Point2 size = this.settings.Sprites.GetSize( direction );
            return new Vector2( position.X - (size.X / 2), position.Y - (size.Y / 2) );
        }
        
        /// <summary>
        /// Fires a projectile of this ProjectilePlayerSpell from the given position
        /// into the given direction.
        /// </summary>
        /// <param name="position">
        /// The spawning position.
        /// </param>
        /// <param name="directionVector">
        /// The actual direction the projectile should travel to.
        /// </param>
        /// <param name="direction">
        /// The direction the projectile should head to visually.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to spawn at.
        /// </param>
        /// <returns>
        /// The Projectile that has been fired.
        /// </returns>
        internal Projectile FireFromInto( Vector2 position, Vector2 directionVector, Direction4 direction, int floorNumber )
        {
            Vector2 speed = directionVector * this.GetProjectileMovementSpeed();
            return this.SpawnProjectile( this.Statable, position, floorNumber, direction, speed ); 
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
                        new Vector2( center.X - size.X, this.Owner.Transform.Y - 1 ),
                        new Vector2( 0.0f, -GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Down:
                    SpawnProjectile(
                        new Vector2( center.X, center.Y ),
                        new Vector2( 0.0f, GetProjectileMovementSpeed() )
                    );
                    break;

                case Direction4.Left:
                    SpawnProjectile(
                        new Vector2( this.Owner.Transform.X - (size.Y / 2), center.Y - 7 ),
                        new Vector2( -GetProjectileMovementSpeed(), 0.0f )
                    );
                    break;

                case Direction4.Right:
                    SpawnProjectile(
                        new Vector2( center.X + 2, center.Y - 7 ),
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
        /// A random value within the range stored in the ProjectileSettings.
        /// </returns>
        protected float GetProjectileMovementSpeed()
        {
            return this.settings.Speed.GetRandomValue( this.Rand );
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

            projectile.Destroyed += this.OnProjectileDestroyed;

            if( this.ProjectileFired != null )
            {
                this.ProjectileFired( this, projectile );
            }

            return projectile;
        }

        /// <summary>
        /// Gets called when a Projectile fired by this ProjectilePlayerSpell has been destroyed;
        /// </summary>
        /// <param name="projectile">
        /// The sender of the event.
        /// </param>
        private void OnProjectileDestroyed( Projectile projectile )
        {
            projectile.Destroyed -= this.OnProjectileDestroyed;

            if( this.ProjectileDestroyed != null )
            {
                this.ProjectileDestroyed( this, projectile );
            }
        }
        
        /// <summary>
        /// The settings applies to the Projectiles spawned by this ProjectilePlayerSpell.
        /// </summary>
        private readonly ProjectileSettings settings = new ProjectileSettings();
    }
}