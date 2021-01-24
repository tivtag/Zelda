// <copyright file="CameraScrollToStringConverter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Converters.CameraScrollToStringConverter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Converters
{
    using System;
    using System.Windows.Data;
    using Atom.Math;

    /// <summary>
    /// Defines an <see cref="IValueConverter"/> that converts
    /// a boolean value that indicates whether Lighting is enabled into
    /// a string that is displayed to the user.
    /// </summary>
    [ValueConversion( typeof( Vector2 ), typeof( string ) )]
    internal sealed class CameraScrollToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture"> The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( value is Vector2 )
            {
                var scroll = (Vector2)value;
                var scrollTile = new Vector2( (int)scroll.X / 16, (int)scroll.Y / 16 );

                return scrollTile.ToString( culture );
            }

            return null;
        }

        /// <summary>
        /// This operation is not supported by this IValueConverter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">This operation is not supported.</exception>
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotSupportedException();
        }
    }
}
