// <copyright file="LogHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.LogHelper utility class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Atom;
    using Atom.Diagnostics;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines helper methods that allow to easily log information
    /// about a specific subsystem.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Gets the full path of the file log used by the application
        /// with the specified name.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application.
        /// </param>
        /// <returns>
        /// The full file path.
        /// </returns>
        internal static string GetFileLogPath( string applicationName )
        {
            return Path.Combine( GameFolders.Logs, applicationName + "'s Log.log" );
        }

        /// <summary>
        /// Creates a new ILog that might be used be the application.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application.
        /// </param>
        /// <returns>
        /// The newly created ILog.
        /// </returns>
        private static ILog Create( string applicationName )
        {
            string fullPath = GetFileLogPath( applicationName );
            Directory.CreateDirectory( Path.GetDirectoryName(fullPath) );

            var multiLog = new MultiLog();
            multiLog.Add( new FileLog( fullPath, Encoding.Unicode ) );

#if TRACE
            multiLog.Add( new TraceLog() );
#endif

            return multiLog;
        }

        /// <summary>
        /// Creates a new ILog and initializes it for use in the application.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application.
        /// </param>
        /// <returns>
        /// The newly created ILog.
        /// </returns>
        public static ILog CreateAndInitialize( string applicationName )
        {
            var log = Create( applicationName );
            log.WriteLine( "Welcome to {0}'s log.", applicationName );

            GlobalServices.Container.AddService( typeof( ILogProvider ), new LogProvider( log ) );
            return log;
        }

        /// <summary>
        /// Writes information about the fmod audio sub-system
        /// into the game's log. This information may be of nice use if the game crashes for an user.
        /// </summary>
        /// <param name="audioSystem">The  Atom.Fmod.AudioSystem object.</param>
        /// <param name="log">The log to write the information into.</param>
        public static void LogInfo(
            Atom.Fmod.AudioSystem audioSystem,
            ILog log )
        {
            var sb = new System.Text.StringBuilder( 100 );

            sb.AppendLine();
            sb.Append( "AudioSystem..." );

            sb.AppendLine();
            sb.Append( "   Audio Driver: " );
            sb.Append( audioSystem.AudioDriverName );

            sb.AppendLine();
            sb.Append( "   FMod Version: " );
            sb.Append( audioSystem.NativeVersion );

            sb.AppendLine();
            sb.Append( "   SpeakerMode: " );
            sb.Append( audioSystem.SpeakerMode.ToString() );

            log.WriteLine( sb.ToString() );
        }

        /// <summary>
        /// Writes information about the graphics device
        /// into the game's log. This information may be of nice use if the game crashes for an user.
        /// </summary>
        /// <param name="deviceInformation">The GraphicsDeviceInformation object.</param>
        /// <param name="log">The log to write the information into.</param>
        public static void LogInfo( 
            Microsoft.Xna.Framework.GraphicsDeviceInformation deviceInformation, 
            ILog log )
        {
            var adapter       = deviceInformation.Adapter;
            var presentParams = deviceInformation.PresentationParameters;

            var culture       = CultureInfo.CurrentCulture;
            var sb            = new System.Text.StringBuilder( 800 );

            sb.AppendLine();
            sb.Append( "Preparing XNA device settings... GraphicsProfile=" );
            sb.AppendLine( deviceInformation.GraphicsProfile.ToString() );

            // Adapter Information
            sb.AppendLine( "   Adapter Information" );
            sb.Append( "       IsWideScreen=" );
            sb.Append( adapter.IsWideScreen.ToString() );;

            sb.Append( "       CurrentDisplayMode=" );
            sb.Append( adapter.CurrentDisplayMode.ToString() );

            sb.AppendLine( "   SupportedDisplayModes=" );
            foreach( DisplayMode displayMode in adapter.SupportedDisplayModes )
            {
                sb.AppendLine( displayMode.ToString() );
            }

            // Presentation Parameters:
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine( "   Presentation Parameters" );

            sb.AppendFormat(
                culture,
                "       BackBuffer=[{0}x{1} {2}]",
                presentParams.BackBufferWidth.ToString( culture ),
                presentParams.BackBufferHeight.ToString( culture ),
                presentParams.BackBufferFormat.ToString()
            );

            sb.Append( " IsFullScreen=" );
            sb.AppendLine( presentParams.IsFullScreen.ToString( culture ) );

            sb.Append( "       DepthStencilFormat=" );
            sb.Append( presentParams.DepthStencilFormat.ToString() );

            sb.Append( " DisplayOrientation=" );
            sb.Append( presentParams.DisplayOrientation.ToString() );

            sb.Append( " Bounds=" );
            sb.AppendLine( presentParams.Bounds.ToString() );

            sb.Append( "       RenderTargetUsage=" );
            sb.Append( presentParams.RenderTargetUsage.ToString() );

            sb.Append( " PresentationInterval=" );
            sb.AppendLine( presentParams.PresentationInterval.ToString() );

            sb.Append( "       MultiSampleCount=" );
            sb.Append( presentParams.MultiSampleCount.ToString() );
            sb.AppendLine();

            log.Write( sb.ToString() );
        }
    }
}
