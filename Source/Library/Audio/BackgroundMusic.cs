// <copyright file="ISaveable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.ISaveable interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Audio
{
    using System.ComponentModel;
    using Zelda.Core.Requirements;
    using Zelda.Saving;

    /// <summary>
    /// Represents a music resource that is played in the background.
    /// </summary>
    /// <remarks>
    /// Every scene has a list of BackgroundMusic from which is randomly choosen.
    /// </remarks>
    public sealed class BackgroundMusic : ISaveable
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies this BackgroundMusic.
        /// </summary>
        /// <remarks>
        /// This does not include the path; but does include the extension.
        /// </remarks>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the volumne this BackgroundMusic is played back at.
        /// </summary>
        /// <value>
        /// A value between 0.0 = silent and 1.0 = full. The default value is 1.0f.
        /// </value>
        public float Volumne
        {
            get
            {
                return this.volumne;
            }

            set
            {
                this.volumne = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRequirement"/> that must be fulfilled for this BackgroundMusic to be
        /// allowed to be played.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [Editor( typeof( Zelda.Core.Requirements.Design.RequirementEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IRequirement Requirement
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.FileName ?? string.Empty );
            context.Write( this.Volumne );
            context.WriteObject( this.Requirement );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.FileName = context.ReadString();
            this.Volumne = context.ReadSingle();
            this.Requirement = context.ReadObject<IRequirement>();
        }
        
        /// <summary>
        /// Represents the storage field of the <see cref="Volumne"/> property.
        /// </summary>
        private float volumne = 1.0f;
    }
}
