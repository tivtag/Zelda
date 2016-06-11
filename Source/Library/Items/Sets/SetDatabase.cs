// <copyright file="SetDatabase.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.SetDatabase class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Atom.Diagnostics;
    using Zelda.Saving;
    
    /// <summary>
    /// Provides a mechanism to receive <see cref="ISet"/>s.
    /// </summary>
    public sealed class SetDatabase : ISetDatabase
    {
        /// <summary>
        /// Initializes a new instance of the SetDatabase class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public SetDatabase( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Tries to get the <see cref="ISet"/> with the specified <paramref name="setName"/>.
        /// </summary>
        /// <param name="setName">
        /// The name of the set.
        /// </param>
        /// <returns>
        /// The requested ISet; or null.
        /// </returns>
        public ISet Get( string setName )
        {
            ISet set;

            if( !this.cachedSets.TryGetValue( setName, out set ) )
            {
                set = this.TryLoad( setName );

                if( set != null )
                {
                    this.cachedSets.Add( setName, set );
                }
            }

            return set;
        }

        /// <summary>
        /// Tries to load the ISet with the given <paramref name="setName"/>.
        /// </summary>
        /// <param name="setName">
        /// The name of the set.
        /// </param>
        /// <returns>
        /// The requested ISet; or null.
        /// </returns>
        private ISet TryLoad( string setName )
        {
            try
            {
                return this.Load( setName );
            }
            catch( Exception exc )
            {
                this.serviceProvider.Log.WriteLine( exc.ToString() );
                return null;
            }
        }

        /// <summary>
        /// Loads the ISet with the given <paramref name="setName"/>.
        /// </summary>
        /// <remarks>
        /// Throws an exception on error.
        /// </remarks>
        /// <param name="setName">
        /// The name of the set.
        /// </param>
        /// <returns>
        /// The requested ISet.
        /// </returns>
        private ISet Load( string setName )
        {
            string fullName = "Content/Items/Sets/" + setName + ".zset";

            using( var stream = File.OpenRead( fullName ) )
            {
                var context = new DeserializationContext( new BinaryReader( stream ), this.serviceProvider );
                return context.ReadObject<ISet>();
            }
        }
        
        /// <summary>
        /// Contains the ISets that have been loaden previously.
        /// </summary>
        private readonly Dictionary<string, ISet> cachedSets = new Dictionary<string, ISet>();

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
