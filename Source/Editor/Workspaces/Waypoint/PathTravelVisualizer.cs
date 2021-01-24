// <copyright file="PathTravelVisualizer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.PathTravelVisualizer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Waypoint
{
    using Atom.Math;
    using Atom.Waypoints;
    using Atom.Xna;

    /// <summary>
    /// Implements a mechanism that visualizes an object travelling on a Path.
    /// </summary>
    public sealed class PathTravelVisualizer : IContentLoadable, IZeldaUpdateable
    {
        /// <summary>
        /// Gets or sets the Path this PathTravelVisualizer is currently visualizing.
        /// </summary>
        public Path Path
        {
            get
            {
                return this._path;
            }

            set
            {
                if( value == this.Path )
                    return;

                this._path = value;

                this.segmentFactor = 0.0f;
                this.waypointIndex = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PathTravelVisualizer class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PathTravelVisualizer( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider; 
        }

        /// <summary>
        /// Loads the content this PathTravelVisualizer requires.
        /// </summary>
        public void LoadContent()
        {
            this.sprite = serviceProvider.SpriteLoader.LoadSprite( "SmallSkeletonHead" );
        }
        
        /// <summary>
        /// Updates this PathTravelVisualizer.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.Path == null || this.Path.Length <= 1 )
                return;

            const float Speed = 0.25f;

            this.segmentFactor += updateContext.FrameTime * Speed;

            if( this.segmentFactor > 1.0f )
            {
                ++this.waypointIndex;

                if( this.waypointIndex >= this.Path.Length - 1 )
                {
                    this.waypointIndex = 0;
                }

                this.segmentFactor = 0.0f;
            }

            Waypoint start = this.Path[this.waypointIndex];
            Waypoint end = this.Path[this.waypointIndex + 1];
            PathSegment segment = start.GetPathSegmentTo( end );
            if( segment == null )
                return;

            bool mustInvert = segment.From == end;
            float factor = mustInvert ? 1.0f - this.segmentFactor : this.segmentFactor;

            this.location = segment.Line.GetPointOnSegment( factor );
            this.location.X = (int)this.location.X;
            this.location.Y = (int)this.location.Y;
        }

        /// <summary>
        /// Draws this PathTravelVisualizer.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        public void Draw( ISpriteDrawContext drawContext )
        {
            if( this.Path == null || this.Path.Length <= 1 )
                return;

            this.sprite.Draw( this.location, drawContext.Batch );
        }

        /// <summary>
        /// The index of the current Waypoint.
        /// </summary>
        private int waypointIndex;

        /// <summary>
        /// A value between [0;1] that represents the time that has been travelled from the current Waypoint
        /// to the next Waypoint.
        /// </summary>
        private float segmentFactor;

        /// <summary>
        /// The location the sprite is drawn at.
        /// </summary>
        private Vector2 location;

        /// <summary>
        /// Represents the Sprite that is used to visualize the traveling object.
        /// </summary>
        private Sprite sprite;

        /// <summary>
        /// Represents the storage field of the Path property.
        /// </summary>
        private Path _path;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
