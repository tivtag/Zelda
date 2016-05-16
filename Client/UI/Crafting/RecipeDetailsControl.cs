// <copyright file="RecipeDetailsControl.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Crafting.RecipeDetailsControl class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Crafting
{
    using System.Globalization;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Zelda.Crafting;
    using Zelda.UI.Items;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents an UIElement that showns detail information about
    /// a single Recipe.
    /// </summary>
    internal sealed class RecipeDetailsControl : UIElement
    {
        /// <summary>
        /// Get or sets the Recipe whose details are shown in this RecipeDetailsControl.
        /// </summary>
        public Recipe Recipe
        {
            get
            {
                return this.recipe;
            }

            set
            {
                this.recipe = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Initializes a new instance of the RecipeDetailsControl class.
        /// </summary> 
        /// <param name="tooltipDrawElement">
        /// The IooltipDrawElement responsible for drawing the actual item information
        /// when player moves the mouse over any of the ItemTooltip the new ItemTooltipGroup contains.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RecipeDetailsControl( ItemTooltipDrawElement tooltipDrawElement, IZeldaServiceProvider serviceProvider )
        {
            this.itemTooltips = new ItemTooltipGroup( tooltipDrawElement ) {
                FloorNumber = 5,
                RelativeDrawOrder = 0.5f
            };

            this.descriptionText = new Text( UIFonts.Tahoma7, TextAlign.Center, Xna.Color.LightGray, new TextBlockSplitter( UIFonts.Tahoma7, 180.0f ) ) {
                LayerDepth = 0.5f
            };

            this.FloorNumber = 4;
            this.SetTransform( new Vector2( 5, 25 ), Vector2.Zero, new Vector2( 180.0f, 210.0f ) );
            this.itemTooltips.SetTransform( this.Position, Vector2.Zero, this.Size );
            this.HideAndDisable();
        }

        /// <summary>
        /// Refreshes the content of this RecipeDetailsControl,
        /// based on the currently set Recipe.
        /// </summary>
        private void Refresh()
        {
            this.itemTooltips.Clear();

            if( this.recipe != null )
            {
                this.descriptionText.TextString = this.recipe.Description;

                Vector2 position = new Vector2( 25.0f, this.descriptionText.TextBlockSize.Y + (string.IsNullOrEmpty( this.recipe.Description ) ? 45.0f : 60.0f) );

                foreach( RequiredRecipeItem recipeItem in this.recipe.Required )
                {
                    var tooltip = this.itemTooltips.AddNew( recipeItem.Item );
                    tooltip.Tag = recipeItem;
                    tooltip.Position = position + new Vector2( 0.0f, tooltip.Size.Y / 2.0f );
                    tooltip.ShowAndEnable();

                    position.Y += tooltip.Size.Y + 5;
                }
            }
            else
            {
                this.descriptionText.TextString = string.Empty;
            }
        }

        /// <summary>
        /// Called when this RecipeDetailsControl is updating itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            if( this.Recipe == null )
                return;

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            drawContext.Batch.DrawRect(
                this.ClientArea.ToXna(),
                UIColors.WindowTitleBackground
            );

            this.DrawName( zeldaDrawContext );
            this.DrawDescription( zeldaDrawContext );
            this.DrawRequiredItems( drawContext );
        }

        private void DrawDescription( ZeldaDrawContext zeldaDrawContext )
        {
            this.descriptionText.Draw( new Vector2( this.X + this.Width / 2, 50.0f ), zeldaDrawContext );
        }

        /// <summary>
        /// Draws the items required by the current Recipe.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawRequiredItems( ISpriteDrawContext drawContext )
        {
            foreach( var tooltip in this.itemTooltips )
            {
                var recipeItem = (RequiredRecipeItem)tooltip.Tag;
                this.fontRecipeItemCount.Draw(
                    "x " + recipeItem.Amount.ToString( CultureInfo.CurrentCulture ),
                    new Vector2( 50.0f, (int)(tooltip.ClientArea.Bottom - this.fontRecipeItemCount.LineSpacing + 1) ),
                    Xna.Color.White,
                    0.45f,
                    drawContext
                );

                tooltip.DrawItemSprite( drawContext );
            }
        }

        /// <summary>
        /// Draws the name of the recipe.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawName( ZeldaDrawContext drawContext )
        {
            var largeFont = UIFonts.Tahoma10;
            var smallFont = UIFonts.Tahoma7;
            int rectHeight = largeFont.LineSpacing + 2;

            // Background
            drawContext.Batch.DrawRect(
                new Xna.Rectangle(
                    (int)this.X,
                    (int)this.Y,
                    (int)this.Width,
                    rectHeight
                ),
                UIColors.WindowTitleBackground,
                0.25f
            );

            string name = this.recipe.Result.Item.LocalizedName;

            IFont font;
            int y;
            if( largeFont.MeasureStringWidth( name ) - 10.0f > this.Width )
            {
                font = smallFont;
                y = 30;
            }
            else
            {
                font = largeFont;
                y = 26;
            }

            // Text
            font.Draw(
                name,
                new Vector2( this.X + this.Width / 2, y ),
                Atom.Xna.Fonts.TextAlign.Center,
                Xna.Color.White,
                0.45f,
                drawContext
            );
        }

        /// <summary>
        /// Called when this RecipeDetailsControl is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // no op.
        }

        /// <summary>
        /// Called when this RecipeDetailsControl has been added to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.itemTooltips );
        }

        /// <summary>
        /// Called when this RecipeDetailsControl has been removed from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.itemTooltips );
        }

        /// <summary>
        /// Called when the IsEnabled state of this RecipeDetailsControl has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.itemTooltips.IsEnabled = this.IsEnabled;
            this.itemTooltips.IsVisible = this.IsEnabled;

            base.OnIsEnabledChanged();
        }

        private Text descriptionText;

        /// <summary>
        /// Contains the ItemTooltips this RecipeDetailsControl contains.
        /// </summary>
        private readonly ItemTooltipGroup itemTooltips;

        /// <summary>
        /// Represents the font used to draw the number of times an item is required by the recipe.
        /// </summary>
        private readonly Atom.Xna.Fonts.IFont fontRecipeItemCount = UIFonts.Tahoma7;

        /// <summary>
        /// Represents the storage field of the Recipe property.
        /// </summary>
        private Recipe recipe;
    }
}
