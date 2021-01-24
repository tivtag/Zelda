// <copyright file="BoxModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines theZelda.UI.Items.Boxes.BoxModel class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI.Items.Boxes
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom.Math;

    /// <summary>
    /// Represents an object made of individual <see cref="IBox"/>es.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The BoxModel is used to calculate the size of resulting model.
    /// </remarks>
    internal class BoxModel
    {
        /// <summary>
        /// Gets the size this BoxModel takes up.
        /// </summary>
        public Point2 Size
        {
            get
            {
                return this.size;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BoxModel class.
        /// </summary>
        /// <param name="boxes">
        /// The boxes that make up the new BoxModel.
        /// </param>
        public BoxModel( IList<IBox> boxes )
        {            
            this.boxes = boxes.ToArray();
            this.size = CalculateSize( this.boxes );
        }

        /// <summary>
        /// Calculates the size the given boxes take up.
        /// </summary>
        /// <param name="boxes">
        /// The boxes that make up the BoxModel.
        /// </param>
        /// <returns>
        /// The size the boxes take up.
        /// </returns>
        private static Point2 CalculateSize( IBox[] boxes )
        {
            Point2 area = new Point2();

            for( int i = 0; i < boxes.Length; ++i )
            {
                Point2 size = boxes[i].Size;

                if( size.X > area.X )
                {
                    area.X = size.X;
                }

                area.Y += size.Y;
            }

            return area;
        }

        /// <summary>
        /// Draws the content of this BoxModel.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        internal void Draw( ZeldaDrawContext drawContext )
        {
            int verticalPosition = 0;
            
            for( int i = 0; i < boxes.Length; ++i )
            {
                IBox box = this.boxes[i];

                // Draw Box Debug Rectangles. 
                //drawContext.Batch.Draw(
                //    drawContext.WhiteTexture,
                //    new Microsoft.Xna.Framework.Rectangle( 0, verticalPosition, box.Size.X, box.Size.Y ),
                //    new Microsoft.Xna.Framework.Color(
                //        (verticalPosition / (float)this.size.Y),
                //        (verticalPosition / (float)this.size.Y),
                //        (verticalPosition / (float)this.size.Y)
                //   )
                //);
                box.Draw( verticalPosition, this.size, drawContext );
                verticalPosition += box.Size.Y;
            }
        }

        /// <summary>
        /// The total size all the boxes take up.
        /// </summary>
        private readonly Point2 size;

        /// <summary>
        /// The boxes that make up this BoxModel.
        /// </summary>
        private readonly IBox[] boxes;
    }
}
