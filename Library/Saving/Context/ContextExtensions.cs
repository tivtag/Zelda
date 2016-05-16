// <copyright file="ContextExtensions.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ContextExtensions class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Storage;
    using Zelda.Saving.Storage;

    /// <summary>
    /// Defines extension methods for the <see cref="IZeldaSerializationContext"/> and
    /// <see cref="IZeldaDeserializationContext"/> interfaces.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Writes a version header to the ISerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="version">
        /// The current version of the object to serialize.
        /// </param>
        public static void WriteHeader( this ISerializationContext context, int version )
        {
            context.Write( version );
        }

        /// <summary>
        /// Writes a default version header to the ISerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public static void WriteDefaultHeader( this ISerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );
        }

        /// <summary>
        /// Reads a version header from the IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="minimumVersion">
        /// The minimum expected version.
        /// </param>
        /// <param name="maximumVersion">
        /// The maximum expected version.
        /// </param>
        /// <param name="type">
        /// The type the header relates to.
        /// </param>
        /// <returns>
        /// The version that has been read.
        /// </returns>
        public static int ReadHeader( this IDeserializationContext context, int minimumVersion, int maximumVersion, Type type )
        {
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, minimumVersion, maximumVersion, type );

            return version;
        }

        /// <summary>
        /// Reads a version header from the IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="maximumVersion">
        /// The maximum expected version.
        /// </param>
        /// <param name="type">
        /// The type the header relates to.
        /// </param>
        /// <returns>
        /// The version that has been read.
        /// </returns>
        public static int ReadHeader( this IDeserializationContext context, int maximumVersion, Type type )
        {
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, 1, maximumVersion, type );

            return version;
        }

        /// <summary>
        /// Reads a default version header from the IDeserializationContext.
        /// </summary>
        /// <typeparam name="Type">
        /// The type the header relates to.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public static void ReadDefaultHeader<Type>( this IDeserializationContext context )
        {
            ReadDefaultHeader( context, typeof( Type ) );
        }

        /// <summary>
        /// Reads a default version header from the IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="type">
        /// The type the header relates to.
        /// </param>
        public static void ReadDefaultHeader( this IDeserializationContext context, Type type )
        {
            const int Version = 1;
            int version = context.ReadInt32();

            ThrowHelper.InvalidVersion( version, Version, type );
        }

        /// <summary>
        /// Reads a default version header to the ISerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="typeName">
        /// The type name of the type the header relates to.
        /// </param>
        public static void ReadDefaultHeader( this IDeserializationContext context, string typeName )
        {
            const int Version = 1;
            int version = context.ReadInt32();

            ThrowHelper.InvalidVersion( version, Version, typeName );
        }

        /// <summary>
        /// Writes the given <typeparamref name="TObject"/>.
        /// </summary>
        /// <typeparam name="TObject">
        /// The <see cref="ISaveable"/> type to serialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="obj">
        /// The object to serialize.
        /// </param>
        public static void WriteObject<TObject>( this IZeldaSerializationContext context, TObject obj )
            where TObject : ISaveable
        {
            if( obj != null )
            {
                string typeName = obj.GetType().GetTypeName();
                context.Write( typeName );

                obj.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }
        }

        /// <summary>
        /// Writes the given <typeparamref name="TObject"/>.
        /// </summary>
        /// <typeparam name="TObject">
        /// The <see cref="IStorable"/> type to serialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="obj">
        /// The object to serialize.
        /// </param>
        public static void WriteStoreObject<TObject>( this IZeldaSerializationContext context, TObject obj )
            where TObject : IStorable
        {
            if( obj != null )
            {
                string typeName = obj.GetType().GetTypeName();
                context.Write( typeName );

                obj.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }
        }

        /// <summary>
        /// Deserializes an object of the specified base type.
        /// </summary>
        /// <typeparam name="TBase">
        /// The <see cref="ISaveable"/> type to deserialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TBase ReadObject<TBase>( this IZeldaDeserializationContext context )
            where TBase : class, ISaveable
        {
            string typeName = context.ReadString();
            if( typeName.Length == 0 )
                return null;

            Type type = Type.GetType( typeName );
            TBase obj = Activator.CreateInstance( type ) as TBase;

            if( obj != null )
            {
                obj.Deserialize( context );

                var setupable = obj as IZeldaSetupable;

                if( setupable != null )
                {
                    setupable.Setup( context.ServiceProvider );
                }
            }

            return obj;
        }

        /// <summary>
        /// Deserializes an object of the specified base type.
        /// </summary>
        /// <typeparam name="TBase">
        /// The <see cref="IStorable"/> type to deserialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TBase ReadStoreObject<TBase>( this IZeldaDeserializationContext context )
            where TBase : class, IStorable
        {
            string typeName = context.ReadString();
            if( typeName.Length == 0 )
                return null;

            Type type = Type.GetType( typeName );
            TBase obj = Activator.CreateInstance( type ) as TBase;

            if( obj != null )
            {
                obj.Deserialize( context );

                var setupable = obj as IZeldaSetupable;

                if( setupable != null )
                {
                    setupable.Setup( context.ServiceProvider );
                }
            }

            return obj;
        }

        /// <summary>
        /// Writes the given <typeparamref name="TObject"/>.
        /// </summary>
        /// <typeparam name="TObject">
        /// The <see cref="ISaveable"/> type to serialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="obj">
        /// The object to serialize.
        /// </param>
        public static void WriteStorage<TObject>( this IZeldaSerializationContext context, TObject obj )
            where TObject : IStorage
        {
            if( obj != null )
            {
                string typeName = obj.GetType().GetTypeName();
                context.Write( typeName );

                obj.SerializeStorage( context );
            }
            else
            {
                context.Write( string.Empty );
            }
        }

        /// <summary>
        /// Deserializes an IStorage of the specified type.
        /// </summary>
        /// <typeparam name="TBase">
        /// The <see cref="ISaveable"/> type to deserialize.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TBase ReadStorage<TBase>( this IZeldaDeserializationContext context )
            where TBase : class, IStorage
        {
            string typeName = context.ReadString();
            if( typeName.Length == 0 )
                return null;

            Type type = Type.GetType( typeName );
            TBase obj = Activator.CreateInstance( type ) as TBase;

            if( obj != null )
            {
                obj.DeserializeStorage( context );

                var setupable = obj as IZeldaSetupable;

                if( setupable != null )
                {
                    setupable.Setup( context.ServiceProvider );
                }
            }

            return obj;
        }

        /// <summary>
        /// Serializes the specified list of ISaveable objects.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the elements in the list.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="list">
        /// The list to serialize.
        /// </param>
        public static void WriteList<TElement>(
            this IZeldaSerializationContext context,
            System.Collections.Generic.IList<TElement> list )
            where TElement : class, ISaveable
        {
            int count = list != null ? list.Count : 0;

            context.Write( count );
            for( int i = 0; i < count; ++i )
            {
                TElement obj = list[i];
                context.WriteObject<TElement>( obj );
            }
        }

        /// <summary>
        /// Deserializes a list of ISaveable objects into the
        /// specified list.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the elements in the list.
        /// </typeparam>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="list">
        /// The list into which should be de-serialized.
        /// </param>
        public static void ReadListInto<TElement>(
            this IZeldaDeserializationContext context,
            System.Collections.Generic.IList<TElement> list )
            where TElement : class, ISaveable
        {
            if( list == null )
                throw new ArgumentNullException( "list" );

            int count = context.ReadInt32();
            list.Clear();

            for( int i = 0; i < count; ++i )
            {
                var obj = context.ReadObject<TElement>();
                list.Add( obj );
            }
        }

        /// <summary>
        /// Writes/Serializes the specified IDrawDataAndStrategy under the specified IZeldaSerializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="strategy">
        /// The IDrawDataAndStrategy to serialize. Can be null.
        /// </param>
        public static void WriteDrawStrategy( this IZeldaSerializationContext context, Zelda.Entities.Drawing.IDrawDataAndStrategy strategy )
        {
            if( strategy != null )
            {
                context.Write( Zelda.Entities.Drawing.DrawStrategyManager.GetName( strategy ) );
                strategy.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }
        }

        /// <summary>
        /// Deserializes and loads an <see cref="Zelda.Entities.Drawing.IDrawDataAndStrategy"/>.
        /// </summary>
        /// <remarks>
        /// The IDrawDataAndStrategy is -not- assigned to the entity.
        /// </remarks>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <param name="entity">
        /// The entity that should be visualized using the IDrawDataAndStrategy.
        /// </param>
        /// <returns>
        /// The newly loaded IDrawDataAndStrategy; -or- null.
        /// </returns>
        public static Zelda.Entities.Drawing.IDrawDataAndStrategy ReadDrawStrategy(
            this IZeldaDeserializationContext context,
            Zelda.Entities.ZeldaEntity entity )
        {
            string typeName = context.ReadString();

            if( typeName.Length != 0 )
            {
                var serviceProvider = context.ServiceProvider;
                var strategyManager = serviceProvider.DrawStrategyManager;

                var dds = strategyManager.GetStrategyClone( typeName, entity );
                dds.Deserialize( context );

                try
                {
                    dds.Load( serviceProvider );
                }
                catch( Exception exc )
                {
                    serviceProvider.Log.WriteLine( Atom.Diagnostics.LogSeverities.Error, exc.ToString() );
                }

                return dds;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Writes/Serializes the given Curve under the specified IZeldaSerializationContext. 
        /// </summary>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <param name="curve">
        /// The curve to serialize.
        /// </param>
        public static void Write( this IZeldaSerializationContext context, Curve curve )
        {
            if( curve == null )
                throw new ArgumentNullException( "curve" );

            context.WriteDefaultHeader();

            context.Write( (byte)curve.PreLoop );
            context.Write( (byte)curve.PostLoop );
            context.Write( curve.Keys.Count );

            foreach( CurveKey key in curve.Keys )
            {
                context.Write( key.Position );
                context.Write( key.Value );
            }
        }

        /// <summary>
        /// Reads/Deserializes a Curve object under the specified IZeldaDeserializationContext. 
        /// </summary>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <param name="curve">
        /// The curve to serialize.
        /// </param>
        public static Curve ReadCurve( this IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader<Curve>();

            Curve curve = new Curve();
            curve.PreLoop = (CurveLoopType)context.ReadByte();
            curve.PostLoop = (CurveLoopType)context.ReadByte();

            int keyCount = context.ReadInt32();

            for( int i = 0; i < keyCount; ++i )
            {
                float position = context.ReadSingle();
                float value = context.ReadSingle();

                curve.Keys.Add( new CurveKey( position, value ) );
            }

            return curve;
        }
    }
}
