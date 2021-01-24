// <copyright file="MiniMapWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.MiniMapWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// This ingame window draws the
    /// mini map of the current ZeldaScene.
    /// </summary>
    internal sealed class MiniMapWindow : IngameWindow, System.IDisposable
    {
        #region [ Conctants ]

        /// <summary>
        /// The size of the border on the x-axis.
        /// </summary>
        private const int HorizontalBorderSize = 20;

        /// <summary>
        /// The size of the border on the y-axis.
        /// </summary>
        private const int VerticalBorderSize = 20;

        /// <summary>
        /// The color of the background rectangle.
        /// </summary>
        private readonly Xna.Color ColorBackground = Xna.Color.Black;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Sets the <see cref="ZeldaScene"/> whose map
        /// is shown in this MiniMapWindow.
        /// </summary>
        public ZeldaScene Scene
        {
            // get
            // {
            //    return this.scene;
            // }
            // ^ Is unused.
            set
            {
                this.scene = value;
                this.CreateMap();
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MiniMapWindow class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MiniMapWindow( IZeldaServiceProvider serviceProvider )
        {
            this.device = serviceProvider.Game.GraphicsDevice;

            // Set Properties.
            this.Size = serviceProvider.ViewSize;
            
            // Create Render Target.
            this.renderTarget = new RenderTarget2D( 
                device,
                (int)this.Size.X - (VerticalBorderSize * 2),
                224, 
                false, 
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents
            );

            // Create RedrawContext.
            this.redrawContext = new SpriteDrawContext( new ComposedSpriteBatch(this.device), this.device );

            // Hook Events.
            this.device.DeviceReset += OnDeviceReset;

            // Load Content.
            this.spriteLinkHead = serviceProvider.SpriteLoader.LoadSprite( "Button_CharacterWindow" );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this MiniMapWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.scene == null )
            {
                return;
            }

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            IComposedSpriteBatch batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect( this.ClientArea, ColorBackground );

            // Draw Scene Name
            UIFonts.TahomaBold11.Draw(
                scene.LocalizedName ?? string.Empty,
                new Vector2( (int)(this.Width / 2), 2.0f ),
                TextAlign.Center, 
                Xna.Color.White,
                0.5f,
                drawContext
            );

            this.DrawMap( zeldaDrawContext );
            this.DrawFogOfWar( zeldaDrawContext );
            this.DrawPlayer( batch );
        }

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawMap( ZeldaDrawContext drawContext )
        {
            if( this.mapTexture == null )
            {
                return;
            }

            drawContext.Batch.Draw(
                this.mapTexture,
                new Vector2( VerticalBorderSize, HorizontalBorderSize ),
                Xna.Color.White,
                0.9f
            );
        }

        /// <summary>
        /// Draws the Fog of War on-top of the map.
        /// </summary>
        /// <param name="zeldaDrawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawFogOfWar( ZeldaDrawContext zeldaDrawContext )
        {
            if( mapTexture == null )
            {
                return;
            }

            // Draw Fog of War.
            FogOfWarStatus fow = scene.Status.FogOfWar;
            IComposedSpriteBatch batch = zeldaDrawContext.Batch;

            int fowCellWidth = mapTexture.Width / fow.Size;
            int fowCellHeight = mapTexture.Height / fow.Size;

            for( int x = 0; x < fow.Size; ++x )
            {
                for( int y = 0; y < fow.Size; ++y )
                {
                    bool isUncovered = fow[x, y];

                    if( !isUncovered )
                    {
                        var rectangle = new Xna.Rectangle(
                            (x * fowCellWidth) + VerticalBorderSize,
                            (y * fowCellHeight) + HorizontalBorderSize,
                            fowCellWidth,
                            fowCellHeight
                        );

                        batch.DrawRect( 
                            rectangle,
                            Xna.Color.Black,
                            0.95f
                       );
                    }
                }
            }
        }

        /// <summary>
        /// Draws a blinking sprite at the position of the player.
        /// </summary>
        /// <param name="batch">
        /// The SpriteBatch to use.
        /// </param>
        private void DrawPlayer( ISpriteBatch batch )
        {
            Entities.PlayerEntity player = scene.Player;
            if( player == null )
            {
                return;
            }

            if( tickTimeBlinkingHead <= 1.0f )
            {
                Vector2 position = player.Collision.Center;
                var projectedPosition = new Vector2(
                    (position.X / scene.WidthInPixels) * renderTarget.Width,
                    (position.Y / scene.HeightInPixels) * renderTarget.Height
                );

                var borderSize   = new Vector2( VerticalBorderSize, HorizontalBorderSize );
                var spriteCenter = new Vector2( spriteLinkHead.Width / 2.0f, spriteLinkHead.Height / 2.0f );
                Vector2 drawPosition = projectedPosition - spriteCenter + borderSize;

                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                spriteLinkHead.Draw( drawPosition, 1.0f, batch );
            }
        }

        /// <summary>
        /// Draws the map; storing it in the mapTexture.
        /// </summary>
        private void CreateMap()
        {
            if( scene == null )
            {
                if( mapTexture != null )
                {
                    mapTexture.Dispose();
                    mapTexture = null;
                }
                return;
            }

            // Find Scaling Transform
            TileMap map       = scene.Map;
            int mapWidth  = map.Width * 16;
            int mapHeight = map.Height * 16;

            var transform = Xna.Matrix.CreateScale( 
                renderTarget.Width / (float)mapWidth,
                renderTarget.Height / (float)mapHeight,
                1.0f
            );

            // Setup Drawing
            device.SetRenderTarget( renderTarget );
            this.redrawContext.Begin( BlendState.NonPremultiplied, SamplerState.LinearClamp, SpriteSortMode.Deferred, transform );

            // Draw :)
            foreach( TileMapFloor floor in map.Floors )
            {
                foreach( TileMapSpriteDataLayer layer in floor.Layers )
                {
                    layer.Draw( 0, 0, mapWidth, mapHeight, this.redrawContext );
                }
            }

            if( this.ShouldDrawAmbientLayer() )
            {
                Xna.Color ambient = this.scene.Settings.AmbientColor;
                byte alpha = 125;

                this.redrawContext.Batch.DrawRect(
                    new Rectangle(
                        0,
                        0,
                        mapWidth,
                        mapHeight
                    ),
                    new Xna.Color( ambient.R, ambient.G, ambient.B, alpha )
                );            
            }

            this.redrawContext.End();
            device.SetRenderTarget( null );

            // Store Draw-Result
            mapTexture = renderTarget;
        }

        /// <summary>
        /// Gets a value indicating whether a rectangle containing
        /// the ambient color should be drawn on-top of the scene.
        /// </summary>
        /// <returns>
        /// true if the ambient color should be drawn
        /// above the scene; otherwise false.
        /// </returns>
        private bool ShouldDrawAmbientLayer()
        {
            return this.scene.Settings.SceneType == SceneType.OutdoorAmbient;
        }

        /// <summary>
        /// Called when this MiniMapWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            this.tickTimeBlinkingHead -= updateContext.FrameTime;

            if( this.tickTimeBlinkingHead <= 0.0f )
            {
                this.tickTimeBlinkingHead = 2.0f;
            }
        }

        /// <summary>
        /// Called when the XNA device has been reset.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contains the event data.
        /// </param>
        private void OnDeviceReset( object sender, EventArgs e )
        {
            this.CreateMap();
        }

        /// <summary>
        /// Immediatly releases the unmanaged resources used by this MiniMapWindow.
        /// </summary>
        public void Dispose()
        {
            if( this.renderTarget != null )
            {
                this.renderTarget.Dispose();
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The scene whose map is currently shown.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The texture that contains the complete map.
        /// </summary>
        private Texture2D mapTexture;

        /// <summary>
        /// Used to make the spriteLinkHead blink.
        /// </summary>
        private float tickTimeBlinkingHead;

        /// <summary>
        /// The sprite of link's head.
        /// </summary>
        private readonly Sprite spriteLinkHead;
                
        /// <summary>
        /// The target the map is drawn onto.
        /// </summary>
        private readonly RenderTarget2D renderTarget;

        /// <summary>
        /// The draw context used when drawing the map
        /// </summary>
        private readonly ISpriteDrawContext redrawContext;

        /// <summary>
        /// Indentifies the XNA graphics device.
        /// </summary>
        private readonly GraphicsDevice device;

        #endregion
    }
}
