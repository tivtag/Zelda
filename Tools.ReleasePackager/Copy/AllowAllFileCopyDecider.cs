// <copyright file="AllowAllFileCopyDecider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.AllowAllFileCopyDecider class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager.Copy
{
    /// <summary>
    /// Implements an IFileCopyDecider that decides to copy -all- files.
    /// </summary>
    public sealed class AllowAllFileCopyDecider : IFileCopyDecider
    {
        public bool ShouldCopy( string fileName )
        {
            return true;
        }
    }
}
