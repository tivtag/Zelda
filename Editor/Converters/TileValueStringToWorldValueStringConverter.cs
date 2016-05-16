// <copyright file="TileValueStringToWorldValueStringConverter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Editor.Converts.TileValueStringToWorldValueStringConverter class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Defines an <see cref="IValueConverter"/> that converts
    /// a tile-space value string into a world-space value string.
    /// </summary>
    [ValueConversion( typeof( string ), typeof( string ))]
    internal sealed class TileValueStringToWorldValueStringConverter : IValueConverter
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
            string str = value as string;
            if( str == null )
                return null;

            if( targetType == typeof( string ) )
            {
                if( str.Length == 0 )
                {
                    return (0).ToString( culture );
                }

                long tileSpace = long.Parse( str, culture );
                return (tileSpace*16).ToString( culture );
            }

            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            string str = value as string;
            if( str == null )
                return null;

            if( targetType == typeof( string ) )
            {
                if( str.Length == 0 )
                    return (0).ToString( culture );

                long worldSpace = long.Parse( str, culture );
                return (worldSpace/16).ToString( culture );
            }

            return null;
        }
    }
}
