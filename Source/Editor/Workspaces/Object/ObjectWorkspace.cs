// <copyright file="ObjectWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Object.ObjectWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Entities;

    /// <summary>
    /// Defines the <see cref="IWorkspace"/> that is used
    /// when the user edits the Objects of the Scene.
    /// </summary>
    public sealed class ObjectWorkspace : IWorkspace
    {
        #region [ Properties ]

        /// <summary>
        /// Receives the WorkspaceType of this IWorkspace.
        /// </summary>
        public WorkspaceType Type
        {
            get 
            {
                return WorkspaceType.Object;
            }
        }
                
        /// <summary>
        /// Gets the SceneViewModel that is currently manipulated by this ObjectWorkspace.
        /// </summary>
        public SceneViewModel Scene
        {
            get
            {
                return this.application.Scene;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectWorkspace"/> class.
        /// </summary>     
        /// <param name="application">
        /// The XnaEditorApp object.
        /// </param>
        public ObjectWorkspace( XnaEditorApp application )
        {
            Contract.Requires<ArgumentNullException>( application != null );

            this.application = application;
            this.application.SceneChanged += OnSceneChanged;
            this.sceneScroller = new SceneScroller( application );
        }

        /// <summary>
        /// Loads any content used by the ObjectWorkspace.
        /// </summary>
        public void LoadContent()
        {
        }

        /// <summary>
        /// Unloads all content used by the ObjectWorkspace.
        /// </summary>
        public void UnloadContent()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this IWorkspace.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.sceneViewModel == null )
                return;

            this.sceneScroller.Update( updateContext );
            this.sceneViewModel.Model.Update( updateContext );
            this.UpdateSoundListenerPosition();
        }
        
        /// <summary>
        /// Updates the position of the 3D sound listener.
        /// </summary>
        private void UpdateSoundListenerPosition()
        {
            var camera     = this.sceneViewModel.Model.Camera;
            var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            // Sound can be easily tested ingame by setting the
            // position of the listener to the current mouse position:
            this.application.AudioSystem.ListenerPosition2D = new Vector2( mouseState.X + camera.Scroll.X, mouseState.Y + camera.Scroll.Y );
        }

        /// <summary>
        /// Draws this IWorkspace.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( sceneViewModel == null )
                return;

            this.application.SceneDrawer.Draw( sceneViewModel.Model, drawContext );
            this.DrawCollisionRectangles( drawContext );
            this.application.DrawQuadTree();
            this.application.DrawTileMapBorder( drawContext );                      
        }

        private void DrawCollisionRectangles( ZeldaDrawContext drawContext )
        {
            if( showCollisionAreas )
            {
                drawContext.Begin( BlendState.NonPremultiplied,
                    SamplerState.PointWrap, SpriteSortMode.Deferred, sceneViewModel.Camera.Model.Transform );
                {
                    foreach( var entity in this.sceneViewModel.Model.VisibleEntities )
                    {
                        drawContext.Batch.DrawRect(
                            entity.Collision.Rectangle,
                            new Microsoft.Xna.Framework.Color( 255, 0, 0, 100 )
                        );
                    }
                }
                drawContext.End();
            }
        }

        /// <summary>
        /// Called the user enters the ObjectWorkspace.
        /// </summary>
        public void Enter()
        {
            ZeldaScene.EntityEditMode = true;
        }

        /// <summary>
        /// Called the user leaves the ObjectWorkspace.
        /// </summary>
        public void Leave()
        {
            this.grapOffset = Vector2.Zero;
            this.grappedEntity = null;
            ZeldaScene.EntityEditMode = false;

            // 'Disable' 3D audio:
            this.application.AudioSystem.ListenerPosition2D = new Vector2( -10000.0f, -10000.0f );

            this.showCollisionAreas = false;
            this.sceneScroller.Reset();
        }

        #region > Events <

        /// <summary>
        /// Gets called when the currently active scene has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ChangedValue{Zelda.Editor.SceneViewModel} that contains the event data.
        /// </param>
        private void OnSceneChanged( object sender, ChangedValue<Zelda.Editor.SceneViewModel> e )
        {
            this.sceneViewModel = e.NewValue;
        }

        #region [ Keyboard ]

        /// <summary>
        /// Gets called when the user presses any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyDown( System.Windows.Input.KeyEventArgs e )
        {
            if( e.Key == System.Windows.Input.Key.LeftCtrl )
            {
                this.showCollisionAreas = !this.showCollisionAreas;
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

        #region [ Mouse ]

        /// <summary>
        /// Gets called when the user clicks on the Scene.
        /// </summary>
        /// <param name="e">
        /// The System.Windows.Forms.MouseEventArgs that contain the event data.
        /// </param>
        public void HandleMouseClick( System.Windows.Forms.MouseEventArgs e )
        {
        }

        /// <summary>
        /// Gets called when the user clicks on the Scene.
        /// </summary>
        /// <param name="e">
        /// The System.Windows.Forms.MouseEventArgs that contain the event data.
        /// </param>
        public void HandleMouseDown( System.Windows.Forms.MouseEventArgs e )
        {
            if( this.sceneViewModel == null )
                return;

            switch( e.Button )
            {
                case System.Windows.Forms.MouseButtons.Left:
                    GrapObject( e.X, e.Y );
                    break;

                case System.Windows.Forms.MouseButtons.Middle:
                    SelectObject( e.X, e.Y );
                    break;

                case System.Windows.Forms.MouseButtons.Right:
                    MoveSelectedObjectTo( e.X, e.Y );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Moves the currently selected object to the given position.
        /// </summary>
        /// <param name="mouseX">The position of the mouse on the x-axis.</param>
        /// <param name="mouseY">The position of the mouse on the y-axis.</param>
        private void MoveSelectedObjectTo( int mouseX, int mouseY )
        {
            if( sceneViewModel == null )
                return;

            ZeldaEntity entity = sceneViewModel.SelectedObject as ZeldaEntity;
            if( entity == null )
                return;

            var scene       = sceneViewModel.Model;
            var scroll      = scene.Camera.Scroll;
            var newPosition = new Vector2( mouseX + scroll.X, mouseY + scroll.Y );

            // Clamp position
            if( newPosition.X < 0.0f )
                newPosition.X = 0.0f;
            else if( newPosition.X > scene.WidthInPixels )
                newPosition.X = scene.WidthInPixels;

            if( newPosition.Y < 0.0f )
                newPosition.Y = 0.0f;
            else if( newPosition.Y > scene.HeightInPixels )
                newPosition.Y = scene.HeightInPixels;

            entity.Transform.Position = newPosition;

            #region Check Entity Integratiy

            if( entity.Scene == null )
            {
                EditorApp.Current.Log.Write(
                    string.Format( System.Globalization.CultureInfo.CurrentCulture,
                        "The entity '{0}' had no Scene. Readding it the Scene.", 
                        entity.ToString() 
                    )
                );

                if( scene.Entities.Contains( entity ) )
                {
                    entity.RemoveFromScene();
                }

                scene.Add( entity );
            }

            #endregion
        }

        /// <summary>
        /// Graps the object at the given position.
        /// </summary>
        /// <param name="mouseX">The position of the mouse on the x-axis.</param>
        /// <param name="mouseY">The position of the mouse on the y-axis.</param>
        private void GrapObject( int mouseX, int mouseY )
        {
            // Find the entitiy the user has clicked on.
            var selectedEntity = this.FindEntityToSelect( mouseX, mouseY );

            // Store change:
            this.grappedEntity                 = selectedEntity;
            this.sceneViewModel.SelectedObject = selectedEntity;

            // Find grap-offset.
            if( this.grappedEntity != null )
            {
                var scene    = sceneViewModel.Model;
                var scroll   = scene.Camera.Scroll;
                var position = new Vector2( mouseX + scroll.X, mouseY + scroll.Y );

                this.grapOffset = this.grappedEntity.Transform.Position - position;
            }
            else
            {
                this.grapOffset = Vector2.Zero;
            }
        }

        /// <summary>
        /// Selects the object at the given position without grapping it.
        /// </summary>
        /// <param name="mouseX">The position of the mouse on the x-axis.</param>
        /// <param name="mouseY">The position of the mouse on the y-axis.</param>
        private void SelectObject( int mouseX, int mouseY )
        {
            if( this.grappedEntity != null )
                return;

            this.sceneViewModel.SelectedObject = this.FindEntityToSelect( mouseX, mouseY );
        }

        /// <summary>
        /// Finds the entity the user tried to select.
        /// </summary>
        /// <param name="mouseX">The position of the mouse on the x-axis.</param>
        /// <param name="mouseY">The position of the mouse on the y-axis.</param>
        /// <returns>
        /// The entity that has been selected.
        /// </returns>
        private ZeldaEntity FindEntityToSelect( int mouseX, int mouseY )
        {
            var scene    = sceneViewModel.Model;
            var scroll   = scene.Camera.Scroll;
            var position = new Vector2( mouseX + scroll.X, mouseY + scroll.Y );

            // Query entities that the user has clicked on.
            var entitiesQuery = from entity in scene.VisibleEntities
                                where entity.Collision.Rectangle.Contains( position )
                                select entity;

            // Evulate the query now.
            var entities = entitiesQuery.ToArray();

            // Give up if there were no entities.
            if( entities.Length == 0 )
                return null;

            // Find the entitiy the user has clicked on.
            if( entities.Length == 1 )
            {
                return entities.First();
            }
            else
            {
                var selectedEntity = sceneViewModel.SelectedObject as ZeldaEntity;

                foreach( var entity in entities )
                {
                    if( entity == selectedEntity )
                        continue;

                    // Ignore light entities if..
                    if( entity is Light && !scene.Settings.IsLightingEnabled )
                        continue;

                    selectedEntity = entity;
                    break;
                }

                return selectedEntity;
            }
        }

        /// <summary>
        /// Gets called when the user releases the mouse.
        /// </summary>
        /// <param name="e">
        /// The System.Windows.Forms.MouseEventArgs that contain the event data.
        /// </param>
        public void HandleMouseUp( System.Windows.Forms.MouseEventArgs e )
        {
            if( e.Button == System.Windows.Forms.MouseButtons.Left )
            {
                if( this.grappedEntity != null )
                {
                    this.grappedEntity = null;
                }
            }
        }

        /// <summary>
        /// Gets called when the user moves the mouse.
        /// </summary>
        /// <param name="e">
        /// The System.Windows.Forms.MouseEventArgs that contain the event data.
        /// </param>
        public void HandleMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            if( sceneViewModel == null )
                return;

            if( grappedEntity != null )
            {
                var scene       = sceneViewModel.Model;
                var scroll      = scene.Camera.Scroll;
                var newPosition = new Vector2( e.X + grapOffset.X + scroll.X, e.Y + grapOffset.Y + scroll.Y );

                // Clamp to tile if the user has Shift pressed.
                if( System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift )
                {
                    newPosition /= 16.0f;
                    newPosition = Vector2.Round( newPosition );
                    newPosition *= 16.0f;
                }

                // Clamp position
                newPosition = Vector2.Clamp( newPosition, Vector2.Zero, sceneViewModel.SizeInPixels );
                grappedEntity.Transform.Position = newPosition;
            }
        }

        #endregion

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The entity the user is currently moving.
        /// </summary>
        private ZeldaEntity grappedEntity;

        /// <summary>
        /// The offset from where the use clicked on the grappedEntity to its position.
        /// </summary>
        private Vector2 grapOffset;

        /// <summary>
        /// Identifies the currently active scene.
        /// </summary>
        private SceneViewModel sceneViewModel;

        /// <summary>
        /// Implements a mechanism that reacts to user input to scroll the scene.
        /// </summary>
        private SceneScroller sceneScroller;

        /// <summary>
        /// States whether the collision areas of all entities are currently visualized.
        /// </summary>
        private bool showCollisionAreas;

        /// <summary>
        /// The XnaEditorApp object.
        /// </summary>
        private readonly XnaEditorApp application;

        #endregion
    }
}
