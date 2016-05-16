// <copyright file="DataStore.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.Storage.DataStore class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving.Storage
{ 
    using System;
    using System.Collections.Generic;
    using Atom;
    
    /// <summary>
    /// Represents a place that stores <see cref="IStorage"/> values
    /// by mapping them onto an identifier string.
    /// </summary>
    public sealed class DataStore : ISaveable
    {
        /// <summary>
        /// Gets the <see cref="IStorage"/> entries in this DataStore.
        /// </summary>
        public IEnumerable<IStorage> Entries
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.dictionary.Count );
            
            foreach( var entry in this.dictionary )
            {
                SerializeEntry( entry, context );
            }
        }

        /// <summary>
        /// Serializes/Writes the data descriping the given ValueStore entry.
        /// </summary>
        /// <param name="entry">
        /// The ValueStore entry to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private static void SerializeEntry( KeyValuePair<string, IStorage> entry, Zelda.Saving.IZeldaSerializationContext context )
        {
            string identifier = entry.Key;
            IStorage storage = entry.Value;

            context.Write( identifier );
            context.Write( storage.GetType().GetTypeName() );

            storage.SerializeStorage( context );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int count = context.ReadInt32();

            for( int i = 0; i < count; ++i )
            {
                this.DeserializeEntry( context );
            }
        }

        /// <summary>
        /// Deserializes/Reads the data descriping an entry in the ValueStore.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void DeserializeEntry( Zelda.Saving.IZeldaDeserializationContext context )
        {
            string identifier = context.ReadString();
            string storageTypeName = context.ReadString();

            Type storageType = Type.GetType( storageTypeName );
            IStorage storage = (IStorage)Activator.CreateInstance( storageType );
            storage.DeserializeStorage( context );

            this.dictionary.Add( identifier, storage );
        }

        public bool Contains( string identifier )
        {
            return this.dictionary.ContainsKey( identifier );
        }

        /// <summary>
        /// Gets the <see cref="IStorage"/> with the given <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <returns>
        /// The requested IStorage; or null.
        /// </returns>
        public IStorage TryGet( string identifier )
        {
            IStorage storage;            
            this.dictionary.TryGetValue( identifier, out storage );
            return storage;
        }

        /// <summary>
        /// Gets the <see cref="IStorage"/> with the given <paramref name="identifier"/> and type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the IStorage to receive.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <returns>
        /// The requested IStorage; or null.
        /// </returns>
        public T TryGet<T>( string identifier )
            where T : class, IStorage
        {
            return this.TryGet( identifier ) as T;
        }

        /// <summary>
        /// Gets the <see cref="IValueStorage{Boolean}"/> with the given <paramref name="identifier"/> .
        /// </summary>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <returns>
        /// The requested IStorage; or null.
        /// </returns>
        public IValueStorage<bool> TryGetBoolean( string identifier )
        {
            return this.TryGet<IValueStorage<bool>>( identifier );
        }

        /// <summary>
        /// Gets or creates the IValueStorage{Boolean} with the given identifier.
        /// </summary>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the new IValueStorage.
        /// </param>
        /// <returns>
        /// The already existing or newly created IValueStorage.
        /// </returns>
        public IValueStorage<bool> GetOrCreateBoolean( string identifier, bool initialValue )
        {
            return this.GetOrCreate<BooleanStorage, bool>( identifier, initialValue );
        }

        /// <summary>
        /// Creates or overwrites the IValueStorage{Boolean} with the given identifier.
        /// </summary>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the new BooleanStorage.
        /// </param>
        public void CreateBoolean( string identifier, bool initialValue )
        {
            this.Create<BooleanStorage, bool>( identifier, initialValue );
        }

        /// <summary>
        /// Creates or overwrites the IValueStorage{TValue} with the given identifier.
        /// </summary>
        /// <typeparam name="TStorage">
        /// The final type of the IValueStorage.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the new IValueStorage.
        /// </param>
        public void Create<TStorage, TValue>( string identifier, TValue initialValue )
            where TStorage : class, IValueStorage<TValue>, new()
        {
            TStorage storage = new TStorage() {
                Value = initialValue
            };

            this.AddOrReplace( identifier, storage );
        }

        /// <summary>
        /// Gets or creates the IValueStorage{TValue} with the given identifier.
        /// </summary>
        /// <typeparam name="TStorage">
        /// The final type of the IValueStorage.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the new IValueStorage.
        /// </param>
        /// <returns>
        /// The already existing or newly created IValueStorage.
        /// </returns>
        public TStorage GetOrCreate<TStorage, TValue>( string identifier, TValue initialValue )
            where TStorage : class, IValueStorage<TValue>, new()
        {
            TStorage storage = this.TryGet<TStorage>( identifier );

            if( storage == null )
            {
                storage = new TStorage() {
                    Value = initialValue
                };

                this.AddOrReplace( identifier, storage );
            }

            return storage;
        }

        /// <summary>
        /// Gets or creates the IStorage with the given identifier.
        /// </summary>
        /// <typeparam name="TStorage">
        /// The final type of the IStorage.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <returns>
        /// The already existing or newly created IStorage.
        /// </returns>
        public TStorage GetOrCreate<TStorage>( string identifier )
            where TStorage : class, IStorage, new()
        {
            return GetOrCreate<TStorage>( identifier, () => new TStorage() );
        }

        /// <summary>
        /// Gets or creates the IStorage with the given identifier.
        /// </summary>
        /// <typeparam name="TStorage">
        /// The final type of the IStorage.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="creationFunction">
        /// The function that is executed when the IStorage has to be created.
        /// </param>
        /// <returns>
        /// The already existing or newly created IStorage.
        /// </returns>
        public TStorage GetOrCreate<TStorage>( string identifier, Func<TStorage> creationFunction )
            where TStorage : class, IStorage
        {
            TStorage storage = this.TryGet<TStorage>( identifier );

            if( storage == null )
            {
                storage = creationFunction();
                this.AddOrReplace( identifier, storage );
            }

            return storage;
        }

        /// <summary>
        /// Tries to the value with the given <paramref name="identifier"/> and type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="identifier">
        /// The full identifier.
        /// </param>
        /// <param name="value">
        /// Will contain the requested value.
        /// </param>
        /// <returns>
        /// Returns true if a value was found for the given <paramref name="identifier"/> and type;
        /// or otherwise false.
        /// </returns>
        public bool TryGetValue<T>( string identifier, out T value )
        {
            IStorage storage;
            
            if( this.dictionary.TryGetValue( identifier, out storage ) )
            {
                IValueStorage<T> valueStorage = storage as IValueStorage<T>;

                if( valueStorage != null )
                {
                    value = valueStorage.Value;
                    return true;
                }
            }

            value = default( T );
            return false;
        }

        public void Add( string identifier )
        {
            this.Add( identifier, null );
        }

        /// <summary>
        /// Adds the specified <see cref="IStorage"/> with the given
        /// unique <paramref name="identifier"/> to this Store.
        /// </summary>
        /// <param name="identifier">
        /// An unique identifier string.
        /// </param>
        /// <param name="storage">
        /// The IStorage to add.
        /// </param>
        public void Add( string identifier, IStorage storage )
        {
            this.dictionary.Add( identifier, storage );
        }

        /// <summary>
        /// Adds the specified <see cref="IStorage"/> with the given
        /// unique <paramref name="identifier"/> to this Store.
        /// </summary>
        /// <param name="identifier">
        /// An unique identifier string.
        /// </param>
        /// <param name="storage">
        /// The IStorage to add.
        /// </param>
        public void AddOrReplace<TStorage>( string identifier, TStorage storage )
            where TStorage : IStorage
        {
            this.dictionary[identifier] = storage;
        }

        /// <summary>
        /// The dictionary that maps identifier strings onto IStorages.
        /// </summary>
        private readonly Dictionary<string, IStorage> dictionary = new Dictionary<string, IStorage>();
    }
}