// <copyright file="ItemInfo.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfo class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using Atom.Math;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.UI.Items.Boxes;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Stores the rendered item information of an <see cref="ItemInstance"/>.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class ItemInfo : IZeldaSetupable
    {
        /// <summary>
        /// Gets the item this ItemInfo instance is visualizing.
        /// </summary>
        public ItemInstance Item
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Gets the pixels this ItemInfo takes up in total.
        /// </summary>
        public Point2 Size
        {
            get
            {
                return this.size;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ItemInfo is invalid;
        /// and as such must be re-
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                if( this.texture != null )
                {
                    return this.texture.IsDisposed;
                }

                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CachingItemInfo class.
        /// </summary>
        /// <param name="boxBuilder">
        /// The ItemInfoBoxBuilder the new CachingItemInfo should use to split Items
        /// into individual boxes that can be rendered by the given ItemInfoRenderer.
        /// </param>
        /// <param name="renderer">
        /// The ItemInfoRenderer the new CachingItemInfo should use to render the ItemInfo.
        /// </param>
        public ItemInfo( ItemInfoBoxBuilder boxBuilder, ItemInfoRenderer renderer )
        {
            this.boxBuilder = boxBuilder;
            this.renderer = renderer;
        }

        /// <summary>
        /// Setups this ItemInfoRenderer.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.viewSize = serviceProvider.ViewSize;
            this.renderTarget = this.renderer.CreateRenderTarget();
        }

        /// <summary>
        /// Renders the given ItemInstance into this CachingItemInfo.
        /// </summary>
        /// <param name="context">
        /// Encapsulates all draw and configuration data required for this operation.
        /// </param>
        public void RenderCache( ItemInfoVisualizationDrawContext context )
        {
            this.item = context.ItemInstance;

            // Extract the model from the given ItemInstance.
            // This is used to calculate the size the item takes up
            // and for rendering the individual pieces of it.
            var boxModel = this.boxBuilder.Build( item, context.Statable, context.EquipmentStatus );

            // Rebuild the model with enabled space compression
            // if it is to large.
            if( boxModel.TotalSize.Y > this.viewSize.Y )
            {
                boxModel = this.boxBuilder.BuildCompressed( item, boxModel, context.Statable, context.EquipmentStatus );
            }

            // Render the box model of the item to the render target
            // of this CachingItemInfo.
            this.texture = this.renderer.Render( boxModel, this.renderTarget );

            // Cache remaining properties.
            this.size = boxModel.TotalSize;
        }

        /// <summary>
        /// Draws the ItemInfo cached in this CachingItemInfo.
        /// </summary>
        /// <param name="position">
        /// The position to draw the ItemInfo at.
        /// </param>
        /// <param name="depth">
        /// The depth at which the ItemInfo should be drawn at.
        /// </param>
        /// <param name="alpha">
        /// The alpha color. 1=fully visible.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( Vector2 position, float depth, float alpha, ZeldaDrawContext drawContext )
        {
            if( this.IsInvalid )
                return;

            var rectangle = new Microsoft.Xna.Framework.Rectangle( 0, 0, this.size.X, this.size.Y );

            drawContext.Batch.Draw(
                this.texture,
                position,
                rectangle,
                new Xna.Color( 1.0f, 1.0f, 1.0f, alpha ),
                0.0f,
                Xna.Vector2.Zero,
                Xna.Vector2.One,
                SpriteEffects.None,
                depth
            );
        }

        /// <summary>
        /// Invalidates this ItemInfo; requiring it to be redrawn using <see cref="RenderCache"/>.
        /// </summary>
        public void Invalidate()
        {
            this.item = null;
            this.texture = null;
        }

        /// <summary>
        /// The item this ItemInfo instance is visualizing.
        /// </summary>
        private ItemInstance item;

        /// <summary>
        /// The offscreen target into which this I
        /// </summary>
        private RenderTarget2D renderTarget;

        /// <summary>
        /// The pixels the rendered Into Information takes up in the RenderTarget.
        /// </summary>
        private Point2 size;

        /// <summary>
        /// The size of the game view.
        /// </summary>
        private Point2 viewSize;

        /// <summary>
        /// The texture that contains the result of drawing the information about the item.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The ItemInfoRenderer this CachingItemInfo uses to render itself.
        /// </summary>
        private readonly ItemInfoRenderer renderer;

        /// <summary>
        /// The ItemInfoBoxBuilder this CachingItemInfo uses to build a box model
        /// from the ItemInstance.
        /// </summary>
        private readonly ItemInfoBoxBuilder boxBuilder;
    }
}
