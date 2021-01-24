// <copyright file="FileCopyDecider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.FileCopyDecider class.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    using System.IO;

    /// <summary>
    /// Implements a mechanism that decides whether a file should be copied
    /// based on a black and whitelist.
    /// </summary>
    public sealed class FileCopyDecider : IFileCopyDecider
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileCopyDecider"/> class.
        /// </summary>
        /// <param name="blackList">
        /// Contains the file to not include.
        /// </param>
        /// <param name="whiteList">
        /// Contains the file to include.
        /// </param>
        public FileCopyDecider( IBlackWhiteList blackList, IBlackWhiteList whiteList )
        {
            this.blackList = blackList;
            this.whiteList = whiteList;
        }

        /// <summary>
        /// Gets a value indicating whether the file with the 
        /// specified fileName should be copied.
        /// </summary>
        /// <param name="fileName">
        /// The relative full path of the file.
        /// </param>
        /// <returns>
        /// true if it should be copied;
        /// otherwise false.
        /// </returns>
        public bool ShouldCopy( string fileName )
        {
            if( this.IsBlacklisted( fileName ) )
            {
                return this.IsWhitelisted( fileName );
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the specified fileName
        /// is blacklisted and as such should not be copied.
        /// </summary>
        /// <param name="fileName">
        /// The full name of the file that should be copied.
        /// </param>
        /// <returns>
        /// true if it is blacklisted;
        /// otherwise false.
        /// </returns>
        private bool IsBlacklisted( string fileName )
        {
            if( this.blackList.HasFile( fileName ) )
            {
                return true;
            }

            if( this.blackList.HasExtension( Path.GetExtension( fileName ) ) )
            {
                return true;
            }

            if( this.blackList.HasDirectory( fileName ) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the specified fileName
        /// is whitelisted and as such should be copied.
        /// </summary>
        /// <param name="fileName">
        /// The full name of the file that should be copied.
        /// </param>
        /// <returns>
        /// true if it is whitelisted;
        /// otherwise false.
        /// </returns>
        private bool IsWhitelisted( string fileName )
        {
            return this.whiteList.HasFile( fileName );
        }

        private readonly IBlackWhiteList blackList;
        private readonly IBlackWhiteList whiteList;
    }
}
