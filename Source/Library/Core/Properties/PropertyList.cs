// <copyright file="PropertyList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.PropertyList class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties
{
    using System;
    using Atom.Diagnostics.Contracts;
    using System.Linq;
    using Atom.Collections;
    using Zelda.Saving;

    /// <summary>
    /// Represents a list of IProperties.
    /// </summary>
    public sealed class PropertyList : RedirectingList<IProperty>, IPropertyList
    {
        /// <summary>
        /// Adds the specified IProperty to this PropertyList.
        /// </summary>
        /// <param name="property">
        /// The property to add.
        /// </param>
        public override void Add( IProperty property )
        {
            this.AnalyzeAttributes( property );            
            base.Add( property );
        }

        /// <summary>
        /// Inserts the given IProperty into this PropertyList.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert.</param>
        /// <param name="property">The IProperty to insert.</param>
        public override void Insert( int index, Zelda.Core.Properties.IProperty property )
        {
            this.AnalyzeAttributes( property );
            base.Insert( index, property );
        }

        /// <summary>
        /// Analyzes the attributes of the specified IProperty.
        /// </summary>
        /// <param name="property">
        /// The IProperty to analyze.
        /// </param>
        private void AnalyzeAttributes( IProperty property )
        {
            Contract.Requires<ArgumentNullException>( property != null );

            Type type = property.GetType();

            foreach( var attibute in type.GetCustomAttributes( true ) )
            {
                if( attibute.GetType() == typeof( UniquePropertyAttribute ) )
                {
                    if( this.HasProperty( property.GetType() ) )
                    {
                        throw new ArgumentException( Resources.ErrorCantAddAnotherPropertyOfThatTypeItIsUnique, "property" );
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IPropertyList contains
        /// an IProperty of the specified type.
        /// </summary>
        /// <param name="type">
        /// The exact type of the property to search for.
        /// </param>
        /// <returns>
        /// true if this IPropertyList contains the specified type;
        /// otherwise false.
        /// </returns>
        public bool HasProperty( Type type )
        {
            return this.List.Any( 
                property => property.GetType() == type
            );
        }

        /// <summary>
        /// Tries to get the IProperty of the specified type.
        /// </summary>
        /// <typeparam name="TProperty">
        /// The exact type of the property to get.
        /// </typeparam>
        /// <returns>
        /// The requested property; or null.
        /// </returns>
        public TProperty TryGet<TProperty>()
            where TProperty : class, IProperty
        {
            return this.List.FirstOrDefault(
                property => property.GetType() == typeof( TProperty )
            ) as TProperty;
        }

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Data.
            context.Write( this.Count );            
            for( int i = 0; i < this.Count; ++i )
            {
                context.WriteObject( this[i] );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Data.
            int propertyCount = context.ReadInt32();

            for( int i = 0; i < propertyCount; ++i )
            {
                var property = context.ReadObject<IProperty>();
                base.Add( property );
            }
        }

        #endregion
    }
}