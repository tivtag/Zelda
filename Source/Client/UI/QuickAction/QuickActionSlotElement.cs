// <copyright file="QuickActionSlotElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.QuickActionSlotElement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom.Xna.UI;
    using Zelda.QuickActions;

    /// <summary>
    /// Represents and visualizes a single QuickActionSlot.
    /// </summary>
    internal sealed class QuickActionSlotElement : UIElement
    {
        /// <summary>
        /// The size of a QuickActionSlot in pixels.
        /// </summary>
        private const int SlotWidth = 21, SlotHeight = 19;
        
        /// <summary>
        /// Fired when this QuickActionSlot has been clicked.
        /// </summary>
        public event MouseInputEventHandler Clicked;

        /// <summary>
        /// Gets or sets the QuickActionSlot that is associated with this QuickActionSlotElement.
        /// </summary>
        public QuickActionSlot Slot
        {
            get 
            {
                return this.slot;
            }

            set
            { 
                this.slot = value;
            }
        }

        /// <summary>
        /// Gets the IQuickAction that is associated with the QuickActionSlot
        /// visualized by this QuickActionSlotElement.
        /// </summary>
        public IQuickAction QuickAction
        {
            get 
            { 
                return this.slot.Action;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the QuickActionSlotElement class.
        /// </summary>
        /// <param name="slotDrawer">
        /// Provides a mechanism to draw a QuickActionSlot.
        /// </param>
        public QuickActionSlotElement( QuickActionSlotDrawer slotDrawer )
        {
            this.Size = new Atom.Math.Vector2( SlotWidth, SlotHeight );
            this.slotDrawer = slotDrawer;
        }

        /// <summary>
        /// Called when this QuickActionSlotElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( this.slot.Action != null )
            {
                this.slotDrawer.Draw( slot.Action, this.Position, (ZeldaDrawContext)drawContext );
            }
        }

        /// <summary>
        /// Called when this QuickActionSlotElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Handles mouse input related to this QuickActionSlotElement.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse last frame.</param>
        /// <returns>Whether input should be passed to elements behind this element.</returns>
        protected override bool HandleRelatedMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( this.Clicked != null )
            {
                this.Clicked( this, ref mouseState, ref oldMouseState );
            }

            return false;
        }

        /// <summary>
        /// Identifies the QuickActionSlot that gets visualized by this QuickActionSlotElement.
        /// </summary>
        private QuickActionSlot slot;
        
        /// <summary>
        /// Provides a mechanism to draw a QuickActionSlot.
        /// </summary>
        private readonly QuickActionSlotDrawer slotDrawer;
    }
}
