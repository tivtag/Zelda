// <copyright file="CastBarDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.CastBarDisplay class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

/*
using System;
using Atom.Math;
using Atom.Xna;
using Atom.Xna.UI;
using Zelda.Casting;
using XnaF = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zelda.UI
{
    /// <summary>
    /// Defines an UIElement that visualizes the CastBar of the Player.
    /// </summary>
    internal sealed class CastBarDisplay : UIElement
    {
        #region [ Constants ]

        /// <summary>
        /// The color of the cast bar.
        /// </summary>
        private static readonly Color ColorCastBar = new Color( 200, 25, 25, 200 );

        /// <summary>
        /// The size of the cast bar.
        /// </summary>
        private const int BarWidth = 20, BarHeight = 3;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the PlayerEntity whose CastBar is visualized by this CastBarDisplay.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the CastBarDisplay class.
        /// </summary>
        public CastBarDisplay()
        {
            this.IsEnabled = true;
            this.IsVisible = true;
            this.Size      = new Atom.Math.Vector2( BarWidth, BarHeight );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this CastBarDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var castBar = this.Player.Castable.CastBar;

            if( castBar.IsCasting )
            {
                var zeldaDrawContext = (ZeldaDrawContext)drawContext;

                var drawPosition = this.Player.Collision.Center - (this.Size / 2.0f) - zeldaDrawContext.Camera.Scroll;              
                var castRatio    = 1.0f - castBar.CastTimeLeft / castBar.CastTime;

                var rectangle = new XnaF.Rectangle(
                    (int)drawPosition.X,
                    (int)drawPosition.Y + 10,
                    (int)(BarWidth * castRatio),
                    BarHeight
                );

                // Do the actual drawing.
                zeldaDrawContext.Batch.Draw(
                    zeldaDrawContext.WhiteTexture,
                    rectangle,
                    ColorCastBar
                );
            }
        }

        /// <summary>
        /// Called when this CastBarDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        #endregion
    }
}
*/