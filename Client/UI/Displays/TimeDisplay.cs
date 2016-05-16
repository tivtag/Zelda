// <copyright file="TimeDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.TimeDisplay class.
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
    /// Visualizes the current ingame time.
    /// </summary>
    internal sealed class TimeDisplay : UIElement
    {
        #region [ Constants ]
        
        /// <summary>
        /// The color of the displayed text.
        /// </summary>
        private readonly Xna.Color TextColor = Xna.Color.White;

        #endregion           
        
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IngameDateTime"/> object,
        /// which captures the current date and time within the game.
        /// </summary>
        public IngameDateTime DateTime
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeDisplay"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal TimeDisplay( IZeldaServiceProvider serviceProvider )
        {
            var viewSize = serviceProvider.ViewSize;
            this.Position = new Atom.Math.Vector2( viewSize.X - 4, viewSize.Y );
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
            if( !this.IsVisible || this.DateTime == null )
                return;

            string dateTimeString = this.DateTime.ToShortTimeString();
            var stringSize        = font.MeasureString( dateTimeString );

            this.font.Draw(
                dateTimeString,
                new Vector2( this.Position.X - stringSize.X, this.Position.Y - stringSize.Y ),
                TextColor,
                0.0f,
                Vector2.Zero,
                1.0f, 
                SpriteEffects.None,
                0.0f,
                drawContext
            );
        }

        /// <summary>
        /// Called when this UIElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // no op.
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The font used to display the date/time.
        /// </summary>
        private readonly IFont font = UIFonts.TahomaBold10;

        #endregion
    }
}