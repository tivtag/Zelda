// <copyright file="RandomFromListEmitterSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.RandomFromListEmitterSettings class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Particles.Settings
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Saving;
    
    /// <summary>
    /// Implements an IEmitterSettings instance that randomly
    /// selects from a list of other IEmitterSettings.
    /// </summary>
    public sealed class RandomFromListEmitterSettings : IEmitterSettings, IZeldaSetupable
    {
        /// <summary>
        /// Gets the list of IEmitterSettings from which this RandomFromListEmitterSettings
        /// randomly selects one IEmitterSetting.
        /// </summary>
        [Editor( "Zelda.Graphics.Particles.Settings.Design.EmitterSettingsListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public IList<IEmitterSettings> Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// Setups this RandomFromListEmitterSettings instance.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Applies these IEmitterSettings to the specified Emitter.
        /// </summary>
        /// <param name="emitter">
        /// The emitter to 'initialize' with the settings.
        /// </param>
        public void ApplyTo( Atom.Xna.Particles.Emitter emitter )
        {
            IEmitterSettings setting = this.GetRandomSetting();
            if( setting == null )
                return;

            setting.ApplyTo( emitter );
        }

        /// <summary>
        /// Randomly selects an IEmitterSettings instance from
        /// the list.
        /// </summary>
        /// <returns>
        /// An IEmitterSettings instance; or null.
        /// </returns>
        private IEmitterSettings GetRandomSetting()
        {
            if( this.settings.Count == 0 )
                return null;

            int index = rand.RandomRange( 0, this.settings.Count - 1 );
            return this.settings[index];
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

            context.WriteList<IEmitterSettings>( this.settings );
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

            context.ReadListInto<IEmitterSettings>( this.settings );
        }

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        /// <summary>
        /// The list of IEmitterSettings from which one IEmitterSetting is randomly picked.
        /// </summary>
        private readonly List<IEmitterSettings> settings = new List<IEmitterSettings>();
    }
}
