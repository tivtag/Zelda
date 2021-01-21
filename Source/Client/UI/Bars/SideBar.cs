// <copyright file="SideBar.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.SideBar class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// The Side Bar provides an easy mechanism
    /// to open the individual windows, such as the 
    /// Character, Equipment, Quest or Inventory window.
    /// </summary>
    internal sealed class SideBar : UIElement
    {
        /// <summary>
        /// The color the sprite of an element in the SideBar is tinted in
        /// when the player moves the mouse over it.
        /// </summary>
        private static readonly Xna.Color ColorMouseOver = Xna.Color.Silver;
        
        /// <summary>
        /// Gets or sets the <see cref="PlayerEntity"/> whose settings / status / etc
        /// can be accessed using this SideBar.
        /// </summary>
        public PlayerEntity Player
        {
            get
            {
                return this.player;
            }

            set
            {
                if( this.player != null )
                {
                    this.player.Statable.LevelUped -= OnPlayerLevelUp;
                    this.player.Equipment.SlotChanged -= OnPlayerEquipmentSlotChanged;
                    this.player.Latern.IsToggledChanged -= OnLaternIsToggledChanged;
                    this.player.Fairy.IsEnabledChanged -= OnFairyIsEnabledChanged;
                }

                this.player = value;
                
                if( this.player != null )
                {
                    this.player.Statable.LevelUped += OnPlayerLevelUp;
                    this.player.Equipment.SlotChanged += OnPlayerEquipmentSlotChanged;
                    this.player.Latern.IsToggledChanged += OnLaternIsToggledChanged;
                    this.player.Fairy.IsEnabledChanged += OnFairyIsEnabledChanged;
                }

                ResetMarkedButtons();
                CheckEquipmentStatus();
                RefreshLaternButton();
                RefreshFairyButton();
            }
        }
        
        private void ResetMarkedButtons()
        {
            var bottleWindow = this.Owner.GetElement<Crafting.CraftingBottleWindow>();
            bottleWindow.UnmarkNewRecipeLearned();
            this.buttonCharacter.ColorDefault = Xna.Color.White;
            this.buttonTalent.ColorDefault = Xna.Color.White;
            this.buttonBottle.ColorDefault = Xna.Color.White;
        }

        /// <summary>
        /// Initializes a new instance of the SideBar class.
        /// </summary>
        /// <param name="ingameState">
        /// The IGameState that controls the behaviour of the actual game.
        /// </param>
        public SideBar( Zelda.GameStates.IngameState ingameState )
            : base( "SideBar" )
        {
            this.FloorNumber       = 5;
            this.RelativeDrawOrder = 0.89f;
            
            this.ingameState   = ingameState;
            this.userInterface = ingameState.UserInterface;
            
            this.InitializeComponents( ingameState.Game );
        }

        /// <summary>
        /// Creates and setups the buttons of the (right) sidebar.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void InitializeComponents( IZeldaServiceProvider serviceProvider )
        {
            ISpriteLoader spriteLoader = serviceProvider.SpriteLoader;
            Point2 viewSize = serviceProvider.ViewSize;

            float offsetX = viewSize.X - 4;
            float offsetY = viewSize.Y - 3;

            // Create Toggle Latern Button
            {
                offsetY -= 35;

                buttonLatern = this.AddButton(
                    "Button_Latern",
                    "Misc_Latern_Red_A_2",
                    new Vector2( offsetX, offsetY ),
                    this.OnLaternButton_Clicked,
                    spriteLoader
                );

                buttonLatern.ColorSelected = Xna.Color.White;
                offsetY -= 2 + buttonLatern.Size.Y;
            }

            // Create Toggle Fairy Button
            {
                buttonFairy = this.AddButton(
                    "Fairy_Gray_2",
                    "Fairy_Violet_2",
                    new Vector2( offsetX, offsetY ),
                    this.OnFairyButton_Clicked,
                    spriteLoader
                );

                buttonFairy.ColorSelected = Xna.Color.White;
                offsetY -= 2 + buttonFairy.Size.Y;
            }

            // Create Quest Log Button
            {
                var button = this.AddButton(
                    "Button_QuestLog",
                    new Vector2( offsetX, offsetY - 2 ),
                    this.OnQuestLogButton_Clicked,
                    spriteLoader
                );

                offsetY -= 2 + button.Size.Y;
            }

            // Create Inventory Button
            {
                var button = this.AddButton(
                    "Button_Inventory",
                    new Vector2( offsetX, offsetY - 2 ),
                    this.OnInventoryButton_Clicked,
                    spriteLoader
                );

                offsetY -= 2 + button.Size.Y;
            }

            // Create MagicBottle Button
            {
                this.buttonBottle = this.AddButton(
                    "Button_MagicBottle",
                    new Vector2( offsetX - 1, offsetY - 2 ),
                    this.OnMagicBottleButton_Clicked,
                    spriteLoader
                );
                
                var recipesWindow = userInterface.GetElement<Crafting.RecipesWindow>();
                recipesWindow.Opened += this.OnRecipesWindowOpened;

                offsetY -= 2 + this.buttonBottle.Size.Y;
            }

            // Create Equipment Window Button
            {
                this.buttonEquipment = this.AddButton(
                    "Button_EquipmentWindow",
                    new Vector2( offsetX - 1, offsetY - 2 ),
                    this.OnEquipmentButton_Clicked,
                    spriteLoader
                );

                offsetY -= 2 + buttonEquipment.Size.Y;
            }

            // Create Character Window Button
            {
                this.buttonTalent = this.AddButton(
                    "Button_TalentsWindow",
                    new Vector2( offsetX, offsetY - 2 ),
                    this.OnTalentsButton_Clicked,
                    spriteLoader
                );

                var talentWindow = userInterface.GetElement<TalentWindow>();
                talentWindow.Opened += this.OnTalentWindowOpened;

                offsetY -= 2 + this.buttonTalent.Size.Y;
            }

            // Create Character Window Button
            {
                this.buttonCharacter = this.AddButton(
                    "Button_CharacterWindow",
                    new Vector2( offsetX, offsetY - 2 ),
                    this.OnCharacterButton_Clicked,
                    spriteLoader
                );

                var characterWindow = userInterface.GetElement<CharacterWindow>();
                characterWindow.Opened += this.OnCharacterWindowOpened;

                offsetY -= 2 + this.buttonCharacter.Size.Y;
            }

            this.Size = new Vector2( 21.0f, viewSize.Y - offsetY - 30.0f );
            this.Position = new Vector2( viewSize.X, viewSize.Y - 33.0f ) - Size;
        }

        /// <summary>
        /// Adds a new SpriteButton to this SideBar.
        /// </summary>
        /// <param name="spriteName">
        /// The name of the sprite.
        /// </param>
        /// <param name="position">
        /// The position of the new Button.
        /// </param>
        /// <param name="onClicked">
        /// The event that should be raised when the new Button is clicked.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism for loading sprite resources.
        /// </param>
        /// <returns>
        /// The newly created SpriteButton.
        /// </returns>
        private SpriteButton AddButton(
            string spriteName,
            Vector2 position,
            MouseInputEventHandler onClicked,
            ISpriteLoader spriteLoader )
        {
            return AddButton( spriteName, spriteName, position, onClicked, spriteLoader );
        }
                
        /// <summary>
        /// Adds a new SpriteButton to this SideBar.
        /// </summary>
        /// <param name="spriteName">
        /// The name of the sprite.
        /// </param>
        /// <param name="spriteSelectedName">
        /// The name of the sprite used when the button is selected.
        /// </param>
        /// <param name="position">
        /// The position of the new Button.
        /// </param>
        /// <param name="onClicked">
        /// The event that should be raised when the new Button is clicked.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism for loading sprite resources.
        /// </param>
        /// <returns>
        /// The newly created SpriteButton.
        /// </returns>
        private SpriteButton AddButton( 
            string spriteName,
            string spriteSelectedName,
            Vector2 position, 
            MouseInputEventHandler onClicked,
            ISpriteLoader spriteLoader )
        {
            ISprite sprite = spriteLoader
                .LoadAsset( spriteName )
                .CreateInstance();

            ISprite spriteSelected;
            if( spriteName == spriteSelectedName )
            {
                spriteSelected = sprite;
            }
            else
            {
                spriteSelected = spriteLoader
                    .LoadAsset( spriteSelectedName )
                    .CreateInstance();
                }

            var button = new SpriteButton( spriteName, sprite, spriteSelected ) {
                ColorSelected     = ColorMouseOver,
                FloorNumber       = this.FloorNumber,
                RelativeDrawOrder = this.RelativeDrawOrder,
                Position          = position - sprite.Size
            };

            button.Clicked += onClicked;

            this.sideBarButtons.Add( button );
            return button;
        }
        
        /// <summary>
        /// Gets a value indicating whether there is any side-bar butotn at the given location.
        /// </summary>
        /// <param name="location">
        /// The location to query.
        /// </param>
        /// <returns>
        /// true if there is a direct hit; -or- otherwise false.
        /// </returns>
        public bool HasButtonAt( Point2 location )
        {            
            return this.ClientArea.Contains( location ) && sideBarButtons.Any( button => button.ClientArea.Contains( location ) );
        }
        
        /// <summary>
        /// Raised when the player has gained a level.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnPlayerLevelUp( object sender, System.EventArgs e )
        {
            this.buttonCharacter.ColorDefault = UIColors.MarkedButton;
            this.buttonTalent.ColorDefault = UIColors.MarkedButton;
            
            CheckNewRecipeLearned();
        }

        private void OnPlayerEquipmentSlotChanged( object sender, Zelda.Items.EquipmentStatusSlot e )
        {
            CheckEquipmentStatus();
        }

        private void CheckEquipmentStatus()
        {
            SetNegativeButtonState( buttonEquipment, player != null && player.Equipment.AnySlotUnfulfilled );
        }
        
        private void CheckNewRecipeLearned()
        {
            if( HasLearnedNewRecipe() )
            {
                MarkNewRecipeLearned();
            }
        }

        private void MarkNewRecipeLearned()
        {
            if( buttonBottle == null )
                throw new System.InvalidOperationException( "buttonBottle is null" );

            if( this.Owner == null )
                throw new System.InvalidOperationException( "Owner is null" );

            this.buttonBottle.ColorDefault = UIColors.MarkedButton;

            var bottleWindow = this.Owner.GetElement<Crafting.CraftingBottleWindow>();
            if( bottleWindow == null )
                throw new System.InvalidOperationException( "bottleWindow is null" );

            bottleWindow.MarkNewRecipeLearned();
        }

        private bool HasLearnedNewRecipe()
        {
            if( ingameState == null )
                throw new System.InvalidOperationException( "ingameState is null" );
            if( ingameState.Game == null )
                throw new System.InvalidOperationException( "Game is null" );

            var recipeDatabase = this.ingameState.Game.GetService<Zelda.Crafting.RecipeDatabase>();

            if( recipeDatabase == null )
                throw new System.InvalidOperationException( "recipeDatabase is null" );
            if( player == null )
                throw new System.InvalidOperationException( "player is null" );
            if( player.Statable == null )
                throw new System.InvalidOperationException( "Statable is null" );
   
            return recipeDatabase.LearnsNewRecipesAt( this.player.Statable.Level );
        }
        
        private void OnLaternIsToggledChanged( Latern sender )
        {
            RefreshLaternButton();
        }

        private void RefreshLaternButton()
        {
            if( player != null )
            {
                buttonLatern.IsSelected = player.Latern.IsToggled;
            }
        }

        private void OnFairyIsEnabledChanged( Fairy sender )
        {
            RefreshFairyButton();
        }

        private void RefreshFairyButton()
        {
            if( player != null )
            {
                buttonFairy.IsSelected = player.Fairy.IsEnabled;
            }
        }

        /// <summary>
        /// Raised when the player has opened the Character Status Window.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnCharacterWindowOpened( object sender, System.EventArgs e )
        {
            this.buttonCharacter.ColorDefault = Xna.Color.White;
        }

        /// <summary>
        /// Raised when the player has opened the Talent Tree Window.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnTalentWindowOpened( object sender, System.EventArgs e )
        {
            this.buttonTalent.ColorDefault = Xna.Color.White;
        }

        /// <summary>
        /// Raised when the player has opened the Recipes List Window.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnRecipesWindowOpened( object sender, System.EventArgs e )
        {
            var window = this.Owner.GetElement<Crafting.CraftingBottleWindow>();
            window.UnmarkNewRecipeLearned();

            this.buttonBottle.ColorDefault = Xna.Color.White;
        }

        /// <summary>
        /// Gets called when this SideBar has been added to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnAdded( UserInterface userInterface )
        {
            foreach( var button in this.sideBarButtons )
            {
                userInterface.AddElement( button );
            }
        }

        /// <summary>
        /// Gets called when this SideBar has been removed from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void OnRemoved( UserInterface userInterface )
        {
            foreach( var button in this.sideBarButtons )
            {
                userInterface.RemoveElement( button );
            }
        }

        /// <summary>
        /// Gets called when the IsVisible state of this SideBar has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            foreach( var button in this.sideBarButtons )
            {
                button.IsVisible = this.IsVisible;
            }
        }

        /// <summary>
        /// Gets called when the IsEnabled state of this SideBar has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            foreach( var button in this.sideBarButtons )
            {
                button.IsEnabled = this.IsEnabled;
            }
        }

        /// <summary>
        /// Called when the latern button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnLaternButton_Clicked( 
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            ingameState.Player.Latern.Toggle();
        }

        /// <summary>
        /// Called when the Fairy button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnFairyButton_Clicked(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            ingameState.Player.Fairy.Toggle();
        }

        /// <summary>
        /// Called when the Inventory Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnInventoryButton_Clicked(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            userInterface.ToggleWindow<InventoryWindow>();
        }

        /// <summary>
        /// Called when the Quest Log Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnQuestLogButton_Clicked(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            userInterface.ToggleWindow<QuestLogWindow>();
        }

        /// <summary>
        /// Called when the Equipment Window Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnEquipmentButton_Clicked(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            userInterface.ToggleWindow<EquipmentWindow>();
        }

        /// <summary>
        /// Called when the Character Window Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnCharacterButton_Clicked( 
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            if( userInterface.IsShiftDown )
            {
                userInterface.ToggleWindow<CharacterDetailsWindow>();
            }
            else
            {
                userInterface.ToggleWindow<CharacterWindow>();
            }
        }

        /// <summary>
        /// Called when the Talent Window Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnTalentsButton_Clicked(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            userInterface.ToggleWindow<TalentWindow>();
        }

        /// <summary>
        /// Called when the Magic Bottle Button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnMagicBottleButton_Clicked( 
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            if( userInterface.IsShiftDown )
            {
                userInterface.ToggleWindow<Zelda.UI.Crafting.RecipesWindow>();
            }
            else
            {
                userInterface.ToggleWindow<Zelda.UI.Crafting.CraftingBottleWindow>();
            }
        }

        /// <summary>
        /// Called when this SideBar is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            // The SideBar itself has no drawing logic
        }

        /// <summary>
        /// Called when this SideBar is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            // The SideBar itself has no update logic
        }

        private static void SetNegativeButtonState( SpriteButton button, bool isNegative )
        {
            button.ColorDefault = isNegative ? UIColors.NegativeBright : Xna.Color.White;
            button.ColorSelected = isNegative ? UIColors.NegativeDark : ColorMouseOver;
        }
        
        /// <summary>
        /// Represents the storage field of the Player property.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The button that when clicked opens various Ingame Window.
        /// </summary>
        private SpriteButton buttonCharacter, buttonTalent, buttonBottle, buttonEquipment, buttonLatern, buttonFairy;        

        /// <summary>
        /// The list of SpriteButtons in this SideBar.
        /// </summary>
        private readonly List<SpriteButton> sideBarButtons = new List<SpriteButton>();

        /// <summary>
        /// Identifies the IngameUserInterface that contains this SideBar.
        /// </summary>
        private readonly Zelda.UI.IngameUserInterface userInterface;

        /// <summary>
        /// Identifies the IGameState that represents the actual game..
        /// </summary>
        private readonly Zelda.GameStates.IngameState ingameState;
    }
}