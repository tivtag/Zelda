// <copyright file="RecipesWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Crafting.RecipesWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI.Crafting
{
    using System;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Crafting;
    using Zelda.UI.Items;
    using XnaF = Microsoft.Xna.Framework;
    using Atom.Xna.UI.Controls;

    /// <summary>
    /// Allows the user to see what recipes he has learned so far;
    /// and how they work.
    /// </summary>
    internal sealed class RecipesWindow : IngameWindow
    {
        /// <summary>
        /// Gets or sets the <see cref="RecipeCategory"/> that the user has currently selected.
        /// </summary>
        private RecipeCategory SelectedCategory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="IngameUserInterface"/> that owns this IngameWindow.
        /// </summary>
        public new IngameUserInterface Owner
        {
            get
            {
                return (IngameUserInterface)base.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the RecipesWindow class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RecipesWindow( IZeldaServiceProvider serviceProvider )
        {
            this.Size = serviceProvider.ViewSize;

            this.itemInfoDisplay = new ItemInfoDisplay( serviceProvider.GetService<IItemInfoVisualizer>() );
            var tooltipDrawElement = new ItemTooltipDrawElement( itemInfoDisplay ); 

            this.recipeDetailsControl = new RecipeDetailsControl( tooltipDrawElement, serviceProvider );
            this.list = new RecipeListBox( tooltipDrawElement, serviceProvider ) {
                FloorNumber = this.FloorNumber + 1
            };

            this.buttonCraftingBottle = new TextButton() {
                Text = Resources.Bottle,
                TextAlign = TextAlign.Right,
                Font = UIFonts.TahomaBold10,

                Position = new Vector2( serviceProvider.ViewSize.X - 2.0f, 1.0f ),
                FloorNumber = 3,

                ColorDefault = new Microsoft.Xna.Framework.Color( 255, 255, 255, 155 ),
                PassInputToSubElements = false,
                IsEnabled = false,
                IsVisible = false
            };

            this.buttonCraftingBottle.Clicked += this.OnButtonCraftingBottleClicked;

            this.list.SelectedItemChanged += this.OnSelectedRecipeChanged;
            this.database = serviceProvider.GetService<RecipeDatabase>();
        }

        /// <summary>
        /// Called when this RecipesWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;

            // Draw Background.
            batch.DrawRect(
                this.ClientArea.ToXna(),
                UIColors.LightWindowBackground
            );

            // Draw Title Background
            batch.DrawRect(
                new XnaF.Rectangle( 0, 0, (int)this.Width, 20 ),
                UIColors.WindowTitleBackground,
                0.0015f
            );

            // Draw Title String
            this.fontTitle.Draw(
                Zelda.Resources.Recipes,
                new Vector2( this.Width / 2.0f, 0.0f ),
                TextAlign.Center,
                Microsoft.Xna.Framework.Color.White,
                0.002f,
                drawContext
            );
        }

        /// <summary>
        /// Raised when the currently selected Recipe has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnSelectedRecipeChanged( object sender, EventArgs e )
        {
            this.recipeDetailsControl.Recipe = this.list.SelectedValue;
        }

        /// <summary>
        /// Called when this RecipesWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }
        
        /// <summary>
        /// Called before this RecipesWindow, or any other UIElement, is updating itself.
        /// </summary>
        protected override void OnPreUpdate()
        {
            this.itemInfoDisplay.Position = this.Owner.MousePosition;

            base.OnPreUpdate();
        }
        
        /// <summary>
        /// Adds the child elements of this RecipesWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.list );
            userInterface.AddElement( this.itemInfoDisplay );
            userInterface.AddElement( this.recipeDetailsControl );
            userInterface.AddElement( this.buttonCraftingBottle );
        }

        /// <summary>
        /// Removes the child elements of this RecipesWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.list );
            userInterface.RemoveElement( this.itemInfoDisplay );
            userInterface.RemoveElement( this.recipeDetailsControl );
            userInterface.RemoveElement( this.buttonCraftingBottle );
        }

        /// <summary>
        /// Gets called when the IsEnabled state of this RecipesWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            this.list.IsEnabled = this.list.IsVisible = this.IsEnabled;
            this.itemInfoDisplay.IsEnabled = this.itemInfoDisplay.IsVisible = this.IsEnabled;
            this.recipeDetailsControl.IsEnabled = this.recipeDetailsControl.IsVisible = this.IsEnabled;
            this.buttonCraftingBottle.IsEnabled = this.buttonCraftingBottle.IsVisible = this.IsEnabled;

            if( this.IsEnabled )
            {
                this.RefreshShownRecipes();
            }
        }

        /// <summary>
        /// Called every frame when this Atom.Xna.UI.UIElement is focused by its owning  Atom.Xna.UI.UserInterface.
        /// </summary>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        protected override void HandleKeyInput( ref XnaF.Input.KeyboardState keyState, ref XnaF.Input.KeyboardState oldKeyState )
        {
            if( keyState.IsKeyDown( XnaF.Input.Keys.Up ) && oldKeyState.IsKeyUp( XnaF.Input.Keys.Up ) )
            {
                this.list.ScrollBy( percentage: -10.0f );
            }

            if( keyState.IsKeyDown( XnaF.Input.Keys.Down ) && oldKeyState.IsKeyUp( XnaF.Input.Keys.Down ) )
            {
                this.list.ScrollBy( percentage: 10.0f );
            }

            base.HandleKeyInput( ref keyState, ref oldKeyState );
        }

        /// <summary>
        /// Gets called when the player clicks on the Bottle button.
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
        private void OnButtonCraftingBottleClicked( object sender, ref XnaF.Input.MouseState mouseState, ref XnaF.Input.MouseState oldMouseState )
        {
            this.Owner.ToggleWindow<Crafting.CraftingBottleWindow>();
        }

        /// <summary>
        /// Refreshes the recipes shown in the RecipesWindow;
        /// based on the currently selected Recipe Category.
        /// </summary>
        private void RefreshShownRecipes()
        {
            var recipes = this.database.GetVisibleRecipes( this.Player.Statable.Level, this.SelectedCategory );
            this.list.ShowRecipes( recipes );
        }

        /// <summary>
        /// Called when the PlayerEntity that owns this IngameWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.itemInfoDisplay.Player = this.Player;            
            base.OnPlayerChanged();
        }

        /// <summary>
        /// Provides access to all of the recipes known to the game.
        /// </summary>
        private readonly RecipeDatabase database;

        /// <summary>
        /// Represents the list of recipes.
        /// </summary>
        private readonly RecipeListBox list;

        /// <summary>
        /// Shows the details of the currently selected recipe.
        /// </summary>
        private readonly RecipeDetailsControl recipeDetailsControl;

        /// <summary>
        /// The IFonts used in the Recipes Window.
        /// </summary>
        private readonly IFont fontTitle = UIFonts.TahomaBold11;

        /// <summary>
        /// Represents an UIControl that draws item information.
        /// </summary>
        private readonly ItemInfoDisplay itemInfoDisplay;

        /// <summary>
        /// The Button that when clicked switches to the Crafting Bottle window.
        /// </summary>
        private readonly Button buttonCraftingBottle;
    }
}