// <copyright file="NoiseOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Overlays.NoiseOverlay class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Overlays
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna.Effects;
    using Microsoft.Xna.Framework.Graphics;
    
    /// <summary>
    /// Represents an <see cref="ISceneOverlay"/> that
    /// draws a <see cref="Noise"/> texture over the scene.
    /// </summary>
    public sealed class NoiseOverlay : ISceneOverlay, IDisposable, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the NoiseOverlay class.
        /// </summary>
        /// <param name="effectLoader">
        /// Provides a mechanism that allows loading of effect asserts.
        /// </param>
        /// <param name="deviceServive">
        /// Provides access to the Microsoft.Xna.Framework.Graphics.GraphicsDevice.
        /// </param>
        public NoiseOverlay( IEffectLoader effectLoader, IGraphicsDeviceService deviceServive )
        {
            this.effect = new NoiseEffect( effectLoader, deviceServive );
            this.effect.LoadContent();
        }

        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            this.effect.LoadContent();
        }

        /// <summary>
        /// Disposes this NoiseOverlay and its underlying <see cref="NoiseEffect"/>.
        /// </summary>
        public void Dispose()
        {
            this.effect.Dispose();
        }

        /// <summary>
        /// Gets the <see cref="NoiseEffect"/> used by this NoiseOverlay.
        /// </summary>
        public NoiseEffect Noise
        {
            get { return this.effect; }
        }

        /// <summary>
        /// Updates this NoiseOverlay.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void ISceneOverlay.Update( ZeldaUpdateContext updateContext )
        {
            this.effect.Update( updateContext );
        }

        /// <summary>
        /// Draw this NoiseOverlay.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        void ISceneOverlay.Draw( ZeldaDrawContext drawContext )
        {
            this.effect.Draw( drawContext );
        }

        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been added to the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void ISceneOverlay.AddedToScene( ZeldaScene scene )
        {
            System.Diagnostics.Debug.Assert( scene != null );
            scene.Camera.ScrollChanged += this.OnCameraScrollChanged;
        }

        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been removed from the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void ISceneOverlay.RemovedFromScene( ZeldaScene scene )
        {
            System.Diagnostics.Debug.Assert( scene != null );
            scene.Camera.ScrollChanged -= this.OnCameraScrollChanged;
        }

        /// <summary>
        /// Gets called when the scroll property of the Camera has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnCameraScrollChanged( object sender, ChangedValue<Vector2> e )
        {
            Vector2 change = e.NewValue - e.OldValue;
            change.Y = -change.Y;

            this.effect.Move( change / 15000.0f );
        }

        /// <summary>
        /// The <see cref="Noise"/> effect that is used by this NoiseOverlay.
        /// </summary>
        private readonly NoiseEffect effect;
    }
}
