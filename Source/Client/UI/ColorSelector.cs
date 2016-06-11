
namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the selected of a RGB color.
    /// </summary>
    public sealed class ColorSelector : UIElement
    {
        /// <summary>
        /// Raised when the Color has changed.
        /// </summary>
        public event EventHandler ColorChanged;

        /// <summary>
        /// Gets or sets the color used by the selector.
        /// </summary>
        public Xna.Color Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.red.Value = value.R;
                this.green.Value = value.G;
                this.blue.Value = value.B;
                this.color = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ColorSelectorclass.
        /// </summary>
        /// <param name="position">
        /// The position of the selector element.
        /// </param>
        /// <param name="name">
        /// The display name of the selector.
        /// </param>
        public ColorSelector( Vector2 position, string name )
            : base( name )
        {
            this.red = new ColorComponentSlider( ColorComponent.Red, position + new Vector2( 0.0f, 15.0f * 1.0f ) );
            this.green = new ColorComponentSlider( ColorComponent.Green, position + new Vector2( 0.0f, 15.0f * 2.0f ) );
            this.blue = new ColorComponentSlider( ColorComponent.Blue, position + new Vector2( 0.0f, 15.0f * 3.0f ) );

            red.ValueChanged += OnValueChanged;
            green.ValueChanged += OnValueChanged;
            blue.ValueChanged += OnValueChanged;

            this.SetTransform( position, Vector2.Zero, new Vector2( red.ClientArea.Width, 15.0f * 4.0f ) );
        }

        private void OnValueChanged( object sender, ChangedValue<byte> e )
        {
            this.color = new Xna.Color( red.Value, green.Value, blue.Value );
            this.ColorChanged.Raise( this );
        }

        /// <summary>
        /// Called when this Atom.Xna.UI.UIElement is drawing.
        /// </summary>
        /// <param name="drawContext">The current Atom.Xna.ISpriteDrawContext.</param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var area = red.ClientArea;
            drawContext.Batch.DrawRect( new RectangleF( area.Left, area.Top - 15.0f, area.Width, 15.0f ), this.color );

            UIFonts.Tahoma9.Draw( this.Name, new Vector2( area.Center.X, area.Top - 15.0f ), TextAlign.Center, ColorHelper.InvertRGB( this.color ), 0.2f, drawContext );
        }

        /// <summary>
        /// Called when this Atom.Xna.UI.UIElement is updating.
        /// </summary>
        /// <param name="updateContext">The current Atom.IUpdateContext.</param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Gets called when the Atom.Xna.UI.UIElement was added to the given Atom.Xna.UI.UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            userInterface.AddElement( red );
            userInterface.AddElement( green );
            userInterface.AddElement( blue );
        }

        /// <summary>
        /// Gets called when the Atom.Xna.UI.UIElement was removed from the given Atom.Xna.UI.UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            userInterface.RemoveElement( red );
            userInterface.RemoveElement( green );
            userInterface.RemoveElement( blue );
        }

        /// <summary>
        /// Called when the IsEnabled state of this UIElement has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            red.IsEnabled = green.IsEnabled = blue.IsEnabled = this.IsEnabled;
        }

        /// <summary>
        /// Called when the IsVisible state of this UIElement has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            red.IsVisible = green.IsVisible = blue.IsVisible = this.IsVisible;
        }

        private Xna.Color color;
        private ColorComponentSlider red, green, blue;
    }
}
