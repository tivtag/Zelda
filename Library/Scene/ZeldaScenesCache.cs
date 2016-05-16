// <copyright file="ZeldaScenesCache.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ZeldaScenesCache class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a mechanism to cache existing <see cref="ZeldaScene"/>s.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// This functionality is used when changing from one ZeldaScene to another.
    /// Without a scene cache the player could quickly change from one scene to another
    /// and back to completly -reset- the scene.
    /// </remarks>
    public sealed class ZeldaScenesCache : IReloadable
    {
        /// <summary>
        /// Adds the given <see cref="ZeldaScene"/> to this ZeldaScenesCache,
        /// and as such caching the given ZeldaScene.
        /// </summary>
        /// <remarks>
        /// Nothing is done if this ZeldaScenesCache already contains the given <see cref="ZeldaScene"/>.
        /// </remarks>
        /// <param name="scene">
        /// The <see cref="ZeldaScene"/> to add to this ZeldaScenesCache.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="scene"/> is null.
        /// </exception>
        public void Add( ZeldaScene scene )
        {
            if( scene == null )
                throw new ArgumentNullException( "scene" );

            if( cachedScenes.Contains( scene ) )
                return;

            this.cachedScenes.Add( scene );
        }

        /// <summary>
        /// Tries to remove the cached <see cref="ZeldaScene"/>
        /// with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that unique identifies the cached ZeldaScene.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is null.
        /// </exception>
        /// <returns>
        /// true if the ZeldaScene with the given <paramref name="name"/> has been
        /// removed from this ZeldaScenesCache;
        /// otherwise false.
        /// </returns>
        internal bool Remove( string name )
        {
            return this.cachedScenes.Remove(
                this.Get( name )
            );
        }

        /// <summary>
        /// Tries to remove the specified ZeldaScene from this ZeldaScenesCache.
        /// </summary>
        /// <param name="scene">
        /// The scene to remove.
        /// </param>
        /// <returns>
        /// true if the given ZeldaScne was removed from this ZeldaScenesCache;
        /// otherwise false.
        /// </returns>
        internal bool Remove( ZeldaScene scene )
        {
            return this.cachedScenes.Remove( scene );
        }

        /// <summary>
        /// Tries to get the cached <see cref="ZeldaScene"/>
        /// with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that unique identifies the cached ZeldaScene.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is null.
        /// </exception>
        /// <returns>
        /// The requested cached <see cref="ZeldaScene"/>;
        /// or null if this ZeldaScenesCache doesn't contain
        /// a ZeldaScene that has the specified <paramref name="name"/>.
        /// </returns>
        public ZeldaScene Get( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            return this.cachedScenes.Find(
                ( scene ) => {
                    return name.Equals( scene.Name, StringComparison.Ordinal );
                }
            );
        }

        /// <summary>
        /// Gets a value indicating whether this ZeldaScenesCache
        /// contains a cached <see cref="ZeldaScene"/>
        /// with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that unique identifies the cached ZeldaScene.
        /// </param>
        /// <returns>
        /// true if this ZeldaScenesCache contains a cached <see cref="ZeldaScene"/>
        /// that has the given <paramref name="name"/>;
        /// otherwise false.
        /// </returns>
        public bool Contains( string name )
        {
            return this.Get( name ) != null;
        }

        /// <summary>
        /// Gets a value indicating whether this this cache contains the given scene.
        /// </summary>
        /// <param name="scene">
        /// The scene to query for.
        /// </param>
        /// <returns>
        /// true if it contains the scene; -or- otherwise false.
        /// </returns>
        public bool Contains( ZeldaScene scene )
        {
            return this.cachedScenes.Contains( scene );
        }

        /// <summary>
        /// Reloads all scenes in this cache.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            for( int i = 0; i < cachedScenes.Count; ++i )
            {
                cachedScenes[i].Reload( serviceProvider );
            }
        }

        /// <summary>
        /// The list of currently cached ZeldaScenes.
        /// </summary>
        private readonly List<ZeldaScene> cachedScenes = new List<ZeldaScene>( 3 );
    }
}
