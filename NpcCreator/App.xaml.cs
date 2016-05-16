// <copyright file="App.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.NpcCreator.App class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.NpcCreator
{
    using System.Windows;
    using Zelda.Difficulties;

    /// <summary>
    /// The NpcCreator tool allows the creation and modification of
    /// Non Player Characters, Enemies, Plants, and other entities.
    /// </summary>
    partial class App : ToolApplication
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Atom.Xna.ISpriteLoader"/> responsible for loading sprite resources.
        /// </summary>
        public override Atom.Xna.ISpriteLoader SpriteLoader
        {
            get
            {
                return this.XnaApp.SpriteLoader;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyWrapperFactory"/> object.
        /// </summary>
        internal PropertyWrapperFactory PropertyWrapperFactory
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Xna application object.
        /// </summary>
        internal XnaApp XnaApp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the XNA Game object that runs in the background of this ToolApplication.
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

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base( "NpcCreator" )
        {
            this.EntityReaderWriterManager = new Zelda.Entities.EntityReaderWriterManager();
            this.BehaviourManager          = new Zelda.Entities.Behaviours.BehaviourManager();
            this.DrawStrategyManager       = new Zelda.Entities.Drawing.DrawStrategyManager();
            this.PropertyWrapperFactory    = new PropertyWrapperFactory();
            this.EntityTemplateManager     = new Zelda.Entities.EntityTemplateManager( this );
            this.ItemManager               = new Zelda.Items.ItemManager( this );

            Zelda.Design.DesignTime.Initialize( this );
        }

        /// <summary>
        /// Loads the defaults of this App.
        /// </summary>
        public void LoadDefaults()
        {
            GameDifficulty.Current = DifficultyId.Easy;
            this.BehaviourManager.LoadDefaults( this );
            this.EntityReaderWriterManager.LoadDefaults( this );
            this.DrawStrategyManager.LoadDefaults( this );
            this.PropertyWrapperFactory.LoadDefaults( this );
        }

        #endregion
    }
}