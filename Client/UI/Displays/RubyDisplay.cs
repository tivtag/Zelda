// <copyright file="RubyDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.RubyDisplay class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Defines an UIElement that visualizes how much rubies the player owns.
    /// </summary>
    internal sealed class RubyDisplay : UIElement
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="Player"/> object.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player
        {
            get;
            set;
        }

        public bool ShowRubyValueThisFrame 
        {
            get 
            { 
                return this.showRubyValue;
            }
            
            set
            {
                this.showRubyValue = value; 
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RubyDisplay"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal RubyDisplay( IZeldaServiceProvider serviceProvider )
        {
            this.spriteRuby = serviceProvider.SpriteLoader.LoadSprite( "RubyUI" );

            var viewSize = serviceProvider.ViewSize;
            this.Offset   = new Vector2( 0.0f, -spriteRuby.Height - 5.0f );
            this.Position = new Vector2( viewSize.X - 20, viewSize.Y - 14 );
            this.Size     = new Vector2( spriteRuby.Width + 6.0f, spriteRuby.Height + 5.0f );

            this.cachedRubyStringSize = font.MeasureString( cachedRubyString );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this UIElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;

            this.spriteRuby.Draw( 
                new Vector2( (int)X + 3, (int)Y - spriteRuby.Height - 5.0f ),
                IsMouseOver ? Xna.Color.Silver : Xna.Color.White,
                drawContext.Batch
            );

            if( this.showRubyValue || this.isCountShownPerma )
            {
                this.font.Draw(
                    this.cachedRubyString,
                    new Vector2( this.X - this.cachedRubyStringSize.X, this.Y - this.cachedRubyStringSize.Y ),
                    Xna.Color.White,
                    drawContext
                );
            }
        }

        /// <summary>
        /// Called when this UIElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            if( this.Player == null )
                return;
        
            long rubies = this.Player.Statable.Rubies;

            if( cachedRubyCount != rubies )
            {
                this.cachedRubyCount      = rubies;
                this.cachedRubyString     = this.cachedRubyCount.ToString( System.Globalization.CultureInfo.CurrentCulture );
                this.cachedRubyStringSize = font.MeasureString( this.cachedRubyString );
            }
        }

        /// <summary>
        /// Called before an update happens to this UIElement.
        /// </summary>
        protected override void OnPreUpdate()
        {
            this.showRubyValue = false;
        }

        /// <summary>
        /// Called when the mouse is over the Atom.Xna.UI.UIElement.ClientArea of this Atom.Xna.UI.UIElement.
        /// </summary>
        /// <param name="mouseState">The state of the mouse. Passed by reference to reduce overhead.</param>
        /// <returns>
        /// True if input should be passed to elements that are behind the Atom.Xna.UI.UIElement,
        /// otherwise false.
        /// </returns>
        protected override bool OnMouseOver( ref Microsoft.Xna.Framework.Input.MouseState mouseState )
        {
            this.showRubyValue = true;
            return true;
        }

        /// <summary>
        /// Called when the user clicks on this UIElement.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        /// <returns>
        /// Whether input should be passed to UIElements behing this UIElement.
        /// </returns>
        protected override bool HandleRelatedMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState, 
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released )
            {
                this.isCountShownPerma = !this.isCountShownPerma;
            }

            return true;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whether to show the ruby value.
        /// </summary>
        private bool showRubyValue;

        /// <summary>
        /// Stores what ruby count is currently cached.
        /// </summary>
        private long cachedRubyCount;

        /// <summary>
        /// The cached ruby count string.
        /// </summary>
        private string cachedRubyString = "0";

        /// <summary>
        /// The dimensions of the currently cached ruby count string.
        /// </summary>
        private Vector2 cachedRubyStringSize;

        /// <summary>
        /// States whether the count of the RubyDisplay is shown permanently.
        /// </summary>
        private bool isCountShownPerma = Settings.Instance.RubyCountShown;

        /// <summary>
        /// The sprite of the ruby.
        /// </summary>
        private readonly Sprite spriteRuby;

        /// <summary>
        /// The font used to display the sprite count.
        /// </summary>
        private readonly IFont font = UIFonts.TahomaBold10;

        #endregion
    }
}
