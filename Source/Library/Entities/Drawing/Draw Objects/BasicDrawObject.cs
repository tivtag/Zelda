// <copyright file="BasicDrawObject.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.BasicDrawObject class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom;

    /// <summary>
    /// Implements some of the properties of a <see cref="IFloorDrawable"/> to allow easy-creation
    /// of new independent draw objects.
    /// </summary>
    internal abstract class BasicDrawObject : IZeldaFloorDrawable
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the visabilty state of this <see cref="IFloorDrawable"/> has changed.
        /// </summary>
        public event EventHandler IsVisibleChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the relative draw order of this <see cref="BasicDrawObject"/>.
        /// </summary>
        public abstract float RelativeDrawOrder
        {
            get;
        }

        /// <summary>
        /// Gets the secondary draw order value of this IZeldaFloorDrawable.
        /// </summary>
        /// <value>
        /// This value is used as a secondary sorting-value that is
        /// used when the RelativeDrawOrder of two IZeldaFloorDrawable is equal.
        /// </value>
        public virtual float SecondaryDrawOrder
        {
            get { return 0.0f; }
        }

        /// <summary>
        /// Gets the floor number this <see cref="BasicDrawObject"/> is drawn on.
        /// </summary>
        public abstract int FloorNumber
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BasicDrawObject"/> is visible.
        /// </summary>
        /// <value>The default value is true.</value>
        public virtual bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if( value == this.isVisible )
                    return;

                this.isVisible = value;
                this.IsVisibleChanged.Raise( this );
            }
        }

        /// <summary>
        /// Gets or sets the ZeldaScene this TileAreaTriggerDrawObject is part of.
        /// </summary>
        public ZeldaScene Scene
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Draws this <see cref="TileAreaTriggerDrawObject"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current DrawContext.
        /// </param>
        public abstract void Draw( IDrawContext drawContext );

        /// <summary>
        /// Called before drawing anything is drawn.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void PreDraw( ZeldaDrawContext drawContext )
        {
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores whether this IFloorDrawable is visible.
        /// </summary>
        private bool isVisible = true;

        #endregion
    }
}
