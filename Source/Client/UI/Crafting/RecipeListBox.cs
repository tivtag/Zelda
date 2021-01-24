// <copyright file="RecipeListBox.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Crafting.RecipeListBox class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Crafting
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Controls;
    using Zelda.Crafting;
    using Zelda.Graphics;
    using Zelda.Items;
    using Zelda.UI.Items;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a scrollable and virtualizing list of recipes.
    /// </summary>
    internal sealed class RecipeListBox : ListBox<Recipe>
    {
        private const int SliderWidth = 12;
        private const int ItemHeight = 22;
        private static int ListWidth = 125;
        private readonly int ListHeight = ItemHeight * 7;
        private const int ListY = 53;
        private static readonly Xna.Color ColorBackground = Xna.Color.Black;

        /// <summary>
        /// Initializes a new instance of the RecipeListBox class.
        /// </summary>
        /// <param name="tooltipDrawElement">
        /// The IooltipDrawElement responsible for drawing the actual item information
        /// when player moves the mouse over any of the ItemTooltip the new RecipeListBox contains.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RecipeListBox( ItemTooltipDrawElement tooltipDrawElement, IZeldaServiceProvider serviceProvider )
            : base( new ZeldaScrollBar() )
        {
            IResolutionService resolution = serviceProvider.GetService<IResolutionService>();
            
            Point2 viewSize = resolution.ViewSize;
            //ListHeight = MathUtilities.GetNearestMul( viewSize.Y - ListY - 5, ItemHeight );

            if( resolution.IsWideAspectRatio )
            {
                ListWidth = 160;
            }

            this.Orientation = Orientation.Vertical;
            this.SetTransform( new Vector2( viewSize.X - ListWidth - 40, ListY ), Vector2.Zero, new Vector2( ListWidth, ListHeight ) );

            this.itemManager = serviceProvider.ItemManager;
            this.itemTooltips = new ItemTooltipGroup( tooltipDrawElement ) { FloorNumber = 3, RelativeDrawOrder = 0.75f };
            this.itemTooltips.SetTransform( this.Position, Vector2.Zero, this.Size );

            this.ScrollBar.Position = new Vector2( this.Position.X + this.Size.X, ListY - ItemHeight );
            this.ScrollBar.Size = new Vector2( SliderWidth, ListHeight + ItemHeight * 2 );

            this.HideAndDisable();
        }
        /// <summary>
        /// Shows the specified Recipes in this RecipesListBox.
        /// </summary>
        /// <param name="recipes">
        /// The recipes to show.
        /// </param>
        public void ShowRecipes( IEnumerable<Recipe> recipes )
        {
            this.ClearItems();

            foreach( var recipe in recipes )
            {
                this.AddItem( recipe );
            }

            this.Refresh();
        }
        
        /// <summary>
        /// Removes all items from this RecipesListBox.
        /// </summary>
        public override void ClearItems()
        {
            base.ClearItems();
            this.itemTooltips.Clear();
        }

        /// <summary>
        /// Called when this RecipeListBox is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {           
            foreach( var item in this.Items )
            {
                item.Draw( drawContext );
            }

            if( this.ScrollBar.IsVisible )
            {
                this.DrawScrollBorders( drawContext );
            }
        }

        /// <summary>
        /// Draws the borders on the top/bottom that hide the items that are
        /// halfway 'scrolling' in or out.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawScrollBorders( ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            
            // Top Cover
            drawContext.Batch.DrawRect(
                new RectangleF(
                    this.Position - new Vector2( 0.0f, ItemHeight ),
                    new Vector2( ListWidth, ItemHeight )
                ),
                ColorBackground,
                1.0f
            );

            // Bottom Cover
            drawContext.Batch.DrawRect(
                new RectangleF(
                    this.Position + new Vector2( 0.0f, ListHeight ),
                    new Vector2( ListWidth, ItemHeight )
                ).ToXna(),
                ColorBackground,
                1.0f
            );
        }

        /// <summary>
        /// Called when this RecipeListBox is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            foreach( var item in this.Items )
            {
                item.Update( updateContext );
            }
        }

        /// <summary>
        /// Called when this RecipeListBox has been added to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.itemTooltips );
            base.OnAdded( userInterface );
        }

        /// <summary>
        /// Called when this RecipeListBox has been removed from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.itemTooltips );
            base.OnRemoved( userInterface );
        }

        /// <summary>
        /// Called when the IsEnabled state of this RecipeListBox has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.itemTooltips.IsEnabled = this.IsEnabled;
            this.itemTooltips.IsVisible = this.IsEnabled;

            base.OnIsEnabledChanged();
        }

        /// <summary>
        /// Creates an new instance of the RecipeListItem class that contains the given Recipe.
        /// </summary>
        /// <param name="recipe">
        /// The input Recipe.
        /// </param>
        /// <returns>
        /// The newly created RecipeListItem.
        /// </returns>
        protected override ListItem<Recipe> CreateItem( Recipe recipe )
        {
            return new RecipeListItem( this ) {
                Value = recipe,
                Size = new Vector2( ListWidth, ItemHeight )
            };
        }
        
        /// <summary>
        /// Represents the font used to draw the name of each Recipe.
        /// </summary>
        private readonly IFont font = UIFonts.Tahoma7;

        /// <summary>
        /// Contains the ItemTooltips of the RecipeListItem this RecipeListBox contains.
        /// </summary>
        private readonly ItemTooltipGroup itemTooltips;

        /// <summary>
        /// Provides a mechanism that loads item data into memory.
        /// </summary>
        private readonly ItemManager itemManager;

        /// <summary>
        /// Represents an item within th RecipeListBox{T}.
        /// </summary>
        private sealed class RecipeListItem : ListItem<Recipe>
        {
            private static float AvailableRecipeNameWidth
            {
                get
                {
                    return ListWidth - 40;
                }
            }

            /// <summary>
            /// Gets the background color of this RecipeListItem.
            /// </summary>
            private Xna.Color BackgroundColor
            {
                get
                {
                    if( this.IsSelected )
                    {
                        return Xna.Color.Red;
                    }
                    else
                    {
                        return this.ListIndex % 2 == 0 ?
                            Xna.Color.Black : Xna.Color.White;
                    }
                }
            }

            /// <summary>
            /// Gets the foreground color of this RecipeListItem.
            /// </summary>
            private Xna.Color ForegroundColor
            {
                get
                {
                    if( this.IsSelected )
                    {
                        return Xna.Color.White;
                    }
                    else
                    {
                        return this.ListIndex % 2 == 0 ?
                            Xna.Color.White : Xna.Color.Black;
                    }
                }
            }

            /// <summary>
            /// Gets the result of Recipe of this RecipeListItem.
            /// </summary>
            private Item Item
            {
                get
                {
                    if( this.Value != null )
                        return this.Value.Result.Item;
                    else
                        return null;
                }
            }

            /// <summary>
            /// Initializes a new instance of the RecipeListItem class.
            /// </summary>
            /// <param name="list">
            /// The RecipeListBox that owns the new RecipeListItem.
            /// </param>
            public RecipeListItem( RecipeListBox list )
                : base( list )
            {
                this.font = list.font;
            }
            
            /// <summary>
            /// Called when the ClientArea of this UIElement has been refreshed.
            /// </summary>
            protected override void OnClientAreaChanged()
            {
                this.RefreshItemTooltipPosition();
            }

            /// <summary>
            /// Refreshes the position of the ItemTooltip.
            /// </summary>
            private void RefreshItemTooltipPosition()
            {
                if( this.itemTooltip != null )
                {
                    this.itemTooltip.Position = this.Position + new Vector2( 15, ItemHeight / 2 );
                }
            }

            /// <summary>
            /// Called when this RecipeListItem is drawing itself.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            protected override void OnDraw( ISpriteDrawContext drawContext )
            {
                var zeldaDrawContext = (ZeldaDrawContext)drawContext;

                this.DrawBackground( zeldaDrawContext );
                this.DrawRecipeName( zeldaDrawContext );
                this.DrawSymbol( drawContext );
            }

            /// <summary>
            /// Draws the sprite of the item created by this RecipeListBox.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            private void DrawSymbol( ISpriteDrawContext drawContext )
            {
                this.itemTooltip.DrawItemSprite( drawContext );
            }
            
            /// <summary>
            /// Draws the background of this RecipeListItem.
            /// </summary>
            /// <param name="drawContext">
            /// The current ZeldaDrawContext.
            /// </param>
            private void DrawBackground( ZeldaDrawContext drawContext )
            {
                drawContext.Batch.DrawRect(
                    this.ClientArea.ToXna(),
                    this.BackgroundColor,
                    0.5f
                );
            }

            /// <summary>
            /// Draws the name of the Recipe.
            /// </summary>
            /// <param name="drawContext">
            /// The current ZeldaDrawContext.
            /// </param>
            private void DrawRecipeName( ZeldaDrawContext drawContext )
            {
                Vector2 position;
                position.X = this.X + 30;
                position.Y = (int)this.Y + 6;

                this.font.Draw(
                    this.recipeName,
                    position,
                    this.ForegroundColor,
                    0.75f,
                    drawContext
                );
            }
            
            /// <summary>
            /// Called when the visability state of this RecipeListItem has changed.
            /// </summary>
            protected override void OnIsVisibleChanged()
            {
                if( this.IsVisible )
                {
                    var list = (RecipeListBox)this.List;
                    this.Value.FullyLoad( list.itemManager );

                    if( this.itemTooltip == null )
                    {
                        this.itemTooltip = list.itemTooltips.AddNew( this.Item );
                    }

                    this.itemTooltip.ShowAndEnable();
                    this.RefreshItemTooltipPosition();
                    this.LoadRecipeName();
                }
                else
                {
                    if( this.itemTooltip != null )
                    {
                        this.itemTooltip.HideAndDisable();
                    }
                }
            }

            /// <summary>
            /// Loads and caches the name of the recipe.
            /// </summary>
            private void LoadRecipeName()
            {
                this.recipeName = this.Item.LocalizedName;
                float width = this.font.MeasureStringWidth( this.recipeName );

                if( width > AvailableRecipeNameWidth )
                {
                    this.ShortenRecipeName();
                }
            }

            /// <summary>
            /// Shortens the name of the recipe to fit
            /// into the ListItem{T}.
            /// </summary>
            private void ShortenRecipeName()
            {
                float width = this.ShortenRecipeNameByRemovingWords();
                
                if( width > AvailableRecipeNameWidth )
                {
                }
            }

            /// <summary>
            /// Shortens the name of the recipe to fit
            /// into the ListItem{T} by removing words from the
            /// end of the name.
            /// </summary>
            /// <example>
            /// "Tigersword of the Fallen Realm"
            /// could become
            /// "Tigersword of the .."
            /// </example>
            /// <returns>
            /// The length of the new recipe name.
            /// </returns>
            private float ShortenRecipeNameByRemovingWords()
            {
                string[] words = this.recipeName.Split( ' ' );

                float width = 0.0f;
                this.recipeName = string.Empty;

                for( int i = 0; i < words.Length; ++i )
                {
                    string word = words[i];

                    float wordWidth = this.font.MeasureStringWidth( word );
                    float totalWidth = width + wordWidth;

                    if( totalWidth <= AvailableRecipeNameWidth )
                    {
                        if( i != 0 )
                        {
                            this.recipeName += " ";
                        }

                        this.recipeName += word;
                        width = totalWidth;
                    }
                    else
                    {
                        this.recipeName += " ..";
                        break;
                    }
                }

                return width;
            }

            /// <summary>
            /// Caches the name of the recipe; as displayed in the UI.
            /// </summary>
            private string recipeName;

            /// <summary>
            /// Represents the tooltip that is used to draw the item created by the Recipe.
            /// </summary>
            private ItemTooltip itemTooltip;

            /// <summary>
            /// Represents the font that is used to draw the name of the Recipe.
            /// </summary>
            private IFont font;
        }
    }
}

