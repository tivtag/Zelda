// <copyright file="LocalizableText.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.LocalizableText class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.ComponentModel;

    /// <summary>
    /// Represents a text that is localized that gets localized
    /// using the <see cref="LocalizableTextResources.ResourceManager"/>.
    /// This class can't be inherited.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class LocalizableText
    {
        /// <summary>
        /// Gets or sets the resource id that is used to receive the <see cref="LocalizedText"/>.
        /// </summary>
        public string Id
        {
            get 
            {
                return this.id; 
            }

            set
            {
                this.id = value ?? string.Empty;

                try
                {
                    this.LocalizedText = LocalizableTextResources.ResourceManager.GetString( this.Id );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedText = value;
                }
            }
        }

        /// <summary>
        /// Gets the localuzed text stored by this <see cref="LocalizableText"/>.
        /// </summary>
        public string LocalizedText
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents the storage field of the <see cref="Id"/> property.
        /// </summary>
        private string id;
    }
}
