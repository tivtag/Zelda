// <copyright file="WeaponInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.WeaponInstance class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// Represents an <see cref="ItemInstance"/> that stores <see cref="Weapon"/>s.
    /// </summary>
    public class WeaponInstance : EquipmentInstance
    {
        #region [ Properties ]
        
        /// <summary>
        /// Gets the <see cref="Weapon"/> that is encapsulated by this <see cref="WeaponInstance"/>.
        /// </summary>
        public Weapon Weapon
        {
            get 
            {
                return (Weapon)base.Item; 
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponInstance"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseWeapon"/> is null.
        /// </exception>
        /// <param name="baseWeapon">
        /// The underlying <see cref="Weapon"/>.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of this EquipmentInstance varies compared to the base Item.
        /// </param>
        internal WeaponInstance( Weapon baseWeapon, float powerFactor )
            : base( baseWeapon, powerFactor )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this WeaponInstance gets equiped.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that is about to equip this <see cref="WeaponInstance"/>.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// If this <see cref="WeaponInstance"/> has already been equipped.
        /// </exception>
        internal override void OnEquip( ExtendedStatable statable )
        {
            base.OnEquip( statable );
            this.RefreshAttackSpeedOfWearer();
        }

        /// <summary>
        /// Called when this <see cref="WeaponInstance"/> gets unquiped.
        /// </summary>
        public override void OnUnequip()
        {
            this.RefreshAttackSpeedOfWearer();
            base.OnUnequip();
        }

        /// <summary>
        /// Called when this WeaponInstance has been equiped or is going to be unequipped.
        /// </summary>
        private void RefreshAttackSpeedOfWearer()
        {
            if( this.IsEquipped )
            {
                if( this.Weapon.AttackType == WeaponAttackType.Melee )
                {
                    this.Wearer.Refresh_MeleeWeaponRelated();
                }
                else
                {
                    this.Wearer.Refresh_AttackSpeedRanged();
                }
            }
        }

        /// <summary>
        /// Called when this <see cref="WeaponInstance"/> gets unquiped.
        /// </summary>
        public override void OnUnequipNoCheck()
        {
            base.OnUnequipNoCheck();
        }

        #region > Storage <
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( (int)ItemType.Weapon );

            const int Version = 2;
            context.Write( Version );

            context.Write( this.BaseItem.Name );
            context.Write( this.Count );
            context.Write( this.PowerFactor ); // New in V2.

            if( this.IsGemmed )
            {
                context.Write( true );
                SerializeGemInfo( context );
            }
            else
            {
                context.Write( false );
            }

            if( this.IsEnchanted )
            {
                context.Write( true );
            }
            else
            {
                context.Write( false );
            }
        }

        /// <summary>
        /// Deserializes the data required to create an <see cref="WeaponInstance"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new WeaponInstance object.
        /// </returns>
        internal static WeaponInstance ReadWeapon( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.WeaponInstance" );

            // typename has been readen at this point.
            string equipmentName = context.ReadString();
            int    count         = context.ReadInt32();
            float powerFactor;

            if( version >= 2 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            var itemManager = context.ServiceProvider.ItemManager;
            var weapon   = (Weapon)itemManager.Get( equipmentName );
            var instance = new WeaponInstance( weapon, powerFactor ) {
                Count = count
            };

            bool isGemmed = context.ReadBoolean();
            if( isGemmed )
            {
                ReadGemInfo( instance, context );
            }

            bool isEnchanted = context.ReadBoolean();
            if( isEnchanted )
            {
                throw new NotImplementedException( "Enchanting is not implemented." );
            }

            return instance;
        }

        #endregion

        #endregion
    }
}
