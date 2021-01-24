// <copyright file="ZeldaSceneDrawer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaSceneDrawer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Atom.Scene.Tiles;
    using Atom.Xna;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    
    /// <summary>
    /// Encapsulates the <see cref="ZeldaScene"/> drawing logic.
    /// </summary>
    public sealed class ZeldaSceneDrawer
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaSceneDrawer class.
        /// </summary>
        /// <param name="lightMap">
        /// The LightMap this ZeldaSceneDrawer should use during the light pass.
        /// </param>
        public ZeldaSceneDrawer( LightMap lightMap )
        {
            this.lightMap = lightMap;
        }

        /// <summary>
        /// Draws the specified <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="scene">
        /// The scene to draw.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaScene scene, ZeldaDrawContext drawContext )
        {
            this.SetupDraw( scene, drawContext );

            this.PreDraw();
            this.DrawUnlit();

            if( this.settings.IsLightingEnabled )
            {
                this.DrawLights();
            }

            this.DrawOverlays();
        }

        /// <summary>
        /// Setups this ZeldaSceneDrawer to draw the specified scene.
        /// </summary>
        /// <param name="scene">
        /// The scene to draw.
        /// </param>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        private void SetupDraw( ZeldaScene scene, ZeldaDrawContext drawContext )
        {
            this.scene = scene;
            this.settings = scene.Settings;
            this.drawContext = drawContext;
            this.drawContext.Camera = scene.Camera;
        }
        
        /// <summary>
        /// Calls the PreDraw method of all visible entities.
        /// </summary>
        private void PreDraw()
        {
            var floors = this.scene.Map.Floors;
            for( int floorIndex = 0; floorIndex < floors.Count; ++floorIndex )
            {
                var tag = floors[floorIndex].Tag as ZeldaTileMapFloorTag;

                if( tag != null )
                {
                    var drawables = tag.VisibleDrawables;
                    int drawablesCount = drawables.Count;

                    for( int i = 0; i < drawablesCount; ++i )
                    {
                        drawables[i].PreDraw( drawContext );
                    }
                }
            }
        }
        
        /// <summary>
        /// Draws the unlit scene to the current RenderTarget.
        /// </summary>
        private void DrawUnlit()
        {
            /*
               Drawing the scene works the following way:
                      
               for( int N = 0; N < FloorCount; ++N )
               {
                  - draw [TileMapFloor N]  
                  - draw [VisibleObjects on Floor N]
               }             
               - draw [Overlays] 
            */

            // Begin Drawing
            drawContext.Begin( BlendState.NonPremultiplied, SamplerState.PointClamp, SpriteSortMode.Immediate, scene.Camera.Transform );

            if( ZeldaScene.EntityEditMode )
            {
                this.ActuallyDrawUnlitEditMode();
            }
            else
            {
                this.ActuallyDrawUnlit();
            }                     

            // End Drawing
            drawContext.End();
        }

        /// <summary>
        /// Draws the unlit scene to the current RenderTarget.
        /// </summary>
        private void ActuallyDrawUnlit()
        {
            var camera = scene.Camera;
            int scrollX = (int)camera.Scroll.X;
            int scrollY = (int)camera.Scroll.Y;
            int viewWidth  = camera.ViewSize.X;
            int viewHeight = camera.ViewSize.Y;
            var floors = this.scene.Map.Floors;

            for( int floorIndex = 0; floorIndex < floors.Count; ++floorIndex )
            {
                // Draw Floor.
                var floor = floors[floorIndex];
                var layers = floor.Layers;

                for( int layerIndex = 0; layerIndex < layers.Count; ++layerIndex )
                {
                    var layer = (TileMapSpriteDataLayer)layers[layerIndex];
                    layer.DrawOffset(
                        scrollX,
                        scrollY,
                        viewWidth,
                        viewHeight,
                        scrollX,
                        scrollY,
                        drawContext
                    );
                }

                if( floor.ActionLayer.IsVisible )
                {
                    var layer = (TileMapSpriteDataLayer)floor.ActionLayer;

                    layer.DrawOffset(
                        scrollX,
                        scrollY,
                        viewWidth,
                        viewHeight,
                        scrollX,
                        scrollY,
                        drawContext
                    );
                }

                // Draw visible drawables of the Floor.
                var tag = floor.Tag as ZeldaTileMapFloorTag;

                if( tag != null )
                {
                    var drawables = tag.VisibleDrawables;
                    int drawablesCount = drawables.Count;

                    for( int i = 0; i < drawablesCount; ++i )
                    {
                        drawables[i].Draw( this.drawContext );
                    }
                }
            }
        }

        /// <summary>
        /// Draws the unlit scene to the current RenderTarget;
        /// prefering the drawing method of <see cref="IEditModeDrawable"/>s
        /// over the normal drawing method.
        /// </summary>
        /// <remarks>
        /// This is used for visualization required for the world editor.
        /// </remarks>
        private void ActuallyDrawUnlitEditMode()
        {
            var camera = scene.Camera;
            int scrollX = (int)camera.Scroll.X;
            int scrollY = (int)camera.Scroll.Y;
            int viewWidth  = camera.ViewSize.X;
            int viewHeight = camera.ViewSize.Y;
                        
            foreach( var floor in this.scene.Map.Floors )
            {
                // Draw Floor.
                foreach( TileMapSpriteDataLayer layer in floor.Layers )
                {
                    layer.DrawOffset(
                        scrollX,
                        scrollY,
                        viewWidth,
                        viewHeight,
                        scrollX,
                        scrollY,
                        drawContext
                    );
                }

                if( floor.ActionLayer.IsVisible )
                {
                    var layer = (TileMapSpriteDataLayer)floor.ActionLayer;

                    layer.DrawOffset(
                        scrollX,
                        scrollY,
                        viewWidth,
                        viewHeight,
                        scrollX,
                        scrollY,
                        drawContext
                    );
                }

                // Draw visible drawables of the Floor.
                var tag = floor.Tag as ZeldaTileMapFloorTag;

                if( tag != null )
                {
                    var drawables = tag.VisibleDrawables;
                    int drawablesCount = drawables.Count;

                    for( int i = 0; i < drawablesCount; ++i )
                    {
                        var drawable = drawables[i];
                        var editModeDrawable = drawable as IEditModeDrawable;

                        if( editModeDrawable != null )
                            editModeDrawable.DrawEditMode( drawContext );
                        else
                            drawable.Draw( drawContext );
                    }
                }
            }
        }

        /// <summary>
        /// Draws the lights in the scene and merges them
        /// with the previously drawn unlit scene.
        /// </summary>
        private void DrawLights()
        {
            if( scene.DayNightCycle.IsEnabled )
            {
                float alpha = scene.DayNightCycle.Alpha;
                byte value = (byte)(1 + (120 * alpha));
                byte extraBlue = (byte)(50 * (1.0f - alpha));

                // Note: Higher Values are too bright. May have to adjust this value in the feature.
                lightMap.AmbientColor = new Color( value, value, value + extraBlue, byte.MaxValue );
            }
            else
            {
                lightMap.AmbientColor = settings.AmbientColor;
            }

            // Let's begin drawing to the light map.
            lightMap.Begin();

            // Now draw all lights:
            drawContext.Begin( BlendState.NonPremultiplied, SamplerState.LinearClamp, SpriteSortMode.Deferred, scene.Camera.Transform );

            var visibleLights = scene.VisibleLights;
            int lightCount = visibleLights.Count;
            for( int i = 0; i < lightCount; ++i )
            {
                visibleLights[i].DrawLight( drawContext );
            }

            // We are finished drawing!
            drawContext.End();
            lightMap.End();

            // Combine with the current unlit scene:
            lightMap.Draw();
        }
        
        /// <summary>
        /// Draws the overlays that are part of the scene.
        /// </summary>
        private void DrawOverlays()
        {
            var overlays = scene.Overlays;
            this.drawContext.Device.BlendState = BlendState.NonPremultiplied;

            for( int i = 0; i < overlays.Count; ++i )
            {
                overlays[i].Draw( drawContext );
            }
        }
        
        /// <summary>
        /// The scene currently drawn.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The settings of the scene that is currently drawn.
        /// </summary>
        private SceneSettings settings;

        /// <summary>
        /// The context under which is currently drawn.
        /// </summary>
        private ZeldaDrawContext drawContext;

        /// <summary>
        /// The LightMap encapsulates the logic of changing RenderTarget
        /// when drawing the invidual lights and then compining them with
        /// the unlit scene.
        /// </summary>
        private readonly LightMap lightMap;
    }
}