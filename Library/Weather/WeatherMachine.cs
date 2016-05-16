// <copyright file="WeatherMachine.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.WeatherMachine class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom.Math;
    using Atom.Diagnostics;
    using Zelda.Weather.Creators;
    using Atom;

    /// <summary>
    /// The WeatherMachine manages the creation and updating of the <see cref="IWeather"/> in a <see cref="ZeldaScene"/>.
    /// </summary>
    public sealed class WeatherMachine : IWeatherMachine
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether this WeatherMachine is currently activated.
        /// </summary>
        public bool IsActivated
        {
            get
            {
                return this.isActivated;
            }

            set
            {
                this.isActivated = value;
            }
        }

        /// <summary>
        /// Gets the ZeldaScene whose IWeather is controlled by this WeatherMachine.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        /// <summary>
        /// Gets the <see cref="IWeatherMachineSettings"/> of this WeatherMachine.
        /// </summary>
        public IWeatherMachineSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any weather effect is currently active.
        /// </summary>
        public bool HasActiveWeather
        {
            get
            {
                return this.currentWeather.Count > 0;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherMachine"/> class.
        /// </summary>
        /// <param name="scene">
        /// The ZeldaScene that owns the new <see cref="WeatherMachine"/>.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal WeatherMachine( ZeldaScene scene, IZeldaServiceProvider serviceProvider )
        {
            Debug.Assert( scene != null );
            Debug.Assert( serviceProvider != null );

            this.serviceProvider = serviceProvider;
            this.scene = scene;

            this.settings = new WeatherMachineSettings( serviceProvider.GetService<IWeatherMachineSettings>() );
            this.creatorMap = serviceProvider.GetService<IWeatherCreatorMap>();
            this.creatorProvider = CreateCreatorProvider( this.creatorMap, serviceProvider.Rand );
        }

        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            foreach( var weather in this.currentWeather )
            {
                var reloadable = weather as IReloadable;

                if( reloadable != null )
                {
                    reloadable.Reload( serviceProvider );
                }                
            }

            this.settings.Reload( serviceProvider );
        }

        /// <summary>
        /// Creates the IWeatherCreatorProvider to be used by the WeatherMachine.
        /// </summary>
        /// <param name="creators">
        /// The IWeatherCreatorMap that contains all the IWeatherCreators used by the new IWeatherCreatorProvider.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// The IWeatherCreatorProvider to use.
        /// </returns>
        private static IWeatherCreatorProvider CreateCreatorProvider( IWeatherCreatorMap creators, IRand rand )
        {
            if( DefaultWeatherCreatorProvider == null )
            {
                DefaultWeatherCreatorProvider = new MonthBasedRandomWeatherCreatorProvider( creators, rand );                
            }

            return DefaultWeatherCreatorProvider;
        }

        #endregion

        #region [ Methods ]

        #region > Updating <

        /// <summary>
        /// Updates this <see cref="WeatherMachine"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            // Update current IWeather
            for( int i = 0; i < this.currentWeather.Count; ++i )
            {
                this.currentWeather[i].Update( updateContext );
            }

            if( !this.isActivated )
                return;

            // Create new IWeather if requested:
            if( this.isNewWeatherRequested )
            {
                this.timeToNextWeatherCreation -= updateContext.FrameTime;
           
                if( this.timeToNextWeatherCreation <= 0.0f )
                {
                    this.CreateNewWeather();
                }
            }
        }

        #endregion

        #region > Organization <

         /// <summary>
        /// Sets the IWeather of this WeatherMachine to the IWeather created by the IWeatherCreator
        /// of the specified <typeparamref name="TWeatherCreator"/>.
        /// </summary>
        /// <typeparam name="TWeatherCreator">
        /// The type of the <see cref="IWeatherCreator"/> to use.
        /// </typeparam>
        /// <returns>
        /// Returns true if the weather could be set;
        /// otherwise false.
        /// </returns>
        public bool SetWeather<TWeatherCreator>()
            where TWeatherCreator : IWeatherCreator
        {
            return this.SetWeather( typeof( TWeatherCreator ) );
        }

        /// <summary>
        /// Sets the IWeather of this WeatherMachine to the IWeather created by the IWeatherCreator
        /// of the given <paramref name="creatorType"/>.
        /// </summary>
        /// <param name="creatorType"> 
        /// The type of the <see cref="IWeatherCreator"/> to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="creatorType"/> is null.
        /// </exception>
        /// <returns>
        /// Returns true if the weather could be set;
        /// otherwise false.
        /// </returns>
        public bool SetWeather( Type creatorType )
        {
            if( creatorType == null )
                throw new ArgumentNullException( "creatorType" );

            // Receive IWeatherCreator
            IWeatherCreator creator = this.creatorMap.Find( creatorType );
            
            // Throw on error
            if( creator == null )
            {
                string message = string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.Error_CouldntFindWeatherCreatorOfTypeX,
                    creatorType.ToString()
                );

                serviceProvider.Log.WriteLine( message );
                return false;
            }                     

            this.StopCurrentWeather();
            this.AddWeather( creator );
            return true;
        }

        public bool SetWeatherByTypeName( string creatorTypeName )
        {
            Type type = Type.GetType( creatorTypeName );
            return this.SetWeather( type );
        }

        /// <summary>
        /// Adds the IWeather created by the IWeatherCreator
        /// of the given <paramref name="creatorType"/> to this WeatherMachine.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="creatorType"/> is null.
        /// </exception>
        /// <exception cref="Atom.NotFoundException">
        /// If the IWeatherCreator of the given <paramref name="creatorType"/>
        /// was not present in this WeatherMachine.
        /// </exception>
        /// <param name="creatorType"> 
        /// The type of the <see cref="IWeatherCreator"/> to use.
        /// </param>
        /// <returns>
        /// Returns true if the Weather was successfully added;
        /// otherwise false.
        /// </returns>
        public bool AddWeather( Type creatorType )
        {
            if( creatorType == null )
                throw new ArgumentNullException( "creatorType" );

            IWeatherCreator creator = this.creatorMap.Find( creatorType );

            if( creator == null )
            {
                throw new Atom.NotFoundException(
                    string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_CouldntFindWeatherCreatorOfTypeX,
                        creatorType.ToString()
                    )
                );
            }

            this.AddWeather( creator );
            return true;
        }

        /// <summary>
        /// Adds the IWeather created by the specified IWeatherCreator to this WeatherMachine.
        /// </summary>
        /// <param name="creator">
        /// The IWeatherCreator to create the IWeather with.
        /// </param>
        private void AddWeather( IWeatherCreator creator )
        {
            Debug.Assert( creator != null );
            IWeather[] weather = creator.CreateWeather( this );

            foreach( var weatherEffect in weather )
            {
                weatherEffect.Start();
                this.currentWeather.Add( weatherEffect );
            }
        }

        #endregion

        /// <summary>
        /// Creates a new random IWeather effect,
        /// based on the current ingame month.
        /// </summary>
        private void CreateNewWeather()
        {
            if( this.currentWeather.Count != 0 )
                return;

            IWeatherCreator creator = this.creatorProvider.GetCreator( this );

            if( creator != null )
            {
                this.AddWeather( creator );
            }

            this.RequestWeatherChange();
        }
        
        // <summary>
        /// Notifies this WeatherMachine that a scene change has occured.
        /// </summary>
        /// <param name="changeType">
        /// States whether the current scene has changed away or to its current scene.
        /// </param>
        public void NotifySceneChange( ChangeType changeType )
        {
            if( changeType == ChangeType.To )
            {
                this.UnpauseCurrentWeather();
            }
            else
            {
                this.PauseCurrentWeather();
            }
        }

        /// <summary>
        /// Pauses the currently active IWeather.
        /// </summary>
        /// <seealso cref="IPauseableWeather"/>
        private void PauseCurrentWeather()
        {
            for( int i = 0; i < this.currentWeather.Count; ++i )
            {
                var pauseableWeather = this.currentWeather[i] as IPauseableWeather;

                if( pauseableWeather != null )
                {
                    pauseableWeather.Pause();
                }
            }
        }

        /// <summary>
        /// Unpauses the currently active IWeather.
        /// </summary>
        /// <seealso cref="IPauseableWeather"/>
        private void UnpauseCurrentWeather()
        {
            foreach( var weather in this.currentWeather )
            {
                var pauseableWeather = weather as IPauseableWeather;

                if( pauseableWeather != null )
                {
                    pauseableWeather.Unpause();
                }
            }
        }

        /// <summary>
        /// Tells this WeatherMachine to change to some random Weather.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        public void SetRandomWeather( RandMT rand )
        {
            this.StopCurrentWeather();

            var creator = this.creatorMap.GetRandom( rand );
            this.AddWeather( creator );
        }

        /// <summary>
        /// Immidiately removes all of the current IWeather simulated by this WeatherMachine.
        /// </summary>
        private void StopCurrentWeather()
        {
            for( int i = 0; i < this.currentWeather.Count; ++i )
            {
                this.currentWeather[i].Stop( false );
            }

            this.currentWeather.Clear();
        }

        /// <summary>
        /// Gets called when an <see cref="IWeather"/> has been stopped for this WeatherMachine.
        /// </summary>
        /// <param name="weather">
        /// The related IWeather instance.
        /// </param>
        public void OnWeatherStopped( IWeather weather )
        {
            if( this.currentWeather.Remove( weather ) )
            {
                if( this.currentWeather.Count == 0 )
                {
                    this.RequestWeatherChange();
                }
            }
        }

        /// <summary>
        /// Notifies this WeatherMachine that a change in IWeather is required.
        /// </summary>
        private void RequestWeatherChange()
        {
            var dateTime = this.scene.IngameDateTime;
            if( dateTime == null )
                return;

            float randValue = this.scene.Rand.RandomRange( 2250.0f, 8500.0f );

            this.isNewWeatherRequested     = true;
            this.timeToNextWeatherCreation = randValue * dateTime.InverseTickSpeed;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The list of currently active <see cref="IWeather"/> StatusEffects.
        /// </summary>
        private readonly List<IWeather> currentWeather = new List<IWeather>();

        /// <summary>
        /// States whether this WeatherMachine is going to create a new IWeather soon.
        /// </summary>
        private bool isNewWeatherRequested = true;

        /// <summary>
        /// The time (in seconds) left until this WeatherMachine is
        /// going to create a new IWeather based on the current IWeatherCreator Hat.
        /// </summary>
        /// <remarks>
        /// This value is only relevant when isUpdatingNewWeather is true.
        /// </remarks>
        private float timeToNextWeatherCreation;

        /// <summary>
        /// States whether this WeatherMachine is currently activated.
        /// </summary>
        private bool isActivated = true;

        /// <summary> 
        /// Provides access to all known IWeatherCreators.
        /// </summary>
        private readonly IWeatherCreatorMap creatorMap;

        /// <summary>
        /// Provides a mechanism that is used to get the IWeatherCreator
        /// that should be used when creating new IWeather.
        /// </summary>
        private readonly IWeatherCreatorProvider creatorProvider;

        /// <summary>
        /// Identifies the <see cref="ZeldaScene"/> that owns this WeatherMachine.
        /// </summary>
        private readonly ZeldaScene scene;

        /// <summary>
        /// Encapsulates the settings this WeatherMachine and its IWeatherCreators use to create new IWeather.
        /// </summary>
        private readonly IWeatherMachineSettings settings;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        /// <summary>
        /// Caches the IWeatherCreatorProvider that is used by default.
        /// </summary>
        private static IWeatherCreatorProvider DefaultWeatherCreatorProvider;

        #endregion
    }
}