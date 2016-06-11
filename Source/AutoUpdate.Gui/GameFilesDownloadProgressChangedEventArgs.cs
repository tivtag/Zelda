// <copyright file="GameFilesDownloadProgressChangedEventArgs.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.GameFilesDownloadProgressChangedEventArgs class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;
    using Atom.AutoUpdate.Manifest;

    /// <summary>
    /// 
    /// </summary>
    public sealed class GameFilesDownloadProgressChangedEventArgs : EventArgs
    {
        // Summary:
        //     Gets the number of bytes received.
        //
        // Returns:
        //     An System.Int64 value that indicates the number of bytes received.
        public long BytesReceived
        { 
            get;
            private set; 
        }

        //
        // Summary:
        //     Gets the total number of bytes in a System.Net.WebClient data download operation.
        //
        // Returns:
        //     An System.Int64 value that indicates the number of bytes that will be received.
        public long TotalBytesToReceive 
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IManifestFile File
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int FileIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int FileCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytesReceived"></param>
        /// <param name="totalBytesToReceive"></param>
        /// <param name="file"></param>
        /// <param name="fileIndex"></param>
        /// <param name="fileCount"></param>
        public GameFilesDownloadProgressChangedEventArgs(
            long bytesReceived,
            long totalBytesToReceive,
            IManifestFile file, 
            int fileIndex, 
            int fileCount )
        {
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
            this.File = file;
            this.FileIndex = fileIndex;
            this.FileCount = fileCount;
        }
    }
}
