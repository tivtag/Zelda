// <copyright file="IWeatherCreatorMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.IWeatherCreatorMap interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Weather.Creators
{
    using System;
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Stores and provides access to the IWeatherCreators that are known to the game.
    /// </summary>
    public interface IWeatherCreatorMap : IContentLoadable
    {        
        /// <summary>
        /// Tries to get the <see cref="IWeatherCreator"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the IWeatherCreator to receive.
        /// </param>
        /// <returns>
        /// The IWeatherCreator instance;
        /// or null if this IWeatherCreatorMap doesn't contain an IWeatherCreator of the Type.
        /// </returns>
        IWeatherCreator Find( Type type );
        
        /// <summary>
        /// Tries to get the <see cref="IWeatherCreator"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the IWeatherCreator to receive.
        /// </typeparam>
        /// <returns>
        /// The IWeatherCreator instance;
        /// or null if this IWeatherCreatorMap doesn't contain an IWeatherCreator of the Type.
        /// </returns>
        IWeatherCreator Find<T>();

        /// <summary>
        /// Randomly pics an IWeatherCreator from this IWeatherCreatorMap.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A randomly picked IWeatherCreator.
        /// </returns>
        IWeatherCreator GetRandom( IRand rand );
    }
}
