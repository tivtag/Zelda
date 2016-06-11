// <copyright file="WaypointMapDrawer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Waypoint.WaypointMapDrawer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Waypoint
{
    using Atom.Math;
    using Atom.Waypoints;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Graphics;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements a mechanism that visualizes a <see cref="WaypointMapViewModel"/>.
    /// </summary>
    internal sealed class WaypointMapDrawer : IContentLoadable
    {
        /// <summary>
        /// Gets or sets a value indicating whether all underlying TilePaths should be shown.
        /// </summary>
        public bool ShowAllTilePaths
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an object travelling the currently selected Path
        /// should be shown.
        /// </summary>
        public bool ShowPathTraveller 
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the <see cref="WaypointMapViewModel"/> this WaypointMapDrawer should visualize.
        /// </summary>
        public WaypointMapViewModel WaypointMap
        {
            get
            {
                return this.waypointMap;
            }

            set
            {
                if( this.waypointMap != null )
                {
                }

                this.waypointMap = value;

                if( this.waypointMap != null )
                {
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaypointWorkspace"/> class.
        /// </summary>
        /// <param name="application">
        /// Provides access to XNA related objects of the Editor application.
        /// </param>
        public WaypointMapDrawer( XnaEditorApp application )
        {
            this.application = application;
            this.pathTravelVisualizer = new PathTravelVisualizer( EditorApp.Current );
        }

        /// <summary>
        /// Loads the content this WaypointWorkspace requires.
        /// </summary>
        public void LoadContent()
        {
            this.pathTravelVisualizer.LoadContent();
            this.circleSprite = this.application.SpriteLoader.LoadSprite( "Circle_16" );
        }

        /// <summary>
        /// Updates this WaypointMapDrawer.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.waypointMap == null )
                return;

            if( this.ShowPathTraveller && this.waypointMap.SelectedPath != null )
            {
                this.pathTravelVisualizer.Path = waypointMap.SelectedPath.Model;
                this.pathTravelVisualizer.Update( updateContext );
            }
        }

        /// <summary>
        /// Draws the current <see cref="WaypointMapViewModel"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.waypointMap == null )
                return;

            var batch = drawContext.Batch;
            var camera = waypointMap.Model.Scene.Camera;

            drawContext.Begin( BlendState.NonPremultiplied, SamplerState.PointWrap, SpriteSortMode.Deferred, camera.Transform );
            {
                this.DrawSegments( waypointMap, drawContext );                
                this.DrawSelectedSegment( waypointMap, drawContext );
                this.DrawSelectedPath( waypointMap, drawContext );

                // Draw waypoints.
                foreach( var waypoint in waypointMap.Waypoints )
                {
                    this.DrawWaypoint( waypoint, new Xna.Color( 255, 255, 255, 180 ), batch );
                }

                if( waypointMap.SelectedWaypoint != null )
                {
                    this.DrawWaypoint( waypointMap.SelectedWaypoint, new Xna.Color( 255, 0, 0, 255 ), batch );
                }
            }
            drawContext.End();
        }

        /// <summary>
        /// Draws the currently selected path.
        /// </summary>
        /// <param name="waypointMap">
        /// The WaypointMapViewModel to visualize.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        private void DrawSelectedPath( WaypointMapViewModel waypointMap, ZeldaDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            var pathViewModel = waypointMap.SelectedPath;
            if( pathViewModel == null )
                return;

            var path = pathViewModel.Model;
            var start = path.Start;
            if( start == null )
                return;
            
            // Draw path segments.
            {
                Waypoint currentWaypoint = start;

                for( int index = 1; index < path.Length; ++index )
                {
                    Waypoint waypoint = path[index];
                    PathSegment segment = currentWaypoint.GetPathSegmentTo( waypoint );

                    this.DrawSegment( segment, Xna.Color.Yellow, batch );
                    currentWaypoint = waypoint;
                }
            }

            // Draw waypoints.
            {
                var end = path.End;
                if( start == end && start != null )
                {
                    this.DrawWaypoint( start, Xna.Color.Black, 1.0f, batch );
                }
                else
                {
                    if( start != null )
                    {
                        this.DrawWaypoint( start, Xna.Color.Green, 1.0f, batch );
                    }

                    if( end != null )
                    {
                        this.DrawWaypoint( end, Xna.Color.Yellow, 1.0f, batch );
                    }
                }

                if( pathViewModel.SelectedWaypoint != null )
                {
                    this.DrawWaypoint( pathViewModel.SelectedWaypoint.Model, Xna.Color.Red, 1.5f, batch );
                }
            }

            this.pathTravelVisualizer.Path = path;

            if( this.ShowPathTraveller )
            {
                this.pathTravelVisualizer.Draw( drawContext );
            }
        }

        /// <summary>
        /// Draws the individual PathSegments of the specified WaypointMapViewModel.
        /// </summary>
        /// <param name="waypointMap">
        /// The WaypointMapViewModel to visualize.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        private void DrawSegments( WaypointMapViewModel waypointMap, ZeldaDrawContext drawContext )
        {
            ITextureDrawBatch lineBatch = drawContext.Batch;

            if( this.ShowAllTilePaths )
            {
                int colorIndex = 0;

                foreach( var segment in waypointMap.PathSegments )
                {
                    ++colorIndex;

                    if( colorIndex > 5 )
                    {
                        colorIndex = 0;
                    }

                    Xna.Color color;

                    switch( colorIndex )
                    {
                        case 0:
                            color = new Xna.Color( 255, 0, 0, 100 );
                            break;
                        case 1:
                            color = new Xna.Color( 0, 255, 0, 100 );
                            break;
                        case 2:
                            color = new Xna.Color( 0, 0, 255, 100 );
                            break;
                        case 3:
                            color = new Xna.Color( 255, 255, 0, 100 );
                            break;
                        case 4:
                            color = new Xna.Color( 255, 255, 255, 100 );
                            break;
                        default:
                            color = new Xna.Color( 255, 0, 255, 100 );
                            break;
                    }

                    this.pathDrawer.Draw( segment.Model.GetTilePath(), color, drawContext );
                    this.DrawSegment( segment, new Xna.Color( 255, 255, 255, 180 ), lineBatch );
                }
            }
            else
            {
                foreach( var segment in waypointMap.PathSegments )
                {
                    this.DrawSegment( segment, new Xna.Color( 255, 255, 255, 180 ), lineBatch );
                }
            }
        }

        /// <summary>
        /// Draws the specified PathSegmentViewModel to the lineBatch using the specified color.
        /// </summary>
        /// <param name="segment">
        /// The segment to draw.
        /// </param>
        /// <param name="color">
        /// The color of the segment to draw.
        /// </param>
        /// <param name="lineBatch">
        /// Provides a mechanism that allows drawing of simple lines.
        /// </param>
        private void DrawSegment( PathSegmentViewModel segment, Xna.Color color, ITextureDrawBatch lineBatch )
        {
            this.DrawSegment( segment.Model, color, lineBatch );
        }

        /// <summary>
        /// Draws the specified PathSegment to the lineBatch using the specified color.
        /// </summary>
        /// <param name="segment">
        /// The segment to draw.
        /// </param>
        /// <param name="color">
        /// The color of the segment to draw.
        /// </param>
        /// <param name="lineBatch">
        /// Provides a mechanism that allows drawing of simple lines.
        /// </param>
        private void DrawSegment( PathSegment segment, Xna.Color color, ITextureDrawBatch lineBatch )
        {
            if( segment != null )
            {
                lineBatch.DrawLine( segment.From.Position, segment.To.Position, color );
            }
        }

        /// <summary>
        /// Draws the currently selected PathSegment.
        /// </summary>
        /// <param name="waypointMap">
        /// The WaypointMapViewModel to visualize.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        private void DrawSelectedSegment( WaypointMapViewModel waypointMap, ZeldaDrawContext drawContext )
        {
            var segment = waypointMap.SelectedPathSegment;

            if( segment != null )
            {
                this.pathDrawer.Draw( segment.Model.GetTilePath(), new Xna.Color( 255, 0, 0, 155 ), drawContext );

                drawContext.Batch.DrawLine(
                    waypointMap.SelectedPathSegment.From.Position,
                    waypointMap.SelectedPathSegment.To.Position,
                    new Xna.Color( 255, 0, 0, 255 )
                );
            }
        }

        /// <summary>
        /// Draws the specified WaypointViewModel in the specified color.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to draw.
        /// </param>
        /// <param name="color">
        /// The color of the Waypoint.
        /// </param>
        /// <param name="batch">
        /// The ISpriteBatch that should be used to draw the Waypoint.
        /// </param>
        private void DrawWaypoint( WaypointViewModel waypoint, Xna.Color color, ISpriteBatch batch )
        {
            this.circleSprite.Draw(
                new Vector2(
                    waypoint.Position.X - this.circleSprite.Width / 2,
                    waypoint.Position.Y - this.circleSprite.Height / 2
                ),
                color,
                batch
            );
        }

        /// <summary>
        /// Draws the specified Waypoint in the specified color.
        /// </summary>
        /// <param name="waypoint">
        /// The Waypoint to draw.
        /// </param>
        /// <param name="color">
        /// The color of the Waypoint.
        /// </param>
        /// <param name="scale">
        /// The scaling factor to apply.
        /// </param>
        /// <param name="batch">
        /// The ISpriteBatch that should be used to draw the Waypoint.
        /// </param>
        private void DrawWaypoint( Waypoint waypoint, Xna.Color color, float scale, ISpriteBatch batch )
        {
            this.circleSprite.Draw(
                new Vector2(
                    waypoint.Position.X - (this.circleSprite.Width / 2) * scale,
                    waypoint.Position.Y - (this.circleSprite.Height / 2) * scale
                ),
                color,
                0.0f,
                Vector2.Zero,
                new Vector2( scale, scale ),
                batch
            );
        }

        /// <summary>
        /// Represents the currently active WaypointMapViewModel.
        /// </summary>
        private WaypointMapViewModel waypointMap;

        /// <summary>
        /// Represents the Sprite that is used to draw the individual Waypoints.
        /// </summary>
        private Sprite circleSprite;

        /// <summary>
        /// Implements a mechanism that visualizes an object travelling on a Path.
        /// </summary>
        private readonly PathTravelVisualizer pathTravelVisualizer;

        /// <summary>
        /// Provides a mechanism that visualizes <see cref="TilePath"/>s.
        /// </summary>
        private readonly TilePathDrawer pathDrawer = new TilePathDrawer();

        /// <summary>
        /// Provides access to XNA related objects of the Editor application.
        /// </summary>
        private readonly XnaEditorApp application;
    }
}
