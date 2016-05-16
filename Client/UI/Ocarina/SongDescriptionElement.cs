// <copyright file="SongDescriptionElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.SongDescriptionElement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Ocarina
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Zelda.Ocarina;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Displays information about a single <see cref="Song"/>.
    /// </summary>
    internal sealed class SongDescriptionElement : UIContainerElement
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SongDescriptionElement class.
        /// </summary>
        /// <param name="noteVisualizer">
        /// Provides a mechanism to draw song notes.
        /// </param>
        public SongDescriptionElement( NoteVisualizer noteVisualizer )
        {
            this.FloorNumber = 3;
            this.noteVisualizer = noteVisualizer;

            var fontDescription = UIFonts.Tahoma10;

            // Description Field.
            this.descriptionField = new TextField() {
                Text = new Text(
                    fontDescription,
                    TextAlign.Left,
                    Xna.Color.White,
                    new TextBlockSplitter(
                        fontDescription,
                        325
                    )
                ),

                FloorNumber = 4,
                Position = new Atom.Math.Vector2( 10, 62 )
            };

            this.AddChild( this.descriptionField );
            this.HideAndDisable();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this SongDescriptionElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( this.song == null )
                return;

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var camera = zeldaDrawContext.Camera;
            var batch  = zeldaDrawContext.Batch;

            batch.DrawRect(
                new Rectangle(
                    0,
                    18,
                    camera.ViewSize.X,
                    94
                ),
                new Xna.Color( 0, 0, 0, 200 )
            );

            this.DrawSongName( drawContext );
            this.DrawNotes( batch );
        }

        /// <summary>
        /// Draws the name string of the Song.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawSongName( ISpriteDrawContext drawContext )
        {
            this.fontName.Draw(
                this.song.DescriptionData.LocalizedName,
                new Vector2( 10.0f, 25.0f ),
                Xna.Color.White,
                0.8f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the notes of the Song.
        /// </summary>
        /// <param name="batch">
        /// A XNA SpriteBatch object.
        /// </param>
        private void DrawNotes( ISpriteBatch batch )
        {
            Vector2 position = new Vector2( 8.0f, 43.0f );

            foreach( var note in this.song.Notes )
            {
                this.noteVisualizer.Draw( note, position, new Xna.Color( 200, 200, 200, 255 ), batch );
                position.X += 16.0f;
            }
        }

        /// <summary>
        /// Called when this SongDescriptionElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Shows the description of the specified Song.
        /// </summary>
        /// <param name="song">
        /// The song whose description should be shown.
        /// Null resets the current description.
        /// </param>
        public void ShowSong( Song song )
        {
            if( song != null )
            {
                var data = song.DescriptionData;
                this.descriptionField.Text.TextString = data.LocalizedDescription;
            }
            else
            {
                this.descriptionField.Text.TextString = string.Empty;
            }

            this.song = song;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The currently selected Song.
        /// </summary>
        private Song song;

        /// <summary>
        /// The TextField that shows the description of the Song.
        /// </summary>
        private readonly TextField descriptionField;

        /// <summary>
        /// The font used to draw the name of the Song.
        /// </summary>
        private readonly IFont fontName = UIFonts.TahomaBold10;

        /// <summary>
        /// Provides a mechanism to draw song notes.
        /// </summary>
        private readonly NoteVisualizer noteVisualizer;

        #endregion
    }
}
