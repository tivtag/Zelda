// <copyright file="EnemyChasePlayerBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.EnemyChasePlayerBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.Diagnostics.Contracts;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Zelda.Entities.Components;
    using Zelda.Status;

    /// <summary>
    /// Defines an IEntityBehaviour which commands an <see cref="Enemy"/> entity 
    /// to chase after the <see cref="PlayerEntity"/>.
    /// </summary>
    public class EnemyChasePlayerBehaviour : IEntityBehaviour
    {
        #region [ Constants ]

        /// <summary>
        /// The time the Enemy waits after it got stuck to try to find
        /// a new path to the player.
        /// </summary>
        private const float StuckWaitTime = 0.5f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this <see cref="EnemyChasePlayerBehaviour"/> is currently active.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public bool IsActive
        {
            get;
            private set;
        } 

        /// <summary>
        /// Gets the current state of this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public State BehaviourState
        {
            get
            {
                return this.state; 
            }
        }

        /// <summary>
        /// Gets or sets the time the Enemy continues to chase the player.
        /// </summary> 
        [System.ComponentModel.Category( "Behaviour" ), System.ComponentModel.DefaultValue( 16.0 ),
         System.ComponentModel.Description( "The time in seconds the Enemy chases the player." )]
        public float ChasingTime
        {
            get
            {
                return this.chasingTime;
            }

            set
            {
                if( value < 0.0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.chasingTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the movement speed increase that is 
        /// applied to the enemy when he charges the player )in percent).
        /// </summary> 
        [System.ComponentModel.Category( "Behaviour" ), System.ComponentModel.DefaultValue( 30.0 ),
         System.ComponentModel.Description( "The movement speed increase that is applied to the enemy when he charges the player in %." )]
        public float ChasingSpeedModifier
        {
            get
            {
                return this.chasingSpeedEffect.Value;
            }

            set
            {
                this.chasingSpeedEffect.Value = value;
            }
        }        

        /// <summary>
        /// Gets or sets a value indicating whether the Enemy is chasing the PlayerEntity forever. Usually boss-scripts.
        /// </summary> 
        [System.ComponentModel.Category( "Behaviour" ), System.ComponentModel.DefaultValue( false ),
         System.ComponentModel.Description( "States whether the Enemy chases the player forever." )]
        public bool IsChasingForever
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the target the enemy is supposed to chase.
        /// </summary>
        private PlayerEntity Target
        {
            get
            {
                return this.enemy.Scene.Player;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyChasePlayerBehaviour"/> class.
        /// </summary>
        internal EnemyChasePlayerBehaviour()
        {
            this.chasingSpeedEffect = new MovementSpeedEffect( 30.0f, StatusManipType.Percental );

            this.chasingAura = new PermanentAura( new StatusEffect[1] { chasingSpeedEffect } );
            this.returningAura = new PermanentAura(
                new StatusEffect[1] {
                    new MovementSpeedEffect( 40.0f, StatusManipType.Percental ) 
                }
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyChasePlayerBehaviour"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The entity that is controlled by the new EnemyChasePlayerBehaviour.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="enemy"/> is null.
        /// </exception>
        public EnemyChasePlayerBehaviour( Enemy enemy )
            : this()
        {
            Contract.Requires<ArgumentNullException>( enemy != null );

            this.enemy    = enemy;
            this.moveable = enemy.Moveable;

            this.eventHandlerEnemyChangedFloor = new Atom.RelaxedEventHandler<Atom.ChangedValue<int>>( OnEnemyChangedFloor );
            this.eventHandlerEnemyAttacked     = new Atom.RelaxedEventHandler<AttackEventArgs>( OnEnemyAttacked );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Notifies this script that the player has attacked the enemy.
        /// </summary>
        public void NotifyAttackedByPlayer()
        {
            this.ContinueToChase();
        }

        /// <summary>
        /// Commands this script to recalculate it's path.
        /// </summary>
        public void RecalculateChasingPath()
        {
            this.pathFollower.ResetPath();
        }

        /// <summary>
        /// Tells the behaviour script to continue to chase the player.
        /// </summary>
        public void ContinueToChase()
        {
            this.chasingTimeLeft = this.chasingTime;
            this.isStuck = false;
        }

        /// <summary>
        /// Called when no path to the player could be found.
        /// </summary>
        private void OnNoPathFound()
        {
            this.OnPathStuck( PathFollowState.HardStuck );
        }
        
        /// <summary>
        /// Called when the Enemy got stuck on his path.
        /// </summary>
        /// <param name="followState">
        /// States what kind of stuck state the entity is in.
        /// </param>
        private void OnPathStuck( PathFollowState followState )
        {
            if( !this.isStuck )
            {
                if( followState == PathFollowState.HardStuck )
                {
                    // Reduce chasing time by 30%.
                    this.chasingTimeLeft *= 0.70f;
                }
                else
                {
                    this.chasingTimeLeft *= 0.9f;
                }

                this.stuckTimeLeft = StuckWaitTime;
                this.isStuck       = true;
            }
        }

        /// <summary>
        /// When the player has died the mobs should give up to chase
        /// and simply start to run around again.
        /// </summary>
        private void GiveUpTargetIsDeadOrGone()
        {
            this.state = State.Returned;
            this.Leave();
        }

        #region > Update Logic <

        /// <summary>
        /// Updates this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            ZeldaScene scene = this.enemy.Scene;
            if( scene == null )
                return;

            var target = this.Target;
            if( target == null || target.Statable.IsDead )
            {
                this.GiveUpTargetIsDeadOrGone();
                return;
            }

            Point2 enemyTilePosition = this.GetEnemyTilePosition();
            Point2 targetTilePosition = GetTilePosition( target );

            // Has the enemy reached the player?
            if( enemyTilePosition.X == targetTilePosition.X &&
                enemyTilePosition.Y == targetTilePosition.Y )
            {
                this.ContinueToChase();
                return;
            }

            if( this.isStuck )
            {
                this.stuckTimeLeft -= updateContext.FrameTime;

                if( this.stuckTimeLeft <= 0.0f )
                {
                    this.isStuck = false;
                    this.RecalculateChasingPath();
                }
            }

            if( this.UpdateChasing( updateContext ) )
            {
                return;
            }           

            // Find a path to the player (if don't have one yet).
            if( !this.pathFollower.HasPath || this.ExistingPathOutdatedTargetMovedAway( targetTilePosition ) )
            {
                int enemyFloor   = this.enemy.FloorNumber;
                var pathSearcher = scene.GetTilePathSearcher( enemyFloor );

                // Find path from the current position to target.
                var path = pathSearcher.FindPathTile<Moveable>(
                    enemyTilePosition.X,
                    enemyTilePosition.Y,
                    targetTilePosition.X,
                    targetTilePosition.Y,
                    this.moveable,
                    this.moveable.TileHandler
                );

                if( path.State == TilePathState.NotFound )
                {
                    this.OnNoPathFound();
                }

                this.pathFollower.Setup( this.enemy, path, scene.Map.GetFloor( enemyFloor ).ActionLayer );
            }

            this.FollowPath( updateContext );
        }

        /// <summary>
        /// Checks whether the currently existing path is considered to be 'outdated'.
        /// This might happen when the target has moved since the path has been calculated.
        /// </summary>
        /// <param name="targetTilePosition">
        /// The position of the target (in tile-space).
        /// </param>
        /// <returns>
        /// true if the path should be recreated;
        /// otherwise false.
        /// </returns>
        private bool ExistingPathOutdatedTargetMovedAway( Point2 targetTilePosition )
        {
            Point2 pathTargetTile = this.pathFollower.TargetTile;
            Point2 difference = targetTilePosition - pathTargetTile;

            int totalTilesOff = System.Math.Abs( difference.X ) + System.Math.Abs( difference.Y );
            return totalTilesOff >= 4;
        }

        /// <summary>
        /// Updates the chasing behaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        /// <returns>
        /// true if the script should cancel at this point;
        /// otherwise false.
        /// </returns>
        private bool UpdateChasing( ZeldaUpdateContext updateContext )
        {
            if( !this.IsChasingForever )
            {
                this.chasingTimeLeft -= updateContext.FrameTime;

                if( this.chasingTimeLeft <= 0.0 )
                {
                    // If not initialise the path.
                    if( this.isMovingBack )
                    {
                        // The enemy is already moving back to its original position.
                        this.pathFollower.Follow( updateContext );

                        if( this.pathFollower.IsAtEndOfPath )
                        {
                            if( this.GetEnemyTilePosition() == originalTile )
                            {
                                this.state = State.Returned;
                                this.Leave();
                                return true;
                            }
                        }                        
                    }
                    else
                    {
                        int enemyFloor = this.enemy.FloorNumber;
                        var enemyPosition = this.GetEnemyTilePosition();

                        var scene = this.enemy.Scene;
                        var pathSearcher = scene.GetTilePathSearcher( enemyFloor );

                        // Find path from the current position to original
                        var path = pathSearcher.FindPathTile<Moveable>(
                           enemyPosition.X,
                           enemyPosition.Y,
                           this.originalTile.X,
                           this.originalTile.Y,
                           this.moveable,
                           this.moveable.TileHandler
                        );

                        // Setup:
                        this.pathFollower.Setup( enemy, path, scene.Map.GetFloor( enemyFloor ).ActionLayer );
                        this.isMovingBack = true;
                        this.state = State.Returning;

                        // Apply Speed Modifier:
                        this.enemy.Statable.AuraList.Add( this.returningAura );
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Commands the enemy to follow the tile path that has been calculated-
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void FollowPath( ZeldaUpdateContext updateContext )
        {
            PathFollowState pathState = this.pathFollower.Follow( updateContext );

            if( this.pathFollower.IsAtEndOfPath )
            {
                this.OnReachedPathEnd();
                return;
            }

            switch( pathState )
            {
                case PathFollowState.Reached:
                    this.OnReachedPathEnd();
                    break;

                case PathFollowState.Stuck:
                case PathFollowState.HardStuck:
                    this.OnPathStuck( pathState );
                    break;

                default:
                case PathFollowState.Following:
                    this.isStuck = false;
                    break;
            }
        }
        
        /// <summary>
        /// Called when the enemy has followed the currently
        /// calculated path to the end.
        /// </summary>
        private void OnReachedPathEnd()
        {
            Point2 thisPosTile = this.GetEnemyTilePosition();
            Point2 targetPosTile = this.GetTargetTilePosition();

            if( thisPosTile.X != targetPosTile.X || thisPosTile.Y != targetPosTile.Y )
            {
                // Find a new path to the goal.
                pathFollower.ResetPath();
            }
        }

        /// <summary>
        /// Gets the current position of the enemy controlled
        /// by this EnemyChasePlayerBehaviour.
        /// </summary>
        /// <returns>
        /// The position of the chaser (in tile-space).
        /// </returns>
        private Point2 GetEnemyTilePosition()
        {
            return GetTilePosition( this.enemy );
        }

        /// <summary>
        /// Gets the current position of the target of the enemy
        /// controlled by this EnemyChasePlayerBehaviour.
        /// </summary>
        /// <returns>
        /// The position of the chased target (in tile-space).
        /// </returns>
        private Point2 GetTargetTilePosition()
        {
            return GetTilePosition( this.Target );
        }      
        
        /// <summary>
        /// Gets the tile position of the specified ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity whose position on the tile-map should be computed.
        /// </param>
        /// <returns></returns>
        private static Point2 GetTilePosition( ZeldaEntity entity )
        {
            Vector2 position = entity.Collision.Center;
            return new Point2( (int)(position.X / 16), (int)(position.Y / 16) );
        }

        #endregion

        #region > State <

        /// <summary>
        /// Enters this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;
            
            // Register events:
            enemy.FloorNumberChanged += eventHandlerEnemyChangedFloor;
            enemy.Attackable.Attacked += eventHandlerEnemyAttacked;

            enemy.Statable.AuraList.Add( chasingAura );
            
            originalTile.X = (int)enemy.Collision.Center.X / 16;
            originalTile.Y = (int)enemy.Collision.Center.Y / 16;
            
            if( enemy.AgressionType == AggressionType.Neutral )
            {
                enemy.AgressionType = AggressionType.Aggressive;
            }

            pathFollower.ResetPath();

            chasingTimeLeft = chasingTime;
            isMovingBack    = false;
            isStuck         = false;
            state           = State.Chasing;

            IsActive        = true;
        }

        /// <summary>
        /// Called when the Enemy entity leaves this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        public void Leave()
        {
            if( !IsActive )
                return;

            // Unregister events:
            enemy.FloorNumberChanged -= eventHandlerEnemyChangedFloor;
            enemy.Attackable.Attacked -= eventHandlerEnemyAttacked;

            if( enemy.AgressionType == AggressionType.Aggressive )
            {
                enemy.AgressionType = AggressionType.Neutral;
            }

            // Remove still active speed modifiers:
            enemy.Statable.AuraList.Remove( chasingAura );
            enemy.Statable.AuraList.Remove( returningAura );

            IsActive = false;
        }

        /// <summary>
        /// Resets this IEntityBehaviour.
        /// </summary>
        public void Reset()
        {
            pathFollower.ResetPath();

            chasingTimeLeft = chasingTime;
            isMovingBack    = false;
            isStuck         = false;
        }

        #endregion

        #region > Events <

        /// <summary>
        /// Gets called when the enemy has changed floor during chasing the player.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The Atom.ChangedValue{Int32} that contains the event data.</param>
        private void OnEnemyChangedFloor( object sender, Atom.ChangedValue<int> e )
        {
            this.RecalculateChasingPath();
        }

        /// <summary>
        /// Gets called when the enemy has been attacked (by the player).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contains the event data.</param>
        private void OnEnemyAttacked( object sender, AttackEventArgs e )
        {
            // following line is not needed because enemies currently can only be attacked by the Player
            // if( e.Attacker == enemy.Scene.Player )
            this.ContinueToChase();
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 2;
            context.Write( Version );

            context.Write( this.chasingTime );
            context.Write( this.IsChasingForever );
            context.Write( this.ChasingSpeedModifier ); // new in 2.
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.chasingTime = context.ReadSingle();
            this.IsChasingForever = context.ReadBoolean();
            this.ChasingSpeedModifier = context.ReadSingle();            
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="EnemyChasePlayerBehaviour"/>
        /// for the given <see cref="Enemy"/> entity.
        /// </summary>
        /// <param name="newOwner">
        /// The entity that wants to be controlled by the newly cloned IEntityBehaviour.
        /// </param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new EnemyChasePlayerBehaviour( (Enemy)newOwner );

            SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="EnemyChasePlayerBehaviour"/> to be a clone of this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        /// <param name="clone">The EnemyChasePlayerBehaviour to setup as a clone of this EnemyChasePlayerBehaviour.</param>
        public void SetupClone( EnemyChasePlayerBehaviour clone )
        {
            clone.chasingTime      = chasingTime;
            clone.IsChasingForever = IsChasingForever;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the Enemy entity that goes after the player.
        /// </summary>
        private readonly Enemy enemy;

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of the Enemy entity.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        /// <summary>
        /// The current state of this <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        private State state = State.Chasing;

        /// <summary>
        /// The time the Enemy continues to chase the player.
        /// </summary>
        private float chasingTime = 16.0f;

        /// <summary>
        /// The time left the enemy will continue chasing after the player.
        /// </summary>
        private double chasingTimeLeft;

        /// <summary>
        /// The time left until the enemy will continue to try 
        /// to chase the player after going stuck.
        /// </summary>
        private double stuckTimeLeft;

        /// <summary>
        /// The position of the object when it started to chase the player.
        /// </summary>
        private Point2 originalTile;

        /// <summary>
        /// States whether the object is currently stuck.
        /// </summary>
        private bool isStuck;

        /// <summary> 
        /// States whether the Enemy gave up on chasing the player,
        /// and is currently returning back to it's original position. 
        /// </summary>
        private bool isMovingBack;

        /// <summary>
        /// The <see cref="TilePathFollower"/> which provides the mechanism
        /// of following the tile path from the current position of the enemy to the position of the player.
        /// </summary>
        private readonly TilePathFollower pathFollower = new TilePathFollower();

        /// <summary>
        /// Identifies the MovementSpeedEffect that is applied to the enemy
        /// when he chases the enemy.
        /// </summary>
        private readonly MovementSpeedEffect chasingSpeedEffect;

        /// <summary>
        /// The modifiers applied to the enemy while he chasing the player.
        /// </summary>
        private readonly PermanentAura chasingAura;

        /// <summary>
        /// The modifiers applied to the enemy while he is returning to his original position.
        /// </summary>
        private readonly PermanentAura returningAura;

        /// <summary>
        /// The eventhandler that gets invoked when the Enemy has changed Floor while chasing the player.
        /// </summary>
        private readonly Atom.RelaxedEventHandler<Atom.ChangedValue<int>> eventHandlerEnemyChangedFloor;

        /// <summary>
        /// The event handler that gets invoked when the enemy gets attacked (by the player).
        /// </summary>
        private readonly Atom.RelaxedEventHandler<AttackEventArgs> eventHandlerEnemyAttacked;

        #endregion

        #region enum State

        /// <summary>
        /// Enumerates the different states of the <see cref="EnemyChasePlayerBehaviour"/>.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// The enemy is chasing the player.
            /// </summary>
            Chasing,

            /// <summary>
            /// The enemy gave up on chasing the player and
            /// is now returning to its original position.
            /// </summary>
            Returning,

            /// <summary>
            /// The enemy gave up on chasing the player and
            /// has returned back to its original position.
            /// </summary>
            Returned
        }

        #endregion
    }
}