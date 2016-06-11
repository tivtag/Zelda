// <copyright file="IngameUserInterface.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.IngameUserInterface class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Profiles;
    using Zelda.UI.Crafting;
    using Zelda.UI.Items;

    /// <summary>
    /// Represents the UserInterface shown during the actual game.
    /// </summary>
    internal sealed class IngameUserInterface : ZeldaUserInterface
    {
        /// <summary>
        /// Gets the currently open <see cref="IngameWindow"/>.
        /// </summary>
        public IngameWindow OpenWindow
        {
            get
            {
                return this.openWindow;
            }
        }

        /// <summary>
        /// Gets the <see cref="SideBar"/> associated with this IngameUserInterface.
        /// </summary>
        public SideBar SideBar
        {
            get
            {
                return this.sideBar;
            }
        }

        /// <summary>
        /// Gets the <see cref="QuickActionSlotsDisplay"/> associated with this IngameUserInterface.
        /// </summary>
        public QuickActionSlotsDisplay ActionSlotsDisplay
        {
            get
            {
                return this.actionSlotsDisplay;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the IngameUserInterface class.
        /// </summary>
        /// <param name="ingameState">
        /// The IngameState of the game.
        /// </param>
        public IngameUserInterface( Zelda.GameStates.IngameState ingameState )
        {
            this.ingameState = ingameState;
            this.inventoryService = new InventoryService( this );
        }

        /// <summary>
        /// Setups this IngameUserInterface.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( this.isSetup )
                return;

            base.Setup( serviceProvider );

            // cooldownVisualizer
            this.cooldownVisualizer = new CooldownVisualizer( serviceProvider );
            this.cooldownVisualizer.LoadContent();
            this.AddElement( cooldownVisualizer );

            // cooldownVisualizerItems
            this.cooldownVisualizerItems = new CooldownVisualizer( serviceProvider ) { FloorNumber = 3 };
            this.cooldownVisualizerItems.LoadContent();
            this.AddElement( cooldownVisualizerItems );

            this.inventoryService.Setup( serviceProvider );
            this.merchantWindowService = new Zelda.UI.Trading.MerchantWindowProvider( this, serviceProvider );

            this.AddElement( new ManaBar( serviceProvider ) );
            this.AddElement( new HeartBar( serviceProvider ) );
            this.AddElement( new TimeDisplay( serviceProvider ) );
            this.AddElement( new RubyDisplay( serviceProvider ) );
            this.AddElement( new BuffBarDisplay( this.cooldownVisualizer, serviceProvider ) );
            this.AddElement( new RespawnTextDisplay( serviceProvider ) );
            this.AddElement( new PickedupItemDisplay( serviceProvider ) );
            this.AddElement( new EnteringRegionDisplay( serviceProvider ) );
            this.AddElement( new DamageMeterDisplay( serviceProvider ) );
            this.AddElement( new SharedChestWindow( this.cooldownVisualizerItems, serviceProvider ) );

            this.actionSlotsDisplay = new QuickActionSlotsDisplay( this.cooldownVisualizer, serviceProvider );
            this.AddElement( actionSlotsDisplay );

            // Windows:
            this.inventoryWindow = new InventoryWindow( this.cooldownVisualizerItems, serviceProvider );
            this.AddElement( inventoryWindow );

            this.craftingBottleWindow = new Zelda.UI.Crafting.CraftingBottleWindow( this.cooldownVisualizerItems, serviceProvider );
            this.AddElement( craftingBottleWindow );

            this.recipesWindow = new RecipesWindow( serviceProvider );
            this.AddElement( recipesWindow );

            this.equipmentWindow = new EquipmentWindow( serviceProvider );
            this.AddElement( equipmentWindow );

            this.characterWindow = new CharacterWindow( serviceProvider );
            this.AddElement( characterWindow );

            this.characterDetailsWindow = new CharacterDetailsWindow( serviceProvider );
            this.AddElement( characterDetailsWindow );

            this.talentWindow = new TalentWindow( serviceProvider );
            this.AddElement( talentWindow );

            this.questLogWindow = new QuestLogWindow( serviceProvider );
            this.AddElement( questLogWindow );

            this.miniMapWindow = new MiniMapWindow( serviceProvider );
            this.AddElement( miniMapWindow );

            this.ocarinaWindow = new Zelda.UI.Ocarina.OcarinaWindow( serviceProvider );
            this.AddElement( ocarinaWindow );

            this.sideBar = new SideBar( this.ingameState );
            this.AddElement( this.sideBar );

            this.SetupWindowRing();
            this.inventoryWindow.SideBar = sideBar;

            this.serviceProvider = serviceProvider;
            this.isSetup = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unload()
        {
            this.CloseOpenWindow();
            this.SetupForPlayer( null ); // Unload all links to player.

            this.RemoveAllElements();
            this.FocusedElement = null;

            this.isSetup = false;
        }

        /// <summary>
        /// Setups the ring that connects each IngameWindow with two other IngameWindows.
        /// </summary>
        private void SetupWindowRing()
        {
            characterWindow.Previous = ocarinaWindow;
            characterWindow.Next = characterDetailsWindow;

            characterDetailsWindow.Previous = characterWindow;
            characterDetailsWindow.Next = talentWindow;

            talentWindow.Previous = characterDetailsWindow;
            talentWindow.Next = inventoryWindow;

            inventoryWindow.Previous = talentWindow;
            inventoryWindow.Next = equipmentWindow;

            equipmentWindow.Previous = inventoryWindow;
            equipmentWindow.Next = craftingBottleWindow;

            craftingBottleWindow.Previous = equipmentWindow;
            craftingBottleWindow.Next = recipesWindow;

            recipesWindow.Previous = craftingBottleWindow;
            recipesWindow.Next = questLogWindow;

            questLogWindow.Previous = recipesWindow;
            questLogWindow.Next = miniMapWindow;

            miniMapWindow.Previous = questLogWindow;
            miniMapWindow.Next = ocarinaWindow;

            ocarinaWindow.Previous = miniMapWindow;
            ocarinaWindow.Next = characterWindow;
        }

        /// <summary>
        /// Setups this IngameUserInterface for the given GameProfile.
        /// </summary>
        /// <param name="profile">
        /// The GameProfile to use.
        /// </param>
        public void SetupForProfile( GameProfile profile )
        {
            this.GetElement<EnteringRegionDisplay>().Reset();
            this.BlendElement.Reset();

            if( profile == null )
                return;

            this.GetElement<TimeDisplay>().DateTime = profile.WorldStatus.IngameDateTime;
            this.SetupForPlayer( profile.Player );
            this.SetupKeyWindowMap( profile.Player.Profile.KeySettings );
        }

        private void SetupForPlayer( Entities.PlayerEntity player )
        {
            if( !isSetup )
                return;

            this.GetElement<ManaBar>().Player = player;
            this.GetElement<HeartBar>().Player = player;
            this.GetElement<RubyDisplay>().Player = player;
            this.GetElement<QuestLogWindow>().QuestLog = player != null ? player.QuestLog : null;
            this.GetElement<BuffBarDisplay>().Player = player;
            this.GetElement<RespawnTextDisplay>().Player = player;
            this.GetElement<PickedupItemDisplay>().Player = player;
            this.GetElement<DamageMeterDisplay>().Player = player;
            this.GetElement<SharedChestWindow>().Player = player;
            this.GetElement<SideBar>().Player = player;

            this.recipesWindow.Player = player;
            this.talentWindow.Player = player;
            this.ocarinaWindow.Player = player;
            this.inventoryWindow.Player = player;
            this.equipmentWindow.Player = player;
            this.characterWindow.Player = player;
            this.characterDetailsWindow.Player = player;
            this.actionSlotsDisplay.Player = player;
            this.craftingBottleWindow.Player = player;

            if( player != null && player.Statable.IsDead )
            {
                this.GetElement<RespawnTextDisplay>().ShowText();
            }
            else
            {
                this.GetElement<RespawnTextDisplay>().HideAndDisable();
            }
        }

        /// <summary>
        /// Setups this IngameUserInterface for the given ZeldaScene.
        /// </summary>
        /// <param name="scene">The ZeldaScene to use.</param>
        public void SetupForScene( ZeldaScene scene )
        {
            this.Scene = scene;
            this.miniMapWindow.Scene = scene;
        }

        /// <summary>
        /// Setups the key - window map to use the specified keyboad settings.
        /// </summary>
        /// <param name="settings">
        /// The keyboard settings to use.
        /// </param>
        private void SetupKeyWindowMap( KeySettings settings )
        {
            this.keyWindowMap.Clear();

            this.AddKeyWindowMapping( settings.EquipmentWindow, this.equipmentWindow );
            this.AddKeyWindowMapping( settings.InventoryWindow, this.inventoryWindow );
            this.AddKeyWindowMapping( settings.CraftingWindow, this.craftingBottleWindow, this.recipesWindow );
            this.AddKeyWindowMapping( settings.QuestLogWindow, this.questLogWindow );
            this.AddKeyWindowMapping( settings.CharacterWindow, this.characterWindow,  this.characterDetailsWindow );
            this.AddKeyWindowMapping( settings.TalentWindow, this.talentWindow );
            this.AddKeyWindowMapping( settings.MiniMapWindow, this.miniMapWindow );
            this.AddKeyWindowMapping( settings.OcarinaWindow, this.ocarinaWindow );
        }

        /// <summary>
        /// Adds a new mapping between keyboard key and an ingame window.
        /// </summary>
        /// <param name="key">
        /// The key that should open the window.
        /// </param>
        /// <param name="windows">
        /// The windows that should be opened. First window = normal key, second window = normal key + shift
        /// </param>
        private void AddKeyWindowMapping( Keys key, params IngameWindow[] windows )
        {
            this.keyWindowMap[key] = windows;
        }

        /// <summary>
        /// Helper functions that toggles the IngameWindow of the given type on/off.
        /// </summary>
        /// <typeparam name="TWindow">
        /// The type of the window to toggle.
        /// </typeparam>
        /// <returns>
        /// true if the given IngameWindow has been toggled;
        /// otherwise false.
        /// </returns>
        public bool ToggleWindow<TWindow>()
            where TWindow : IngameWindow
        {
            return this.ToggleWindow( this.GetElement<TWindow>() );
        }

        /// <summary>
        /// Helper functions that toggles the given <see cref="IngameWindow"/> on/off.
        /// </summary>
        /// <param name="window">
        /// The window to toggle on/off.
        /// </param>
        /// <returns>
        /// true if the given IngameWindow has been toggled;
        /// otherwise false.
        /// </returns>
        public bool ToggleWindow( IngameWindow window )
        {
            if( this.Dialog.IsEnabled )
                return false;

            if( window.IsEnabled )
            {
                Debug.Assert( openWindow == window );
                window.Close();

                this.openWindow = null;

                this.ingameState.IsPaused = false;
                this.actionSlotsDisplay.IsEnabled = true;
            }
            else
            {
                if( !window.CanBeOpened )
                    return false;

                if( this.openWindow != null )
                    this.openWindow.Close();

                this.openWindow = window;
                this.openWindow.Open();

                this.ingameState.IsPaused = true;
                this.actionSlotsDisplay.IsEnabled = false;
                this.actionSlotsDisplay.RestorePickedUp();
            }

            return true;
        }

        /// <summary>
        /// Closes the current IngameWindow off, and opens the following IngameWindow.
        /// </summary>
        /// <param name="currentWindow">
        /// The current window.
        /// </param>
        private void ToggleWindowRight( IngameWindow currentWindow )
        {
            var nextWindow = currentWindow.Next;
            if( nextWindow == null )
                return;

            if( nextWindow.CanBeOpened )
            {
                this.ToggleWindow( nextWindow );
            }
            else
            {
                // Skip next window.
                this.ToggleWindowRight( nextWindow.Next );
            }
        }

        /// <summary>
        /// Closes the current IngameWindow off, and opens the previous IngameWindow.
        /// </summary>
        /// <param name="currentWindow">
        /// The current window.
        /// </param>
        private void ToggleWindowLeft( IngameWindow currentWindow )
        {
            var nextWindow = currentWindow.Previous;
            if( nextWindow == null )
                return;

            if( nextWindow.CanBeOpened )
            {
                this.ToggleWindow( nextWindow );
            }
            else
            {
                // Skip next window.
                this.ToggleWindowRight( nextWindow.Previous );
            }
        }

        /// <summary>
        /// Restores the state of this IngameUserInterface to prepare it for saving or exiting the game.
        /// </summary>
        public void RestoreForSaveOrExit()
        {
            this.actionSlotsDisplay.RestorePickedUp();
        }

        /// <summary>
        /// Handles the event of the player pressing any key.
        /// </summary>
        /// <param name="key">
        /// The key that has been pressed and should now be handled.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        /// <returns>
        /// Returns whether the key has been handled by this IngameUserInterfacce.
        /// </returns>
        public bool HandleKeyDown( Keys key, ref KeyboardState oldKeyState )
        {
            if( !oldKeyState.IsKeyUp( key ) )
                return false;

            IngameWindow[] windows;

            if( this.keyWindowMap.TryGetValue( key, out windows ) )
            {
                IngameWindow window;
                if( windows.Length > 1 )
                {
                    window = IsShiftDown ? windows[1] : windows[0];
                }
                else
                {
                    window = windows[0];
                }
                    
                this.ToggleWindow( window );
                return true;
            }
            else
            {
                switch( key )
                {
                    case Keys.Left:
                        if( this.openWindow != null )
                        {
                            this.ToggleWindowLeft( openWindow );
                            return true;
                        }
                        break;

                    case Keys.Right:
                        if( this.openWindow != null )
                        {
                            this.ToggleWindowRight( openWindow );
                            return true;
                        }
                        break;

                    case Keys.Escape:
                        if( !this.CloseOpenWindow() )
                        {
                            this.ChangeToIngameOptionsScreen();
                        }
                        break;

                    case Keys.Back:
                        this.CloseOpenWindow();
                        break;

                    default:
                        break;
                }

                return false;
            }
        }

        /// <summary>
        /// Closes the currently openWindow.
        /// </summary>
        /// <returns>
        /// true if it has been closed;
        /// otherwise false.
        /// </returns>
        private bool CloseOpenWindow()
        {
            if( this.openWindow != null )
            {
                this.openWindow.Close();
                this.openWindow = null;
                this.ingameState.IsPaused = false;
                this.actionSlotsDisplay.IsEnabled = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Changes from the ingame state to the ingame options state.
        /// </summary>
        private void ChangeToIngameOptionsScreen()
        {
            var gameStateManager = serviceProvider.GetService<GameStateManager>();
            gameStateManager.Push<Zelda.GameStates.IngameMenuState>();
        }

        /// <summary>
        /// Tries to get the service of the specified type.
        /// </summary>
        /// <param name="serviceType">
        /// The type of service object to get.
        /// </param>
        /// <returns>
        /// The requested service object; or null.
        /// </returns>
        public override object GetService( System.Type serviceType )
        {
            if( serviceType == typeof( Zelda.Trading.UI.IMerchantWindowService ) )
            {
                return this.merchantWindowService;
            }
            else if( serviceType == typeof( IInventoryService ) )
            {
                return this.inventoryService;
            }

            return base.GetService( serviceType );
        }

        /// <summary>
        /// Immediatly releases the unmanaged resources used by this IngameUserInterface.
        /// </summary>
        public void Dispose()
        {
            if( this.miniMapWindow != null )
            {
                this.miniMapWindow.Dispose();
                this.miniMapWindow = null;
            }
        }
        
        /// <summary>
        /// States whether this IngameUserInterface has been setup.
        /// </summary>
        private bool isSetup;

        /// <summary>
        /// Identifies the ingame state.
        /// </summary>
        private readonly Zelda.GameStates.IngameState ingameState;

        /// <summary>
        /// Enables the visualization of <see cref="Cooldown"/> data.
        /// </summary>
        private CooldownVisualizer cooldownVisualizer, cooldownVisualizerItems;

        /// <summary>
        /// Identifies the QuickActionSlotsDisplay UIElement.
        /// </summary>
        private QuickActionSlotsDisplay actionSlotsDisplay;

        /// <summary>
        /// The currently active <see cref="IngameWindow"/>, if any.
        /// </summary>
        /// <remarks>Only one IngameWindow can be active at any time.</remarks>
        private IngameWindow openWindow;

        /// <summary>
        /// Indentifies the <see cref="InventoryWindow"/> UIElement.
        /// </summary>
        private InventoryWindow inventoryWindow;

        /// <summary>
        /// Indentifies the <see cref="CraftingBottleWindow"/> UIElement.
        /// </summary>
        private Zelda.UI.Crafting.CraftingBottleWindow craftingBottleWindow;

        /// <summary>
        /// Indentifies the <see cref="EquipmentWindow"/> UIElement.
        /// </summary>
        private EquipmentWindow equipmentWindow;

        /// <summary>
        /// Indentifies the <see cref="CharacterWindow"/> UIElement.
        /// </summary>
        private CharacterWindow characterWindow;

        /// <summary>
        /// Indentifies the <see cref="CharacterDetailsWindow"/> UIElement.
        /// </summary>
        private CharacterDetailsWindow characterDetailsWindow;

        /// <summary>
        /// Indentifies the <see cref="TalentWindow"/> UIElement.
        /// </summary>
        private TalentWindow talentWindow;

        /// <summary>
        /// Indentifies the <see cref="QuestLogWindow"/> UIElement.
        /// </summary>
        private QuestLogWindow questLogWindow;

        /// <summary>
        /// Indentifies the <see cref="MiniMapWindow"/> UIElement.
        /// </summary>
        private MiniMapWindow miniMapWindow;

        /// <summary>
        /// Indentifies the <see cref="RecipesWindow"/> UIElement.
        /// </summary>
        private RecipesWindow recipesWindow;

        /// <summary>
        /// Indentifies the <see cref="Zelda.UI.Ocarina.OcarinaWindow"/> UIElement.
        /// </summary>
        private Zelda.UI.Ocarina.OcarinaWindow ocarinaWindow;

        /// <summary>
        /// Indentifies the <see cref="SideBar"/> UIElement.
        /// </summary>
        private SideBar sideBar;

        /// <summary>
        /// Provides a mechanism that allows the player to
        /// interact with the offerings of an IMerchant.
        /// </summary>
        private Zelda.Trading.UI.IMerchantWindowService merchantWindowService;

        /// <summary>
        /// Provides a mechanism for opening the UI for the various Inventory implementations.
        /// </summary>
        private readonly IInventoryService inventoryService;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        /// <summary>
        /// Maps input keys onto ingame windows.
        /// </summary>
        private readonly Dictionary<Keys, IngameWindow[]> keyWindowMap = new Dictionary<Keys, IngameWindow[]>();
    }
}