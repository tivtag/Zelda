// <copyright file="UIFonts.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.UI.UIFonts class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom.Xna.Fonts;

    /// <summary>
    /// Static class that allows access to the IFont assets used in the UserInterface.
    /// </summary>
    public static class UIFonts
    {
        /// <summary>
        /// Gets the "Tahoma, size 7" font asset.
        /// </summary>
        public static IFont Tahoma7       { get; private set; }

        /// <summary>
        /// Gets the "Tahoma, size 9" font asset.
        /// </summary>
        public static IFont Tahoma9       { get; private set; }

        /// <summary>
        /// Gets the "Tahoma, size 10" font asset.
        /// </summary>
        public static IFont Tahoma10      { get; private set; }

        /// <summary>
        /// Gets the "Tahoma, size 14" font asset.
        /// </summary>
        public static IFont Tahoma14      { get; private set; }
        
        /// <summary>
        /// Gets the "bold Tahoma, size 8" font asset.
        /// </summary>
        public static IFont TahomaBold8   { get; private set; }

        /// <summary>
        /// Gets the "bold Tahoma, size 10" font asset.
        /// </summary>
        public static IFont TahomaBold10  { get; private set; }

        /// <summary>
        /// Gets the "bold Tahoma, size 11" font asset.
        /// </summary>
        public static IFont TahomaBold11  { get; private set; }

        /// <summary>
        /// Gets the "bold Verdana, size 11" font asset.
        /// </summary>
        public static IFont VerdanaBold11 { get; private set; }

        /// <summary>
        /// Loads the font assets that are used in the UI.
        /// </summary>
        /// <param name="fontLoader">
        /// Provides a mechanism for loading IFont assets.
        /// </param>
        public static void Load( IFontLoader fontLoader )
        {
            Tahoma7       = fontLoader.Load( "Tahoma7" );
            Tahoma9       = fontLoader.Load( "Tahoma9" );
            Tahoma10      = fontLoader.Load( "Tahoma10" );
            Tahoma14      = fontLoader.Load( "Tahoma14" );
            TahomaBold8   = fontLoader.Load( "TahomaBold8" );
            TahomaBold10  = fontLoader.Load( "TahomaBold10" );
            TahomaBold11  = fontLoader.Load( "TahomaBold11" );
            VerdanaBold11 = fontLoader.Load( "Verdana11_Bold" );
        }
    }
}
