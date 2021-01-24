// <copyright file="PathFollower.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.PathFollower class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.Diagnostics;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Zelda.Entities;
    using Zelda.Entities.Components;

    /// <summary>
    /// Provides a mechanism for a <see cref="Moveable"/> <see cref="ZeldaEntity"/> to follow a <see cref="TilePath"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class TilePathFollower
    {
        #region [ Constants ]
        
        /// <summary>
        /// The time an entity that hasn't moved until it is considered to be 'stuck'.
        /// </summary>
        private const float TimeUnmovedUntilDeclaredStuck = 2.0f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether to follow the current path.
        /// </summary>
        public bool FollowPath
        {
            get 
            { 
                return this.followPath;
            }
            
            set
            {
                this.followPath = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ZeldEntity that is using this PathFollower
        /// has reached the end of the path.
        /// </summary>
        public bool IsAtEndOfPath
        {
            get 
            {
                return this.path == null || this.pathIndex + 1 >= this.path.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there exists
        /// a path this PathFollower can follow.
        /// </summary>
        public bool HasPath
        {
            get
            {
                return this.path != null;
            }
        }

        /// <summary>
        /// Gets the position of the tile the current path ends at.
        /// </summary>
        public Point2 TargetTile
        {
            get
            {
                return this.targetTile;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary> 
        /// Setups this <see cref="TilePathFollower"/> for a new following task.
        /// </summary>
        /// <param name="entity">
        /// The IMoveableEntity that should follow the path.
        /// </param>
        /// <param name="path">
        /// The path to follow.
        /// </param>
        /// <param name="actionLayer">
        /// The layer the PathFollowerer should operate on.
        /// </param>
        public void Setup( IMoveableEntity entity, TilePath path, TileMapDataLayer actionLayer )
        {
            Debug.Assert( entity != null );
            Debug.Assert( path != null );
            Debug.Assert( actionLayer != null );

            this.path = path;
            this.pathIndex = 0;

            if( path != null && path.State == TilePathState.Found && path.Length > 0 )
                this.targetTile = path[path.Length - 1];
            else
                this.targetTile = new Point2( -1, -1 );

            this.stuckTimer = 0.0f;
            this.oldTilePosition = Point2.Zero;

            this.entity = entity;
            this.moveable = entity.Moveable;
            this.actionLayer = actionLayer;
        }

        /// <summary> 
        /// When called the <see cref="TilePathFollower"/> tries to
        /// tell the set ZeldaEntity how to follow the set Path.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        /// <returns>
        /// Returns the current state of this PathFollower.
        /// </returns>
        public PathFollowState Follow( ZeldaUpdateContext updateContext )
        {
            if( this.ShouldFollowPath() && this.HasNotReachedEndOfPath() )
            {
                Vector2 position = entity.Collision.Center;
                Point2 positionTile = new Point2(
                    (int)(position.X / 16),
                    (int)(position.Y / 16)
                );

                if( this.UpdateStuckDetection( positionTile, updateContext ) )
                    return PathFollowState.Stuck;

                Point2 next = this.path[this.pathIndex + 1];
                if( positionTile.X == next.X && positionTile.Y == next.Y )
                    ++this.pathIndex;

                int nextTileId = this.actionLayer.GetTileAt( next.X, next.Y );
                if( this.moveable.TileHandler.IsWalkable( nextTileId, moveable ) == false )
                    return PathFollowState.HardStuck;

                Direction8 dir = path.GetDirToNext( positionTile.X, positionTile.Y, this.pathIndex );
                this.moveable.MoveDir( dir, updateContext.FrameTime );

                return PathFollowState.Following;
            }

            return PathFollowState.Reached;
        }

        /// <summary>
        /// Gets a value indicating whether this PathFollower has
        /// not reached the end of the path yet.
        /// </summary>
        /// <returns>
        /// true if it has not reached the end of the path yet and should continue to follow it;
        /// otherwise false.
        /// </returns>
        private bool HasNotReachedEndOfPath()
        {
            int pathLength = this.path.Length;
            return this.pathIndex >= 0 && this.pathIndex < pathLength && this.pathIndex + 1 != pathLength;
        }

        /// <summary>
        /// Gets a value indicating whether the PathFollower should continue
        /// to follow the current path.
        /// </summary>
        /// <returns>
        /// true if it should continue;
        /// otherwise false.
        /// </returns>
        private bool ShouldFollowPath()
        {
            if( this.path == null || this.entity == null )
                return false;

            return this.followPath && this.moveable.CanMove;
        }

        /// <summary>
        /// Updates the logic that tries to detect whether the entity following
        /// the path got stuck.
        /// </summary>
        /// <param name="tilePosition">
        /// The position of the entity that is following the path (in tile-space).
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        /// <returns>
        /// true if the entity is considered to be stuck;
        /// otherwise false.
        /// </returns>
        private bool UpdateStuckDetection( Point2 tilePosition, ZeldaUpdateContext updateContext )
        {
            if( this.oldTilePosition == tilePosition )
            {
                this.stuckTimer += updateContext.FrameTime;

                if( this.stuckTimer >= TimeUnmovedUntilDeclaredStuck )
                {
                    return true;
                }
            }
            else
            {
                this.stuckTimer = 0.0f;
            }

            this.oldTilePosition = tilePosition;
            return false;
        }

        /// <summary>
        /// Tells this PathFollower to follow no path.
        /// </summary>
        public void ResetPath()
        {
            this.path = null;
            this.pathIndex = 0;
            this.stuckTimer = 0.0f;
            this.targetTile = new Point2( -1, -1 );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The ZeldaEntity that follows the path.
        /// </summary>
        private IMoveableEntity entity;

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of the ZeldaEntity.
        /// </summary>
        private Moveable moveable;

        /// <summary>
        /// The underlying collision layer.
        /// </summary>
        private TileMapDataLayer actionLayer;

        /// <summary>
        /// The path to follow.
        /// </summary>
        private TilePath path;

        /// <summary>
        /// The tile at which the current path ends.
        /// </summary>
        private Point2 targetTile;

        /// <summary>
        /// The location of the moveable Entity in the path.
        /// </summary>
        private int pathIndex;

        /// <summary>
        /// Indicates whether to follow the current path.
        /// </summary>
        private bool followPath = true;

        /// <summary>
        /// The time an entity hasn't moved.
        /// </summary>
        private float stuckTimer;

        /// <summary>
        /// The old position of the entity.
        /// </summary>
        private Point2 oldTilePosition;

        #endregion
    }
}
