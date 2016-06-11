// <copyright file="WeatherCreatorTypeNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Weather.Creators.AmbientRainStormCreator class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Weather.Creators.Design
{
    using System;
    using System.Collections.Generic;
    using Atom;

    /// <summary>
    /// Defines an UITypeEditor that lets the user select the type-name of a weather creator.
    /// </summary>
    public sealed class WeatherCreatorTypeNameEditor : Atom.Design.BaseTypeSelectionEditor
    {
        /// <summary>
        /// Gets the final value that is returned by this SongTypeNameEditor.
        /// </summary>
        /// <param name="selectedItem">
        /// The object the used has selected.
        /// </param>
        /// <returns>
        /// The object that is returned from the SongTypeNameEditor.
        /// </returns>
        protected override object GetFinalValue( Atom.Design.NameableObjectWrapper<Type> selectedItem )
        {
            return selectedItem.Object.GetTypeName();
        }

        protected override Atom.Design.NameableObjectWrapper<Type> CreateTypeWrapper( Type type )
        {
            return base.CreateTypeWrapper( type );
        }

        /// <summary>
        /// Gets the types that the user can select in this SongTypeNameEditor.
        /// </summary>
        /// <returns>
        /// The types the user can select.
        /// </returns>
        protected override IEnumerable<Type> GetTypes()
        {
            return WeatherCreatorTypeNameEditor.types;
        }

        /// <summary>
        /// The types the user can select in this SongTypeNameEditor.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( AmbientRainStormCreator )
        };
    }
}
