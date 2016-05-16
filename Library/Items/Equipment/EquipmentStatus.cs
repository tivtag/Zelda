// <copyright file="EquipmentStatus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.EquipmentStatus class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom;
    using Zelda.Status;

    /// <summary>
    /// Stores the <see cref="Equipment"/> that an extended-statable ZeldaEntity
    /// is currenly wearing.
    /// This class can't be inherited.
    /// </summary>
    /// <seealso cref="EquipmentStatusSlot"/>
    public sealed class EquipmentStatus
    {
        #region [ Events ]

        /// <summary>
        /// Called when the currently equipped <see cref="Weapon"/> in the <see cref="WeaponHand"/> has changed;
        /// or if the requirement state of the currently equipped Weapon has changed.
        /// </summary>
        public event EventHandler WeaponHandChanged;

        /// <summary>
        /// Called when the state of a slot has changed.
        /// </summary>
        public event RelaxedEventHandler<EquipmentStatusSlot> SlotChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the total armor the equiped items provide together.
        /// </summary>
        public int Armor
        {
            get { return this.armor; }
        }

        /// <summary>
        /// Gets the total strength the equiped items provide together.
        /// </summary>
        public int Strength
        {
            get { return this.strength; }
        }

        /// <summary>
        /// Gets the total dexterity the equiped items provide together.
        /// </summary>
        public int Dexterity
        {
            get { return this.dexterity; }
        }

        /// <summary>
        /// Gets the total agility the equiped items provide together.
        /// </summary>
        public int Agility
        {
            get { return this.agility; }
        }

        /// <summary>
        /// Gets the total vitality the equiped items provide together.
        /// </summary>
        public int Vitality
        {
            get { return this.vitality; }
        }

        /// <summary>
        /// Gets the total intelligence the equiped items provide together.
        /// </summary>
        public int Intelligence
        {
            get { return this.intelligence; }
        }

        /// <summary>
        /// Gets the total luck the equiped items provide together.
        /// </summary>
        public int Luck
        {
            get { return this.luck; }
        }

        /// <summary>
        /// Gets the item in the melee-hand slot, if any.
        /// </summary>
        public WeaponInstance WeaponHand
        {
            get { return this.weaponHand; }
        }

        /// <summary>
        /// Gets the item in the shield-hand slot, if any.
        /// </summary>
        public EquipmentInstance ShieldHand
        {
            get { return this.shieldHand; }
        }

        /// <summary>
        /// Gets the item in the ranged-weapon slot, if any.
        /// </summary>
        public WeaponInstance Ranged
        {
            get { return this.ranged; }
        }

        /// <summary>
        /// Gets the item in the staff slot, if any.
        /// </summary>
        public EquipmentInstance Staff
        {
            get { return this.staff; }
        }

        /// <summary>
        /// Gets a value indicating whether the extended-statable ZeldaEntity
        /// has a ranged weapon equipped and fulfills the required requirements.
        /// </summary>
        public bool CanUseRanged
        {
            get
            {
                return this.ranged != null && this.reqsFulfilledRanged;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the extended-statable ZeldaEntity
        /// has a staff equipped and fulfills the required requirements.
        /// </summary>
        public bool CanCast
        {
            get
            {
                return this.staff != null && this.reqsFulfilledStaff;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the extended-statable ZeldaEntity is
        /// currently wearing a Dagger in his WeaponHand.
        /// </summary>
        public bool IsWearingDagger
        {
            get
            {
                return this.weaponHand != null &&
                       this.reqsFulfilledWeaponHand &&
                       this.weaponHand.Weapon.WeaponType == WeaponType.Dagger;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any item slot has currently
        /// an disabled item.
        /// </summary>
        public bool AnySlotUnfulfilled
        {
            get
            {
                for( int i = 0; i < (int)EquipmentStatusSlot._EnumCount; ++i )
                {
                    var slot = (EquipmentStatusSlot)i;

                    if( !IsRequirementFulfilled( slot ) && HasItemInSlot( slot ) )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStatus"/> class.
        /// </summary>
        /// <param name="statable">
        /// The <see cref="ExtendedStatable"/> component of the ZeldaEntity whos 
        /// equipment status the new <see cref="EquipmentStatus"/> represents.
        /// </param>
        internal EquipmentStatus( ExtendedStatable statable )
        {
            System.Diagnostics.Debug.Assert( statable != null );
            this.statable = statable;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this EquipmentStatus.
        /// </summary>
        public void Update()
        {
            if( this.requirementsRecheckIsNeeded )
            {
                this.CheckItemRequirements();
            }
        }

        #region Enquip

        /// <summary>
        /// Tries to enquip the specified <see cref="Equipment"/> in the specified <see cref="EquipmentStatusSlot"/>.
        /// </summary>
        /// <param name="equipment">
        /// The equipment to enquip. Can be null.
        /// </param>
        /// <param name="slot">
        /// The slot to enquip the item in.
        /// </param>
        /// <param name="oldExchangedEquipment">
        /// Will contain the old item in the slot; if any.
        /// </param>
        /// <returns> true if it could be enquiped, otherwise false. </returns>
        public bool Equip( EquipmentInstance equipment, EquipmentStatusSlot slot, out EquipmentInstance oldExchangedEquipment )
        {
            // First handle the special case
            // of the player sending in no equipment.
            if( equipment == null )
            {
                this.SetNull( slot, out oldExchangedEquipment );
                if( oldExchangedEquipment != null )
                    oldExchangedEquipment.OnUnequip();
                return true;
            }

            // Test for basic requirement fulfillment.
            if( !equipment.FulfillsRequirements( statable ) ||
                !FitsSlot( equipment.Equipment.Slot, slot ) )
            {
                oldExchangedEquipment = null;
                return false;
            }

            // Unequip the current item.
            this.SetNull( slot, out oldExchangedEquipment );
            if( oldExchangedEquipment != null )
            {
                oldExchangedEquipment.OnUnequip();
                this.CheckItemRequirements();
            }

            // Try to equip the item now.
            if( equipment.FulfillsRequirements( statable ) )
            {
                // Yay :).
                this.SetRequirementFulfilled( slot, true );
                this.Set( slot, equipment );

                equipment.OnEquip( statable );
                this.NotifyRequirementsRecheckNeeded();
                return true;
            }
            else
            {
                // :(. Reequip the old item.
                this.Set( slot, oldExchangedEquipment );
                if( oldExchangedEquipment != null )
                    oldExchangedEquipment.OnEquip( statable );

                this.NotifyRequirementsRecheckNeeded();
                return false;
            }
        }

        /// <summary>
        /// Sets the item in the given EquipmentStatusSlot to null.
        /// </summary>
        /// <param name="slot">
        /// The slot to set for.
        /// </param>
        /// <param name="oldExchangedEquipment">
        /// Will contain the old item in the slot; if any.
        /// </param>
        private void SetNull( EquipmentStatusSlot slot, out EquipmentInstance oldExchangedEquipment )
        {
            switch( slot )
            {
                case EquipmentStatusSlot.Head:
                    oldExchangedEquipment = this.head;
                    this.head = null;
                    break;
                case EquipmentStatusSlot.Ranged:
                    oldExchangedEquipment = this.ranged;
                    this.ranged = null;
                    break;

                case EquipmentStatusSlot.WeaponHand:
                    oldExchangedEquipment = this.weaponHand;
                    this.weaponHand = null;
                    this.RefreshCanBlock();
                    break;
                case EquipmentStatusSlot.ShieldHand:
                    oldExchangedEquipment = this.shieldHand;
                    this.shieldHand = null;
                    this.RefreshCanBlock();
                    break;

                case EquipmentStatusSlot.Staff:
                    oldExchangedEquipment = this.staff;
                    this.staff = null;
                    break;
                case EquipmentStatusSlot.Chest:
                    oldExchangedEquipment = this.chest;
                    this.chest = null;
                    break;
                case EquipmentStatusSlot.Cloak:
                    oldExchangedEquipment = this.cloak;
                    this.cloak = null;
                    break;
                case EquipmentStatusSlot.Gloves:
                    oldExchangedEquipment = this.gloves;
                    this.gloves = null;
                    break;
                case EquipmentStatusSlot.Belt:
                    oldExchangedEquipment = this.belt;
                    this.belt = null;
                    break;
                case EquipmentStatusSlot.Boots:
                    oldExchangedEquipment = this.boots;
                    this.boots = null;
                    break;

                case EquipmentStatusSlot.Necklace1:
                    oldExchangedEquipment = this.necklace1;
                    this.necklace1 = null;
                    break;
                case EquipmentStatusSlot.Necklace2:
                    oldExchangedEquipment = this.necklace2;
                    this.necklace2 = null;
                    break;

                case EquipmentStatusSlot.Ring1:
                    oldExchangedEquipment = this.ring1;
                    this.ring1 = null;
                    break;
                case EquipmentStatusSlot.Ring2:
                    oldExchangedEquipment = this.ring2;
                    this.ring2 = null;
                    break;

                case EquipmentStatusSlot.Trinket1:
                    oldExchangedEquipment = this.trinket1;
                    this.trinket1 = null;
                    break;
                case EquipmentStatusSlot.Trinket2:
                    oldExchangedEquipment = this.trinket2;
                    this.trinket2 = null;
                    break;

                case EquipmentStatusSlot.Relic1:
                    oldExchangedEquipment = this.relic1;
                    this.relic1 = null;
                    break;
                case EquipmentStatusSlot.Relic2:
                    oldExchangedEquipment = this.relic2;
                    this.relic2 = null;
                    break;

                case EquipmentStatusSlot.Bag1:
                    oldExchangedEquipment = this.bag1;
                    this.bag1 = null;
                    break;
                case EquipmentStatusSlot.Bag2:
                    oldExchangedEquipment = this.bag2;
                    this.bag2 = null;
                    break;

                default:
                    oldExchangedEquipment = null;
                    break;
            }
        }

        #endregion

        #region Unequip

        /// <summary>
        /// Unequips the item in the specified <see cref="EquipmentStatusSlot"/>.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentStatusSlot.
        /// </param>
        /// <returns>
        /// The item that has been unequipped; if any.
        /// </returns>
        public EquipmentInstance Unequip( EquipmentStatusSlot slot )
        {
            EquipmentInstance equipment = Get( slot );
            if( equipment == null )
                return null;

            Set( slot, null );

            equipment.OnUnequip();
            CheckItemRequirements();

            return equipment;
        }

        #endregion

        #region FitsSlot

        /// <summary>
        /// Gets whether the specified slots work with eachother.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentSlot.
        /// </param>
        /// <param name="statusSlot">
        /// The related EquipmentStatusSlot.
        /// </param>
        /// <returns>
        /// Returns true if the given EquipmentSlot fits into the given EquipmentStatusSlot;
        /// otherwise false.
        /// </returns>
        public static bool FitsSlot( EquipmentSlot slot, EquipmentStatusSlot statusSlot )
        {
            switch( slot )
            {
                case EquipmentSlot.Head:
                    return statusSlot == EquipmentStatusSlot.Head;
                case EquipmentSlot.WeaponHand:
                    return statusSlot == EquipmentStatusSlot.WeaponHand;
                case EquipmentSlot.Ranged:
                    return statusSlot == EquipmentStatusSlot.Ranged;
                case EquipmentSlot.Staff:
                    return statusSlot == EquipmentStatusSlot.Staff;
                case EquipmentSlot.ShieldHand:
                    return statusSlot == EquipmentStatusSlot.ShieldHand;
                case EquipmentSlot.Chest:
                    return statusSlot == EquipmentStatusSlot.Chest;
                case EquipmentSlot.Gloves:
                    return statusSlot == EquipmentStatusSlot.Gloves;
                case EquipmentSlot.Belt:
                    return statusSlot == EquipmentStatusSlot.Belt;
                case EquipmentSlot.Boots:
                    return statusSlot == EquipmentStatusSlot.Boots;
                case EquipmentSlot.Cloak:
                    return statusSlot == EquipmentStatusSlot.Cloak;
                case EquipmentSlot.Necklace:
                    return statusSlot == EquipmentStatusSlot.Necklace1 || statusSlot == EquipmentStatusSlot.Necklace2;
                case EquipmentSlot.Ring:
                    return statusSlot == EquipmentStatusSlot.Ring1 || statusSlot == EquipmentStatusSlot.Ring2;
                case EquipmentSlot.Trinket:
                    return statusSlot == EquipmentStatusSlot.Trinket1 || statusSlot == EquipmentStatusSlot.Trinket2;
                case EquipmentSlot.Relic:
                    return statusSlot == EquipmentStatusSlot.Relic1 || statusSlot == EquipmentStatusSlot.Relic2;
                case EquipmentSlot.Bag:
                    return statusSlot == EquipmentStatusSlot.Bag1 || statusSlot == EquipmentStatusSlot.Bag2;
                default:
                    return false;
            }
        }

        #endregion

        #region TryToEnquipMissingItems

        /// <summary>
        /// Tries to enquip the items in the specified list.
        /// </summary>
        /// <param name="equipmentList">
        /// The list that contains the items that should be equiped.
        /// </param>
        /// <returns>
        /// Returns true if atleast one item has been enquiped;
        /// otherwise false.
        /// </returns>
        private bool TryToEnquip( List<EquipmentData> equipmentList )
        {
            bool hasEnquipedItem = false;
            EquipmentInstance dummy;

            for( int i = 0; i < equipmentList.Count; ++i )
            {
                EquipmentData data = equipmentList[i];

                if( this.Equip( data.Equipment, data.Slot, out dummy ) )
                {
                    equipmentList.RemoveAt( i );
                    hasEnquipedItem = true;
                }

                Debug.Assert( dummy == null );
            }

            return hasEnquipedItem;
        }

        #endregion

        #region > Has <

        /// <summary>
        /// Gets a value indicating whether the given EquipmentInstance is equipped in any slot.
        /// </summary>
        /// <param name="equipmentInstance">
        /// The equipmentInstance to check.
        /// </param>
        /// <returns>
        /// true if there is indeed equipped the given EquipmentInstance.
        /// otherwise false.
        /// </returns>
        public bool HasEquipped( EquipmentInstance equipmentInstance )
        {
            if( equipmentInstance == null )
            {
                return false;
            }

            EquipmentSlot equipmentSlot = equipmentInstance.Equipment.Slot;
            EquipmentStatusSlot slot = GetSlotForItem( equipmentSlot, getSecondSlotIfAvailable: false );
            if( Get( slot ) == equipmentInstance )
            {
                return true;
            }

            if( HasTwoSlots( equipmentSlot ) )
            {
                slot = GetSlotForItem( equipmentSlot, getSecondSlotIfAvailable: true );
                if( Get( slot ) == equipmentInstance )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the given EquipmentSlot has two slots.
        /// </summary>
        /// <param name="equipmentSlot">
        /// The slots to check.
        /// </param>
        /// <returns>
        /// true if there are two slots; -or- otherwise false. 
        /// </returns>
        private static bool HasTwoSlots( EquipmentSlot equipmentSlot )
        {
            switch( equipmentSlot )
            {
                case EquipmentSlot.Ring:
                case EquipmentSlot.Necklace:
                case EquipmentSlot.Trinket:
                case EquipmentSlot.Relic:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is an item equipped in any EquipmentStatusSlot
        /// that has the specified itemName.
        /// </summary>
        /// <param name="itemName">
        /// The name of the item.
        /// </param>
        /// <returns>
        /// true if there is indeed an item with the specified itemName;
        /// otherwise false.
        /// </returns>
        public bool HasEquipped( string itemName )
        {
            for( int i = 1; i < (int)EquipmentStatusSlot._EnumCount; ++i )
            {
                if( this.HasEquipped( itemName, (EquipmentStatusSlot)i ) )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether there is an item equipped in
        /// the specified EquipmentStatusSlot that has the specified itemName.
        /// </summary>
        /// <param name="itemName">
        /// The name of the item.
        /// </param>
        /// <param name="slot">
        /// The slot to check.
        /// </param>
        /// <returns>
        /// true if there is indeed an item with the specified itemName;
        /// otherwise false.
        /// </returns>
        private bool HasEquipped( string itemName, EquipmentStatusSlot slot )
        {
            var equipmentInstance = this.Get( slot );
            if( equipmentInstance == null )
                return false;

            string equippedItemName = GetBaseName( equipmentInstance );
            return equippedItemName.Equals( itemName, StringComparison.Ordinal );
        }

        /// <summary>
        /// Gets the base name of the specified EquipmentInstance.
        /// </summary>
        /// <param name="equipmentInstance">
        /// The input EquipmentInstance.
        /// </param>
        /// <returns>
        /// The base name of the specified EquipmentInstance.
        /// </returns>
        private static string GetBaseName( EquipmentInstance equipmentInstance )
        {
            Item item = null;

            var affixedInstance = equipmentInstance as Affixes.IAffixedItemInstance;
            if( affixedInstance != null )
            {
                item = affixedInstance.AffixedItem.BaseItem;
            }
            else
            {
                item = equipmentInstance.Item;
            }

            return item.Name;
        }

        /// <summary>
        /// Gets a value indicating whether there is an item in the specified EquipmentStatusSlot.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentStatusSlot.
        /// </param>
        /// <returns>
        /// Returns true if there is an item in the given EquipmentStatusSlot;
        /// otherwise false.
        /// </returns>
        public bool HasItemInSlot( EquipmentStatusSlot slot )
        {
            return Get( slot ) != null;
        }

        #endregion

        #region > Others <

        /// <summary>
        /// Fires the Changed event for the given EquipmentStatusSlot. 
        /// </summary>
        /// <param name="slot">
        /// The slot that has changed.
        /// </param>
        private void RaiseChangedEvent( EquipmentStatusSlot slot )
        {
            switch( slot )
            {
                case EquipmentStatusSlot.WeaponHand:
                    this.WeaponHandChanged.Raise( this );
                    break;

                default:
                    break;
            }

            SlotChanged.Raise( this, slot );
        }

        #endregion

        #region > Refresh <

        /// <summary>
        /// Recalculates how many stats the items give in the <see cref="EquipmentStatus"/>.
        /// </summary>
        public void RefreshStatsFromItems()
        {
            float modifier = 1.0f;
            var statModifiers = statable.EquipmentSlotStatModifiers;

            int updatedStrength = 0, updatedDexterity = 0, updatedAgility = 0,
                updatedVitality = 0, updatedIntelligence = 0, updatedLuck = 0,
                updatedArmor = 0;

            Equipment enquip;

            if( weaponHand != null && reqsFulfilledWeaponHand )
            {
                enquip = weaponHand.Equipment;
                updatedStrength += enquip.Strength;
                updatedDexterity += enquip.Dexterity;
                updatedAgility += enquip.Agility;
                updatedVitality += enquip.Vitality;
                updatedIntelligence += enquip.Intelligence;
                updatedLuck += enquip.Luck;
                updatedArmor += enquip.Armor;
            }

            if( ranged != null && reqsFulfilledRanged )
            {
                enquip = ranged.Equipment;
                updatedStrength += enquip.Strength;
                updatedDexterity += enquip.Dexterity;
                updatedAgility += enquip.Agility;
                updatedVitality += enquip.Vitality;
                updatedIntelligence += enquip.Intelligence;
                updatedLuck += enquip.Luck;
                updatedArmor += enquip.Armor;
            }

            if( shieldHand != null && reqsFulfilledShieldHand )
            {
                enquip = shieldHand.Equipment;
                updatedStrength += enquip.Strength;
                updatedDexterity += enquip.Dexterity;
                updatedAgility += enquip.Agility;
                updatedVitality += enquip.Vitality;
                updatedIntelligence += enquip.Intelligence;
                updatedLuck += enquip.Luck;
                updatedArmor += enquip.Armor;
            }

            if( staff != null && reqsFulfilledStaff )
            {
                enquip = staff.Equipment;
                updatedStrength += enquip.Strength;
                updatedDexterity += enquip.Dexterity;
                updatedAgility += enquip.Agility;
                updatedVitality += enquip.Vitality;
                updatedIntelligence += enquip.Intelligence;
                updatedLuck += enquip.Luck;
                updatedArmor += enquip.Armor;
            }

            if( head != null && reqsFulfilledHead )
            {
                enquip = head.Equipment;
                modifier = statModifiers.Head;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( gloves != null && reqsFulfilledGloves )
            {
                enquip = gloves.Equipment;
                modifier = statModifiers.Gloves;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( cloak != null && reqsFulfilledCloak )
            {
                enquip = cloak.Equipment;
                modifier = statModifiers.Cloak;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( belt != null && reqsFulfilledBelt )
            {
                enquip = belt.Equipment;
                modifier = statModifiers.Belt;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( boots != null && reqsFulfilledBoots )
            {
                enquip = boots.Equipment;
                modifier = statModifiers.Boots;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( chest != null && reqsFulfilledChest )
            {
                enquip = chest.Equipment;
                modifier = statModifiers.Chest;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( ring1 != null && reqsFulfilledRing1 )
            {
                enquip = ring1.Equipment;
                modifier = statModifiers.Rings;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( ring2 != null && reqsFulfilledRing2 )
            {
                enquip = ring2.Equipment;
                modifier = statModifiers.Rings;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( necklace1 != null && reqsFulfilledNecklace1 )
            {
                enquip = necklace1.Equipment;
                modifier = statModifiers.Necklaces;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( necklace2 != null && reqsFulfilledNecklace2 )
            {
                enquip = necklace2.Equipment;
                modifier = statModifiers.Necklaces;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( trinket1 != null && reqsFulfilledTrinket1 )
            {
                enquip = this.trinket1.Equipment;
                modifier = statModifiers.Trinkets;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( trinket2 != null && reqsFulfilledTrinket2 )
            {
                enquip = this.trinket2.Equipment;
                modifier = statModifiers.Trinkets;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( relic1 != null && reqsFulfilledRelic1 )
            {
                enquip = this.relic1.Equipment;
                modifier = statModifiers.Relics;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            if( relic2 != null && reqsFulfilledRelic2 )
            {
                enquip = this.relic2.Equipment;
                modifier = statModifiers.Relics;

                updatedStrength += (int)System.Math.Round( enquip.Strength * modifier );
                updatedDexterity += (int)System.Math.Round( enquip.Dexterity * modifier );
                updatedAgility += (int)System.Math.Round( enquip.Agility * modifier );
                updatedVitality += (int)System.Math.Round( enquip.Vitality * modifier );
                updatedIntelligence += (int)System.Math.Round( enquip.Intelligence * modifier );
                updatedLuck += (int)System.Math.Round( enquip.Luck * modifier );

                updatedArmor += enquip.Armor;
            }

            // Capture Old.
            int oldStrength = this.strength;
            int oldDexterity = this.dexterity;
            int oldAgility = this.agility;
            int oldVitality = this.vitality;
            int oldIntelligence = this.intelligence;
            int oldLuck = this.luck;
            int oldArmor = this.armor;

            // Update.
            this.strength = updatedStrength;
            this.dexterity = updatedDexterity;
            this.agility = updatedAgility;
            this.vitality = updatedVitality;
            this.intelligence = updatedIntelligence;
            this.luck = updatedLuck;
            this.armor = updatedArmor;

            // Refresh
            if( updatedStrength != oldStrength )
                this.statable.Refresh_Strength();

            if( updatedDexterity != oldDexterity )
                this.statable.Refresh_Dexterity();

            if( updatedVitality != oldVitality )
                this.statable.Refresh_Vitality();

            if( updatedIntelligence != oldIntelligence )
                this.statable.Refresh_Intelligence();

            if( updatedLuck != oldLuck )
                this.statable.Refresh_Luck();

            if( updatedAgility != oldAgility )
            {
                this.statable.Refresh_Agility();
            }
            else
            {
                // Refresh_Agility also calls Refresh_TotalArmor.
                if( updatedArmor != oldArmor )
                {
                    this.statable.Refresh_TotalArmor();
                }
            }
        }

        /// <summary>
        /// Refreshes whether the ExtendedStatable whose equipment status is descriped by this
        /// EquipmentStatus instance can block.
        /// </summary>
        /// <remarks>
        /// Blocking only works when an actual shield (armor > 0) is equipped, 
        /// and no two-handed weapon.
        /// </remarks>
        private void RefreshCanBlock()
        {
            if( this.shieldHand != null && this.shieldHand.Equipment.Armor > 0 )
            {
                if( this.weaponHand == null || this.weaponHand.Weapon.WeaponType.IsOneHanded() )
                {
                    this.statable.CanBlock = true;
                    return;
                }
            }

            this.statable.CanBlock = false;
        }

        #endregion

        #region > Gets / Sets <

        #region GetSlotForItem

        /// <summary>
        /// Gets the <see cref="EquipmentStatusSlot"/> that should be filled
        /// by an item that uses the specified <see cref="EquipmentSlot"/>.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentSlot.
        /// </param>
        /// <param name="preferSecondSlot">
        /// States whether the second slot should be prefered for items
        /// that have two slots.
        /// </param>
        /// <returns>
        /// The EquipmentStatusSlot for this EquipmentSlot.
        /// </returns>
        public EquipmentStatusSlot GetEmptySlotForItem( EquipmentSlot slot, bool preferSecondSlot = false )
        {
            switch( slot )
            {
                case EquipmentSlot.Head:
                    return EquipmentStatusSlot.Head;
                case EquipmentSlot.Belt:
                    return EquipmentStatusSlot.Belt;
                case EquipmentSlot.Boots:
                    return EquipmentStatusSlot.Boots;
                case EquipmentSlot.Chest:
                    return EquipmentStatusSlot.Chest;
                case EquipmentSlot.Cloak:
                    return EquipmentStatusSlot.Cloak;
                case EquipmentSlot.Gloves:
                    return EquipmentStatusSlot.Gloves;
                case EquipmentSlot.ShieldHand:
                    return EquipmentStatusSlot.ShieldHand;
                case EquipmentSlot.WeaponHand:
                    return EquipmentStatusSlot.WeaponHand;
                case EquipmentSlot.Ranged:
                    return EquipmentStatusSlot.Ranged;
                case EquipmentSlot.Staff:
                    return EquipmentStatusSlot.Staff;
                case EquipmentSlot.Necklace:
                    if( preferSecondSlot )
                    {
                        if( this.necklace2 == null )
                            return EquipmentStatusSlot.Necklace2;
                        if( this.necklace1 == null )
                            return EquipmentStatusSlot.Necklace1;
                        return EquipmentStatusSlot.Necklace2;
                    }
                    else
                    {
                        if( this.necklace1 == null )
                            return EquipmentStatusSlot.Necklace1;
                        if( this.necklace2 == null )
                            return EquipmentStatusSlot.Necklace2;
                        return EquipmentStatusSlot.Necklace1;
                    }

                case EquipmentSlot.Ring:
                    if( preferSecondSlot )
                    {
                        if( this.ring2 == null )
                            return EquipmentStatusSlot.Ring2;
                        if( this.ring1 == null )
                            return EquipmentStatusSlot.Ring1;
                        return EquipmentStatusSlot.Ring2;
                    }
                    else
                    {
                        if( this.ring1 == null )
                            return EquipmentStatusSlot.Ring1;
                        if( this.ring2 == null )
                            return EquipmentStatusSlot.Ring2;
                        return EquipmentStatusSlot.Ring1;
                    }
                case EquipmentSlot.Trinket:
                    if( preferSecondSlot )
                    {
                        if( this.trinket2 == null )
                            return EquipmentStatusSlot.Trinket2;
                        if( this.trinket1 == null )
                            return EquipmentStatusSlot.Trinket1;
                        return EquipmentStatusSlot.Trinket2;
                    }
                    else
                    {
                        if( this.trinket1 == null )
                            return EquipmentStatusSlot.Trinket1;
                        if( this.trinket2 == null )
                            return EquipmentStatusSlot.Trinket2;
                        return EquipmentStatusSlot.Trinket1;
                    }

                case EquipmentSlot.Relic:
                    if( preferSecondSlot )
                    {
                        if( this.relic2 == null )
                            return EquipmentStatusSlot.Relic2;
                        if( this.relic1 == null )
                            return EquipmentStatusSlot.Relic1;
                        return EquipmentStatusSlot.Relic2;
                    }
                    else
                    {
                        if( this.relic1 == null )
                            return EquipmentStatusSlot.Relic1;
                        if( this.relic2 == null )
                            return EquipmentStatusSlot.Relic2;
                        return EquipmentStatusSlot.Relic1;
                    }

                case EquipmentSlot.Bag:
                    if( preferSecondSlot )
                    {
                        if( this.bag2 == null )
                            return EquipmentStatusSlot.Bag2;
                        if( this.bag1 == null )
                            return EquipmentStatusSlot.Bag1;
                        return EquipmentStatusSlot.Bag2;
                    }
                    else
                    {
                        if( this.bag1 == null )
                            return EquipmentStatusSlot.Bag1;
                        if( this.bag2 == null )
                            return EquipmentStatusSlot.Bag2;
                        return EquipmentStatusSlot.Bag1;
                    }

                default:
                    return EquipmentStatusSlot.None;
            }
        }

        #endregion

        #region GetSlotForItem

        /// <summary>
        /// Gets the <see cref="EquipmentStatusSlot"/> that corresponds with the specified <see cref="EquipmentSlot"/>.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentSlot.
        /// </param>
        /// <param name="getSecondSlotIfAvailable">
        /// States whether the second slot (if available for that slot) should be received.
        /// </param>
        /// <returns>
        /// The EquipmentStatusSlot for this EquipmentSlot.
        /// </returns>
        public EquipmentStatusSlot GetSlotForItem( EquipmentSlot slot, bool getSecondSlotIfAvailable = false )
        {
            switch( slot )
            {
                case EquipmentSlot.Head:
                    return EquipmentStatusSlot.Head;
                case EquipmentSlot.Belt:
                    return EquipmentStatusSlot.Belt;
                case EquipmentSlot.Boots:
                    return EquipmentStatusSlot.Boots;
                case EquipmentSlot.Chest:
                    return EquipmentStatusSlot.Chest;
                case EquipmentSlot.Cloak:
                    return EquipmentStatusSlot.Cloak;
                case EquipmentSlot.Gloves:
                    return EquipmentStatusSlot.Gloves;
                case EquipmentSlot.ShieldHand:
                    return EquipmentStatusSlot.ShieldHand;
                case EquipmentSlot.WeaponHand:
                    return EquipmentStatusSlot.WeaponHand;
                case EquipmentSlot.Ranged:
                    return EquipmentStatusSlot.Ranged;
                case EquipmentSlot.Staff:
                    return EquipmentStatusSlot.Staff;
                case EquipmentSlot.Necklace:
                    if( getSecondSlotIfAvailable )
                    {
                        return EquipmentStatusSlot.Necklace2;
                    }
                    else
                    {
                        return EquipmentStatusSlot.Necklace1;
                    }

                case EquipmentSlot.Ring:
                    if( getSecondSlotIfAvailable )
                    {
                        return EquipmentStatusSlot.Ring2;
                    }
                    else
                    {
                        return EquipmentStatusSlot.Ring1;
                    }

                case EquipmentSlot.Trinket:
                    if( getSecondSlotIfAvailable )
                    {
                        return EquipmentStatusSlot.Trinket2;
                    }
                    else
                    {
                        return EquipmentStatusSlot.Trinket1;
                    }

                case EquipmentSlot.Relic:
                    if( getSecondSlotIfAvailable )
                    {
                        return EquipmentStatusSlot.Relic2;
                    }
                    else
                    {
                        return EquipmentStatusSlot.Relic1;
                    }

                case EquipmentSlot.Bag:
                    if( getSecondSlotIfAvailable )
                    {
                        return EquipmentStatusSlot.Bag2;
                    }
                    else
                    {
                        return EquipmentStatusSlot.Bag1;
                    }

                default:
                    return EquipmentStatusSlot.None;
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Gets the <see cref="Equipment"/> at the specified slot.
        /// </summary>
        /// <param name="slot"> The slot to get. </param>
        /// <returns> The <see cref="Equipment"/> or null. </returns>
        public EquipmentInstance Get( EquipmentStatusSlot slot )
        {
            switch( slot )
            {
                case EquipmentStatusSlot.Head:
                    return head;
                case EquipmentStatusSlot.Chest:
                    return chest;
                case EquipmentStatusSlot.Belt:
                    return belt;
                case EquipmentStatusSlot.Boots:
                    return boots;
                case EquipmentStatusSlot.Cloak:
                    return cloak;
                case EquipmentStatusSlot.Gloves:
                    return gloves;
                case EquipmentStatusSlot.Necklace1:
                    return necklace1;
                case EquipmentStatusSlot.Necklace2:
                    return necklace2;
                case EquipmentStatusSlot.Ring1:
                    return ring1;
                case EquipmentStatusSlot.Ring2:
                    return ring2;
                case EquipmentStatusSlot.ShieldHand:
                    return shieldHand;
                case EquipmentStatusSlot.Trinket1:
                    return trinket1;
                case EquipmentStatusSlot.Trinket2:
                    return trinket2;
                case EquipmentStatusSlot.WeaponHand:
                    return weaponHand;
                case EquipmentStatusSlot.Ranged:
                    return ranged;
                case EquipmentStatusSlot.Staff:
                    return staff;
                case EquipmentStatusSlot.Relic1:
                    return relic1;
                case EquipmentStatusSlot.Relic2:
                    return relic2;
                case EquipmentStatusSlot.Bag1:
                    return bag1;
                case EquipmentStatusSlot.Bag2:
                    return bag2;
                default:
                    return null;
            }
        }

        #endregion

        #region Set

        /// <summary>
        /// Helper method that sets the <see cref="Equipment"/> at the specified slot.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentStatusSlot to set.
        /// </param>
        /// <param name="value">
        /// The EquipmentInstance to set for the given EquipmentStatusSlot.
        /// </param>
        private void Set( EquipmentStatusSlot slot, EquipmentInstance value )
        {
            if( this.Get( slot ) == value )
                return;

            switch( slot )
            {
                case EquipmentStatusSlot.Head:
                    this.head = value;
                    break;
                case EquipmentStatusSlot.Chest:
                    this.chest = value;
                    break;
                case EquipmentStatusSlot.Belt:
                    this.belt = value;
                    break;
                case EquipmentStatusSlot.Boots:
                    this.boots = value;
                    break;
                case EquipmentStatusSlot.Gloves:
                    this.gloves = value;
                    break;
                case EquipmentStatusSlot.Cloak:
                    this.cloak = value;
                    break;
                case EquipmentStatusSlot.Necklace1:
                    this.necklace1 = value;
                    break;
                case EquipmentStatusSlot.Necklace2:
                    this.necklace2 = value;
                    break;
                case EquipmentStatusSlot.Ring1:
                    this.ring1 = value;
                    break;
                case EquipmentStatusSlot.Ring2:
                    this.ring2 = value;
                    break;
                case EquipmentStatusSlot.Trinket1:
                    this.trinket1 = value;
                    break;
                case EquipmentStatusSlot.Trinket2:
                    this.trinket2 = value;
                    break;
                case EquipmentStatusSlot.WeaponHand:
                    this.weaponHand = (WeaponInstance)value;
                    this.RefreshCanBlock();
                    break;
                case EquipmentStatusSlot.ShieldHand:
                    this.shieldHand = value;
                    this.RefreshCanBlock();
                    break;
                case EquipmentStatusSlot.Ranged:
                    this.ranged = (WeaponInstance)value;
                    break;
                case EquipmentStatusSlot.Staff:
                    this.staff = value;
                    break;
                case EquipmentStatusSlot.Relic1:
                    this.relic1 = value;
                    break;
                case EquipmentStatusSlot.Relic2:
                    this.relic2 = value;
                    break;
                case EquipmentStatusSlot.Bag1:
                    this.bag1 = value;
                    break;
                case EquipmentStatusSlot.Bag2:
                    this.bag2 = value;
                    break;
                default:
                    break;
            }

            this.RaiseChangedEvent( slot );
        }

        #endregion

        #endregion

        #region > Requirements <

        /// <summary>
        /// Notifies this EquipmentStatus that the item requirements
        /// should be re-checked next frame.
        /// </summary>
        public void NotifyRequirementsRecheckNeeded()
        {
            this.requirementsRecheckIsNeeded = true;
        }

        /// <summary>
        /// Checks whether items fulfill their requirements and adjusts the
        /// inventory on new situations.
        /// </summary>
        public void CheckItemRequirements()
        {
            int life = this.statable.Life;
            int mana = this.statable.Mana;
            this.CheckAllSlots();

            // restore the previous power status
            this.statable.Life = life;
            this.statable.Mana = mana;
            this.requirementsRecheckIsNeeded = false;
        }

        /// <summary>
        /// Disables -all- equipped items by setting the requirement fulfillment to false.
        /// </summary>
        public void DisableAllItems()
        {
            for( int i = 0; i < (int)EquipmentStatusSlot._EnumCount; ++i )
            {
                var slot = (EquipmentStatusSlot)i;
                EquipmentInstance equip = Get( slot );

                if( equip != null )
                {
                    SetRequirementFulfilled( slot, false );
                    equip.OnUnequipNoCheck();
                }
            }

            RefreshStatsFromItems();
        }

        /// <summary>
        /// Checks whether items fulfill their requirements and adjusts the
        /// inventory on new situations.
        /// </summary>
        private void CheckAllSlots()
        {
            DisableAllItems();

            while( TryEnableItems() ) { }
        }

        private bool TryEnableItems()
        {
            bool hasEnabledAny = false;

            for( int i = 0; i < (int)EquipmentStatusSlot._EnumCount; ++i )
            {
                var slot = (EquipmentStatusSlot)i;

                EquipmentInstance equip = Get( slot );

                if( equip != null )
                {
                    if( !IsRequirementFulfilled( slot ) )
                    {
                        if( equip.FulfillsRequirements( statable ) )
                        {
                            hasEnabledAny = true;
                            SetRequirementFulfilled( slot, true );
                            equip.OnEquip( statable );
                        }
                    }
                }
            }

            return hasEnabledAny;
        }

        /// <summary>
        /// Checks whether the specified slot fulfills all requirements.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentStatusSlot.
        /// </param>
        /// <returns>
        /// Whether a recheck of all slots is needed.
        /// </returns>
        private bool CheckSlotFulfillsReqs( EquipmentStatusSlot slot )
        {
            EquipmentInstance equip = Get( slot );
            if( equip == null )
                return false;

            bool needRecheckAll = false;

            // remove and see whether it fulfills them without it
            this.Set( slot, null );
            equip.OnUnequipNoCheck();
            this.RefreshStatsFromItems();

            if( equip.FulfillsRequirements( statable ) )
            {
                this.SetRequirementFulfilled( slot, true );
                this.Set( slot, equip );

                equip.OnEquipNoCheck( statable );
                this.RefreshStatsFromItems();
            }
            else
            {
                // we need to recheck all items if this item previously fulfilled it
                needRecheckAll = this.IsRequirementFulfilled( slot );
                this.SetRequirementFulfilled( slot, false );
                this.Set( slot, equip );
            }

            // reset
            return needRecheckAll;
        }

        /// <summary>
        /// Gets whether the requirements are fulfilled for the specified slot.
        /// </summary>
        /// <param name="slot">
        /// The related EquipmentStatusSlot.
        /// </param>
        /// <returns>
        /// Returns true if the player fulfills the requirements
        /// for the item in the specified EquipmentStatusSlot;
        /// otherwise false.
        /// </returns>
        public bool IsRequirementFulfilled( EquipmentStatusSlot slot )
        {
            switch( slot )
            {
                case EquipmentStatusSlot.Head:
                    return this.reqsFulfilledHead;
                case EquipmentStatusSlot.Belt:
                    return this.reqsFulfilledBelt;
                case EquipmentStatusSlot.Boots:
                    return this.reqsFulfilledBoots;
                case EquipmentStatusSlot.Chest:
                    return this.reqsFulfilledChest;
                case EquipmentStatusSlot.Cloak:
                    return this.reqsFulfilledCloak;
                case EquipmentStatusSlot.Gloves:
                    return this.reqsFulfilledGloves;
                case EquipmentStatusSlot.Necklace1:
                    return this.reqsFulfilledNecklace1;
                case EquipmentStatusSlot.Necklace2:
                    return this.reqsFulfilledNecklace2;
                case EquipmentStatusSlot.Ring1:
                    return this.reqsFulfilledRing1;
                case EquipmentStatusSlot.Ring2:
                    return this.reqsFulfilledRing2;
                case EquipmentStatusSlot.ShieldHand:
                    return this.reqsFulfilledShieldHand;
                case EquipmentStatusSlot.Trinket1:
                    return this.reqsFulfilledTrinket1;
                case EquipmentStatusSlot.Trinket2:
                    return this.reqsFulfilledTrinket2;
                case EquipmentStatusSlot.WeaponHand:
                    return this.reqsFulfilledWeaponHand;
                case EquipmentStatusSlot.Ranged:
                    return this.reqsFulfilledRanged;
                case EquipmentStatusSlot.Staff:
                    return this.reqsFulfilledStaff;
                case EquipmentStatusSlot.Relic1:
                    return this.reqsFulfilledRelic1;
                case EquipmentStatusSlot.Relic2:
                    return this.reqsFulfilledRelic2;
                case EquipmentStatusSlot.Bag1:
                    return this.reqsFulfilledBag1;
                case EquipmentStatusSlot.Bag2:
                    return this.reqsFulfilledBag2;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Sets whether the requirements are fulfilled for the specified slot.
        /// </summary>
        /// <param name="slot">The related EquipmentStatusSlot.</param>
        /// <param name="value">The new value.</param>
        private void SetRequirementFulfilled( EquipmentStatusSlot slot, bool value )
        {
            if( this.IsRequirementFulfilled( slot ) == value )
                return;

            switch( slot )
            {
                case EquipmentStatusSlot.Head:
                    this.reqsFulfilledHead = value;
                    break;
                case EquipmentStatusSlot.Boots:
                    this.reqsFulfilledBoots = value;
                    break;
                case EquipmentStatusSlot.Belt:
                    this.reqsFulfilledBelt = value;
                    break;
                case EquipmentStatusSlot.Chest:
                    this.reqsFulfilledChest = value;
                    break;
                case EquipmentStatusSlot.Gloves:
                    this.reqsFulfilledGloves = value;
                    break;
                case EquipmentStatusSlot.Cloak:
                    this.reqsFulfilledCloak = value;
                    break;
                case EquipmentStatusSlot.Necklace1:
                    this.reqsFulfilledNecklace1 = value;
                    break;
                case EquipmentStatusSlot.Necklace2:
                    this.reqsFulfilledNecklace2 = value;
                    break;
                case EquipmentStatusSlot.Ring1:
                    this.reqsFulfilledRing1 = value;
                    break;
                case EquipmentStatusSlot.Ring2:
                    this.reqsFulfilledRing2 = value;
                    break;
                case EquipmentStatusSlot.ShieldHand:
                    this.reqsFulfilledShieldHand = value;
                    break;
                case EquipmentStatusSlot.Trinket1:
                    this.reqsFulfilledTrinket1 = value;
                    break;
                case EquipmentStatusSlot.Trinket2:
                    this.reqsFulfilledTrinket2 = value;
                    break;
                case EquipmentStatusSlot.WeaponHand:
                    this.reqsFulfilledWeaponHand = value;
                    break;
                case EquipmentStatusSlot.Ranged:
                    this.reqsFulfilledRanged = value;
                    break;
                case EquipmentStatusSlot.Staff:
                    this.reqsFulfilledStaff = value;
                    break;
                case EquipmentStatusSlot.Relic1:
                    this.reqsFulfilledRelic1 = value;
                    break;
                case EquipmentStatusSlot.Relic2:
                    this.reqsFulfilledRelic2 = value;
                    break;
                case EquipmentStatusSlot.Bag1:
                    this.reqsFulfilledBag1 = value;
                    break;
                case EquipmentStatusSlot.Bag2:
                    this.reqsFulfilledBag2 = value;
                    break;
                default:
                    break;
            }

            this.RaiseChangedEvent( slot );
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes/Writes the data to descripe this EquipmentStatus.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 7;
            context.Write( CurrentVersion );

            this.WriteSlot( EquipmentStatusSlot.WeaponHand, context );
            this.WriteSlot( EquipmentStatusSlot.ShieldHand, context );
            this.WriteSlot( EquipmentStatusSlot.Ranged, context );
            this.WriteSlot( EquipmentStatusSlot.Chest, context );
            this.WriteSlot( EquipmentStatusSlot.Head, context );
            this.WriteSlot( EquipmentStatusSlot.Boots, context );
            this.WriteSlot( EquipmentStatusSlot.Ring1, context );
            this.WriteSlot( EquipmentStatusSlot.Ring2, context );
            this.WriteSlot( EquipmentStatusSlot.Necklace1, context );
            this.WriteSlot( EquipmentStatusSlot.Necklace2, context );
            this.WriteSlot( EquipmentStatusSlot.Trinket1, context );
            this.WriteSlot( EquipmentStatusSlot.Trinket2, context );
            this.WriteSlot( EquipmentStatusSlot.Belt, context );
            this.WriteSlot( EquipmentStatusSlot.Staff, context );
            this.WriteSlot( EquipmentStatusSlot.Relic1, context );
            this.WriteSlot( EquipmentStatusSlot.Relic2, context );
            this.WriteSlot( EquipmentStatusSlot.Gloves, context );
            this.WriteSlot( EquipmentStatusSlot.Cloak, context );
            this.WriteSlot( EquipmentStatusSlot.Bag1, context ); // New in 7
            this.WriteSlot( EquipmentStatusSlot.Bag2, context );
        }

        /// <summary>
        /// Writes the state of the given EquipmentStatusSlot using the given BinaryWriter. 
        /// </summary>
        /// <param name="slot">
        /// The EquipmentStatusSlot to write.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void WriteSlot( EquipmentStatusSlot slot, Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( (int)slot );
            var equipment = Get( slot );

            if( equipment == null )
            {
                context.Write( false );
            }
            else
            {
                context.Write( true );
                equipment.Serialize( context );
            }
        }

        /// <summary>
        /// Deserializes/Reads the data to descripe this EquipmentStatus.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 7;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            this.statable.RefreshStatus();

            ReadSlot( EquipmentStatusSlot.WeaponHand, context );
            ReadSlot( EquipmentStatusSlot.ShieldHand, context );
            ReadSlot( EquipmentStatusSlot.Ranged, context );
            ReadSlot( EquipmentStatusSlot.Chest, context );
            ReadSlot( EquipmentStatusSlot.Head, context );
            ReadSlot( EquipmentStatusSlot.Boots, context );
            ReadSlot( EquipmentStatusSlot.Ring1, context );
            ReadSlot( EquipmentStatusSlot.Ring2, context );
            ReadSlot( EquipmentStatusSlot.Necklace1, context );
            ReadSlot( EquipmentStatusSlot.Necklace2, context );
            ReadSlot( EquipmentStatusSlot.Trinket1, context );
            ReadSlot( EquipmentStatusSlot.Trinket2, context );

            if( version >= 2 )
            {
                ReadSlot( EquipmentStatusSlot.Belt, context );
            }

            if( version >= 3 )
            {
                ReadSlot( EquipmentStatusSlot.Staff, context );
            }

            if( version >= 4 )
            {
                ReadSlot( EquipmentStatusSlot.Relic1, context );
                ReadSlot( EquipmentStatusSlot.Relic2, context );
            }

            if( version >= 5 )
            {
                ReadSlot( EquipmentStatusSlot.Gloves, context );
            }

            if( version >= 6 )
            {
                ReadSlot( EquipmentStatusSlot.Cloak, context );
            }

            if( version >= 7 )
            {
                ReadSlot( EquipmentStatusSlot.Bag1, context );
                ReadSlot( EquipmentStatusSlot.Bag2, context );
            }

            CheckItemRequirements();
        }

        /// <summary>
        /// Reads the specified <see cref="EquipmentStatusSlot"/>.
        /// </summary>
        /// <param name="slot">
        /// The EquipmentStatusSlot to read.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void ReadSlot( EquipmentStatusSlot slot, Zelda.Saving.IZeldaDeserializationContext context )
        {
            var actualSlot = (EquipmentStatusSlot)context.ReadInt32();
            Debug.Assert( slot == actualSlot );

            if( context.ReadBoolean() )
            {
                var equipment = ItemInstance.Read( context ) as EquipmentInstance;
                if( equipment == null )
                    return;

                Set( actualSlot, equipment );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The total armor the equiped items provide.
        /// </summary>
        private int armor;

        /// <summary>
        /// The stats the equiped items provide.
        /// </summary>
        private int strength, dexterity, agility, vitality, intelligence, luck;

        /// <summary>
        /// States whether <see cref="CheckItemRequirements"/> should
        /// be called in the next Update.
        /// </summary>
        private bool requirementsRecheckIsNeeded;

        /// <summary>
        /// Stores whether the equiped items fulfill their requirements.
        /// </summary>
        private bool reqsFulfilledWeaponHand, reqsFulfilledShieldHand, reqsFulfilledRanged, reqsFulfilledStaff,
                     reqsFulfilledChest, reqsFulfilledBoots, reqsFulfilledBelt, reqsFulfilledHead, reqsFulfilledGloves,
                     reqsFulfilledCloak,
                     reqsFulfilledNecklace1, reqsFulfilledNecklace2,
                     reqsFulfilledRing1, reqsFulfilledRing2,
                     reqsFulfilledTrinket1, reqsFulfilledTrinket2,
                     reqsFulfilledRelic1, reqsFulfilledRelic2,
                     reqsFulfilledBag1, reqsFulfilledBag2;

        /// <summary>
        /// The main hand slot.
        /// </summary>
        private WeaponInstance weaponHand;

        /// <summary>
        /// The ranged weapon slot.
        /// </summary>
        private WeaponInstance ranged;

        /// <summary>
        /// The shield hand slot.
        /// </summary>
        private EquipmentInstance shieldHand;

        /// <summary>
        /// The staff slot.
        /// </summary>
        private EquipmentInstance staff;

        /// <summary>
        /// The head slot.
        /// </summary>
        private EquipmentInstance head;

        /// <summary>
        /// The chest slot.
        /// </summary>
        private EquipmentInstance chest;

        /// <summary>
        /// The gloves slot.
        /// </summary>
        private EquipmentInstance gloves;

        /// <summary>
        /// The boots slot.
        /// </summary>
        private EquipmentInstance boots;

        /// <summary>
        /// The belt slot.
        /// </summary>
        private EquipmentInstance belt;

        /// <summary>
        /// The cloak slot.
        /// </summary>
        private EquipmentInstance cloak;

        /// <summary>
        /// The necklace slots.
        /// </summary>
        private EquipmentInstance necklace1, necklace2;

        /// <summary>
        /// The ring slots.
        /// </summary>
        private EquipmentInstance ring1, ring2;

        /// <summary>
        /// The trinket slots.
        /// </summary>
        private EquipmentInstance trinket1, trinket2;

        /// <summary>
        /// The relic slots.
        /// </summary>
        private EquipmentInstance relic1, relic2;

        /// <summary>
        /// The bag slots.
        /// </summary>
        private EquipmentInstance bag1, bag2;

        /// <summary>
        /// Identifies the <see cref="ExtendedStatable"/> component of the ZeldaEntity whos 
        /// equipment status the <see cref="EquipmentStatus"/> represents.
        /// </summary>
        private readonly ExtendedStatable statable;

        #endregion

        #region struct EquipmentData

        /// <summary>
        /// Stores the data of an loaded item.
        /// </summary>
        private struct EquipmentData
        {
            /// <summary>
            /// The EquipmentInstance object.
            /// </summary>
            public readonly EquipmentInstance Equipment;

            /// <summary>
            /// The EquipmentStatusSlot the EquipmentInstance should have gone in.
            /// </summary>
            public readonly EquipmentStatusSlot Slot;

            /// <summary>
            /// Initializes a new instance of the <see cref="EquipmentData"/> structure.
            /// </summary>
            /// <param name="equip">The EquipmentInstance object.</param>
            /// <param name="slot">The EquipmentStatusSlot the EquipmentInstance should have gone in.</param>
            public EquipmentData( EquipmentInstance equip, EquipmentStatusSlot slot )
            {
                this.Equipment = equip;
                this.Slot = slot;
            }
        }

        #endregion
    }
}