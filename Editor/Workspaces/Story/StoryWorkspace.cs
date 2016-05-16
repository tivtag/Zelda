// <copyright file="StoryWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Story.StoryWorkspace class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Story
{
    using System;
    using Atom;

    /// <summary>
    /// Defines the <see cref="IWorkspace"/> that allows the
    /// user to modify the Storyboard of the current Scene.
    /// </summary>
    public sealed class StoryWorkspace : IWorkspace
    {
        #region [ Properties ]

        /// <summary>
        /// Receives the <see cref="WorkspaceType"/> of this <see cref="IWorkspace"/>.
        /// </summary>
        public WorkspaceType Type
        {
            get
            {
                return WorkspaceType.Story;
            }
        }

        /// <summary>
        /// Gets the <see cref="SceneViewModel"/> that is currently modified.
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
        /// Initializes a new instance of the <see cref="StoryWorkspace"/> class.
        /// </summary>
        /// <param name="application">
        /// The XnaEditorApp object.
        /// </param>
        public StoryWorkspace( XnaEditorApp application )
        {
            if( application == null )
                throw new ArgumentNullException( "application" );

            this.application = application;
            this.sceneScroller = new SceneScroller( application );

            this.application.SceneChanged += this.OnSceneChanged;
        }

        public void LoadContent()
        {
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

            this.scene.Model.Update( updateContext );
            this.sceneScroller.Update( updateContext );
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
        }

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
            this.scene = e.NewValue;
        }
                
        #region } Mouse {

        /// <summary>
        /// Gets called when the user clicks on the Xna-PictureBox.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseClick( System.Windows.Forms.MouseEventArgs e )
        {
        }

        /// <summary>
        /// Gets called when the user presses the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseDown( System.Windows.Forms.MouseEventArgs e )
        {
        }

        /// <summary>
        /// Gets called when the user releases the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseUp( System.Windows.Forms.MouseEventArgs e )
        {
        }

        /// <summary>
        /// Gets called when the user moves the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        public void HandleMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
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

        #region [ Fields ]

        /// <summary>
        /// Identifies the currently active scene.
        /// </summary>
        private SceneViewModel scene;

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
