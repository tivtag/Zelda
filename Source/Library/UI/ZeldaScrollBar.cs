// <copyright file="ZeldaScrollBar.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.ZeldaScrollBar class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI.Controls;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Represents a <see cref="ScrollBar"/> that is used in the Zelda game..
    /// </summary>
    public sealed class ZeldaScrollBar : ScrollBar
    {        
        /// <summary>
        /// Initializes a new instance of the ZeldaScrollBar class.
        /// </summary>
        public ZeldaScrollBar()
        {
            this.FloorNumber = 4;
            this.HideAndDisableNoEvent();
        }

        /// <summary>
        /// Called when this ZeldaScrollBar is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            this.DrawSlider( zeldaDrawContext );
            this.DrawThumb( zeldaDrawContext );
        }

        /// <summary>
        /// Draws the slider area of this ZeldaScrollBar.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawSlider( ZeldaDrawContext drawContext )
        {
            // ##  slider dummy area
            // == actual slider area
            //
            // ##===============##
            //
            
            // Full slider including dummy area
            drawContext.Batch.DrawRect(
                (Rectangle)this.ClientArea,
                Xna.Color.Red
            );

            // Actual slider area
            drawContext.Batch.DrawRect(
                (Rectangle)this.SliderArea,
                Xna.Color.LightGray.WithAlpha( 180 ),
                0.1f
            );
        }

        /// <summary>
        /// Draws the thumb of this ZeldaScrollBar.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawThumb( ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect(
                new Rectangle( this.ThumbArea.X, this.ThumbArea.Y, this.ThumbArea.Width, this.ThumbArea.Height ),
                Xna.Color.DarkGray,
                0.2f
            ); 

            drawContext.Batch.DrawRect(
                new Rectangle( this.ThumbArea.X + 1, this.ThumbArea.Y + 1, this.ThumbArea.Width-2, this.ThumbArea.Height - 2 ),
                Xna.Color.Black,
                0.3f
            ); 
        }
    }
}
