// <copyright file="Dialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Dialog class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents a simple one-way dialog.
    /// </summary>
    public class Dialog : ZeldaUIElement, IZeldaSetupable
    {
        /// <summary>
        /// Fired when this dialog has ended.
        /// </summary>
        public event EventHandler Ended;

        /// <summary>
        /// Fired when this dialog has been canceled.
        /// </summary>
        public event EventHandler Canceled;

        /// <summary>
        /// Gets the <see cref="Text"/> storage object
        /// that manages the text displayed in this <see cref="Dialog"/>.
        /// </summary>
        public AnimatedTypeWriterText Text
        {
            get
            {
                return (AnimatedTypeWriterText)this.textField.Text;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Dialog class.
        /// </summary>
        public Dialog()
        {
            this.FloorNumber = 10;
            this.textField.HideAndDisable();
            this.HideAndDisable();
        }

        /// <summary>
        /// Setups this Dialog.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;

            const int Width = 218;
            var font = UIFonts.TahomaBold10;

            // Setup Text
            var text = new AnimatedTypeWriterText(
                font,
                TextAlign.Left,
                Microsoft.Xna.Framework.Color.Black,
                new TextBlockSplitter( font, Width )
            );

            text.LinesShown = 3;
            text.TextBlockWidth = Width;
            text.TimeSpendPerCharacter = 0.065f;
            text.LayerDepth = 0.5f;

            // Setup Text Field
            var spriteLoader = serviceProvider.SpriteLoader;
            textField.Sprite = spriteLoader.LoadSprite( "Dialog_Box" );
            textField.SpriteReachedEnd = spriteLoader.LoadSprite( "Dialog_NextPageArrow" );
            textField.SpriteReachedBlockEnd = spriteLoader.LoadSprite( "Dialog_PressSpace" );

            textField.Text = text;
            textField.FloorNumber = this.FloorNumber = 5;

            const float BorderSizeX = 19.0f, BorderSizeY = 15.0f;
            var viewSize = serviceProvider.ViewSize;

            textField.Position = new Vector2(
                (int)((viewSize.X / 2.0f) - (textField.Sprite.Width / 2) + BorderSizeX),
                (int)((viewSize.Y / 2.0f) - (textField.Sprite.Height / 2) + BorderSizeY)
            );

            textField.SpriteOffset = new Vector2( -BorderSizeX, -BorderSizeY + 1 );
            DisableDialog();
        }

        /// <summary>
        /// Enables this <see cref="Dialog"/> to show the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">
        /// The text to show.
        /// </param>
        public void Show( string text )
        {
            this.Text.TextString = text;
            this.Text.Reset();

            this.IsEnabled = textField.IsEnabled = true;
            this.IsVisible = textField.IsVisible = true;
            this.isFirstTrigger = true;

            if( this.Owner != null )
            {
                this.Owner.FocusedElement = this;

                if( this.Owner.Scene != null )
                {
                    this.Owner.Scene.IsPaused = true;
                }
            }
        }

        /// <summary>
        /// Disables this <see cref="Dialog"/>.
        /// </summary>
        private void DisableDialog()
        {
            this.IsEnabled = textField.IsEnabled = false;
            this.IsVisible = textField.IsVisible = false;

            if( this.Owner != null )
            {
                if( this.Owner.FocusedElement == this )
                    this.Owner.FocusedElement = null;

                if( this.Owner.Scene != null )
                    this.Owner.Scene.IsPaused = false;
            }
        }

        /// <summary>
        /// Handles keyboard input related to this Dialog UIElement.
        /// </summary>
        /// <param name="keyState">The current state of the keyboard.</param>
        /// <param name="oldKeyState">The state of the keyboard one frame ago.</param>
        protected override void HandleKeyInput( ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            Zelda.Entities.PlayerEntity player = this.Owner.Scene.Player;
            KeySettings keySettings = player.Profile.KeySettings;

            if( (keyState.IsKeyDown( Keys.Space ) && oldKeyState.IsKeyUp( Keys.Space )) )
            {
                this.TriggerNextDialogState();
                isFirstTrigger = false;
            }
            else if( keyState.IsKeyDown( keySettings.UsePickup ) && oldKeyState.IsKeyUp( keySettings.UsePickup ) )
            {
                if( !isFirstTrigger )
                {
                    this.TriggerNextDialogState();
                }

                isFirstTrigger = false;
            }
            else if( keyState.IsKeyDown( Keys.Escape ) && oldKeyState.IsKeyUp( Keys.Escape ) )
            {
                this.HandleInputEscapeKey();
            }
        }

        /// <summary>
        /// Handles the case of the uses pressing the Space key 
        /// while this Dialog element is focused.
        /// </summary>
        private void TriggerNextDialogState()
        {
            var text = textField.Text as AnimatedTypeWriterText;
            if( text == null )
                return;

            if( text.HasReachedEnd )
            {
                this.PlaySample( "TextEnd.ogg" );
                this.DisableDialog();

                this.Ended.Raise( this );
            }
            else if( text.HasReachedEndOfBlock )
            {
                text.JumpBlock();
                this.PlaySample( "TextNext.ogg" );
            }
            else
            {
                text.JumpToEndOfBlock();
            }
        }

        /// <summary>
        /// Handles the case of the uses pressing the Escape key 
        /// while this Dialog element is focused.
        /// </summary>
        private void HandleInputEscapeKey()
        {
            this.DisableDialog();
            this.Canceled.Raise( this );
        }

        /// <summary>
        /// Helper method that plays the audio-sample with the given name.
        /// </summary>
        /// <param name="name">
        /// The name of the sample to play.
        /// </param>
        private void PlaySample( string name )
        {
            var sample = serviceProvider.AudioSystem.GetSample( name );

            if( sample != null )
            {
                sample.LoadAsSample( false );
                sample.Play();
            }
        }

        /// <summary>
        /// Called when this Dialog UIElement has been added to the given <see cref="UserInterface"/>.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            userInterface.AddElement( this.textField );
        }

        /// <summary>
        /// Called when this Dialog UIElement has been removed from the given <see cref="UserInterface"/>.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            userInterface.RemoveElement( this.textField );

            if( this.IsEnabled )
            {
                ZeldaScene scene = ((ZeldaUserInterface)userInterface).Scene;
                scene.IsPaused = false;
            }
        }

        /// <summary>
        /// Called when this Dialog is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            // Drawing is done by the Text Field
        }

        /// <summary>
        /// Called when this Dialog is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        private bool isFirstTrigger;

        /// <summary>
        /// The TextField that manages all rendering logic.
        /// </summary>
        private readonly DialogTextField textField = new DialogTextField();

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }
}
