// <copyright file="CharacterClassTalentKey.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Classes.CharacterClassTalentKey structure.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Classes
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents the key that uniquely identifies a CharacterClass.
    /// </summary>
    public struct CharacterClassTalentKey : IEquatable<CharacterClassTalentKey>
    {
        /// <summary>
        /// Gets the melee level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        /// <value>A value between 0 and 5.</value>
        public int Melee
        {
            get
            {
                return this.melee;
            }
        }

        /// <summary>
        /// Gets the ranged level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        /// <value>A value between 0 and 5.</value>
        public int Ranged
        {
            get
            {
                return this.ranged;
            }
        }

        /// <summary>
        /// Gets the magic level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        /// <value>A value between 0 and 5.</value>
        public int Magic
        {
            get
            {
                return this.magic;
            }
        }

        /// <summary>
        /// Gets the support level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        /// <value>A value between 0 and 5.</value>
        public int Support
        {
            get
            {
                return this.support;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CharacterClassTalentKey struct.
        /// </summary>
        /// <param name="melee">
        /// The melee level of the class associated with the new CharacterClassTalentKey.
        /// </param>
        /// <param name="ranged">
        /// The ranged level of the class associated with the new CharacterClassTalentKey.
        /// </param>
        /// <param name="magic">
        /// The magic level of the class associated with the new CharacterClassTalentKey.
        /// </param>
        /// <param name="support">
        /// The support level of the class associated with the new CharacterClassTalentKey.
        /// </param>
        public CharacterClassTalentKey( int melee, int ranged, int magic, int support )
        {
            Debug.Assert( melee >= 0 && melee <= 5 );
            Debug.Assert( ranged >= 0 && ranged <= 5 );
            Debug.Assert( magic >= 0 && magic <= 5 );
            Debug.Assert( support >= 0 && support <= 5 );

            this.melee = melee;
            this.ranged = ranged;
            this.magic = magic;
            this.support = support;
        }

        /// <summary>
        /// Indicates whether this CharacterClassTalentKey is equal to another Object.
        /// </summary>
        /// <param name="obj">
        /// The Object to compare with this CharacterClassTalentKey.
        /// </param>
        /// <returns>
        /// true if this CharacterClassKey is equal to the other Object;
        /// otherwise false.
        /// </returns>
        public override bool Equals( object obj )
        {
            if( obj is CharacterClassTalentKey )
            {
                return this.Equals( (CharacterClassTalentKey)obj );
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this CharacterClassTalentKey is equal to another CharacterClassTalentKey.
        /// </summary>
        /// <param name="other">
        /// The CharacterClassTalentKey to compare with this CharacterClassTalentKey.
        /// </param>
        /// <returns>
        /// true if this CharacterClassTalentKey is equal to the other CharacterClassTalentKey;
        /// otherwise false.
        /// </returns>
        public bool Equals( CharacterClassTalentKey other )
        {
            return other.melee == this.melee &&
                   other.ranged == this.ranged &&
                   other.magic == this.magic &&
                   other.support == this.support;
        }

        /// <summary>
        /// Gets the hashcode of this CharacterClassTalentKey.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            var hashBuilder = new Atom.HashCodeBuilder();

            hashBuilder.AppendStruct( this.melee );
            hashBuilder.AppendStruct( this.ranged );
            hashBuilder.AppendStruct( this.magic );
            hashBuilder.AppendStruct( this.support );

            return hashBuilder.GetHashCode();
        }

        /// <summary>
        /// The melee level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        private readonly int melee;

        /// <summary>
        /// The ranged level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        private readonly int ranged;

        /// <summary>
        /// The magic level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        private readonly int magic;

        /// <summary>
        /// The support level of the class that is associated with this CharacterClassTalentKey.
        /// </summary>
        private readonly int support;
    }
}
