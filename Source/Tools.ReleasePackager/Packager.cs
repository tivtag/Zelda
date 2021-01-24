// <copyright file="Packager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Packager class.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager
{
    using System;
    using System.IO;
    using Tools.ReleasePackager.Copy;
    using Tools.ReleasePackager.Installer;
    using Tools.ReleasePackager.Manifest;
    using System.Diagnostics;

    /// <summary>
    /// Defines the main IProcedure of the Release Packager.
    /// </summary>
    public sealed class Packager : ParentProcedure
    {
        /// <summary>
        /// Initializes a new instance of the Packager class.
        /// </summary>
        /// <param name="compiledDirectory">
        /// The directory that contains the compiled game files.
        /// </param>
        /// <param name="packagedDirectory">
        /// The directory that should contain the packaged game files.
        /// </param>
        /// <param name="installerDirectory">
        /// The directory that contains the installer files.
        /// </param>
        public Packager( string compiledDirectory, string packagedDirectory, string installerDirectory )
        {
            this.compiledDirectory = compiledDirectory;
            this.packagedDirectory = packagedDirectory;
            this.installerDirectory = installerDirectory;

            Directory.CreateDirectory( packagedDirectory );
        }

        /// <summary>
        /// Runs this Packager.
        /// </summary>
        /// <returns>
        /// true if the Packager has sucesfully run;
        /// otherwise false.
        /// </returns>
        public override bool Run()
        {
            var copyToPackageProcedure = this.CreateCopyFromCompiledToPackegedProcedure();
            if( !RunProcedure( copyToPackageProcedure, "Copy files from Compiled to Packaged" ) )
            {
                return false;
            }

            var installerProcedure = new InstallerProcedure( this.packagedDirectory, this.installerDirectory );
            RunProcedure( installerProcedure );

            var manifestCreationProcedure = new ManifestCreationProcedure( this.packagedDirectory );
            if( !RunProcedure( manifestCreationProcedure, "Create Manifest" ) )
            {
                return false;
            }

            OpenExplorer( this.packagedDirectory );
            return true;
        }

        /// <summary>
        /// Opens the Windows Explorer at the specified path.
        /// </summary>
        /// <param name="path">
        /// The path that should be opened.
        /// </param>
        private static void OpenExplorer( string path )
        {
            if( Directory.Exists( path ) )
            {
                Process.Start( "explorer.exe", path );
            }
        }

        /// <summary>
        /// Creates the IProcedure responsible for copying files from the Compiled directory
        /// to the Packaged directory.
        /// </summary>
        /// <returns>
        /// The IProcedure that has been created.
        /// </returns>
        private IProcedure CreateCopyFromCompiledToPackegedProcedure()
        {
            var blackWhiteListBuilder = new XmlBlackWhiteListBuilder();
            var filesToCopyDecider = new FileCopyDecider( 
                blackWhiteListBuilder.Build( "Copy.Configuration.PackageBlackList.xml" ),
                blackWhiteListBuilder.Build( "Copy.Configuration.PackageWhiteList.xml" )
            );

            return new CopyProcedure( 
                this.compiledDirectory,
                this.packagedDirectory,
                new FilesToCopyFinder( this.compiledDirectory, filesToCopyDecider )
            );
        }

        private readonly string compiledDirectory;
        private readonly string packagedDirectory;
        private readonly string installerDirectory;
    }
}
