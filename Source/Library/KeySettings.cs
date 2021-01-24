// <copyright file="KeySettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.KeySettings class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.IO;
    using Atom.Configuration;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary> 
    /// Stores the current key settings of the game.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// There exists one global KeySettings instance.
    /// But also KeySettings can be saved per Profile.
    /// </remarks>
    public sealed class KeySettings : Config
    {
        #region [ Movement ]

        /// <summary>
        /// Gets or sets the first key that moves the player left.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.Left, StorageName = "left-1" )]
        public Keys MoveLeft1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second key that moves the player left.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.A, StorageName = "left-2" )]
        public Keys MoveLeft2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first key that moves the player right.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.Right, StorageName = "right-1" )]
        public Keys MoveRight1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second key that moves the player right.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D, StorageName = "right-2" )]
        public Keys MoveRight2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first key that moves the player up.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.Up, StorageName = "up-1" )]
        public Keys MoveUp1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second key that moves the player up.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.W, StorageName = "up-2" )]
        public Keys MoveUp2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first key that moves the player down.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.Down, StorageName = "down-1" )]
        public Keys MoveDown1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second key that moves the player down.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.S, StorageName = "down-2" )]
        public Keys MoveDown2
        {
            get;
            set;
        }

        #endregion

        #region [ Gameplay ]

        /// <summary>
        /// Gets or sets the key that pick ups an item, talks to a NPC or uses anything else.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.E, StorageName = "use-pickup" )]
        public Keys UsePickup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that uses the currently 'best' available healing potion.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.H, StorageName = "use-healing-potion" )]
        public Keys UseHealingPotion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that uses the currently 'best' available mana potion.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.J, StorageName = "use-mana-potion" )]
        public Keys UseManaPotion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Gets or sets the key with which the player can toggle Gets or sets Gets or sets the latern.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.L, StorageName = "toggle-latern" )]
        public Keys ToggleLatern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Gets or sets the key with which the player can toggle Gets or sets Gets or sets the latern.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.P, StorageName = "toggle-fairy" )]
        public Keys ToggleFairy
        {
            get;
            set;
        }

        #endregion

        #region [ Windows ]

        /// <summary>
        /// Gets or sets the key that open the character window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.C, StorageName = "character-window" )]
        public Keys CharacterWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that opens the talent window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.T, StorageName = "talent-window" )]
        public Keys TalentWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that open the equipment window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.U, StorageName = "equipment-window" )]
        public Keys EquipmentWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that open the inventory window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.I, StorageName = "inventory-window" )]
        public Keys InventoryWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that opens the crafting bottle window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.B, StorageName = "crafting-window" )]
        public Keys CraftingWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that open the quest-log window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.O, StorageName = "quest-window" )]
        public Keys QuestLogWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that open the mini map window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.M, StorageName = "map-window" )]
        public Keys MiniMapWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that open ocarina window.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.K, StorageName = "ocarina-window" )]
        public Keys OcarinaWindow
        {
            get;
            set;
        }

        #endregion

        #region [ Quick Actions ]

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the first slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D1, StorageName = "action-1" )]
        public Keys Action1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the second slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D2, StorageName = "action-2" )]
        public Keys Action2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the third slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D3, StorageName = "action-3" )]
        public Keys Action3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the fourth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D4, StorageName = "action-4" )]
        public Keys Action4
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the fifth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D5, StorageName = "action-5" )]
        public Keys Action5
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the sixth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D6, StorageName = "action-6" )]
        public Keys Action6
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the seventh slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D7, StorageName = "action-7" )]
        public Keys Action7
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the eight slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D8, StorageName = "action-8" )]
        public Keys Action8
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the ninth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D9, StorageName = "action-9" )]
        public Keys Action9
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the tenth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.D0, StorageName = "action-10" )]
        public Keys Action10
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the eleventh slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.None, StorageName = "action-11" )]
        public Keys Action11
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the twelfth slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.None, StorageName = "action-12" )]
        public Keys Action12
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the 13th slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.None, StorageName = "action-13" )]
        public Keys Action13
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that triggers the quick action in the 14th slot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.None, StorageName = "action-14" )]
        public Keys Action14
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Gets or sets the key that saves the game.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.F5, StorageName = "quick-save" )]
        public Keys QuickSave
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that toggles the user-interface visibility.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.F10, StorageName = "toggle-ui" )]
        public Keys ToggleUserInterface
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that toggles the damage-per-second meter visibility.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.F12, StorageName = "toggle-dps-meter" )]
        public Keys ToggleDpsMeter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key that takes a screenshot.
        /// </summary>
        [ConfigProperty( DefaultValue = Keys.PrintScreen, StorageName = "take-screenshot" )]
        public Keys TakeScreenshot
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the full file name of the configuration file.
        /// </summary>
        private static string ConfigFileName
        {
            get
            {
                return Path.Combine( GameFolders.UserData, "key-settings.txt" );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeySettings"/> class.
        /// </summary>
        public KeySettings()
            : base( CreateConfigStore() )
        {
            this.LoadDefaults();

            if( File.Exists( ConfigFileName ) )
            {
                this.Load();
            }

            this.Save();
        }

        /// <summary>
        /// Creates a bew <see cref="IConfigStore"/> in which the KeySettings are stored.
        /// </summary>
        /// <returns>
        /// The newly created IConfigStore.
        /// </returns>
        private static IConfigStore CreateConfigStore()
        {
            return new PlainTextConfigStore( ConfigFileName );
        }

        public Keys GetActionAt( int index )
        {
            string name = "Action" + (index + 1).ToString();
            Type type = typeof( KeySettings );
            var property = type.GetProperty( name );

            object value = property.GetValue( this, null );
            return (Keys)value;
        }
    }
}
