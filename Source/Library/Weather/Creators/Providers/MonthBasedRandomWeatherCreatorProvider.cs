// <copyright file="MonthBasedRandomWeatherCreatorProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.MonthBasedRandomWeatherCreatorProvider class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System.Diagnostics;
    using Atom.Collections;
    using Atom.Math;

    /// <summary>
    /// Implements an IWeatherCreatorProvider that based on the current ingame month
    /// randomly chooses an IWeatherCreator.
    /// </summary>
    internal sealed class MonthBasedRandomWeatherCreatorProvider : IWeatherCreatorProvider
    {
        /// <summary>
        /// Initializes a new instance of the MonthBasedRandomWeatherCreatorProvider class.
        /// </summary>
        /// <param name="creators">
        /// Provides access to all known IWeaterCreators.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        public MonthBasedRandomWeatherCreatorProvider( IWeatherCreatorMap creators, IRand rand )
        {
            Debug.Assert( creators != null );
            Debug.Assert( rand != null );

            this.creators = creators;
            this.weatherHats = CreateWeatherHats( rand );
            this.SetupEuropeanWeather();
        }

        /// <summary>
        /// Creates the array of IWeatherCreator Hats which are responsible
        /// for choosing the random weather; based on the current ingame month.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A new array of uninitialized IWeatherCreator hats; sorted by month.
        /// </returns>
        private static Hat<IWeatherCreator>[] CreateWeatherHats( IRand rand )
        {
            var hats = new Hat<IWeatherCreator>[12];

            for( int i = 0; i < hats.Length; ++i )
            {
                hats[i] = new Hat<IWeatherCreator>( rand );
            }

            return hats;
        }

        /// <summary>
        /// Setups this MonthBasedRandomWeatherCreatorProvider to create european-like IWeather.
        /// </summary>
        private void SetupEuropeanWeather()
        {
            var fog       = this.creators.Find<FogCreator>();
            var mist      = this.creators.Find<MistCreator>();
            var rain      = this.creators.Find<RainCreator>();
            var snow      = this.creators.Find<SnowCreator>();
            var noise     = this.creators.Find<NoiseCreator>();
            var fogNoise  = this.creators.Find<FogNoiseCreator>();
            var rainStorm = this.creators.Find<RainStormCreator>();

            // January
            var hat = this.weatherHats[0];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1000 );
            hat.Insert( noise, 1000 );
            hat.Insert( fogNoise, 1050 );
            hat.Insert( mist, 1000 );
            hat.Insert( rain, 1250 );
            hat.Insert( rainStorm, 900 );
            hat.Insert( snow, 1250 );

            // February
            hat = this.weatherHats[1];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1000 );
            hat.Insert( noise, 1000 );
            hat.Insert( fogNoise, 1000 );
            hat.Insert( mist, 1000 );
            hat.Insert( rain, 1250 );
            hat.Insert( rainStorm, 1200 );
            hat.Insert( snow, 1000 );

            // March
            hat = this.weatherHats[2];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1000 );
            hat.Insert( noise, 1000 );
            hat.Insert( fogNoise, 1000 );
            hat.Insert( mist, 1000 );
            hat.Insert( rain, 1355 );
            hat.Insert( rainStorm, 1000 );
            hat.Insert( snow, 100 );

            // April
            hat = this.weatherHats[3];
            hat.Insert( null, 2500 );
            hat.Insert( fog, 2500 );
            hat.Insert( noise, 2500 );
            hat.Insert( fogNoise, 2500 );
            hat.Insert( mist, 2500 );
            hat.Insert( rain, 2550 );
            hat.Insert( rainStorm, 2520 );
            hat.Insert( snow, 900 );

            // May
            hat = this.weatherHats[4];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1500 );
            hat.Insert( noise, 1500 );
            hat.Insert( fogNoise, 1500 );
            hat.Insert( mist, 2000 );
            hat.Insert( rain, 1550 );
            hat.Insert( rainStorm, 1150 );
            hat.Insert( snow, 100 );

            // July
            hat = this.weatherHats[5];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1000 );
            hat.Insert( noise, 1000 );
            hat.Insert( fogNoise, 1000 );
            hat.Insert( mist, 600 );
            hat.Insert( rain, 950 );
            hat.Insert( rainStorm, 1150 );
            hat.Insert( snow, 50 );

            // June
            hat = this.weatherHats[6];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1000 );
            hat.Insert( noise, 1100 );
            hat.Insert( fogNoise, 1050 );
            hat.Insert( mist, 700 );
            hat.Insert( rain, 850 );
            hat.Insert( rainStorm, 950 );
            hat.Insert( snow, 50 );

            // August
            hat = this.weatherHats[7];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1050 );
            hat.Insert( noise, 1100 );
            hat.Insert( fogNoise, 1100 );
            hat.Insert( mist, 900 );
            hat.Insert( rain, 1155 );
            hat.Insert( rainStorm, 1255 );
            hat.Insert( snow, 700 );

            // September
            hat = this.weatherHats[8];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1250 );
            hat.Insert( noise, 1050 );
            hat.Insert( fogNoise, 1050 );
            hat.Insert( mist, 1000 );
            hat.Insert( rain, 855 );
            hat.Insert( rainStorm, 950 );
            hat.Insert( snow, 100 );

            // October
            hat = this.weatherHats[9];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1250 );
            hat.Insert( noise, 1050 );
            hat.Insert( fogNoise, 1250 );
            hat.Insert( mist, 1500 );
            hat.Insert( rain, 1050 );
            hat.Insert( rainStorm, 1150 );
            hat.Insert( snow, 800 );

            // November
            hat = this.weatherHats[10];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1400 );
            hat.Insert( noise, 1300 );
            hat.Insert( fogNoise, 1200 );
            hat.Insert( mist, 1500 );
            hat.Insert( rain, 1255 );
            hat.Insert( rainStorm, 950 );
            hat.Insert( snow, 2500 );

            // December
            hat = this.weatherHats[11];
            hat.Insert( null, 5000 );
            hat.Insert( fog, 1100 );
            hat.Insert( noise, 1000 );
            hat.Insert( fogNoise, 1000 );
            hat.Insert( mist, 1500 );
            hat.Insert( rain, 1050 );
            hat.Insert( rainStorm, 950 );
            hat.Insert( snow, 3000 );
        }

        /// <summary>
        /// Gets an IWeatherCreator from this MonthBasedRandomWeatherCreatorProvider.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that wants to receive an IWeatherCreator.
        /// </param>
        /// <returns>
        /// An IWeatherCreator instance; or null.
        /// </returns>
        public IWeatherCreator GetCreator( IWeatherMachine weatherMachine )
        {
            int month = GetMonth( weatherMachine );
            var hat = this.weatherHats[month];

            return hat.Get();
        }

        /// <summary>
        /// Gets the current ingame month for the specified IWeatherMachine.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine that controls the IWeather.
        /// </param>
        /// <returns>
        /// The current month; a value from 0 to 11.
        /// </returns>
        private static int GetMonth( IWeatherMachine weatherMachine )
        {
            return weatherMachine.Scene.IngameDateTime.Current.Month - 1;
        }

        /// <summary>
        /// The list (sorted by month) of IWeatherCreator Hats.
        /// </summary>
        /// <remarks>
        /// An IWeatherCreator Hat is used to randomly receive the current weather in the scene.
        /// </remarks>
        private readonly Hat<IWeatherCreator>[] weatherHats;

        /// <summary> 
        /// Provides access to all known IWeatherCreators.
        /// </summary>
        private readonly IWeatherCreatorMap creators;
    }
}