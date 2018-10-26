// <copyright file="SceneScroller.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.SceneScroller class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    using System;
    using Atom.Diagnostics.Contracts;
    using System.Windows.Input;
    using Atom;
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Implements a mechanism that reacts to user input to scroll the scene.
    /// </summary>
    internal sealed class SceneScroller : IUpdateable
    {
        /// <summary>
        /// Initializes a new instance of the SceneScroller class.
        /// </summary>
        /// <param name="providerContainer">
        /// Provides a mechanism for receiving the IObjectProvider that returns currently active ZeldaCamera.
        /// </param>
        public SceneScroller( IObjectProviderContainer providerContainer )
        {
            Contract.Requires<ArgumentNullException>( providerContainer != null );

            this.cameraProvider = providerContainer.GetObjectProvider<ZeldaCamera>();
        }

        /// <summary>
        /// Updates this SceneScroller, scrolling the scene if requested.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( IUpdateContext updateContext )
        {
            ZeldaCamera camera = this.cameraProvider.Resolve();
            if( camera == null )
                return;

            if( this.scrollDirections == Directions.None )
                return;

            float speed = this.isMovingFast ? 200.0f : 100.0f;
            Vector2 direction = this.scrollDirections.ToVector();
            Vector2 change = (direction * speed) * updateContext.FrameTime;

            camera.Scroll += change;  
        }

        /// <summary>
        /// Gets called when the user presses any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyDown( KeyEventArgs e )
        {
            switch( e.Key )
            {
                case Key.A:
                    this.scrollDirections |= Directions.Left;
                    break;

                case Key.D:
                    this.scrollDirections |= Directions.Right;
                    break;

                case Key.W:
                    this.scrollDirections |= Directions.Up;
                    break;

                case Key.S:
                    this.scrollDirections |= Directions.Down;
                    break;

                default:
                    break;
            }

            this.isMovingFast = e.KeyboardDevice.Modifiers == ModifierKeys.Shift;
        }

        /// <summary>
        /// Gets called when the user releases any key.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        public void HandleKeyUp( KeyEventArgs e )
        {
            switch( e.Key )
            {
                case Key.A:
                    this.scrollDirections &= ~Directions.Left;
                    e.Handled = true;
                    break;

                case Key.D:
                    this.scrollDirections &= ~Directions.Right;
                    e.Handled = true;
                    break;

                case Key.W:
                    this.scrollDirections &= ~Directions.Up;
                    e.Handled = true;
                    break;

                case Key.S:
                    this.scrollDirections &= ~Directions.Down;
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the state of this SceneScroller.
        /// </summary>
        /// <remarks>
        /// Should be called when focus moves away from this SceneScroller.
        /// </remarks>
        public void Reset()
        {
            this.isMovingFast = false;
            this.scrollDirections = Directions.None;
        }

        /// <summary>
        /// The currently active scroll direction
        /// </summary>
        private Directions scrollDirections;

        /// <summary>
        /// States whether the user is moving fast right now.
        /// </summary>
        private bool isMovingFast;

        /// <summary>
        /// Provides a mechanism for receiving the currently active ZeldaCamera.
        /// </summary>
        private readonly IObjectProvider<ZeldaCamera> cameraProvider;
    }
}
