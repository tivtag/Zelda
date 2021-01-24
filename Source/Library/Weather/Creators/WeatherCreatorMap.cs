// <copyright file="WeatherCreatorMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.WeatherCreatorMap class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Atom.Diagnostics;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Stores and provides access to the IWeatherCreators that are known to the game.
    /// </summary>
    public sealed class WeatherCreatorMap : IWeatherCreatorMap
    {
        /// <summary>
        /// Randomly pics an IWeatherCreator from this WeatherCreatorMap.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A randomly picked IWeatherCreator.
        /// </returns>
        public IWeatherCreator GetRandom( IRand rand )
        {
            int index = rand.RandomRange( 0, this.creators.Count - 1 );
            return this.creators.Values.ElementAt( index );
        }

        /// <summary>
        /// Adds the default IWeatherCreators to this WeatherCreatorMap.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void AddDefault( IZeldaServiceProvider serviceProvider )
        {
            this.Add( new FogCreator( serviceProvider ) );
            this.Add( new MistCreator( serviceProvider ) );
            this.Add( new RainCreator( serviceProvider ) );
            this.Add( new RainStormCreator( serviceProvider ) );
            this.Add( new SnowCreator( serviceProvider ) );
            this.Add( new AmbientRainStormCreator( serviceProvider ) );
            this.Add( new NoiseCreator( serviceProvider ) );
            this.Add( new FogNoiseCreator( serviceProvider ) );
        }

        /// <summary>
        /// Adds the specified <see cref="IWeatherCreator"/> to this WeatherCreatorMap.
        /// </summary>
        /// <param name="creator"> 
        /// The <see cref="IWeatherCreator"/> to add.
        /// </param>
        private void Add( IWeatherCreator creator )
        {
            Debug.Assert( creator != null );
            this.creators.Add( creator.GetType(), creator );
        }

        /// <summary>
        /// Tries to get the <see cref="IWeatherCreator"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the IWeatherCreator to receive.
        /// </param>
        /// <returns>
        /// The IWeatherCreator instance;
        /// or null if this WeatherCreatorMap doesn't contain an IWeatherCreator of the Type.
        /// </returns>
        public IWeatherCreator Find( Type type )
        {
            IWeatherCreator creator;

            if( this.creators.TryGetValue( type, out creator ) )
            {
                return creator;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to get the <see cref="IWeatherCreator"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the IWeatherCreator to receive.
        /// </typeparam>
        /// <returns>
        /// The IWeatherCreator instance;
        /// or null if this WeatherCreatorMap doesn't contain an IWeatherCreator of the Type.
        /// </returns>
        public IWeatherCreator Find<T>()
        {
            return this.Find( typeof( T ) );
        }

        public void LoadContent()
        {
            foreach( IWeatherCreator creator in this.creators.Values )
            {
                var content = creator as IContentLoadable;

                if( content != null )
                {
                    content.LoadContent();
                }
            }
        }

        /// <summary> 
        /// The dictionary containing all IWeatherCreators, sorted by type.
        /// </summary>
        private readonly Dictionary<Type, IWeatherCreator> creators = new Dictionary<Type, IWeatherCreator>();
    }
}
