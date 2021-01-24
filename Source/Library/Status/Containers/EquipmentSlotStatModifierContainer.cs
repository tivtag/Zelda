// <copyright file="EquipmentSlotStatModifierContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.EquipmentSlotStatModifierContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Containers
{
    using System;
    using Zelda.Items;

    /// <summary>
    /// Encapsulates the stat modifiers that are applied to the stats
    /// given from items with a specific <see cref="EquipmentSlot"/>.
    /// This class can't be inherited.
    /// </summary>
    /// <seealso cref="EquipmentSlotStatModifierEffect"/>
    public sealed class EquipmentSlotStatModifierContainer
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Chest"/>.
        /// </summary>
        public float Chest
        {
            get
            { 
                return this.chest;
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Boots"/>.
        /// </summary>
        public float Boots
        {
            get 
            {
                return this.boots;
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Belt"/>.
        /// </summary>
        public float Belt
        {
            get
            {
                return this.belt;
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Head"/>.
        /// </summary>
        public float Head
        {
            get 
            { 
                return this.head; 
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Gloves"/>.
        /// </summary>
        public float Gloves
        {
            get 
            {
                return this.gloves;
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Cloak"/>.
        /// </summary>
        public float Cloak
        {
            get
            { 
                return this.cloak; 
            }
        }
        
        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Ring"/>.
        /// </summary>
        public float Rings
        {
            get 
            { 
                return this.rings;
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Trinket"/>.
        /// </summary>
        public float Trinkets
        {
            get 
            { 
                return this.trinkets; 
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Necklace"/>.
        /// </summary>
        public float Necklaces
        {
            get 
            {
                return this.necklaces; 
            }
        }

        /// <summary>
        /// Gets the modifier value applied to the stats given 
        /// by items in the <see cref="EquipmentSlot.Relic"/>.
        /// </summary>
        public float Relics
        {
            get 
            { 
                return this.relics;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes a new instance of the EquipmentSlotStatModifierContainer class.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that owns the new EquipmentSlotStatModifierContainer.
        /// </param>
        internal EquipmentSlotStatModifierContainer( ExtendedStatable statable )
        {
            this.statable = statable;
        }

        /// <summary>
        /// Refreshes the multiplier modifier value for the specified EquipmentSlot.
        /// </summary>
        /// <param name="slot">
        /// The EquipmentSlot that should be refreshed.
        /// </param>
        public void Refresh( EquipmentSlot slot )
        {
            float multiplier = this.statable.AuraList.GetPercentalEffectValue(
                EquipmentSlotStatModifierEffect.GetIdentifier( slot )
            );

            this.Set( slot, multiplier );
            this.statable.Equipment.RefreshStatsFromItems();
        }

        /// <summary>
        /// Sets the multiplier value for the specified EquipmentSlot.
        /// </summary>
        /// <param name="slot">
        /// The slot whose multiplier should be set.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        private void Set( EquipmentSlot slot, float value )
        {
            switch( slot )
            {
                case EquipmentSlot.Belt:
                    this.belt = value;
                    break;

                case EquipmentSlot.Boots:
                    this.boots = value;
                    break;

                case EquipmentSlot.Chest:
                    this.chest = value;
                    break;

                case EquipmentSlot.Cloak:
                    this.cloak = value;
                    break;

                case EquipmentSlot.Gloves:
                    this.gloves = value;
                    break;

                case EquipmentSlot.Head:
                    this.head = value;
                    break;

                case EquipmentSlot.Necklace:
                    this.necklaces = value;
                    break;

                case EquipmentSlot.Relic:
                    this.relics = value;
                    break;

                case EquipmentSlot.Ring:
                    this.rings = value;
                    break;

                case EquipmentSlot.Trinket:
                    this.trinkets = value;
                    break;

                default:
                    throw new NotImplementedException( slot.ToString() );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the multiplier values that are applied to stats provided by items of a specic EquipmentSlot.
        /// </summary>
        private float 
            chest = 1.0f, boots = 1.0f, head = 1.0f, 
            gloves = 1.0f, belt = 1.0f, cloak = 1.0f,
            rings = 1.0f, necklaces = 1.0f,
            trinkets = 1.0f, relics = 1.0f;

        /// <summary>
        /// Identifies the ExtendedStatable component that owns this EquipmentSlotStatModifierContainer.
        /// </summary>
        private readonly ExtendedStatable statable;

        #endregion
    }
}
