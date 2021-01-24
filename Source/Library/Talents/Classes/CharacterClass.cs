// <copyright file="CharacterClass.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Classes.CharacterClass class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Classes
{
    /// <summary>
    /// Represents the character class of the player.
    /// </summary>
    /// <remarks>
    /// The player becomes a specific class by simply investing into
    /// the four talent trees: Melee, Ranged, Magic and Support.
    /// <para>
    /// Each of those trees is divided into levels.
    /// Based on those levels the current class is choosen.
    /// </para>
    /// </remarks>
    public sealed class CharacterClass
    {
        /// <summary>
        /// Gets the CharacterClassKey that uniquely identifies this CharacterClass.
        /// </summary>
        public CharacterClassKey Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets the (localized) name of this CharacterClass.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return this.localizedName;
            }
        }

        /// <summary>
        /// Gets the (localized) name of this CharacterClass in HC.
        /// </summary>
        public string LocalizedHardcoreName
        {
            get
            {
                return this.localizedHardcoreName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CharacterClass class.
        /// </summary>
        /// <param name="key">
        /// The CharacterClassKey that uniquely identifies the new CharacterClass.
        /// </param>
        /// <param name="localizedName">
        /// The (localized) name of the new CharacterClass.
        /// </param>
        /// <param name="localizedHardcoreName">
        /// The (localized) name of the new CharacterClass in HC.
        /// </param>
        internal CharacterClass( CharacterClassKey key, string localizedName, string localizedHardcoreName )
        {
            this.key = key;
            this.localizedName = localizedName;
            this.localizedHardcoreName = localizedHardcoreName ?? localizedName;
        }

        /// <summary>
        /// Stores the CharacterClassKey that uniquely identifies this CharacterClass.
        /// </summary>
        private readonly CharacterClassKey key;

        /// <summary>
        /// Stores the (localized) name of this CharacterClass.
        /// </summary>
        private readonly string localizedName, localizedHardcoreName;
    }
}
