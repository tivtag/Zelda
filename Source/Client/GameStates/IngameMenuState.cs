// <copyright file="IngameOptionsScreenState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.IngameOptionsScreenState class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Collections;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.UI;

    /// <summary>
    /// The ingame options screen allows the player to change various ingame settings, save the current game
    /// and exit to the character selection state.
    /// </summary>
    internal sealed class IngameMenuState : IGameState, ISceneProvider
    {   
        /// <summary>
        /// Gets the ZeldaScene that is drawn in the background of this IngameMenuState.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.ingameState.Scene;
            }
        }

        /// <summary>
        /// Initializes a new instance of the IngameOptionsScreenState class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public IngameMenuState( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Loads this IngameOptionsScreenState.
        /// </summary>
        public void Load()
        {
            this.userInterface = new ZeldaUserInterface( providesDialog: false );
            this.userInterface.KeyboardInput += this.OnKeyboardInput;
            this.userInterface.Setup( this.serviceProvider );

            var viewSize = this.serviceProvider.ViewSize;
            var sl = this.serviceProvider.SpriteLoader;

            float buttonPositionX = (int)(viewSize.X / 2.0f - 40.0f);
            float centerY = (int)(viewSize.Y / 2.0f);
            const float HalfButtonHeight = 11.0f;
                 
            var saveButton = new SpriteButton( "Save", sl.LoadSprite( "Button_Save_Default" ), sl.LoadSprite( "Button_Save_Selected" ) ) {
                Position = new Vector2( buttonPositionX, centerY - HalfButtonHeight * 4 )
            };

            var settingsButton = new SpriteButton( "Settings", sl.LoadSprite( "Button_Settings_Default" ), sl.LoadSprite( "Button_Settings_Selected" ) )            {
                Position = new Vector2( buttonPositionX, centerY - HalfButtonHeight )
            };

            var exitButton = new SpriteButton( "Exit", sl.LoadSprite( "Button_Exit_Default" ), sl.LoadSprite( "Button_Exit_Selected" ) )
            {
                Position = new Vector2( buttonPositionX, centerY + HalfButtonHeight * 2 )
            };

            // Back Button
            var backButton = new NavButton( "BackButton", serviceProvider ) {
                Position = new Vector2( 3, serviceProvider.ViewSize.Y - 23 ),
                ButtonMode = NavButton.Mode.Back
            };

            backButton.Clicked += OnBackButtonClicked;
            userInterface.AddElement( backButton );

            exitButton.Clicked += this.OnExitButtonClicked;
            exitButton.MouseEntering += this.OnMouseEnteringButton;
            saveButton.Clicked += this.OnSaveButtonClicked;
            saveButton.MouseEntering += this.OnMouseEnteringButton;
            settingsButton.Clicked += this.OnSettingsButtonClicked;
            settingsButton.MouseEntering += this.OnMouseEnteringButton;
            
            this.AddButton( saveButton );
            this.AddButton( settingsButton );
            this.AddButton( exitButton );
            this.SelectButton( saveButton );
        }

        private void OnMouseEnteringButton( Atom.Xna.UI.UIElement sender )
        {
            this.SelectButton( (Button)sender );
        }
        
        /// <summary>
        /// Executes the Click action of the currently selected button.
        /// </summary>
        private void ExecuteSelectedButton()
        {
            Button button = this.GetSelectedButton();
            button.Click();
        }

        /// <summary>
        /// Gets the Button that has been selected by the player.
        /// </summary>
        /// <returns>
        /// The Button that has been selected.
        /// </returns>
        private Button GetSelectedButton()
        {
            return this.buttons.First( button => button.IsSelected );
        }

        /// <summary>
        /// Moves the selected button down or up by the specified amount.
        /// </summary>
        /// <param name="amount">
        /// The amount to move in index space.
        /// </param>
        private void MoveSelectedButtonIndexBy( int amount )
        {
            int index = this.buttons.IndexOf( button => button.IsSelected );

            if( index != -1 )
            {
                if( index == 0 && amount == -1 )
                {
                    index = this.buttons.Count - 1;
                }
                else
                {
                    index = Math.Abs( (index + amount) % this.buttons.Count );
                }

                this.SelectButtonAt( index );
            }
        }

        /// <summary>
        /// Selects the Button at the specified zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the button to select.
        /// </param>
        private void SelectButtonAt( int index )
        {
            this.SelectButton( this.buttons[index] );
        }

        /// <summary>
        /// Selects the specified button.
        /// </summary>
        /// <param name="button">
        /// The button to select.
        /// </param>
        private void SelectButton( Button button )
        {
            this.buttons.ForEach( b => b.IsSelected = false );
            button.IsSelected = true;
        }

        /// <summary>
        /// Adds the specified Button to the list of buttons that allow the player
        /// to excute actions in this IngameOptiosnScreenState.
        /// </summary>
        /// <param name="button">
        /// The button to add.
        /// </param>
        private void AddButton( Button button )
        {
            this.buttons.Add( button );
            this.userInterface.AddElement( button );
        }

        /// <summary>
        /// Called when the Exit Button has been clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnExitButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.ingameState.ExitToCharacterSelectionState();
        }

        private void OnBackButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {   
            this.ChangeToIngameState();

            // Fix picking up of the action slot after closing it
            this.ingameState.UserInterface.MouseState = this.ingameState.UserInterface.OldMouseState = mouseState;
        }

        /// <summary>
        /// Called when the Save Button has been clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnSaveButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.ingameState.Save();
            this.ChangeToIngameState();
        }

        /// <summary>
        /// Called when the Settings Button has been clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnSettingsButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            var gameStateManager = this.serviceProvider.GetService<GameStateManager>();
            gameStateManager.Push<SettingsState>();
        }

        /// <summary>
        /// Unloads this IngameOptionsScreenState.
        /// </summary>
        public void Unload()
        {
            this.buttons.Clear();
            this.userInterface = null;
        }

        /// <summary>
        /// Returns to the IngameState.
        /// </summary>
        private void ChangeToIngameState()
        {
            var gameStateManager = this.serviceProvider.GetService<GameStateManager>();
            gameStateManager.Pop();
        }

        /// <summary>
        /// Draws this IngameOptionsScreenState.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( IDrawContext drawContext )
        {
            var pipeline = this.ingameState.Game.Graphics.Pipeline;
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            pipeline.InitializeFrame( this.ingameState.Scene, this.userInterface, zeldaDrawContext );
            {
                pipeline.BeginScene();
                {
                    var batch = zeldaDrawContext.Batch;
                    zeldaDrawContext.Begin();
                    {
                        batch.DrawRect(
                            new Microsoft.Xna.Framework.Rectangle(
                                0,
                                0,
                                this.serviceProvider.ViewSize.X,
                                this.serviceProvider.ViewSize.Y
                            ),
                            Microsoft.Xna.Framework.Color.Black.WithAlpha( 200 )
                        );
                    }
                    zeldaDrawContext.End();
                }
                pipeline.EndScene();
                pipeline.BeginUserInterface();
                pipeline.EndUserInterface();
            }
        }

        /// <summary>
        /// Updates this IngameOptionsScreenState.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( IUpdateContext updateContext )
        {
            if( this.timeNotCloseable > 0.0f )
            {
                this.timeNotCloseable -= updateContext.FrameTime;
            }

            this.userInterface.Update( updateContext );
        }

        /// <summary>
        /// Gets called when the current GameState changed to this GameState.
        /// </summary>
        /// <param name="oldState">
        /// The old state.
        /// </param>
        public void ChangedFrom( IGameState oldState )
        {
            var ingameState = oldState as IngameState;

            if( ingameState == null )
            {
                if( !(oldState is SettingsState) )
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                this.ingameState = ingameState;
                this.ingameState.UserInterface.RestoreForSaveOrExit();
            }

            this.timeNotCloseable = TimeNotCloseable;
            this.Load();

            this.userInterface.RefreshMouseInputState();
            this.userInterface.ProcessMouseInput();
        }

        /// <summary>
        /// Gets called when the current GameState changed away from this GameState.
        /// </summary>
        /// <param name="newState">
        /// The bew state.
        /// </param>
        public void ChangedTo( IGameState newState )
        {
            this.Unload();
        }

        /// <summary>
        /// Called every frame to handle input.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="keyState">
        /// The current state of the Keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the Keyboard one frame ago.
        /// </param>
        private void OnKeyboardInput( object sender, ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            var pressedKeys = keyState.GetPressedKeys();

            foreach( var key in pressedKeys )
            {
                switch( key )
                {
                    case Keys.Enter:
                        if( oldKeyState.IsKeyUp( Keys.Enter ) )
                        {
                            this.ExecuteSelectedButton();
                        }
                        break;

                    case Keys.Escape:
                        if( oldKeyState.IsKeyUp( Keys.Escape ) )
                        {
                            if( this.timeNotCloseable <= 0.0f )
                            {
                                this.ChangeToIngameState();
                            }
                        }
                        break;

                    case Keys.W:
                    case Keys.Up:
                        if( oldKeyState.IsKeyUp( key ) )
                        {
                            this.MoveSelectedButtonIndexBy( -1 );
                        }
                        break;

                    case Keys.S:
                    case Keys.Down:
                        if( oldKeyState.IsKeyUp( key ) )
                        {
                            this.MoveSelectedButtonIndexBy( 1 );
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// The time left this IngameOptionsScreenState can't be closed for in seconds.
        /// </summary>
        private float timeNotCloseable;

        /// <summary>
        /// The time this IngameOptionsScreenState can't be closed for in seconds.
        /// </summary>
        private const float TimeNotCloseable = 0.25f;

        /// <summary>
        /// The current IngameState.
        /// </summary>
        private IngameState ingameState;

        /// <summary>
        /// Represents the user interface shown in the IngameOptionsScreenState.
        /// </summary>
        private ZeldaUserInterface userInterface;

        /// <summary>
        /// The buttons that allow the user to execute an action in this IngameOptionsScreenState.
        /// </summary>
        private readonly List<Button> buttons = new List<Button>();

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
