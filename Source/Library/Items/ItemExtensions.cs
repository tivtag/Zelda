// <copyright file="ItemExtensions.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the static Zelda.Items.ItemExtensions classs.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    /// <summary>
    /// Provides extension methods for <see cref="Item"/> related enumerations.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the WeaponType represents a ranged weapon.
        /// </summary>
        /// <param name="weaponType">
        /// The input WeaponType.
        /// </param>
        /// <returns>
        /// True if the <paramref name="weaponType"/> represents a ranged weapon; 
        /// otherwise false.
        /// </returns>
        public static bool IsRanged( this WeaponType weaponType )
        {
            switch( weaponType )
            {
                case WeaponType.Bow:
                case WeaponType.Crossbow:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the EquipmentSlot may contain a Trinket, Relic, Ring or Necklace.
        /// </summary>
        /// <param name="slot">
        /// The input EquipmentSlot.
        /// </param>
        /// <returns>
        /// True if the <paramref name="slot"/> may contain any jewelry; 
        /// otherwise false.
        /// </returns>
        public static bool IsJewelry( this EquipmentSlot slot )
        {
            switch( slot )
            {
                case EquipmentSlot.Trinket:
                case EquipmentSlot.Relic:
                case EquipmentSlot.Ring:
                case EquipmentSlot.Necklace:
                    return true;

                default:
                    return false;
            }
        }      

        /// <summary>
        /// Gets a value indicating whether the EquipmentSlot may contain a Melee Weapon, Ranged Weapon, Shield or Staff.
        /// </summary>
        /// <param name="slot">
        /// The input EquipmentSlot.
        /// </param>
        /// <returns>
        /// True if the <paramref name="slot"/> may contain any weapon or shield; 
        /// otherwise false.
        /// </returns>
        public static bool IsWeaponOrShield( this EquipmentSlot slot )
        {
            switch( slot )
            {
                case EquipmentSlot.WeaponHand:
                case EquipmentSlot.Ranged:
                case EquipmentSlot.ShieldHand:
                case EquipmentSlot.Staff:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this WeaponType descripes a weapon
        /// that can wield with one weapon; and as such allows blocking.
        /// </summary>
        /// <param name="weaponType">
        /// The input WeaponType.
        /// </param>
        /// <returns>
        /// True if the <paramref name="weaponType"/> descripes an one-handed weapon; 
        /// otherwise false.
        /// </returns>
        public static bool IsOneHanded( this WeaponType weaponType )
        {
            switch( weaponType )
            {
                case WeaponType.OneHandedAxe:
                case WeaponType.OneHandedMace:
                case WeaponType.OneHandedSword:
                case WeaponType.Dagger:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Affixes.AffixType"/> allows
        /// the item to have an <see cref="Affixes.IPrefix"/>.
        /// </summary>
        /// <param name="affixType">
        /// The AffixType to check.
        /// </param>
        /// <returns>
        /// True if the given <paramref name="affixType"/> is 
        /// <see cref="Affixes.AffixType.Prefix"/>, <see cref="Affixes.AffixType.Both"/> or <see cref="Affixes.AffixType.AlwaysBoth"/>;
        /// otherwise false.
        /// </returns>
        public static bool AllowsPrefix( this Affixes.AffixType affixType )
        {
            switch( affixType )
            {
                case Affixes.AffixType.Prefix:
                case Affixes.AffixType.Both:
                case Affixes.AffixType.AlwaysBoth:
                case Affixes.AffixType.AlwaysOneOrBoth:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Affixes.AffixType"/> allows
        /// the item to have an <see cref="Affixes.ISuffix"/>.
        /// </summary>
        /// <param name="affixType">
        /// The AffixType to check.
        /// </param>
        /// <returns>
        /// True if the given <paramref name="affixType"/> is 
        /// <see cref="Affixes.AffixType.Suffix"/>, <see cref="Affixes.AffixType.Both"/> or <see cref="Affixes.AffixType.AlwaysBoth"/>;
        /// otherwise false.
        /// </returns>
        public static bool AllowsSuffix( this Affixes.AffixType affixType )
        {
            switch( affixType )
            {
                case Affixes.AffixType.Suffix:
                case Affixes.AffixType.Both:
                case Affixes.AffixType.AlwaysBoth:
                case Affixes.AffixType.AlwaysOneOrBoth:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given Equipment is an offensive offhand item
        /// that goes into the Shield slot.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment to check.
        /// </param>
        /// <returns>
        /// True if the given Equipment is an offensive offhand item;
        /// otherwise false.
        /// </returns>
        internal static bool IsOffensiveShieldHand( Equipment equipment )
        {
            return equipment.Slot == EquipmentSlot.ShieldHand && equipment.Armor <= 0;
        }

        /// <summary>
        /// Gets a value indicating whether the given Equipment is a protective shield.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment to check.
        /// </param>
        /// <returns>
        /// True if the given Equipment is an protective shield;
        /// otherwise false.
        /// </returns>
        internal static bool IsProtectiveShield( Equipment equipment )
        {
            return equipment.Slot == EquipmentSlot.ShieldHand && equipment.Armor > 0;
        }

        /// <summary>
        /// Gets a value indicating whether the given SpecialItemType represents
        /// and item made of Metal or Chains.
        /// </summary>
        /// <param name="itemType">
        /// The Equipment to check.
        /// </param>
        /// <returns>
        /// True if the given SpecialItemType represents an item made of Metal or Chains;
        /// otherwise false.
        /// </returns>
        internal static bool IsMetalOrChain( this SpecialItemType itemType )
        {
            switch( itemType )
            {
                case SpecialItemType.MetalHeavy:
                case SpecialItemType.MetalLight:
                case SpecialItemType.ChainsHeavy:
                case SpecialItemType.Chains:
                    return true;

                default:
                    return false;
            }
        }
    }
}
