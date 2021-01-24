// <copyright file="App.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.App class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.ItemCreator
{
    using System;
    using System.Windows;

    /// <summary>
    /// The ItemCreator tool allows the creation and modification of Item,
    /// Equipment and Weapon defintion files.
    /// </summary>
    public sealed partial class App : ToolApplication, IZeldaServiceProvider
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Xna application object.
        /// </summary>
        internal XnaApp XnaApp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Xna application object.
        /// </summary>
        public override Microsoft.Xna.Framework.Game Game
        {
            get 
            {
                return this.XnaApp; 
            }
        }

        /// <summary>
        /// Gets the currently running application object.
        /// </summary>
        public static new App Current
        {
            get
            {
                return (App)Application.Current; 
            }
        }

        /// <summary>
        /// Gets the ISpriteLoader object that provides a mechanism for loading ISprite assets. 
        /// </summary>
        public override Atom.Xna.ISpriteLoader SpriteLoader
        {
            get
            {
                return this.XnaApp.SpriteLoader;            
            }
        }

        /// <summary>
        /// Gets the ISpriteSheetLoader object that provides a mechanism for loading ISpriteSheet assets. 
        /// </summary>
        public override Atom.Xna.ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return this.XnaApp.SpriteSheetLoader;
            }
        }

        /// <summary>
        /// Gets the <see cref="Atom.Xna.ITexture2DLoader"/> responsible for loading texture resources.
        /// </summary>
        public override Atom.Xna.ITexture2DLoader TextureLoader
        {
            get
            {
                return this.XnaApp.TextureLoader;  
            }
        }        
        
        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base( "ItemCreator" )
        {
            var audio = new Zelda.Audio.ZeldaAudioSystem();
            this.AudioSystem = audio;
            this.setDatabase = new Zelda.Items.Sets.SetDatabase( this );

            this.ItemManager = new Zelda.Items.ItemManager( this );
            this.EntityReaderWriterManager = new Zelda.Entities.EntityReaderWriterManager();
            this.BehaviourManager          = new Zelda.Entities.Behaviours.BehaviourManager();
            this.DrawStrategyManager       = new Zelda.Entities.Drawing.DrawStrategyManager();
            this.EntityTemplateManager     = new Zelda.Entities.EntityTemplateManager( this );

            this.BehaviourManager.LoadDefaults( this );
            this.EntityReaderWriterManager.LoadDefaults( this );
            this.DrawStrategyManager.LoadDefaults( this );

            Zelda.Design.DesignTime.Initialize( this );
            audio.Initialize( this );
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Tries to receive the service object of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceType">
        /// The type of the service object to receive.
        /// </param>
        /// <returns>
        /// The requested service object;
        /// or null if the service couldn't be found.
        /// </returns>
        public override object GetService( Type serviceType )
        {
            if( serviceType == typeof( Items.Sets.ISetDatabase ) )
                return this.setDatabase;

            return base.GetService( serviceType );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The ISetDatabase object.
        /// </summary>
        private readonly Zelda.Items.Sets.ISetDatabase setDatabase;

        #endregion
    }
}
