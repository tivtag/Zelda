// <copyright file="ProfileSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ProfileSettings class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides access to various game related folder paths.
    /// </summary>
    public static class GameFolders
    {
        /// <summary>
        /// Gets the full path to the folder that contains the profile data.
        /// </summary>
        public static string Profiles
        {
            get
            {
                return Path.Combine( UserData, "Profiles" );
            }
        }

        /// <summary>
        /// Gets the full path to the folder that contains the logging data.
        /// </summary>
        public static string Logs
        {
            get
            {
                return Path.Combine( UserData, "Logs" );
            }
        }

        /// <summary>
        /// Gets the full path to the folder that contains the screenshots that have been taken by the user.
        /// </summary>
        public static string Screenshots
        {
            get
            {
                return Path.Combine( UserData, "Screenshots" );
            }
        }

        /// <summary>
        /// Gets the full path to the folder that contains the any user specific data.
        /// </summary>
        public static string UserData
        {
            get
            {
                string profileFolder = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

                profileFolder = Path.Combine( profileFolder, "My Games" );
                profileFolder = Path.Combine( profileFolder, Resources.GameName );

                return profileFolder;
            }
        }
    }
}
