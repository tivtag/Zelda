// <copyright file="CreateFilesWxsProcedure.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Installer.CreateFilesWxsProcedure class.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Installer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Dynamically creates the Filex.wxs file used by the installer.
    /// </summary>
    public sealed class CreateFilesWxsProcedure : IProcedure
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CreateFilesWxsProcedure"/> class.
        /// </summary>
        /// <param name="installerDirectory">
        /// The directory in which the Windows Installer-XML is created.
        /// </param>
        public CreateFilesWxsProcedure( string installerDirectory )
        {
            this.installerDirectory = installerDirectory;
            this.installerContentDirectory = Path.Combine( installerDirectory, "Content" );
        }

        /// <summary>
        /// Runs this CreateFilexWxsProcedure.
        /// </summary>
        /// <returns>
        /// true if this IProcedure has sucesfully run;
        /// otherwise false.
        /// </returns>
        public bool Run()
        {
            XDocument document = this.LoadBaseDocument();
            var directories = this.GetRelevantDirectoryElements( document ).ToList();

            foreach( XElement directory in directories )
            {
                this.PopulateDirectory( directory );
            }

            this.SaveDocument( document );
            return true;
        }

        /// <summary>
        /// Loads up the base File.wxs file that is populated
        /// by this CreateFilexWxsProcedure.
        /// </summary>
        /// <returns>
        /// The XDocument that has been loaded.
        /// </returns>
        private XDocument LoadBaseDocument()
        {          
            var assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = string.Format(
                 CultureInfo.InvariantCulture,
                 @"{0}.Installer.Configuration.Files.wxs.xml", 
                 assembly.GetName().Name
            );

            using( Stream stream = assembly.GetManifestResourceStream( fullResourceName ) )
            {
                return XDocument.Load( XmlReader.Create( stream ) );
            }
        }

        private void SaveDocument( XDocument document )
        {
            string savePath = Path.Combine( this.installerDirectory, @"Source\Files.wxs" );
            document.Save( savePath );
        }

        /// <summary>
        /// Gets all relevant directory XElements in the specified XDocument.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetRelevantDirectoryElements( XDocument document )
        {
            return from directory in this.GetDirectoryElements( document )
                   where IsRelevantDirectory( directory )
                   select directory;
        }

        /// <summary>
        /// Gets a value indicating whether the specified directory element
        /// is relevant for the FilesWxsProcedure.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool IsRelevantDirectory( XElement directory )
        {
            XAttribute id = directory.Attribute( "Id" );
            if( id == null )
            {
                throw new InvalidOperationException( "The directory doesn't contain the Id attribute." );
            }

            switch( id.Value )
            {
                case "GameProgramMenuFolder":
                case "ProgramMenuFolder":
                case "ProgramFilesFolder":
                case "INSTALLDIR":
                    return false;

                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Returns all "Directory" elements in the specified XDocument.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetDirectoryElements( XDocument document )
        {
            return from element in document.DescendantNodes().OfType<XElement>()
                   where element.Name.LocalName.Equals( "Directory", StringComparison.InvariantCulture )
                   select element;
        }

        /// <summary>
        /// Populates the specified directory XElement with the
        /// files that should be contained by the directory.
        /// </summary>
        /// <param name="directory">
        /// </param>
        private void PopulateDirectory( XElement directory )
        {
            string path = this.GetDirectoryPathOnHardDisc( directory );
            string[] files = Directory.GetFiles( path, "*", SearchOption.TopDirectoryOnly );

            XElement component = directory.DescendantNodes().OfType<XElement>().First(
                (element) => element.Name.LocalName.Equals( "Component", StringComparison.InvariantCulture ) 
            );

            foreach( string file in files )
            {
                XElement fileElement = this.CreateFileElement( file );
                component.Add( fileElement );
            }
        }

        /// <summary>
        /// Creates a bew XElement that descripes with the specified file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private XElement CreateFileElement( string file )
        {
            string source = file.Replace( this.installerDirectory + @"\", string.Empty );
            string name   = Path.GetFileName( file );
            string id     = name.Replace( ' ', '_' );

            var element = new XElement( XName.Get( "File", "http://schemas.microsoft.com/wix/2006/wi" ) );
            element.Add( new XAttribute( "Source", source ) );
            element.Add( new XAttribute( "Name", name ) );
            element.Add( new XAttribute( "Id", id.Replace('-', '_') ) );

            return element;
        }

        /// <summary>
        /// Gets the full directory path on the harddisc that is related to the specified
        /// directory XElement.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetDirectoryPathOnHardDisc( XElement directory )
        {
            string relative = this.GetRelativeDirectoryPathOnHardDisc( directory );
            return Path.Combine( this.installerContentDirectory, relative );
        }

        /// <summary>
        /// Gets the relative directory path on the harddisc that is related to the specified
        /// directory XElement.
        /// </summary>
        private string GetRelativeDirectoryPathOnHardDisc( XElement directory )
        {
            XAttribute name = directory.Attribute( "Name" );
            if( name == null )
            {
                throw new InvalidOperationException( "The directory doesn't contain the Name attribute." );
            }

            switch( name.Value )
            {
                case "Content":
                    return "Content";

                case "Sets":
                    return @"Content\Items\Sets";

                case "Particles":
                    return @"Content\Textures\Particles";

                default:
                    return Path.Combine( "Content", name.Value );
            }
        }

        private readonly string installerDirectory;
        private readonly string installerContentDirectory;
    }
}
