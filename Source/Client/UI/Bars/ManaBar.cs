// <copyright file="ManaBar.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.ManaBar class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Visualizes the mana status of the player.
    /// </summary>
    internal sealed class ManaBar : UIElement
    {
        /// <summary>
        /// The color of the mana.
        /// </summary>
        private readonly Xna.Color Color = new Xna.Color( 48, 104, 216 );

        /// <summary>
        /// The color of the empty mana.
        /// </summary>
        private readonly Xna.Color ColorEmpty = new Xna.Color( 107, 66, 82 );

        /// <summary> 
        /// Gets or sets the PlayerEntity whos Life State
        /// is visualized by the <see cref="ManaBar"/>. 
        /// </summary>
        public PlayerEntity Player
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ManaBar"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ManaBar( IZeldaServiceProvider serviceProvider )
            : base( "ManaBar" )
        {
            this.Position = new Vector2( 4.0f, 28.0f );
            this.spriteBar = serviceProvider.SpriteLoader.LoadSprite( "ManaBar" );

            this.Size = new Vector2( this.spriteBar.Width, this.spriteBar.Height );
        }

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

            int x = (int)this.Position.X, 
                y = (int)this.Position.Y;

            var batch    = drawContext.Batch;
            var statable = this.Player.Statable;

            float factor   = (float)statable.Mana / (float)statable.MaximumMana;
            int manaPixels = (int)(factor * (this.spriteBar.Width - 4));

            // 1. Draw Empty Mana
            batch.DrawRect( 
                new Xna.Rectangle( x + 4, y + 4, this.spriteBar.Width - 7, this.spriteBar.Height - 4 ),
                ColorEmpty
            );

            // 2. Draw Full Mana ontop
            batch.DrawRect( 
                new Xna.Rectangle( x + 4, y + 4, manaPixels - 3, this.spriteBar.Height - 4 ),
                Color,
                0.0001f
            );

            // 3. Draw Sprite
            spriteBar.Draw( new Vector2( x, y ), 0.0002f, batch );
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

        /// <summary>
        /// The sprite that is used to visualize the ManaBar.
        /// </summary>
        private readonly Sprite spriteBar;
    }
}