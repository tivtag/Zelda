// <copyright file="QuickActionSlotDrawer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Ocarina.QuickActionSlotDrawer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.QuickActions;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Encapsulates the process of drawing a Quick Action Slot.
    /// </summary>
    internal sealed class QuickActionSlotDrawer
    {
        #region [ Constants ]

        /// <summary>
        /// The size of a QuickActionSlot in pixels.
        /// </summary>
        private const int SlotWidth = 21, SlotHeight = 19;

        /// <summary>
        ///  Specifies the color of a quick action that is not useable.
        /// </summary>
        private static readonly Xna.Color ColorUnuseableAction = new Xna.Color( 255, 0, 0, 154 );

        /// <summary>
        ///  Specifies the color of a quick action that is not useable.
        /// </summary>
        private static readonly Xna.Color ColorInactiveAction = new Xna.Color( 0, 0, 0, 100 );

        #endregion
        
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the QuickActionSlotDrawer class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Allows the visualization of <see cref="Cooldown"/> data.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public QuickActionSlotDrawer(  CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
        {
            this.defaultSprite = serviceProvider.SpriteLoader.LoadSprite( "Symbol_Empty" );
            this.cooldownVisualizer = cooldownVisualizer;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Visualizes the given IQuickAction.
        /// </summary>
        /// <param name="quickAction">The IQuickAction to draw.</param>
        /// <param name="drawPosition">The drawing position.</param>
        /// <param name="drawContext">The current IDrawContext.</param>
        public void Draw( IQuickAction quickAction, Vector2 drawPosition, ZeldaDrawContext drawContext )
        {
            this.DrawSymbol( quickAction, drawPosition, 0.0f, drawContext );
            this.DrawCooldown( quickAction, drawPosition );
            DrawIndicators( quickAction, drawPosition, drawContext );
        }

        /// <summary>
        /// Draws the symbol sprite of the IQuickAction.
        /// </summary>
        /// <param name="quickAction">The IQuickAction this operation works on.</param>
        /// <param name="drawPosition">The drawing position.</param>
        /// <param name="layerDepth">The layer depth value.</param>
        /// <param name="drawContext">The current IDrawContext.</param>
        public void DrawSymbol( IQuickAction quickAction, Vector2 drawPosition, float layerDepth, ZeldaDrawContext drawContext )
        {
            if( HasDefaultSize( quickAction.Symbol ) )
            {
                quickAction.Symbol.Draw( drawPosition, quickAction.SymbolColor, layerDepth, drawContext.Batch );
            }
            else
            {
                ISprite symbol = quickAction.Symbol;

                Vector2 center = drawPosition + new Vector2(
                    (int)((SlotWidth / 2) - (symbol.Width / 2)),
                    (int)((SlotHeight / 2) - (symbol.Height / 2))
                );

                defaultSprite.Draw( drawPosition, layerDepth, drawContext.Batch );
                quickAction.Symbol.Draw( center, quickAction.SymbolColor, layerDepth + 0.025f, drawContext.Batch );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given Sprite has the deafult size of a quick slot.
        /// </summary>
        /// <param name="sprite">
        /// The sprite shown in the QuickActionSlot.
        /// </param>
        /// <returns>
        /// true if the Sprite has the default size;
        /// otherwise false.
        /// </returns>
        private static bool HasDefaultSize( Atom.Xna.ISprite sprite )
        {
            return sprite.Width == SlotWidth && sprite.Height == SlotHeight;
        }

        /// <summary>
        /// Draws the cooldown of the QuickAction; if there is any.
        /// </summary>
        /// <param name="quickAction">The IQuickAction this operation works on.</param>
        /// <param name="drawPosition">The drawing position.</param>
        private void DrawCooldown( IQuickAction quickAction, Vector2 drawPosition )
        {
            if( quickAction.CooldownLeft > 0.0f )
            {
                this.cooldownVisualizer.PushCooldown(
                    quickAction.CooldownLeft,
                    quickAction.CooldownTotal,
                    drawPosition + 1.0f,
                    new Vector2( 19.0f, 17.0f ),
                    UIColors.Cooldown,
                    false
                );
            }
        }

        /// <summary>
        /// Draws additional useability indicators ontop of the rest.
        /// </summary>
        /// <param name="quickAction">The IQuickAction this operation works on.</param>
        /// <param name="drawPosition">The drawing position.</param>
        /// <param name="drawContext">The current IDrawContext.</param>
        private static void DrawIndicators( IQuickAction quickAction, Vector2 drawPosition, ZeldaDrawContext drawContext )
        {
            if( !quickAction.IsExecuteable )
            {
                if( !quickAction.IsActive )
                {
                    drawContext.Batch.DrawRect(
                        new Xna.Rectangle( (int)drawPosition.X + 1, (int)drawPosition.Y + 1, 19, 17 ),
                        ColorInactiveAction,
                        0.1f
                    );
                }
                else
                {
                    if( IsLimitedByAnythingButCooldown( quickAction ) )
                    {
                        drawContext.Batch.DrawRect(
                            new Xna.Rectangle( (int)drawPosition.X + 1, (int)drawPosition.Y + 1, 19, 17 ),
                            ColorUnuseableAction,
                            0.1f
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given IQuickAction is not 
        /// useable and limited by something else rather than its cooldown.
        /// </summary>
        /// <param name="quickAction">
        /// The IQuickAction this operation works on.
        /// </param>
        /// <returns></returns>
        private static bool IsLimitedByAnythingButCooldown( IQuickAction quickAction )
        {
            return !quickAction.IsOnlyLimitedByCooldown && quickAction.CooldownLeft <= 0.0f;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The sprite that is used to draw a QuickActionSlot if
        /// no specific Sprite is specified.
        /// </summary>
        private readonly Sprite defaultSprite;

        /// <summary>
        /// Allows the visualization of <see cref="Cooldown"/> data.
        /// </summary>
        private readonly CooldownVisualizer cooldownVisualizer;

        #endregion
    }
}
