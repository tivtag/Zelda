// <copyright file="GameProfile.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Profiles.GameProfile class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Profiles
{
    using System;
    using Atom.Xna;
    using Zelda.Difficulties;
    using Zelda.Entities;
    using Zelda.Saving;

    /// <summary>
    /// A <see cref="GameProfile"/> provides all the data
    /// of the player's character.
    /// This is a sealed class.
    /// </summary>
    public sealed class GameProfile : IGameProfile
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the <see cref="GameProfile"/>.
        /// </summary>
        public string Name
        {
            get
            { 
                return this.saveFile.Name;
            }

            set
            {
                this.saveFile.Name = value;
                this.player.Name = value;
            }
        }

        /// <summary>
        /// Gets the level of the player.
        /// </summary>
        public int CharacterLevel
        {
            get
            { 
                return this.player.Statable.Level;
            }
        }

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string CharacterName
        {
            get
            {
                return this.player.Name;
            }
        }

        /// <summary>
        /// Gets the class of the player.
        /// </summary>
        public string CharacterClass
        {
            get 
            {
                return this.player.ClassName;
            }
        }

        /// <summary>
        /// Gets the <see cref="SaveFile"/> that stores
        /// the information that descripes the game's state.
        /// </summary>
        public SaveFile SaveFile
        {
            get 
            {
                return this.saveFile;
            }
        }      
        
        /// <summary>
        /// Gets the name of the region the player has saved in.
        /// </summary>
        public string RegionName
        {
            get
            {
                return this.player.Scene != null ? this.player.Scene.LocalizedName : "???";
            }
        }
        
        /// <summary>
        /// Gets the <see cref="PlayerEntity"/> associated with the <see cref="GameProfile"/>.
        /// </summary>
        public PlayerEntity Player
        {
            get
            {
                return this.player;
            }
        }

        /// <summary>
        /// Gets the KeySettings of the profile.
        /// </summary>
        public KeySettings KeySettings
        {
            get 
            {
                return this.keySettings; 
            }
        }

        /// <summary>
        /// Gets the object that descripes the current status of the world.
        /// </summary>
        public WorldStatus WorldStatus
        {
            get
            { 
                return this.SaveFile.WorldStatus;
            }
        }

        /// <summary>
        /// Gets the ID that uniquely identifies the difficulty the player has choosen
        /// for his game.
        /// </summary>
        public DifficultyId Difficulty
        {
            get
            {
                return this.saveFile.Difficulty;
            }

            set
            {
                this.saveFile.Difficulty = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player choose to play on hardcore.
        /// </summary>
        public bool Hardcore
        {
            get
            {
                return this.saveFile.Hardcore;
            }

            set
            {
                this.saveFile.Hardcore = value;
            }
        }

        /// <summary>
        /// Gets or sets the last safe <see cref="SavePoint"/> the player has used.
        /// </summary>
        public SavePoint LastSavePoint
        {
            get
            {
                return this.saveFile.LastSavePoint;
            }
            
            set
            {
                this.saveFile.LastSavePoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the color tint that is applied to the main characters sprite.
        /// </summary>
        public Zelda.Entities.Drawing.LinkSpriteColorTint CharacterColorTint 
        {
            get
            {
                return this.saveFile.CharacterColorTint;
            }

            set
            {
                this.saveFile.CharacterColorTint = value;
            }
        }

        #endregion

        #region [ Static Properties ]

        /// <summary>
        /// Gets or sets the current <see cref="GameProfile"/>.
        /// </summary>
        public static GameProfile Current
        {
            get
            {
                return GameProfile.current;
            }

            set
            {
                GameProfile.current = value;

                if( value != null )
                {
                    GameDifficulty.Current = value.Difficulty;                   
                }
            }
        }

        /// <summary>
        /// Represents the storage field of the static Current property.
        /// </summary>
        private static GameProfile current;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Prevents the default creation of GameProfile instances.
        /// </summary>
        private GameProfile()
        {
        }

        /// <summary>
        /// Creates a new <see cref="GameProfile"/> which is set-up
        /// to start a new adventure. (level 1!)
        /// </summary>
        /// <param name="keySettings">
        /// The KeySettings instance the new GameProfile uses.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The new GameProfile.
        /// </returns>
        public static GameProfile StartNewAdventure(
            KeySettings keySettings, 
            IZeldaServiceProvider serviceProvider )
        {
            var profile = new GameProfile();
            
            profile.keySettings = keySettings;

            string profileName = string.Empty;
            profile.player = PlayerEntity.StartNewAdventure( profileName, profile, serviceProvider );
            profile.saveFile = new SaveFile( profileName, profile, serviceProvider );

            // Set the starting date and time of the game:
            profile.WorldStatus.IngameDateTime.Current = new DateTime( 2012, 1, 1, 16, 30, 0 );
            
            // TiaWoodsNorth

            // Set starting point of the game:
            profile.SaveFile.LastSavePoint = new SavePoint( "HeroesHome", "SpawnPoint_Start" );

            return profile;
        }

        /// <summary>
        /// Loads an already existing adventure.
        /// </summary>
        /// <param name="profileName">
        /// The name of the profile to load.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly loaded GameProfile.
        /// </returns>
        public static GameProfile LoadAdventure( string profileName, IZeldaServiceProvider serviceProvider )
        {
            var profile = new GameProfile() {
                keySettings = new KeySettings()
            };
            
            profile.player = PlayerEntity.CreateExisting( profileName, profile, serviceProvider );
            profile.saveFile = new SaveFile( profileName, profile, serviceProvider );
            profile.saveFile.Load();

            return profile;
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves the <see cref="GameProfile"/>.
        /// </summary>
        /// <returns>
        /// true if this IGameProfile has been succesfully saved;
        /// otherwise false.
        /// </returns>
        public bool Save()
        {
            return this.SaveFile.Save();
        }

        /// <summary>
        /// <see cref="GameProfile"/>s are always in a loaden state.
        /// </summary>
        /// <returns>
        /// Returns this GameProfile.
        /// </returns>
        GameProfile IGameProfile.Load()
        {
            return this;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the file in which the data about this GameProfile is stored.
        /// </summary>
        private SaveFile saveFile;

        /// <summary>
        /// The <see cref="KeySettings"/> of the profile.
        /// </summary>
        private KeySettings keySettings;

        /// <summary>
        /// The <see cref="PlayerEntity"/>.
        /// </summary>
        private PlayerEntity player;

        #endregion
    }
}
