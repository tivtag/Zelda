// <copyright file="XmlBlackWhiteListBuilder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.XmlBlackWhiteListBuilder class.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Implements a mechanism for creating <see cref="IBlackWhiteList"/>s from XML.
    /// </summary>
    public sealed class XmlBlackWhiteListBuilder : IXmlBlackwhiteListBuilder
    {
        /// <summary>
        /// Builds an <see cref="IBlackWhiteList"/> by loading xml data from
        /// the Assembly manifest resource with the given resourceName.
        /// </summary>
        /// <param name="resourceName">
        /// The name of the resource.
        /// </param>
        /// <returns>
        /// The loaded IBlackWhiteList.
        /// </returns>
        public IBlackWhiteList Build( string resourceName )
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = string.Format(
                 CultureInfo.InvariantCulture,
                 @"{0}.{1}", 
                 assembly.GetName().Name,
                 resourceName 
            );

            using( var stream = assembly.GetManifestResourceStream( fullResourceName ) )
            {
                var document = XDocument.Load( XmlReader.Create( stream ) );
                return Build( document );
            }
        }

        /// <summary>
        /// Builds an <see cref="IBlackWhiteList"/> by loading the data stored in
        /// the specified XDocument.
        /// </summary>
        /// <param name="document">
        /// The XDocument to query for IBlackWhiteList data.
        /// </param>
        /// <returns>
        /// The loaded IBlackWhiteList
        /// </returns>
        public IBlackWhiteList Build( XDocument document )
        {
            var directories = 
                from dir in document.Descendants( "directory" )
                select dir.Value;

            var files = 
                from file in document.Descendants( "file" )
                select file.Value;

            var extensions = 
                from file in document.Descendants( "extension" )
                select file.Value;
            
            return new BlackWhiteList( directories.ToArray(), files.ToArray(), extensions.ToArray() );
        }
    }
}
