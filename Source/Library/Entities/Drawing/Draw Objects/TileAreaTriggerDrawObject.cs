// <copyright file="TileAreaTriggerDrawObject.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.TileAreaTriggerDrawObject class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Events;
    using Microsoft.Xna.Framework;
    
    /// <summary>
    /// Defines an <see cref="IFloorDrawable"/> that can be used to visualize a <see cref="TileAreaEventTrigger"/>.
    /// </summary>
    internal sealed class TileAreaTriggerDrawObject : BasicDrawObject
    {
        #region [ Constants ]

        /// <summary>
        /// The color used to visualize a TileAreaEventTrigger.
        /// </summary>
        private static readonly Color DrawColor = new Color( 255, 50, 50, 150 );

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the relative draw order of this <see cref="TileAreaTriggerDrawObject"/>.
        /// </summary>
        public override float RelativeDrawOrder
        {
            get
            {
                if( this.Scene == null )
                    return 0.0f;

                float mapHeight = this.Scene.Map.Height * 16;
                float relativeY = (float)this.trigger.Area.Center.Y / mapHeight;

                return relativeY + (mapHeight * (int)EntityFloorRelativity.IsAbove);
            }
        }

        /// <summary>
        /// Gets the floor number this <see cref="TileAreaTriggerDrawObject"/> is drawn on.
        /// </summary>
        public override int FloorNumber
        {
            get 
            {
                return this.trigger.FloorNumber;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileAreaTriggerDrawObject"/> class.
        /// </summary>
        /// <param name="trigger">
        /// The TileAreaEventTrigger the new TileAreaTriggerDrawObject is going to draw.
        /// </param>
        public TileAreaTriggerDrawObject( TileAreaEventTrigger trigger )
        {
            Contract.Requires<ArgumentNullException>( trigger != null );

            this.trigger   = trigger;
            this.IsVisible = true;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Draws this <see cref="TileAreaTriggerDrawObject"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current DrawContext.
        /// </param>
        public override void Draw( IDrawContext drawContext )
        {
            if( !IsVisible )
                return;

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Draw the area of the trigger.
            zeldaDrawContext.Batch.DrawRect(
                trigger.Area,
                DrawColor
            );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The trigger this TileAreaTriggerDrawObject draws.
        /// </summary>
        private readonly TileAreaEventTrigger trigger;

        #endregion
    }
}
