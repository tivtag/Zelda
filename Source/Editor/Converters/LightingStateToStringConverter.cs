// <copyright file="LightingStateToStringConverter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Converters.LightingStateToStringConverter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Defines an <see cref="IValueConverter"/> that converts
    /// a boolean value that indicates whether Lighting is enabled into
    /// a string that is displayed to the user.
    /// </summary>
    [ValueConversion( typeof( bool ), typeof( string ))]
    internal sealed class LightingStateToStringConverter : IValueConverter
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
            if( value is bool )
            {
                bool state = (bool)value;
                return state ? Properties.Resources.Text_LightOn : Properties.Resources.Text_LightOff;
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
