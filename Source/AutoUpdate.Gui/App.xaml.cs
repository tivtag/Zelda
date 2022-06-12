// <copyright file="App.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Updater.App class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Updater
{
    using System.Windows;

    /// <summary>
    /// The Zelda.Updater application is reponsible for
    /// updating the game by downloading only the files
    /// that have been updated.
    /// </summary>
    public sealed partial class App : Application
    {
        //protected override void OnStartup( StartupEventArgs e )
        //{
        //    string firstArg = e.Args.FirstOrDefault();

        //    if( !string.IsNullOrWhiteSpace( firstArg ) )
        //    {
        //        if( Directory.Exists( firstArg ) )
        //        {
        //            Directory.SetCurrentDirectory( firstArg );
        //        }
        //    }
        //    else
        //    {
        //        Directory.SetCurrentDirectory( @"C:\Users\paule\Desktop\ZeldaBlackCrown" );
        //    }

        //    base.OnStartup( e );
        //}
    }
}
