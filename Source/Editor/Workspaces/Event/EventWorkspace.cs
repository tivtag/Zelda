// <copyright file="EventWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Event.EventWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Event
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Atom;
    using Atom.Events;
    using Atom.Math;

    /// <summary>
    /// Defines the <see cref="IWorkspace"/> that is used
    /// when the user edits the Events and EventTriggers of the Scene.
    /// </summary>
    public sealed class EventWorkspace : IWorkspace
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="WorkspaceType"/> of this IWorkspace.
        /// </summary>
        public WorkspaceType Type
        {
            get 
            { 
                return WorkspaceType.Event; 
            }
        }

        /// <summary>
        /// Gets the <see cref="SceneViewModel"/> this EventWorkspace currently modifies.
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
        /// Initializes a new instance of the <see cref="EventWorkspace"/> class.
        /// </summary>     
        /// <param name="application">
        /// The XnaEditorApp object.
        /// </param>
        public EventWorkspace( XnaEditorApp application )
        {
            if( application == null )
                throw new ArgumentNullException( "application" );

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

            this.sceneViewModel.Model.Update( updateContext );
            this.sceneScroller.Update( updateContext );
        }

        /// <summary>
        /// Draws this IWorkspace.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.sceneViewModel == null )
                return;

            this.application.SceneDrawer.Draw( this.sceneViewModel.Model, drawContext );
            this.application.DrawQuadTree();
            this.application.DrawTileMapBorder( drawContext );
        }
        
        /// <summary>
        /// Graps the AreaEventTrigger at the given position.
        /// </summary>
        /// <param name="mouseX">
        /// The position of the mouse on the x-axis.
        /// </param>
        /// <param name="mouseY">
        /// The position of the mouse on the y-axis.
        /// </param>
        private void GrapAreaTrigger( int mouseX, int mouseY )
        {
            var scene    = this.sceneViewModel.Model;
            var scroll   = scene.Camera.Scroll;
            var position = new Point2( mouseX + (int)scroll.X, mouseY + (int)scroll.Y );

            var triggers = 
                (from trigger in scene.EventManager.Triggers                           
                 let areaTrigger = trigger as AreaEventTrigger
                 where areaTrigger != null && areaTrigger.Contains( position )
                 orderby (areaTrigger.Area.Width * areaTrigger.Area.Height)
                 select areaTrigger).ToList();
            
            // Give up if there were no triggers.
            if( triggers.Count == 0 )
                return;

            // Stores the new grapped trigger.
            AreaEventTrigger newGrappedTrigger = null;

            // Now find the trigger to grap:
            // Receive the trigger the user has selected
            // in the list.
            var selectedAreaTrigger = sceneViewModel.EventManager.Triggers.CurrentItem as AreaEventTrigger;
            
            // If the user has clicked on the currently selected trigger
            // then choose that one.
            if( selectedAreaTrigger != null )
            {
                if( triggers.Contains( selectedAreaTrigger ) )
                    newGrappedTrigger = selectedAreaTrigger;
            }

            if( newGrappedTrigger == null )
                newGrappedTrigger = triggers[0];

            this.grappedTrigger = newGrappedTrigger;
            this.grapOffset     = grappedTrigger.Area.Position - position;

            this.sceneViewModel.EventManager.Triggers.MoveCurrentTo( grappedTrigger );
        }

        /// <summary>
        /// Moves the currently selected AreaEventTrigger to the specified position.
        /// </summary>
        /// <param name="mouseX">The position of the mouse on the x-axis.</param>
        /// <param name="mouseY">The position of the mouse on the y-axis.</param>
        private void MoveSelectedTriggerTo( int mouseX, int mouseY )
        {
            var trigger = this.sceneViewModel.EventManager.SelectedTrigger as AreaEventTrigger;
            if( trigger == null )
                return;

            var scroll = this.sceneViewModel.Camera.Scroll;
            var position = new Point2( mouseX + (int)scroll.X, mouseY + (int)scroll.Y );
         
            var area = trigger.Area;
            trigger.Area = new Rectangle( position, area.Size );
        }

        /// <summary>
        /// Called the user enters the ObjectWorkspace.
        /// </summary>
        public void Enter()
        {
            ZeldaScene.EventTriggersAreDrawn = true;

            if( sceneViewModel != null )
                sceneViewModel.Model.NotifyVisabilityUpdateNeeded();
        }

        /// <summary>
        /// Called the user leaves the ObjectWorkspace.
        /// </summary>
        public void Leave()
        {
            this.grapOffset = Point2.Zero;
            this.grappedTrigger = null;
            ZeldaScene.EventTriggersAreDrawn = false;

            if( this.sceneViewModel != null )
            {
                this.sceneViewModel.Model.NotifyVisabilityUpdateNeeded();
            }

            this.sceneScroller.Reset();
        }

        #region > Events <

        #region [ General ]

        /// <summary>
        /// Gets called when the currently active scene has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The value that has changed.
        /// </param>
        private void OnSceneChanged( object sender, ChangedValue<Zelda.Editor.SceneViewModel> e )
        {
            this.sceneViewModel = e.NewValue;
        }

        #endregion

        #region [ Keyboard ]

        /// <summary>
        /// Gets called when the user presses any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyDown( System.Windows.Input.KeyEventArgs e )
        {
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
                    this.GrapAreaTrigger( e.X, e.Y );
                    break;

                case System.Windows.Forms.MouseButtons.Right:
                    this.MoveSelectedTriggerTo( e.X, e.Y );
                    break;

                default:
                    break;
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
                if( this.grappedTrigger != null )
                {
                    this.grappedTrigger = null;
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
            if( this.sceneViewModel == null )
                return;

            if( this.grappedTrigger != null )
            {
                var scene       = sceneViewModel.Model;
                var scroll      = scene.Camera.Scroll;

                var size        = grappedTrigger.Area.Size;
                var newPosition = new Point2( e.X + grapOffset.X + (int)scroll.X, e.Y + grapOffset.Y + (int)scroll.Y );

                // Clamp to tile if the user has Shift pressed.
                if( System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift )
                {
                    newPosition /= 16;
                    newPosition *= 16;
                }
                
                // Clamp position
                if( newPosition.X < 0 )
                    newPosition.X = 0;
                else if( newPosition.X + size.X > scene.WidthInPixels )
                    newPosition.X = scene.WidthInPixels - size.X;

                if( newPosition.Y < 0 )
                    newPosition.Y = 0;
                else if( newPosition.Y + size.Y > scene.HeightInPixels )
                    newPosition.Y = scene.HeightInPixels - size.Y;

                grappedTrigger.Area = new Rectangle( newPosition, size );
            }
        }

        #endregion

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The offset from where the use clicked on the grappedEntity to its position.
        /// </summary>
        private Point2 grapOffset;

        /// <summary>
        /// The AreaEventTrigger the user is currently tragging around.
        /// </summary>
        private AreaEventTrigger grappedTrigger;

        /// <summary>
        /// Identifies the currently active scene.
        /// </summary>
        private SceneViewModel sceneViewModel;

        /// <summary>
        /// The XnaEditorApp object.
        /// </summary>
        private readonly XnaEditorApp application;

        /// <summary>
        /// Implements a mechanism that reacts to user input to scroll the scene.
        /// </summary>
        private readonly SceneScroller sceneScroller;

        #endregion
    }
}
