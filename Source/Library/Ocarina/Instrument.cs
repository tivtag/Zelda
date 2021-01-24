// <copyright file="Instrument.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Ocarina.Instrument class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Ocarina
{
    using System.Diagnostics;
    using Atom.Fmod;
    
    /// <summary>
    /// An Instrument provides a mechanism to play <see cref="Note"/>s.
    /// </summary>
    public sealed class Instrument
    {
        #region [ Initialization ]

        /// <summary>
        /// Creates a new Instrument, loading the Ocarina data.
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem object.
        /// </param>
        /// <returns>
        /// The newly created Instrument.
        /// </returns>
        public static Instrument LoadOcarina( AudioSystem audioSystem )
        {
            Debug.Assert( audioSystem != null, "audioSystem may not be null." );

            var ocarina = new Instrument();

            ocarina.soundNoteLeft = audioSystem.LoadSample( "Instrument_Ocarina_Tune_Left.ogg" );
            ocarina.soundNoteRight = audioSystem.LoadSample( "Instrument_Ocarina_Tune_Right.ogg" );
            ocarina.soundNoteUp = audioSystem.LoadSample( "Instrument_Ocarina_Tune_Up.ogg" );
            ocarina.soundNoteDown = audioSystem.LoadSample( "Instrument_Ocarina_Tune_Down.ogg" );

            return ocarina;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Plays the given <see cref="Note"/> on this Instrument.
        /// </summary>
        /// <param name="note">
        /// The <see cref="Note"/> to play.
        /// </param>
        public void PlayNote( Note note )
        {
            var sound = this.GetSound( note );

            if( sound != null )
            {
                sound.Play();
            }
        }

        /// <summary>
        /// Gets the <see cref="Sound"/> associated with the given <see cref="Note"/>.
        /// </summary>
        /// <param name="note">
        /// The input note.
        /// </param>
        /// <returns>
        /// The output sound.
        /// </returns>
        private Sound GetSound( Note note )
        {
            switch( note )
            {
                case Note.Left:
                    return this.soundNoteLeft;

                case Note.Right:
                    return this.soundNoteRight;

                case Note.Up:
                    return this.soundNoteUp;

                case Note.Down:
                    return this.soundNoteDown;

                default:
                    return null;
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The sound that is associated with the <see cref="Note.Left"/> of this Instrument.
        /// </summary>
        private Sound soundNoteLeft;

        /// <summary>
        /// The sound that is associated with the <see cref="Note.Right"/> of this Instrument.
        /// </summary>
        private Sound soundNoteRight;

        /// <summary>
        /// The sound that is associated with the <see cref="Note.Up"/> of this Instrument.
        /// </summary>
        private Sound soundNoteUp;

        /// <summary>
        /// The sound that is associated with the <see cref="Note.Down"/> of this Instrument.
        /// </summary>
        private Sound soundNoteDown;

        #endregion
    }
}
