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
    using Atom.ErrorReporting.Formatters;
    using Atom.ErrorReporting.Reporters;
    using Atom.Mail;
    using Atom.Mail.Modifiers;

    /// <summary>
    /// Implement an <see cref="Atom.ErrorReporting.IErrorReporter"/> that sends an error report per e-mail.
    /// </summary>
    public sealed class ZeldaMailErrorReporter : MailErrorReporter
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaMailErrorReporter class.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application.
        /// </param>
        public ZeldaMailErrorReporter( string applicationName )
            : base( applicationName, "tivtag@gmail.com", new MailBodyErrorFormatter(), new SafeMailSender() )
        {
            this.MailModifier = new AddAttachmentModifier() {
                FilePath = LogHelper.GetFileLogPath( applicationName )
            };
        }
    }
}
