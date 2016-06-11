// <copyright file="IProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.IProcedure interface.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager
{
    /// <summary>
    /// Represents an execution part of the Release Packager tool.
    /// </summary>
    public interface IProcedure
    {
        /// <summary>
        /// Runs this IProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucesfully run;
        /// otherwise false.
        /// </returns>
        bool Run();
    }
}
