// <copyright file="CooldownVisualizer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.CooldownVisualizer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    using Atom.Xna.Effects;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Graphics;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the visualization of <see cref="Cooldown"/>s.
    /// </summary>
    internal sealed class CooldownVisualizer : UIElement
    {
        #region [ Constants ]

        /// <summary>
        /// Represents the (unique) name that indentifies this <see cref="UIElement"/>.
        /// </summary>
        public const string ElementName = "CooldownVisualizer";

        /// <summary>
        /// The number of triangles needed for to draw a full cooldown.
        /// </summary>
        private const int TriangleCountForFullCooldown = 8;

        /// <summary>
        /// The maximum number of cooldowns that may be drawn at a time.
        /// </summary>
        private const int MaximumCooldownCount = 30;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownVisualizer"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal CooldownVisualizer( IZeldaServiceProvider serviceProvider )
            : base( ElementName )
        {
            Debug.Assert( serviceProvider != null );

            this.FloorNumber     = 1;
            this.serviceProvider = serviceProvider;
            this.graphicsDevice  = serviceProvider.Game.GraphicsDevice;
        }

        /// <summary>
        /// Loads the content used by this <see cref="CooldownVisualizer"/>.
        /// </summary>
        public void LoadContent()
        {
            var window  = this.serviceProvider.Game.Window;
            var effectLoader = this.serviceProvider.GetService<IEffectLoader>();

            this.effectSolidColorFill = effectLoader.Load( "SolidColor" );

            var resolutionService = this.serviceProvider.GetService<IResolutionService>();
            this.SetupProjection( resolutionService );
        }

        /// <summary>
        /// Setups the projection matrix that is used when rendering
        /// the cooldowns.
        /// </summary>
        /// <param name="resolutionService">
        /// Provides access to the current output resolution.
        /// </param>
        private void SetupProjection( IResolutionService resolutionService )
        {
            var size = resolutionService.ViewSize;
            var projection = Xna.Matrix.CreateOrthographicOffCenter( 0.0f, size.X, size.Y, 0.0f, 0.0f, 1.0f );
            this.effectSolidColorFill.Parameters["WorldViewProj"].SetValue( projection );
        }

        /// <summary>
        /// Unloads the content used by this <see cref="CooldownVisualizer"/>.
        /// </summary>
        public void UnloadContent()
        {
            effectSolidColorFill = null;
        }

        #endregion

        #region [ Methods ]

        #region > Push <

        /// <summary>
        /// Tells this CooldownVisualizer to draw the given <see cref="Cooldown"/> at the given position
        /// using the given settings at the next Draw call.
        /// </summary>
        /// <param name="cooldown">
        /// The <see cref="Cooldown"/> to visualize using this CooldownVisualizer.
        /// </param>
        /// <param name="position">
        /// The position to the draw the cooldown at.
        /// </param>
        /// <param name="drawSize">
        /// The drawing size of the cooldown.
        /// </param>
        /// <param name="color">
        /// The color of the cooldown.
        /// </param>
        /// <param name="isInversed">
        /// States whether the cooldown is filling up or decreasing over time.
        /// </param>
        internal void PushCooldown(
            Cooldown cooldown,
            Vector2  position,
            Vector2  drawSize,
            Xna.Color    color,
            bool     isInversed )
        {
            if( cooldown == null )
                return;

            this.PushCooldown(
                cooldown.TimeLeft,
                cooldown.TotalTime,
                position,
                drawSize,
                color,
                isInversed
            );
        }

        /// <summary>
        /// Tells this CooldownVisualizer to draw the given <see cref="Cooldown"/> at the given position
        /// using the given settings at the next Draw call.
        /// </summary>
        /// <param name="timeLeft">
        /// The time left until the cooldown ends.
        /// </param>
        /// <param name="totalTime">
        /// The total duration of the cooldown.
        /// </param>
        /// <param name="position">
        /// The position to the draw the cooldown at.
        /// </param>
        /// <param name="drawSize">
        /// The drawing size of the cooldown.
        /// </param>
        /// <param name="color">
        /// The color of the cooldown.
        /// </param>
        /// <param name="isInversed">
        /// States whether the cooldown is filling up or decreasing over time.
        /// </param>
        internal void PushCooldown( 
            float   timeLeft,
            float   totalTime,
            Vector2 position,
            Vector2 drawSize,
            Xna.Color   color, 
            bool    isInversed )
        {
            float ratio = timeLeft / totalTime;            
            if( isInversed )
                ratio = 1.0f - ratio;

            float widthHalf = drawSize.X / 2.0f, heightHalf = drawSize.Y / 2.0f;
            Vector2 center  = new Vector2( position.X + widthHalf, position.Y + heightHalf );
            Vector2 end     = new Vector2( position.X + drawSize.X, position.Y + drawSize.Y );

            if( ratio >= 0.0f )
            {
                float ratio2 = ratio >= 0.125f ? 1.0f : ratio / 0.125f;
                PushTriangle(
                    center,
                    new Vector2( center.X, position.Y ),
                    new Vector2( center.X + (widthHalf * ratio2), position.Y ),
                    color 
                );
            }

            if( ratio >= 0.125f )
            {
                float ratio2 = ratio >= 0.25f ? 1.0f : (ratio - 0.125f) / 0.125f;
                PushTriangle( 
                    center,
                    new Vector2( end.X, position.Y ), 
                    new Vector2( end.X, position.Y + (heightHalf * ratio2) ), 
                    color 
                );
            }

            if( ratio >= 0.25f )
            {
                float ratio2 = ratio >= 0.375f ? 1.0f : (ratio - 0.25f) / 0.125f;
                PushTriangle(
                    center,
                    new Vector2( end.X, center.Y ),
                    new Vector2( end.X, center.Y + (heightHalf * ratio2) ),
                    color 
                );
            }

            if( ratio >= 0.375f )
            {
                float ratio2 = ratio >= 0.5f ? 1.0f : (ratio - 0.375f) / 0.125f;
                PushTriangle( 
                    center,
                    new Vector2( end.X, end.Y ),
                    new Vector2( end.X - (widthHalf * ratio2), end.Y ), 
                    color 
                );
            }

            if( ratio >= 0.5f )
            {
                float ratio2 = ratio >= 0.625f ? 1.0f : (ratio - 0.5f) / 0.125f;
                PushTriangle( 
                    center,
                    new Vector2( center.X, end.Y ), 
                    new Vector2( center.X - (widthHalf * ratio2), end.Y ),
                    color 
                );
            }

            if( ratio >= 0.625f )
            {
                float ratio2 = ratio >= 0.75f ? 1.0f : (ratio - 0.625f) / 0.125f;
                PushTriangle( 
                    center, 
                    new Vector2( position.X, end.Y ),
                    new Vector2( position.X, end.Y - (heightHalf * ratio2) ), 
                    color 
                );
            }

            if( ratio >= 0.75f )
            {
                float ratio2 = ratio >= 0.875f ? 1.0f : (ratio - 0.75f) / 0.125f;
                PushTriangle( 
                    center,
                    new Vector2( position.X, center.Y ),
                    new Vector2( position.X, center.Y - (heightHalf * ratio2) ),
                    color
                );
            }

            if( ratio >= 0.875f )
            {
                float ratio2 = ratio >= 1.0f ? 1.0f : (ratio - 0.875f) / 0.125f;
                PushTriangle( 
                    center, 
                    new Vector2( position.X, position.Y ),
                    new Vector2( position.X + (widthHalf * ratio2), position.Y ), 
                    color 
                );
            }
        }

        /// <summary>
        /// Pushes a new triangle onto the vertex stack.
        /// </summary>
        /// <param name="vertexA">The first vertex.</param>
        /// <param name="vertexB">The second vertex.</param>
        /// <param name="vertexC">The third vertex.</param>
        /// <param name="color">The color of the triangle.</param>
        private void PushTriangle( Vector2 vertexA, Vector2 vertexB, Vector2 vertexC, Xna.Color color )
        {
            if( vertexIndex + 3 <= vertices.Length )
            {
                vertices[vertexIndex++] = new VertexPositionColor( new Microsoft.Xna.Framework.Vector3( vertexA.X, vertexA.Y, 0.0f ), color );
                vertices[vertexIndex++] = new VertexPositionColor( new Microsoft.Xna.Framework.Vector3( vertexB.X, vertexB.Y, 0.0f ), color );
                vertices[vertexIndex++] = new VertexPositionColor( new Microsoft.Xna.Framework.Vector3( vertexC.X, vertexC.Y, 0.0f ), color );

                ++triangleCount;
            }
            else
            {
                serviceProvider.Log.Write( "Warning - CooldownVisualizer: Reached triangle limit." );
            }
        }

        #endregion

        #region > Drawing <

        /// <summary>
        /// Called when this <see cref="UIElement"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( triangleCount > 0 )
            {
                
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                                
                foreach( EffectPass pass in this.effectSolidColorFill.CurrentTechnique.Passes )
                {
                    pass.Apply();

                    graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList,
                        vertices,
                        0,
                        triangleCount
                    );
                }
                
                vertexIndex   = 0;
                triangleCount = 0;
            }
        }

        #endregion

        /// <summary>
        /// Called when this <see cref="UIElement"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }
        
        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the next index to be written at.
        /// </summary>
        private int vertexIndex;

        /// <summary>
        /// The number of triangles that have been pushed into the CooldownVisualizer.
        /// </summary>
        private int triangleCount;

        /// <summary>
        /// The vertices that have been set to be send to the graphics hardware.
        /// </summary>
        private readonly VertexPositionColor[] vertices = new VertexPositionColor[3 * TriangleCountForFullCooldown * MaximumCooldownCount];

        /// <summary>
        /// The effect which is used to draw the Cooldowns.
        /// </summary>
        private Effect effectSolidColorFill;
        
        /// <summary>
        /// The XNA GraphicsDevice.
        /// </summary>
        private readonly GraphicsDevice graphicsDevice;

        /// <summary>
        /// Provides fast access to game related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
