

namespace Zelda.UI
{
    using Zelda.Items;
    using Zelda.Status;

    public sealed class ItemInfoVisualizationDrawContext
    {            
        /// <summary>
        /// The position of the upper-left corner on the x-axis.
        /// Might be modified to make the visualization to fit on the screen.
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// The position of the upper-left corner on the y-axis.
        /// Might be modified to make the visualization to fit on the screen.
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// The depth to start drawing at.
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// The alpha color. 1=fully visible.
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// The instance of the Item to draw.
        /// </summary>
        public ItemInstance ItemInstance { get; set; }

        /// <summary>
        /// The ExtendedStatable component of the entity that owns the given itemInstance.
        /// </summary>
        public ExtendedStatable Statable { get; set; }

        /// <summary>
        /// The current ZeldaDrawContext.
        /// </summary>
        public ZeldaDrawContext DrawContext { get; set; }

        public EquipmentStatus EquipmentStatus { get; set; }
    }
}
