// <copyright file="Projectile.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.Projectile class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Projectiles
{
    using System;
    using Atom;
    using Atom.Collections.Pooling;
    using Atom.Fmod;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Attacks.Ranged;
    using Zelda.Entities.Components;
    using Zelda.Entities.Projectiles.Drawing;
    using Zelda.Status;
    
    /// <summary>
    /// Represents a projectile, such as an arrow.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Projectile : ZeldaEntity, ISceneChangeListener
    {
        #region [ Events ]

        /// <summary>
        /// Fired when this ProjectileObject got destroyed.
        /// </summary>
        public event SimpleEventHandler<Projectile> Destroyed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the optional data of this Projectile.
        /// </summary>
        /// <value>The default value is zero.</value>
        public object OptionalData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the distance this Projectile has travelled; squared.
        /// </summary>
        public float DistanceTravelledSquared
        {
            get
            {
                return this.travelledDistanceSquared;
            }
        }

        /// <summary>
        /// Gets the <see cref="IStatefulProjectileUpdateLogic"/> instance
        /// that has been assigned to this Projectile.
        /// </summary>
        public IStatefulProjectileUpdateLogic StatefulUpdateLogic
        {
            get
            {
                return this.statefulUpdateLogic;
            }
        }

        /// <summary>
        /// Gets the ProjectileDrawDataAndStrategy used to draw this Projectile.
        /// </summary>
        internal new ProjectileDrawDataAndStrategy DrawDataAndStrategy
        {
            get
            {
                return (ProjectileDrawDataAndStrategy)base.DrawDataAndStrategy;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        public Projectile()
            : base( 4 )
        {
            this.IsEditable = false;
            this.IsSaved = false;
            this.Removed += this.OnRemoved;

            // Components:
            this.moveable = new Moveable() {
                CanBePushed = false,
                CanSwim = true,
                CanSlide = false
            };

            this.moveable.MapCollisionOccurred += this.OnMapCollisionOccurred;
            this.moveable.TileHandler           = FlyingTileHandler.Instance;
            this.Components.Add( this.moveable );            
            
            // Other:
            this.meleeAttack         = new ProjectileMeleeAttack( this );
            base.DrawDataAndStrategy = new ProjectileDrawDataAndStrategy( this );
        }

        /// <summary>
        /// Creates a new projectile object with the given settings
        /// and spawns it in the scene.
        /// </summary>
        /// <param name="statable">
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
        /// The movement speed of the projectile.
        /// </param>
        /// <param name="settings">
        /// The settings used for the Projectile.
        /// </param>
        /// <param name="hitSettings">
        /// The ProjectileHitSettings used for the Projectile.
        /// </param>
        /// <param name="damageMethod">
        /// The damage method to use when the projectile hits an enemy.
        /// </param>
        /// <returns>
        /// The ProjectileObject; taken from a pool of inactive projectiles.
        /// </returns>
        public static Projectile Spawn(
            Statable statable,
            Vector2 position,
            int floorNumber,
            Direction4 direction,
            Vector2 speed,
            ProjectileSettings settings,
            ProjectileHitSettings hitSettings,
            AttackDamageMethod damageMethod )
        {
            PoolNode<Projectile> node = pool.Get();
            Projectile projectile     = node.Item;

            // General:
            projectile.Destroyed           = null;
            projectile.FloorNumber         = floorNumber;
            projectile.Transform.Position  = position;
            projectile.Transform.Direction = direction;
            projectile.Transform.Scale = Vector2.One;

            var sprite = settings.Sprites.Get( direction );
            projectile.Collision.Size = new Vector2( sprite.Width, sprite.Height );

            // Other:
            projectile.poolNode                 = node;
            projectile.creator                  = statable;
            projectile.settings                 = settings;
            projectile.hitSettings              = hitSettings;
            projectile.OptionalData             = null;

            projectile.statefulUpdateLogic      = settings.GetStatefulUpdateLogicInstance();
            projectile.movementSpeed            = speed;
            projectile.travelledDistanceSquared = 0.0f;

            // Attack:
            projectile.meleeAttack.Owner        = statable.Owner;
            projectile.meleeAttack.DamageMethod = damageMethod;
            projectile.meleeAttack.HitEffect = hitSettings != null ? hitSettings.AttackHitEffect : null; 
            projectile.meleeAttack.Limiter.Reset();

            // Drawing:
            projectile.DrawDataAndStrategy.Sprite = sprite;

            // Add:
            projectile.AddToScene( statable.Scene );
            projectile.StartTravellingSound();

            return projectile;
        }

        /// <summary>
        /// Creates the dynamic pool that stores Projectile objects.
        /// </summary>
        /// <returns>
        /// The newly created Pool{Projectile}.
        /// </returns>
        private static Pool<Projectile> CreateProjectilePool()
        {
            const int InitialPoolSize = 40;

            var pool = new Pool<Projectile>( InitialPoolSize, () => new Projectile() );
            pool.AddNodes( InitialPoolSize );

            return pool;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this Projectile.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.ExecuteCustomUpdateLogic( updateContext );

            this.UpdateAttack( updateContext );
            this.UpdateMovement( updateContext );
            this.UpdateTravellingSound();
            base.Update( updateContext );
        }

        /// <summary>
        /// Updates the movement of this Projectile.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateMovement( ZeldaUpdateContext updateContext )
        {
            Vector2 change;
            this.moveable.Move( this.movementSpeed, updateContext.FrameTime, out change );

            this.travelledDistanceSquared += change.SquaredLength;
            if( this.travelledDistanceSquared >= this.settings.TravelRangeSquared )
            {
                this.Destroy();
            }
        }

        /// <summary>
        /// Executes the custom update logic attached to this Projectile.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void ExecuteCustomUpdateLogic( ZeldaUpdateContext updateContext )
        {
            if( this.statefulUpdateLogic != null )
            {
                this.statefulUpdateLogic.Update( this, updateContext );
            }
        }

        /// <summary>
        /// Updates the attack logic of this Projectile.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateAttack( ZeldaUpdateContext updateContext )
        {
            this.meleeAttack.Update( updateContext );
            this.TestCollision();
        }

        /// <summary>
        /// Tests for collesion against enemy objects.
        /// </summary>
        private void TestCollision()
        {
            // Only test if nescassary
            if( !this.meleeAttack.IsReady )
                return;

            var rectangle = this.Collision.Rectangle;
            rectangle.X      -= 3.0f;
            rectangle.Y      -= 3.0f;
            rectangle.Width  += 6.0f;
            rectangle.Height += 6.0f;

            var entities = this.Scene.VisibleEntities;

            for( int i = 0; i < entities.Count; ++i )
            {
                var target = entities[i];

                if( this.FloorNumber == target.FloorNumber &&
                    target.Collision.Intersects( ref rectangle ) )
                {
                    var attackable = target.Components.Get<Attackable>();

                    if( attackable != null )
                    {
                        var statable = attackable.Statable;

                        if( statable != null )
                        {
                            if( creator.IsFriendly != statable.IsFriendly )
                            {
                                if( this.meleeAttack.Fire( attackable ) )
                                {
                                    this.OnProjectileHitTarget( target, statable );
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if( this.meleeAttack.Fire( attackable ) )
                            {
                                if( target.Collision.IsSolid )
                                {
                                    this.Destroy();
                                }

                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when this Projectile hit an enemy target.
        /// </summary>
        /// <param name="target">
        /// The target that got hit.
        /// </param>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that got hit.
        /// </param>
        private void OnProjectileHitTarget( ZeldaEntity target, Statable statable )
        {
            this.TryPierceTarget( target, statable );
        }

        /// <summary>
        /// Tries to pierce the hit enemy Entity;
        /// the Projectile gets destroyed if the target wasn't pierced.
        /// </summary>
        /// <param name="target">
        /// The target that got hit.
        /// </param>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that got hit.
        /// </param>
        private void TryPierceTarget( ZeldaEntity target, Statable statable )
        {
            if( target.Collision.IsSolid )
            {
                if( this.ShouldPierce() == false )
                {
                    this.Destroy();
                }
            }
            else
            {
                if( statable.IsInvincible )
                {
                    // Reset the limiter of the projectile
                    // incase the target is invincible and 
                    // not solid; mostly damage entities that
                    // aren't meant to deal damage.
                    this.meleeAttack.Limiter.Reset();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Projectile should
        /// pierce its next target.
        /// </summary>
        /// <returns>
        /// Returns true if the Projectile should pierce;
        /// otherwise false.
        /// </returns>
        private bool ShouldPierce()
        {
            if( this.settings.CanPierce )
            {
                float piercingChance = this.settings.GetChanceToPierce( this.creator );
                float piercingRand   = this.Scene.Rand.RandomRange( 0.0f, 100.0f );

                if( piercingRand <= piercingChance )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Destroys this Projectile, removing it from the scene.
        /// </summary>
        private void Destroy()
        {
            if( this.Scene != null )
            {
                this.RemoveFromScene();
            }

            this.OnDestroyed();
        }

        /// <summary>
        /// Returns this Projectile to the projectile pool.
        /// </summary>
        private void ReturnProjectileToPool()
        {
            if( this.poolNode.IsActive )
            {
                pool.Return( this.poolNode );
            }

            this.ReturnSprite();
        }

        /// <summary>
        /// Returns the ISprite used by this Projectile to the IProjectileSprites.
        /// </summary>
        private void ReturnSprite()
        {
            this.settings.Sprites.Return( this.DrawDataAndStrategy.Sprite );
        }

        /// <summary>
        /// Called when this Projectile has been destroyed.
        /// </summary>
        private void OnDestroyed()
        {
            this.StopPlayingTravellingSound();
            this.PlayHitSoundSample();

            if( this.Destroyed != null )
            {
                this.Destroyed( this );
            }
        }

        // <summary>
        /// Notifies this Projectile that a scene change has occured.
        /// </summary>
        /// <param name="changeType">
        /// States whether the current scene has changed away or to its current scene.
        /// </param>
        public void NotifySceneChange( ChangeType changeType )
        {
            if( changeType == ChangeType.To )
            {
                this.StartTravellingSound();
            }
            else
            {
                this.StopPlayingTravellingSound();
            }
        }

        #region > Sound <

        /// <summary>
        /// Starts to play the travelling sound.
        /// </summary>
        private void StartTravellingSound()
        {
            if( this.settings.TravellingSound != null )
            {
                this.travellingSoundChannel = this.settings.TravellingSound.PlayAt( this.Collision.Center );
            }
        }

        /// <summary>
        /// Updates the travelling sound.
        /// </summary>
        private void UpdateTravellingSound()
        {
            if( this.travellingSoundChannel != null )
            {
                try
                {
                    Vector2 center = this.Collision.Center;
                    this.travellingSoundChannel.Set3DAttributes( center.X, center.Y );
                }
                catch( Atom.Fmod.AudioException )
                {
                    this.travellingSoundChannel = null;
                }
            }
        }

        /// <summary>
        /// Stops to play the travelling sound.
        /// </summary>
        private void StopPlayingTravellingSound()
        {
            if( this.travellingSoundChannel != null )
            {
                try
                {
                    if( this.travellingSoundChannel.IsPlaying )
                    {
                        this.travellingSoundChannel.Stop();
                    }
                }
                catch( Atom.Fmod.AudioException )
                {
                }

                this.travellingSoundChannel = null;
            }
        }

        /// <summary>
        /// Plays the 'Projectile Hit' sound sample.
        /// </summary>
        private void PlayHitSoundSample()
        {
            if( this.hitSettings != null )
            {
                this.hitSettings.SoundSample.PlayAt( this.Collision.Center );
            }
        }

        #endregion

        /// <summary>
        /// Gets called when this Projectile has collided with the TileMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnMapCollisionOccurred( Moveable sender )
        {
            this.Destroy();
        }

        /// <summary>
        /// Gets called when this Projectile has been removed from a ZeldaScene.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="scene">The scene from which the Projectile got removed.</param>
        private void OnRemoved( object sender, ZeldaScene scene )
        {
            this.ReturnProjectileToPool();
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">This operation is not supported.</exception>
        /// <returns>Nothing is returned.</returns>
        public override ZeldaEntity Clone()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The settings applied to this Projectile.
        /// </summary>
        private ProjectileSettings settings;

        /// <summary>
        /// The ProjectileHitSettings applied to this Projectile.
        /// </summary>
        private ProjectileHitSettings hitSettings;

        /// <summary>
        /// The Statable component of the Entity that has fired this Projectile.
        /// </summary>
        private Statable creator;

        /// <summary>
        /// The movement speed of this Projectile.
        /// </summary>
        private Vector2 movementSpeed;

        /// <summary>
        /// The distance the projectile has travelled.
        /// </summary>
        private float travelledDistanceSquared;

        /// <summary>
        /// The additional update logic executed for this Projectile.
        /// </summary>
        private IStatefulProjectileUpdateLogic statefulUpdateLogic;

        /// <summary>
        /// The channel in which the sound is played for this Projectile.
        /// </summary>
        private Channel travellingSoundChannel;

        /// <summary>
        /// The melee attack that happens when the arrow hits an enemy.
        /// </summary>
        private readonly ProjectileMeleeAttack meleeAttack;

        /// <summary>
        /// Indentifies the Moveable component of this Projectile Entity.
        /// </summary>
        private readonly Moveable moveable;
        
        /// <summary>
        /// The current pool-node of the this Projectile.
        /// </summary>
        private PoolNode<Projectile> poolNode;

        /// <summary>
        /// The dynamic pool in which unused Projectiles are cached.
        /// </summary>
        private static readonly Pool<Projectile> pool = CreateProjectilePool();

        #endregion
    }
}