// <copyright file="StatusCalc.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Status.StatusCalc utility class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using Zelda.Items;

    /// <summary>
    /// Static utility class that encapsulates all status related calculations.
    /// </summary>
    public static class StatusCalc
    {
        #region - Item Budget -

        /// <summary>
        /// Gets the available number of Item Points for the item with the given semantics.
        /// </summary>
        /// <param name="itemLevel">
        /// The level of the item.
        /// </param>
        /// <param name="quality">
        /// The quality of the item.
        /// </param>
        /// <param name="slot">
        /// The slot the item goes in.
        /// </param>
        /// <returns>
        /// The item budget available to be spend.
        /// </returns>
        public static int GetAvailableItemBudget( int itemLevel, ItemQuality quality, EquipmentSlot slot )
        {
            float slotMod         = GetSlotMod( slot );
            float qualityMod      = GetItemQualityMod( quality );
            float qualityModExtra = GetItemQualityModExtra( quality );

            if( qualityMod == 0.0f || slotMod == 0.0f )
                return 0;

            return (int)(((itemLevel * 2.2f) - qualityModExtra) / slotMod / qualityMod);
        }
        
        /// <summary>
        /// Calculates the item budget used by an item, only taking into account the basic stats of the item.
        /// </summary>
        /// <param name="strength">The total strength of the item.</param>
        /// <param name="dexterity">The total dexterity of the item.</param>
        /// <param name="agility">The total agility of the item.</param>
        /// <param name="vitality">The total vitality of the item.</param>
        /// <param name="intelligence">The total intelligence of the item.</param>
        /// <param name="luck">The total luck of the item.</param>
        /// <returns>
        /// The calculated item budget.
        /// </returns>
        public static double GetItemBudgetUsed( int strength, int dexterity, int agility, int vitality, int intelligence, int luck )
        {
            float mod     = StatusCalc.GetStatMod( Stat.Strength );
            double budget = System.Math.Pow( strength * mod, 1.5 );

            mod    = StatusCalc.GetStatMod( Stat.Dexterity );
            budget += System.Math.Pow( dexterity * mod, 1.5 );

            mod = StatusCalc.GetStatMod( Stat.Agility );
            budget += System.Math.Pow( agility * mod, 1.5 );

            mod = StatusCalc.GetStatMod( Stat.Vitality );
            budget += System.Math.Pow( vitality * mod, 1.5 );

            mod = StatusCalc.GetStatMod( Stat.Intelligence );
            budget += System.Math.Pow( intelligence * mod, 1.5 );

            mod = StatusCalc.GetStatMod( Stat.Luck );
            budget += System.Math.Pow( luck * mod, 1.5 );

            budget = System.Math.Pow( budget, 1 / 1.5 );
            budget = System.Math.Ceiling( budget );

            return budget;
        }

        #region GetItemQualityMod

        /// <summary>
        /// Gets the modifier value for the given ItemQuality.
        /// </summary>
        /// <remarks>
        /// This value is used to calculate the item-point Budget of an item.
        /// </remarks>
        /// <param name="quality">
        /// The ItemQuality to aquire the modification value for.
        /// </param>
        /// <returns>
        /// The modifier value for the given ItemQuality.
        /// </returns>
        private static float GetItemQualityMod( ItemQuality quality )
        {
            switch( quality )
            {
                case ItemQuality.Magic:
                    return 3.0f;
                case ItemQuality.Rare:
                    return 1.9f;
                case ItemQuality.Epic:
                    return 1.6f;
                case ItemQuality.Legendary:
                    return 1.3f;
                case ItemQuality.Artefact:
                    return 1.0f;
                default:
                    return 0.0f;
            }
        }

        #endregion

        #region GetItemQualityModExtra

        /// <summary>
        /// Gets the extra modifier value for the given ItemQuality.
        /// </summary>
        /// <remarks>
        /// This value is used to calculate the item-point Budget of an item.
        /// </remarks>
        /// <param name="quality">
        /// The ItemQuality to aquire the extra modification value for.
        /// </param>
        /// <returns>
        /// The extra modifier value for the given ItemQuality.
        /// </returns>
        private static float GetItemQualityModExtra( ItemQuality quality )
        {
            switch( quality )
            {
                case ItemQuality.Rare:
                    return 4.25f;
                case ItemQuality.Magic:
                    return 3.245f;
                case ItemQuality.Epic:
                    return 2.1f;
                case ItemQuality.Legendary:
                    return 1.4f;
                case ItemQuality.Artefact:
                    return 1.0f;
                default:
                    return 0.0f;
            }
        }

        #endregion

        #region GetSlotMod

        /// <summary>
        /// Gets a modifier value for the given <see cref="EquipmentSlot"/>.
        /// </summary>
        /// <remarks>
        /// This value is used to calculate the item budget available for a specific item.
        /// </remarks>
        /// <param name="slot">
        /// The EquipmentSlot to aquire the slot modifier value for.
        /// </param>
        /// <returns>
        /// The slot modifier value for the given EquipmentSlot.
        /// </returns>
        private static float GetSlotMod( EquipmentSlot slot )
        {
            switch( slot )
            {
                case EquipmentSlot.Chest:
                    return 1.0f;
                case EquipmentSlot.Head:
                    return 1.15f;

                case EquipmentSlot.WeaponHand:
                case EquipmentSlot.Ranged:
                    return 1.20f;

                case EquipmentSlot.Staff:
                case EquipmentSlot.Gloves:
                case EquipmentSlot.Boots:
                    return 1.29f;

                case EquipmentSlot.Relic:
                case EquipmentSlot.Belt:
                case EquipmentSlot.Cloak:
                    return 1.5f;

                case EquipmentSlot.Trinket:
                case EquipmentSlot.ShieldHand:
                case EquipmentSlot.Necklace:
                case EquipmentSlot.Ring:
                    return 1.82f;
                default:
                    return -1.0f;
            }
        }

        #endregion

        #region GetStatMod

        /// <summary>
        /// Gets the value of how much the specified stat costs in terms of item budget.
        /// </summary>
        /// <remarks>
        /// This value is used to calculate the item budget a stat costs.
        /// </remarks>
        /// <param name="stat">
        /// The related Stat.
        /// </param>
        /// <returns>
        /// The modifier value for the given Stat.
        /// </returns>
        public static float GetStatMod( Stat stat )
        {
            switch( stat )
            {
                case Stat.Strength:
                case Stat.Dexterity:
                case Stat.Intelligence:
                    return 1.1f;

                case Stat.Agility:
                case Stat.Vitality:
                case Stat.Luck:
                    return 1.0f;

                default:
                    return 0.0f;
            }
        }

        #endregion

        #endregion

        #region - Attack/Cast Speed -

        /// <summary>
        /// Gets the normalized weapon speed for the weapon of the given <see cref="WeaponType"/>.
        /// </summary>
        /// <param name="weaponType">
        /// The input WeaponType.
        /// </param>
        /// <returns>
        /// The normalized speed for the given WeaponType.
        /// </returns>
        public static float GetNormalizedSpeed( WeaponType weaponType )
        {
            switch( weaponType )
            {
                case WeaponType.Rod:
                case WeaponType.OneHandedAxe:
                case WeaponType.OneHandedMace:
                case WeaponType.OneHandedSword:
                    return 2f;

                case WeaponType.Staff:
                case WeaponType.TwoHandedAxe:
                case WeaponType.TwoHandedMace:
                case WeaponType.TwoHandedSword:
                    return 3f;

                case WeaponType.Dagger:
                    return 1.75f;

                case WeaponType.Bow:
                    return 2.25f;

                case WeaponType.Crossbow:
                    return 3.0f;

                case WeaponType.None:
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Converts the given rating value into an actual modifying value.
        /// </summary>
        /// <param name="rating">
        /// The input rating value.
        /// </param>
        /// <param name="level">
        /// The level of the statable Entity that wants to use the rating value.
        /// </param>
        /// <returns>
        /// The actual converted value.
        /// </returns>
        internal static float ConvertAttackSpeedRating( float rating, int level )
        {
            if( level == 0 )
                return 0.0f;

            return rating / (level * 0.5f);
        }

        /// <summary>
        /// Gets the final attack speed based on the given parameters.
        /// </summary>
        /// <param name="weaponSpeed">The speed of the player's weapon.</param>
        /// <param name="agility">The total agility of the player.</param>
        /// <param name="multiplier">The attack-speed multiplier value.</param>
        /// <param name="ratingValue">
        /// The attack speed rating value.
        /// </param>
        /// <param name="level">
        /// The level of the statable Entity.
        /// </param>
        /// <returns>The attack speed of the player.</returns>
        internal static float GetAttackSpeed( float weaponSpeed, int agility, float multiplier, float ratingValue, int level )
        {
            multiplier -= ConvertAttackSpeedRating( ratingValue, level ) / 100.0f;

            float AgilityNeededForOnePercentReduction = 5.0f + (level / 50.0f);
            float reductionMul = (agility / AgilityNeededForOnePercentReduction) / 100.0f;

            float reduction    = weaponSpeed * reductionMul;
            float attackSpeed  = (weaponSpeed - reduction) * multiplier;

            // Can't get faster than 10% of the original speed
            if( attackSpeed < weaponSpeed * 0.1f )
                attackSpeed = weaponSpeed * 0.1f;

            return attackSpeed;
        }

        /// <summary>
        /// Gets the cast speed modifier value of the player.
        /// </summary>
        /// <param name="dexterity">
        /// The total dexterity of the player.
        /// </param>
        /// <param name="ratingValue">
        /// The spell haste rating value.
        /// </param>
        /// <param name="percentualValue">
        /// The percentual speed modifier value.
        /// </param>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// The cast speed modifier value.
        /// </returns>
        internal static float GetCastSpeedModifier( int dexterity, float ratingValue, float percentualValue, int level )
        {
            float DexterityForOnePercent = 10.0f + (level / 10.0f);

            float fromRating     = ConvertRating( ratingValue, level ) / 100.0f;
            float fromDexterity  = (dexterity / DexterityForOnePercent / 100.0f);
            float fromPercentage = percentualValue - 1.0f;

            float modifier = 1.0f - fromRating - fromDexterity - fromPercentage;
            if( modifier < 0.1f )
                modifier = 0.1f;

            return modifier;
        }

        #endregion

        #region - Leveling -

        /// <summary>
        /// Gets the expierence needed for the specified level to levelup.
        /// </summary>
        /// <param name="level">
        /// The input level.
        /// </param>
        /// <returns>
        /// The experience required for the given level.
        /// </returns>
        internal static long GetExperienceNeeded( int level )
        {
            if( level <= 0 )
                return 0;
            if( level == 1 )
                return 7;
            if( level == 2 )
                return 20;
            if( level >= 101 )
                return GetExperienceRecursive( 101 );

            return GetExperienceRecursive( level );
        }

        /// <summary>
        /// Gets the expierence needed for the specified level to levelup.
        /// </summary>
        /// <param name="level">
        /// The input level.
        /// </param>
        /// <returns>
        /// The experience required for the given level.
        /// </returns>
        private static long GetExperienceRecursive( int level )
        {
            if( level == 3 )
                return 33;

            long prevEp = GetExperienceNeeded( level - 1 );
            if( level % 2 == 0 )
                return (long)(prevEp * 1.25);
            else
                return (long)(prevEp * 1.10);
        }

        /// <summary>
        /// Gets the amount of extra Stat Points one gains at the specified <paramref name="level"/>.
        /// </summary>
        /// <param name="level">
        /// The new level.
        /// </param>
        /// <returns>
        /// The number of stat points awarded at the next level.
        /// </returns>
        internal static int GetStatPointGainedForLevelUp( int level )
        {
            int points = 5 + ((level +  1) / 3) + (level % 2 == 0 ? 3 : 0);

            // Every level the factor increases.
            float factor = 1.0f + (level * 0.0125f);
            points = (int)(points * factor);

            return points;
        }

        /// <summary>
        /// Tries to estimate the number of stat points the player has got.
        /// </summary>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// The number of stat points.
        /// </returns>
        internal static int EstimateStatPointsGained( int level )
        {
            int statPoints = ExtendedStatable.InitialFreeStatPoints;

            for( int currentLevel = 2; currentLevel <= level; ++currentLevel )
            {
                statPoints += GetStatPointGainedForLevelUp( currentLevel );
            }

            return statPoints;
        }

        #endregion

        #region GetStatPointsNeeded

        /// <summary>
        /// Gets the number of stat-points needed to increase the specified <paramref name="baseValue"/> by one.
        /// </summary>
        /// <param name="baseValue">
        /// The base value of the stat.
        /// </param>
        /// <returns>
        /// The amount of stat-points required.
        /// </returns>
        internal static int GetStatPointsNeeded( int baseValue )
        {
            return ((baseValue - 1) / 10) + 2;
        }

        #endregion

        #region GetMagicFindExtra

        /// <summary>
        /// Gets the amount of additional Magic Find provided by Luck.
        /// </summary>
        /// <param name="luck">
        /// The total luck of the player.
        /// </param>
        /// <returns>
        /// The magic find value awarded by luck.
        /// </returns>
        internal static float GetMagicFindExtra( int luck )
        {
            return (luck / 6.0f) + ((luck / 10) * 2);
        }

        #endregion
        
        #region GetMitigationFromArmor

        /// <summary>
        /// Gets the mitigation % the armor provides against an object with the specified level.
        /// </summary>
        /// <param name="targetArmor">
        /// The armor of the target.
        /// </param>
        /// <param name="attackerLevel">
        /// The level of the attacker.
        /// </param>
        /// <returns>
        /// The mitigation armor provides.
        /// </returns>
        public static float GetMitigationFromArmor( int targetArmor, int attackerLevel )
        {
            if( targetArmor < 0 )
                return 0.0f;

            float mitigation = targetArmor / ((targetArmor / 110.0f) + (2.0f + (attackerLevel * 0.85f)));

            if( mitigation > 75.0f )
                return 75.0f;

            return mitigation;
        }

        #endregion

        #region GetAdditionalArmor

        /// <summary>
        /// Gets the additional armor provided by agility and luck.
        /// </summary>
        /// <param name="agility">The total agility of the player.</param>
        /// <returns>The additional armor.</returns>
        internal static int GetAdditionalArmor( int agility )
        {
            return (int)(agility * 1.456789f);
        }

        #endregion

        #region GetAdditionalBlockValue

        /// <summary>
        /// Gets the additional block value provided by vitality.
        /// </summary>
        /// <param name="strength">
        /// The total strength of the player.
        /// </param>
        /// <returns>
        /// The additional block value.
        /// </returns>
        internal static float GetAdditionalBlockValue( int strength )
        {
            const int BaseBlockValue = 1;
            const int BonusEvery10Strength = 2;
            const float StrengthNeededFor1BlockValue = 4.5f;

            float blockFromStrength = strength / StrengthNeededFor1BlockValue;
            int bonusBlockEvery10 = (strength / 10) * BonusEvery10Strength;

            return BaseBlockValue + blockFromStrength + bonusBlockEvery10;
        }

        #endregion
        
        /// <summary>
        /// Calculates the armor ignore multiplier value.
        /// </summary>
        /// <param name="percentalValue">The percentual armor ignore value.</param>
        /// <param name="ratingValue">The rating armor ignore value.</param>
        /// <param name="level">The level of the player.</param>
        /// <returns>The calculated multiplier.</returns>
        internal static float GetArmorIgnoreMultiplier( float percentalValue, float ratingValue, int level )
        {
            float multiplier = percentalValue - (StatusCalc.ConvertArmorIgnoreRating( ratingValue, level ) / 100.0f); 
            return Atom.Math.MathUtilities.Clamp( multiplier, 0.5f, 1.0f );
        }

        /// <summary>
        /// Converts armor ignore rating into armor ignore %.
        /// </summary>
        /// <param name="rating">The rating value.</param>
        /// <param name="level">The level of the player.</param>
        /// <returns>The coverted value.</returns>
        internal static float ConvertArmorIgnoreRating( float rating, int level )
        {
            if( level == 0 )
                return 0;

            return rating / (level * 0.5f);
        }

        /// <summary>
        /// Converts the given rating value into a fixed value.
        /// </summary>
        /// <param name="rating">The rating value to convert.</param>
        /// <param name="level">The level of the entity.</param>
        /// <returns>The converted fixed ChanceToStatus value.</returns>
        internal static float ConvertRating( float rating, int level )
        {
            if( level == 0 )
                return 0.0f;

            return rating / (level * 0.5f);
        }

        /// <summary>
        /// Calculates the movement speed of an extended-statable/moveable ZeldaEntity.
        /// </summary>
        /// <param name="baseSpeed">The base movement speed.</param>
        /// <param name="agility">The total agility of the extended-statable entity.</param>
        /// <param name="addValue">The fixed value.</param>
        /// <param name="mulValue">The multiplicative value.</param>
        /// <param name="ratingValue">The rating value.</param>
        /// <param name="level">The level of the entity.</param>
        /// <returns>
        /// The movement speed.
        /// </returns>
        internal static float GetMovementSpeed( float baseSpeed, int agility, float addValue, float mulValue, float ratingValue, int level )
        {
            float speedFromAgility = agility / 150.0f;
            float speedFromRating = ConvertRating( ratingValue, level ) ;

            float fixedSpeed = (baseSpeed + addValue + speedFromAgility + speedFromRating);

            float multiplier = mulValue;
            float finalSpeed = fixedSpeed * multiplier;

            return finalSpeed;
        }

        /// <summary>
        /// Calculates the movement speed of a statable/moveable ZeldaEntity.
        /// </summary>
        /// <param name="baseSpeed">The base movement speed.</param>
        /// <param name="addValue">The fixed value.</param>
        /// <param name="mulValue">The multiplicative value.</param>
        /// <param name="ratingValue">The rating value.</param>
        /// <param name="level">The level of the entity.</param>
        /// <returns>
        /// The movement speed.
        /// </returns>
        internal static float GetMovementSpeed( float baseSpeed, float addValue, float mulValue, float ratingValue, int level )
        {
            return GetMovementSpeed( baseSpeed, 0, addValue, mulValue, ratingValue, level );
        }

        #region - Resistances -

        /// <summary>
        /// Converts the given resistance rating value into the actual chance to resist.
        /// </summary>
        /// <param name="rating">The input resist rating value.</param>
        /// <param name="level">The level of the statable entity.</param>
        /// <returns>The chance to resist the given rating provides.</returns>
        internal static float ConvertResistRating( float rating, int level )
        {
            return rating / (1.0f + (1.7f * level));
        }

        /// <summary>
        /// Gets the chance for magical attacks to be resisted.
        /// </summary>
        /// <param name="fixedValue">
        /// The fixed chance to be resisted.
        /// </param>
        /// <param name="ratingValue">
        /// The rating value that is converted into a fixed chance.
        /// </param>
        /// <param name="multiplicativeValue">
        /// The multiplier that is applied to the result.
        /// </param>
        /// <param name="level">
        /// The level of the entity.
        /// </param>
        /// <returns>
        /// The final additional chance for spells to not be resisted.
        /// </returns>
        internal static float GetChanceToBeResisted( float fixedValue, float ratingValue, float multiplicativeValue, int level )
        {
            return -GetChanceToResist( fixedValue, ratingValue, multiplicativeValue, level );
        }
        
        /// <summary>
        /// Gets the chance to resist a magical attack.
        /// </summary>
        /// <param name="fixedValue">The fixed resist chance.</param>
        /// <param name="ratingValue">The resist rating value.</param>
        /// <param name="multiplicativeValue">The multiplicator value to apply.</param>
        /// <param name="level">The level of the statable entity.</param>
        /// <returns>The final chance to resist the given rating provides.</returns>
        internal static float GetChanceToResist( float fixedValue, float ratingValue, float multiplicativeValue, int level )
        {
            return (fixedValue + ConvertResistRating( ratingValue, level )) * multiplicativeValue;
        }

        #endregion

        #region - Chance To -
        
        /// <summary>
        /// Converts the given <see cref="ChanceToStatus"/> rating value into a fixed value.
        /// </summary>
        /// <param name="rating">The rating value to convert.</param>
        /// <param name="stat">The related ChanceToStatus.</param>
        /// <param name="level">The level of the entity.</param>
        /// <returns>The converted fixed ChanceToStatus value.</returns>
        internal static float ConvertRating( float rating, ChanceToStatus stat, int level )
        {
            return ConvertRating( rating, level );
        }

        /// <summary>
        /// Gets the chance to dodge melee and ranged attacks in %.
        /// </summary>
        /// <param name="agility">The total agility of the statable entity.</param>
        /// <param name="fixedValue">The additive/fixed extra dodge chance.</param>
        /// <param name="multiplier">The percental extra dodge chance.</param>
        /// <param name="ratingValue">The dodge rating value.</param>
        /// <param name="level">The level of the statable entity.</param>
        /// <returns>
        /// The total chance to dodge.
        /// </returns>
        internal static float GetChanceToDodge( int agility, float fixedValue, float multiplier, float ratingValue, int level )
        {
            float AgiToDodgeRatio = 10.0f + ((float)level / 30.0f);
            const float BaseChance = 2.4f;
                        
            int agilityDiv10 = agility / 10;
            float ratingBonus = (agilityDiv10 * agilityDiv10) / 20.0f;
            ratingValue += ratingBonus;

            float dodgeFromRating = StatusCalc.ConvertRating( ratingValue, ChanceToStatus.Dodge, level );
            float dodge = (BaseChance + (agility / AgiToDodgeRatio) + fixedValue + dodgeFromRating) * multiplier;

            if( dodge > 75.0f )
                dodge = 75.0f;

            return dodge;
        }
        
        /// <summary>
        /// Gets the chance to parry melee attacks in %.
        /// </summary>
        /// <param name="strength">The total strength of the statable entity.</param>
        /// <param name="fixedValue">The additive/fixed extra parry chance.</param>
        /// <param name="multiplier">The percental extra parry chance.</param>
        /// <param name="ratingValue">The parry rating value.</param>
        /// <param name="level">The level of the statable entity.</param>
        /// <returns>
        /// The chance to parry.
        /// </returns>
        internal static float GetChanceToParry( int strength, float fixedValue, float multiplier, float ratingValue, int level )
        {
            float StrengthToParryRatio = 40.5f + ((float)level / 25.0f);
            const float BaseChance = 2.48f;

            int strengthDiv10 = strength / 10;
            float ratingBonus = (strengthDiv10 * strengthDiv10) / 20.0f;
            ratingValue += ratingBonus;

            float parryFromRating = StatusCalc.ConvertRating( ratingValue, ChanceToStatus.Parry, level );
            float parry = (BaseChance + (strength / StrengthToParryRatio) + fixedValue + parryFromRating) * multiplier;

            if( parry > 50.0f )
                parry = 50.0f;

            return parry;
        }

        /// <summary>
        /// Gets the chance to get a critical hit in %.
        /// </summary>
        /// <param name="luck">The total luck of the player.</param>
        /// <param name="fixedValue">The additive extra crit chance.</param>
        /// <param name="multiplier">The multiplicative extra crit chance multiplier value.</param>
        /// <param name="ratingValue">The dodge rating value.</param>
        /// <param name="level">The level of the statable entity.</param>
        /// <returns>The total crit chance.</returns>
        internal static float GetChanceToCrit( int luck, float fixedValue, float multiplier, float ratingValue, int level )
        {
            float LuckToCritRatio = 9.75f + ((float)level / 50.0f);
            const float BaseChance = 2.40f;

            int luckDiv10 = luck / 10;
            float ratingBonus = (luckDiv10 * luckDiv10) / 3.0f;
            ratingValue += ratingBonus;

            float critFromRating = ConvertRating( ratingValue, ChanceToStatus.Crit, level );
            float value          = ((luck / LuckToCritRatio) + fixedValue + critFromRating + BaseChance) * multiplier;

            if( value < 0.0f )
                return 0.0f;
            if( value > 100.0f )
                return 100.0f;
            return value;
        }
        
        /// <summary>
        /// Gets the chance to miss in %.
        /// </summary>
        /// <param name="dexterity">The total dexterity of the StatusObject.</param>
        /// <param name="fixedExtraChance">The additive extra hit chance.</param>
        /// <param name="multiplier">The multiplicative hit chance multiplier value.</param>
        /// <param name="ratingValue">The hit rating value.</param>
        /// <param name="level">The level of the player.</param>
        /// <returns>
        /// The chance for the player to miss.
        /// </returns>
        internal static float GetChanceToMiss( int dexterity, float fixedExtraChance, float multiplier, float ratingValue, int level )
        {
            float DexterityToHitRatio = 20.0f + ((float)level / 25.0f);
            const float BaseChance = 10.05f;

            float miss = BaseChance;

            miss += fixedExtraChance;
            miss += ConvertRating( ratingValue, ChanceToStatus.Miss, level );   
            miss -= (float)(dexterity / DexterityToHitRatio);         
            miss *= multiplier;

            if( miss < 0.0f )
                miss = 0.0f;

            return miss;
        }

        /// <summary>
        /// Calculates the chance for ranged projectiles to pierce through enemies.
        /// </summary>
        /// <param name="fixedValue">The additive modifier value.</param>
        /// <param name="multiplier">The multiplcative modifier value.</param>
        /// <param name="ratingValue">The rating value that gets converted into a fixed increase.</param>
        /// <param name="level">The level of the player.</param>
        /// <returns>The total chance to pierce.</returns>
        internal static float GetChanceToPierce( float fixedValue, float multiplier, float ratingValue, int level )
        {
            fixedValue += ConvertRating( ratingValue, level );

            const float BaseChance = 5.0f;
            float piercing = (BaseChance + fixedValue) * multiplier;

            if( piercing > 75.0f )
                piercing = 75.0f;

            return piercing;
        }

        /// <summary>
        /// Gets the chance to block melee and ranged attacks in %.
        /// </summary>
        /// <param name="fixedValue">
        /// The additive/fixed extra block chance chance.
        /// </param>
        /// <param name="multiplier">
        /// The percental extra block chance.
        /// </param>
        /// <param name="ratingValue">
        /// The block chance rating value.
        /// </param>
        /// <param name="level">
        /// The level of the statable entity.
        /// </param>
        /// <returns>
        /// The total chance to dodge.
        /// </returns>
        internal static float GetChanceToBlock( float fixedValue, float multiplier, float ratingValue, int level )
        {
            const float BaseChance = 25.0f;

            float blockChanceFromRating = StatusCalc.ConvertRating( ratingValue, ChanceToStatus.Block, level );
            float blockChance           = (BaseChance + fixedValue + blockChanceFromRating) * multiplier;

            if( blockChance > 75.0f )
                blockChance = 75.0f;

            return blockChance;
        }

        #endregion

        #region - Chance To Be -
        
        /// <summary>
        /// Gets the chance to be crit, not taking into account the crit chance of the attacker.
        /// </summary>
        /// <param name="vitality">The total vitality of the player.</param>
        /// <param name="fixedValue">The fixed modifier value (from items/talents).</param>
        /// <returns>The chance to be crit that gets added to the attackers crit chance.</returns>
        internal static float GetChanceToBeCrit( int vitality, float fixedValue )
        {
            // 125 Vitality reduces chance to be crit by 1%.
            // 250 Vitality reduces chance to be crit by 2%.
            // ...
            return fixedValue - (vitality / 125.0f);
        }

        /// <summary>
        /// Gets the chance to be hit, not taking into account the hit chance of the attacker.
        /// </summary>
        /// <param name="fixedValue">The fixed modifier value (from items/talents).</param>
        /// <param name="percentValue">The multiplier modifier value (from items/talents).</param>
        /// <param name="ratingValue">The rating modifier value (from items/talents).</param>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// <returns>
        /// The chance to be hit that gets added to the attackers crit chance.</returns>
        /// </returns>
        internal static float GetChanceToBeHit( float fixedValue, float percentValue, float ratingValue, int level )
        {
            return (fixedValue + ConvertRating( ratingValue, level )) * percentValue;
        }

        #endregion

        #region - Damage -

        /// <summary>
        /// Gets the Critical Damage Bonus Modifier value.
        /// </summary>
        /// <param name="baseModifier">
        /// The fixed base value.
        /// </param>
        /// <param name="percentalValue">
        /// The fixed percental modifier value.
        /// </param>
        /// <param name="ratingValue">
        /// The rating value.
        /// </param>
        /// <param name="level">
        /// The level of the statable ZeldaEntity.
        /// </param>
        /// <returns>
        /// The Critical Damage Bonus Modifier.
        /// </returns>
        internal static float GetCriticalDamageBonusModifier(
            float baseModifier,
            float percentalValue,
            float ratingValue,
            int level )
        {
            float modifierFromRating = StatusCalc.ConvertRating( ratingValue, level ) / 100.0f;
            float modifier = percentalValue + modifierFromRating;

            return baseModifier + (modifier - 1.0f);
        }

        #region > Melee <

        /// <summary>
        /// Gets the increase of (minimum) Melee Damage provided by the given Stats.
        /// </summary>
        /// <param name="strength">The total strength of the player.</param>
        /// <param name="dexterity">The total dexterty of the player</param>
        /// <param name="attackSpeed">The melee attack speed of the player.</param>
        /// <returns>The minimum melee damage increase.</returns>s
        internal static int GetMinimumMeleeDamageIncrease( int strength, int dexterity, float attackSpeed )
        {
            int strengthDiv10 = strength / 10;
            int dextertyDiv10 = dexterity / 10;

            float apFromStr = (strength * 1.35f) + (strengthDiv10 * 20.0f);
            float apFromDex = dexterity          + (dextertyDiv10 * 25.0f);

            const int BaseAttackPower = 10;
            float attackPower = BaseAttackPower + apFromStr + apFromDex;
            
            const float AttackPowerForOneDps = 10.0f;
            return (int)System.Math.Floor( attackPower * attackSpeed / AttackPowerForOneDps );
        }

        /// <summary>
        /// Gets the increase of (maximum) Melee Damage provided by the given Stats.
        /// </summary>
        /// <param name="strength">The total strength of the player.</param>
        /// <param name="dexterity">The total dexterty of the player</param>
        /// <param name="attackSpeed">The melee attack speed of the player.</param>
        /// <returns>The maximum melee damage increase.</returns>
        internal static int GetMaximumMeleeDamageIncrease( int strength, int dexterity, float attackSpeed )
        {
            int strengthDiv10 = strength / 10;
            int dextertyDiv10 = dexterity / 10;

            float apFromStr = (strength * 4.25f) + (strengthDiv10 * 55.0f);
            float apFromDex = (dexterity / 1.5f) + (dextertyDiv10 * 10.0f);

            const int BaseAttackPower = 12;
            float attackPower = BaseAttackPower + apFromStr + apFromDex;

            const float AttackPowerForOneDps = 10.0f;
            return (int)System.Math.Floor( attackPower * attackSpeed / AttackPowerForOneDps );
        }

        #endregion

        #region > Ranged <

        /// <summary>
        /// Gets the increase of (minimum) Ranged Damage provided by the given Stats.
        /// </summary>
        /// <param name="dexterity">
        /// The total dexterity.
        /// </param>
        /// <param name="attackSpeed">
        /// The base attack speed of the ranged-weapon.
        /// </param>
        /// <returns>The minumum ranged damage increase.</returns>
        internal static int GetMinimumRangedDamageIncrease( int dexterity, float attackSpeed )
        {
            const int BaseAttackPower = 10;
            int dexterityDiv10 = dexterity / 10;

            float apFromDex   = (dexterity * 1.65f) + (dexterity / 4.0f) + (dexterityDiv10 * 35.0f);
            float attackPower = BaseAttackPower + apFromDex;

            const float AttackPowerForOneDps = 10.0f;
            return (int)System.Math.Floor( attackPower * attackSpeed / AttackPowerForOneDps );
        }

        /// <summary>
        /// Gets the increase of (maximum) Ranged Damage provided by the given Stats.
        /// </summary>
        /// <param name="dexterity">
        /// The total dexterity.
        /// </param>
        /// <param name="agility">
        /// The total agility.
        /// </param>
        /// <param name="attackSpeed">
        /// The base attack speed of the ranged-weapon.
        /// </param>
        /// <returns>The maximum ranged damage increase.</returns>
        internal static int GetMaximumRangedDamageIncrease( int dexterity, int agility, float attackSpeed )
        {
            const int BaseAttackPower = 10;
            int dexterityDiv10 = dexterity / 10;
            int agilityDiv10   = dexterity / 10;

            float apFromDex = (dexterity * 2.65f) + (dexterity / 2.0f) + (dexterityDiv10 * 50.0f);
            float apFromAgi = (agility / 1.25f) + (agilityDiv10 * 15.0f);

            float attackPower = BaseAttackPower + apFromDex + apFromAgi;
         
            const float AttackPowerForOneDps = 10.0f;
            return (int)System.Math.Floor( attackPower * attackSpeed / AttackPowerForOneDps );
        }

        #endregion

        #region > Magic <

        /// <summary>
        /// Gets the increase of (minimum) Magic Damage provided by the given Stats.
        /// </summary>
        /// <param name="intelligence">The total intelligence of the player.</param>
        /// <returns>The minimum magic damage increase.</returns>
        internal static int GetMinimumMagicDamageIncrease( int intelligence )
        {
            int bonusDamagePer10 = Math.Min( (intelligence / 5), 5 );
            int bonus = (intelligence / 10) * bonusDamagePer10;

            return (int)( (intelligence * 0.825) + bonus);
        }

        /// <summary>
        /// Gets the increase of (maximum) Magic Damage provided by the given Stats.
        /// </summary>
        /// <param name="intelligence">The total intelligence of the player.</param>
        /// <returns>The maximum magic damage increase.</returns>
        internal static int GetMaximumMagicDamageIncrease( int intelligence )
        {
            int bonusDamagePer10 = Math.Min( (intelligence / 6), 15 );
            int bonus = (intelligence / 10) * bonusDamagePer10;

            return (int)((intelligence * 1.15) + bonus);
        }

        #endregion

        #endregion

        #region - Life / Mana -

        #region GetTotalLife

        /// <summary>
        /// Calculates the total life of a PlayerObject.
        /// </summary>
        /// <param name="level">The level of the player.</param>
        /// <param name="vitality">The total vitality of the player.</param>
        /// <param name="fixedIncrease">The fixed life increase.</param>
        /// <param name="multiplier">The percental life increase.</param>
        /// <returns>The total life of the player.</returns>
        internal static int GetTotalLife( int level, int vitality, float fixedIncrease, float multiplier )
        {
            int lifeFromLevel = GetLifeFromLevel( level );

            const int BaseLife     = 30;
            const int BonusPer10   = 30;
            const float LifePerVit = 10.0f;

            return (int)(((BaseLife + lifeFromLevel) + fixedIncrease + (vitality * LifePerVit) + ((vitality / 10) * BonusPer10)) * multiplier);
        }

        /// <summary>
        /// Gets the life given for free each level-up.
        /// </summary>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// The life given for free.
        /// </returns>
        private static int GetLifeFromLevel( int level )
        {
            int lifeFromLevel = level;

            for( int i = 1; i < level; ++i )
            {
                lifeFromLevel += i / 2;
            }

            return (int)(lifeFromLevel * 1.45f);
        }

        #endregion

        #region GetTotalMana

        /// <summary>
        /// Calculates the base mana pool of the extended statable ZeldaEntity.
        /// </summary>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// The base mana of the player.
        /// </returns>
        internal static int GetBaseMana( int level )
        {
            int manaFromLevel = level;
            for( int i = 1; i < level; ++i )
                manaFromLevel += i / 2;

            manaFromLevel = (int)(manaFromLevel * 1.425f);

            const int BaseMana = 20;
            return BaseMana + manaFromLevel;
        }

        /// <summary>
        /// Calculates the total mana pool of the player.
        /// </summary>
        /// <param name="baseMana">The base mana of the player.</param>
        /// <param name="intelligence">The total intelligence.</param>
        /// <param name="fixedIncrease">The fixed mana increase.</param>
        /// <param name="multiplier">The percental mana increase.</param>
        /// <returns> The total mana of the player.</returns>
        internal static int GetTotalMana(
            int baseMana,
            int intelligence,
            float fixedIncrease,
            float multiplier )
        {
            const float BonusPer10 = 20.0f;
            const float ManaPerInt = 5.25f;
            float bonusMana = (intelligence / 10) * BonusPer10;

            int totalMana = (int)((baseMana + fixedIncrease + (intelligence * ManaPerInt) + bonusMana) * multiplier);

            return totalMana;
        }

        #endregion

        #region GetLifeRegen

        /// <summary>
        /// Calculates the amount of life regenerated each tick.
        /// </summary>
        /// <param name="vitality">
        /// The total amount of vitality.
        /// </param>
        /// <param name="totalLife">
        /// The total amount of life.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed value.
        /// </param>
        /// <param name="multiplier">
        /// The multiplier value.
        /// </param>
        /// <returns>
        /// The life regenerated each tick.
        /// </returns>
        internal static int GetLifeRegen( int vitality, int totalLife, float fixedIncrease, float multiplier )
        {
            return (int)((1 + (vitality / 2.25f) + (totalLife / 80.0f) + fixedIncrease) * multiplier);
        }

        #endregion

        #region GetManaRegen

        /// <summary>
        /// Calculates the amount of mana regenerated each tick.
        /// </summary>
        /// <param name="intelligence">
        /// The total amount of intelligence.
        /// </param>
        /// <param name="totalMana">
        /// The total amount of mana.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed value.
        /// </param>
        /// <param name="multiplier">
        /// The multiplier value.
        /// </param>
        /// <returns>
        /// The mana regenerated each tick.
        /// </returns>
        internal static int GetManaRegen( int intelligence, int totalMana, float fixedIncrease, float multiplier )
        {
            float bonusRegenEvery10 = 1 + (intelligence / 30.0f);
            int bonusFromInt = (int)((intelligence / 10) * bonusRegenEvery10);

            return (int)((1 + (intelligence / 7.0f) + (totalMana / 40.0f) + fixedIncrease + bonusFromInt) * multiplier);
        }

        #endregion

        #endregion        

        #region - Potion Effectiviness -

        /// <summary>
        /// Calculates the total effectiviness bonus the player
        /// gets for life potions.
        /// </summary>
        /// <param name="vitality">
        /// The total vitality of the player.
        /// </param>
        /// <param name="baseEffectiviness">
        /// The base effectiviness value of the player.
        /// </param>
        /// <returns>
        /// The effectiviness value for life potions.
        /// </returns>
        internal static float GetLifePotionEffectiviness( int vitality, float baseEffectiviness )
        {
            return baseEffectiviness + ((vitality / 1000.0f) * 2.0f);
        }

        /// <summary>
        /// Calculates the total effectiviness bonus the player
        /// gets for mana potions.
        /// </summary>
        /// <param name="intelligence">
        /// The total intelligence of the player.
        /// </param>
        /// <param name="baseEffectiviness">
        /// The base effectiviness value of the player.
        /// </param>
        /// <returns>
        /// The effectiviness value for mana potions.
        /// </returns>
        internal static float GetManaPotionEffectiviness( int intelligence, float baseEffectiviness )
        {
            return baseEffectiviness + ((intelligence / 1000.0f) * 2.0f);
        }

        #endregion
    }
}
