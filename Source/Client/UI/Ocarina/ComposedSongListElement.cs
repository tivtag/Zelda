// <copyright file="ComposedSongListElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.ComposedSongListElement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Ocarina
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Zelda.Ocarina;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Visualizes the list of Ocarina Songs the Player knows.
    /// </summary>
    internal sealed class ComposedSongListElement : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the ComposedSongListElement class.
        /// </summary>
        /// <param name="noteVisualizer">
        /// Provides a mechanism to draw song notes.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        public ComposedSongListElement( NoteVisualizer noteVisualizer, IZeldaServiceProvider serviceProvider )
        {
            this.Size = new Vector2( serviceProvider.ViewSize.X, 80.0f );
            this.FloorNumber = 3;

            this.selectionElement = new SongSelectionElement( serviceProvider );
            this.selectionElement.SelectedSongChanged += this.OnSelectedSongChanged;

            this.descriptionElement = new SongDescriptionElement( noteVisualizer );
            this.HideAndDisable();
        }
        
        /// <summary>
        /// Resets this ComposedSongListElement. 
        /// </summary>
        public void Reset()
        {
            this.selectionElement.DeselectSong();
        }

        /// <summary>
        /// Called when this OcarinaSongListDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Called when this OcarinaSongListDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var camera = zeldaDrawContext.Camera;
            var batch  = zeldaDrawContext.Batch;

            // Draw Title Header
            batch.DrawRect(
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    camera.ViewSize.X,
                    20
                ),
                new Xna.Color( 0, 0, 0, 200 )
            );

            // Draw Title String
            UIFonts.TahomaBold11.Draw(
                "Ocarina",
                new Vector2( this.Width / 2.0f, 0.0f ),
                Atom.Xna.Fonts.TextAlign.Center,
                Microsoft.Xna.Framework.Color.White,
                0.002f,
                drawContext
            );
        }
        
        /// <summary>
        /// Gets called when the UIElement was added to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            userInterface.AddElement( this.selectionElement );
            userInterface.AddElement( this.descriptionElement );
        }

        /// <summary>
        /// Gets called when the UIElement was removed from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            userInterface.RemoveElement( this.selectionElement );
            userInterface.RemoveElement( this.descriptionElement );
        }
        
        /// <summary>
        /// Called when the visability state of this OcarinaSongListDisplay has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            this.Toggle( this.IsVisible );
        }

        /// <summary>
        /// Toggles the internal song list display on or off.
        /// </summary>
        /// <param name="on">
        /// States whether the list should be toggled on; or off.
        /// </param>
        public void Toggle( bool on )
        {
            if( on )
            {
                this.selectionElement.ShowEnableAndRefresh();
                this.selectionElement.SelectSongIfNoneSelected();

                this.descriptionElement.ShowAndEnable();
            }
            else
            {
                this.selectionElement.HideAndDisable();
                this.descriptionElement.HideAndDisable();
            }
        }
        
        /// <summary>
        /// Called when the currently selected Song has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="song">
        /// The now selected Song.
        /// </param>
        private void OnSelectedSongChanged( object sender, Song song )
        {
            this.descriptionElement.ShowSong( song );
        }

        /// <summary>
        /// The UIElement that exposes the list of Songs the user might select.
        /// </summary>
        private readonly SongSelectionElement selectionElement;

        /// <summary>
        /// The UIElement that shows information about the currently selected Song.
        /// </summary>
        private readonly SongDescriptionElement descriptionElement;
    }
}
