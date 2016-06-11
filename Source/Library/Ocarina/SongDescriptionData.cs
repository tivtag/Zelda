// <copyright file="SongDescriptionData.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.SongDescriptionData class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Ocarina
{
    using System.Diagnostics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Encapsulates the data of a <see cref="Song"/>
    /// that is visible to the player.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SongDescriptionData
    {
        /// <summary>
        /// Gets the localized name of the Song.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                return this.localizedName; 
            }
        }

        /// <summary>
        /// Gets the localized description of the Song.
        /// </summary>
        public string LocalizedDescription
        {
            get
            { 
                return this.localizedDescription;
            }
        }

        /// <summary>
        /// Gets the color of the note that is used to
        /// visualize the Song in the song list.
        /// </summary>
        public Color NoteColor
        {
            get
            { 
                return this.noteColor;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SongDescriptionData class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the Song.
        /// </param>
        /// <param name="localizedDescription">
        /// The localized description of the Song.
        /// </param>
        /// <param name="noteColor">
        /// The color of the note that is used to visualize the Song in the song list.
        /// </param>
        public SongDescriptionData( string localizedName, string localizedDescription, Color noteColor )
        {
            Debug.Assert( localizedName != null, "localizedName may not be null." );
            Debug.Assert( localizedDescription != null, "localizedDescription may not be null." );
   
            this.localizedName = localizedName;
            this.localizedDescription = localizedDescription;
            this.noteColor = noteColor;
        }

        /// <summary>
        /// The localized name of the Song.
        /// </summary>
        private readonly string localizedName;

        /// <summary>
        /// The localized description of the Song.
        /// </summary>
        private readonly string localizedDescription;

        /// <summary>
        /// The color of the note that is used to visualize the Song
        /// in the song list.
        /// </summary>
        private readonly Color noteColor;
    }
}
