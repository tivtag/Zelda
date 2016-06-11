// <copyright file="ItemInfoRenderer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfoRenderer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.UI.Items.Boxes;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements a mechanism that renders an ItemBoxModel to a Texture.
    /// </summary>
    internal sealed class ItemInfoRenderer : IZeldaSetupable
    {
        /// <summary>
        /// Creates a RenderTarget to which this ItemInfoRenderer
        /// should render ItemInfos to.
        /// </summary>
        /// <returns>
        /// A newly created RenderTarget2D.
        /// </returns>
        public RenderTarget2D CreateRenderTarget()
        {
            var viewSize = this.serviceProvider.ViewSize;

            return new RenderTarget2D(
                this.device,
                2 * viewSize.X,
                2 * viewSize.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        }

        /// <summary>
        /// Setups this ItemInfoRenderer.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.device = serviceProvider.Game.GraphicsDevice;
            this.serviceProvider = serviceProvider;

            this.drawContext = new ZeldaDrawContext( serviceProvider.Game.GraphicsDevice );
        }

        /// <summary>
        /// Renders the given BoxModel to
        /// </summary>
        /// <param name="itemBoxModel">
        /// The BoxModel of the item to draw.
        /// </param>
        /// <param name="renderTarget">
        /// The target to render to.
        /// </param>
        public Texture2D Render( ItemBoxModel itemBoxModel, RenderTarget2D renderTarget )
        {
            // Change to new RenderTarget and cache old.
            var oldRenderTarget = this.device.GetRenderTarget2D();
            this.device.SetRenderTarget( renderTarget );

            // Draw the box model of the item.
            this.ActualyDraw( itemBoxModel );

            // Get rendered content and reset RenterTarget.
            this.device.SetRenderTarget( oldRenderTarget );
            return renderTarget;
        }

        /// <summary>
        /// Executes the actual drawing logic.
        /// </summary>
        /// <param name="itemBoxModel">
        /// The BoxModel of the item to draw.
        /// </param>
        private void ActualyDraw( ItemBoxModel itemBoxModel )
        {
            this.device.Clear( ClearOptions.Target, Xna.Color.Transparent, 0.0f, 0 );

            var batch = this.drawContext.Batch;

            // Draw Background.
            this.drawContext.Begin( BlendState.NonPremultiplied, SamplerState.PointWrap, SpriteSortMode.Deferred );

            Point2 totalSize = itemBoxModel.TotalSize;

            batch.DrawRect(
                new Xna.Rectangle( 0, 0, totalSize.X, totalSize.Y ),
                ItemInfoColors.Background
            );

            this.drawContext.End();

            // Draw Model.
            this.drawContext.Begin(
                BlendState.NonPremultiplied,
                SamplerState.PointWrap,
                SpriteSortMode.Deferred,
                Xna.Matrix.CreateTranslation( ItemBoxModel.VerticalBorderWidth, ItemBoxModel.HorizontalBorderHeight, 0.0f )
            );

            itemBoxModel.Draw( this.drawContext );

            this.drawContext.End();
        }

        /// <summary>
        /// The xna graphics device that used during rendering.
        /// </summary>
        private GraphicsDevice device;

        /// <summary>
        /// The context to which
        /// </summary>
        private ZeldaDrawContext drawContext;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }
}
