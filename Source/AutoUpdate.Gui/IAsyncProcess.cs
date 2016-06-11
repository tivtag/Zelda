// <copyright file="IAsyncProcess.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.IAsyncProcess interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Updater
{
    using System;

    /// <summary>
    /// Represents a process that runs asynchronous to the rest of the application.
    /// </summary>
    public interface IAsyncProcess
    {
        /// <summary>
        /// Raised when the asynchronous has been completed.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Raises when an error has ocurred during the asynchronous process.
        /// </summary>
        event EventHandler<System.IO.ErrorEventArgs> Errored;
    }
}
