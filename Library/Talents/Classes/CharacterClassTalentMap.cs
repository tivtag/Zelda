// <copyright file="CharacterClassTalentMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Classes.CharacterClassTalentMap class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Classes
{
    using System.Collections.Generic;

    /// <summary>
    /// Maps <see cref="CharacterClassTalentKey"/>s onto <see cref="CharacterClass"/>es.
    /// </summary>
    public sealed class CharacterClassTalentMap
    {
        /// <summary>
        /// Initializes a new instance of the CharacterClassTalentMap class.
        /// </summary>
        /// <param name="classList">
        /// The list that contains all CharacterClasses.
        /// </param>
        public CharacterClassTalentMap( CharacterClassList classList )
        {
            this.classList = classList;
           
            //        M  R  Ma
            this.Map( 0, 0, 0, CharacterClassKey.Novice );
            this.Map( 0, 1, 0, CharacterClassKey.Archer );
            this.Map( 0, 2, 0, CharacterClassKey.Ranger );
            this.Map( 0, 3, 0, CharacterClassKey.Sharpshooter );
            this.Map( 0, 4, 0, CharacterClassKey.MasterSharpshooter );

            this.Map( 1, 0, 0, CharacterClassKey.Swordsman );
            this.Map( 1, 1, 0, CharacterClassKey.Rogue );
            this.Map( 1, 2, 0, CharacterClassKey.MeleeRanger );
            this.Map( 1, 3, 0, CharacterClassKey.Sharpshooter );
            this.Map( 1, 4, 0, CharacterClassKey.MasterSharpshooter );

            this.Map( 2, 0, 0, CharacterClassKey.Warrior );
            this.Map( 2, 1, 0, CharacterClassKey.SwiftWarrior );
            this.Map( 2, 2, 0, CharacterClassKey.Assassin );
            this.Map( 2, 3, 0, CharacterClassKey.EnragedSharpshooter );
            this.Map( 2, 4, 0, CharacterClassKey.EnragedMeleeSharpshooter );
            
            this.Map( 3, 0, 0, CharacterClassKey.Knight );
            this.Map( 3, 1, 0, CharacterClassKey.SwiftKnight );
            this.Map( 3, 2, 0, CharacterClassKey.EnragedAssassin );
            this.Map( 3, 3, 0, CharacterClassKey.BalancedWarrior );
            this.Map( 3, 4, 0, CharacterClassKey.SwiftMasterAssassin );

            this.Map( 4, 0, 0, CharacterClassKey.MasterSwordsman );
            this.Map( 4, 1, 0, CharacterClassKey.SwiftKnight );
            this.Map( 4, 2, 0, CharacterClassKey.SwiftMasterSwordsman );
            this.Map( 4, 3, 0, CharacterClassKey.RangedFighter );
            this.Map( 4, 4, 0, CharacterClassKey.Jedi );

            this.Map( 0, 0, 1, CharacterClassKey.Apprentice );
            this.Map( 0, 0, 2, CharacterClassKey.Mage );
            this.Map( 0, 0, 3, CharacterClassKey.Wizard );
            this.Map( 0, 0, 4, CharacterClassKey.Warlock );

            this.Map( 0, 1, 1, CharacterClassKey.Apprentice );
            this.Map( 0, 1, 2, CharacterClassKey.Mage );
            this.Map( 0, 1, 3, CharacterClassKey.Wizard );
            this.Map( 0, 1, 4, CharacterClassKey.Warlock );

            this.Map( 0, 2, 1, CharacterClassKey.ApprenticeOfWinds );
            this.Map( 0, 2, 2, CharacterClassKey.MageOfWinds );
            this.Map( 0, 2, 3, CharacterClassKey.WizardOfWinds );
            this.Map( 0, 2, 4, CharacterClassKey.Warlock );
            
            this.Map( 0, 3, 1, CharacterClassKey.ApprenticeOfWinds );
            this.Map( 0, 3, 2, CharacterClassKey.MageOfWinds );
            this.Map( 0, 3, 3, CharacterClassKey.Spellthief );
            this.Map( 0, 3, 4, CharacterClassKey.HexSpellthief );

            this.Map( 2, 0, 1, CharacterClassKey.ApprenticeOfWinds );
            this.Map( 2, 0, 2, CharacterClassKey.MageOfWinds );
            this.Map( 2, 0, 3, CharacterClassKey.WizardOfWinds );
            this.Map( 2, 0, 4, CharacterClassKey.Hexblade );
        }

        /// <summary>
        /// Tries to get the CharacterClass that is associated with the specified CharacterClassTalentKey.
        /// </summary>
        /// <param name="talentKey">
        /// The CharacterClassTalentKey that maps onto the CharacterClass to get.
        /// </param>
        /// <returns>
        /// The requested CharacterClass; 
        /// or null.
        /// </returns>
        public CharacterClass TryGet( CharacterClassTalentKey talentKey )
        {
            CharacterClass @class;

            if( this.map.TryGetValue( talentKey, out @class ) )
            {
                return @class;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Maps the specified CharacterClassTalentKey onto the CharacterClass for this CharacterClassMap.
        /// </summary>
        /// <remarks>
        /// All support levels are mapped onto the same class.
        /// </remarks>
        /// <param name="meleeLevel">
        /// The level of melee tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="rangedLevel">
        /// The level of ranged tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="magicLevel">
        /// The level of magic tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="characterClass">
        /// The CharacterClass that should be added.
        /// </param>
        private void Map( int meleeLevel, int rangedLevel, int magicLevel, CharacterClassKey characterClass )
        {
            var @class = this.classList.GetClass( characterClass );

            const int MaximumTalentTreeLevel = 5;
            for( int supportLevel = 0; supportLevel < MaximumTalentTreeLevel; ++supportLevel )
            {
                this.Map( meleeLevel, rangedLevel, magicLevel, supportLevel, @class );
            }
        }

        /// <summary>
        /// Maps the specified CharacterClassTalentKey onto the CharacterClass for this CharacterClassMap.
        /// </summary>
        /// <param name="meleeLevel">
        /// The level of melee tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="rangedLevel">
        /// The level of ranged tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="magicLevel">
        /// The level of magic tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="supportLevel">
        /// The level of support tree that wraps onto the specified CharacterClass.
        /// </param>
        /// <param name="characterClass">
        /// The CharacterClass that should be added.
        /// </param>
        private void Map( int meleeLevel, int rangedLevel, int magicLevel, int supportLevel, CharacterClass characterClass )
        {
            this.Map(
                new CharacterClassTalentKey( meleeLevel, rangedLevel, magicLevel, supportLevel ),
                characterClass
            );
        }

        /// <summary>
        /// Maps the specified CharacterClassTalentKey onto the CharacterClass for this CharacterClassMap.
        /// </summary>
        /// <param name="key">
        /// The key the class should be mapped on.
        /// </param>
        /// <param name="class">
        /// The CharacterClass that should be added.
        /// </param>
        private void Map( CharacterClassTalentKey key, CharacterClass @class )
        {
            this.map.Add( key, @class );
        }

        /// <summary>
        /// The dictionary that maps CharacterClassTalentKeys onto CharacterClasses.
        /// </summary>
        private readonly Dictionary<CharacterClassTalentKey, CharacterClass> map =
            new Dictionary<CharacterClassTalentKey, CharacterClass>();

        /// <summary>
        /// Identifies the list that contains all CharacterClasses.
        /// </summary>
        private readonly CharacterClassList classList;
    }
}