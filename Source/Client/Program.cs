// <copyright file="Program.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the main entry point into the game.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using System.Threading;

    /// <summary>
    /// Stores the entry point of the game.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the game.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using Mutex mutex = new Mutex( true, "Zelda.Mutex", out bool createdNew );

            if( createdNew )
            {
                Run();
            }
        }

        private static void Run()
        {
            ZeldaGame game = null;

            try
            {
                game = new ZeldaGame();
                game.Run();
            }
            catch( Exception exc )
            {
                if( game != null )
                {
                    game.ReportError( exc, true );
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if( game != null )
                {
                    try
                    {
                        game.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}