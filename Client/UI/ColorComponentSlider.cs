
namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the user to select the value of a single color component.
    /// </summary>
    public sealed class ColorComponentSlider : UIElement
    {
        /// <summary>
        /// Raised when the value of this ColorComponentSlider changed.
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<byte>> ValueChanged;

        /// <summary>
        /// Gets or sets the value of this ColorComponentSlider.
        /// </summary>
        public byte Value
        {
            get
            {
                return this.value;
            }

            set
            {
                byte oldValue = this.value;
                this.value = value;

                this.ValueChanged.Raise( this, new ChangedValue<byte>( oldValue, value ) );
            }
        }

        private RectangleF SliderArea
        {
            get
            {
                return new RectangleF(
                    this.buttonDecrease.ClientArea.Right,
                    this.buttonDecrease.ClientArea.Top,
                    this.buttonIncrease.ClientArea.Left - this.buttonDecrease.ClientArea.Right,
                    buttonIncrease.Size.Y
                );
            }
        }

        /// <summary>
        /// Initializes a new instance of the ColorComponentSlider class.
        /// </summary>
        /// <param name="component">
        /// The component that this slider affects.
        /// </param>
        /// <param name="position">
        /// The position of the UIElement.
        /// </param>
        public ColorComponentSlider( ColorComponent component, Vector2 position )
        {
            this.component = component;

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
                ColorDefault = Xna.Color.Black,
                ColorSelected = Xna.Color.Red,
                BackgroundColorDefault = Xna.Color.White.WithAlpha( 200 ),
                BackgroundColorSelected = Xna.Color.White.WithAlpha( 200 ),
                Font = UIFonts.Tahoma10,
                TextAlign = TextAlign.Center,
                Size = new Vector2( 14, 14 ),
                TextOffset = new Vector2( 0, -1 ),
                PassInputToSubElements = true
            };
        }

        /// <summary>
        /// Called when this ColorComponentSlider is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current content.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            Xna.Color color = Atom.Xna.ColorHelper.Create( value, component );
            float factor = value / (float)byte.MaxValue;

            float totalWidth = this.buttonIncrease.ClientArea.Left - this.buttonDecrease.ClientArea.Right;
            float width = totalWidth * factor;

            drawContext.Batch.DrawRect(
                new RectangleF(
                    this.buttonDecrease.ClientArea.Right,
                    this.buttonDecrease.ClientArea.Top,
                    width,
                    buttonIncrease.Size.Y
                ),
                color
            );

            string str = string.Format( "{0} {1}", value.ToString(), LocalizedEnums.GetShort( this.component ) );
            UIFonts.Tahoma7.Draw(
                str,
                new Vector2( this.buttonDecrease.ClientArea.Right + (totalWidth / 2.0f), this.buttonDecrease.ClientArea.Top + 2 ),
                TextAlign.Center,
                Xna.Color.White,
                0.5f,
                drawContext
            );
        }

        /// <summary>
        /// Called when this ColorComponentSlider is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current context.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // no op
        }

        private void OnButtonDecreaseClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            ChangeValueBy( -1 );
        }

        private void OnButtonIncreaseClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            ChangeValueBy( 1 );
        }

        /// <summary>
        /// Callen when mouse input happened within this UIElement.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The old state of the mouse.
        /// </param>
        /// <returns>
        /// True if input should be bassed to elements behind this element;-or- otherwise false.
        /// </returns>
        protected override bool HandleRelatedMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            int delta = mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;

            if( delta != 0 )
            {
                ChangeValueBy( 10 * Math.Sign( delta ) );
                return false;
            }

            return base.HandleRelatedMouseInput( ref mouseState, ref oldMouseState );
        }

        /// <summary>
        /// Called when mouse input happens while this UIElement is active.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The old state of the mouse.
        /// </param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed )
            {
                HandleLeftPress( ref mouseState, ref oldMouseState );
            }
            else
            {
                wasClickedWithinSliderArea = false;
            }

            base.HandleMouseInput( ref mouseState, ref oldMouseState );
        }

        private void HandleLeftPress( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            RectangleF sliderArea = this.SliderArea;
            RectangleF sliderTouchArea = new RectangleF(
                sliderArea.X - 30,
                sliderArea.Y,
                sliderArea.Width + 60,
                sliderArea.Height
            );

            // Use extended trigger area when the player is sliding the slider, instead of clicking it
            // This is required for the edge cases at the start and end of the slider to better register
            bool isClick = oldMouseState.LeftButton == ButtonState.Released;
            if( !isClick )
            {
                if( !wasClickedWithinSliderArea )
                {
                    return;
                }
            }

            RectangleF testArea = isClick ? sliderArea : sliderTouchArea;

            if( testArea.Contains( new Point2( mouseState.X, mouseState.Y ) ) )
            {
                float percentage = 1.0f - ((sliderArea.Right - mouseState.X) / (float)sliderArea.Width);
                SetValue( (int)Math.Ceiling( byte.MaxValue * percentage ) );
                wasClickedWithinSliderArea = true;
            }
        }

        private void ChangeValueBy( int fixedValue )
        {
            SetValue( this.Value + fixedValue );
        }

        private void SetValue( int value )
        {
            this.Value = (byte)MathUtilities.Clamp( value, 0, 255 );
        }

        /// <summary>
        /// Called when the IsVisible state of this UIElement has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            buttonDecrease.IsVisible = buttonIncrease.IsVisible = this.IsVisible;
        }

        /// <summary>
        /// Called when the IsEnabled state of this UIElement has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            buttonDecrease.IsEnabled = buttonIncrease.IsEnabled = this.IsEnabled;
        }

        /// <summary>
        /// Called when this UIElement has been added to the UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            userInterface.AddElement( buttonDecrease );
            userInterface.AddElement( buttonIncrease );
        }

        /// <summary>
        /// Called when this UIElement has been removed from the UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            userInterface.RemoveElement( buttonDecrease );
            userInterface.RemoveElement( buttonIncrease );
        }

        private byte value;
        private bool wasClickedWithinSliderArea;
        private readonly ColorComponent component;
        private readonly TextButton buttonDecrease = CreateButton( "-" );
        private readonly TextButton buttonIncrease = CreateButton( "+" );
    }
}
