// <copyright file="TileWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.TileWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Tile
{
    using System;
    using System.Windows.Input;
    using Atom;
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Editor.Tile.ViewModels;

    /// <summary>
    /// Defines the <see cref="IWorkspace"/> that is used
    /// when the user edits the TileMap of the Scene.
    /// </summary>
    public sealed class TileWorkspace : IWorkspace
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileWorkspace"/> class.
        /// </summary>
        /// <param name="application">
        /// The XnaEditorApp object.
        /// </param>
        public TileWorkspace( XnaEditorApp application )
        {
            if( application == null )
                throw new ArgumentNullException( "application" );

            this.application = application;
            this.sceneScroller = new SceneScroller( application );
            this.sheetDrawComponent = new SpriteSheetDrawComponent() {            
                MaximumSpritesPerRow = 10,
                IsUpdatingSheet      = false,
                SpriteWidth          = 16,
                SpriteHeight         = 16
            };

            this.sheetDrawComponent.Position = new Vector2(
                application.WindowSize.X - (sheetDrawComponent.MaximumSpritesPerRow+1)*sheetDrawComponent.SpriteWidth,
                4 * sheetDrawComponent.SpriteHeight 
            );
            
            application.SceneChanged += this.OnSceneChanged;
        }

        /// <summary>
        /// Loads the content used by this <see cref="IWorkspace"/>.
        /// </summary>
        public void LoadContent()
        {
            this.fontSelectedSprite = application.FontLoader.Load( "Verdana11_Bold" );
        }

        /// <summary>
        /// Unloads the content used by this <see cref="IWorkspace"/>.
        /// </summary>
        public void UnloadContent()
        {
            this.fontSelectedSprite = null;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Receives the <see cref="WorkspaceType"/> of this <see cref="IWorkspace"/>.
        /// </summary>
        public WorkspaceType Type
        {
            get 
            {
                return WorkspaceType.Tile; 
            }
        }

        /// <summary>
        /// Gets the currently modified <see cref="SceneViewModel"/>.
        /// </summary>
        public SceneViewModel Scene
        {
            get
            {
                return this.application.Scene;
            }
        }

        /// <summary>
        /// Gets the tile position based on the last mouse position.
        /// </summary>
        public Point2? TilePosition
        {
            get
            {
                return GetTilePosition( lastMousePosition );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.scene == null )
                return;

            if( this.activeTileLayingMode == TileLayingMode.Selected )
            {
                this.LayTile( this.lastMousePosition.X, this.lastMousePosition.Y );
            }

            this.scene.Model.Update( updateContext );
            this.sceneScroller.Update( updateContext );
        }

        /// <summary>
        /// Lays a tile at the given position using the currently active TileLayingMode.
        /// </summary>
        /// <param name="mouseX">
        /// The position of the mouse within the Xna Window on the x-axis.
        /// </param>
        /// <param name="mouseY">
        /// The position of the mouse within the Xna Window on the y-axis.
        /// </param>
        private void LayTile( int mouseX, int mouseY )
        {
            #region Verify State

            if( currentLayer == null || currentSheet == null )
            {
                activeTileLayingMode = TileLayingMode.None;
                return;
            }

            var selectedSprite = currentSheet.SelectedSprite;
            if( selectedSprite.Index == TileMap.InvalidTile )
            {
                activeTileLayingMode = TileLayingMode.None;
                return;
            }

            #endregion

            Point2? tilePosition = GetTilePosition( new Point2( mouseX, mouseY ) );

            if( tilePosition.HasValue )
            {
                Point2 tile = tilePosition.Value;
                var layer = currentLayer.Model;

                // Set the Tile depending on the currently active TileLayingMode:
                switch( activeTileLayingMode )
                {
                    case TileLayingMode.SelectedOnce:
                        layer.SetTile( tile.X, tile.Y, currentSheet.SelectedSprite.Index ); 
                        activeTileLayingMode = TileLayingMode.None;
                        break;

                    case TileLayingMode.EraseOnce:
                        layer.SetTile( tile.X, tile.Y, 0 ); 
                        activeTileLayingMode = TileLayingMode.None;
                        break;

                    case TileLayingMode.Selected:
                        layer.SetTile( tile.X, tile.Y, currentSheet.SelectedSprite.Index ); 
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Transforms mouse position into tile position on the tile map.
        /// </summary>
        /// <param name="mousePosition">
        /// The position of the mouse.
        /// </param>
        /// <returns>
        /// The tile position; -or- null if the mouse is not within the tile map.
        /// </returns>
        public Point2? GetTilePosition( Point2 mousePosition )
        {
            if( currentLayer == null )
                return null;

            // Transform the orgin of the input position
            // to the origin of the TileMap:
            Point2 transformedPosition;
            transformedPosition.X = mousePosition.X + application.TileMapArea.X;
            transformedPosition.Y = mousePosition.Y + application.TileMapArea.Y;
            
            // We only handle those requests that are in the
            // drawing area of the TileMap.
            if( application.TileMapArea.Contains( transformedPosition ) )
            {
                TileMapDataLayer layer = currentLayer.Model;

                // Add the current scrolling factor:
                transformedPosition.X += (int)scene.Model.Camera.Scroll.X;
                transformedPosition.Y += (int)scene.Model.Camera.Scroll.Y;

                // Convert the position from WorldSpace into TileSpace:
                Point2 tilePosition = transformedPosition / 16;
                return tilePosition;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Draws this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.scene == null )
                return;

            this.application.SceneDrawer.Draw( this.scene.Model, drawContext );
            this.application.DrawQuadTree();
            this.application.DrawTileMapBorder( drawContext );
            this.DrawSpriteSheet( drawContext );
        }
        
        /// <summary>
        /// Draws the current SpriteSheet and SelectedSprite.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawSpriteSheet( ZeldaDrawContext drawContext )
        {
            if( currentSheet == null )
                return;

            drawContext.Begin( BlendState.NonPremultiplied );
            {
                // Draw Sheet:
                this.sheetDrawComponent.Sheet = currentSheet.Model;
                this.sheetDrawComponent.Draw( drawContext );

                // Draw Selected Sprite Info:
                var selectedSprite = currentSheet.SelectedSprite;

                if( selectedSprite.Index != TileMap.InvalidTile )
                {
                    string spriteName = null;

                    if( selectedSprite.Sprite != null )
                    {
                        spriteName = selectedSprite.Sprite.Name;
                        selectedSprite.Sprite.Draw( sheetDrawComponent.Position + new Vector2( 16*3.5f, -16*2 ), drawContext.Batch );
                    }
                    else
                    {
                        if( selectedSprite.Index == 0 )
                            spriteName = Properties.Resources.NullSprite;
                    }

                    // Draw the name of the Sprite:
                    if( spriteName != null )
                    {
                        Vector2 position;
                        position.X = (int)(sheetDrawComponent.Position.X + 16*(sheetDrawComponent.MaximumSpritesPerRow/2.0f) -
                                           fontSelectedSprite.MeasureString( spriteName ).X/2);
                        position.Y = (int)(sheetDrawComponent.Position.Y - 16*3.5f);

                        fontSelectedSprite.Draw( spriteName, position, new Microsoft.Xna.Framework.Color( 240, 240, 240, 255 ), drawContext );
                    }

                    fontSelectedSprite.Draw( this.TilePosition.ToString(), new Vector2( application.WindowSize.X, application.WindowSize.Y - 15 ), TextAlign.Right, Microsoft.Xna.Framework.Color.White, drawContext );
                }
            }
            drawContext.End();
        }
        
        /// <summary>
        /// Gets called when the application has entered this <see cref="IWorkspace"/>.
        /// </summary>
        public void Enter()
        {
        }

        /// <summary>
        /// Gets called when the application has left this <see cref="IWorkspace"/>.
        /// </summary>
        public void Leave()
        {
            this.sceneScroller.Reset();
            this.activeTileLayingMode = TileLayingMode.None;
        }

        /// <summary>
        /// Refreshes the fields that indicate on what data the user currently works under.
        /// </summary>
        private void RefreshCurrent()
        {
            if( scene != null )
            {
                var floor = scene.Map.SelectedFloor;
                if( floor != null )
                {
                    switch( scene.Map.FloorEditMode )
                    {
                        case TileMapFloorEditMode.EditSelectedLayer:
                            {
                                var layer = floor.SelectedLayer;
                                if( layer != null )
                                {
                                    currentLayer = layer;
                                    currentSheet = layer.Sheet;
                                    sheetDrawComponent.Sheet = currentSheet != null ? currentSheet.Model : null;
                                    return;
                                }
                            }
                            break;

                        case TileMapFloorEditMode.EditActionLayer:
                            currentLayer = floor.ActionLayer;
                            currentSheet = floor.ActionLayer.Sheet;
                            sheetDrawComponent.Sheet = currentSheet != null ? currentSheet.Model : null;
                            return;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            currentLayer = null;
            currentSheet = null;
            sheetDrawComponent.Sheet = null;
        }

        #region > Events <

        #region - Input -

        #region } Mouse {

        /// <summary>
        /// Gets called when the user clicks on the Xna-PictureBox.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseClick( System.Windows.Forms.MouseEventArgs e )
        {
            lastMousePosition.X = e.X;
            lastMousePosition.Y = e.Y;

            if( scene == null )
                return;

            if( e.Button == System.Windows.Forms.MouseButtons.Left )
            {
                // Test Left mouse click against SpriteSheet:
                if( sheetDrawComponent.Sheet != null )
                {
                    System.Diagnostics.Debug.Assert( sheetDrawComponent.Sheet == currentSheet.Model );

                    if( sheetDrawComponent.BoundingRectangle.Contains( e.X, e.Y ) )
                    {
                        int index;
                        ISprite sprite = sheetDrawComponent.GetSpriteAt(
                            new Point2(
                                e.X - (int)sheetDrawComponent.Position.X,
                                e.Y - (int)sheetDrawComponent.Position.Y
                            ),
                            out index
                         );

                        currentSheet.SelectedSprite = new SelectedSpriteContainer( sprite, index );
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets called when the user presses the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseDown( System.Windows.Forms.MouseEventArgs e )
        {
            lastMousePosition.X = e.X;
            lastMousePosition.Y = e.Y;

            if( activeTileLayingMode != TileLayingMode.None )
                return;

            if( application.TileMapArea.Contains( e.X, e.Y ) )
            {
                switch( e.Button )
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        activeTileLayingMode = TileLayingMode.Selected;
                        LayTile( e.X, e.Y );
                        break;

                    case System.Windows.Forms.MouseButtons.Right:
                        activeTileLayingMode = TileLayingMode.EraseOnce;
                        LayTile( e.X, e.Y );
                        break;

                    case System.Windows.Forms.MouseButtons.Middle:
                        activeTileLayingMode = TileLayingMode.SelectedOnce;
                        LayTile( e.X, e.Y );
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets called when the user releases the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseUp( System.Windows.Forms.MouseEventArgs e )
        {
            // Stop laying the selected tile when the user
            // releases the middle mouse button.
            if( e.Button == System.Windows.Forms.MouseButtons.Left )
            {
                if( activeTileLayingMode == TileLayingMode.Selected )
                    activeTileLayingMode = TileLayingMode.None;
            }

            lastMousePosition.X = e.X;
            lastMousePosition.Y = e.Y;
        }

        /// <summary>
        /// Gets called when the user moves the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            lastMousePosition.X = e.X;
            lastMousePosition.Y = e.Y;

            if( activeTileLayingMode == TileLayingMode.Selected )
                LayTile( e.X, e.Y );
        }

        #endregion

        #region } Keyboard {

        /// <summary>
        /// Gets called when the user presses any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyDown( System.Windows.Input.KeyEventArgs e )
        {
            if( this.scene == null )
                return;

            switch( e.Key )
            {
                case Key.Space:
                    this.scene.Map.ToggleFloorEditMode();
                    e.Handled = true;
                    return;

                default:
                    break;
            }
            
            this.sceneScroller.HandleKeyDown( e );
        }

        /// <summary>
        /// Gets called when the user releases any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyUp( System.Windows.Input.KeyEventArgs e )
        {
            this.sceneScroller.HandleKeyUp( e );
        }

        #endregion

        #endregion

        #region - General -

        /// <summary>
        /// Gets called when the currently active scene has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ChangedValue{SceneViewModel} that contains the event data.
        /// </param>
        private void OnSceneChanged( object sender, ChangedValue<SceneViewModel> e )
        {
            if( e.OldValue != null )
            {
                e.OldValue.Map.SelectedFloorChanged -= this.OnMapSelectedFloorChanged;
                e.OldValue.Map.FloorEditModeChanged -= this.OnMapFloorEditModeChanged;
            }

            if( e.NewValue != null )
            {
                e.NewValue.Map.SelectedFloorChanged += this.OnMapSelectedFloorChanged;
                e.NewValue.Map.FloorEditModeChanged += this.OnMapFloorEditModeChanged;
            }

            this.scene = e.NewValue;
        }

        /// <summary>
        /// Gets called when the currently selected Floor has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ChangedValue{TileMapFloorViewModel} that contains the event data.
        /// </param>
        private void OnMapSelectedFloorChanged( object sender, ChangedValue<TileMapFloorViewModel> e )
        {
            var oldFloor = e.OldValue;
            var newFloor = e.NewValue;

            if( oldFloor != null )
                oldFloor.SelectedLayerChanged -= this.OnMapFloorSelectedLayerChanged;

            if( newFloor != null )
                newFloor.SelectedLayerChanged += this.OnMapFloorSelectedLayerChanged;

            this.RefreshCurrent();
        }

        /// <summary>
        /// Gets called when the currently selected layer of the currently selected Floor has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ChangedValue{TileMapSpriteDataLayerViewModel} that contains the event data.
        /// </param>
        private void OnMapFloorSelectedLayerChanged( object sender, ChangedValue<TileMapSpriteDataLayerViewModel> e )
        {
            this.RefreshCurrent();
        }

        /// <summary>
        /// Gets called when the TileMapFloorEditMode has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnMapFloorEditModeChanged( object sender, EventArgs e )
        {
            this.RefreshCurrent();
        }

        #endregion

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the currently active scene.
        /// </summary>
        private SceneViewModel scene;

        /// <summary>
        /// Identifies the sprite sheet the user currently works with.
        /// </summary>
        private SpriteSheetViewModel currentSheet;

        /// <summary>
        /// Identifies the layer the user currently works with.
        /// </summary>
        private TileMapDataLayerViewModel currentLayer;
        
        /// <summary>
        /// States the currently active TileLayingMode. 
        /// </summary>
        private TileLayingMode activeTileLayingMode;

        /// <summary>
        /// Stores the last known mouse position.
        /// </summary>
        private Point2 lastMousePosition;

        /// <summary>
        /// Provides a mechanism to drawn a SpriteSheet.
        /// </summary>
        private readonly SpriteSheetDrawComponent sheetDrawComponent;

        /// <summary>
        /// Identifies the font that is used to draw the name of the currently selected sprite.
        /// </summary>
        private IFont fontSelectedSprite;

        /// <summary>
        /// Identifies the <see cref="XnaEditorApp"/> object.
        /// </summary>
        private readonly XnaEditorApp application;

        /// <summary>
        /// Implements a mechanism that reacts to user input to scroll the scene.
        /// </summary>
        private readonly SceneScroller sceneScroller;

        #endregion
    }
}
