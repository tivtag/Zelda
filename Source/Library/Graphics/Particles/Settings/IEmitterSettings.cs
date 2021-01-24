// <copyright file="IEmitterSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.IEmitterSettings class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles.Settings
{
    using System.ComponentModel;
    using Atom.Xna.Particles;
    using Zelda.Saving;
    
    /// <summary>
    /// Encapsulates the settings that are used to create a new ParticleEmitter.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public interface IEmitterSettings : ISaveable
    {
        /// <summary>
        /// Applies these IEmitterSettings to the specified Emitter.
        /// </summary>
        /// <param name="emitter">
        /// The emitter to 'initialize' with the settings.
        /// </param>
        void ApplyTo( Emitter emitter );
    }
}
