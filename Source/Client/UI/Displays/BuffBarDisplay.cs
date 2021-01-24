// <copyright file="BuffBarDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.BuffBarDisplay class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System.Diagnostics;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Batches;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an UIElement that visualizes the visible buffs/debuffs that affect the PlayerEntity.
    /// </summary>
    internal sealed class BuffBarDisplay : UIElement
    {
        /// <summary>
        /// The default size of a symbol.
        /// </summary>
        private readonly Point2 DefaultBuffSymbolSize = new Point2( 21, 19 );

        /// <summary>
        /// The color of a visualized cooldowns.
        /// </summary>
        private readonly Xna.Color ColorCooldown = new Xna.Color( 0, 0, 0, 154 );
        
        /// <summary>
        /// Gets or sets to the PlayerEntity whos buffs/debuffs are visualized by this BuffbarDisplay.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player
        {
            get; 
            set; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuffBarDisplay"/> class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Provides a mechanism to visualize the durations of buffs/debuffs.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BuffBarDisplay( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
        {
            Debug.Assert( serviceProvider != null );
            Debug.Assert( cooldownVisualizer != null );

            this.cooldownVisualizer = cooldownVisualizer;
            this.Size               = serviceProvider.ViewSize;

            this.PassInputToSubElements = true;
        }
        
        /// <summary>
        /// Called when this <see cref="BuffBarDisplay"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// THe current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Gets called when this <see cref="BuffBarDisplay"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;
            const int BorderSize = 5;

            var auras = this.Player.Statable.AuraList.VisibleAuras;
            if( auras == null )
                return;

            Vector2 position = new Vector2( this.Width - BorderSize, BorderSize );
            var     batch    = drawContext.Batch;

            // Draw Buffs:
            for( int i = 0; i < auras.Count; ++i )
            {
                var aura = auras[i];
                if( !aura.IsDebuff )
                {
                    position = DrawAura( aura, position, batch );
                }
            }

            // Move position down:
            position.X  = this.Width - BorderSize;
            position.Y += DefaultBuffSymbolSize.Y + 2;

            // Draw Debuffs:
            for( int i = 0; i < auras.Count; ++i )
            {
                var aura = auras[i];
                if( aura.IsDebuff )
                {
                    position = DrawAura( aura, position, batch );
                }
            }
        }

        /// <summary>
        /// Draws the given buff or debuff Aura.
        /// </summary>
        /// <param name="aura">
        /// The aura to draw.
        /// </param>
        /// <param name="position">
        /// The position of the aura to draw.
        /// </param>
        /// <param name="batch">
        /// The XNA SpriteBatch object.
        /// </param>
        /// <returns>
        /// The new position.
        /// </returns>
        private Vector2 DrawAura( Aura aura, Vector2 position, IComposedSpriteBatch batch )
        {
            const int GabBetweenBuffs = 2;
            const float SpriteDepth = 0.1f;

            Sprite  sprite = aura.Symbol;
            Vector2 symbolDimensions = new Vector2();

            if( sprite == null )
            {
                symbolDimensions.X = DefaultBuffSymbolSize.X;
                symbolDimensions.Y = DefaultBuffSymbolSize.Y;

                position.X -= symbolDimensions.X;
                position.X -= GabBetweenBuffs;

                // Draw pink debug rectangle: :)
                batch.DrawRect(
                    new Xna.Rectangle( (int)position.X, (int)position.Y, DefaultBuffSymbolSize.X, DefaultBuffSymbolSize.Y ),
                    Xna.Color.Pink,
                    SpriteDepth 
                );
            }
            else
            {
                symbolDimensions.X = sprite.Width;
                symbolDimensions.Y = sprite.Height;

                position.X -= symbolDimensions.X;
                position.X -= GabBetweenBuffs;

                sprite.Draw( position, aura.SymbolColor, SpriteDepth, batch );
            }

            TimedAura timedAura = aura as TimedAura;

            if( timedAura != null )
            {
                cooldownVisualizer.PushCooldown(
                    timedAura.Cooldown,
                    new Vector2( position.X, position.Y ),
                    new Vector2( symbolDimensions.X, symbolDimensions.Y ),
                    ColorCooldown,
                    true 
                );
            }

            return position;
        }
        
        /// <summary>
        /// Provides a mechanism to visualize the duration of timed buffs/debuffs.
        /// </summary>
        private readonly CooldownVisualizer cooldownVisualizer;
    }
}
