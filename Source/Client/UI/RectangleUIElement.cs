
namespace Zelda.UI
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a simple colored rectangle; usually used as a background element.
    /// </summary>
    public sealed class RectangleUIElement : UIElement
    {
        /// <summary>
        /// Gets or sets the color of the rectangle.
        /// </summary>
        public Xna.Color Color { get; set; }

        /// <summary>
        /// Called when this Atom.Xna.UI.UIElement is drawing.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            drawContext.Batch.DrawRect(
               ClientArea,
               Color,
               RelativeDrawOrder
           );
        }

        /// <summary>
        /// Called when this Atom.Xna.UI.UIElement is updating.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( IUpdateContext updateContext )
        {
            // no op
        }
    }
}
