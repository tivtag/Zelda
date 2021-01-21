// <copyright file="FirePillarSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.FirePillarSpell class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting.Spells
{
    using Atom.Collections.Pooling;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Attacks;
    using Zelda.Attacks.Limiter;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Represents a static pillar of fire that deals area damage.
    /// </summary>
    public class FirePillarSpell : Spell
    {
        /// <summary>
        /// The time in seconds betweens attacks of a single pillar against a specific target.
        /// </summary>
        public float TimeBetweenPillarAttacks 
        {
            get;
            set;
        }

        /// <summary>
        /// The indices of the spell animation that are dealing damage.
        /// </summary>
        public IntegerRange AllowedAnimationIndexRange
        {
            get { return allowedAnimationIndexRange; }
            set { allowedAnimationIndexRange = value; }
        }

        /// <summary>
        /// The Statable component of the caster of this Spell.
        /// </summary>
        public Zelda.Status.Statable OwnerStatable => ownerStatable;

        /// <summary>
        /// Initializes a new instance of the FirePillarSpell class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new FirePillarSpell.
        /// </param>
        /// <param name="ownerStatable">
        /// The statable component of the entity that owns the new FirePillarSpell.
        /// </param>
        /// <param name="castTime">
        /// The time in seconds it takes to cast the new FirePillarSpell.
        /// </param>
        /// <param name="damageMethod">
        /// The method that should be used to calculate the damage done by the new FirePillarSpell.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <param name="animationName">
        /// The name of the pillar sprite animation.
        /// </param>
        public FirePillarSpell( ZeldaEntity owner, Zelda.Status.Statable ownerStatable, float castTime, AttackDamageMethod damageMethod, IZeldaServiceProvider serviceProvider, string animationName = "FirePillar_A" )
            : base( owner, castTime, damageMethod )
        {
            this.animationName = animationName;

            this.TimeBetweenPillarAttacks = 1.0f;
            this.ownerStatable = ownerStatable;
            this.sprite = serviceProvider.SpriteLoader.LoadAnimatedSprite( animationName );

            this.pool = new EntityPool<DamageEffectEntity>( 0, this.CreatePoolableFirePillarEntity );
        }

        /// <summary>
        /// Creates an new Fire Pillar entity.
        /// </summary>
        /// <returns>
        /// The newly created Fire Pillar.
        /// </returns>
        private DamageEffectEntity CreateFirePillarEntity()
        {
            var entity = new DamageEffectEntity() {
                Creator = this.ownerStatable
            };

            // Collision
            entity.Collision.IsSolid = false;
            entity.Collision.Set( new Vector2( -1.0f, -4.0f ), new Vector2( 16.0f, 16.0f ) );

            // Drawing
            SpriteAnimation animation = this.sprite.CreateInstance();
            entity.DrawDataAndStrategy = new Zelda.Entities.Drawing.TintedOneDirAnimDrawDataAndStrategy( entity ) {
                Animation = animation,
                SpriteGroup = animationName
            };
            
            // Damage
            entity.MeleeAttack.Limiter = new FirePillarAttackLimiter( this.TimeBetweenPillarAttacks, animation, allowedAnimationIndexRange );
            entity.MeleeAttack.DamageMethod = this.DamageMethod;

            this.SetupFirePillarEntity( entity );
            return entity;
        }

        /// <summary>
        /// Allows custom initialization logic for Fire Pillars created by this FirePillarSpell
        /// to be inserted.
        /// </summary>
        /// <param name="firePillar">
        /// The Fire Pillar that was created created.
        /// </param>
        protected virtual void SetupFirePillarEntity( DamageEffectEntity firePillar )
        {
        }

        /// <summary>
        /// Creates a new poolable Fire Pillar entity.
        /// </summary>
        /// <returns>
        /// The newly created Fire Pillar.
        /// </returns>
        private IPooledObjectWrapper<DamageEffectEntity> CreatePoolableFirePillarEntity()
        {
            DamageEffectEntity entity = CreateFirePillarEntity();
            return new PooledObjectWrapper<DamageEffectEntity>( entity );
        }

        /// <summary>
        /// Fires this FirePillarSpell.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            return true;
        }

        /// <summary>
        /// Creates and spawns a Fire Pillar at the specified location.
        /// </summary>
        /// <param name="position">
        /// The position to spawn the Fire Pillar at.
        /// </param>
        /// <param name="floorNumber">
        /// The floor number to spawn the Fire Pillar at.
        /// </param>
        /// <param name="scene">
        /// The scene to spawn in.
        /// </param>
        /// <returns>
        /// The newly spawned Fire Pillar.
        /// </returns>
        public DamageEffectEntity SpawnFirePillarAt( Vector2 position, int floorNumber, ZeldaScene scene )
        {
            if( CanSpawnAt( position, floorNumber, scene ) )
            {
                // Get from pool.
                PoolNode<IPooledObjectWrapper<DamageEffectEntity>> firePillarNode = this.pool.Get();
                DamageEffectEntity firePillar = firePillarNode.Item.PooledObject;

                // Setup
                firePillar.FloorNumber = floorNumber;
                firePillar.Transform.Position = position;
                firePillar.MeleeAttack.Limiter.Reset();
                firePillar.Behaveable.Enter();

                var drawData = (IAnimatedDrawDataAndStrategy)firePillar.DrawDataAndStrategy;
                drawData.CurrentAnimation.Reset();

                firePillar.AddToScene( scene );
                return firePillar;
            }
            
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the pillar can spawn at the specified position.
        /// </summary>
        /// <param name="position">
        /// The position to spawn at.
        /// </param>
        /// <param name="floorNumber"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        private static bool CanSpawnAt( Vector2 position, int floorNumber, ZeldaScene scene )
        {
            Atom.Scene.Tiles.TileMapFloor floor = scene.Map.Floors[floorNumber];
            int tileId = floor.ActionLayer.GetTileAtSafe( (int)((position.X + 8.0f) / 16), (int)((position.Y + 8.0f) / 16) );

            return DefaultTileHandler.IsWalkable( tileId );
        }

        /// <summary>
        /// Defines the IAttackLimiter that is responsible for limiting damage done by Fillar Pillar.
        /// </summary>
        private sealed class FirePillarAttackLimiter : TimedForEachTargetAttackLimiter
        {
            /// <summary>
            /// Gets a value indicating whether attacking in general is allowed.
            /// </summary>
            /// <value>
            /// true if it is allowed;
            /// otherwise false.
            /// </value>
            public override bool IsAllowed
            {
                get 
                {
                    return this.allowedRange.Contains( this.animation.FrameIndex );
                }
            }

            /// <summary>
            /// Initializes a new instance of the FirePillarAttackLimiter class.
            /// </summary>
            /// <param name="attackDelay">
            /// The time between an attack can hit the same enemy twice.
            /// </param>
            /// <param name="animation">
            /// Identifies the animation of the Fire Pillar whose attacks are limited by the new FirePillarAttackLimiter.
            /// </param>
            /// <param name="allowedAnimationIndexRange">
            /// The indices of the spell animation that are dealing damage.
            /// </param>
            public FirePillarAttackLimiter( float attackDelay, SpriteAnimation animation, IntegerRange allowedAnimationIndexRange )
            {
                this.AttackDelay = attackDelay;
                this.animation = animation;
                this.allowedRange = allowedAnimationIndexRange;
            }
            
            private IntegerRange allowedRange;
            
            /// <summary>
            /// Identifies the animation of the Fire Pillar.
            /// </summary>
            private readonly SpriteAnimation animation;
        }
        
        private IntegerRange allowedAnimationIndexRange = new IntegerRange( 0, 8 );

        /// <summary>
        /// The name of the sprite animation asset used by the FirePillar.
        /// </summary>
        private readonly string animationName;

        /// <summary>
        /// Stores the AnimatedSprite that is sued to visualuze the Fire Pillars.
        /// </summary>
        private readonly AnimatedSprite sprite;

        /// <summary>
        /// The statable component of the entity that owns this FirePillarSpell.
        /// </summary>
        private readonly Zelda.Status.Statable ownerStatable;

        /// <summary>
        /// The pool of Fire Pillars, used to avoid setup time and allocating memory during gameplay.
        /// </summary>
        private readonly EntityPool<DamageEffectEntity> pool;
    }
}
