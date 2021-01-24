// <copyright file="MultiWeatherCreator.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.MultiWeatherCreator class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an <see cref="IWeatherCreator"/> that creates <see cref="IWeather"/> by
    /// merging the IWeather created by multiple other IWeatherCreators.
    /// </summary>
    public abstract class MultiWeatherCreator : BaseWeatherCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiWeatherCreator"/> class.
        /// </summary>
        /// <param name="creatorCapacity">
        /// The initial number of IWeatherCreators that this MultiWeatherCreator may contain.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        protected MultiWeatherCreator( int creatorCapacity, IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.creators = new List<IWeatherCreator>( creatorCapacity );
        }

        /// <summary>
        /// Creates a new instance of the IWeather this IWeatherCreator creates.
        /// </summary>
        /// <param name="weatherMachine">
        /// The IWeatherMachine for which the IWeather should be created for.
        /// </param>
        /// <returns> 
        /// The <see cref="IWeather"/> instances that make up the weather.
        /// </returns>
        public override IWeather[] CreateWeather( IWeatherMachine weatherMachine )
        {
            var list = new List<IWeather>();

            foreach( var creator in this.creators )
            {
                list.AddRange( creator.CreateWeather( weatherMachine ) );
            }

            return list.ToArray();
        }

        /// <summary>
        /// Adds the given <see cref="IWeatherCreator"/> to this <see cref="MultiWeatherCreator"/>;
        /// adding the IWeather created by it to the list of IWeather created by this MultiWeatherCreator.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="creator"/> is null.
        /// </exception>
        /// <param name="creator">
        /// The IWeatherCreator to add.
        /// </param>
        protected void AddCreator( IWeatherCreator creator )
        {
            if( creator == null )
                throw new ArgumentNullException( "creator" );

            this.creators.Add( creator );
        }

        /// <summary>
        /// The list of IWeatherCreators that individually create the IWeather of this MultiWeatherCreator.
        /// </summary>
        private readonly List<IWeatherCreator> creators;
    }
}
