// <copyright file="Program.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.ReleasePackager.Program class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
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
    sealed class Program
    {
        static void Main( string[] args )
        {
            // ToDo: Use relative paths
            var packager = new Packager(
                // Compiled 
                @"C:\Projects\Zelda\Compiled\Release",

                // Packaged
                @"C:\Projects\Zelda\Packaged",

                // Installer
                @"C:\Projects\Zelda\Installer"
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
