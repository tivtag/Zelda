// <copyright file="Settings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaGame class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System.IO;
    using System.Text;
    using Atom.Configuration;
    using Zelda.Graphics;

    /// <summary>
    /// Stores the configuration data of the zelda game.
    /// </summary>
    public sealed class Settings : Config
    {
        /// <summary>
        /// Gets or sets the resolution of the game on the x-axis.
        /// </summary>
        [ConfigProperty( DefaultValue = 720, StorageName = "width" )]
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the resolution of the game on the y-axis.
        /// </summary>
        [ConfigProperty( DefaultValue = 480, StorageName = "height" )]
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this game
        /// currently runs in fullscreen mode.
        /// </summary>
        [ConfigProperty( DefaultValue = false, StorageName = "fullscreen" )]
        public bool IsFullscreen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the game.
        /// </summary>
        /// <remarks>
        /// If false then the game will
        /// </remarks>
        [ConfigProperty( DefaultValue = AspectRatio.Normal, StorageName = "aspect-ratio", Comment = "Supported aspect-ratios: Normal, Wide16to9, Wide16to10" )]
        public AspectRatio AspectRatio
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the game
        /// will stretch the game view to the fully available space if <see cref="IsFullscreen"/> is true.
        /// </summary>
        /// <remarks>
        /// If false then the game will add black-borders around the un-stretched game view.
        /// </remarks>
        [ConfigProperty( DefaultValue = false, StorageName = "stretch-fullscreen" )]
        public bool IsFullscreenStretched
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether virtual synchronisation is enabled.
        /// </summary>
        [ConfigProperty( DefaultValue = true, StorageName = "vsync" )]
        public bool VSync { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a fixed-step game loop is used.
        /// </summary>
        [ConfigProperty( DefaultValue = false, StorageName = "fixed-step" )]
        public bool FixedStep { get; set; }

        /// <summary>
        /// Gets or sets the name of the last saved profile.
        /// </summary>
        [ConfigProperty( DefaultValue = "", StorageName = "profile" )]
        public string LastSavedProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ruby count
        /// is permanently shown.
        /// </summary>
        [ConfigProperty( DefaultValue = false, StorageName = "ruby-count-shown" )]
        public bool RubyCountShown
        {
            get;
            set;
        }

        #region [ Audio ]

        /// <summary>
        /// Gets or sets the master volume of the game; where 0 = silent and 1 = full volume.
        /// </summary>
        [ConfigProperty( DefaultValue = 1.0f, StorageName = "master-volume" )]
        public float MasterVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the music volume of the game; where 0 = silent and 1 = full volume.
        /// </summary>
        [ConfigProperty( DefaultValue = 1.0f, StorageName = "music-volume" )]
        public float MusicVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the effect volume of the game; where 0 = silent and 1 = full volume.
        /// </summary>
        [ConfigProperty( DefaultValue = 1.0f, StorageName = "effect-volume" )]
        public float EffectVolume
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the game automatically saves the progress of the player.
        /// </summary>
        [ConfigProperty( DefaultValue = true, StorageName = "auto-save" )]
        public bool AutoSaveEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the alpha-value of the color of the item description background. 0=invisible, 255=fully visible
        /// </summary>
        [ConfigProperty( DefaultValue = (byte)255, StorageName = "ui-item-desc-box-alpha", Comment = "The alpha-value of the color of the item description background. 0=invisible, 255=fully visible" )]
        public byte ItemDescriptionBoxAlpha
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the full path at which the settings file is located.
        /// </summary>
        private static string FileName
        {
            get
            {
                return Path.Combine( GameFolders.UserData, "settings.txt" );
            }
        }

        /// <summary>
        /// Stores the singleton instance of the Settings class.
        /// </summary>
        private static readonly Settings instance = new Settings();

        /// <summary>
        /// Gets the singleton instance of the Settings class.
        /// </summary>
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Prevents the creation of <see cref="Settings"/> class.
        /// </summary>
        private Settings()
            : base( new PlainTextConfigStore( FileName, Encoding.UTF8 ) )
        {
        }
    }
}
