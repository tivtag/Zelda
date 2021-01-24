// <copyright file="CraftingBottleWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Crafting.CraftingBottleWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Crafting
{
    using System.Globalization;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Crafting;
    using Zelda.Items;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Defines the <see cref="IngameWindow"/> that is used to
    /// visualize the <see cref="CraftingBottle"/> of the Player.
    /// </summary>
    internal sealed class CraftingBottleWindow : InventoryBaseWindow
    {             
        /// <summary>
        /// Initializes a new instance of the <see cref="CraftingBottleWindow"/> class.
        /// </summary>
        /// <param name="cooldownVisualizer">
        /// Provides a mechanism to visualize the cooldown on the ItemUseEffect of an item.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal CraftingBottleWindow( CooldownVisualizer cooldownVisualizer, IZeldaServiceProvider serviceProvider )
            : base( cooldownVisualizer, serviceProvider )
        {
            // Set fields
            this.itemManager = serviceProvider.ItemManager;
            this.recipeDatabase = serviceProvider.GetService<RecipeDatabase>();

            this.viewSize = serviceProvider.ViewSize;

            // Load Content
            this.rubySample = serviceProvider.AudioSystem.LoadSample( "RubyCatch.ogg" );

            var spriteLoader = serviceProvider.SpriteLoader;

            // Setup          
            this.Size = new Atom.Math.Vector2(
                CellSize * CraftingBottle.BottleGridWidth,
                CellSize * CraftingBottle.BottleGridHeight
             );

            this.Position = new Vector2(
                (serviceProvider.ViewSize.X / 2) - (CellSize * CraftingBottle.BottleGridWidth / 2),
                (serviceProvider.ViewSize.Y / 2) - (CellSize * CraftingBottle.BottleGridHeight / 2)
            );

            // Create Buttons
            this.buttonTransform = new SpriteButton(
                "Btn_MagicBottle_Transform",
                spriteLoader.LoadSprite( "Button_CubeTransform_Default" ),
                spriteLoader.LoadSprite( "Button_CubeTransform_Selected" )
            ) {
                FloorNumber = 3,
                Position    = new Vector2( this.Position.X + this.Size.X +  10, this.Position.Y ),
                IsEnabled   = false,
                IsVisible   = false
            };

            this.buttonTransformRubies = new SpriteButton(
                "Btn_MagicBottle_TransformRubies",
                spriteLoader.LoadSprite( "Button_CubeTransformRubies_Default" ),
                spriteLoader.LoadSprite( "Button_CubeTransformRubies_Selected" )
            ) {
                FloorNumber = 3,
                Position    = new Vector2( this.buttonTransform.Position.X, this.Position.Y + 42 ),
                IsEnabled   = false,
                IsVisible   = false
            };

            this.buttonRecipeList = new TextButton() {
                Text = Resources.Recipes,
                TextAlign = TextAlign.Right,
                Font = UIFonts.TahomaBold10,

                Position = new Vector2( serviceProvider.ViewSize.X - 2.0f, 1.0f ),
                FloorNumber = 4,

                ColorDefault = UIColors.DefaultUpperRowButton,
                PassInputToSubElements = false,
                IsEnabled = false,
                IsVisible = false
            };

            // Hook Events.
            this.buttonTransform.Clicked       += this.OnButtonTransformClicked;
            this.buttonTransformRubies.Clicked += this.OnButtonTransformRubiesClicked;
            this.buttonRecipeList.Clicked      += this.OnButtonRecipeListClicked;
        }

        internal void MarkNewRecipeLearned()
        {
            this.buttonRecipeList.ColorDefault = UIColors.MarkedButton;
        }

        internal void UnmarkNewRecipeLearned()
        {
            this.buttonRecipeList.ColorDefault = UIColors.DefaultUpperRowButton;
        }
        
        /// <summary>
        /// Called when this CraftingCubeWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        { 
            if( this.Player == null )
                return;

            var batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect( 
                new Xna.Rectangle( 0, 0, viewSize.X, viewSize.Y ),
                UIColors.LightWindowBackground
            );

            // Draw Title Background
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, viewSize.X, 20 ),
                UIColors.WindowTitleBackground,
                0.0015f
            );

            // Draw Title
            this.fontTitle.Draw(
                Resources.MagicCraftingBottle,
                new Vector2( viewSize.X / 2.0f, 0.0f ),
                TextAlign.Center, 
                Xna.Color.White,
                0.002f,
                drawContext
            );

            this.DrawCellsAndItems( drawContext );
            this.DrawPickedUpItemIndicator( 
                this.player.PickedupItemContainer.Item, 
                this.MousePosition,
                (ZeldaDrawContext)drawContext 
            );

            // Draw Tooltips
            if( this.buttonTransform.IsMouseOver )
            {
                Recipe recipe = this.player.CraftingBottle.FindWorkingRecipe( this.recipeDatabase );
                string text = null;

                if( recipe != null )
                {
                    Item item = itemManager.Get( recipe.Result.Name );                    
                    text = string.Format( CultureInfo.CurrentCulture, "Click to transform into {0}.", item != null ? item.LocalizedName : "??" );
                }
                else if( player.CraftingBottle.ItemCount == 0 )
                {
                    text = "Transforms the items that are placed in the bottle into an item.";
                }
                else
                {
                    text = "Nothing to do there. Are you missing a recipe?";
                }

                var font = UIFonts.Tahoma7;
                float width = font.MeasureStringWidth( text );

                font.Draw( text, new Vector2( viewSize.X / 2.0f, viewSize.Y - 40 ), TextAlign.Center, Xna.Color.White, 0.003f, drawContext );
                drawContext.Batch.DrawRect( new RectangleF( viewSize.X / 2.0f - width / 2.0f - 2, viewSize.Y - 42, width + 3, font.LineSpacing + 2 ), Xna.Color.Black.WithAlpha( 180 ), 0.002f );
            }

            if( this.buttonTransformRubies.IsMouseOver )
            {
                long rubiesWorth = this.player.CraftingBottle.TotalRubiesWorth;
                string text = rubiesWorth > 0 ?
                    string.Format( CultureInfo.CurrentCulture, "Click to transform into {0} rupees.", rubiesWorth.ToString( CultureInfo.CurrentCulture ) ) :
                    "Transforms the items that are placed in the bottle into rupees.";
                
                var font = UIFonts.Tahoma7;
                float width = font.MeasureStringWidth( text );

                font.Draw( text, new Vector2( viewSize.X / 2.0f, viewSize.Y - 40 ), TextAlign.Center, Xna.Color.White, 0.003f, drawContext );
                drawContext.Batch.DrawRect( new RectangleF( viewSize.X / 2.0f - width / 2.0f - 2, viewSize.Y - 42, width + 3, font.LineSpacing + 2 ), Xna.Color.Black.WithAlpha( 180 ), 0.002f );
            }
        }

        /// <summary>
        /// Handles mouse input related for this <see cref="CraftingBottleWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        protected override void HandleMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            if( !this.ClientArea.Contains( mouseState.X, mouseState.Y ) )
            {
                this.ItemInfoDisplay.ItemInstance = null;
            }

            base.HandleMouseInput( ref mouseState, ref oldMouseState );
        }

        /// <summary>
        /// Handles mouse input related to this <see cref="InventoryWindow"/>.
        /// </summary>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        /// <returns>Whether input should be passed to elements behind this UIElement.</returns>
        protected override bool HandleRelatedMouseInput(
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( this.Player == null )
                return true;
            
            Point2 cell = GetMouseCellPosition( ref mouseState );
            var bottle = this.Player.CraftingBottle;

            if( bottle.IsValidCell( cell ) )
            {
                ItemInstance itemInstance = bottle.GetItemAt( cell );

                if( mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released )
                {
                    bottle.HandleLeftClick( cell.X, cell.Y, this.Owner.IsShiftDown );
                }
                else
                if( mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released )
                {
                    bottle.HandleRightClick( cell.X, cell.Y, this.Owner.IsControlDown );
                }

                RefreshItemInfoDisplay();
            }

            return false;
        }
        
        /// <summary>
        /// Gets called when the IsEnabled state of this CraftingBottleWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.buttonTransform.IsEnabled = this.IsEnabled;
            this.buttonTransformRubies.IsEnabled = this.IsEnabled;
            this.buttonRecipeList.IsEnabled = this.IsEnabled;

            base.OnIsEnabledChanged();
        }

        /// <summary>
        /// Gets called when the IsVisible state of this CraftingBottleWindow has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            this.buttonTransform.IsVisible       = this.IsVisible;
            this.buttonTransformRubies.IsVisible = this.IsVisible;
            this.buttonRecipeList.IsVisible = this.IsVisible;

            base.OnIsVisibleChanged();
        }

        /// <summary>
        /// Gets called when the player clicks on the Transform button.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnButtonTransformClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.Player.CraftingBottle.Transform( this.recipeDatabase, this.itemManager );
        }

        /// <summary>
        /// Gets called when the player clicks on the Transform Rubies button.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnButtonTransformRubiesClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            if( this.Player.CraftingBottle.TransformItemsIntoRubies() )
            {
                if( rubySample != null )
                {
                    this.rubySample.Play();
                }
            }
        }
        
        /// <summary>
        /// Gets called when the player clicks on the Recipes button.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnButtonRecipeListClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.Owner.ToggleWindow<Crafting.RecipesWindow>();
        }
        
        /// <summary>
        /// Gets called when this CraftingBottleWindow has been added to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.buttonTransform );
            userInterface.AddElement( this.buttonTransformRubies );
            userInterface.AddElement( this.buttonRecipeList );

            base.OnAdded( userInterface );
        }

        /// <summary>
        /// Gets called when this CraftingBottleWindow has been removed from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.buttonTransform );
            userInterface.RemoveElement( this.buttonTransformRubies );
            userInterface.RemoveElement( this.buttonRecipeList );

            base.OnRemoved( userInterface );
        }
        
        /// <summary>
        /// Gets called when the PlayerEntity whose Inventory is visualized by this CraftingBottleWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.player = this.Player;
            base.OnPlayerChanged();
        }

        /// <summary>
        /// Gets the Inventory that gets visualized by this CraftingBottleWindow.
        /// </summary>
        /// <returns>
        /// An Inventory instance.
        /// </returns>
        protected override Inventory GetInventory()
        {
            return this.player.CraftingBottle;
        }

        /// <summary>
        /// The size of the ingame-view window.
        /// </summary>
        private readonly Point2 viewSize;

        /// <summary>
        /// Identifies the PlayerEntity whose Inventory is visualized using this CraftingBottleWindow.
        /// </summary>
        private Zelda.Entities.PlayerEntity player;

        /// <summary>
        /// Identifies the buttons shown in the CraftingBottleWindow.
        /// </summary>
        private readonly Atom.Xna.UI.Controls.SpriteButton buttonTransform, buttonTransformRubies;
        
        /// <summary>
        /// The Button that when clicked switches to the Crafting Recipes List.
        /// </summary>
        private readonly TextButton buttonRecipeList;
        
        /// <summary>
        /// Provides a mechanism to load Items.
        /// </summary>
        private readonly ItemManager itemManager;

        /// <summary>
        /// Provides access to all Recipes in the game.
        /// </summary>
        private readonly RecipeDatabase recipeDatabase;

        /// <summary>
        /// The IFonts used in the Crafting Bottle Window.
        /// </summary>
        private readonly IFont fontTitle = UIFonts.TahomaBold11;
        
        /// <summary>
        /// Represents the sound sample that is played when the player transforms
        /// items into rubies.
        /// </summary>
        private readonly Atom.Fmod.Sound rubySample;
    }
}
