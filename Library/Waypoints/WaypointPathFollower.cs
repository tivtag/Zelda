// <copyright file="WaypointPathFollower.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.WaypointPathFollower class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Waypoints
{
    using Atom;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WaypointPathFollower : IZeldaUpdateable
    {
        /// <summary>
        /// Raised when the entity that currently uses this WaypointPathFollower
        /// has reached a Waypoint.
        /// </summary>
        public event RelaxedEventHandler<WaypointPathFollower, ZeldaWaypoint> WaypointReached;

        /// <summary>
        /// Raised when the entity has reached the end of the Path.
        /// </summary>
        public event SimpleEventHandler<WaypointPathFollower> EndReached;

        /// <summary>
        /// Gets or sets a value indicating whether when the entity has reached the end of the
        /// path it should start over at the first Waypoint.
        /// </summary>
        /// <remarks>
        /// This works only on circular paths.
        /// </remarks>
        public bool IsLooping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is actively following the path.
        /// </summary>
        public bool IsActive 
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the WaypointPathFollower class.
        /// </summary>
        public WaypointPathFollower()
        {
            this.IsActive = true;
        }

        /// <summary> 
        /// Setups this <see cref="WaypointPathFollower"/> for a new following task.
        /// </summary>
        /// <param name="entity">
        /// The IMoveableEntity that should follow the path.
        /// </param>
        /// <param name="path">
        /// The path to follow.
        /// </param>
        public void Setup( IMoveableEntity entity, ZeldaPath path )
        {
            this.path = path;
            this.entity = entity;

            if( entity != null && path != null && path.Length > 0 )
            {
                this.moveable = entity.Moveable;

                this.previousWaypoint = null;
                this.nextWaypointIndex = 0;
                this.nextWaypoint = path[0];
            }
            else
            {
                this.previousWaypoint = null;
                this.nextWaypointIndex = -1;
                this.nextWaypoint = null;
                this.moveable = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tilePath"></param>
        private void SetTilePath( TilePath tilePath )
        {
            var scene = this.entity.Scene;
            if( scene == null )
            {
                throw new InvalidOperationException( "The Entity must be part of a scene." );
            }

            var floor = scene.Map.GetFloor( this.entity.FloorNumber );

            if( floor != null )
            {
                this.tilePathFollower.Setup( entity, tilePath, floor.ActionLayer );
            }
            else
            {
                this.tilePathFollower.Setup( null, null, null );
            }
        }

        /// <summary>
        /// Attempts to find a path from the current location of the entity to the specified ZeldaWaypoint.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to move towards.
        /// </param>
        /// <returns>
        /// The TilePath that has been found.
        /// </returns>
        private TilePath FindPathFromCurrentLocationTo( ZeldaWaypoint waypoint )
        {
            var scene = this.entity.Scene;
            if( scene == null )
                return null;

            var pathSearcher = scene.GetTilePathSearcher( waypoint.FloorNumber );
            if( pathSearcher == null )
                return null;

            Vector2 center = this.entity.Collision.Center;
            return pathSearcher.FindPath( 
                (int)center.X,
                (int)center.Y, 
                (int)waypoint.Position.X,
                (int)waypoint.Position.Y,
                this.moveable,
                this.moveable.TileHandler
            );
        }

        /// <summary>
        /// Updates this WaypointPathFollower.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            if( this.tilePathFollower.HasPath )
            {
                PathFollowState followState = this.tilePathFollower.Follow( updateContext );

                switch( followState )
                {
                    case PathFollowState.HardStuck:
                    case PathFollowState.Stuck:
                        break;

                    case PathFollowState.Reached:
                        this.OnTilePathEndReached();
                        break;
                }
            }
            else
            {
                var tilePath = this.FindPathFromCurrentLocationTo( this.nextWaypoint );
                this.SetTilePath( tilePath );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnTilePathEndReached()
        {
            if( this.nextWaypoint != null )
            {
                this.WaypointReached.Raise( this, this.nextWaypoint );
                this.previousWaypoint = this.nextWaypoint;

                this.SelectNextWaypoint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectNextWaypoint()
        {
            if( this.nextWaypointIndex + 1 >= this.path.Length )
            {
                this.EndReached.Raise( this );

                if( this.IsLooping )
                {
                    this.nextWaypointIndex = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                ++this.nextWaypointIndex;
            }

            this.nextWaypoint = this.path[this.nextWaypointIndex];
            this.currentSegment = this.previousWaypoint.GetPathSegmentTo( this.nextWaypoint );
            this.FollowCurrentSegment();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FollowCurrentSegment()
        {
            if( this.currentSegment != null )
            {
                this.SetTilePath( this.currentSegment.GetTilePath() );
            }
            else
            {
                this.SetTilePath( null );
            }
        }

        private ZeldaPath path;
        private IMoveableEntity entity;
        private Moveable moveable;

        private ZeldaWaypoint previousWaypoint;
        private ZeldaWaypoint nextWaypoint;
        private int nextWaypointIndex;
        private ZeldaPathSegment currentSegment;

        private readonly TilePathFollower tilePathFollower = new TilePathFollower();
    }
}
