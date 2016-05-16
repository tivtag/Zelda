// <copyright file="InstallerProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.ReleasePackager.Installer.InstallerProcedure class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.ReleasePackager.Installer
{
    using System.IO;
    using Tools.ReleasePackager.Copy;

    /// <summary>
    /// Defines the ParentProcedure that encapsulates the logic related
    /// to updating the Windows Installer Xml of the game.
    /// </summary>
    public sealed class InstallerProcedure : ParentProcedure
    {
        public InstallerProcedure( string packagedDirectory, string installerDirectory )
        {
            this.packagedDirectory = packagedDirectory;
            this.installerDirectory = installerDirectory;
        }

        /// <summary>
        /// Runs this InstallerProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucesfully run;
        /// otherwise false.
        /// </returns>
        public override bool Run()
        {
            var copyToInstallerProcedure = this.CreateCopyFromPackegedToInstallerProcedure();
            if( !RunProcedure( copyToInstallerProcedure, "Copy files from Packaged to Installer" ) )
                return false;

            var createFilexWxsProcedure = new CreateFilesWxsProcedure( this.installerDirectory );
            if( !RunProcedure( createFilexWxsProcedure, "Create Installer files" ) )
                return false;

            return true;
        }

        /// <summary>
        /// Creates the IProcedure responsible for copying files from the Packaged directory
        /// to the Installer directory.
        /// </summary>
        /// <returns>
        /// The IProcedure that has been created.
        /// </returns>
        private IProcedure CreateCopyFromPackegedToInstallerProcedure()
        {
            return new CopyProcedure(
                this.packagedDirectory,
                Path.Combine( this.installerDirectory, "Content" ),
                new FilesToCopyFinder( this.packagedDirectory, new AllowAllFileCopyDecider() )
            );
        }

        private readonly string packagedDirectory;
        private readonly string installerDirectory;
    }
}
