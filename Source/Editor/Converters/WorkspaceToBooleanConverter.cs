// <copyright file="LightingStateToStringConverter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Converters.WorkspaceToBooleanConverter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Implements an IValueConverter that taken an IWorkspace returns true if and only if
    /// the WorkspaceType of the IWorkspace matches a specific WorkspaceType.
    /// </summary>
    [ValueConversion( typeof( IWorkspace ), typeof( bool ) )]
    internal sealed class WorkspaceToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the type the input workspace must have for this IValueConverter
        /// to return 'true'.
        /// </summary>
        public WorkspaceType WorkspaceType
        {
            get;
            set;
        }

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
            var workspace = value as IWorkspace;
            if( workspace == null )
                return false;
            
            return workspace.Type == this.WorkspaceType;
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
