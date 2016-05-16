
namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna.UI.Controls;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a Button that is used to navigate back and forth in e.g. a wizard-style UI.
    /// </summary>
    public sealed class NavButton : SpriteTextButton
    {
        /// <summary>
        /// Enumerates the different modes that the NavButton can be in.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// A disabled NavButton.
            /// </summary>
            None,

            /// <summary>
            /// The NavButton navigates backwards.
            /// </summary>
            Back,

            /// <summary>
            /// The NavButton navigates forwards.
            /// </summary>
            Next,

            /// <summary>
            /// The NavButton completes the process.
            /// </summary>
            Complete
        }

        /// <summary>
        /// Gets or sets the current Mode of the NavButton.
        /// </summary>
        public Mode ButtonMode
        {
            get
            {
                return this.mode;
            }

            set
            {
                switch( value )
                {
                    case Mode.Back:
                        Text = "<";
                        TextOffset = new Vector2( 1, -2 );
                        break;

                    case Mode.Next:
                        Text = ">";
                        TextOffset = new Vector2( 3, -2 );
                        break;

                    case Mode.Complete:                        
                        Text = "X";
                        TextOffset = new Vector2( 4, -1 );
                        break;

                    case Mode.None:
                    default:
                        break;
                }

                this.mode = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the NavButton class.
        /// </summary>
        /// <param name="name">The name fo the button.</param>
        /// <param name="serviceProvider">Provides fast access to game-related services.</param>
        public NavButton( string name, IZeldaServiceProvider serviceProvider )
            : base( name, serviceProvider.SpriteLoader.LoadSprite( "Button_Small_Dark_Default" ), serviceProvider.SpriteLoader.LoadSprite( "Button_Small_Dark_Selected" ) )
        {
            Position = new Vector2( 3, serviceProvider.ViewSize.Y - 23 );
            FloorNumber = 10;
            Font = UIFonts.Tahoma14;
            ColorTextSelected = Xna.Color.Black;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this NavButton is currently enabled and useable.
        /// </summary>
        /// <param name="useable">true if the button is useable; -or- otherwise false.</param>
        public void SetUseable( bool useable )
        {
            if( useable )
            {
                ColorDefault = ColorTextDefault = Xna.Color.White;
                IsEnabled = true;
            }
            else
            {
                ColorDefault = ColorTextDefault = new Xna.Color( 255, 55, 55, 100 );
                IsEnabled = false;
            }
        }

        private Mode mode;
    }
}
