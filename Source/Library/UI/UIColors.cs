// <copyright file="UIColors.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.UI.UIColors class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Static class that contains shared color constants used in the UserInterface.
    /// </summary>
    public static class UIColors
    {  
        /// <summary>
        /// The first color used for positive 'events' and UI elements.
        /// </summary>
        public static readonly Color PositiveDark = new Color( 83, 202, 50, 255 );

        /// <summary>
        /// The secondary color used for positive 'events' and UI elements.
        /// </summary>
        public static readonly Color PositiveLight = new Color( 97, 229, 61, 255 );
        
        /// <summary>
        /// The secondary color used for positive 'events' and UI elements.
        /// </summary>
        public static readonly Color PositiveBright = new Color( 107, 255, 67, 255 );

        /// <summary>
        /// The first color used for negative 'events' and UI elements.
        /// </summary>
        public static readonly Color NegativeDark = new Color( 215, 1, 1, 255 );
            
        /// <summary>
        /// The secondary color used for negative 'events' and UI elements.
        /// </summary>
        public static readonly Color NegativeLight = new Color( 200, 13, 13, 255 );
        
        /// <summary>
        /// The secondary color used for negative 'events' and UI elements.
        /// </summary>
        public static readonly Color NegativeBright = new Color( 255, 44, 44, 255 );

        /// <summary>
        /// Specifies the color of the rectangle that is drawn in the background of
        /// an IngameWindow.
        /// </summary>
        public static readonly Color LightWindowBackground = new Color( 255, 255, 255, 153 );

        /// <summary>
        /// Specifies the color of the rectangle that is drawn in the background of
        /// title of an IngameWindow.
        /// </summary>
        public static readonly Color WindowTitleBackground = new Color( 0, 0, 0, 200 );

        /// <summary>
        /// Specifies the color of the rectangle that is drawn in the background of
        /// an IngameWindow.
        /// </summary>
        public static readonly Color DarkWindowBackground = new Color( 0, 0, 0, 155 );

        /// <summary>
        /// Specifies the color used to visualize a Cooldown.
        /// </summary>
        public static readonly Color Cooldown = new Color( 0, 0, 0, 154 );

        /// <summary>
        /// Specifies the color used to tell that a requirement is not fulfilled.
        /// </summary>
        public static readonly Color RequirementNotFulfilled = Color.Red;

        /// <summary>
        /// Specifies the color used by temp. highlighted buttons.
        /// </summary>
        public static readonly Color MarkedButton = PositiveBright;

        /// <summary>
        /// Specifies the color used by the buttons in the upper title bar in ingame windows.
        /// </summary>
        public static readonly Color DefaultUpperRowButton = new Color( 255, 255, 255, 155 );
        
        /// <summary>
        /// Gets the color of the given <see cref="Zelda.Items.ItemQuality"/>.
        /// </summary>
        /// <param name="itemQuality">
        /// The enumeration to get the associated Color for.
        /// </param>
        /// <returns>
        /// The associated Color.
        /// </returns>
        public static Color Get( Zelda.Items.ItemQuality itemQuality )
        {
            switch( itemQuality )
            {
                case Zelda.Items.ItemQuality.Rubbish:
                    return Color.Gray;

                case Zelda.Items.ItemQuality.Quest:
                    return Color.Yellow;

                case Zelda.Items.ItemQuality.Common:
                    return Color.White;

                case Zelda.Items.ItemQuality.Magic:
                    return new Color( 48, 208, 24 );

                case Zelda.Items.ItemQuality.Rare:
                    return new Color( 32, 104, 200 );

                case Zelda.Items.ItemQuality.Epic:
                    return new Color( 200, 32, 38 );

                case Zelda.Items.ItemQuality.Legendary:
                    return new Color( 0, 162, 255 );

                case Zelda.Items.ItemQuality.Artefact:
                    return Color.Gold;

                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
