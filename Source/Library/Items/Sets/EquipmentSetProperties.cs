// <copyright file="EquipmentSetProperties.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.EquipmentSetProperties class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Sets
{
    using System;

    /// <summary>
    /// Descripes the <see cref="ISet"/> properties of an <see cref="EquipmentInstance"/>.
    /// </summary>
    public sealed class EquipmentSetProperties
    {
        /// <summary>
        /// Initializes a new instance of the EquipmentSetProperties class.
        /// </summary>
        /// <param name="equipmentInstance">
        /// The EquipmentInstance that is part of an ISet.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="equipmentInstance"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the specified EquipmentInstance is not part of an <see cref="ISet"/>.
        /// </exception>
        internal EquipmentSetProperties( EquipmentInstance equipmentInstance )
        {
            if( equipmentInstance == null )
                throw new ArgumentNullException( "equipmentInstance" );

            var equipment = equipmentInstance.Equipment;            
            if( equipment.Set == null )
                throw new ArgumentException( "The Equipment is not part of an ISet.", "equipmentInstance" );

            this.set = equipment.Set;
            this.equipmentInstance = equipmentInstance;

            this.Hook();
        }

        /// <summary>
        /// Hooks up with the event handlers.
        /// </summary>
        private void Hook()
        {
            this.equipmentInstance.Equipped += this.OnEquipped;
            this.equipmentInstance.Unequipped += this.OnUnequipped;
        }

        /// <summary>
        /// Called when the EquipmentInstance has been equipped.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="wearer">
        /// The ExtendedStatable component that has equipped the EquipmentInstance.
        /// </param>
        private void OnEquipped( object sender, Zelda.Status.ExtendedStatable wearer )
        {
            if( this.HasEquippedAllRequiredItems( wearer ) )
            {
                this.ApplySetBonus( wearer );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified ExtendedStatable
        /// has currently equipped all required items.
        /// </summary>
        /// <param name="wearer">
        /// The wearer of the ISet.
        /// </param>
        /// <returns>
        /// true if the wearer has equipped all item of the ISet;
        /// otherwise false.
        /// </returns>
        private bool HasEquippedAllRequiredItems( Zelda.Status.ExtendedStatable wearer )
        {
            foreach( var item in this.set.Items )
            {
                if( !HasItem( item, wearer ) )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the specified <paramref name="wearer"/>
        /// has the specified <see cref="ISetItem"/> equipped.
        /// </summary>
        /// <param name="item">
        /// The item that is part of an ISet.
        /// </param>
        /// <param name="wearer">
        /// The wearer of the ISet.
        /// </param>
        /// <returns>
        /// true if the wearer has the item equipped;
        /// otherwise false.
        /// </returns>
        private static bool HasItem( ISetItem item, Zelda.Status.ExtendedStatable wearer )
        {
            return wearer.Equipment.HasEquipped( item.Name );
        }

        /// <summary>
        /// Applies the set bonus to the specified ExtendedStatable.
        /// </summary>
        /// <param name="wearer">
        /// The wearer of the ISet.
        /// </param>
        private void ApplySetBonus( Zelda.Status.ExtendedStatable wearer )
        {
            if( !this.IsSetBonusApplied() )
            {
                this.set.Bonus.Enable( wearer );
            }
        }

        /// <summary>
        /// Called when the EquipmentInstance has been unequipped.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="wearer">
        /// The ExtendedStatable component that has unequipped the EquipmentInstance.
        /// </param>
        private void OnUnequipped( object sender, Zelda.Status.ExtendedStatable wearer )
        {
            if( this.IsSetBonusApplied() )
            {
                this.set.Bonus.Disable();
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the set bonus
        /// has been applied.
        /// </summary>
        /// <returns>
        /// true if it has been applied and is currently active;
        /// otherwise false.
        /// </returns>
        private bool IsSetBonusApplied()
        {
            return this.set.Bonus.IsApplied;
        }

        /// <summary>
        /// Identifies the ISet the EquipmentInstance is part of.
        /// </summary>
        private readonly ISet set;

        /// <summary>
        /// The EquipmentInstance whose set properties are descriped
        /// by this EquipmentSetProperties.
        /// </summary>
        private readonly EquipmentInstance equipmentInstance;
    }
}
