// <copyright file="EquipmentSlotStatModifierEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.EquipmentSlotStatModifierEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System.Globalization;
    using Zelda.Items;

    /// <summary>
    /// Represents a <see cref="StatusValueEffect"/> that
    /// manipulates how many <see cref="Stat"/> points items
    /// in a specific <see cref="EquipmentSlot"/> give.
    /// </summary>
    public sealed class EquipmentSlotStatModifierEffect : StatusValueEffect
    {
        #region [ Identification ]
        
        /// <summary>
        /// Gets the string that uniquely identifies what a specific EquipmentStatModifierEffect
        /// manipulates.
        /// </summary>
        /// <param name="equipmentSlot">
        /// The <see cref="EquipmentSlot"/> the EquipmentStatModifierEffect is related to.
        /// </param>
        /// <returns>
        /// The manipulation identification string.
        /// </returns>
        internal static string GetIdentifier( EquipmentSlot equipmentSlot )
        {
            return "StatMod_" + equipmentSlot.ToString();
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.EquipmentSlot );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="EquipmentSlot"/> this EquipmentStatModifierEffect
        /// is related to.
        /// </summary>
        public EquipmentSlot EquipmentSlot
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Resources.ED_EquipmentStatModifierEffect,
                LocalizedEnums.Get( this.EquipmentSlot )
            );

            if( this.Value >= 0.0f )
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ED_PosPercentage,
                    message,
                    this.Value.ToString( CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ED_NegPercentage,
                    message,
                    (-this.Value).ToString( CultureInfo.CurrentCulture )
                );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the EquipmentSlotStatModifierEffect class.
        /// </summary>
        public EquipmentSlotStatModifierEffect()
            : this( 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the EquipmentSlotStatModifierEffect class.
        /// </summary>
        /// <param name="value">
        /// The initial effect value of the new EquipmentSlotStatModifierEffect.
        /// </param>
        public EquipmentSlotStatModifierEffect( float value )
            : base( value, StatusManipType.Percental )
        {
        }

        /// <summary>
        /// Initializes a new instance of the EquipmentSlotStatModifierEffect class.
        /// </summary>
        /// <param name="slot">
        /// The <see cref="EquipmentSlot"/> the new EquipmentSlotStatModifierEffect is related to.
        /// </param>
        /// <param name="value">
        /// The initial effect value of the new EquipmentSlotStatModifierEffect.
        /// </param>
        public EquipmentSlotStatModifierEffect( EquipmentSlot slot, float value )
            : base( value, StatusManipType.Percental )
        {
            this.EquipmentSlot = slot;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.Refresh( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.Refresh( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled/disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void Refresh( ExtendedStatable user )
        {
            user.EquipmentSlotStatModifiers.Refresh( this.EquipmentSlot );
        }

        /// <summary>
        /// Returns a clone of this EquipmentSlotStatModifierEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new EquipmentSlotStatModifierEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}