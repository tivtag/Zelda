// <copyright file="IXmlBlackwhiteListBuilder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Tools.ReleasePackager.Copy.IXmlBlackwhiteListBuilder interface.</summary>
// <author>Paul Ennemoser</author>

namespace Tools.ReleasePackager.Copy
{
    /// <summary>
    /// Provides a mechanism for creating <see cref="IBlackWhiteList"/>s from XML.
    /// </summary>
    public interface IXmlBlackwhiteListBuilder
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
        IBlackWhiteList Build( string resourceName );
        
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
        IBlackWhiteList Build( System.Xml.Linq.XDocument document );
    }
}
