// <copyright file="NoteVisualizer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.NoteVisualizer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Ocarina
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Zelda.Ocarina;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Provides a mechanism to visualize the notes of an Ocarina Song.
    /// </summary>
    internal sealed class NoteVisualizer
    {
        /// <summary>
        /// Initializes a new instance of the NoteVisualizer class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public NoteVisualizer( IZeldaServiceProvider serviceProvider )
        {
            var spriteLoader = serviceProvider.SpriteLoader;
            this.spriteNoteLeft  = spriteLoader.LoadSprite( "Ocarina_Note_Left" );
            this.spriteNoteRight = spriteLoader.LoadSprite( "Ocarina_Note_Right" );
            this.spriteNoteUp    = spriteLoader.LoadSprite( "Ocarina_Note_Up" );
            this.spriteNoteDown  = spriteLoader.LoadSprite( "Ocarina_Note_Down" );
        }

        /// <summary>
        /// Draws the given Note at the given position.
        /// </summary>
        /// <param name="note">The note to draw.</param>
        /// <param name="postion">The drawing position.</param>
        /// <param name="batch">The XNA SpriteBatch.</param>
        public void Draw( Note note, Vector2 postion, ISpriteBatch batch )
        {
            this.GetSprite( note ).Draw( postion, 1.0f, batch );
        }

        /// <summary>
        /// Draws the given Note at the given position.
        /// </summary>
        /// <param name="note">The note to draw.</param>
        /// <param name="postion">The drawing position.</param>
        /// <param name="color">The color the Note should be tinted in.</param>
        /// <param name="batch">The XNA SpriteBatch.</param>
        public void Draw( Note note, Vector2 postion, Xna.Color color, ISpriteBatch batch )
        {
            this.GetSprite( note ).Draw( postion, color, 1.0f, batch );
        }

        /// <summary>
        /// Gets the Sprite associated with the given Note.
        /// </summary>
        /// <param name="note">
        /// The input Note.
        /// </param>
        /// <returns>
        /// The sprite related to the given note.
        /// </returns>
        public Sprite GetSprite( Note note )
        {
            switch( note )
            {
                case Note.Left:
                    return this.spriteNoteLeft;

                case Note.Right:
                    return this.spriteNoteRight;

                case Note.Up:
                    return this.spriteNoteUp;

                case Note.Down:
                    return this.spriteNoteDown;

                default:
                    return null;
            }
        }

        /// <summary>
        /// The sprites that represent the notes.
        /// </summary>
        private readonly Sprite spriteNoteLeft, spriteNoteRight, spriteNoteUp, spriteNoteDown;
    }
}
