// <copyright file="ManifestCreationProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Manifest.ManifestCreationProcedure class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Tools.ReleasePackager.Manifest
{
    using System.IO;
    using Atom.AutoUpdate;
    using Atom.AutoUpdate.Manifest;
    using Atom.AutoUpdate.Manifest.Serialization;

    /// <summary>
    /// Defines an IProcedure that is responsible for creating an IManifest file
    /// used by the Auto Updater of the game.
    /// </summary>
    public sealed class ManifestCreationProcedure : IProcedure
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ManifestCreationProcedure"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// The directory in which the manifest should be created.
        /// </param>
        public ManifestCreationProcedure( string workingDirectory )
            : this(
                workingDirectory,
                new ManifestBuilder( new FileSystem( workingDirectory ) ),
                new BinaryManifestSerializer() { SerializeHash = true }
            )
        {
        }

        private ManifestCreationProcedure(
            string workingDirectory,
            IManifestBuilder manifestBuilder,
            IManifestSerializer manifestSerializer )
        {
            this.workingDirectory = workingDirectory;
            this.manifestBuilder = manifestBuilder;
            this.manifestSerializer = manifestSerializer;
        }

        /// <summary>
        /// Runs this ManifestCreationProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucesfully run;
        /// otherwise false.
        /// </returns>
        public bool Run()
        {
            IManifest manifest = manifestBuilder.BuildFromDirectory( this.workingDirectory );
            this.SaveManifest( manifest );
            return true;
        }

        /// <summary>
        /// Saves the specified IManifest to a file.
        /// </summary>
        /// <param name="manifest">
        /// The IManifest to save.
        /// </param>
        private void SaveManifest( IManifest manifest )
        {
            using( var stream = new FileStream( this.GetManifestSavePath(), FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                this.manifestSerializer.Serialize( manifest, stream );
            }
        }

        /// <summary>
        /// Gets the path in which the IManifest file is saved.
        /// </summary>
        /// <returns></returns>
        private string GetManifestSavePath()
        {
            return Path.Combine( this.workingDirectory, "manifest.txt" );
        }

        private readonly string workingDirectory;
        private readonly IManifestBuilder manifestBuilder;
        private readonly IManifestSerializer manifestSerializer;
    }
}
