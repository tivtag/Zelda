// <copyright file="ShortGameProfile.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//    Defines the Zelda.Profiles.ShortGameProfile class.
// </summary>
// <author>
//    Paul Ennemoser
// </author>

namespace Zelda.Profiles
{
    using System;
    using Zelda.Saving;
    using Zelda.Difficulties;

    /// <summary>
    /// Represents an <see cref="IGameProfile"/>
    /// that has loaden only the information needed
    /// to display the profile in the CharacterSelectionScreen.
    /// </summary>
    public sealed class ShortGameProfile : IGameProfile
    {
        /// <summary>
        /// Gets the name of this ShortGameProfile.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string CharacterName
        {
            get 
            {
                return this.shortInfo.CharacterName; 
            }
        }

        /// <summary>
        /// Gets the class of the character.
        /// </summary>
        public string CharacterClass
        {
            get 
            { 
                return this.shortInfo.CharacterClass; 
            }
        }

        /// <summary>
        /// Gets the name of the region the player has saved in.
        /// </summary>
        public string RegionName 
        {
            get
            {
                return shortInfo.RegionName;
            }
        }

        /// <summary>
        /// Gets the level of the character.
        /// </summary>
        public int CharacterLevel
        {
            get 
            { 
                return this.shortInfo.CharacterLevel; 
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
                return this.shortInfo.Difficulty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the player choose to play on hardcore.
        /// </summary>
        public bool Hardcore
        {
            get 
            {
                return this.shortInfo.Hardcore; 
            }
        }


        /// <summary>
        /// Gets the color tint that is applied to the main characters sprite.
        /// </summary>
        public Zelda.Entities.Drawing.LinkSpriteColorTint CharacterColorTint
        {
            get
            {
                return this.shortInfo.CharacterColorTint;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the ShortGameProfile class.
        /// </summary>
        /// <param name="profileName">
        /// The name of the profile.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public ShortGameProfile( string profileName, IZeldaServiceProvider serviceProvider )
        {
            this.Name      = profileName;
            this.shortInfo = SaveFile.LoadShortGameInfo( profileName, serviceProvider );

            this.serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// Fully loads this ShortGameProfile.
        /// </summary>
        /// <returns>
        /// The loaden GameProfile.
        /// </returns>
        public GameProfile Load()
        {
            return GameProfile.LoadAdventure( this.Name, this.serviceProvider );
        }

        /// <summary>
        /// This operation is not supported by ShortGameProfiles.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        bool IGameProfile.Save()
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Stores the short game information about the loaden profile.
        /// </summary>
        private readonly ShortSaveGameInfo shortInfo;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
