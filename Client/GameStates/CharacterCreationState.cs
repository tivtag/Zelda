// <copyright file="CharacterCreationState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.CharacterCreationState class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.GameStates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Collections;
    using Atom.Fmod;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Difficulties;
    using Zelda.Entities.Drawing;
    using Zelda.Profiles;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines the IGameState in which the user can create a new GameProfile.
    /// </summary>
    internal sealed class CharacterCreationState : BaseOutgameState, IGameState
    {
        /// <summary>
        /// Gets the <see cref="GameProfile"/> that the player has created
        /// using this CharacterCreationState.
        /// </summary>
        public GameProfile Profile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the entered profileName is unique.
        /// </summary>
        /// <returns>
        /// true if the entered name is unique;
        /// otherwise false.
        /// </returns>
        private bool IsUniqueProfileName
        {
            get
            {
                foreach( IGameProfile profile in this.selectionState.Profiles )
                {
                    if( string.Equals( profile.Name, this.Profile.Name, StringComparison.OrdinalIgnoreCase ) )
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CharacterCreationState class.
        /// </summary>
        /// <param name="game">
        /// The game that owns the new CharacterCreationState.
        /// </param>
        public CharacterCreationState( ZeldaGame game )
            : base( game )
        {
            this.game = game;
        }

        /// <summary>
        /// Loads this CharacterCreationState.
        /// </summary>
        public override void Load()
        {
            this.linkSprites = game.GetService<LinkSprites>();
            this.LoadUserInterface();
        }

        /// <summary>
        /// Unloads this CharacterCreationState.
        /// </summary>
        public override void Unload()
        {
            this.sampleChannel = null;
            this.difficultyButtons.Clear();

            base.Unload();
        }

        /// <summary>
        /// Overriden to setup the UserInterface of this CharacterCreationState.
        /// </summary>
        protected override void SetupUserInterface()
        {
            // Difficulty
            this.difficultyButtons.Clear();

            foreach( DifficultyId difficulty in GetAvailableDifficulties() )
            {
                var newButton = new LambdaButton( ( button, updateContext ) => { }, this.DrawDifficultyButton ) {
                    Tag = difficulty
                };
                newButton.Clicked += this.OnDifficultyButtonClicked;

                this.difficultyButtons.Add( newButton );
                this.UserInterface.AddElement( newButton );
            }

            this.LayoutButtons();

            this.checkBoxHardcore = new UI.CheckBox() {
                Position = new Vector2( game.ViewSize.X / 2 + 44.0f, game.ViewSize.Y - 30.0f )
            };
            this.checkBoxHardcore.HideAndDisable();
            this.UserInterface.AddElement( checkBoxHardcore );

            // Stats
            this.characterWindow = new CharacterCreationStatusWindow( this.game );
            this.UserInterface.AddElement( characterWindow );

            // Color
            this.colorClothSelector = new ColorSelector( new Vector2( game.ViewSize.X / 2.0f - 50.0f, 81.0f ), "Cloth Main" );
            this.UserInterface.AddElement( colorClothSelector );
            colorClothSelector.ColorChanged += OnColorSelectorColorChanged;

            this.colorClothHighlightSelector = new ColorSelector( new Vector2( game.ViewSize.X / 2.0f + 50.0f, 81.0f ), "Cloth Highlight" );
            this.UserInterface.AddElement( colorClothHighlightSelector );
            colorClothHighlightSelector.ColorChanged += OnColorSelectorColorChanged;

            this.colorHairSelector = new ColorSelector( new Vector2( game.ViewSize.X / 2.0f - 50.0f, 154.0f ), "Hair Main" );
            this.UserInterface.AddElement( colorHairSelector );
            colorHairSelector.ColorChanged += OnColorSelectorColorChanged;

            this.colorHairHighlightSelector = new ColorSelector( new Vector2( game.ViewSize.X / 2.0f + 50.0f, 154.0f ), "Hair Highlight" );
            this.UserInterface.AddElement( colorHairHighlightSelector );
            colorHairHighlightSelector.ColorChanged += OnColorSelectorColorChanged;

            // Buttons
            this.backButton = new NavButton( "BackButton", game ) {
                Position = new Vector2( 3, game.ViewSize.Y - 23 ),
                ButtonMode = NavButton.Mode.Back
            };
            this.nextButton = new NavButton( "NextButton", game ) {
                Position = new Vector2( game.ViewSize.X - 23, game.ViewSize.Y - 23 ),
                ButtonMode = NavButton.Mode.Next
            };

            this.UserInterface.AddElement( backButton );
            this.UserInterface.AddElement( nextButton );
            backButton.Clicked += OnBackButtonClicked;
            nextButton.Clicked += OnNextButtonClicked;

            DisableStep( CreationStep.SelectLooks );
            DisableStep( CreationStep.SelectDifficulty );
        }

        private void OnColorSelectorColorChanged( object sender, EventArgs e )
        {
            linkSprites.SetColorTint( new LinkSpriteColorTint( colorClothSelector.Color, colorClothHighlightSelector.Color, colorHairSelector.Color, colorHairHighlightSelector.Color ) );
        }

        private void OnBackButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            ProceedToPreviousStep();
        }

        private void OnNextButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            ProceedToNextStep();
        }

        /// <summary>
        /// Gets the currently available game difficulties.
        /// </summary>
        /// <returns>
        /// The available difficulties.
        /// </returns>
        private IEnumerable<DifficultyId> GetAvailableDifficulties()
        {
            yield return DifficultyId.Easy;
            yield return DifficultyId.Normal;
            yield return DifficultyId.Nightmare;
            yield return DifficultyId.Hell;

            if( this.selectionState.Profiles.Any( profile => profile.Difficulty == DifficultyId.Hell && profile.CharacterLevel >= InsaneDifficulty.MinimumHellLevel ) )
            {
                yield return DifficultyId.Insane;
            }
        }

        /// <summary>
        /// Layouts the difficulty selection buttons.
        /// </summary>
        private void LayoutButtons()
        {
            const float ButtonGap = 5.0f;
            Vector2 ButtonSize = new Vector2( 120.0f, 20.0f );

            float totalHeight = this.difficultyButtons.Count * (ButtonSize.Y + ButtonGap);

            Vector2 position = new Vector2(
                (int)(this.game.ViewSize.X / 2.0f - ButtonSize.X / 2.0f),
                (int)(this.game.ViewSize.Y / 2.0f - totalHeight / 2.0f)
            );

            for( int index = 0; index < this.difficultyButtons.Count; ++index )
            {
                Button button = this.difficultyButtons[index];
                button.SetTransform(
                    position: position,
                    offset: Vector2.Zero,
                    size: ButtonSize
                );

                position.Y += ButtonSize.Y + ButtonGap;
            }

            this.SelectDifficultyAt( 1 );
        }

        /// <summary>
        /// Called when the user has clicked on any of the difficulty buttons.
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
        private void OnDifficultyButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.SelectDifficulty( (Button)sender );
        }

        /// <summary>
        /// Gets called when the focus has changed from the given IGameState to this IGameState.
        /// </summary>
        /// <param name="oldState">
        /// The old IGameState.
        /// </param>
        public override void ChangedFrom( IGameState oldState )
        {
            this.selectionState = oldState as CharacterSelectionState;
            if( this.selectionState == null )
                throw new InvalidOperationException();

            StealParticleEffect( selectionState );
            Load();
            ResetEnteredValues();

            ChangeStep( CreationStep.EnterName );
            base.ChangedFrom( oldState );
        }

        private void ResetEnteredValues()
        {
            this.Profile = GameProfile.StartNewAdventure(
                game.GlobalKeySettings,
                game
            );

            this.characterWindow.Player = this.Profile.Player;

            this.spriteDirection = Direction4.Down;
            this.linkSprites.ResetTint();
            this.colorClothSelector.Color = LinkSprites.ColorDefaults.ClothMain;
            this.colorClothHighlightSelector.Color = LinkSprites.ColorDefaults.ClothHighlight;
            this.colorHairSelector.Color = LinkSprites.ColorDefaults.HairMain;
            this.colorHairHighlightSelector.Color = LinkSprites.ColorDefaults.HairHighlight;
        }

        /// <summary>
        /// Gets called when the focus has changed away from this IGameState to the given IGameState.
        /// </summary>
        /// <param name="newState">
        /// The new IGameState.
        /// </param>
        public override void ChangedTo( IGameState newState )
        {
            if( !(newState is CharacterSelectionState) )
                throw new InvalidOperationException();

            this.Unload();
            base.ChangedTo( newState );
        }

        /// <summary>
        /// Draws the UserInterface of this CharacterSelectionState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void DrawUserInterface( ISpriteDrawContext drawContext )
        {
            switch( currentStep )
            {
                case CreationStep.EnterName:
                    this.DrawEnterNameStep( drawContext );
                    break;

                case CreationStep.SelectLooks:
                    this.DrawLooksStep( drawContext );
                    break;

                case CreationStep.SelectStats:
                    this.DrawStatStep( drawContext );
                    break;

                case CreationStep.SelectDifficulty:
                    this.DrawDifficultyStep( drawContext );
                    break;

                default:
                    break;
            }
        }

        private void DrawLooksStep( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            drawContext.Begin();
            {
                DrawTitleHeader( "Customize Looks", drawContext );
                
                ISprite sprite = linkSprites.GetMove( spriteDirection );

                int drawOffsetX = 0;
                if( spriteDirection == Direction4.Right )
                {
                    drawOffsetX = -1;
                }

                sprite.Draw( new Vector2( (game.ViewSize.X / 2.0f) - (sprite.Width / 2) + drawOffsetX, 42 ), batch );
            }
            drawContext.End();
            UserInterface.Draw( drawContext );
        }

        private void DrawStatStep( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            drawContext.Begin();
            {
                DrawTitleHeader( "Invest Status Points", drawContext );
            }
            drawContext.End();
            UserInterface.Draw( drawContext );
        }

        private void DrawDifficultyStep( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            drawContext.Begin();
            {
                DrawTitleHeader( "Select Difficulty", drawContext );

                // Draw Hardcore String
                UIFonts.TahomaBold11.Draw(
                    "Hardcore:",
                    new Vector2( game.ViewSize.X / 2 - 25.0f, game.ViewSize.Y - 30.0f ),
                    TextAlign.Center,
                    Microsoft.Xna.Framework.Color.White,
                    0.002f,
                    drawContext
                );
            }
            batch.End();
        }

        private void DrawTitleHeader( string title, ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;

            // Draw Title Background
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, game.ViewSize.X, 20 ),
                UIColors.LightWindowBackground,
                0.001f
            );

            // Draw Title String
            UIFonts.TahomaBold11.Draw(
                title,
                new Vector2( game.ViewSize.X / 2, 0.0f ),
                TextAlign.Center,
                new Microsoft.Xna.Framework.Color( 0, 0, 0, 155 ),
                0.002f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the specified LambdaButton.
        /// </summary>
        /// <param name="button">
        /// The LambdaButton to draw.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawDifficultyButton( LambdaButton button, ISpriteDrawContext drawContext )
        {
            if( !button.IsVisible )
                return;

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var batch = drawContext.Batch;

            batch.DrawRect(
                (Rectangle)button.ClientArea,
                new Xna.Color( 255, 255, 255, 100 ),
                0.0f
            );

            if( button.IsSelected )
            {
                batch.DrawLineRect(
                    (Rectangle)button.ClientArea,
                    Xna.Color.White,
                    1,
                    0.5f
                );
            }

            this.Font.Draw(
                LocalizedEnums.Get( (DifficultyId)button.Tag ),
                button.Position + new Vector2( button.Size.X / 2.0f, 2.0f ),
                Atom.Xna.Fonts.TextAlign.Center,
                button.IsSelected ? Xna.Color.Red : Xna.Color.White,
                1.0f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the "Enter Name" UserInterface of this CharacterSelectionState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawEnterNameStep( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            drawContext.Begin();
            {
                int centerX = game.ViewSize.X / 2;
                DrawTitleHeader( Resources.Text_EnterNameOfHero, drawContext );

                bool isBlinking = blinkTimer < 0.5f;
                string name = isBlinking ? this.Profile.Name + "_" : this.Profile.Name;
                var strNameSize = this.FontLarge.MeasureString( this.Profile.Name );
                var strBlinkNameSize = this.FontLarge.MeasureString( name );

                // Draw black rectangle behind name:
                batch.DrawRect(
                    new Rectangle( centerX - (int)(strNameSize.X / 2) - 2, 110, (int)strBlinkNameSize.X + 4, (int)strNameSize.Y - 2 ),
                    new Xna.Color( 0, 0, 0, 180 )
                );

                // Draw the name the player has entered
                bool uniqueName = this.IsUniqueProfileName;

                this.FontLarge.Draw(
                    name,
                    new Vector2( centerX - (int)(strNameSize.X / 2), 110.0f ),
                    uniqueName ? Xna.Color.White : Xna.Color.Red,
                    drawContext
                );

                if( !uniqueName )
                {
                    var smallFont = Zelda.UI.UIFonts.Tahoma9;
                    const string TextNameInUse = "name already in use :(";

                    smallFont.Draw(
                        TextNameInUse,
                        new Vector2( centerX - (int)(smallFont.MeasureStringWidth( TextNameInUse ) / 2), 135.0f ),
                        Xna.Color.White,
                        drawContext
                    );
                }
            }
            drawContext.End();
        }

        protected override void Update( ZeldaUpdateContext updateContext )
        {
            switch( this.currentStep )
            {
                case CreationStep.EnterName:
                    blinkTimer += updateContext.FrameTime;
            
                    if( blinkTimer >= 2.0f )
                    {
                        blinkTimer = 0.0f;
                    }
                    break;

                case CreationStep.SelectLooks:
                    SelectLooksStep( updateContext );
                    break;
                default:
                    break;
            }

            base.Update( updateContext );
        }

        private void SelectLooksStep( ZeldaUpdateContext updateContext )
        {
            spriteRotationTimer += updateContext.FrameTime;

            if( spriteRotationTimer >= 1.0f )
            {
                switch( spriteDirection )
                {
                    case Direction4.Down:
                        spriteDirection = Direction4.Left;
                        break;

                    case Direction4.Left:
                        spriteDirection = Direction4.Up;
                        break;

                    case Direction4.Up:
                        spriteDirection = Direction4.Right;
                        break;

                    default:
                    case Direction4.Right:
                        spriteDirection = Direction4.Down;
                        break;
                }

                spriteRotationTimer = 0.0f;
            }

            linkSprites
                .GetMove( spriteDirection )
                .Animate( updateContext.FrameTime );
        }

        /// <summary>
        /// Handles keyboard input; called once every frame.
        /// </summary>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        protected override void OnKeyboardInput( ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            Keys[] pressedKeys = keyState.GetPressedKeys();

            for( int i = 0; i < pressedKeys.Length; ++i )
            {
                Keys key = pressedKeys[i];
                if( oldKeyState.IsKeyDown( key ) )
                    continue;

                switch( key )
                {
                    case Keys.Space:
                    case Keys.Enter:
                        if( oldKeyState.IsKeyUp( Keys.Enter ) )
                        {
                            this.ProceedToNextStep();
                            return;
                        }
                        break;
                    default:
                        break;
                }

                this.HandleKey( key );
            }
        }

        /// <summary>
        /// Handles pressing of the specified Key.
        /// </summary>
        /// <param name="key">
        /// The key that has been prressed.
        /// </param>
        private void HandleKey( Keys key )
        {
            if( this.currentStep == CreationStep.EnterName )
            {
                if( (int)key >= (int)Keys.A && (int)key <= (int)Keys.Z )
                {
                    if( this.Profile.Name.Length != MaximumAllowedNameLength )
                    {
                        string ch = key.ToString();

                        if( !this.UserInterface.IsShiftDown )
                        {
                            ch = ch.ToLower();
                        }

                        this.Profile.Name += ch;
                    }
                }

                else if( key == Keys.Delete || key == Keys.Back )
                {
                    if( this.Profile.Name.Length > 0 )
                    {
                        this.Profile.Name = this.Profile.Name.Substring( 0, this.Profile.Name.Length - 1 );
                    }
                }

                RefreshNavigationButtonStates();
            }
            else if( this.currentStep == CreationStep.SelectDifficulty )
            {
                if( key == Keys.Up || key == Keys.W )
                {
                    this.MoveSelectedDifficultyUp();
                }
                else if( key == Keys.Down || key == Keys.S )
                {
                    this.MoveSelectedDifficultyDown();
                }
            }
        }

        /// <summary>
        /// Moves the currently selected Difficulty by the specified number of indices.
        /// </summary>
        /// <param name="by">
        /// The value to move for.
        /// </param>
        private void MoveSelectedDifficulty( int by )
        {
            int indexOfCurrent = this.difficultyButtons.IndexOf( button => (DifficultyId)button.Tag == this.Profile.Difficulty );
            if( indexOfCurrent == -1 )
                return;

            indexOfCurrent += by;

            if( indexOfCurrent >= 0 && indexOfCurrent < this.difficultyButtons.Count )
            {
                this.SelectDifficultyAt( indexOfCurrent );
            }
        }

        /// <summary>
        /// Moves the currently selected Difficulty up by one index.
        /// </summary>
        private void MoveSelectedDifficultyUp()
        {
            this.MoveSelectedDifficulty( by: -1 );
        }

        /// <summary>
        /// Moves the currently selected Difficulty up by down by one index.
        /// </summary>
        private void MoveSelectedDifficultyDown()
        {
            this.MoveSelectedDifficulty( by: 1 );
        }

        /// <summary>
        /// Selects the difficulty at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the difficultyButton to select.
        /// </param>
        private void SelectDifficultyAt( int index )
        {
            Button button = this.difficultyButtons[index];
            this.SelectDifficulty( button );
        }

        /// <summary>
        /// Selects the difficulty of the specified Button.
        /// </summary>
        /// <param name="button">
        /// The button that contain the difficulty data in its tag.
        /// </param>
        private void SelectDifficulty( Button button )
        {
            var difficulty = (DifficultyId)button.Tag;
            foreach( var btn in this.difficultyButtons )
            {
                btn.IsSelected = false;
            }
            button.IsSelected = true;

            if( this.Profile == null || difficulty == this.Profile.Difficulty )
                return;

            this.Profile.Difficulty = difficulty;
            this.PlayDifficultySound();
        }

        /// <summary>
        /// Plays the sound that matches the currently selectedDifficulty.
        /// </summary>
        private void PlayDifficultySound()
        {
            Atom.Fmod.Sound sample = null;
            float volume = 1.0f;

            switch( this.Profile.Difficulty )
            {
                case DifficultyId.Hell:
                case DifficultyId.Insane:
                    sample = this.game.AudioSystem.GetSample( "Butcher.ogg" );
                    volume = 0.70f;
                    break;
            }

            if( sample != null )
            {
                sample.LoadAsSample( false );

                if( this.sampleChannel != null && this.sampleChannel.IsPlaying )
                {
                    this.sampleChannel.Stop();
                }

                this.sampleChannel = sample.Play( true );
                this.sampleChannel.Volume = volume;
                this.sampleChannel.Unpause();
            }
        }

        private void RefreshNavigationButtonStates()
        {
            bool nextButtonValid = true;
            var mode = NavButton.Mode.Next;
            
            if( this.currentStep == CreationStep.EnterName )
            {
                nextButtonValid = this.IsNameValid();
            }
            else if( this.currentStep == CreationStep.SelectDifficulty )
            {
                mode = NavButton.Mode.Complete;
            }

            nextButton.ButtonMode = mode;
            nextButton.SetUseable( nextButtonValid );
        }

        /// <summary>
        /// Attempts to proceed to the next step in the creation process.
        /// </summary>
        private void ProceedToNextStep()
        {
            switch( this.currentStep )
            {
                case CreationStep.EnterName:
                    if( this.IsNameValid() )
                    {
                        this.ChangeStep( CreationStep.SelectLooks );
                    }
                    break;

                case CreationStep.SelectLooks:
                    this.ChangeStep( CreationStep.SelectStats );
                    break;

                case CreationStep.SelectStats:
                    this.ChangeStep( CreationStep.SelectDifficulty );
                    break;

                case CreationStep.SelectDifficulty:
                    this.CreateCharacter();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Attempts to proceed to the next step in the creation process.
        /// </summary>
        private void ProceedToPreviousStep()
        {
            switch( this.currentStep )
            {
                case CreationStep.EnterName:
                    this.LeaveToPreviousState();
                    break;

                case CreationStep.SelectLooks:
                    this.ChangeStep( CreationStep.EnterName );
                    break;

                case CreationStep.SelectStats:
                    this.ChangeStep( CreationStep.SelectLooks );
                    break;

                case CreationStep.SelectDifficulty:
                    this.ChangeStep( CreationStep.SelectStats );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the currently enteredName is valid.
        /// </summary>
        /// <returns>
        /// true if the enteredName can be used;
        /// otherwise false.
        /// </returns>
        private bool IsNameValid()
        {
            return this.Profile.Name.Length != 0 && this.IsUniqueProfileName;
        }

        /// <summary>
        /// Attempts to create a new character/profile with the currently
        /// entered information.
        /// </summary>
        private void CreateCharacter()
        {
            this.Profile.Hardcore = this.checkBoxHardcore.IsSelected;
            this.Profile.CharacterColorTint = this.linkSprites.ColorTint;
            this.ChangeToSelectionState();
        }

        /// <summary>
        /// Changes from this CharacterCreationState back to the CharacterSelectionState.
        /// </summary>
        private void ChangeToSelectionState()
        {
            this.game.States.Replace<CharacterSelectionState>();
        }

        /// <summary>
        /// Changes the current CreationStep.
        /// </summary>
        /// <param name="step">
        /// The CreationStep to change to.
        /// </param>
        private void ChangeStep( CreationStep step )
        {
            DisableStep( this.currentStep );

            switch( step )
            {
                case CreationStep.EnterName:
                    characterWindow.Close();
                    break;

                case CreationStep.SelectStats:
                    characterWindow.Open();
                    break;

                case CreationStep.SelectLooks:
                    colorClothSelector.ShowAndEnable();
                    colorClothHighlightSelector.ShowAndEnable();
                    colorHairSelector.ShowAndEnable();
                    colorHairHighlightSelector.ShowAndEnable();
                    break;

                case CreationStep.SelectDifficulty:
                    foreach( var button in this.difficultyButtons )
                    {
                        button.ShowAndEnable();
                    }

                    this.SelectDifficultyAt( 1 );
                    this.checkBoxHardcore.ShowAndEnable();
                    break;

                default:
                    break;
            }

            this.currentStep = step;
            this.RefreshNavigationButtonStates();
        }

        private void DisableStep( CreationStep step )
        {
            switch( step )
            {
                case CreationStep.SelectStats:
                    characterWindow.Close();
                    break;

                case CreationStep.SelectLooks:
                    colorClothSelector.HideAndDisable();
                    colorClothHighlightSelector.HideAndDisable();
                    colorHairSelector.HideAndDisable();
                    colorHairHighlightSelector.HideAndDisable();
                    break;

                case CreationStep.SelectDifficulty:                    
                    foreach( var button in this.difficultyButtons )
                    {
                        button.HideAndDisable();
                    }
                    this.checkBoxHardcore.HideAndDisable();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Enumerates the different steps in the Character Creation procedure.
        /// </summary>
        private enum CreationStep
        {
            /// <summary>
            /// The first step requires the player to enter a name.
            /// </summary>
            EnterName,

            /// <summary>
            /// The second step requires the player to select his starting status points.
            /// </summary>
            SelectStats,

            /// <summary>
            /// The third step requires the player to select how is character looks.
            /// </summary>
            SelectLooks,

            /// <summary>
            /// The last step requires the player to select a difficulty.
            /// </summary>
            SelectDifficulty
        }

        /// <summary>
        /// Called when the user has pressed the Escape key.
        /// </summary>
        protected override void LeaveToPreviousState()
        {
            this.Profile = null;
            this.ChangeToSelectionState();
        }

        /// <summary>
        /// Holds the sprites to draw link.
        /// </summary>
        private LinkSprites linkSprites;

        /// <summary>
        /// The time that controls the blinking of the cursor.
        /// </summary>
        private float blinkTimer;
        
        /// <summary>
        /// The time that controls the rotation of the player character.
        /// </summary>
        private float spriteRotationTimer;

        /// <summary>
        /// The rotation index for the rotating character.
        /// </summary>
        private Direction4 spriteDirection = Direction4.Down;

        /// <summary>
        /// Represents the current step in the character creation process.
        /// </summary>
        private CreationStep currentStep;

        /// <summary>
        /// Specifies the maximum number of characters the name can have.
        /// </summary>
        private const int MaximumAllowedNameLength = 12;

        /// <summary>
        /// Represents a reference to the CharacterSelectionState.
        /// </summary>
        private CharacterSelectionState selectionState;

        /// <summary>
        /// The currently playing sample channel.
        /// </summary>
        private Channel sampleChannel;

        /// <summary>
        /// Used to allow the user to enable hardcore modus.
        /// </summary>
        private Zelda.UI.CheckBox checkBoxHardcore;

        /// <summary>
        /// Used to invest the status points of the player.
        /// </summary>
        private Zelda.UI.CharacterWindow characterWindow;

        /// <summary>
        /// The UI-movement buttons.
        /// </summary>
        private NavButton backButton, nextButton;

        /// <summary>
        /// Represents the game that owns this CharacterCreationState.
        /// </summary>
        private readonly ZeldaGame game;

        /// <summary>
        /// Enumerates the Buttons that allow the user to change the difficulty of the game.
        /// </summary>
        private readonly List<Button> difficultyButtons = new List<Button>();

        /// <summary>
        /// Enumerates the ui elements that allow the user to change the colors of the player sprite.
        /// </summary>
        private ColorSelector colorClothSelector, colorClothHighlightSelector, colorHairSelector, colorHairHighlightSelector;
    }
}
