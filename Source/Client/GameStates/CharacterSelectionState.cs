// <copyright file="CharacterSelectionState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.CharacterSelectionState class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    using Zelda.Profiles;
    using Zelda.Timing;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines the IGameState in which the user
    /// can select the character he wishes to play.
    /// </summary>
    internal sealed class CharacterSelectionState : BaseOutgameState
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterSelectionState"/> class.
        /// </summary>
        /// <param name="game">
        /// The ZeldaGame object.
        /// </param>
        public CharacterSelectionState( ZeldaGame game )
            : base( game )
        {
            this.game = game;
            this.difficultyIndicatorDrawer = new Difficulties.DifficultyIndicatorDrawer( game ) {
                Alignment = HorizontalAlignment.Center
            };
        }

        #region > Load <

        /// <summary>
        /// Loads this IGameState.
        /// </summary>
        public override void Load()
        {
            this.linkSprites = this.game.GetService<LinkSprites>();
            this.LoadMusic();
            this.LoadUserInterface();
            this.LoadProfiles();
        }

        /// <summary>
        /// Partially loads the profiles stored in the profile folder.
        /// </summary>
        private void LoadProfiles()
        {
            // Clear
            this.selectedProfileIndex = -1;
            this.selectedProfile = null;
            this.profiles.Clear();

            // Receive folders
            string profileFolder = GameFolders.Profiles;
            Directory.CreateDirectory( profileFolder );

            string[] profileFolders = Directory.GetDirectories( profileFolder );

            // Load profiles
            foreach( string folder in profileFolders )
            {
                string profileName = folder.Remove( 0, profileFolder.Length + 1 );

                try
                {
                    profiles.Add( new ShortGameProfile( profileName, game ) );
                }
                catch( Exception exc )
                {
                    game.Log.WriteLine( string.Format( "An error occurred while loading the game profile '{0}.", profileName ) );
                    game.Log.WriteLine( exc.ToString() );
                    game.Log.WriteLine();
                }
            }

            this.SelectLastSavedProfile();
            this.RefreshIndexButtonVisability();
        }

        /// <summary>
        /// Overriden to setup the UserInterface of this CharacterSelectionState.
        /// </summary>
        protected override void SetupUserInterface()
        {
            ISpriteLoader spriteLoader = this.game.SpriteLoader;
            this.rectCharacterBackground = new Rectangle( 20, 20, 120, this.game.ViewSize.Y - 40 );

            this.difficultyIndicatorDrawer.Position = new Vector2(
                this.rectCharacterBackground.X + this.rectCharacterBackground.Width / 2.0f,
                this.rectCharacterBackground.Y + 25
            );
            this.difficultyIndicatorDrawer.LoadContent();

            // Button New Adventure
            Sprite spriteButtonNewDefault = spriteLoader.LoadSprite( "Button_New_Default" );
            this.buttonNew = new SpriteButton( "New", spriteButtonNewDefault, spriteLoader.LoadSprite( "Button_New_Selected" ) ) {
                Position = new Vector2( game.ViewSize.X - spriteButtonNewDefault.Width - 10, 40.0f )
            };

            this.buttonNew.Clicked += this.OnNewButtonClicked;
            this.UserInterface.AddElement( this.buttonNew );

            // Button Settings
            var settingsButton = new SpriteButton( "Settings", spriteLoader.LoadSprite( "Button_Settings_Default" ), spriteLoader.LoadSprite( "Button_Settings_Selected" ) ) {
                Position = this.buttonNew.Position + new Vector2( 1.0f, 30.0f )
            };

            settingsButton.Clicked += this.OnSettingsButtonClicked;
            this.UserInterface.AddElement( settingsButton );

            // Button Change Selected Profile Left
            Sprite spriteArrow = spriteLoader.LoadSprite( "Button_ChangeProfile_Left" );

            this.buttonProfileLeft = new SpriteButton( "Profile_Left", spriteArrow, spriteArrow ) {
                ColorDefault = new Xna.Color( 150, 150, 150, 255 ),
                ColorSelected = Xna.Color.White,
                Position = new Vector2( 24.0f, (game.ViewSize.Y / 2.0f) - 8.0f ),
                FloorNumber = 2,
                PassInputToSubElements = false
            };

            this.buttonProfileLeft.Clicked += this.OnButtonProfileLeftClicked;
            this.UserInterface.AddElement( buttonProfileLeft );

            // Button Change Selected Profile Right
            spriteArrow = spriteLoader.LoadSprite( "Button_ChangeProfile_Right" );
            this.buttonProfileRight = new SpriteButton( "Profile_Right", spriteArrow, spriteArrow ) {
                ColorDefault = new Xna.Color( 150, 150, 150, 255 ),
                ColorSelected = Xna.Color.White,
                Position = new Vector2( 120.0f, (game.ViewSize.Y / 2.0f) - 8.0f ),
                FloorNumber = 2,
                PassInputToSubElements = false
            };

            this.buttonProfileRight.Clicked += this.OnButtonProfileRightClicked;
            this.UserInterface.AddElement( buttonProfileRight );
        }

        /// <summary>
        /// Loads and plays the music of the CharacterSelectionState.
        /// </summary>
        private void LoadMusic()
        {
            if( this.channel != null )
            {
                return;
            }

            var files = new Atom.Collections.Hat<string>( game.Rand, 2 );
            files.Insert( "Select Screen.mid", 70.0f );
            files.Insert( "Triforce Piano.mid", 40.0f );

            this.music = game.AudioSystem.GetMusic( files.Get() );

            if( this.music != null )
            {
                this.music.LoadAsMusic( true );
                this.channel = this.music.Play();
            }
        }

        #endregion

        #region > Unload <

        /// <summary>
        /// Unloads this IGameState.
        /// </summary>
        public override void Unload()
        {
            UnloadMusic();
            base.Unload();
        }

        private void UnloadMusic()
        {
            if( this.channel != null )
            {
                this.channel.Stop();
                this.channel = null;
            }

            if( this.music != null )
            {
                this.music.Release();
                this.music = null;
            }
        }

        #endregion

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="GameProfile"/> which has been selected by the user in the CharacterSelectionState.
        /// </summary>
        public GameProfile SelectedProfile
        {
            // The profile is in a loaden state when this property is called.
            get
            {
                return (GameProfile)this.selectedProfile;
            }
        }

        /// <summary>
        /// Gets the <see cref="IGameProfile"/> that have been loaden.
        /// </summary>
        public IEnumerable<IGameProfile> Profiles
        {
            get
            {
                return this.profiles;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called before the scene is drawn.
        /// </summary>
        /// <param name="zeldaDrawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        protected override void DrawPreScene( ZeldaDrawContext zeldaDrawContext )
        {
        }

        /// <summary>
        /// Updates this BaseCharacterState.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        protected override void Update( ZeldaUpdateContext updateContext )
        {
            if( Scene != null )
            {
                this.Scene.Update( updateContext );
            }

            this.actionDisallowedTimer.Update( updateContext );
        }

        /// <summary>
        /// Refreshes the visability of Change Profile Buttons.
        /// </summary>
        private void RefreshIndexButtonVisability()
        {
            if( this.profiles.Count <= 0 )
            {
                this.buttonProfileLeft.IsEnabled = this.buttonProfileLeft.IsVisible = false;
                this.buttonProfileRight.IsEnabled = this.buttonProfileRight.IsVisible = false;
            }
            else
            {
                if( this.selectedProfileIndex == 0 )
                {
                    this.buttonProfileLeft.IsEnabled = this.buttonProfileLeft.IsVisible = false;
                }
                else
                {
                    this.buttonProfileLeft.IsEnabled = this.buttonProfileLeft.IsVisible = true;
                }

                if( this.selectedProfileIndex == this.profiles.Count - 1 )
                {
                    this.buttonProfileRight.IsEnabled = this.buttonProfileRight.IsVisible = false;
                }
                else
                {
                    this.buttonProfileRight.IsEnabled = this.buttonProfileRight.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Moves the currently selected profile left by one entry.
        /// </summary>
        private void MoveProfileIndexLeft()
        {
            if( this.selectedProfileIndex <= 0 )
            {
                return;
            }

            this.SelectProfile( this.profiles[this.selectedProfileIndex - 1] );
            this.RefreshIndexButtonVisability();
        }

        /// <summary>
        /// Moves the currently selected profile right by one entry.
        /// </summary>
        private void MoveProfileIndexRight()
        {
            if( this.selectedProfileIndex == this.profiles.Count - 1 )
            {
                return;
            }

            this.SelectProfile( this.profiles[this.selectedProfileIndex + 1] );

            this.RefreshIndexButtonVisability();
        }

        private void SelectProfile( IGameProfile profile )
        {
            this.selectedProfile = profile;
            this.selectedProfileIndex = profiles.IndexOf( profile );
            RefreshCharacterTint();
        }

        private void RefreshCharacterTint()
        {
            if( selectedProfile != null )
            {
                linkSprites.SetColorTint( selectedProfile.CharacterColorTint );
            }
        }

        /// <summary>
        /// Selects the profile the user has used last.
        /// </summary>
        private void SelectLastSavedProfile()
        {
            string lastSavedProfile = Settings.Instance.LastSavedProfile;

            for( int i = 0; i < this.profiles.Count; ++i )
            {
                IGameProfile profile = this.profiles[i];

                if( lastSavedProfile.Equals( profile.Name, StringComparison.OrdinalIgnoreCase ) )
                {
                    SelectProfile( profile );
                    return;
                }
            }

            // There was no last saved profile:
            if( this.profiles.Count > 0 )
            {
                this.SelectProfile( profiles[0] );
            }
        }

        /// <summary>
        /// Draws the background and ParticleEffect of the BaseCharacterState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void DrawBackground( ISpriteDrawContext drawContext )
        {
            base.DrawBackground( drawContext );

            // Draw black character overlay:
            if( this.profiles != null && this.profiles.Count > 0 )
            {
                drawContext.Begin();

                drawContext.Batch.DrawRect(
                    this.rectCharacterBackground,
                    new Xna.Color( 0, 0, 0, 180 )
                );

                drawContext.End();
            }
        }

        /// <summary>
        /// Draws the Normal State of the CharacterSelectionState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void DrawUserInterface( ISpriteDrawContext drawContext )
        {
            IComposedSpriteBatch batch = drawContext.Batch;
            drawContext.Begin();

            // Draw selected profile inside the black character overlay:
            if( selectedProfile != null )
            {
                int centerX = rectCharacterBackground.X + (rectCharacterBackground.Width / 2);
                DrawLinkSprite( centerX, batch );

                // Character Name.
                (selectedProfile.CharacterName.Length <= 8 ? this.FontLarge : this.Font).Draw(
                    selectedProfile.CharacterName ?? string.Empty,
                    new Vector2( centerX, 140.0f ),
                    TextAlign.Center,
                    selectedProfile.Hardcore ? Xna.Color.Red : Xna.Color.White,
                    drawContext
                );

                // Character Class.
                this.Font.Draw(
                    selectedProfile.CharacterClass ?? string.Empty,
                    new Vector2( centerX, 163.0f ),
                    TextAlign.Center,
                    Xna.Color.White,
                    drawContext
                );

                // Character Level.
                this.Font.Draw(
                    Resources.Level + " " + selectedProfile.CharacterLevel.ToString( CultureInfo.CurrentCulture ),
                    new Vector2( centerX, 177.0f ),
                    TextAlign.Center,
                    Xna.Color.White,
                    drawContext
                );

                // Region.
                this.Font.Draw(
                    selectedProfile.RegionName ?? string.Empty,
                    new Vector2( centerX, 195.0f ),
                    TextAlign.Center,
                    Xna.Color.White,
                    drawContext
                );

                // Difficulty
                this.difficultyIndicatorDrawer.Draw( this.selectedProfile.Difficulty, this.selectedProfile.Hardcore, drawContext );
            }

            drawContext.End();
        }

        private void DrawLinkSprite( int centerX, Atom.Xna.Batches.IComposedSpriteBatch batch )
        {
            Sprite sprite = linkSprites.StandDown;
            sprite.Draw( new Vector2( centerX - (sprite.Width / 2), 115 ), batch );
        }

        #region > Input <

        /// <summary>
        /// Called when the user presses on the "New" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mosue one frame ago.</param>
        private void OnNewButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            {
                this.game.States.Replace<CharacterCreationState>();
            }
        }

        /// <summary>
        /// Called when the user presses on the "Settings" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mosue one frame ago.</param>
        private void OnSettingsButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            {
                this.game.States.Push<SettingsState>();
            }
        }

        /// <summary>
        /// Called when the user presses on the "Left" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mosue one frame ago.</param>
        private void OnButtonProfileLeftClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.MoveProfileIndexLeft();
        }

        /// <summary>
        /// Called when the user presses on the "Right" button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mosue one frame ago.</param>
        private void OnButtonProfileRightClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.MoveProfileIndexRight();
        }

        /// <summary>
        /// Handles mouse input; called once every frame.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        protected override void OnMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed )
            {
                if( this.buttonProfileLeft.ClientArea.Contains( mouseState.X, mouseState.Y ) )
                {
                    return;
                }

                if( this.buttonProfileRight.ClientArea.Contains( mouseState.X, mouseState.Y ) )
                {
                    return;
                }

                // Enter game if the player clicks on the character rectangle.
                if( this.rectCharacterBackground.Contains( new Point2( mouseState.X, mouseState.Y ) ) )
                {
                    this.EnterGame();
                }
            }
            else if( (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released) &&
                     game.IsActive )
            {
                SpawnRandomEnemy();
            }
        }

        private void SpawnRandomEnemy() => randomTitleScreenEnemySpawner.SpawnRandomEnemy( game, UserInterface, Scene );
        /// <summary>
        /// Enters the game by loading the selected profile.
        /// </summary>
        private void EnterGame()
        {
            if( this.actionDisallowedTimer.HasNotEnded || this.selectedProfile == null )
            {
                return;
            }

            this.selectedProfile = this.selectedProfile.Load();
            this.game.States.Replace<IngameState>();
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
            if( (keyState.IsKeyDown( Keys.Enter ) && oldKeyState.IsKeyUp( Keys.Enter )) ||
                (keyState.IsKeyDown( Keys.Space ) && oldKeyState.IsKeyUp( Keys.Space )) )
            {
                this.EnterGame();
                return;
            }

            if( (keyState.IsKeyDown( Keys.Left ) && oldKeyState.IsKeyUp( Keys.Left )) ||
                (keyState.IsKeyDown( Keys.A ) && oldKeyState.IsKeyUp( Keys.A )) )
            {
                this.MoveProfileIndexLeft();
            }

            if( (keyState.IsKeyDown( Keys.Right ) && oldKeyState.IsKeyUp( Keys.Right )) ||
                (keyState.IsKeyDown( Keys.D ) && oldKeyState.IsKeyUp( Keys.D )) )
            {
                this.MoveProfileIndexRight();
            }
        }

        /// <summary>
        /// Called when the user has pressed the Escape key.
        /// </summary>
        protected override void LeaveToPreviousState()
        {
            this.Exit();
        }

        /// <summary>
        /// Exits the game, unless disallowExitingGameOnce is true.
        /// </summary>
        private void Exit()
        {
            if( this.actionDisallowedTimer.HasNotEnded )
            {
                return;
            }

            this.game.Exit();
        }

        #endregion

        #region > State <

        /// <summary>
        /// Gets called when the focus has changed from the given IGameState to this IGameState.
        /// </summary>
        /// <param name="oldState">
        /// The old IGameState.
        /// </param>
        public override void ChangedFrom( Atom.IGameState oldState )
        {
            var creationState = oldState as CharacterCreationState;

            if( creationState != null )
            {
                if( creationState.Profile != null )
                {
                    this.profiles.Add( creationState.Profile );
                    this.SelectProfile( creationState.Profile );
                    this.RefreshIndexButtonVisability();
                }
                else
                {
                    RefreshCharacterTint();
                }
            }
            else
            {
                this.game.ItemManager.Unload();
                this.Load();
                this.RefreshCharacterTint();
            }

            this.actionDisallowedTimer.Reset();
            base.ChangedFrom( oldState );

            if( this.Scene == null )
            {
                LoadBackupScene();
            }
        }

        private void LoadBackupScene()
        {
            // Setup Scene
            var scene = ZeldaScene.Load( "TitleScreen", this.game );
            scene.SetVisibilityStateActionLayer( false );
            scene.IngameDateTime.Current = new DateTime( 1000, 3, 1, 8, 45, 0 );
            scene.IngameDateTime.TickSpeed = 310.0f;

            // Setup camera:
            ZeldaCamera camera = scene.Camera;
            camera.ViewSize = game.ViewSize;
            camera.Scroll = new Vector2( 180 * 16.0f, 16 * 16.0f );

            this.Scene = scene;
        }

        /// <summary>
        /// Gets called when the focus has changed away from this IGameState to the given IGameState.
        /// </summary>
        /// <param name="newState">
        /// The new IGameState.
        /// </param>
        public override void ChangedTo( Atom.IGameState newState )
        {
            if( newState is IngameState )
            {
                this.Unload();
            }

            base.ChangedTo( newState );
        }

        #endregion

        #endregion

        #region [ Fields ]

        #region > Profile related <

        /// <summary>
        /// The index of the currently selected IGameProfile.
        /// </summary>
        private int selectedProfileIndex = -1;

        /// <summary>
        /// The currently selcted IGameProfile.
        /// </summary>
        private IGameProfile selectedProfile;

        /// <summary>
        /// The list of IGameProfiles.
        /// </summary>
        private readonly List<IGameProfile> profiles = new List<IGameProfile>();

        #endregion

        #region > Music related <

        /// <summary>
        /// The music that is played in the background.
        /// </summary>
        private Atom.Fmod.Sound music;
        private Atom.Fmod.Channel channel;

        #endregion

        #region > UI related <

        /// <summary>
        /// The rectangle that defines the area of the black character background.
        /// </summary>
        private Rectangle rectCharacterBackground;

        /// <summary>
        /// The Buttons active in the UI.
        /// </summary>
        private SpriteButton buttonNew, buttonProfileLeft, buttonProfileRight;

        /// <summary>
        /// The sprites used to draw link.
        /// </summary>
        private LinkSprites linkSprites;

        /// <summary>
        /// Implements a mechanism that draws a simple indicator for the difficulty of the currently selected profile.
        /// </summary>
        private readonly Difficulties.DifficultyIndicatorDrawer difficultyIndicatorDrawer;

        #endregion

        /// <summary>
        /// The timer that controls the time for which no action, such as entering or leaving the game, are not allowed to
        /// be executed.
        /// </summary>
        private readonly ResetableTimer actionDisallowedTimer = new ResetableTimer( 0.25f );

        /// <summary>
        /// Spawns the random enemies on right mouse-click.
        /// </summary>
        private readonly RandomTitleScreenEnemySpawner randomTitleScreenEnemySpawner = new RandomTitleScreenEnemySpawner();

        /// <summary>
        /// The ZeldaGame object.
        /// </summary>
        private readonly ZeldaGame game;

        #endregion
    }
}