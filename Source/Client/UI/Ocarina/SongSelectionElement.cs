// <copyright file="SongSelectionElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.SongSelectionElement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Ocarina
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Ocarina;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the player to select a song from a list of all Songs
    /// the player has learned.
    /// </summary>
    internal sealed class SongSelectionElement : UIElement
    {
        #region [ Constants ]

        /// <summary>
        /// The maximum number of songs that can be seen at the same time in the Song Selection Element.
        /// </summary>
        private const int MaximumVisibleSongCount = 5;

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when the currently selected Song has changed.
        /// </summary>
        public event Atom.RelaxedEventHandler<Song> SelectedSongChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the PlayerEntity whose songs are selectable in this SongSelectionElement.
        /// </summary>
        private Zelda.Entities.PlayerEntity Player
        {
            get
            {
                var userInterface = (ZeldaUserInterface)this.Owner;
                var scene = userInterface.Scene;
                return scene.Player;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SongSelectionElement class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public SongSelectionElement( IZeldaServiceProvider serviceProvider )
        {
            this.FloorNumber = 3;

            this.SetupNoteButtons( serviceProvider );
            this.HideAndDisable();
        }

        /// <summary>
        /// Setups and creates the note buttons of this SongSelectionElement.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void SetupNoteButtons( IZeldaServiceProvider serviceProvider )
        {
            var spriteNote = serviceProvider.SpriteLoader.LoadSprite( "Ocarina_Note" );

            for( int i = 0; i < this.noteButtons.Length; ++i )
            {
                this.noteButtons[i] = this.CreateAndSetupNoteButton( i, spriteNote );
            }
        }

        /// <summary>
        /// Creates and setups a new note button.
        /// </summary>
        /// <param name="index">The index of the new button.</param>
        /// <param name="spriteNote">The note sprite.</param>
        /// <returns>The newly created button.</returns>
        private SpriteButton CreateAndSetupNoteButton( int index, Sprite spriteNote )
        {
            var button = new SpriteButton() {
                ColorDefault = new Xna.Color( 255, 255, 255, 155 ),
                SpriteDefault = spriteNote,
                SpriteSelected = spriteNote,

                Position = new Vector2( 5 + ((spriteNote.Width + 5) * index), 2 ),
                FloorNumber = 4,

                PassInputToSubElements = false
            };

            button.Clicked += OnNoteButtonClicked;

            return button;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Shows, enables and refreshes this SongSelectionSelement.
        /// </summary>
        public void ShowEnableAndRefresh()
        {
            this.RefreshNoteButtons();
            this.ShowAndEnable();
        }

        /// <summary>
        /// Selects a Song if no Song is currently selected.
        /// </summary>
        public void SelectSongIfNoneSelected()
        {
            if( this.selectedSong == null )
            {
                var button = this.noteButtons[0];
                var song   = GetSongRelatedTo( button );

                if( song != null )
                {
                    this.Select( button );
                }
            }
        }

        /// <summary>
        /// Deselects the currently selected song (if any).
        /// </summary>
        public void DeselectSong()
        {
            this.Select( null );
        }

        /// <summary>
        /// Refreshes the state of all note buttons that link to the Songs
        /// of the player.
        /// </summary>
        private void RefreshNoteButtons()
        {
            var ocarinaBox = this.Player.OcarinaBox;

            for( int noteIndex = 0; noteIndex < MaximumVisibleSongCount; ++noteIndex )
            {
                this.RefreshNoteButton( noteIndex, ocarinaBox );
            }
        }

        /// <summary>
        /// Refreshes the note button at the given index.
        /// </summary>
        /// <param name="noteIndex">
        /// The index of the note button to refresh
        /// </param>
        /// <param name="ocarinaBox">
        /// The OcarinaBox that contains the Songs the player knows.
        /// </param>
        private void RefreshNoteButton( int noteIndex, OcarinaBox ocarinaBox )
        {
            var button    = this.noteButtons[noteIndex];
            int songIndex = noteIndex + this.indexOffset;

            if( ocarinaBox.IsValidIndex( songIndex ) )
            {
                this.RefreshNoteButton( ocarinaBox.GetSong( songIndex ), button );
            }
            else
            {
                this.RefreshNoteButton( null, button );
            }
        }

        /// <summary>
        /// Refreshes the given note button to link to the given Song.
        /// </summary>
        /// <param name="song">
        /// The Song the note button should link to. 
        /// If null the button is totally un-linked.
        /// </param>
        /// <param name="button">
        /// The note button to refresh.
        /// </param>
        private void RefreshNoteButton( Song song, SpriteButton button )
        {
            if( song != null )
            {
                button.ColorDefault = GetDefaultButtonColor( song.DescriptionData );
                button.ColorSelected = GetSelectedButtonColor( song.DescriptionData );

                button.IsSelected = (song == this.selectedSong);
                button.ShowAndEnable();
            }
            else
            {
                button.IsSelected = false;
                button.HideAndDisable();
            }

            button.Tag = song;
        }

        /// <summary>
        /// Gets the default color of a note button.
        /// </summary>
        /// <param name="descriptionData">
        /// The data descriping the visualization data of the Song.
        /// </param>
        /// <returns>
        /// The Xna.Color the note button should have when not selected.
        /// </returns>
        private static Xna.Color GetDefaultButtonColor( SongDescriptionData descriptionData )
        {
            const byte DefaultAlpha = 200;

            var noteColor = descriptionData.NoteColor;
            return new Xna.Color( noteColor.R, noteColor.G, noteColor.B, DefaultAlpha );
        }

        /// <summary>
        /// Gets the selected color of a note button.
        /// </summary>
        /// <param name="descriptionData">
        /// The data descriping the visualization data of the Song.
        /// </param>
        /// <returns>
        /// The Xna.Color the note button should have when selected.
        /// </returns>
        private static Xna.Color GetSelectedButtonColor( SongDescriptionData descriptionData )
        {
            return descriptionData.NoteColor;
        }

        /// <summary>
        /// Called when this SongSelectionElement has been added to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            foreach( var button in this.noteButtons )
            {
                userInterface.AddElement( button );
            }
        }
        
        /// <summary>
        /// Called when this SongSelectionElement has been removed from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            foreach( var button in this.noteButtons )
            {
                userInterface.RemoveElement( button );
            }
        }

        /// <summary>
        /// Gets called when this SongSelectionElement gets enabled or disabled.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            bool isEnabled = this.IsEnabled;

            foreach( var button in this.noteButtons )
            {
                button.IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Gets called when this SongSelectionElement gets hidden or shown.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            bool isVisible = this.IsVisible;

            if( isVisible )
            {
                this.RefreshNoteButtons();
            }
            else
            {
                foreach( var button in this.noteButtons )
                {
                    button.IsVisible = false;
                }
            }
        }

        /// <summary>
        /// Called when the player clicks on one of the note buttons.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse last frame.</param>
        private void OnNoteButtonClicked( 
            object sender, 
            ref MouseState mouseState, 
            ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed && 
                oldMouseState.LeftButton == ButtonState.Released )
            {
                var noteButton = (Button)sender;
                this.HandleClick( noteButton );
            }
        }

        /// <summary>
        /// Handles the clicking of the given note button.
        /// </summary>
        /// <param name="noteButton">
        /// The button that should be toggled.
        /// </param>
        private void HandleClick( Button noteButton )
        {
            if( this.ShouldNotSelect( noteButton ) )
                return;

            this.Select( noteButton );
        }

        /// <summary>
        /// Selects the given note button; changing
        /// the currently selected Song.
        /// </summary>
        /// <param name="noteButton">
        /// The note button to select.
        /// </param>
        private void Select( Button noteButton )
        {
            if( this.selectedNoteButton != null )
                this.selectedNoteButton.IsSelected = false;

            this.selectedSong = GetSongRelatedTo( noteButton );
            this.selectedNoteButton = noteButton;

            if( this.selectedNoteButton != null )
                this.selectedNoteButton.IsSelected = true;

            this.OnSelectedSongChanged( this.selectedSong );
        }

        /// <summary>
        /// Gets a value indicating whether selection should
        /// change to the given note button.
        /// </summary>
        /// <param name="noteButton">
        /// The note button to select.
        /// </param>
        /// <returns>
        /// True if the given button should not selected;
        /// otherwise false.
        /// </returns>
        private bool ShouldNotSelect( Button noteButton )
        {
            if( this.selectedNoteButton == noteButton )
                return true;

            var song = GetSongRelatedTo( noteButton );
            if( song == null )
                return true;

            return false;
        }

        /// <summary>
        /// Fires the <see cref="SelectedSongChanged"/> event.
        /// </summary>
        /// <param name="song">
        /// The now selected song.
        /// </param>
        private void OnSelectedSongChanged( Song song )
        {
            if( this.SelectedSongChanged != null )
            {
                this.SelectedSongChanged( this, song );
            }
        }

        /// <summary>
        /// Gets the Song the given note button is related to.
        /// </summary>
        /// <param name="noteButton">
        /// The button.
        /// </param>
        /// <returns>
        /// The song related to the given note button.
        /// </returns>
        private static Song GetSongRelatedTo( Button noteButton )
        {
            if( noteButton == null )
                return null;

            return noteButton.Tag as Song;
        }

        /// <summary>
        /// Called when this SongSelectionElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
        }

        /// <summary>
        /// Called when this SongSelectionElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The index offset from the array of buttons to the list of songs the player knows.
        /// </summary>
        private int indexOffset = 0;

        /// <summary>
        /// The currently selected note button.
        /// </summary>
        private Button selectedNoteButton;

        /// <summary>
        /// The currently selected song.
        /// </summary>
        private Song selectedSong;

        /// <summary>
        /// The buttons that when clicked change the currently selected note.
        /// </summary>
        private readonly SpriteButton[] noteButtons = new SpriteButton[MaximumVisibleSongCount];

        #endregion
    }
}
