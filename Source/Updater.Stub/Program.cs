// <copyright file="Program.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Updater.Program class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Updater
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// The Updater.Stub (Zelda Updater.exe) re-directs the actual updating logic
    /// into the shadow-copied Updater.Gui (Zelda.Updater.Gui.exe) assembly.
    /// This is required or the Zelda.Updater.Gui.exe and its used dlls would be locked
    /// and as such not updateable.
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main( string[] args )
        {
            bool createdNew = true;
            using( Mutex mutex = new Mutex( true, "Zelda.Updater.Mutex", out createdNew ) )
            {
                if( createdNew )
                {
                    Run();
                }
            }
        }

        private static void Run()
        {
            string startUpPath = Path.GetDirectoryName( Assembly.GetEntryAssembly().Location );
            string updaterAssembly = Path.Combine( startUpPath, @"Content\Updater\Zelda.Updater.Gui.exe" );

            var domain = AppDomain.CreateDomain( "Update Domain" );
            domain.SetShadowCopyFiles();
            
            domain.ExecuteAssembly( updaterAssembly );
        }
    }
}