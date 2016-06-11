// <copyright file="OcarinaWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.OcarinaWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Ocarina
{
    using System.Collections.Generic;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities.Drawing;
    using Zelda.GameStates;
    using Zelda.Ocarina;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// The <see cref="OcarinaWindow"/> is used to allow the
    /// player access to his ocarina skills.
    /// </summary>
    internal sealed class OcarinaWindow : IngameWindow
    {
        #region [ Contants ]

        /// <summary>
        /// The maximum number of notes a song can have.
        /// </summary>
        private const int MaximumPlayedNotes = 6;

        /// <summary>
        /// The minimum time that must have been passed 
        /// before another note can be played.
        /// </summary>
        private const float NoteCooldown = 0.5f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the OcarinaBox associated with the current player.
        /// </summary>
        private OcarinaBox OcarinaBox
        {
            get
            {
                return this.Player.OcarinaBox;
            }
        }

        /// <summary>
        /// Gets the currently used Instrument.
        /// </summary>
        private Instrument Instrument
        {
            get
            {
                return this.OcarinaBox.Ocarina;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IngameWindow can currently be opened.
        /// </summary>
        public override bool CanBeOpened
        {
            get
            {
                if( this.OcarinaBox.IsPlaying )
                    return false;

                if( this.Player.IsCasting || this.Player.Moveable.IsSwimming )
                    return false;

                return this.Player.DrawDataAndStrategy.SpecialAnimation == PlayerSpecialAnimation.None;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the OcarinaWindow class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        public OcarinaWindow( IZeldaServiceProvider serviceProvider )
        {
            this.noteVisualizer = new NoteVisualizer( serviceProvider );
            this.songListElement = new ComposedSongListElement( noteVisualizer, serviceProvider );

            this.Size      = serviceProvider.ViewSize;
            this.spriteBox = serviceProvider.SpriteLoader.LoadSprite( "Ocarina_Play_Box" );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this OcarinaWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var viewSize         = zeldaDrawContext.Camera.ViewSize;
            var batch            = zeldaDrawContext.Batch;

            batch.DrawRect(
                new Xna.Rectangle( 0, 0, viewSize.X, viewSize.Y ),
                UIColors.LightWindowBackground,
                0.001f
            );

            this.DrawBoxAndNotes( batch );
        }

        /// <summary>
        /// Draws the ocarina note box and the currently played notes.
        /// </summary>
        /// <param name="batch">
        /// The current SpriteBatch.
        /// </param>
        private void DrawBoxAndNotes( ISpriteBatch batch )
        {
            Vector2 boxCorner = new Vector2(
                (int)((this.Width / 2.0f) - (this.spriteBox.Width / 2.0f)),
                (int)((this.Height * 0.7f) + 9 - (this.spriteBox.Height / 2.0f))
            );

            this.spriteBox.Draw( boxCorner, 0.5f, batch );

            float offsetX = boxCorner.X + 70.0f;
            float offsetY = boxCorner.Y + 19.0f;

            foreach( var note in this.playedNotes )
            {
                Vector2 postion = new Vector2(
                    offsetX,
                    offsetY + GetOffsetY( note )
                );

                noteVisualizer.Draw( note, postion, batch );
                offsetX += 25.0f;
            }
        }

        /// <summary>
        /// Gets the offset on the y-axis the given Note should be drawn at.
        /// </summary>
        /// <param name="note">
        /// The input Note.
        /// </param>
        /// <returns>
        /// The offset related to the given note.
        /// </returns>
        private static float GetOffsetY( Note note )
        {
            switch( note )
            {
                default:
                case Note.Up:
                    return 0.0f;

                case Note.Left:
                    return 8.0f;

                case Note.Right:
                    return 16.0f;

                case Note.Down:
                    return 24.0f;
            }
        }

        /// <summary>
        /// Called when this OcarinaWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            if( this.noteCooldown >= 0.0f )
            {
                this.noteCooldown -= updateContext.FrameTime;
            }

            this.Player.DrawDataAndStrategy.Update( (ZeldaUpdateContext)updateContext );
        }

        /// <summary>
        /// Called when this OcarinaWindow is opening.
        /// </summary>
        protected override void Opening()
        {
            this.ClearNotes();
            
            // Reduce the volume of the background music.
            var channelGroup = this.Player.IngameState.BackgroundMusic.ChannelGroup;
            channelGroup.Volume = 0.5f;

            // Change player graphics:
            var playerDrawData = this.Player.DrawDataAndStrategy;

            this.oldPlayerAnimation = playerDrawData.SpecialAnimation;
            playerDrawData.SpecialAnimation = PlayerSpecialAnimation.PlayOcarina;

            // Elements:
            this.songListElement.ShowAndEnable();

            if( firstOpen )
            {
                this.songListElement.Toggle( true );
                firstOpen = false;
            }
        }

        /// <summary>
        /// Called when this OcarinaWindow is closing.
        /// </summary>
        protected override void Closing()
        {
            // Restore the volume of the background music.
            var channelGroup = this.Player.IngameState.BackgroundMusic.ChannelGroup;
            channelGroup.Volume = 1.0f;

            // Restore player graphics:
            var playerDrawData = this.Player.DrawDataAndStrategy;
            playerDrawData.SpecialAnimation = oldPlayerAnimation;

            // Elements:
            this.songListElement.HideAndDisable();
        }

        /// <summary>
        /// Plays the given Note.
        /// </summary>
        /// <param name="note">
        /// The Note to play.
        /// </param>
        private void PlayNote( Note note )
        {            
            if( this.noteCooldown > 0.0f || this.Instrument == null )
                return;
            
            if( this.playedNotes.Count >= MaximumPlayedNotes )
            {
                this.ClearNotes();
            }

            this.playedNotes.Add( note );
            this.Instrument.PlayNote( note );

            this.AnalyzePlayedNotes();
            this.noteCooldown = NoteCooldown;
        }

        /// <summary>
        /// Analyzes the notes the player has played, so far.
        /// </summary>
        private void AnalyzePlayedNotes()
        {
            if( this.Player.IsDead )
                return;

            if( this.OcarinaBox.PlaySong( this.playedNotes ) )
            {
                // Close this window.
                var ingameState = (IngameState)this.Player.IngameState;
                ingameState.UserInterface.ToggleWindow( this );
            }
        }

        /// <summary>
        /// Clears the list of notes the player has played.
        /// </summary>
        private void ClearNotes()
        {
            this.playedNotes.Clear();
        }

        /// <summary>
        /// Attempts to remove the last played note.
        /// </summary>
        private void DeleteLastNote()
        {
            if( this.playedNotes.Count > 0 )
            {
                this.playedNotes.RemoveAt( this.playedNotes.Count - 1 );
            }
        }

        /// <summary>
        /// Handles keyboard input related to the OcarinaWindow.
        /// </summary>
        /// <param name="keyState">The current state of the keyboard.</param>
        /// <param name="oldKeyState">The state of the keyboard one frame ago.</param>
        protected override void HandleKeyInput( 
            ref Microsoft.Xna.Framework.Input.KeyboardState keyState,
            ref Microsoft.Xna.Framework.Input.KeyboardState oldKeyState )
        {
            const Keys KeyNoteUp    = Keys.D1, KeyNoteUp2    = Keys.W;
            const Keys KeyNoteLeft  = Keys.D2, KeyNoteLeft2  = Keys.A;
            const Keys KeyNoteRight = Keys.D3, KeyNoteRight2 = Keys.D;
            const Keys KeyNoteDown  = Keys.D4, KeyNoteDown2  = Keys.S;
            const Keys KeyDelete    = Keys.Back, KeyDelete2  = Keys.Delete;

            if( (keyState.IsKeyDown( KeyNoteLeft ) && oldKeyState.IsKeyUp( KeyNoteLeft )) ||
                (keyState.IsKeyDown( KeyNoteLeft2 ) && oldKeyState.IsKeyUp( KeyNoteLeft2 )) )
            {
                this.PlayNote( Note.Left );
            }
            else if(
                (keyState.IsKeyDown( KeyNoteRight ) && oldKeyState.IsKeyUp( KeyNoteRight )) ||
                (keyState.IsKeyDown( KeyNoteRight2 ) && oldKeyState.IsKeyUp( KeyNoteRight2 )) )
            {
                this.PlayNote( Note.Right );
            }
            else if( 
                (keyState.IsKeyDown( KeyNoteUp ) && oldKeyState.IsKeyUp( KeyNoteUp )) ||
                (keyState.IsKeyDown( KeyNoteUp2 ) && oldKeyState.IsKeyUp( KeyNoteUp2 )) )
            {
                this.PlayNote( Note.Up );
            }
            else if( 
                (keyState.IsKeyDown( KeyNoteDown ) && oldKeyState.IsKeyUp( KeyNoteDown )) ||
                (keyState.IsKeyDown( KeyNoteDown2 ) && oldKeyState.IsKeyUp( KeyNoteDown2 )) )
            {
                this.PlayNote( Note.Down );
            }
            else if(
                (keyState.IsKeyDown( KeyDelete ) && oldKeyState.IsKeyUp( KeyDelete )) ||
                (keyState.IsKeyDown( KeyDelete2 ) && oldKeyState.IsKeyUp( KeyDelete2 )) )
            {
                this.DeleteLastNote();
            }
        }

        /// <summary>
        /// Adds the child elements of this IngameWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.songListElement );
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.songListElement );
        }

        /// <summary>
        /// Called when the PlayerEntity that owns this IngameWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.songListElement.Reset();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whether the OcarinaWindow was open before.
        /// </summary>
        private bool firstOpen = true;

        /// <summary>
        /// States the time until another note can be played.
        /// </summary>
        private float noteCooldown;

        /// <summary>
        /// The Notes that have been played by the player.
        /// </summary>
        private readonly List<Note> playedNotes = new List<Note>( MaximumPlayedNotes );

        /// <summary>
        /// Shows all songs known to the player.
        /// </summary>
        private readonly ComposedSongListElement songListElement;

        /// <summary>
        /// Provides a mechanism to draw the notes on the Ocarina Window.
        /// </summary>
        private readonly NoteVisualizer noteVisualizer;

        #region > Visualization <

        /// <summary>
        /// Stores the animation the player was in before he was playing on the Ocarina.
        /// </summary>
        private PlayerSpecialAnimation oldPlayerAnimation;

        /// <summary>
        /// The Sprite that represents the box in which the notes/notes are shown.
        /// </summary>
        private readonly Sprite spriteBox;
        
        #endregion

        #endregion
    }
}
