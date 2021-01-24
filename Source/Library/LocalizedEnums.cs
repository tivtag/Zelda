// <copyright file="LocalizedEnums.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.LocalizedEnums utility class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;

    /// <summary>
    /// Static helper class that contains methods that localizes enums.
    /// </summary>
    public static class LocalizedEnums
    {
        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Factions.ReputationLevel"/>.
        /// </summary>
        /// <param name="level">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Factions.ReputationLevel level )
        {
            switch( level )
            {
                case Zelda.Factions.ReputationLevel.Neutral:
                    return Resources.Neutral;
                case Zelda.Factions.ReputationLevel.Friendly:
                    return Resources.Friendly;
                case Zelda.Factions.ReputationLevel.Hated:
                    return Resources.Hated;
                case Zelda.Factions.ReputationLevel.Unfriendly:
                    return Resources.Unfriendly;
                case Zelda.Factions.ReputationLevel.Honored:
                    return Resources.Honored;
                case Zelda.Factions.ReputationLevel.Revered:
                    return Resources.Revered;
                case Zelda.Factions.ReputationLevel.Exalted:
                    return Resources.Exalted;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.Stat"/>.
        /// </summary>
        /// <param name="stat">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Status.Stat stat )
        {
            switch( stat )
            {
                case Zelda.Status.Stat.Strength:
                    return Resources.Strength;
                case Zelda.Status.Stat.Dexterity:
                    return Resources.Dexterity;
                case Zelda.Status.Stat.Agility:
                    return Resources.Agility;
                case Zelda.Status.Stat.Vitality:
                    return Resources.Vitality;
                case Zelda.Status.Stat.Intelligence:
                    return Resources.Intelligence;
                case Zelda.Status.Stat.Luck:
                    return Resources.Luck;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.ChanceToStatus"/>.
        /// </summary>
        /// <param name="stat">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Status.ChanceToStatus stat )
        {
            switch( stat )
            {
                case Zelda.Status.ChanceToStatus.Crit:
                    return Resources.Crit;

                case Zelda.Status.ChanceToStatus.Miss:
                    return Resources.Hit;

                case Zelda.Status.ChanceToStatus.Dodge:
                    return Resources.Dodge;

                case Zelda.Status.ChanceToStatus.Parry:
                    return Resources.Parry;

                case Zelda.Status.ChanceToStatus.Block:
                    return Resources.BlockChance;

                case Zelda.Status.ChanceToStatus.Pierce:
                    return Resources.PierceChance;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.ChanceToStatus"/>.
        /// </summary>
        /// <param name="stat">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string GetRating( Zelda.Status.ChanceToStatus stat )
        {
            switch( stat )
            {
                case Zelda.Status.ChanceToStatus.Crit:
                    return Resources.CritRating;

                case Zelda.Status.ChanceToStatus.Miss:
                    return Resources.HitRating;

                case Zelda.Status.ChanceToStatus.Dodge:
                    return Resources.DodgeRating;

                case Zelda.Status.ChanceToStatus.Parry:
                    return Resources.ParryRating;

                case Zelda.Status.ChanceToStatus.Block:
                    return Resources.BlockChanceRating;

                case Zelda.Status.ChanceToStatus.Pierce:
                    return Resources.PierceChanceRating;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Attacks.AttackType"/>.
        /// </summary>
        /// <param name="modifier">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Attacks.AttackType modifier )
        {
            switch( modifier )
            {
                case Zelda.Attacks.AttackType.Melee:
                    return Resources.Melee;
                case Zelda.Attacks.AttackType.Ranged:
                    return Resources.Ranged;
                case Zelda.Attacks.AttackType.Spell:
                    return Resources.Magic;
                case Zelda.Attacks.AttackType.All:
                    return Resources.All;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.RaceType"/>.
        /// </summary>
        /// <param name="race">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Status.RaceType race )
        {
            switch( race )
            {
                case Zelda.Status.RaceType.Undead:
                    return Resources.Undead;

                case Zelda.Status.RaceType.Slime:
                    return Resources.Slime;

                case Zelda.Status.RaceType.Plant:
                    return Resources.Plant;

                case Zelda.Status.RaceType.Human:
                    return Resources.Human;

                case Zelda.Status.RaceType.Fairy:
                    return Resources.Fairy;

                case Zelda.Status.RaceType.DemiHuman:
                    return Resources.DemiHuman;

                case Zelda.Status.RaceType.DemiBeast:
                    return Resources.DemiBeast;

                case Zelda.Status.RaceType.Beast:
                    return Resources.Beast;

                case Zelda.Status.RaceType.Demon:
                    return Resources.Demon;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the localized plural string for the specified <see cref="Zelda.Status.RaceType"/>.
        /// </summary>
        /// <param name="race">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string GetPlural( Zelda.Status.RaceType race )
        {
            switch( race )
            {
                case Zelda.Status.RaceType.Undead:
                    return Resources.UndeadPlural;

                case Zelda.Status.RaceType.Slime:
                    return Resources.Slimes;

                case Zelda.Status.RaceType.Plant:
                    return Resources.Plants;

                case Zelda.Status.RaceType.Human:
                    return Resources.Humans;

                case Zelda.Status.RaceType.Fairy:
                    return Resources.Fairies;

                case Zelda.Status.RaceType.DemiHuman:
                    return Resources.DemiHumans;

                case Zelda.Status.RaceType.DemiBeast:
                    return Resources.DemiBeasts;

                case Zelda.Status.RaceType.Beast:
                    return Resources.Beasts;

                case Zelda.Status.RaceType.Demon:
                    return Resources.Demons;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Items.EquipmentSlot"/>.
        /// </summary>
        /// <param name="slot">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Items.EquipmentSlot slot )
        {
            switch( slot )
            {
                case Zelda.Items.EquipmentSlot.Boots:
                    return Resources.Boots;

                case Zelda.Items.EquipmentSlot.Belt:
                    return Resources.Belt;

                case Zelda.Items.EquipmentSlot.Chest:
                    return Resources.Chest;

                case Zelda.Items.EquipmentSlot.Head:
                    return Resources.Head;

                case Zelda.Items.EquipmentSlot.Cloak:
                    return Resources.Cloak;

                case Zelda.Items.EquipmentSlot.Necklace:
                    return Resources.Necklace;

                case Zelda.Items.EquipmentSlot.Ring:
                    return Resources.Ring;

                case Zelda.Items.EquipmentSlot.Trinket:
                    return Resources.Trinket;

                case Zelda.Items.EquipmentSlot.WeaponHand:
                    return Resources.WeaponHand;

                case Zelda.Items.EquipmentSlot.ShieldHand:
                    return Resources.ShieldHand;

                case Zelda.Items.EquipmentSlot.Ranged:
                    return Resources.Ranged;

                case Zelda.Items.EquipmentSlot.Staff:
                    return Resources.Staff;

                case Zelda.Items.EquipmentSlot.Relic:
                    return Resources.Relic;

                case Zelda.Items.EquipmentSlot.Gloves:
                    return Resources.Gloves;

                case Zelda.Items.EquipmentSlot.Bag:
                    return Resources.Bag;

                case Zelda.Items.EquipmentSlot.None:
                    return string.Empty;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Items.WeaponType"/>.
        /// </summary>
        /// <param name="type">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Items.WeaponType type )
        {
            switch( type )
            {
                case Zelda.Items.WeaponType.Dagger:
                    return Resources.Dagger;

                case Zelda.Items.WeaponType.OneHandedAxe:
                    return Resources.OneHandedAxe;

                case Zelda.Items.WeaponType.OneHandedMace:
                    return Resources.OneHandedMace;

                case Zelda.Items.WeaponType.OneHandedSword:
                    return Resources.OneHandedSword;

                case Zelda.Items.WeaponType.Bow:
                    return Resources.Bow;

                case Zelda.Items.WeaponType.Crossbow:
                    return Resources.Crossbow;

                case Zelda.Items.WeaponType.Rod:
                    return Resources.Rod;

                case Zelda.Items.WeaponType.Staff:
                    return Resources.Staff;

                case Zelda.Items.WeaponType.TwoHandedAxe:
                    return Resources.TwoHandedAxe;

                case Zelda.Items.WeaponType.TwoHandedMace:
                    return Resources.TwoHandedMace;

                case Zelda.Items.WeaponType.TwoHandedSword:
                    return Resources.TwoHandedSword;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.ElementalSchool"/>.
        /// </summary>
        /// <param name="element">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Status.ElementalSchool element )
        {
            switch( element )
            {
                case Zelda.Status.ElementalSchool.Fire:
                    return Resources.Fire;

                case Zelda.Status.ElementalSchool.Light:
                    return Resources.Light;

                case Zelda.Status.ElementalSchool.Nature:
                    return Resources.Nature;

                case Zelda.Status.ElementalSchool.Shadow:
                    return Resources.Shadow;

                case Zelda.Status.ElementalSchool.Water:
                    return Resources.Water;

                case Zelda.Status.ElementalSchool.All:
                    return Resources.Prismatic;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized string for the specified <see cref="Zelda.Status.ElementalSchool"/>.
        /// </summary>
        /// <param name="element">
        /// The enumeration to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string GetResist( Zelda.Status.ElementalSchool element )
        {
            switch( element )
            {
                case Zelda.Status.ElementalSchool.Fire:
                    return Resources.FireResistance;

                case Zelda.Status.ElementalSchool.Light:
                    return Resources.LightResistance;

                case Zelda.Status.ElementalSchool.Nature:
                    return Resources.NatureResistance;

                case Zelda.Status.ElementalSchool.Shadow:
                    return Resources.ShadowResistance;

                case Zelda.Status.ElementalSchool.Water:
                    return Resources.WaterResistance;

                case Zelda.Status.ElementalSchool.All:
                    return Resources.AllResistances;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of a gem of the given ElementalSchool.
        /// </summary>
        /// <param name="gemColor">
        /// The color of the gem.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string GetGemType( Zelda.Status.ElementalSchool gemColor )
        {
            switch( gemColor )
            {
                case Zelda.Status.ElementalSchool.Fire:
                    return Resources.FireGem;

                case Zelda.Status.ElementalSchool.Light:
                    return Resources.LightGem;

                case Zelda.Status.ElementalSchool.Nature:
                    return Resources.NatureGem;

                case Zelda.Status.ElementalSchool.Shadow:
                    return Resources.ShadowGem;

                case Zelda.Status.ElementalSchool.Water:
                    return Resources.WaterGem;

                case Zelda.Status.ElementalSchool.All:
                    return Resources.PrismaticGem;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of the given Critical Damage Bonus type.
        /// </summary>
        /// <param name="source">
        /// The source of the damage.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string GetCriticalDamageBonus( Zelda.Status.Damage.DamageSource source )
        {
            switch( source )
            {
                case Zelda.Status.Damage.DamageSource.Melee:
                    return Resources.MeleeCriticalDamageBonus;

                case Zelda.Status.Damage.DamageSource.Ranged:
                    return Resources.RangedCriticalDamageBonus;

                case Zelda.Status.Damage.DamageSource.Spell:
                    return Resources.SpellCriticalDamageBonus;

                case Zelda.Status.Damage.DamageSource.All:
                    return Resources.CriticalDamageBonus;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of the given DamageSchool enumeration.
        /// </summary>
        /// <param name="damageSchool">
        /// The DamageSchool to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string Get( Zelda.Status.Damage.DamageSchool damageSchool )
        {
            switch( damageSchool )
            {
                case Zelda.Status.Damage.DamageSchool.Magical:
                    return Resources.Magical;

                case Zelda.Status.Damage.DamageSchool.Physical:
                    return Resources.Physical;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of the given DamageSource enumeration.
        /// </summary>
        /// <param name="damageSource">
        /// The DamageSource to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string Get( Zelda.Status.Damage.DamageSource damageSource )
        {
            switch( damageSource )
            {
                case Zelda.Status.Damage.DamageSource.Melee:
                    return Resources.Melee;

                case Zelda.Status.Damage.DamageSource.Ranged:
                    return Resources.Ranged;

                case Zelda.Status.Damage.DamageSource.Spell:
                    return Resources.Spell;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of the given SpecialDamageType enumeration.
        /// </summary>
        /// <param name="damageType">
        /// The SpecialDamageType to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string Get( Zelda.Status.Damage.SpecialDamageType damageType )
        {
            switch( damageType )
            {
                case Zelda.Status.Damage.SpecialDamageType.DamagerOverTime:
                    return Resources.DamageOverTime;

                case Zelda.Status.Damage.SpecialDamageType.Poison:
                    return Resources.Poison;

                case Zelda.Status.Damage.SpecialDamageType.Bleed:
                    return Resources.Bleed;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the localized name of the given LifeMana enumeration.
        /// </summary>
        /// <param name="lifeMana">
        /// The LifeMana to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        internal static string GetRegen( Zelda.Status.LifeMana lifeMana )
        {
            switch( lifeMana )
            {
                case Zelda.Status.LifeMana.Life:
                    return Resources.LifeRegen;

                case Zelda.Status.LifeMana.Mana:
                    return Resources.ManaRegen;

                case Zelda.Status.LifeMana.Both:
                    return Resources.LifeManaRegeneration;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the localized equivalent of the given DifficultyId enumeration.
        /// </summary>
        /// <param name="difficulty">
        /// The difficulty to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string Get( Zelda.Difficulties.DifficultyId difficulty )
        {
            switch( difficulty )
            {
                case Difficulties.DifficultyId.Easy:
                    return Resources.Easy;

                case Difficulties.DifficultyId.Normal:
                    return Resources.Normal;

                case Difficulties.DifficultyId.Nightmare:
                    return Resources.Nightmare;

                case Difficulties.DifficultyId.Hell:
                    return Resources.Hell;

                case Difficulties.DifficultyId.Insane:
                    return Resources.Insane;

                default:
                    throw new NotImplementedException();
            }
        }

        public static string Get( Crafting.RecipeCategory slot )
        {
            return slot.ToString();
        }

        /// <summary>
        /// Gets the localized equivalent of the given ColorComponent enumeration.
        /// </summary>
        /// <param name="component">
        /// The component to localize.
        /// </param>
        /// <returns>
        /// The localized string.
        /// </returns>
        public static string GetShort( Atom.Xna.ColorComponent value )
        {
            switch( value )
            {
                case Atom.Xna.ColorComponent.Red:
                    return "red";

                case Atom.Xna.ColorComponent.Green:
                    return "green";

                case Atom.Xna.ColorComponent.Blue:
                    return "blue";

                default:
                    return string.Empty;
            }
        }
    }
}
