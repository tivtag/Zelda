// <copyright file="ZeldaAudioSystem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Audio.ZeldaAudioSystem class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Audio
{
    using System;
    using Atom.Diagnostics;
    using Atom.Fmod;
    using Atom.Fmod.Native;

    /// <summary>
    /// Represents the FMOD AudioSystem used by the Zelda game.
    /// </summary>
    public sealed class ZeldaAudioSystem : AudioSystem
    {
        /// <summary>
        /// Initializes a new instance of the ZeldaAudioSystem class.
        /// </summary>
        public ZeldaAudioSystem()
        {
#if DEBUG           
            this.ThrowExceptionOnResourceNotFound = true;
#else
            this.ThrowExceptionOnResourceNotFound = false;
#endif
        }

        /// <summary>
        /// Initializes this <see cref="ZeldaAudioSystem"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// true if the initialization was successful; -or- otherwise false.
        /// </returns>
        public bool Initialize( IZeldaServiceProvider serviceProvider )
        {
            this.ErrorLog = serviceProvider.Log;
            try
            {
                this.Initialize( INITFLAGS.NORMAL, 64, SPEAKERMODE.STEREO );

                this.LoadStaticSounds( serviceProvider );
                LogHelper.LogInfo( this, this.ErrorLog );
                return true;
            }
            catch( Exception ex )
            {
                this.ErrorLog.WriteLine( "Error initializing FMODEx Audio System: " + ex.ToString() );
                return false;
            }
        }
        
        /// <summary>
        /// Loads all statically accessable Sounds.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void LoadStaticSounds( IZeldaServiceProvider serviceProvider )
        {
            Zelda.Items.ItemSounds.Load( this, serviceProvider.Rand );
        }
    }
}
