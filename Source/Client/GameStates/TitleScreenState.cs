// <copyright file="TitleScreenState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.TitleScreenState class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Difficulties;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// The Title Screen shows a short introduction to the game.
    /// </summary>
    internal sealed class TitleScreenState : IGameState, ISceneProvider
    {
        #region [ Properties ]

        public ZeldaScene Scene => this.scene;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleScreenState"/> class.x
        /// </summary>
        /// <param name="game">
        /// The game object.
        /// </param>
        public TitleScreenState( ZeldaGame game )
        {
            this.game = game;
        }

        /// <summary>
        /// Loads the TitleScreenState. This is automatically handled.
        /// </summary>
        public void Load()
        {
            this.LoadMusic();

            // Load assets:
            this.spriteLogo = game.SpriteLoader.LoadSprite( "TitleLogo" );

            // To allow loading of enemies
            GameDifficulty.Current = DifficultyId.Easy;

            // Setup Scene
            scene = ZeldaScene.Load( "TitleScreen", this.game );
            scene.SetVisibilityStateActionLayer( false );

            scene.IngameDateTime.Current = new DateTime( 1000, 3, 1, 8, 45, 0 );
            scene.IngameDateTime.TickSpeed = 310.0f;

            // Setup UI
            userInterface = new ZeldaUserInterface( providesDialog: false );
            userInterface.Setup( game );

            // Setup camera:
            ZeldaCamera camera = scene.Camera;
            camera.ViewSize = game.ViewSize;
            camera.Scroll = new Vector2( 11 * 16.0f, 16 * 16.0f );

            scene.WeatherMachine.SetRandomWeather( this.game.Rand );
            game.Scripts.OnSceneChanged( scene );
        }

        /// <summary>
        /// Loads the music playn in the Title Screen.
        /// </summary>
        private void LoadMusic()
        {
            musicLogo = game.AudioSystem.GetMusic( "Title Selected.mid" );
            music = game.AudioSystem.GetMusic( this.GetMusicName() );

            if( music != null )
            {
                music.LoadAsMusic( false );

                musicChannel = music.Play( true );

                musicChannel.Volume = 0.0f;
                musicChannel.Ended += this.OnMusicEnded;
                musicChannel.IsPaused = false;
            }

            if( musicLogo != null )
            {
                musicLogo.LoadAsSample( false );
            }

            this.musicFadeIner = new Zelda.Audio.MusicFadeIner( musicChannel, 7.0f );
        }

        /// <summary>
        /// Randomly chooses the music that should be played in the title screen.
        /// </summary>
        /// <returns>
        /// The name that uniquely identifies the music resource to be played.
        /// </returns>
        private string GetMusicName()
        {
            var titles = new Tuple<float, string>[] {
                Tuple.Create( 300.0f, "Gerudo Valley.mid" ),
                Tuple.Create( 150.0f, "Grossbauer.mp3" ),
                Tuple.Create( 700.0f, "KaiRo-Inspiration-Medley.mp3" )
            };

            return this.game.Rand.Select( titles );
        }

        /// <summary>
        /// Unloads the TitleScreenState. This is automatically handled.
        /// </summary>
        public void Unload()
        {
            if( this.music != null )
            {
                this.game.AudioSystem.Remove( this.music.Name );
                this.music.Release();
                this.music = null;
            }

            if( this.musicLogo != null )
            {
                this.game.AudioSystem.Remove( this.musicLogo.Name );
                this.musicLogo.Release();
                this.musicLogo = null;
            }

            if( this.scene != null )
            {
                // this.scene.NotifySceneChange( ChangeType.Away );
               //this.scene = null;
            }

            this.musicChannel = null;
            this.spriteLogo = null;

            // The TitleScreenState won't be needed anymore,
            // until the player restarts the game.
            this.game.States.Remove<TitleScreenState>();
        }

        #endregion

        #region [ Methods ]

        #region > Draw <

        /// <summary>
        /// Draws the TitleScreenState.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( Atom.IDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            Graphics.IDrawingPipeline pipeline = this.game.Graphics.Pipeline;

            pipeline.InitializeFrame( this.scene, null, zeldaDrawContext );
            {
                pipeline.BeginScene();
                pipeline.EndScene();

                pipeline.BeginUserInterface();
                {
                    // Only show the "Press Space" string 
                    // when the logo is not shown!
                    if( isLogoShown )
                    {
                        DrawLogo( zeldaDrawContext );
                    }
                    else
                    {
                        DrawPressSpace( zeldaDrawContext );
                    }
                }
                pipeline.EndUserInterface();
            }
        }

        /// <summary>
        /// Draws the Press Space string.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawPressSpace( ZeldaDrawContext drawContext )
        {
            if( pressSpaceTickTime >= PressSpaceTickInterval &&
                pressSpaceTickTime < PressSpaceShownEndTime )
            {
                drawContext.Begin();

                var textPosition = new Vector2(
                    (game.ViewSize.X / 2.0f) - (font.MeasureString( StringPressSpace ).X / 2.0f),
                    game.ViewSize.Y * 2.0f / 3.0f
                 );

                this.font.Draw( StringPressSpace, textPosition, Xna.Color.White, drawContext );
                drawContext.End();
            }
            else if( pressSpaceTickTime >= PressSpaceShownEndTime )
            {
                pressSpaceTickTime = 0.0f;
            }
        }

        /// <summary>
        /// Draws the TLoZ BC logo.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawLogo( ISpriteDrawContext drawContext )
        {
            drawContext.Begin();

            const byte Rgb = 255;

            // Draw it centered:
            spriteLogo.Draw(
                new Point2(
                    (game.ViewSize.X / 2) - (spriteLogo.Width / 2),
                    (game.ViewSize.Y / 2) - (spriteLogo.Height / 2)
                ),
                new Xna.Color( Rgb, Rgb, Rgb, (byte)(logoAlpha * 255) ),
                drawContext.Batch
            );

            drawContext.End();
        }

        #endregion

        #region > Update <

        /// <summary>
        /// Updates the TitleScreenState.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( Atom.IUpdateContext updateContext )
        {
            ZeldaUpdateContext zeldaUpdateContext = (ZeldaUpdateContext)updateContext;
            userInterface.Update( updateContext );
            if( HandleInput() )
            {
                return;
            }

            if( isLogoShown )
            {
                logoAlpha += 0.085f * zeldaUpdateContext.FrameTime;
                if( logoAlpha > 1.0f )
                {
                    logoAlpha = 1.0f;
                }
            }
            else
            {
                pressSpaceTickTime += zeldaUpdateContext.FrameTime;
            }

            UpdateScroll( updateContext );

            TriggerEvents();

            scene.Update( zeldaUpdateContext );
            UpdateMusic( zeldaUpdateContext );
        }

        private void UpdateScroll( IUpdateContext updateContext )
        {
            if( updateContext.FrameTime == 0.0f )
            {
                return;
            }

            KeyboardState keyState = userInterface.KeyState;
            float scrollSpeed = 16.0f + (keyState.IsKeyDown( Keys.LeftAlt ) ? 128.0f : 0.0f) + (keyState.IsKeyDown( Keys.LeftShift ) ? 64.0f : 0.0f);
            ZeldaCamera camera = scene.Camera;

            if( !hasScrollReachedEnd )
            {
                Vector2 oldScroll = camera.Scroll;
                camera.Scroll += new Vector2( updateContext.FrameTime * scrollSpeed, 0.0f );

                if( camera.Scroll == oldScroll )
                {
                    hasScrollReachedEnd = true;
                }
            }
            else
            {
                scrollSpeed += 84.0f;

                if( keyState.IsKeyDown( Keys.Left ) )
                {
                    camera.Scroll -= new Vector2( updateContext.FrameTime * scrollSpeed, 0.0f );
                }
                if( keyState.IsKeyDown( Keys.Right ) )
                {
                    camera.Scroll += new Vector2( updateContext.FrameTime * scrollSpeed, 0.0f );
                }
                if( keyState.IsKeyDown( Keys.Up ) )
                {
                    camera.Scroll -= new Vector2( 0.0f, updateContext.FrameTime * scrollSpeed );
                }
                if( keyState.IsKeyDown( Keys.Down ) )
                {
                    camera.Scroll += new Vector2( 0.0f, updateContext.FrameTime * scrollSpeed );
                }
            }
        }

        private void TriggerEvents()
        {
            var area = new Rectangle( (int)scene.Camera.Scroll.X, (int)scene.Camera.Scroll.Y, 16, scene.Camera.ViewSize.Y );
            scene.EventManager.TriggerEvents( null, 0, area );
        }

        /// <summary>
        /// Updates music-related stuff.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateMusic( ZeldaUpdateContext updateContext )
        {
            if( musicChannel == null )
            {
                return;
            }

            UpdateMusicFadeIn( updateContext );
            UpdateMusicFadeOut( updateContext );
        }

        /// <summary>
        /// Handles the fade-in of the background music.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateMusicFadeIn( ZeldaUpdateContext updateContext )
        {
            this.musicFadeIner.Update( updateContext );
        }

        /// <summary>
        /// Handles the fade-out of the background music.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateMusicFadeOut( ZeldaUpdateContext updateContext )
        {
            uint position = musicChannel.GetPosition( Atom.Fmod.Native.TIMEUNIT.MS );
            uint length = musicChannel.Sound.GetLength( Atom.Fmod.Native.TIMEUNIT.MS );

            float factor = (float)position / (float)length;
            if( factor >= 0.95f )
            {
                fadeMusicOut = true;
            }

            if( fadeMusicOut )
            {
                float newVolume = musicChannel.Volume - (updateContext.FrameTime / 4.0f);

                if( newVolume <= 0.0f )
                {
                    newVolume = 0.0f;
                    fadeMusicOut = false;
                }

                musicChannel.Volume = newVolume;

                if( musicChannel.Volume == 0.0f && musicChannel.IsPlaying )
                {
                    musicChannel.Stop();
                    musicChannel = null;
                }
            }
        }

        #endregion

        #region > Input <

        /// <summary>
        /// Handles the input of the user.
        /// </summary>
        /// <returns>
        /// true when the user has decided to leave the TitleScreen;
        /// otherwise false.
        /// </returns>
        private bool HandleInput()
        {
            KeyboardState keyState = userInterface.KeyState;
            MouseState mouseState = userInterface.MouseState;

            if( (keyState.IsKeyDown( Keys.Space ) && oldKeyState.IsKeyUp( Keys.Space )) ||
                (keyState.IsKeyDown( Keys.Enter ) && oldKeyState.IsKeyUp( Keys.Enter )) ||
                (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released) && 
                game.IsActive )
            {
                if( isLogoShown )
                {
                    game.States.Replace<CharacterSelectionState>();
                    return true;
                }
                else
                {
                    this.ShowTitleLogo();
                }
            }
            else if( keyState.IsKeyDown( Keys.Escape ) )
            {
                this.game.Exit();
            }
            else if( (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released) &&
                     game.IsActive )
            {
                SpawnRandomEnemy();
            }

            oldKeyState = keyState;
            oldMouseState = mouseState;
            return false;
        }

        private void SpawnRandomEnemy()
        {
            Atom.Collections.Hat<string> templateHat = new Atom.Collections.Hat<string>( game.Rand );
            templateHat.Insert( "Skeleton", 11.0f );
            templateHat.Insert( "Spider_Small_45", 5.0f );
            templateHat.Insert( "Skeleton_Red", 4.0f );
            templateHat.Insert( "SkeletonHead", 4.0f );
            templateHat.Insert( "Ghost", 3.0f );
            templateHat.Insert( "Boss_RudrasEye", 1.0f );

            ZeldaEntity entity = game.EntityTemplateManager
                .GetTemplate( templateHat.Get() )
                .CreateInstance();

            entity.Transform.Position = scene.Camera.Scroll + userInterface.MousePosition;

            if( entity.Components.Get<Moveable>().IsWalkableAt( entity.Transform.Position, scene.Map.Floors[0].ActionLayer ) )
            {
                scene.Add( entity );
            }
        }

        #endregion

        #region > Change <

        /// <summary>
        /// Tells the IGameState that the title-logo should be shown now.
        /// </summary>
        private void ShowTitleLogo()
        {
            if( isLogoShown )
            {
                return;
            }

            if( musicChannel != null && musicChannel.IsPlaying )
            {
                this.fadeMusicOut = true;
            }

            if( musicLogo != null )
            {
                musicLogo.Play();
            }

            isLogoShown = true;
        }

        /// <summary>
        /// Gets called when the current GameState changed to this GameState.
        /// </summary>
        /// <param name="oldState">
        /// The old state.
        /// </param>
        public void ChangedFrom( IGameState oldState )
        {
            this.Load();
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

        #endregion

        /// <summary>
        /// When the music stops we want to show the 'Title Logo'.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnMusicEnded( Atom.Fmod.Channel sender )
        {
            this.ShowTitleLogo();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The scene displayed in the TitleScreenState.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The logo-sprite.
        /// </summary>
        private Sprite spriteLogo;

        /// <summary>
        /// States whether the title-logo is shown.
        /// </summary>
        private bool isLogoShown;

        /// <summary>
        /// Alpha value of the logo. Used for blending.
        /// </summary>
        private float logoAlpha;

        /// <summary>
        /// Stores the current tick value for the "Press Space" string.
        /// </summary>
        private double pressSpaceTickTime;

        /// <summary>
        /// States whether the automatic scrolling of the scene has reached the end.
        /// </summary>
        private bool hasScrollReachedEnd;

        /// <summary>
        /// Used for input detection.
        /// </summary>
        private ZeldaUserInterface userInterface;

        #region > Constants <

        /// <summary>
        /// Stores the localized string 'Press Space'.
        /// </summary>
        private static readonly string StringPressSpace = Resources.Text_PressSpace;

        /// <summary>
        /// States how long the "Press Space" string is shown.
        /// </summary>
        private const double PressSpaceShownTime = 2.0;

        /// <summary>
        /// States how long it takes until the "Press Space" string is shown again
        /// once it was shown.
        /// </summary>
        private const double PressSpaceTickInterval = 8.0;

        /// <summary>
        /// States when the "Press Space" string is hidden again.
        /// </summary>
        private const double PressSpaceShownEndTime = PressSpaceTickInterval + PressSpaceShownTime;

        #endregion

        #region > Audio <

        /// <summary>
        /// The music that is played in the background.
        /// </summary>
        private Atom.Fmod.Sound music;

        /// <summary>
        /// The channel the music plays in.
        /// </summary>
        private Atom.Fmod.Channel musicChannel;

        /// <summary>
        /// States whether the music should be faded out.
        /// </summary>
        private bool fadeMusicOut;

        /// <summary>
        /// The music played when the user presses 'Space' for the first time.
        /// </summary>
        private Atom.Fmod.Sound musicLogo;

        /// <summary>
        /// The object responsible for fading in the background music of the TitleScreen.
        /// </summary>
        private Zelda.Audio.MusicFadeIner musicFadeIner;

        #endregion

        #region > Input <

        private KeyboardState oldKeyState;
        private MouseState oldMouseState;

        #endregion

        /// <summary>
        /// The font used to draw the "Press Space" text.
        /// </summary>
        private readonly IFont font = Zelda.UI.UIFonts.VerdanaBold11;

        /// <summary>
        /// The game object.
        /// </summary>
        private readonly ZeldaGame game;

        #endregion
    }
}
