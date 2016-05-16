// <copyright file="DefaultRainEmitterSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.DefaultRainEmitterSettings class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Particles.Settings
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines the default settings for the RainEmitter.
    /// </summary>
    public sealed class DefaultRainEmitterSettings : IEmitterSettings, IZeldaSetupable
    {
        /// <summary>
        /// Indenitfies the texture used for rain by default.
        /// </summary>
        private const string ParticleTextureName = @"Content/Textures/Particles/RainDrop3";

        /// <summary>
        /// Gets the texture that is used to represents
        /// the particles.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Microsoft.Xna.Framework.Graphics.Texture2D ParticleTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Applies these IEmitterSettings to the specified Emitter.
        /// </summary>
        /// <param name="emitter">
        /// The emitter to 'initialize' with the settings.
        /// </param>
        public void ApplyTo( Atom.Xna.Particles.Emitter emitter )
        {
            emitter.ParticleTexture = this.ParticleTexture;
        }

        /// <summary>
        /// Initializes a new instance of the DefaultRainEmitterSettings class.
        /// </summary>
        public DefaultRainEmitterSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DefaultRainEmitterSettings class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public DefaultRainEmitterSettings( IZeldaServiceProvider serviceProvider )
        {
            this.Setup( serviceProvider );
        }

        /// <summary>
        /// Setups this DefaultRainEmitterSettings instance.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.ParticleTexture = serviceProvider.TextureLoader.Load( ParticleTextureName );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // no op.
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Zelda.Saving.ISaveable.Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            // no op.
        }
    }
}
