// <copyright file="ItemInfoVisualizer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.ItemInfoVisualizer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Items
{
    using Atom.Math;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.UI.Items.Boxes;

    /// <summary>
    /// Implements an IItemInfoVisualizer that creates ItemInfos by splitting Items into
    /// individual IBoxes of data and provides caching of the last draw ItemInfos.
    /// </summary>
    internal sealed class ItemInfoVisualizer : IItemInfoVisualizer
    {
        /// <summary>
        /// Initializes a new instance of the ItemInfoVisualizer class.
        /// </summary>
        public ItemInfoVisualizer()
        {
            this.boxBuilder = new ItemInfoBoxBuilder( this.resources );
            this.cache = new ItemInfoCache( this.boxBuilder, this.renderer );
        }

        /// <summary>
        /// Setups this NewItemInfoVisualizer.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.resources.Setup( serviceProvider );
            this.renderer.Setup( serviceProvider );
            this.cache.Setup( serviceProvider );
        }

        /// <summary>
        /// Draws information about the given Item at the given position and depth,
        /// selecting the correct drawing method.
        /// </summary>
        /// <param name="context">
        /// Encapsulates all draw and configuration data required for this operation.
        /// </param>
        public void Draw( ItemInfoVisualizationDrawContext context )
        {
            var itemInfo = this.cache.GetItemInfoFor( context );
            Point2 position = GetDrawPosition( context.PositionX, context.PositionY, itemInfo, context.DrawContext );

            itemInfo.Draw( 
                position,
                context.Depth,
                context.Alpha,
                context.DrawContext 
            );
        }

        /// <summary>
        /// Tries to ensure that the given position is drawing the given
        /// ItemInfo within the game view.
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="itemInfo"></param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        /// <returns>
        /// The draw position.
        /// </returns>
        private static Point2 GetDrawPosition( 
            int positionX, 
            int positionY,
            ItemInfo itemInfo,
            ZeldaDrawContext drawContext )
        {
            Point2 viewSize = drawContext.Camera.ViewSize;
            Point2 size = itemInfo.Size;

            if( positionX + size.X > viewSize.X )
            {
                positionX = (int)(viewSize.X - size.X);
            }

            if( positionY + size.Y > viewSize.Y )
            {
                positionY = (int)(viewSize.Y - size.Y);
            }

            if( positionY < 0 )
            {
                positionY = 0;
            }

            return new Point2( positionX, positionY );
        }

        /// <summary>
        /// Clears the Item Info cache used by this IItemInfoVisualizer.
        /// </summary>
        public void ResetCache()
        {
            this.cache.Invalidate();
        }
        
        private readonly ItemInfoResources resources = new ItemInfoResources();
        private readonly ItemInfoRenderer renderer = new ItemInfoRenderer();
        private readonly ItemInfoCache cache;
        private readonly ItemInfoBoxBuilder boxBuilder;
    }
}
