// <copyright file="Program.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.ReleasePackager.Program class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Tools.ReleasePackager
{
    using System;

    /// <summary>
    /// The Release Packager is responsible for packing up the compiled game
    /// </summary>
    /// <remarks>
    /// This works by sequentially running multiple IProcedures.
    /// </remarks>
    public static class Program
    {
        public static void Main( string[] args )
        {
            // ToDo: Use relative paths
            var packager = new Packager(
                compiledDirectory: @"C:\Projects\Zelda\Compiled\Release\net6.0-windows\win-x64",
                packagedDirectory:  @"C:\Projects\Zelda\Packaged",
                installerDirectory: @"C:\Projects\Zelda\Source\Installer"
            );
            
            bool hadError = !packager.Run();

            if( hadError )
            {
                Console.ReadKey();
            }
            Console.ReadKey();
        }
    }
}
