// <copyright file="StatusBarInfoController.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Updater.StatusBarInfoController class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Updater
{
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Encapsulates the logic of showing information about the current status
    /// of the Zelda.Updater to the user.
    /// </summary>
    public sealed class StatusBarInfoController
    {
        public StatusBarInfoController( StatusBarItem statusBarItem )
        {
            this.statusBarItem = statusBarItem;
        }

        /// <summary>
        /// Shows the given information to the user.
        /// </summary>
        /// <remarks>
        /// The information is only shown when no error
        /// is currently shown.
        /// </remarks>
        /// <param name="text">
        /// The text to show.
        /// </param>
        internal void ShowInformation( string text )
        {
            if( !this.isShowingError )
            {
                this.SetText( text );
            }
        }

        /// <summary>
        /// Shows the given error information to the user.
        /// </summary>
        /// <param name="text">
        /// The text to show.
        /// </param>
        internal void ShowError( string text )
        {
            this.SetText( text );
            this.isShowingError = true;
        }

        /// <summary>
        /// Sets the current text shown in the Status Bar.
        /// </summary>
        /// <param name="text">
        /// The text to show.
        /// </param>
        private void SetText( string text )
        {
            Execute.OnUIThread( () => {
                this.statusBarItem.Content = text;
                this.statusBarItem.ToolTip = text;
            } );
        }

        /// <summary>
        /// States whether the StatusBar is currently showing error information.
        /// </summary>
        private bool isShowingError;
        
        /// <summary>
        /// Identifies the StatusBarItem responsible for actually showing the status information.
        /// </summary>
        private readonly StatusBarItem statusBarItem;
    }
}
