// <copyright file="EmitterSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Graphics.Particles.Settings.EmitterSettings class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Graphics.Particles.Settings
{
    using System.ComponentModel;
    using Atom.Xna;
    using Atom.Xna.Particles;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Implements the IEmitterSettings interface.
    /// </summary>
    public sealed class EmitterSettings : IEmitterSettings
    {
        /// <summary>
        /// Gets or sets the initial color of the particles.
        /// </summary>
        public Color ReleaseColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the texture that is used to represents
        /// the particles.
        /// </summary>
        [Editor( typeof( Zelda.Graphics.Design.TextureEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Microsoft.Xna.Framework.Graphics.Texture2D ParticleTexture
        {
            get;
            set;
        }

        /// <summary>
        /// Applies these IEmitterSettings to the specified Emitter.
        /// </summary>
        /// <param name="emitter">
        /// The emitter to 'initialize' with the settings.
        /// </param>
        public void ApplyTo( Emitter emitter )
        {
            emitter.ReleaseColor = this.ReleaseColor;
            emitter.ParticleTexture = this.ParticleTexture;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.ReleaseColor );
            context.Write( this.ParticleTexture != null ? this.ParticleTexture.Name : string.Empty );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.ReleaseColor = context.ReadColor();
            string particleTextureAssetName = context.ReadString();

            if( particleTextureAssetName.Length > 0 )
            {
                this.ParticleTexture = context.ServiceProvider.TextureLoader.Load( particleTextureAssetName );
                this.ParticleTexture.Name = particleTextureAssetName;
            }
        }
    }
}
