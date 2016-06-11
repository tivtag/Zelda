// <copyright file="ItemInfoCache.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.ItemInfoCache class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.UI.Items
{
    using Atom;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.UI.Items.Boxes;

    /// <summary>
    /// Implements the chaching mechanism for the <see cref="ItemInfo"/> class.
    /// </summary>
    internal sealed class ItemInfoCache : IZeldaSetupable
    {
        /// <summary>
        /// Initializes a new instance of the ItemInfoCache class.
        /// </summary>
        /// <param name="boxBuilder">
        /// The ItemInfoBoxBuilder to use when preparing ItemInstances
        /// for rendering into the new CachingItemInfo.
        /// </param>
        /// <param name="renderer">
        /// The ItemInfoRenderer to use when rendering ItemInstances
        /// into the new CachingItemInfo.
        /// </param>
        public ItemInfoCache( ItemInfoBoxBuilder boxBuilder, ItemInfoRenderer renderer )
        {
            this.boxBuilder = boxBuilder;
            this.renderer = renderer;
            this.cache = new ItemInfo( this.boxBuilder, this.renderer );
        }

        /// <summary>
        /// Setups this ItemInfoCache.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            var rescaler = serviceProvider.GetService<Zelda.Graphics.IViewToWindowRescaler>();
            rescaler.ScaleChanged += (sender, e) => this.Invalidate();

            this.cache.Setup( serviceProvider );
        }

        /// <summary>
        /// Gets the ItemInfo for the given ItemInstance.
        /// </summary>
        /// <param name="context">
        /// Encapsulates all draw and configuration data required for this operation.
        /// </param>
        /// <returns></returns>
        public ItemInfo GetItemInfoFor( ItemInfoVisualizationDrawContext context )
        {
            if( this.cache.Item == context.ItemInstance && !this.cache.IsInvalid )
            {
                return this.cache;
            }

            this.cache.RenderCache( context );
            return this.cache;
        }

        /// <summary>
        /// Invalidates this ItemInfoCache.
        /// </summary>
        public void Invalidate()
        {
            this.cache.Invalidate();
        }

        /// <summary>
        /// The ItemInfo that contains the last rendered item data.
        /// </summary>
        private readonly ItemInfo cache;

        /// <summary>
        /// The ItemInfoRenderer that is used to render the ItemInstances
        /// into CachingItemInfos.
        /// </summary>
        private readonly ItemInfoRenderer renderer;

        /// <summary>
        /// The ItemInfoBoxBuilder that is used to split items into individual
        /// renderable boxes that contain information.
        /// </summary>
        private readonly ItemInfoBoxBuilder boxBuilder;
    }
}
