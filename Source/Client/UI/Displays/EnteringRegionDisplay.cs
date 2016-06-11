// <copyright file="EnteringRegionDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.EnteringRegionDisplay class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System;
    using Atom.Math;
    using Atom.Xna.Effects;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Displays the (blending in-and-out) name of the region 
    /// the player is about to enter.
    /// </summary> 
    internal sealed class EnteringRegionDisplay : TextField
    {
        /// <summary>
        /// The base color of the Text.
        /// </summary>
        private static readonly Xna.Color TextColor = new Xna.Color( 255, 255, 255, 0 );

        /// <summary>
        /// Initializes a new instance of the EnteringRegionDisplay class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal EnteringRegionDisplay( IZeldaServiceProvider serviceProvider )
        {
            var viewSize = serviceProvider.ViewSize;

            // Set Properties.
            this.Position    = new Vector2( (int)(viewSize.X / 2.0f), (int)(viewSize.Y / 3.0f) );
            this.FloorNumber = 1;

            // Create Text.
            this.Text = new Text( UIFonts.VerdanaBold11, TextAlign.Center, TextColor ) {
                LayerDepth = 0.9f
            };

            var blendEffect = new AlphaBlendInOutColorEffect( 8.0f, 3.0f, 5.0f );
            blendEffect.Ended += this.OnBlendEffectEnded;

            this.Text.AddColorEffect( blendEffect );
        }

        /// <summary>
        /// Commands this EnteringRegionDisplay to show 
        /// entering of the region with the given <paramref name="regionName"/>.
        /// </summary>
        /// <param name="regionName">
        /// The (localized) name of the region.
        /// </param>
        public void Show( string regionName )
        {
            string text;
            
            if( regionName != null )
            {
                text = "** " + regionName + " **";
            }
            else
            {
                text = string.Empty;
            }

            this.Text.TextString = text;
            this.Text.ResetColorEffects();
            this.ShowAndEnable();
        }
        
        /// <summary>
        /// Called when the blending effect applied to the Text has ended.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnBlendEffectEnded( object sender, EventArgs e )
        {
            Reset();
        }

        public void Reset()
        {
            this.HideAndDisable();
            this.Text.Color = TextColor;
        }
    }
}
