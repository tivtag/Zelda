// <copyright file="EntityPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.EntityPropertyWrapper{TEntity} class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Design;
    using System.ComponentModel;

    /// <summary>
    /// Defines a basic implemention of an <see cref="IObjectPropertyWrapper"/>
    /// that wraps around the properties of a <see cref="Zelda.Entities.ZeldaEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity beeing wrapped by this IObjectPropertyWrapper.
    /// </typeparam>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    internal abstract class EntityPropertyWrapper<TEntity> : BaseObjectPropertyWrapper<TEntity>
        where TEntity : Zelda.Entities.ZeldaEntity
    {
        [LocalizedDisplayName( "PropDisp_Name" )]
        [LocalizedCategory( "PropCate_Identification" )]
        [LocalizedDescriptionAttribute( "PropDesc_Name" )]
        public string Name
        {
            get
            {
                return this.WrappedObject.Name;
            }
            set
            {
                if( value == this.Name )
                    return;

                VerifyName( value );

                // Apply new name.
                this.WrappedObject.Name = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        [LocalizedDisplayName( "PropDisp_TypeName" )]
        [LocalizedCategory( "PropCate_Identification" )]
        [LocalizedDescriptionAttribute( "PropDesc_TypeName" )]
        public string TypeName
        {
            get
            {
                return Atom.ReflectionExtensions.GetTypeName( this.WrappedObject.GetType() );
            }
        }

        /// <summary>
        /// Varifies whether the given name would be valid for the wrapped entity.
        /// </summary>
        /// <param name="newName">
        /// The new name of the entity.
        /// </param>
        private void VerifyName( string newName )
        {
            var scene = this.WrappedObject.Scene;

            if( scene != null )
            {
                if( scene.HasEntity( newName ) )
                {
                    throw new System.ArgumentException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Properties.Resources.Error_ThereAlreadyExistsEntityWithNameX,
                            newName
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Gets a short string descriping the content of this EntityPropertyWrapper{TEntity}.
        /// </summary>
        /// <returns>A short string descriping the content of this EntityPropertyWrapper{TEntity}.</returns>
        public override string ToString()
        {
            if( this.WrappedObject == null )
                return this.GetType().Name;

            return this.WrappedObject.Name;
        }
    }
}
