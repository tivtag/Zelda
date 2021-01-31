
namespace Zelda.GameStates
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Graphics;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;
    using Atom.Fmod;

    internal sealed class SettingsState : BaseOutgameState
    {
        private const int AspectRatioButtonY = 45 * 1;
        private const int RenderModeButtonY = 45 * 2;
        private const int VsyncBoxY = 3 + (45 * 3);
        private const int MasterVolumeButtonY = (int)(45 * 3.65f);
        private const int MusicVolumeButtonY = MasterVolumeButtonY + 44;
        private const int EffectVolumeButtonY = MasterVolumeButtonY + 44;

        public SettingsState( ZeldaGame game )
            : base( game )
        {
            this.game = game;
        }

        public override void Load()
        {
            this.LoadUserInterface();
        }

        public override void ChangedFrom( IGameState oldState )
        {
            base.ChangedFrom( oldState );

            this.Load();
            this.RefreshUiState();
        }

        private void RefreshUiState()
        {
            this.buttonWindowMode.IsSelected = !this.settings.IsFullscreen;
            this.buttonFullscreenMode.IsSelected = this.settings.IsFullscreen;

            this.masterVolume.Value = settings.MasterVolume;
            this.musicVolume.Value = settings.MusicVolume;
            this.effectVolume.Value = settings.EffectVolume;

            this.SelectCurrentAspectRatioButton();
            this.checkBoxVsync.IsSelected = this.settings.VSync;
        }

        public override void ChangedTo( IGameState newState )
        {
            this.settings.Save();
            this.game.Graphics.ChangePipeline( Zelda.Graphics.DrawingPipeline.Normal );

            base.ChangedTo( newState );
        }

        private void SelectCurrentAspectRatioButton()
        {
            var button = this.aspectRatioButtons.First( b => (AspectRatio)b.Tag == settings.AspectRatio );
            button.IsSelected = true;
        }

        protected override void SetupUserInterface()
        {
            Point2 viewSize = this.game.ViewSize;
            float centerX = (int)viewSize.X / 2.0f;
            this.aspectRatioButtons.Clear();

            Button button = this.AddAspectRatioButton( "Normal", AspectRatio.Normal );
            button.Position = new Vector2( centerX - 50.0f, AspectRatioButtonY );

            button = this.AddAspectRatioButton( "16:9", AspectRatio.Wide16to9 );
            button.Position = new Vector2( centerX, AspectRatioButtonY );

            button = this.AddAspectRatioButton( "16:10", AspectRatio.Wide16to10 );
            button.Position = new Vector2( centerX + 45.0f, AspectRatioButtonY );

            button = this.buttonWindowMode = this.AddButton( "Window" );
            button.Position = new Vector2( centerX - 40.0f, RenderModeButtonY );
            button.Clicked += this.OnWindowedRenderModeButtonClicked;

            button = this.buttonFullscreenMode = this.AddButton( "Fullscreen" );
            button.Position = new Vector2( centerX + 40.0f, RenderModeButtonY );
            button.Clicked += this.OnFullscreenRenderModeButtonClicked;

            // Master Volume
            masterVolume = new VolumeControl( new Vector2( centerX, MasterVolumeButtonY ) );
            masterVolume.ValueChanged += OnMasterVolumeValueChanged;
            UserInterface.AddElement( masterVolume );

            // Music Volume
            musicVolume = new VolumeControl( new Vector2( centerX - 60.0f, MusicVolumeButtonY ) );
            musicVolume.ValueChanged += OnMusicVolumeValueChanged;
            UserInterface.AddElement( musicVolume );
            
            // Effect Volume
            effectVolume = new VolumeControl( new Vector2( centerX + 60.0f, EffectVolumeButtonY ) );
            effectVolume.ValueChanged += OnEffectVolumeValueChanged;
            UserInterface.AddElement( effectVolume );
                        
            checkBoxVsync = new CheckBox();
            checkBoxVsync.Position = new Vector2( centerX + 29.0f, VsyncBoxY - 20.0f );
            checkBoxVsync.Clicked += OnCheckBoxVsyncClicked;
            UserInterface.AddElement( checkBoxVsync );
            
            // Back Button
            this.backButton = new NavButton( "BackButton", game ) {
                Position = new Vector2( 3, game.ViewSize.Y - 23 ),
                ButtonMode = NavButton.Mode.Back
            };

            backButton.Clicked += OnBackButtonClicked;
            UserInterface.AddElement( backButton );

            var backgroundRectangle = new RectangleUIElement() {
                Size = viewSize,
                Color = Microsoft.Xna.Framework.Color.Black.WithAlpha( 200 )
            };

            UserInterface.AddElement( backgroundRectangle );
        }

        private void OnMasterVolumeValueChanged( object sender, ChangedValue<float> e )
        {
            if( game.AudioSystem.IsInitialized )
            {
                ChannelGroup group = game.AudioSystem.MasterChannelGroup;
                group.Volume = e.NewValue;
            }

            Settings.Instance.MasterVolume = e.NewValue;
        }

        private void OnMusicVolumeValueChanged( object sender, ChangedValue<float> e )
        {
            if( game.AudioSystem.IsInitialized )
            {
                SoundGroup group = game.AudioSystem.MusicGroup;
                group.Volume = e.NewValue;
            }

            Settings.Instance.MusicVolume = e.NewValue;
        }

        private void OnEffectVolumeValueChanged( object sender, ChangedValue<float> e )
        {
            if( game.AudioSystem.IsInitialized )
            {
                SoundGroup group = game.AudioSystem.SampleGroup;
                group.Volume = e.NewValue;
            }

            Settings.Instance.EffectVolume = e.NewValue;
        }
             
        private void OnWindowedRenderModeButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.buttonWindowMode.IsSelected = true;
            this.buttonFullscreenMode.IsSelected = false;

            this.settings.IsFullscreen = false;
            game.Graphics.SetFullScreen( false );
        }

        private void OnFullscreenRenderModeButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.buttonWindowMode.IsSelected = false;
            this.buttonFullscreenMode.IsSelected = true;

            this.settings.IsFullscreen = true;
            this.RefreshFullscreenStretchedSetting();
            game.Graphics.SetFullScreen( true );
        }

        private void OnBackButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.LeaveToPreviousState();
        }

        private void RefreshFullscreenStretchedSetting()
        {
            if( this.settings.IsFullscreen )
            {
                this.settings.IsFullscreenStretched = this.settings.AspectRatio != AspectRatio.Normal;
            }
        }

        private void OnAspectRatioButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            Button button = (Button)sender;
            AspectRatio ratio = (AspectRatio)button.Tag;

            this.DeselectAspectRatioButtons();
            button.IsSelected = true;

            this.settings.AspectRatio = ratio;
            this.RefreshFullscreenStretchedSetting();
        }

        private void DeselectAspectRatioButtons()
        {
            foreach( Button aspectRatioButton in this.aspectRatioButtons )
            {
                aspectRatioButton.IsSelected = false;
            }
        }

        private Button AddAspectRatioButton( string text, AspectRatio ratio )
        {
            TextButton button = this.AddButton( text );

            button.Tag = ratio;
            button.Clicked += this.OnAspectRatioButtonClicked;
            this.aspectRatioButtons.Add( button );
            return button;
        }
        
        private TextButton AddButton( string text )
        {
            TextButton button = new TextButton() {
                Text = " " + text + " ",
                ColorDefault = Xna.Color.White,
                ColorSelected = Xna.Color.Red,
                BackgroundColorDefault = Xna.Color.Black.WithAlpha( 200 ),
                BackgroundColorSelected = Xna.Color.White.WithAlpha( 200 ),
                Font = UIFonts.Tahoma10,
                TextAlign = TextAlign.Center
            };

            this.UserInterface.AddElement( button );
            return button;
        }

        protected override void DrawUserInterface( Atom.Xna.ISpriteDrawContext drawContext )
        {
            drawContext.Begin();
            {
                Point2 viewSize = this.game.ViewSize;
                float centerX = (int)viewSize.X / 2.0f;

                // Draw Title Background
                drawContext.Batch.DrawRect(
                    new Xna.Rectangle( 0, 0, game.ViewSize.X, 20 ),
                    UIColors.LightWindowBackground,
                    0.001f
                );

                // Draw Title String
                UIFonts.TahomaBold11.Draw(
                    "Settings",
                    new Vector2( game.ViewSize.X / 2, 0.0f ),
                    TextAlign.Center,
                    new Microsoft.Xna.Framework.Color( 0, 0, 0, 155 ),
                    0.002f,
                    drawContext
                );

                DrawHeader( "Aspect Ratio *", new Vector2( centerX, AspectRatioButtonY - 20.0f ), drawContext );
                DrawHeader( "Render Mode", new Vector2( centerX, RenderModeButtonY - 20.0f ), drawContext );
                DrawHeader( "V-sync", new Vector2( centerX - 10.0f, VsyncBoxY - 20.0f ), drawContext );
                DrawHeader( "Master Volume", new Vector2( centerX, MasterVolumeButtonY - 20.0f ), drawContext );
                DrawHeader( "Music Volume", new Vector2( centerX - 60.0f, MusicVolumeButtonY - 20.0f ), drawContext );
                DrawHeader( "Effect Volume", new Vector2( centerX + 60.0f, EffectVolumeButtonY - 20.0f ), drawContext );

                UIFonts.Tahoma7
                    .Draw( "* requires restart to take effect", new Vector2( viewSize.X - 4, viewSize.Y - UIFonts.Tahoma7.LineSpacing ),
                            TextAlign.Right, Xna.Color.White, drawContext );
            }
            drawContext.End();
        }

        private static void DrawHeader( string text, Vector2 position, ISpriteDrawContext drawContext )
        {
            UIFonts.TahomaBold11
                .Draw( text, position, TextAlign.Center, Xna.Color.White, drawContext );
        }

        protected override void OnKeyboardInput( ref Microsoft.Xna.Framework.Input.KeyboardState keyState, ref Microsoft.Xna.Framework.Input.KeyboardState oldKeyState )
        {
        }

        protected override void LeaveToPreviousState()
        {
            this.game.States.Pop();
        }

        private void OnCheckBoxVsyncClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.settings.VSync = checkBoxVsync.IsSelected;
            game.Graphics.SetVsync( this.settings.VSync );
        }

        private Settings settings = Settings.Instance;

        private CheckBox checkBoxVsync;
        private Button buttonWindowMode, buttonFullscreenMode;
        private VolumeControl masterVolume, musicVolume, effectVolume;
        private NavButton backButton;

        private readonly IList<Button> aspectRatioButtons = new List<Button>();
        private readonly ZeldaGame game;
    }
}
