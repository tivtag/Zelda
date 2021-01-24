// <copyright file="ToolApplication.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ToolApplication class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Atom;

    /// <summary>
    /// Represents the abstract base application class that all Zelda tool applications
    /// inherit from.
    /// </summary>
    public abstract class ToolApplication : Application, IZeldaServiceProvider
    {
        #region [ Properties ]

        #region [ Graphics ]
        
        /// <summary>
        /// Gets or sets the size the view area takes up in pixels.
        /// </summary>
        public virtual Atom.Math.Point2 ViewSize
        {
            get
            {
                return Atom.Math.Point2.Zero;
            }
        }

        /// <summary>
        /// Gets a texture that can be used to render arabitary colored rectangles.
        /// </summary>
        public virtual Microsoft.Xna.Framework.Graphics.Texture2D WhiteTexture
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region [ Loaders ]
        
        /// <summary>
        /// Gets the <see cref="Atom.Xna.ITexture2DLoader"/> responsible for loading texture resources.
        /// </summary>
        public virtual Atom.Xna.ITexture2DLoader TextureLoader
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Atom.Xna.ISpriteLoader"/> responsible for loading sprite resources.
        /// </summary>
        public virtual Atom.Xna.ISpriteLoader SpriteLoader
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Atom.Xna.ISpriteSheetLoader"/> responsible for loading sprite sheet resources.
        /// </summary>
        public virtual Atom.Xna.ISpriteSheetLoader SpriteSheetLoader
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region [ Managers ]

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Entities.EntityReaderWriterManager"/> that manages the serialization of ZeldaEntities.
        /// </summary>
        public Zelda.Entities.EntityReaderWriterManager EntityReaderWriterManager
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Entities.EntityTemplateManager"/> that manages the creation of
        /// new ZeldaEntities based upon existing template entities.
        /// </summary>
        public Zelda.Entities.EntityTemplateManager EntityTemplateManager
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Entities.Behaviours.BehaviourManager"/> that manages
        /// the creation and initialization of IEntityBehaviour instances.
        /// </summary>
        public Zelda.Entities.Behaviours.BehaviourManager BehaviourManager
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Entities.Drawing.DrawStrategyManager"/> that manages
        /// the creation and initialization of IDrawDataAndStrategy instances.
        /// </summary>
        public Zelda.Entities.Drawing.DrawStrategyManager DrawStrategyManager
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Items.ItemManager"/> that manages
        /// the loading and caching of Items.
        /// </summary>
        public Zelda.Items.ItemManager ItemManager
        {
            get;
            protected set;
        }

        #endregion

        #region [ Others ]

        /// <summary>
        /// Gets the ILog of this ToolApplication.
        /// </summary>
        public Atom.Diagnostics.ILog Log
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the random number generator this ToolApplication uses.
        /// </summary>
        public Atom.Math.RandMT Rand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the XNA Game object that runs in the background of this ToolApplication.
        /// </summary>
        public abstract Microsoft.Xna.Framework.Game Game
        {
            get;
        }

        /// <summary>
        /// Gets the current ContentManager.
        /// </summary>
        public Microsoft.Xna.Framework.Content.ContentManager Content
        {
            get
            {
                return this.Game.Content;
            }
        }

        /// <summary>
        /// Gets or sets the FMOD AudioSystem responsible for loading and managing audio resources.
        /// </summary>
        public Zelda.Audio.ZeldaAudioSystem AudioSystem
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <see cref="Atom.Patterns.Provider.IObjectProviderContainerRegistrar"/> that can be used to receive
        /// and register IObjectProviders.
        /// </summary>
        public Atom.Patterns.Provider.IObjectProviderContainerRegistrar ProviderContainer
        {
            get
            {
                return this.providerContainer;
            }
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ToolApplication class.
        /// </summary>
        public ToolApplication()
        {
            throw new NotSupportedException( "The default constructor is required because of a WPF/XAML bug." );
        }

        /// <summary>
        /// Initializes a new instance of the ToolApplication class.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the new ToolApplication.
        /// </param>
        protected ToolApplication( string applicationName )
        {
            this.Log = LogHelper.CreateAndInitialize( applicationName );
            this.Rand = new Atom.Math.RandMT();

            AppDomain.CurrentDomain.UnhandledException += this.OnAppDomainUnhandledException;
            Application.Current.DispatcherUnhandledException += this.OnApplicationDispatcherUnhandledException;
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
        public virtual object GetService( Type serviceType )
        {
            if( serviceType.IsAssignableFrom( GetType() ) )
            {
                return this;
            }

            object service = providerContainer.TryResolve( serviceType );
            if( service != null )
            {
                return service;
            }

            return this.Game.Services.GetService( serviceType );
        }

        /// <summary>
        /// Attempts to receive the IObjectProvider for the specified object type.
        /// </summary>
        /// <param name="type">
        /// The type of object for which an IObjectProvider should be requested.
        /// </param>
        /// <returns>
        /// The associated IObjectProvider; -or- null if no IObjectProvider has been registered
        /// at this IObjectProviderContainer for the specified <see cref="Type"/>.
        /// </returns>
        public Atom.IObjectProvider<object> TryGetObjectProvider( Type type )
        {
            return this.providerContainer.TryGetObjectProvider( type );
        }

        /// <summary>
        /// Gets called when an unhandled exception has occurred.
        /// </summary>
        /// <param name="exceptionObject">Th exception object.</param>
        /// <param name="isCritical">Whether the exception was critical.</param>
        private void OnUnhandledException( object exceptionObject, bool isCritical )
        {
            string caption = isCritical ? 
                Atom.ErrorStrings.CriticalUnhandledExceptionHasOccurred : Atom.ErrorStrings.UnhandledExceptionHasOccurred;

            string text = exceptionObject.ToString();

            MessageBox.Show( text, caption, MessageBoxButton.OK, isCritical ? MessageBoxImage.Error : MessageBoxImage.Warning );
            this.Log.Write( string.Format( CultureInfo.CurrentCulture, "{0}\n\n{1}\n", caption, text ) );
        }

        /// <summary>
        /// Gets called when an unhandled exception occurred in the current AppDomain.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The UnhandledExceptionEventArgs that contains the event data.
        /// </param>
        private void OnAppDomainUnhandledException( object sender, UnhandledExceptionEventArgs e )
        {
            this.OnUnhandledException( e.ExceptionObject, e.IsTerminating );
        }

        /// <summary>
        /// Gets called when an unhandled exception occurred in the app's dispatcher.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The DispatcherUnhandledExceptionEventArgs that contains the event data.
        /// </param>
        private void OnApplicationDispatcherUnhandledException( 
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        {
            this.OnUnhandledException( e.Exception, true );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Provides a mechanism for receiving and registering IObjectProviders.
        /// </summary>
        private readonly Atom.Patterns.Provider.IObjectProviderContainerRegistrar providerContainer =
                    new Atom.Patterns.Provider.ObjectProviderContainer();

        #endregion
    }
}
