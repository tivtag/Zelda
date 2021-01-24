// <copyright file="ZeldaErrorReporter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Errors.ZeldaErrorReporter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Errors
{
    using Atom.Diagnostics;
    using Atom.ErrorReporting;
    using Atom.ErrorReporting.Dialogs;

    /// <summary>
    /// Implements the IErrorReporter for the Zelda application.
    /// </summary>
    public sealed class ZeldaErrorReporter : IErrorReporter
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaErrorReporter class.
        /// </summary>
        /// <param name="log">
        /// The ILog to which errors are written.
        /// </param>
        /// <param name="shutdownLogic">
        /// The logic to execute when a fatal error has occurred.
        /// </param>
        /// <param name="errorReportDialogFactory">
        /// The factory that is used to create the dialog used to visualize the error to the user.
        /// </param>
        public ZeldaErrorReporter( ILog log, IShutdownLogic shutdownLogic, IErrorReportDialogFactory errorReportDialogFactory )
        {
            this.log = log;
            this.shutdownLogic = shutdownLogic;
            this.errorReportDialogFactory = errorReportDialogFactory;
        }
        
        /// <summary>
        /// Reports the specified IError.
        /// </summary>
        /// <param name="error">
        /// The IError that has occurred.
        /// </param>
        public void Report( IError error )
        {
            try
            {
                this.Log( error );

                using( var dialog = this.errorReportDialogFactory.Build() )
                {
                    dialog.Show( error );
                }
            }
            catch { }
            finally
            {
                if( error.IsFatal && this.shutdownLogic != null )
                {
                    this.shutdownLogic.DoShutdown();
                }
            }
        }

        /// <summary>
        /// Logs the specified IError.
        /// </summary>
        /// <param name="error">
        /// The error that has occurred.
        /// </param>
        private void Log( IError error )
        {
            this.log.WriteLine();
            this.log.WriteLine( error.ToString() );
            this.log.WriteLine();
        }

        /// <summary>
        /// The ILog to which errors are written.
        /// </summary>
        private readonly ILog log;

        /// <summary>
        /// The logic to execute when a fatal error has occurred.
        /// </summary>
        private readonly IShutdownLogic shutdownLogic;

        /// <summary>
        /// The factory that is used to create the dialog used to visualize the error to the user.
        /// </summary>
        private readonly IErrorReportDialogFactory errorReportDialogFactory;
    }
}
