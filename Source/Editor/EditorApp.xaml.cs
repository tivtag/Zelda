// <copyright file="EditorApp.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.EditorApp class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Atom;
    using Atom.Diagnostics;

    /// <summary>
    /// The Zelda.Editor allows the creation and modification of ZeldaScenes.
    /// It is the main content creation tool of the game.
    /// </summary>
    public partial class EditorApp
    {
        #region [ Properties ]

        #region > Graphics <

        /// <summary>
        /// Gets the size of the (unscaled) game window.
        /// </summary>
        public override Atom.Math.Point2 ViewSize
        {
            get 
            {
                var bounds = this.AppObject.Window.ClientBounds;
                return new Atom.Math.Point2( bounds.Width, bounds.Height );
            }
        }

        /// <summary>
        /// Receives a Texture object which can be used to easily draw
        /// any colored Rectangle.
        /// </summary>
        public override Microsoft.Xna.Framework.Graphics.Texture2D WhiteTexture
        {
            get
            {
                return this.xnaApp.WhiteTexture;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading ISprite assets.
        /// </summary>
        public override Atom.Xna.ISpriteLoader SpriteLoader
        {
            get
            {
                return this.xnaApp.SpriteLoader;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading Texture2D assets.
        /// </summary>
        public override Atom.Xna.ITexture2DLoader TextureLoader
        {
            get
            {
                return this.xnaApp.TextureLoader;
            }
        }

        /// <summary>
        /// Gets the IAssetLoader responsible for loading ISpriteSheet assets.
        /// </summary>
        public override Atom.Xna.ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return this.xnaApp.SpriteSheetLoader;
            }
        }
        
        #endregion

        #region > Managers <

        /// <summary>
        /// Receives the <see cref="Zelda.Editor.Object.ObjectPropertyWrapperFactory"/> object.
        /// </summary>
        internal Zelda.Editor.Object.EntityPropertyWrapperFactory ObjectPropertyWrapperFactory
        {
            get;
            private set;
        }

        #endregion

        #region > Misc <

        /// <summary>
        /// Receives the <see cref="EditorApp"/> object of the current AppDomain.
        /// </summary>
        public new static EditorApp Current
        {
            get 
            {
                return (EditorApp)Application.Current;
            }
        }

        /// <summary>
        /// Receives the Xna application object.
        /// </summary>
        internal XnaEditorApp AppObject
        {
            get 
            {
                return this.xnaApp; 
            }

            set
            {
                this.xnaApp = value; 
            }
        }
        
        /// <summary>
        /// Receives the Xna application object.
        /// </summary>
        public override Microsoft.Xna.Framework.Game Game
        {
            get 
            {
                return this.xnaApp; 
            }
        }

        #endregion

        #endregion

        #region [ Initialiation ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorApp"/> class.
        /// </summary>
        public EditorApp()
            : base( "Editor" )
        {
            this.AudioSystem = new Zelda.Audio.ZeldaAudioSystem();
            this.DrawStrategyManager = new Entities.Drawing.DrawStrategyManager();
            this.EntityReaderWriterManager = new Entities.EntityReaderWriterManager();
            this.BehaviourManager = new Entities.Behaviours.BehaviourManager();

            this.ObjectPropertyWrapperFactory = new Zelda.Editor.Object.EntityPropertyWrapperFactory( this );
            this.EntityTemplateManager        = new Zelda.Entities.EntityTemplateManager( this );
            this.ItemManager                  = new Zelda.Items.ItemManager( this );

            // Various other initializations
            Zelda.Events.ZeldaEventManager.RegisterEvents();
            Zelda.Design.DesignTime.Initialize( this );
            Zelda.ZeldaScene.EditorMode = true;

            GlobalServices.Container.AddService<IZeldaServiceProvider>( this );     
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public void Initialize()
        {
            this.DrawStrategyManager.LoadDefaults( this );
            this.BehaviourManager.LoadDefaults( this );
            this.EntityReaderWriterManager.LoadDefaults( this );

            this.AudioSystem.Initialize( 64 );
        }

        /// <summary>
        /// Called when the EditorApp is closing for good.
        /// </summary>
        /// <param name="e">
        /// The ExitEventArgs that contain the event data.
        /// </param>
        protected override void OnExit( ExitEventArgs e )
        {
            this.AudioSystem.Shutdown();
            base.OnExit( e );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Reference ot the xna application object.
        /// </summary>
        private XnaEditorApp xnaApp;

        #endregion
    }
}
