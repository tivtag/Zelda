// <copyright file="ExtendedStatable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.ExtendedStatable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Zelda.Entities;
    using Zelda.Items;
    using Zelda.Saving;
    using Zelda.Status.Containers;
    using Zelda.Status.Damage;
    using Zelda.Status.Damage.Containers;

    /// <summary>
    /// Defines a <see cref="Statable"/> that provides extended functionallity.
    /// </summary>
    /// <remarks>
    /// This component is currently only used by the PlayerEntity,
    /// all other statable entities use the simpler Statable component.
    /// </remarks>
    public sealed class ExtendedStatable : Statable
    {
        #region [ Constants ]

        /// <summary>
        /// The initial number of free stat points an extended-statable ZeldaEntity has.
        /// </summary>
        internal const int InitialFreeStatPoints = 24;

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when the extended-statable ZeldaEntity
        /// has received a level-up.
        /// </summary>
        public event EventHandler LevelUped;

        /// <summary>
        /// Fired when the extended-statable ZeldaEntity
        /// has gained experience.
        /// </summary>
        public event Atom.RelaxedEventHandler<long> ExperienceGained;

        /// <summary>
        /// Called when the total <see cref="Agility"/> of the 
        /// extended-statable ZeldaEntity has been refreshed.
        /// </summary>
        public event EventHandler AgilityUpdated;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the container that manages and stores the spell power properties of this ExtendedStatable.
        /// </summary>
        public SpellPowerContainer SpellPower
        {
            get
            {
                return this.spellPower;
            }
        }

        /// <summary>
        /// Gets the container that manages and stores the damage done properties of this ExtendedStatable.
        /// </summary>
        public new ExtendedDamageDoneContainer DamageDone
        {
            get
            {
                return this.damageDone;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates various
        /// chance-to properties of this ExtendedStatable.
        /// </summary>
        public new ExtendedChanceToContainer ChanceTo
        {
            get
            {
                return this.chanceTo;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates various
        /// chance-to-be properties of this ExtendedStatable.
        /// </summary>
        public new ExtendedChanceToBeContainer ChanceToBe
        {
            get
            {
                return this.chanceToBe;
            }
        }

        /// <summary>
        /// Gets the container that encapuslates the stat modifiers that are applied to the stats
        /// given from items with a specific <see cref="EquipmentSlot"/>.
        /// </summary>
        public EquipmentSlotStatModifierContainer EquipmentSlotStatModifiers
        {
            get
            {
                return this.equipmentSlotStatModifiers;
            }
        }

        /// <summary>
        /// Gets or sets the number of rubies the extended statable ZeldaEntity has.
        /// </summary>
        public long Rubies
        {
            get 
            {
                return this.rubies;
            }

            set
            {
                if( value < 0 )
                    value = 0;

                this.rubies = value;
            }
        }

        /// <summary>
        /// Gets the experience of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public long Experience
        {
            get 
            {
                return this.experience;
            }
        }

        /// <summary>
        /// Gets the experience needed by the extended-statable <see cref="ZeldaEntity"/> to level-up.
        /// </summary>
        public long ExperienceNeeded
        {
            get 
            {
                return this.experienceNeeded;
            }
        }

        /// <summary>
        /// Gets the number of stat points that haven't been invested by the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int FreeStatPoints
        {
            get 
            { 
                return this.freeStatPoints; 
            }
        }
        
        /// <summary>
        /// Gets the number of stat points that have been awarded to the player so far.
        /// </summary>
        public int TotalStatPoints
        {
            get
            {
                return this.totalStatPoints;
            }
        }

        /// <summary>
        /// Gets or sets the level of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public override int Level
        {
            get
            {
                return base.Level;
            }

            set
            {
                base.Level = value;

                this.experienceNeeded = StatusCalc.GetExperienceNeeded( value );
                this.RefreshStatus_LevelBased();
            }
        }

        /// <summary>
        /// Gets the <see cref="SharedCooldownMap"/> which stores the shared <see cref="Cooldown"/>s 
        /// corresponding to the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public SharedCooldownMap SharedCooldowns
        {
            get
            {
                return this.sharedCooldowns; 
            }
        }

        /// <summary>
        /// Gets the <see cref="EquipmentStatus"/> of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public EquipmentStatus Equipment
        {
            get
            {
                return this.equipmentStatus;
            }
        }

        /// <summary>
        /// Gets the value that is multiplicated with the final pushing force of an attack.
        /// </summary>
        public float PushingForceMultiplicative
        {
            get 
            { 
                return this.pushingForceMultiplicative;
            }
        }

        /// <summary>
        /// Gets the value that is added to the final pushing force of an attack.
        /// </summary>
        public float PushingForceAdditive
        {
            get
            {
                return this.pushingForceAdditive; 
            }
        }

        /// <summary>
        /// Gets the magic find value of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <remarks>
        /// This value represents a multiplicand that is integrated into the
        /// Drop Chance of Rare Items. A higher MF value increases the chance
        /// to find those Rare Items.
        /// </remarks>
        public float MagicFind
        {
            get 
            {
                return this.magicFind;
            }
        }

        /// <summary>
        /// Gets the average of the melee AttackSpeed and the melee WeaponSpeed.
        /// </summary>
        public float AttackSpeedMeleeNormalized
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the average of the ranged AttackSpeed and the ranged WeaponSpeed.
        /// </summary>
        public float AttackSpeedRangedNormalized
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the flat amount of enemy armor this extended-statable ignores.
        /// </summary>
        public int ArmorIgnore
        {
            get
            {
                return this.armorIgnore;
            }
        }

        /// <summary>
        /// Gets the percentage of enemy armor this extended-statable ignores.
        /// </summary>
        public float ArmorIgnoreMultiplier
        {
            get
            {
                return this.armorIgnoreMultiplier;
            }
        }

        /// <summary>
        /// Gets the fixed additional experience that is added to the experience the extended-statable ZeldaEntity gains.
        /// </summary>
        /// <remarks>
        /// This won't modify experience gained by Quests.
        /// </remarks>
        public int FixedExperienceGainedModifier 
        {
            get
            {
                return this.fixedExperienceGainedModifier;
            }
        }

        /// <summary>
        /// Gets the modifer applied to experience the extended-statable ZeldaEntity gains.
        /// </summary>
        /// <remarks>
        /// This won't modify experience gained by Quests.
        /// </remarks>
        public float ExperienceGainedModifier
        {
            get
            {
                return this.experienceGainedModifier;
            }
        }

        #region > Damage <

        /// <summary>
        /// Gets or sets the normalized physcial damage the ZeldaEntity can do with melee attacks.
        /// </summary>
        /// <remarks>
        /// Normalized damage is calculated using the normalized weapon speed of the equipped weapon,
        /// instead of the actual weapon speed.
        /// The normalized weapon speed can be looked-up using the StatusCalc.
        /// </remarks>
        public int DamageMeleeNormalizedMin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the normalized physcial damage the ZeldaEntity can do with melee attacks.
        /// </summary>   
        /// <remarks>
        /// Normalized damage is calculated using the normalized weapon speed of the equipped weapon,
        /// instead of the actual weapon speed.
        /// The normalized weapon speed can be looked-up using the StatusCalc.
        /// </remarks>
        public int DamageMeleeNormalizedMax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the normalized physcial damage the ZeldaEntity can do with ranged attacks.
        /// </summary>
        /// <remarks>
        /// Normalized damage is calculated using the normalized weapon speed of the equipped weapon,
        /// instead of the actual weapon speed.
        /// The normalized weapon speed can be looked-up using the StatusCalc.
        /// </remarks>
        public int DamageRangedNormalizedMin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the normalized physcial damage this <see cref="ZeldaEntity"/> can do with ranged attacks.
        /// </summary>   
        /// <remarks>
        /// Normalized damage is calculated using the normalized weapon speed of the equipped weapon,
        /// instead of the actual weapon speed.
        /// The normalized weapon speed can be looked-up using the StatusCalc.
        /// </remarks>
        public int DamageRangedNormalizedMax
        {
            get;
            set;
        }

        #endregion

        #region > Potion Effectiviness <

        /// <summary>
        /// Gets the effectiviness multiplier for Life Potions 
        /// this extended-statable <see cref="ZeldaEntity"/> uses.
        /// </summary>
        public float LifePotionEffectiviness
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the effectiviness multiplier for Mana Potions 
        /// this extended-statable <see cref="ZeldaEntity"/> uses.
        /// </summary>
        public float ManaPotionEffectiviness
        {
            get;
            private set;
        }

        #endregion

        #region > Stats <

        #region - Base <

        /// <summary>
        /// Gets the base strength of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseStrength
        {
            get { return baseStrength; }
        }

        /// <summary>
        /// Gets the base dexterity of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseDexterity
        {
            get { return baseDexterity; }
        }

        /// <summary>
        /// Gets the base agility of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseAgility
        {
            get { return baseAgility; }
        }

        /// <summary>
        /// Gets the base vitality of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseVitality
        {
            get { return baseVitality; }
        }

        /// <summary>
        /// Gets the base intelligence of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseIntelligence
        {
            get { return baseIntelligence; }
        }

        /// <summary>
        /// Gets the base luck of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseLuck
        {
            get { return baseLuck; }
        }

        /// <summary>
        /// Gets the total strength of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Strength
        {
            get { return totalStrength; }
        }

        #endregion

        #region - Total -

        /// <summary>
        /// Gets the total dexterity of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Dexterity
        {
            get { return totalDexterity; }
        }

        /// <summary>
        /// Gets the total agility of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Agility
        {
            get { return totalAgility; }
        }

        /// <summary>
        /// Gets the total vitality of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Vitality
        {
            get { return totalVitality; }
        }

        /// <summary>
        /// Gets the total intelligence of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Intelligence
        {
            get { return totalIntelligence; }
        }

        /// <summary>
        /// Gets the total luck of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Luck
        {
            get { return totalLuck; }
        }

        #endregion

        #endregion        

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedStatable"/> class.
        /// </summary>
        public ExtendedStatable()
            : base( new ExtendedDamageDoneContainer(), new ExtendedChanceToContainer(), new ExtendedChanceToBeContainer() )
        {
            this.sharedCooldowns = new SharedCooldownMap( this  );
            this.equipmentStatus = new EquipmentStatus( this );
            this.spellPower = new SpellPowerContainer( this );
            this.equipmentSlotStatModifiers = new EquipmentSlotStatModifierContainer( this );

            this.damageDone = (ExtendedDamageDoneContainer)base.DamageDone;
            this.chanceTo = (ExtendedChanceToContainer)base.ChanceTo;
            this.chanceToBe = (ExtendedChanceToBeContainer)base.ChanceToBe;
        }

        /// <summary>
        /// Setups this Statable ExtendedStatable.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.spellPower.Setup( serviceProvider );
            base.Setup( serviceProvider );
        }

        /// <summary>
        /// Setups the initial status of this <see cref="ExtendedStatable"/>.
        /// </summary>
        internal void SetupInitialStatus()
        {
            this.Level            = 1;
            this.freeStatPoints   = InitialFreeStatPoints;
            this.totalStatPoints  = InitialFreeStatPoints;
            
            this.ResetBaseStats();
        }

        #endregion

        #region [ Methods ]

        #region > Updating <

        /// <summary>
        /// Updates this <see cref="ExtendedStatable"/> component.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            float frameTime = updateContext.FrameTime;

            foreach( Cooldown cooldown in sharedCooldowns.Values )
                cooldown.Update( frameTime );

            this.equipmentStatus.Update();
            base.Update( updateContext );
        }

        #endregion

        #region > Get <
        
        /// <summary>
        /// Calculates the mitigation multiplier a physical attack of this extended-statable
        /// against a target with the given <paramref name="targetArmor"/> and <paramref name="targetLevel"/>.
        /// </summary>
        /// <param name="targetArmor">
        /// The armor of the target of the physical attack.
        /// </param>
        /// <param name="targetLevel">
        /// The level of the target of the physical attack.
        /// </param>
        /// <returns>
        /// The mitigation multiplier that simply gets applied to the damage.
        /// </returns>
        internal override float GetPhysicalMitigationOf( int targetArmor, int targetLevel )
        {
            int  finalArmor  = this.GetModifiedTargetArmor( targetArmor );
            float mitigation = StatusCalc.GetMitigationFromArmor( finalArmor, targetLevel );

            return 1.0f - (mitigation / 100.0f);
        }

        /// <summary>
        /// Gets the final armor value by modifying the given armor value.
        /// </summary>
        /// <param name="armor">
        /// The armor of the target.
        /// </param>
        /// <returns>
        /// The modifier armor value.
        /// </returns>
        /// <seealso cref="ArmorIgnoreEffect"/>
        private int GetModifiedTargetArmor( int armor )
        {
            return (int)((armor - this.armorIgnore) * this.armorIgnoreMultiplier);
        }

        /// <summary>
        /// Receives a value that indicates how much of a given <see cref="Stat"/> 
        /// the extended-statable ZeldaEntity has invested in.
        /// </summary>
        /// <param name="stat">The Stat to receive the value for.</param>
        /// <returns>The number of points the Stat has.</returns>
        public int GetBaseStat( Stat stat )
        {
            switch( stat )
            {
                case Stat.Strength:
                    return baseStrength;

                case Stat.Dexterity:
                    return baseDexterity;

                case Stat.Agility:
                    return baseAgility;

                case Stat.Vitality:
                    return baseVitality;

                case Stat.Intelligence:
                    return baseIntelligence;

                case Stat.Luck:
                    return baseLuck;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Receives a value that indicates how much of a given <see cref="Stat"/> 
        /// the extended-statable ZeldaEntity has in total.
        /// </summary>
        /// <param name="stat">The Stat to receive the value for.</param>
        /// <returns>The total number of points the Stat has.</returns>
        public int GetStat( Stat stat )
        {
            switch( stat )
            {
                case Stat.Strength:
                    return totalStrength;

                case Stat.Dexterity:
                    return totalDexterity;

                case Stat.Agility:
                    return totalAgility;

                case Stat.Vitality:
                    return totalVitality;

                case Stat.Intelligence:
                    return totalIntelligence;

                case Stat.Luck:
                    return totalLuck;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the given percentage of the base mana of the extended-statable ZeldaEntity.
        /// </summary>
        /// <remarks>
        /// Base Mana is the Maia gained just by leveling-up.
        /// </remarks>
        /// <param name="percentage">
        /// The percentage to get; where 1 equals 100%.
        /// </param>
        /// <returns>The requested value.</returns>
        public int GetPercentageOfBaseMana( float percentage )
        {
            return (int)(baseMana * percentage);
        }

        #endregion

        #region > Remove <

        /// <summary>
        /// Removes a percentage of base mana from the extended-statable ZeldaEntity.
        /// </summary>
        /// <param name="manaCost">
        /// The mana to remove in %; where 1 = 100%.
        /// </param>
        internal void RemovePercentageOfBaseMana( float manaCost )
        {
            this.Mana -= this.GetPercentageOfBaseMana( manaCost );
        }

        #endregion

        #region > Leveling <

        /// <summary>
        /// Adds the given amount of experience, modified by the ExperienceGainedModifier,
        /// to the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="amount">
        /// The amount of experience to add to the extended-statable ZeldaEntity.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than 0.
        /// </exception>
        public void AddExperienceModified( long amount )
        {
            this.AddExperience( (long)((amount + this.fixedExperienceGainedModifier) * this.experienceGainedModifier) );
        }

        /// <summary>
        /// Adds the given amount of experience to the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="amount">
        /// The amount of experience to add to the extended-statable ZeldaEntity.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="amount"/> is less than 0.
        /// </exception>
        public void AddExperience( long amount )
        {
            if( amount == 0 )
                return;
            if( amount < 0 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "amount" );

            this.experience += amount;

            if( this.ExperienceGained != null )
                this.ExperienceGained( this, amount );

            if( this.experience >= this.experienceNeeded )
            {
                // Carry the over flow experience to the next level.
                long overflowExp = this.experience - this.experienceNeeded;

                if( overflowExp >= 0 )
                    this.experience = overflowExp;
                else
                    this.experience = 0;

                // The following line also sets how much exp is needed for the next level!
                this.Level += 1;

                // Add more stat points :)
                int gainedStatPoints = StatusCalc.GetStatPointGainedForLevelUp( this.Level );
                this.AddStatPoints( gainedStatPoints );

                // Heal-Up
                this.RestoreFully();
                this.LevelUped.Raise( this );
            }
        }

        #endregion

        #region > Stats <

        /// <summary>
        /// Adds the given number of stat points to the extended-statable ZeldaEntity.
        /// </summary>
        /// <remarks>
        /// Those points can be used to invest into one of the main <see cref="Stat"/>s.
        /// </remarks>
        /// <param name="count">
        /// The number of points to add.
        /// </param>
        public void AddStatPoints( int count )
        {
            this.freeStatPoints += count;
            this.totalStatPoints += count;
        }

        /// <summary>
        /// Receives a value that indicates whether the extented-statable ZeldaEntity
        /// has enough free Stat Points to invest in the given <see cref="Stat"/>.
        /// </summary>
        /// <param name="stat">
        /// The stat the user wants to invest in.
        /// </param>
        /// <returns>
        /// true if there are enough points left to invest atleast one time;
        /// otherwise false.
        /// </returns>
        public bool CanInvestInStat( Stat stat )
        {
            return StatusCalc.GetStatPointsNeeded( GetBaseStat( stat ) ) <= this.freeStatPoints;
        }

        /// <summary>
        /// Tries to invest FreeStatPoints to increase the given <see cref="Stat"/> by one.
        /// </summary>
        /// <param name="stat">
        /// The Stat to invest in.
        /// </param>
        /// <returns>
        /// True if it was possible to invest into the Stat; 
        /// False if it wasn't possible to invest - aka. there were not enough free stat points available.
        /// </returns>
        public bool InvestInStat( Stat stat )
        {
            int statPointsNeeded = this.GetPointsRequiredForStat( stat );

            if( statPointsNeeded <= this.freeStatPoints )
            {
                this.IncreaseStat( stat );
                this.freeStatPoints -= statPointsNeeded;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to undo the last investment into the given <see cref="Stat"/>.
        /// </summary>
        /// <param name="stat">
        /// The Stat to undo the invest for.
        /// </param>
        /// <returns>
        /// True if it was possible to undo the investment into the Stat; otherwise false.
        /// </returns>
        public bool UndoInvestmentInStat( Stat stat )
        {
            if( GetStat(stat) > 1 )
            {
                this.freeStatPoints += this.GetPointsGainedByDecreasingStat( stat );
                this.DecreaseStat( stat );
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the number of status points that are required to invest one point
        /// into the specified Stat.
        /// </summary>
        /// <param name="stat">
        /// The Stat to invest in.
        /// </param>
        /// <returns>
        /// The number of status points that are required.
        /// </returns>
        public int GetPointsRequiredForStat( Stat stat )
        {
            return StatusCalc.GetStatPointsNeeded( this.GetBaseStat( stat ) );
        }

        /// <summary>
        /// Gets the number of status points that would be gained by decreasing the given stat point by one.
        /// </summary>
        /// <param name="stat">
        /// The Stat to derease.
        /// </param>
        /// <returns>
        /// The number of status points that would be gained.
        /// </returns>
        public int GetPointsGainedByDecreasingStat( Stat stat )
        {
            return StatusCalc.GetStatPointsNeeded( this.GetBaseStat( stat ) - 1 );
        }

        /// <summary>
        /// Increases the given <see cref="Stat"/> by one.
        /// </summary>
        /// <param name="stat">The Stat to increase by one.</param>
        public void IncreaseStat( Stat stat )
        {
            ChangeStatBy( stat, 1 );
        }

        /// <summary>
        /// Decreases the given <see cref="Stat"/> by one.
        /// </summary>
        /// <param name="stat">The Stat to decrase by one.</param>
        public void DecreaseStat( Stat stat )
        {
            ChangeStatBy( stat, -1 );
        }

        private void ChangeStatBy( Stat stat, int amount )
        {
            switch( stat )
            {
                case Stat.Strength:
                    this.baseStrength += amount;
                    this.Refresh_Strength();
                    break;

                case Stat.Dexterity:
                    this.baseDexterity += amount;
                    this.Refresh_Dexterity();
                    break;

                case Stat.Vitality:
                    this.baseVitality += amount;
                    this.Refresh_Vitality();
                    break;

                case Stat.Agility:
                    this.baseAgility += amount;
                    this.Refresh_Agility();
                    break;

                case Stat.Intelligence:
                    this.baseIntelligence += amount;
                    this.Refresh_Intelligence();
                    break;

                case Stat.Luck:
                    this.baseLuck += amount;
                    this.Refresh_Luck();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        
        #region > Refresh <

        /// <summary>
        /// Refreshes all status data of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void RefreshStatus()
        {
            this.Refresh_Strength();
            this.Refresh_Dexterity();
            this.Refresh_Agility();
            this.Refresh_Vitality();
            this.Refresh_Intelligence();
            this.Refresh_Luck();
        }

        /// <summary>
        /// Refreshes all level-based statuses of the extended-statable <see cref="ZeldaEntity"/>..
        /// </summary>
        private void RefreshStatus_LevelBased()
        {
            this.Refresh_TotalLife();
            this.Refresh_TotalMana();
            this.Refresh_RatingDependend();
        }

        /// <summary>
        /// Refreshes all status values that might scale with a rating.
        /// </summary>
        private void Refresh_RatingDependend()
        {
            this.Refresh_AttackSpeedMelee();
            this.Refresh_AttackSpeedRanged();
            this.Refresh_CastTimeModifier();

            this.Refresh_MovementSpeed();
            this.Refresh_ExperienceGained();

            this.chanceTo.Refresh();
            this.chanceToBe.Refresh();

            this.damageDone.WithCritical.Refresh( DamageSource.All );
            this.Resistances.Refresh( ElementalSchool.All );
        }

        /// <summary>
        /// Refreshes the total <see cref="Statable.Armor"/> of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal override void Refresh_TotalArmor()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( ArmorEffect.IdentifierString, out addValue, out mulValue );

            this.BaseArmor = equipmentStatus.Armor + StatusCalc.GetAdditionalArmor( this.Agility );
            this.Armor     = (int)((BaseArmor + addValue) * mulValue);
        }

        /// <summary>
        /// Refreshes the ArmorIgnore value of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <seealso cref="ArmorIgnoreEffect"/>
        internal void Refresh_ArmorIgnore()
        {
            float fixedValue, percentalValue, ratingValue;
            this.AuraList.GetEffectValues( ArmorIgnoreEffect.IdentifierString, out fixedValue, out percentalValue, out ratingValue );

            this.armorIgnore = (int)fixedValue;
            this.armorIgnoreMultiplier = StatusCalc.GetArmorIgnoreMultiplier( percentalValue, ratingValue, this.Level );
        }

        /// <summary>
        /// Refreshes the Magic Find value of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_MagicFind()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( MagicFindEffect.IdentifierString, out addValue, out mulValue );

            float additiveFactor = (addValue + StatusCalc.GetMagicFindExtra( this.Luck )) / 100.0f;

            const float BaseFactor = 1.0f;
            this.magicFind = BaseFactor + (additiveFactor * mulValue);
        }

        /// <summary>
        /// Refreshes all status values that might change when changing the currently
        /// equiped melee weapon.
        /// </summary>
        internal void Refresh_MeleeWeaponRelated()
        {
            this.Refresh_AttackSpeedMelee();
            this.Refresh_Pushing();
        }

        /// <summary>
        /// Refreshes the current PushingExtraForce value.
        /// </summary>
        internal void Refresh_Pushing()
        {
            this.AuraList.GetEffectValues( 
                PushingForceEffect.IdentifierString,
                out this.pushingForceAdditive,
                out this.pushingForceMultiplicative
            );

            if( this.Equipment.IsWearingDagger )
            {
                this.pushingForceMultiplicative *= 0.85f;
            }
        }

        /// <summary>
        /// Refreshes the block value that indicates how much physical damage the
        /// extended-statable ZeldaEntity may block.
        /// </summary>
        internal void Refresh_BlockValue()
        {
            float fixedValue, percentalValue;
            this.AuraList.GetEffectValues( BlockValueEffect.IdentifierString, out fixedValue, out percentalValue );

            this.BlockValue = (int)((fixedValue + StatusCalc.GetAdditionalBlockValue( this.Strength )) * percentalValue);
        }

        /// <summary>
        /// Refreshes the Movement Speed of the statable <see cref="ZeldaEntity"/>
        /// by taking into account the <see cref="MovementSpeedEffect"/>s currently active
        /// on the ZeldaEntity.
        /// </summary>
        /// <seealso cref="MovementSpeedEffect"/>
        internal override void Refresh_MovementSpeed()
        {
            var moveable = this.Owner.Components.Get<Zelda.Entities.Components.Moveable>();
            if( moveable == null )
                return;

            float addValue, mulValue, ratingValue;
            this.AuraList.GetEffectValues( MovementSpeedEffect.IdentifierString, out addValue, out mulValue, out ratingValue );

            moveable.Speed = StatusCalc.GetMovementSpeed( moveable.BaseSpeed, this.Agility, addValue, mulValue, ratingValue, this.Level );
        }

        /// <summary>
        /// Refreshes the Experience Gained Modifier value of the statable <see cref="ZeldaEntity"/>
        /// by taking into account the <see cref="ExperienceGainedEffect"/>s currently active
        /// on the ZeldaEntity.
        /// </summary>
        /// <seealso cref="ExperienceGainedEffect"/>
        internal void Refresh_ExperienceGained()
        {
            float fixedValue, percentValue, ratingValue;
            this.AuraList.GetEffectValues(
                 ExperienceGainedEffect.IdentifierString,
                 out fixedValue,
                 out percentValue,
                 out ratingValue
            );

            this.fixedExperienceGainedModifier = (int)fixedValue;
            this.experienceGainedModifier = percentValue + StatusCalc.ConvertRating( ratingValue, this.Level ) / 100.0f;
        }

        #region - Stats -

        /// <summary>
        /// Refreshes the total strength and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Strength()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierStrength, out addValue, out mulValue );

            totalStrength = (int)((baseStrength + equipmentStatus.Strength + addValue) * mulValue);

            this.chanceTo.RefreshParry();
            this.Refresh_DamageMelee();
            this.Refresh_BlockValue();
        }

        /// <summary>
        /// Refreshes the total dexterity and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Dexterity()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierDexterity, out addValue, out mulValue );

            totalDexterity = (int)((baseDexterity + equipmentStatus.Dexterity + addValue) * mulValue);

            this.chanceTo.RefreshMiss();
            this.Refresh_DamageMelee();
            this.Refresh_DamageRanged();
            this.Refresh_CastTimeModifier();
        }

        /// <summary>
        /// Refreshes the total vitality and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Vitality()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierVitality, out addValue, out mulValue );

            totalVitality = (int)((baseVitality + equipmentStatus.Vitality + addValue) * mulValue);

            this.Refresh_TotalLife();
            this.Refresh_LifePotionEffectiveness();
            this.chanceToBe.RefreshCrit();
        }

        /// <summary>
        /// Refreshes the total agility and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Agility()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierAgility, out addValue, out mulValue );
            totalAgility = (int)((baseAgility + equipmentStatus.Agility + addValue) * mulValue);

            this.chanceTo.RefreshDodge();
            this.Refresh_TotalArmor();
            this.Refresh_AttackSpeedMelee();
            this.Refresh_AttackSpeedRanged();
            this.Refresh_MovementSpeed();
            
            this.AgilityUpdated.Raise( this );
        }

        /// <summary>
        /// Refreshes the total intelligence and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Intelligence()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierIntelligence, out addValue, out mulValue );

            totalIntelligence = (int)((baseIntelligence + equipmentStatus.Intelligence + addValue) * mulValue);

            this.Refresh_TotalMana();
            this.Refresh_ManaPotionEffectiveness();
            this.Refresh_DamageMagic();
        }

        /// <summary>
        /// Refreshes the total luck and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_Luck()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( StatEffect.IdentifierLuck, out addValue, out mulValue );

            totalLuck = (int)((baseLuck + equipmentStatus.Luck + addValue) * mulValue);

            this.chanceTo.RefreshCrit();
            this.Refresh_MagicFind();
        }

        #endregion

        #region - Life / Mana -

        /// <summary>
        /// Refreshes the total life and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal override void Refresh_TotalLife()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( "Life", out addValue, out mulValue );

            this.MaximumLife = StatusCalc.GetTotalLife( Level, Vitality, addValue, mulValue );
            this.Refresh_LifeRegen();
        }

        /// <summary>
        /// Refreshes the total mana and all related other stats of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_TotalMana()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( "Mana", out addValue, out mulValue );

            this.baseMana    = StatusCalc.GetBaseMana( this.Level );
            this.MaximumMana = StatusCalc.GetTotalMana( baseMana, this.Intelligence, addValue, mulValue );

            Refresh_ManaRegen();
        }

        /// <summary>
        /// Refreshes the life regeneration value the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_LifeRegen()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( 
                LifeManaRegenEffect.IdentifierLife,
                LifeManaRegenEffect.IdentifierBoth,
                out addValue, 
                out mulValue
            );

            this.LifeRegeneration = StatusCalc.GetLifeRegen( this.Vitality, this.MaximumLife, addValue, mulValue );
        }

        /// <summary>
        /// Refreshes the mana regeneration value the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_ManaRegen()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues(
                LifeManaRegenEffect.IdentifierMana,
                LifeManaRegenEffect.IdentifierBoth,
                out addValue, 
                out mulValue
            );

            this.ManaRegeneration = StatusCalc.GetManaRegen( this.Intelligence, this.MaximumMana, addValue, mulValue );
        }

        #endregion

        #region - Potion Effectiveness -

        /// <summary>
        /// Refreshes the life potion effectiviness multiplciator value
        /// of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_LifePotionEffectiveness()
        {
            float multiplier = this.AuraList.GetPercentalEffectValue( LifeManaPotionEffectivenessEffect.IdentifierLife );
            this.LifePotionEffectiviness = StatusCalc.GetLifePotionEffectiviness( this.Vitality, multiplier );
        }

        /// <summary>
        /// Refreshes the mana potion effectiviness multiplciator value
        /// of the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        internal void Refresh_ManaPotionEffectiveness()
        {
            float multiplier = this.AuraList.GetPercentalEffectValue( LifeManaPotionEffectivenessEffect.IdentifierLife );
            this.ManaPotionEffectiviness = StatusCalc.GetManaPotionEffectiviness( this.Intelligence, multiplier );
        }

        #endregion

        #region - Damage -

        #region | Melee |

        /// <summary>
        /// Refreshes the damage the extended-statable ZeldaEntity does in Melee Combat.
        /// </summary>
        internal void Refresh_DamageMelee()
        {
            int min = 0, max = 0, fixedIncrease = 0;
            float weaponSpeed = 1.0f, normalizedWeaponSpeed = 1.0f, damageMultiplier = 1.0f;

            var weaponHand = equipmentStatus.WeaponHand;
            
            if( weaponHand != null )
            {
                var weapon = weaponHand.Weapon;

                min = weapon.DamageMinimum;
                max = weapon.DamageMaximum;

                weaponSpeed           = weapon.AttackSpeed;
                normalizedWeaponSpeed = StatusCalc.GetNormalizedSpeed( weapon.WeaponType );

                float fixedValue;
                this.AuraList.GetEffectValues(
                    WeaponDamageTypeBasedEffect.GetIdentifier( weapon.WeaponType ),
                    out fixedValue,
                    out damageMultiplier
                );

                fixedIncrease += (int)fixedValue;
            }

            this.CalculateMeleeDamage( min, max, weaponSpeed, fixedIncrease, damageMultiplier );
            this.CalculateNormalizedMeleeDamage( min, max, normalizedWeaponSpeed, fixedIncrease, damageMultiplier );
        }

        /// <summary>
        /// Helpers method that calculates the melee damage done by the player.
        /// </summary>
        /// <param name="min">
        /// The minimum damage; includes weapon damage.
        /// </param>
        /// <param name="max">
        /// The maximum damage; includes weapon damage.
        /// </param>
        /// <param name="weaponSpeed">
        /// The speed of the weapon.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed damage increase.
        /// </param>
        /// <param name="damageMultiplier">
        /// The multiplicative damage modifier.
        /// </param>
        private void CalculateMeleeDamage( int min, int max, float weaponSpeed, int fixedIncrease, float damageMultiplier )
        {
            min += StatusCalc.GetMinimumMeleeDamageIncrease( this.Strength, this.Dexterity, weaponSpeed );
            min += fixedIncrease;
            min = (int)(min * damageMultiplier);

            max += StatusCalc.GetMaximumMeleeDamageIncrease( this.Strength, this.Dexterity, weaponSpeed );
            max += fixedIncrease;
            max = (int)(max * damageMultiplier);

            if( min > max )
                min = max;

            this.DamageMeleeMin = min;
            this.DamageMeleeMax = max;
        }

        /// <summary>
        /// Helpers method that calculates the normalized melee damage done by the player.
        /// </summary>
        /// <param name="min">
        /// The minimum damage; includes weapon damage.
        /// </param>
        /// <param name="max">
        /// The maximum damage; includes weapon damage.
        /// </param>
        /// <param name="normWeaponSpeed">
        /// The normalized speed of the weapon.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed damage increase.
        /// </param>
        /// <param name="damageMultiplier">
        /// The multiplicative damage modifier.
        /// </param>
        private void CalculateNormalizedMeleeDamage( int min, int max, float normWeaponSpeed, int fixedIncrease, float damageMultiplier )
        {
            min += StatusCalc.GetMinimumMeleeDamageIncrease( this.Strength, this.Dexterity, normWeaponSpeed );
            min += fixedIncrease;
            min = (int)(min * damageMultiplier);

            max += StatusCalc.GetMaximumMeleeDamageIncrease( this.Strength, this.Dexterity, normWeaponSpeed );
            max += fixedIncrease;
            max = (int)(max * damageMultiplier);

            if( min > max )
                min = max;

            this.DamageMeleeNormalizedMin = min;
            this.DamageMeleeNormalizedMax = max;
        }

        #endregion

        #region | Ranged |

        /// <summary>
        /// Refreshes the damage the extended-statable ZeldaEntity does in Ranged Combat.
        /// </summary>
        internal void Refresh_DamageRanged()
        {
            int min = 0, max = 0, fixedIncrease = 0;
            float weaponSpeed = 1.0f, normalizedWeaponSpeed = 1.0f, damageMultiplier = 1.0f;

            var rangedSlot = equipmentStatus.Ranged;

            if( rangedSlot != null )
            {
                var ranged = rangedSlot.Weapon;

                min = ranged.DamageMinimum;
                max = ranged.DamageMaximum;

                weaponSpeed           = ranged.AttackSpeed;
                normalizedWeaponSpeed = StatusCalc.GetNormalizedSpeed( ranged.WeaponType );

                float fixedValue;
                this.AuraList.GetEffectValues(
                    WeaponDamageTypeBasedEffect.GetIdentifier( ranged.WeaponType ),
                    out fixedValue,
                    out damageMultiplier
                );

                fixedIncrease += (int)fixedValue;
            }

            this.CalculateRangedDamage( min, max, weaponSpeed, fixedIncrease, damageMultiplier );
            this.CalculateNormaliizedRangedDamage( min, max, normalizedWeaponSpeed, fixedIncrease, damageMultiplier );
        }

        /// <summary>
        /// Helpers method that calculates the ranged damage done by the player.
        /// </summary>
        /// <param name="min">
        /// The minimum damage; includes weapon damage.
        /// </param>
        /// <param name="max">
        /// The maximum damage; includes weapon damage.
        /// </param>
        /// <param name="weaponSpeed">
        /// The speed of the weapon.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed damage increase.
        /// </param>
        /// <param name="damageMultiplier">
        /// The multiplicative damage modifier.
        /// </param>
        private void CalculateRangedDamage( int min, int max, float weaponSpeed, int fixedIncrease, float damageMultiplier )
        {
            min += StatusCalc.GetMinimumRangedDamageIncrease( this.Dexterity, weaponSpeed );
            min += fixedIncrease;
            min = (int)(min * damageMultiplier);

            max += StatusCalc.GetMaximumRangedDamageIncrease( this.Dexterity, this.Agility, weaponSpeed );
            max += fixedIncrease;
            max = (int)(max * damageMultiplier);

            if( min > max )
                min = max;

            this.DamageRangedMin = min;
            this.DamageRangedMax = max;
        }

        /// <summary>
        /// Helpers method that calculates the normalized ranged damage done by the player.
        /// </summary>
        /// <param name="min">
        /// The minimum damage; includes weapon damage.
        /// </param>
        /// <param name="max">
        /// The maximum damage; includes weapon damage.
        /// </param>
        /// <param name="normWeaponSpeed">
        /// The normalized speed of the weapon.
        /// </param>
        /// <param name="fixedIncrease">
        /// The fixed damage increase.
        /// </param>
        /// <param name="damageMultiplier">
        /// The multiplicative damage modifier.
        /// </param>
        private void CalculateNormaliizedRangedDamage( int min, int max, float normWeaponSpeed, int fixedIncrease, float damageMultiplier )
        {
            min += StatusCalc.GetMinimumRangedDamageIncrease( this.Dexterity, normWeaponSpeed );
            min += fixedIncrease;
            min = (int)(min * damageMultiplier);

            max += StatusCalc.GetMaximumRangedDamageIncrease( this.Dexterity, this.Agility, normWeaponSpeed );
            max += fixedIncrease;
            max = (int)(max * damageMultiplier);

            if( min > max )
                min = max;

            this.DamageRangedNormalizedMin = min;
            this.DamageRangedNormalizedMax = max;
        }

        #endregion

        /// <summary>
        /// Refreshes the damage the extended-statable ZeldaEntity does in Magic Combat.
        /// </summary>
        internal void Refresh_DamageMagic()
        {
            int min = StatusCalc.GetMinimumMagicDamageIncrease( this.Intelligence );
            int max = StatusCalc.GetMaximumMagicDamageIncrease( this.Intelligence );

            if( min > max )
                min = max;

            this.DamageMagic = new Atom.Math.IntegerRange( min, max );
            this.spellPower.RefreshTotal();
        }

        #endregion

        #region - Attack Speed -

        /// <summary>
        /// Refreshes the attack speed/delay for the default melee attack.
        /// </summary>
        internal void Refresh_AttackSpeedMelee()
        {
            float weaponSpeed = 1.0f;
            var weapon = this.equipmentStatus.WeaponHand;
            if( weapon != null )
                weaponSpeed = weapon.Weapon.AttackSpeed;

            float percentualValue, ratingValue;
            this.AuraList.GetPercentalAndRatingEffectValues( 
                AttackSpeedEffect.IdentifierMelee,
                AttackSpeedEffect.IdentifierAll,
                out percentualValue,
                out ratingValue
            );
            
            this.AttackSpeedMelee = StatusCalc.GetAttackSpeed( 
                weaponSpeed, 
                this.Agility,
                percentualValue, 
                ratingValue,
                this.Level
            );

            this.AttackSpeedMeleeNormalized = (this.AttackSpeedMelee + weaponSpeed) / 2.0f;          
            this.Refresh_DamageMelee();
        }

        /// <summary>
        /// Refreshes the attack speed/delay for the default ranged attack.
        /// </summary>
        internal void Refresh_AttackSpeedRanged()
        {
            float weaponSpeed = 1.0f;
            var weapon = this.equipmentStatus.Ranged;
            if( weapon != null )
                weaponSpeed = weapon.Weapon.AttackSpeed;

            float percentualValue, ratingValue;
            this.AuraList.GetPercentalAndRatingEffectValues(
                AttackSpeedEffect.IdentifierRanged,
                AttackSpeedEffect.IdentifierAll,
                out percentualValue,
                out ratingValue
            );

            this.AttackSpeedRanged = StatusCalc.GetAttackSpeed(
                weaponSpeed,
                this.Agility,
                percentualValue,
                ratingValue,
                this.Level
            );
            
            this.AttackSpeedRangedNormalized = (this.AttackSpeedRanged + weaponSpeed) / 2.0f; 
            this.Refresh_DamageRanged();
        }

        /// <summary>
        /// Refreshes the cast time modifier value.
        /// </summary>
        internal void Refresh_CastTimeModifier()
        {
            float percentualValue, ratingValue;
            this.AuraList.GetPercentalAndRatingEffectValues(
                SpellHasteEffect.IdentifierString,
                out percentualValue,
                out ratingValue
            );

            this.CastTimeModifier = StatusCalc.GetCastSpeedModifier( this.Dexterity, ratingValue, percentualValue, this.Level );
        }

        #endregion

        #endregion

        #region > Storage <

        /// <summary>
        /// Writes the current status state of this ExtendedStatable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void SerializeExtended( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 3;
            context.Write( CurrentVersion );

            context.Write( this.Level );
            context.Write( this.Experience );
            
            context.Write( this.rubies );
            context.Write( this.freeStatPoints );
            context.Write( this.totalStatPoints ); // new in V2.

            context.Write( this.BaseStrength );
            context.Write( this.BaseDexterity );
            context.Write( this.BaseAgility );
            context.Write( this.BaseVitality );
            context.Write( this.BaseIntelligence );
            context.Write( this.BaseLuck );

            this.WriteCooldowns( context );
        }

        /// <summary>
        /// Saves the currently active shared cooldowns of the ExtendedStatable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void WriteCooldowns( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Filter out the relevant cooldowns.
            var cooldowns         = this.SharedCooldowns;
            var relevantCooldowns = new System.Collections.Generic.List<Cooldown>( cooldowns.Count );

            foreach( var cooldown in cooldowns.Values )
            {
                Debug.Assert( cooldown.IsShared );

                if( cooldown.TimeLeft > 0.0f )
                    relevantCooldowns.Add( cooldown );
            }

            // Write
            context.Write( relevantCooldowns.Count );

            foreach( var cooldown in relevantCooldowns )
            {
                context.Write( cooldown.Id );
                context.Write( cooldown.TotalTime );
                context.Write( cooldown.TimeLeft );
            }
        }

        /// <summary>
        /// Reads the current status state of this ExtendedStatable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void DeserializeExtended( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 3;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            this.Level      = context.ReadInt32();
            this.experience = context.ReadInt64();

            if( version <= 2 )
            {
                this.Life = context.ReadInt32();
                this.Mana = context.ReadInt32();
            }

            this.Rubies         = context.ReadInt64();
            this.freeStatPoints = context.ReadInt32();
            
            if( version >= 2 )
            {
                this.totalStatPoints = context.ReadInt32();
            }
            else
            {
                this.totalStatPoints = StatusCalc.EstimateStatPointsGained( this.Level );
            }

            this.baseStrength     = context.ReadInt32();
            this.baseDexterity    = context.ReadInt32();
            this.baseAgility      = context.ReadInt32();
            this.baseVitality     = context.ReadInt32();
            this.baseIntelligence = context.ReadInt32();
            this.baseLuck         = context.ReadInt32();

            // Allow loading of Items for this ExtendedStatable.
            var itemManager = context.ServiceProvider.ItemManager;
            itemManager.Statable = this;

            this.ReadCooldowns( context );
        }

        /// <summary>
        /// Serializes the current life and mana status of the extended statable entity.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// </param>
        internal void SerializePowerStatus( Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.Life );
            context.Write( this.Mana );
        }

        /// <summary>
        /// Deserializes the current life and mana status of the extended statable entity.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// </param>
        internal void DeserializePowerStatus( Saving.IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( "ExtendedStatable.PowerStatus" );

            this.Life = context.ReadInt32();
            this.Mana = context.ReadInt32();
        }

        /// <summary>
        /// Reads the currently active shared cooldowns of the player.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadCooldowns( Zelda.Saving.IZeldaDeserializationContext context )
        {
            var cooldowns     = this.SharedCooldowns;
            int cooldownCount = context.ReadInt32();

            cooldowns.Clear();

            for( int i = 0; i < cooldownCount; ++i )
            {
                int id          = context.ReadInt32();
                float totalTime = context.ReadSingle();
                float timeLeft  = context.ReadSingle();

                cooldowns.Add( id, new Cooldown( id, totalTime, true ) { TimeLeft = timeLeft } );
            }
        }

        #endregion
                
        /// <summary>
        /// Resets the stats of the extended-statable entity
        /// has aquired so far.
        /// </summary>
        public void ResetStats()
        {
            // Reset and refresh stats:
            this.ResetBaseStats();
            this.freeStatPoints = this.totalStatPoints;
            this.RefreshStatus();

            this.Equipment.NotifyRequirementsRecheckNeeded();
        }

        /// <summary>
        /// Sets all of the base stats to 1.
        /// </summary>
        private void ResetBaseStats()
        {
            this.baseStrength     = 1;
            this.baseDexterity    = 1;
            this.baseAgility      = 1;
            this.baseVitality     = 1;
            this.baseIntelligence = 1;
            this.baseLuck         = 1;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the shared <see cref="Cooldown"/>s corresponding to the
        /// extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly SharedCooldownMap sharedCooldowns;

        /// <summary>
        /// Stores what equipment the extended-statable <see cref="ZeldaEntity"/>
        /// has currently equiped.
        /// </summary>
        private readonly EquipmentStatus equipmentStatus;

        /// <summary> 
        /// The stat value which is increased by the status points.
        /// </summary>
        private int baseStrength, baseDexterity, baseAgility, baseVitality, baseIntelligence, baseLuck;

        /// <summary> 
        /// The stat value which is created by 
        /// adding together the base value, the value from items and additional StatEffect values.
        /// This value is then multiplicated by multiplicative StatEffects.
        /// </summary>
        private int totalStrength, totalDexterity, totalAgility, totalVitality, totalIntelligence, totalLuck;

        /// <summary>
        /// The base mana of the extended-statable ZeldaEntity. This value is often used
        /// to get the mana costs of a Skill.
        /// </summary>
        private int baseMana;

        /// <summary>
        /// Stores the value of the <see cref="Rubies"/> property.
        /// </summary>
        private long rubies;

        /// <summary>
        /// Stores how much experience the extended-statable ZeldaEntity has.
        /// </summary>
        private long experience;

        /// <summary>
        /// Stores how much experience the extended-statable ZeldaEntity needs for the next level-up.
        /// </summary>
        private long experienceNeeded;

        /// <summary>
        /// The fixed additional experience that is added to the experience the extended-statable ZeldaEntity gains.
        /// </summary>
        private int fixedExperienceGainedModifier;

        /// <summary>
        /// The modifer applied to experience the extended-statable ZeldaEntity gains.
        /// </summary>
        /// <remarks>
        /// This won't modify experience gained by Quests.
        /// </remarks>
        private float experienceGainedModifier = 1.0f;

        /// <summary>
        /// The number of stat points that haven't been invested by the extended-statable <see cref="ZeldaEntity"/>.
        /// </summary>
        private int freeStatPoints;

        /// <summary>
        /// The total number of stat points this extended-statable ZeldaEntity has gained so far.
        /// </summary>
        private int totalStatPoints;
        
        /// <summary>
        /// The armor this extended-statable ZeldaEntity ignores.
        /// </summary>
        private int armorIgnore = 0;

        /// <summary>
        /// The multiplier applied to the total armor of an enemy.
        /// </summary>
        private float armorIgnoreMultiplier = 1.0f;

        /// <summary>
        /// The MF value is a multiplicand that is integrated into the
        /// Drop Chance of Rare Items. A higher MF value increases the chance
        /// to find those Rare Items.
        /// </summary>
        private float magicFind = 1.0f;

        /// <summary>
        /// An extra value added to the final pushing force of an attack.
        /// </summary>
        private float pushingForceAdditive, pushingForceMultiplicative = 1.0f;

        /// <summary>
        /// Encapsulates the various chance-to properties of this ExtendedStatable.
        /// </summary>
        private readonly ExtendedChanceToContainer chanceTo;

        /// <summary>
        /// Encapsulates the various chance-to-be properties of this ExtendedStatable.
        /// </summary>
        private readonly ExtendedChanceToBeContainer chanceToBe;

        /// <summary>
        /// Stores and manages the Spell Power related values of this ExtendedStatable.
        /// </summary>
        private readonly SpellPowerContainer spellPower;

        /// <summary>
        /// Encapsulates the damage done modifiers of this ExtendedStatable.
        /// </summary>
        private readonly ExtendedDamageDoneContainer damageDone;

        /// <summary>
        /// Encapuslates the stat modifiers that are applied to the stats
        /// given from items with a specific <see cref="EquipmentSlot"/>.
        /// </summary>
        private readonly EquipmentSlotStatModifierContainer equipmentSlotStatModifiers;

        #endregion 
    }
}