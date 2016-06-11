
namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Xna = Microsoft.Xna.Framework;

    public sealed class VolumeControl : UIElement
    {
        public event RelaxedEventHandler<ChangedValue<float>> ValueChanged;

        public float Value 
        {
            get
            {
                return this.value;
            }

            set
            {
                float oldValue = this.value;
                this.value = MathUtilities.Clamp( value, 0.0f, 1.0f );

                this.ValueChanged.Raise( this, new ChangedValue<float>( oldValue, value ) );
            }
        }

        public VolumeControl( Vector2 position )
        {
            buttonDecrease.Position = new Vector2( position.X - 40.0f, position.Y );
            buttonDecrease.TextOffset = new Vector2( 2.0f, 0.0f );
            buttonDecrease.Clicked += OnButtonDecreaseClicked;

            buttonIncrease.Position = new Vector2( position.X + 40.0f, position.Y );
            buttonIncrease.Clicked += OnButtonIncreaseClicked;

            this.SetTransform( buttonDecrease.ClientArea.Position, Vector2.Zero, new Vector2( buttonIncrease.ClientArea.Right - buttonDecrease.ClientArea.X, 15 ) );
        }
                
        private static TextButton CreateButton( string text )
        {
            return new TextButton() {
                Text = " " + text + " ",
                ColorDefault = Xna.Color.White,
                ColorSelected = Xna.Color.Red,
                BackgroundColorDefault = Xna.Color.White.WithAlpha( 200 ),
                BackgroundColorSelected = Xna.Color.Black.WithAlpha( 200 ),
                Font = UIFonts.Tahoma10,
                TextAlign = TextAlign.Center,
                Size = new Vector2( 15, 15 ),
                TextOffset = new Vector2( 0, -1 ),
                PassInputToSubElements = true
            };
        }

        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            Xna.Color color = Atom.Xna.ColorHelper.Lerp( 
                Xna.Color.WhiteSmoke.WithAlpha( 200 ), 
                Xna.Color.Red.WithAlpha( 150 ), 
                this.value 
            );

            float top = this.buttonDecrease.ClientArea.Top + (int)Math.Floor(buttonIncrease.Size.Y * (1.0f - value));

            drawContext.Batch.DrawRect(
                new RectangleF(
                    this.buttonDecrease.ClientArea.Right,
                    top,
                    this.buttonIncrease.ClientArea.Left - this.buttonDecrease.ClientArea.Right,
                    this.buttonDecrease.ClientArea.Bottom - top
                ),
                color
            );
        }

        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        private void OnButtonDecreaseClicked( object sender, ref Xna.Input.MouseState mouseState, ref Xna.Input.MouseState oldMouseState )
        {
            ChangeValueBy( -0.1f );
        }

        private void OnButtonIncreaseClicked( object sender, ref Xna.Input.MouseState mouseState, ref Xna.Input.MouseState oldMouseState )
        {
            ChangeValueBy( 0.1f );
        }

        protected override bool HandleRelatedMouseInput( ref Xna.Input.MouseState mouseState, ref Xna.Input.MouseState oldMouseState )
        {
            int delta = mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;

            if( delta != 0 )
            {
                ChangeValueBy( delta / 1800.0f );
                return false;
            }

            return base.HandleRelatedMouseInput( ref mouseState, ref oldMouseState );
        }

        protected override void HandleMouseInput( ref Xna.Input.MouseState mouseState, ref Xna.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Xna.Input.ButtonState.Pressed )
            {
                RectangleF area = new RectangleF(
                    this.buttonDecrease.ClientArea.Right,
                    this.buttonDecrease.ClientArea.Top,
                    this.buttonIncrease.ClientArea.Left - this.buttonDecrease.ClientArea.Right,
                    buttonIncrease.Size.Y
                );

                RectangleF clampArea = new RectangleF(
                    area.X,
                    area.Y - 2,
                    area.Width,
                    area.Height + 4
                );

                if( clampArea.Contains( mouseState.Position() ) )
                {
                    float offset = area.Bottom - mouseState.Y;
                    this.Value = offset / area.Height;
                }
            }

            base.HandleMouseInput( ref mouseState, ref oldMouseState );
        }

        private void ChangeValueBy( float fixedValue )
        {
            this.Value = this.Value + fixedValue;
        }

        protected override void OnAdded( UserInterface userInterface )
        {
            userInterface.AddElement( buttonDecrease );
            userInterface.AddElement( buttonIncrease );
        }

        protected override void OnRemoved( UserInterface userInterface )
        {
            userInterface.RemoveElement( buttonDecrease );
            userInterface.RemoveElement( buttonIncrease );
        }

        private float value;
        private readonly TextButton buttonDecrease = CreateButton( "-" );
        private readonly TextButton buttonIncrease = CreateButton( "+" );
    }
}
