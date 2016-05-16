
namespace Zelda.Entities.Behaviours
{
    using System;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Zelda.Entities.Components;
    using Zelda.Timing;

    /// <summary>
    /// Implements the movement logic of the fairy.
    /// </summary>
    public sealed class FairyMovementBehaviour : IEntityBehaviour
    {
        private static readonly Vector2 OffsetStrength = new Vector2( 18, 20 );
        private static readonly FloatRange TargetOffsetTimeChange = new FloatRange( 1, 5.5f );
        private const float DistanceDirectMovement = 5000.0f;
        private const float DistanceTeleport = 100000.0f;

        private ZeldaEntity TargetEntity
        {
            get
            {
                return this.fairy.Owner;
            }
        }

        public bool IsActive
        {
            get;
            private set;
        }

        public FairyMovementBehaviour( Fairy fairy, IZeldaServiceProvider serviceProvider )
        {
            this.fairy = fairy;
            this.rand = serviceProvider.Rand;

            this.timerTargetOffset.Ended += this.OnTimerTargetOffsetEnded;
            this.timerTargetOffset.Setup( serviceProvider );

            this.timerTargetPosition.Ended += this.OnTimerTargetPositionEnded;
        }

        private void RefreshTargetEntityPosition()
        {
            this.targetEntityPosition = this.TargetEntity.Collision.Center;
            this.timerTargetPosition.Reset();

            this.RefreshTargetPosition();
        }

        private void RefreshTargetPosition()
        {
            this.targetPosition = Vector2.Floor( this.targetEntityPosition + this.targetOffset );
            this.targetTilePosition = new Point2(  (int)(this.targetEntityPosition.X / 16.0f), (int)(this.targetEntityPosition.Y / 16.0f) );
        }

        public void Update( ZeldaUpdateContext updateContext )
        {
            this.timerTargetPosition.Update( updateContext );
            this.timerTargetOffset.Update( updateContext );

            Vector2 actualPosition = this.fairy.Collision.Center;
            Vector2 delta = this.targetEntityPosition - actualPosition;
            float distance = delta.SquaredLength;

            if( distance >= DistanceTeleport )
            {
                this.TeleportToOwner();
            }
            else
            {
                if( distance <= DistanceDirectMovement )
                {
                    ResetMovementSpeed();

                    if( !(actualPosition.IsApproximate( targetPosition, 5.0f ) ) )
                    {
                        this.fairy.Moveable.MoveToCenter( targetPosition, updateContext.FrameTime );
                    }

                    this.pathFollower.ResetPath();
                }
                else
                {
                    if( !this.pathFollower.HasPath || this.ExistingPathOutdatedTargetMovedAway() )
                    {
                        SetupPath();
                    }

                    FollowPath( updateContext );
                    FindMovementSpeedAt( distance );
                }
            }
        }

        private void FindMovementSpeedAt( float distance )
        {
            float bonus = distance / (32.0f * 32.0f);
            SetMovementSpeed( bonus );
        }

        private void ResetMovementSpeed()
        {
            SetMovementSpeed( 0.0f );
        }

        private void SetMovementSpeed( float speedBonus )
        {
            fairy.Moveable.Speed = Fairy.DefaultMovementSpeed + speedBonus;
        }

        private bool ExistingPathOutdatedTargetMovedAway()
        {
            Point2 difference = this.lastTargetTile - this.pathFollower.TargetTile;

            int totalTilesOff = System.Math.Abs( difference.X ) + System.Math.Abs( difference.Y );
            return totalTilesOff >= 4;
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

                default:
                    break;
            }
        }
   
        private void SetupPath()
        {
            var scene = this.fairy.Scene;
            int fairyFloor = this.fairy.FloorNumber;
            var pathSearcher = scene.GetTilePathSearcher( fairyFloor );

            var fairyTilePosition = (Point2)(fairy.Collision.Center / 16.0f);

            // Find path from the current position to target.
            var path = pathSearcher.FindPathTile<Moveable>(
                fairyTilePosition.X,
                fairyTilePosition.Y,
                this.targetTilePosition.X,
                this.targetTilePosition.Y,
                this.fairy.Moveable,
                this.fairy.Moveable.TileHandler
            );

            if( path.State == TilePathState.NotFound )
            {
                this.OnNoPathFound();
            }
            else
            {
                this.lastTargetTile = this.targetTilePosition;
                this.pathFollower.Setup( this.fairy, path, scene.Map.GetFloor( fairyFloor ).ActionLayer );
            }
        }

        private void OnNoPathFound()
        {
            this.targetOffset = Vector2.Zero;
            this.RefreshTargetPosition();
            this.pathFollower.ResetPath();
        }

        private void OnReachedPathEnd()
        {
            this.SetupPath();
        }    

        private void TeleportToOwner()
        {
            this.fairy.TeleportToOwner();

            this.ResetMovementSpeed();
            this.RefreshTargetEntityPosition();
            this.RefreshTargetPosition();
        }

        public void Enter()
        {
            this.IsActive = true;
        }

        public void Leave()
        {
            this.IsActive = false;
        }

        public void Reset()
        {
            this.timerTargetPosition.TimeLeft = 0.0f;
            this.timerTargetOffset.Reset();
            this.targetOffset = Point2.Zero;
        }

        private void OnTimerTargetOffsetEnded( ITimer sender )
        {
            Vector2 factor = new Vector2( rand.RandomRange( -1.0f, 1.0f ), rand.RandomRange( -1.0f, 1.0f ) );
            this.targetOffset = factor * OffsetStrength;

            this.timerTargetOffset.Reset();
            this.RefreshTargetPosition();
        }

        private void OnTimerTargetPositionEnded( ITimer sender )
        {
            this.RefreshTargetEntityPosition();
        }

        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            throw new NotSupportedException();
        }

        public void Serialize( Saving.IZeldaSerializationContext context )
        {
            throw new NotSupportedException();
        }

        public void Deserialize( Saving.IZeldaDeserializationContext context )
        {
            throw new NotSupportedException();
        }

        private Vector2 targetPosition;
        private Vector2 targetEntityPosition;
        private Vector2 targetOffset;

        private ResetableTimer timerTargetPosition = new ResetableTimer() { Time = 1 };
        private ResetableRangeTimer timerTargetOffset = new ResetableRangeTimer() { TimeRange = TargetOffsetTimeChange };
        
        /// <summary>
        /// The <see cref="TilePathFollower"/> which provides the mechanism
        /// of following the tile path from the current position of the fairy to the position of the player.
        /// </summary>
        private readonly TilePathFollower pathFollower = new TilePathFollower();

        private readonly IRand rand;
        private readonly Fairy fairy;
        private Point2 targetTilePosition;
        private Point2 lastTargetTile;
    }
}
