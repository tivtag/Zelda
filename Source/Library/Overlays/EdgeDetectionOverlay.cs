// <copyright file="EdgeDetectionOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Overlays.EdgeDetectionOverlay class.</summary>
// <author>Paul Ennemoser</author>

/* // Experimental Code.

namespace Zelda.Overlays
{
    using System;
    using Atom.Xna.Effects;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EdgeDetectionOverlay : ISceneOverlay, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the EdgeDetectionOverlay class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public EdgeDetectionOverlay( IZeldaServiceProvider serviceProvider )
        {
            var game = serviceProvider.AppObject;
            this.resolveTexture = new ResolveTexture2D(
                game.GraphicsDevice,
                serviceProvider.ViewSize.X,
                serviceProvider.ViewSize.Y,
                0,
                SurfaceFormat.Color
            );

            this.effect = new EdgeDetectionEffect( game );
            this.effect.Initialize();
        }

        /// <summary>
        /// Disposes this EdgeDetectionOverlay and its underlying <see cref="EdgeDetectionEffect"/>.
        /// </summary>
        public void Dispose()
        {
            this.effect.Dispose();
            this.resolveTexture.Dispose();
        }

        /// <summary>
        /// Gets the <see cref="EdgeDetectionEffect"/> used by this EdgeDetectionOverlay.
        /// </summary>
        public EdgeDetectionEffect Effect
        {
            get { return this.effect; }
        }

        /// <summary>
        /// Updates this EdgeDetectionOverlay.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            this.effect.Texture = this.GetBackBufferTexture( drawContext.GraphicsDevice );
            this.effect.Draw( drawContext.GameTime );
        }

        /// <summary>
        /// Gets the current content of the Back Buffer.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private Texture2D GetBackBufferTexture( GraphicsDevice device )
        {
            var renderTarget = device.GetRenderTarget( 0 ) as RenderTarget2D;

            if( renderTarget != null )
            {
                device.SetRenderTarget( 0, null );
                var texture = renderTarget.GetTexture();
                device.SetRenderTarget( 0, renderTarget );
                return texture;
            }
            else
            {
                device.ResolveBackBuffer( resolveTexture );
                return resolveTexture;
            }
        }

        /// <summary>
        /// Updates this EdgeDetectionOverlay.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void ISceneOverlay.Update( ZeldaUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been added to the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void ISceneOverlay.AddedToScene( ZeldaScene scene )
        {
        }

        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been removed from the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void ISceneOverlay.RemovedFromScene( ZeldaScene scene )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly ResolveTexture2D resolveTexture;

        /// <summary>
        /// The <see cref="EdgeDetectionEffect"/> used by this EdgeDetectionOverlay.
        /// </summary>
        private readonly EdgeDetectionEffect effect;
    }
}
*/
