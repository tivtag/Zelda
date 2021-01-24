// <copyright file="ShortSaveGameInfo.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//      Defines the Zelda.Saving.ShortSaveGameInfo structure.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Saving
{
    using Zelda.Difficulties;

    /// <summary>
    /// Represents a POCObject holds the basic game information stored in a SaveFile.
    /// </summary>
    public struct ShortSaveGameInfo
    {
        /// <summary>
        /// The name of the character.
        /// </summary>
        public readonly string CharacterName;

        /// <summary>
        /// The class of the character.
        /// </summary>
        public readonly string CharacterClass;

        /// <summary>
        /// The level of the character.
        /// </summary>
        public readonly int CharacterLevel;

        /// <summary>
        /// The name of the region the player has saved in.
        /// </summary>
        public readonly string RegionName;

        /// <summary>
        /// The color tint that is applied to the main characters sprite.
        /// </summary>
        public readonly Zelda.Entities.Drawing.LinkSpriteColorTint CharacterColorTint;

        /// <summary>
        /// The ID that uniquely identifies the difficulty of the game.
        /// </summary>
        public readonly DifficultyId Difficulty;

        /// <summary>
        /// States whether the player choose to play on hardcore.
        /// </summary>
        public readonly bool Hardcore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortSaveGameInfo"/> structure.
        /// </summary>
        /// <param name="characterName">
        /// The name of the character.
        /// </param>
        /// <param name="characterClass">
        /// The class of the character.
        /// </param>
        /// <param name="characterLevel">
        /// The level of the character.
        /// </param>
        /// <param name="regionName">
        /// The name of the region the player has saved in.
        /// </param>
        /// <param name="characterColorTint">
        /// The color in which the player's character is tinted in.
        /// </param>
        /// <param name="difficulty">
        /// The ID that uniquely identifies the difficulty of the game.
        /// </param>
        /// <param name="hardcore">
        /// States whether the player choose to play on hardcore.
        /// </param>
        internal ShortSaveGameInfo(
            string characterName,
            string characterClass,
            int characterLevel,
            string regionName,
            Zelda.Entities.Drawing.LinkSpriteColorTint characterColorTint,
            DifficultyId difficulty,
            bool hardcore )
        {
            this.CharacterName = characterName;
            this.CharacterClass = characterClass;
            this.CharacterLevel = characterLevel;
            this.CharacterColorTint = characterColorTint;
            this.RegionName = regionName;
            this.Difficulty = difficulty;
            this.Hardcore = hardcore;
        }
    }
}
