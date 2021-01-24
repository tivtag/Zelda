// <copyright file="MerchantItemElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Trading.MerchantItemElement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Trading
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Zelda.Trading;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Visualizes a single <see cref="MerchantItem"/>.
    /// </summary>
    public sealed class MerchantItemElement : UIContainerElement
    {
        #region [ Constants ]

        /// <summary>
        /// The width of a single MerchantItemElement.
        /// </summary>
        public const int ElementWidthDefault = 18 * 5, ElementWidthSmall = (18 * 4) + 3;
        
        private const int BorderSize = 1;
        private static readonly Xna.Color BorderColor = Xna.Color.Black;

        private static readonly Xna.Color DefaultBackgroundColor = new Xna.Color( 0, 0, 0, 150 );
        private static readonly Xna.Color SelectedBackgroundColor = new Xna.Color( 255, 255, 255, 150 );

        private static readonly Xna.Color DefaultFontColor = Xna.Color.White;
        private static readonly Xna.Color SelectedFontColor = Xna.Color.Black;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the MerchantItem this MerchantItemElement visualizes.
        /// </summary>
        public MerchantItem MerchantItem
        {
            get
            {
                return this.container.MerchantItem;
            }

            set
            {
                this.container.MerchantItem = value;
                this.InitializeForItem();
            }
        }

        /// <summary>
        /// Gets or sets the width of this element.
        /// </summary>
        public int ElementWidth
        {
            get
            {
                return this.elementWidth;
            }

            set
            {
                if( elementWidth == value )
                    return;

                this.elementWidth = value;
                this.InitializeForItem();
            }
        }

        /// <summary>
        /// Gets or sets the entity that wants to buy this MerchantItemElement.
        /// </summary>
        public Zelda.Entities.PlayerEntity Buyer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the aligment of the content within this MerchantItemElement.
        /// </summary>
        public HorizontalDirection Alignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this MerchantItemElement is selected.
        /// </summary>
        public bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the area of the MerchantItemContainer of this MerchantItemElement.
        /// </summary>
        public RectangleF ContainerArea
        {
            get
            {
                return this.container.ClientArea;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MerchantItemElement class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public MerchantItemElement( IZeldaServiceProvider serviceProvider )
            : base( 1 )
        {
            this.FloorNumber = 3;
            this.Alignment = HorizontalDirection.Left;

            this.spriteRuby = serviceProvider.SpriteLoader.LoadSprite( "RubyUI2" );
            
            this.container = new MerchantItemContainer( serviceProvider );
            this.AddChild( this.container );
        }
        
        /// <summary>
        /// Initializes this MerchantItemElement for the current MerchantItem.
        /// </summary>
        private void InitializeForItem()
        {
            if( this.MerchantItem == null )
            {
                this.Size = Vector2.Zero;
            }
            else
            {
                this.Size = new Vector2(
                    elementWidth,
                    this.container.Size.Y
                );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Positions this MerchantItemElement at the given position.
        /// </summary>
        /// <param name="position">
        /// The position this MerchantItemElement should be positioned at.
        /// </param>
        internal void PositionAt( Vector2 position )
        {
            this.Position = position;

            if( this.Alignment == HorizontalDirection.Left )
            {
                this.container.Position = position;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Called when this <see cref="MerchantItemElement"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            this.DrawBackground( zeldaDrawContext );
            this.DrawBorders( zeldaDrawContext );

            this.DrawQuantity( zeldaDrawContext );
            this.DrawPrice( zeldaDrawContext );
        }

        /// <summary>
        /// Draws the background of this MerchantItemElement.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawBackground( ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect(
                this.ClientArea.ToXna(),
                this.GetBackgroundColor()
            );
        }

        /// <summary>
        /// Draws the borders around this MerchantItemElement.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawBorders( ZeldaDrawContext drawContext )
        {
            // top
            DrawBorder(
                new Rectangle(
                    (int)this.X - 1,
                    (int)(this.Y - 1),
                    elementWidth + 2,
                    BorderSize
                ),
                drawContext
            );

            // bottom
            DrawBorder(
                new Rectangle(
                    (int)this.X - 1,
                    (int)(this.Y + this.Size.Y),
                    elementWidth + 2,
                    BorderSize
                ),
                drawContext 
            );
            
            // left
            DrawBorder(
                new Rectangle(
                    (int)this.X - 1,
                    (int)this.Y,
                    1,
                    (int)this.Size.Y
                ),
                drawContext
            );

            // right
            DrawBorder(
                new Rectangle(
                    (int)(this.X + this.Width),
                    (int)this.Y,
                    1,
                    (int)this.Size.Y
                ),
                drawContext
            );
        }

        /// <summary>
        /// Draws a border at the given rectangle.
        /// </summary>
        /// <param name="rectangle">
        /// The area of the border.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private static void DrawBorder( Rectangle rectangle, ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect(
                rectangle,
                BorderColor,
                0.1f
            );
        }

        /// <summary>
        /// Gets the background color of this MerchantItemElement.
        /// </summary>
        /// <returns></returns>
        private Xna.Color GetBackgroundColor()
        {
            return this.IsSelected ? SelectedBackgroundColor : DefaultBackgroundColor;
        }

        /// <summary>
        /// Draws the stock count of the MerchantItem.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawQuantity( ZeldaDrawContext drawContext )
        {
            int stockCount = this.MerchantItem.StockCount;
            if( stockCount == 1 )
                return;

            if( this.Alignment == HorizontalDirection.Left )
            {
                string stockCountString = "x" + stockCount.ToString();

                this.fontItemCount.Draw(
                    stockCountString,
                    new Vector2( 
                        (int)(this.X + container.Size.X + 1),
                        (int)(this.Y + this.Size.Y - this.fontItemCount.LineSpacing + 2)
                    ),
                    this.GetFontColor(),
                    0.1f,
                    drawContext
                );
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Draws the price of the MerchantItem.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawPrice( ZeldaDrawContext drawContext )
        {
            int price = this.MerchantItem.GetFinalPrice( this.Buyer );
            var batch = drawContext.Batch;

            if( this.Alignment == HorizontalDirection.Left )
            {
                this.fontItemCount.Draw(
                    price.ToString(),
                    new Vector2(
                        this.X + elementWidth - spriteRuby.Width - 2.0f,
                        this.Y + this.Size.Y - this.fontItemCount.LineSpacing + 2
                    ),
                    TextAlign.Right,
                    this.GetPriceFontColor(),
                    0.1f,
                    drawContext
                );

                this.spriteRuby.Draw(
                    new Vector2(
                        this.X + elementWidth - spriteRuby.Width - 1.0f,
                        this.Y + this.Size.Y - spriteRuby.Height - 1.0f
                    ),
                    0.11f,
                    batch
                );
            }
            else 
            {
            }
        }

        /// <summary>
        /// Gets the font color for drawing the price of this MerchantItemElement.
        /// </summary>
        /// <returns></returns>
        private Xna.Color GetPriceFontColor()
        {
            if( this.MerchantItem.HasRequiredRubies( this.Buyer ) )
            {
                return this.GetFontColor();
            }
            else
            {
                return UIColors.RequirementNotFulfilled;
            }
        }

        /// <summary>
        /// Gets the font color of this MerchantItemElement.
        /// </summary>
        /// <returns></returns>
        private Xna.Color GetFontColor()
        {
            return this.IsSelected ? SelectedFontColor : DefaultFontColor;
        }

        /// <summary>
        /// Called when this <see cref="MerchantItemElement"/> is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        #endregion

        #region [ Fields ]

        private int elementWidth = ElementWidthDefault;

        /// <summary>
        /// The UIElement that draw the actual item.
        /// </summary>
        private readonly MerchantItemContainer container;
        
        /// <summary>
        /// The sprite that represents a ruby.
        /// </summary>
        private readonly Sprite spriteRuby;

        /// <summary>
        /// The IFont that is used when drawing the ItemCount.
        /// </summary>
        private readonly IFont fontItemCount = UIFonts.Tahoma7;

        #endregion
    }
}
