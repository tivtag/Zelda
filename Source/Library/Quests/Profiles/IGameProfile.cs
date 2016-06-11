// <copyright file="IGameProfile.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Profiles.IGameProfile interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Profiles
{
    using Atom;
    using Zelda.Difficulties;
    using Zelda.Entities.Drawing;
    
    /// <summary>
    /// An <see cref="IGameProfile"/> stores all
    /// the data one has to know about a player.
    /// </summary>
    /// <remarks>
    /// The different profile types:
    /// <para>
    ///     ShortGameProfile - 
    ///         provides only the basic data about the player,
    ///         Created when the user enters the Character Selection Screen.
    ///         Must be converted to a GameProfile before it can be loaden.
    /// </para>
    /// <para>
    ///     GameProfile -
    ///         provides all the data about the player.
    /// </para>
    /// </remarks>
    public interface IGameProfile : IReadOnlyNameable
    {
        /// <summary>
        /// Gets the level of the game-character.
        /// </summary>
        int CharacterLevel { get; }

        /// <summary>
        /// Gets the name of the game-character.
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// Gets the name of the class of the game-character.
        /// </summary>
        string CharacterClass { get; }

        /// <summary>
        /// Gets the name of the region the player has saved in.
        /// </summary>
        string RegionName { get; }
        
        /// <summary>
        /// Gets the ID that uniquely identifies the difficulty the player has choosen
        /// for his game.
        /// </summary>
        DifficultyId Difficulty { get; }

        /// <summary>
        /// Gets a value indicating whether the player choose to play on hardcore.
        /// </summary>
        bool Hardcore { get; }
        
        /// <summary>
        /// Gets the color tint that is applied to the main characters sprite.
        /// </summary>
        LinkSpriteColorTint CharacterColorTint { get; }

        /// <summary>
        /// Saves this <see cref="IGameProfile"/>.
        /// </summary>
        /// <returns>
        /// true if this IGameProfile has been succesfully saved;
        /// otherwise false.
        /// </returns>
        bool Save();

        /// <summary>
        /// Loads this <see cref="IGameProfile"/>.
        /// </summary>
        /// <returns>
        /// The loaded <see cref="GameProfile"/>.
        /// </returns>
        GameProfile Load();
    }
}
