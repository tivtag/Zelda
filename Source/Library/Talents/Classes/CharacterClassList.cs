// <copyright file="CharacterClassList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Classes.CharacterClassList class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Classes
{
    using System.Collections.Generic;

    /// <summary>
    /// Stores all known <see cref="CharacterClass"/>es; sorted by their CharacterClassKey.
    /// </summary>
    public sealed class CharacterClassList
    {
        /// <summary>
        /// Initializes a new instance of the CharacterClassList class.
        /// </summary>
        public CharacterClassList()
        {
            this.classes = new CharacterClass[(int)CharacterClassKey.__Count__];

            for( int i = 0; i < this.classes.Length; ++i )
            {
                this.classes[i] = Build( (CharacterClassKey)i );
            }
        }
        
        /// <summary>
        /// Gets the CharacterClass with the specified CharacterClassKey.
        /// </summary>
        /// <param name="key">
        /// The CharacterClassKey that uniquely identifies the CharacterClass to get.
        /// </param>
        /// <returns>
        /// The requested CharacterClass.
        /// </returns>
        public CharacterClass GetClass( CharacterClassKey key )
        {
            return this.classes[(int)key];
        }
        
        /// <summary>
        /// Builds the CharacterClass for the specified CharacterClassKey.
        /// </summary>
        /// <param name="key">
        /// The CharacterClassKey that uniquely identifies the CharacterClass to create.
        /// </param>
        /// <returns>
        /// The newly created CharacterClass.
        /// </returns>
        private static CharacterClass Build( CharacterClassKey key )
        {
            string keyName = key.ToString();
            string name = ClassResources.ResourceManager.GetString( "CN_" + keyName );
            string hardcoreName = ClassResources.ResourceManager.GetString( "CN_" + keyName + "_HC" );

            return new CharacterClass( key, name, hardcoreName );
        }

        /// <summary>
        /// All CharacterClasses, sorted by their CharacterClassKey.
        /// </summary>
        private readonly CharacterClass[] classes;
    }
}
