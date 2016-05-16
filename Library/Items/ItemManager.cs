// <copyright file="ItemManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemManager classs.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Zelda.Saving;

    /// <summary>
    /// Manages the loading of <see cref="Item"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ItemManager
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Status.ExtendedStatable"/> for which
        /// Items should be loaded.
        /// </summary>
        public Zelda.Status.ExtendedStatable Statable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative path to the folder that contains the item data.
        /// </summary>
        /// <value>
        /// The default value is "Content/Items/".
        /// </value>
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        #endregion

        #region [ Iniitalization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public ItemManager( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Receives the <see cref="Item"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// The Item gets loaden from the HD if it's
        /// not cached by this <see cref="ItemManager"/>.
        /// </remarks>
        /// <param name="name">The name of the <see cref="Item"/> to get.</param>
        /// <returns>The requested <see cref="Item"/>.</returns>
        public Item Get( string name )
        {
            Item item;
            if( this.dict.TryGetValue( name, out item ) )
                return item;

            try
            {
                item = this.Load( name );
                this.dict.Add( item.Name, item );
            }
            catch( System.IO.FileNotFoundException exc )
            {
                if( this.serviceProvider != null )
                {
                    this.serviceProvider.Log.WriteLine( "\nAn Item Definition file could not be found." );
                    this.serviceProvider.Log.WriteLine( "==============================================" );
                    this.serviceProvider.Log.WriteLine( exc.ToString() );
                    this.serviceProvider.Log.WriteLine( "==============================================" );
                }
                else
                {
                    throw;
                }
            }

            return item;
        }

        /// <summary>
        /// Receives the <see cref="Item"/> with the given <paramref name="name"/>
        /// by loading it directly from the HD.
        /// </summary>
        /// <remarks>
        /// This won't add the Item to the cache, nor use the cache to load the Item.
        /// </remarks>
        /// <param name="name">The name of the <see cref="Item"/> to get.</param>
        /// <returns>The requested <see cref="Item"/>.</returns>
        public Item Load( string name )
        {
            try
            {
                string fullPath = System.IO.Path.Combine( this.path, name ) + Item.Extension;

                using( var reader = new System.IO.BinaryReader( System.IO.File.OpenRead( fullPath ) ) )
                {
                    string typeName = reader.ReadString();
                    Type type = Type.GetType( typeName );

                    Item item = (Item)Activator.CreateInstance( type );
                    item.Deserialize( this.GetDeserializationContext( reader ) );

                    return item;
                }
            }
            catch( Exception ex )
            {
                if( serviceProvider != null )
                {
                    var log = this.serviceProvider.Log;
                    if( log != null )
                    {
                        log.WriteLine( "\nError loading " + name );
                        log.WriteLine( ex.ToString() );
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Gets an IItemDeserializationContext to be used when deserializing an Itme.
        /// </summary>
        /// <param name="reader">
        /// The BinaryReader from which should be read.
        /// </param>
        /// <returns>
        /// The IItemDeserializationContext to use.
        /// </returns>
        private IZeldaDeserializationContext GetDeserializationContext( System.IO.BinaryReader reader )
        {
            return new DeserializationContext( reader, this.serviceProvider );
        }

        /// <summary>
        /// Receives a value that indicates whether this <see cref="ItemManager"/> has the <see cref="Item"/>
        /// with the given <paramref name="name"/> cached. 
        /// </summary>
        /// <remarks>
        /// Items get cached when they are loaden from the disc when using the <see cref="Get"/> method.
        /// </remarks>
        /// <param name="name">The name of the Item.</param>
        /// <returns>true if the item is cached; otherwise false.</returns>
        public bool HasCached( string name )
        {
            return this.dict.ContainsKey( name );
        }

        public IEnumerable<Gem> GetGems()
        {
            foreach( string fileName in GetGemFileNames() )
            {
                string assetName = System.IO.Path.GetFileNameWithoutExtension( fileName );
                var gem = this.Load( assetName ) as Gem;

                if( gem != null )
                {
                    yield return gem;
                }
            }
        }

        private static string[] GetGemFileNames()
        {
            var query =
                from file in Directory.GetFiles( "Content\\Items" )
                where file.StartsWith( "Content\\Items\\Gem", System.StringComparison.Ordinal )
                select file;

            return query.ToArray();
        }

        public void Unload()
        {
            this.dict.Clear();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The relative path to the folder that contains the item data.
        /// </summary>
        private string path = "Content/Items/";

        /// <summary>
        /// The dictory that contains the <see cref="Item"/>s which have been loaden and cached, sorted by their name.
        /// </summary>
        private readonly Dictionary<string, Item> dict = new Dictionary<string, Item>();

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
