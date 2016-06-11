// <copyright file="RefactorResult.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.RefactorResult class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{   
    /// <summary>
    /// Represents the result of a single refactoring step or a full refactor operations.
    /// </summary>
    public enum RefactorResult
    {
        /// <summary>
        /// States that the refactoring step was not successful.
        /// </summary>
        Error,

        /// <summary>
        /// States that there was nothing to refactor.
        /// </summary>
        Nothing,

        /// <summary>
        /// States that the refactoring step was successful.
        /// </summary>
        Success
    }
}
