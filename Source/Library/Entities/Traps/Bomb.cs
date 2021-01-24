// <copyright file="Bomb.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Bomb class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom.Collections.Pooling;
    using Atom.Fmod;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Attacks;
    using Zelda.Entities.Components;
    using Zelda.Status;

    /// <summary>
    /// Represents a bomb that deals area damage on explosion.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Bomb : ZeldaEntity
    {
        /// <summary>
        /// Returns a ready-to-be-used <see cref="Bomb"/>.
        /// </summary>
        /// <param name="statable">
        /// The <see cref="Statable"/> component of the Entity that wants to spawn a new Bomb.
        /// </param>
        /// <param name="position">
        /// The position to spawn the Bomb.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to spawn the bomb on.
        /// </param>
        /// <param name="damageMethod">
        /// The <see cref="AttackDamageMethod"/> that is used to calculate the damage
        /// done by the Bomb.
        /// </param>
        /// <param name="hitEffect">
        /// The <see cref="IAttackHitEffect"/> to apply when the Bomb hits. Can be null.
        /// </param>
        /// <param name="explosionRadius">
        /// The radius of the explosion.
        /// </param>
        /// <param name="pushingPower">
        /// The pushing power at the center of the explosion.
        /// </param>
        /// <param name="animation">
        /// The sprite animation that is used to visualize the ticking bomb.
        /// </param>
        /// <param name="animationExplosion">
        /// The sprite animation that is used to visualize the bomb explosion.
        /// </param>
        /// <param name="drawOffsetExplosion">
        /// The offset that is applied while drawing the bomb explosion.
        /// </param>
        /// <param name="explosionSound">
        /// The (loaded) sound to play when the bomb explodes.
        /// </param>
        /// <returns>
        /// A ready-to-be-used <see cref="Bomb"/>.
        /// </returns>
        public static Bomb Spawn(
            Statable           statable,
            Vector2            position,
            int                floorNumber,
            AttackDamageMethod damageMethod,
            IAttackHitEffect   hitEffect,
            float              explosionRadius,
            float              pushingPower,
            SpriteAnimation    animation,
            SpriteAnimation    animationExplosion,
            Vector2            drawOffsetExplosion,
            Sound              explosionSound )
        {
            PoolNode<Bomb> poolNode = bombPool.Get();
            Bomb           bomb     = poolNode.Item;

            // Power
            bomb.FloorNumber  = floorNumber;
            bomb.statable     = statable;
            bomb.hitEffect    = hitEffect;
            bomb.damageMethod = damageMethod;

            bomb.pushingPower    = pushingPower;
            bomb.explosionRadius = explosionRadius;

            // Visualization:
            if( animation != null )
            {
                animation.Reset();
                animation.IsLooping = false;
            }

            if( animationExplosion != null )
            {
                animationExplosion.Reset();
                animationExplosion.IsLooping = false;
            }

            bomb.animation          = animation;
            bomb.animationExplosion = animationExplosion;
            bomb.drawOffsetExplosion = drawOffsetExplosion;
            bomb.FloorRelativity     = EntityFloorRelativity.Normal;

            // Misc
            bomb.Transform.Position = position;
            bomb.poolNode           = poolNode;
            bomb.explosionSound     = explosionSound;
            bomb.state              = State.Ticking;

            return bomb;
        }

        /// <summary>
        /// Prevents a default instance of the Bomb class from being created.
        /// </summary>
        private Bomb()
        {
            this.Collision.Size = new Vector2( 12.0f, 12.0f );
        }
        
        /// <summary>
        /// Updates this Bomb.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            switch( this.state )
            {
                case State.Ticking:
                    if( animation != null )
                    {
                        animation.Animate( updateContext.FrameTime );

                        if( animation.Time >= animation.TotalTime )
                        {
                            this.Explode();
                        }
                    }

                    break;

                case State.Exploding:
                    if( animationExplosion != null )
                    {
                        animationExplosion.Animate( updateContext.FrameTime );

                        if( animationExplosion.Time >= animationExplosion.TotalTime )
                        {
                            this.OnExplosionEnded();
                        }
                    }

                    break;

                default:
                case State.None:
                    break;
            }
        }

        /// <summary>
        /// Explodes this Bomb.
        /// </summary>
        public void Explode()
        {
            this.Explode( this.Scene );
        }

        /// <summary>
        /// Explodes this Bomb in the given scene.
        /// </summary>
        /// <param name="scene">
        /// The scene that is queried for bombed enemies.
        /// </param>
        public void Explode( ZeldaScene scene )
        {
            this.state = State.Exploding;
            this.FloorRelativity = EntityFloorRelativity.IsAbove;
            scene.NotifyVisabilityUpdateNeededSoon();

            Vector2 explosionCenter = this.Transform.Position;
            Circle circle = new Circle( explosionCenter, this.explosionRadius );

            foreach( ZeldaEntity target in scene.VisibleEntities )
            {
                if( this.FloorNumber == target.FloorNumber && target.Collision.Intersects( ref circle ) )
                {
                    this.BombTarget( target, ref explosionCenter );
                }
            }

            this.PlayExplosionSound( explosionCenter );
        }

        /// <summary>
        /// Plays the Explosion Sound at the given position.
        /// </summary>
        /// <param name="explosionCenter">
        /// The center of the explosion.
        /// </param>
        private void PlayExplosionSound( Vector2 explosionCenter )
        {
            if( explosionSound != null )
            {
                this.explosionSound.PlayAt( explosionCenter, new FloatRange( 16.0f, this.explosionRadius * 10.0f ) );
            }
        }

        /// <summary>
        /// Bombs the given <paramref name="target"/>
        /// </summary>
        /// <param name="target">
        /// The target to explode.
        /// </param>
        /// <param name="explosionCenter">
        /// The center of the explosion.
        /// </param>
        private void BombTarget( ZeldaEntity target, ref Vector2 explosionCenter )
        {
            if( DealDamage( target ) )
            {
                Push( target, ref explosionCenter );
            }
        }

        private bool DealDamage( ZeldaEntity target )
        {
            IAttackableEntity attacker = this.statable.Owner as IAttackableEntity;
            Attackable attackable = target.Components.Get<Attackable>();

            if( attackable != null )
            {
                Statable statable = attackable.Statable;

                if( statable != null )
                {
                    AttackDamageResult result = this.damageMethod.GetDamageDone( this.statable, statable );
                    attackable.Attack( attacker, result );

                    if( result.AttackReceiveType == AttackReceiveType.Resisted ||
                        result.AttackReceiveType == AttackReceiveType.PartialResisted )
                    {
                        return false;
                    }

                    if( this.hitEffect != null )
                    {
                        this.hitEffect.OnHit( this.statable, statable );
                    }
                }
                else
                {
                    attackable.Attack( attacker, new AttackDamageResult() );
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void Push( ZeldaEntity target, ref Vector2 explosionCenter )
        {
            Moveable targetMoveable = target.Components.Get<Moveable>();

            if( targetMoveable != null && targetMoveable.CanBePushed )
            {
                Vector2 delta = target.Transform.Position - explosionCenter;
                Direction4 dir = delta.ToDirection4();

                float length = delta.Length * 0.75f;
                if( length > explosionRadius )
                {
                    length = explosionRadius;
                }

                float powerFactor = 1.0f - (length / explosionRadius);
                if( powerFactor < 0.11f )
                {
                    powerFactor = 0.11f;
                }

                float power = powerFactor * this.pushingPower;

                // Push the target into the direction the owner was looking
                targetMoveable.Push( power, dir );
            }
        }

        /// <summary>
        /// Called when the Bomb Explosion has ended.
        /// </summary>
        private void OnExplosionEnded()
        {
            this.state = State.None;

            if( this.Scene != null )
            {
                this.RemoveFromScene();
            }

            if( this.poolNode != null )
            {
                bombPool.Return( this.poolNode );
                this.poolNode = null;
            }
        }

        /// <summary>
        /// Draws this Bomb.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            switch( this.state )
            {
                case State.Ticking:
                    if( animation != null )
                    {
                        Vector2 drawPosition = this.Transform.Position;
                        drawPosition.X = (int)drawPosition.X;
                        drawPosition.Y = (int)drawPosition.Y;

                        animation.Draw( drawPosition, drawContext.Batch );
                    }
                    break;

                case State.Exploding:
                    if( animationExplosion != null )
                    {
                        Vector2 drawPosition = this.Transform.Position + drawOffsetExplosion;
                        drawPosition.X = (int)drawPosition.X;
                        drawPosition.Y = (int)drawPosition.Y;

                        animationExplosion.Draw( drawPosition, drawContext.Batch );
                    }
                    break;

                default:
                case State.None:
                    break;
            }
        }
        
        #region [ Fields ]

        /// <summary>
        /// Enumerates the different states a Bomb can be in.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// No specific state.
            /// </summary>
            None,

            /// <summary>
            /// The bomb is currently ticking.
            /// </summary>
            Ticking,

            /// <summary>
            /// The bomb is currently exploding.
            /// </summary>
            Exploding
        }

        /// <summary>
        /// The current state this Bomb is in.
        /// </summary>
        private State state;

        /// <summary>
        /// The Statable that controls the power of the bomb explosion.
        /// </summary>
        private Statable statable;

        /// <summary>
        /// The AttackDamageMethod that is used to calculate the damage of the bomb explosion.
        /// </summary>
        private AttackDamageMethod damageMethod;

        /// <summary>
        /// The effect that gets applied when the bomb explosion hits an enemy.
        /// </summary>
        private IAttackHitEffect hitEffect;

        /// <summary>
        /// The pushing power at the center of the explosion.
        /// </summary>
        private float pushingPower;

        /// <summary>
        /// The SpriteAnimation showing the ticking bomb.
        /// </summary>
        private SpriteAnimation animation;

        /// <summary>
        /// The SpriteAnimation showing the explosion effect.
        /// </summary>
        private SpriteAnimation animationExplosion;

        /// <summary>
        /// The draw offset to apply when drawing the explosion.
        /// </summary>
        private Vector2 drawOffsetExplosion;

        /// <summary>
        /// The PoolNode{Bomb} that connects this Bomb with the bombPool.
        /// </summary>
        private PoolNode<Bomb> poolNode;

        /// <summary>
        /// The radius of the explosion.
        /// </summary>
        private float explosionRadius;

        /// <summary>
        /// The Sound to play when the bomb explodes.
        /// </summary>
        private Sound explosionSound;

        /// <summary>
        /// The pool in which Bomb instances are cached.
        /// </summary>
        private static readonly Pool<Bomb> bombPool = Pool<Bomb>.Create( 2, () => new Bomb() );

        #endregion
    }
}