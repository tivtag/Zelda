namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Xna = Microsoft.Xna.Framework;

    public sealed class CheckBox : Button
    {
        public Xna.Color BorderColor { get; set; }
        public Xna.Color BackgroundColor { get; set; }
        public Xna.Color SelectionColor { get; set; }

        private RectangleF InnerArea
        {
            get
            {
                const int InnerOffset = 3;
                var area = this.ClientArea;
                return new RectangleF( area.X + InnerOffset, area.Y + InnerOffset, area.Width - 4, area.Height - 4 );
            }
        }

        public CheckBox()
        {
            this.Size = new Vector2( 18.0f, 18.0f );

            this.BorderColor = Xna.Color.Black.WithAlpha( 150 );
            this.BackgroundColor = Xna.Color.White.WithAlpha( 150 );
            this.SelectionColor = Xna.Color.Red.WithAlpha( 200 );

            this.Clicked += new MouseInputEventHandler( OnClicked );
        }

        private void OnClicked( object sender, ref Xna.Input.MouseState mouseState, ref Xna.Input.MouseState oldMouseState )
        {
            this.IsSelected = !this.IsSelected;
        }

        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            drawContext.Batch.DrawRect( this.ClientArea, BackgroundColor );
            drawContext.Batch.DrawLineRect( (Rectangle)this.ClientArea, BorderColor, 2, 0.1f );

            if( this.IsSelected )
            {
                drawContext.Batch.DrawRect( this.InnerArea, SelectionColor, 0.2f );
            }
            else if( this.IsMouseOver )
            {
                drawContext.Batch.DrawRect( this.InnerArea, SelectionColor.WithAlpha( (byte)(SelectionColor.A * 0.7f) ), 0.2f );
            }
        }

        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }
    }
}
