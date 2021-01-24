// <copyright file="EquipmentInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.EquipmentInstance class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Zelda.Status;
    
    /// <summary>
    /// Represents an ItemInstance that uses a <see cref="Equipment"/> as its template.
    /// </summary>
    public class EquipmentInstance : ItemInstance
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this EquipmentInstance has been equipped.
        /// </summary>
        public event RelaxedEventHandler<ExtendedStatable> Equipped;

        /// <summary>
        /// Raised when this EquipmentInstance has been unequipped.
        /// </summary>
        public event RelaxedEventHandler<ExtendedStatable> Unequipped;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Equipment"/> that is encapsulated by this <see cref="EquipmentInstance"/>.
        /// </summary>
        public Equipment Equipment
        {
            get
            {
                return this.modifiedEquipment; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Equipment"/> template that is encapsulated by this <see cref="EquipmentInstance"/>.
        /// </summary>
        public Equipment BaseEquipment
        {
            get
            {
                return this.baseEquipment;
            }
        }

        /// <summary>
        /// Gets the ItemSocketProperties of this EquipmentInstance.
        /// </summary>
        public ItemSocketProperties SocketProperties
        {
            get 
            { 
                return this.socketProperties;
            }
        }

        /// <summary>
        /// Gets the EquipmentSetProperties of this EquipmentInstance.
        /// </summary>
        public Zelda.Items.Sets.EquipmentSetProperties SetProperties
        {
            get
            {
                return this.setProperties;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EquipmentInstance"/>
        /// has any gems in its sockets.
        /// </summary>
        public bool IsGemmed
        {
            get
            {
                return this.socketProperties.HasGems;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EquipmentInstance"/>
        /// has any enchants applied to it.
        /// </summary>
        public bool IsEnchanted
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EquipmentInstance"/> is currently equipped.
        /// </summary>
        public bool IsEquipped
        {
            get 
            { 
                return this.wearer != null; 
            }
        }

        /// <summary>
        /// Gets the object that wears this <see cref="EquipmentInstance"/>, if any.
        /// </summary>
        public ExtendedStatable Wearer
        {
            get
            { 
                return this.wearer; 
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentInstance"/> class.
        /// </summary>
        /// <param name="baseEquipment">
        /// The underlying <see cref="Equipment"/>.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of this EquipmentInstance varies compared to the base Item.
        /// </param>
        internal EquipmentInstance( Equipment baseEquipment, float powerFactor = 1.0f )
            : base( baseEquipment, powerFactor )
        {
            this.baseEquipment = baseEquipment;
            this.modifiedEquipment = (Equipment)base.Item;

            if( baseEquipment.Set != null )
            {
                this.setProperties = new Zelda.Items.Sets.EquipmentSetProperties( this );
            }

            this.socketProperties = baseEquipment.SocketProperties.Clone();
        }

        #endregion

        #region [ Methods ]

        #region - Equip -

        /// <summary>
        /// Called when this EquipmentInstance gets equiped.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that is about to equip this <see cref="EquipmentInstance"/>.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// If this <see cref="EquipmentInstance"/> has already been equipped.
        /// </exception>
        internal virtual void OnEquip( ExtendedStatable statable )
        {
            Debug.Assert( statable != null );

            if( this.IsEquipped )
            {
                throw new System.InvalidOperationException(
                    string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_EquipmentXIsAlreadyEquippedByY,
                        this.Equipment.Name,
                        this.wearer.Owner.Name
                    )
                );
            }

            if( !this.baseEquipment.FulfillsRequirements( statable ) )
                return;

            this.wearer = statable;

            if( this.modifiedEquipment.AdditionalEffects != null )
            {
                statable.AuraList.Add( this.modifiedEquipment.AdditionalEffectsAura );
            }

            this.socketProperties.OnEquip( statable );
            statable.Equipment.RefreshStatsFromItems();

            this.Equipped.Raise( this, statable );
        }

        /// <summary>
        /// Called when this EquipmentInstance gets equiped.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that is about to equip this <see cref="EquipmentInstance"/>.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// If this <see cref="EquipmentInstance"/> has already been equipped.
        /// </exception>
        internal virtual void OnEquipNoCheck( ExtendedStatable statable )
        {
            Debug.Assert( statable != null );

            if( this.IsEquipped )
            {
                throw new System.InvalidOperationException(
                    string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_EquipmentXIsAlreadyEquippedByY,
                        this.Equipment.Name,
                        wearer.Owner.Name
                    )
                );
            }

            this.wearer = statable;

            if( this.modifiedEquipment.AdditionalEffectsAura != null )
                wearer.AuraList.Add( this.modifiedEquipment.AdditionalEffectsAura );

            this.socketProperties.OnEquip( statable );
            this.Equipped.Raise( this, statable );
        }

        #endregion

        #region - Unequip -

        /// <summary>
        /// Called when this <see cref="EquipmentInstance"/> gets dequiped.
        /// </summary>
        public virtual void OnUnequip()
        {
            if( !this.IsEquipped )
                return;

            if( this.modifiedEquipment.AdditionalEffectsAura != null )
                this.wearer.AuraList.Remove( this.modifiedEquipment.AdditionalEffectsAura );

            this.socketProperties.OnUnequip();

            this.wearer.Equipment.NotifyRequirementsRecheckNeeded();

            var oldWearer = this.wearer;
            this.wearer = null;
            this.Unequipped.Raise( this, oldWearer );
        }

        /// <summary>
        /// Called when this EquipmentInstance gets unequipped.
        /// </summary>
        public virtual void OnUnequipNoCheck()
        {
            if( !this.IsEquipped )
                return;

            if( this.modifiedEquipment.AdditionalEffectsAura != null )
                this.wearer.AuraList.Remove( this.modifiedEquipment.AdditionalEffectsAura );

            this.socketProperties.OnUnequip();

            var oldWearer = this.wearer;
            this.wearer = null;
            this.Unequipped.Raise( this, oldWearer );
        }

        #endregion

        #region > Requirments <

        /// <summary>
        /// Gets whether the given <see cref="ExtendedStatable"/> fulfills all requirements 
        /// to wear this <see cref="EquipmentInstance"/>.
        /// </summary>
        /// <param name="statable">
        /// The <see cref="ExtendedStatable"/> component of the ZeldaEntity
        /// that wishes to wear an instance of this <see cref="Equipment"/>.
        /// </param>
        /// <returns>
        /// true if the ExtendedStatable would be able to wear an instance of this <see cref="Equipment"/>;
        /// otherwise false.
        /// </returns>
        public bool FulfillsRequirements( ExtendedStatable statable )
        {
            return this.baseEquipment.FulfillsRequirements( statable );
        }

        #endregion

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
            context.Write( (int)ItemType.Equipment );

            const int Version = 2;
            context.Write( Version );

            context.Write( this.baseEquipment.Name );
            context.Write( this.Count );
            context.Write( this.PowerFactor ); // New in V2

            if( this.IsGemmed )
            {
                context.Write( true );
                this.SerializeGemInfo( context );
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
        /// Serializes/Writes the gem data of this <see cref="EquipmentInstance"/>
        /// using the given <see cref="System.IO.BinaryWriter"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal protected void SerializeGemInfo( Zelda.Saving.IZeldaSerializationContext context )
        {
            this.socketProperties.SerializeState( context );
        }

        /// <summary>
        /// Deserializes the data required to create a <see cref="EquipmentInstance"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new EquipmentInstance object.
        /// </returns>
        internal static EquipmentInstance ReadEquipment( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.EquipmentInstance" );

            // typename has been readen at this point.
            string equipmentName = context.ReadString();
            int    count         = context.ReadInt32();
            
            var itemManager = context.ServiceProvider.ItemManager;
            var equipment = (Equipment)itemManager.Get( equipmentName );
            if( equipment == null )
                return null;
            
            float powerFactor;

            if( version >= 2 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            var instance = new EquipmentInstance( equipment, powerFactor ) {
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

        /// <summary>
        /// Reads the gem data of the given <see cref="EquipmentInstance"/>.
        /// </summary>
        /// <param name="instance">
        /// The related EquipmentInstance object.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal protected static void ReadGemInfo( EquipmentInstance instance, Zelda.Saving.IZeldaDeserializationContext context )
        {
            instance.socketProperties.DeserializeState( context );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The object that has this <see cref="EquipmentInstance"/> equipped, if any yet.
        /// </summary>
        private ExtendedStatable wearer;
        
        /// <summary>
        /// The modified Equipment that is encapsulated by the <see cref="EquipmentInstance"/>.
        /// </summary>
        private readonly Equipment modifiedEquipment;

        /// <summary>
        /// The Equipment template that is encapsulated by the <see cref="EquipmentInstance"/>.
        /// </summary>
        private readonly Equipment baseEquipment;
        
        /// <summary>
        /// A clone of the socket propertiess of the <see cref="Equipment"/>.
        /// </summary>
        private readonly ItemSocketProperties socketProperties;

        /// <summary>
        /// The set properties of this EquipmentInstance.
        /// </summary>
        private readonly Zelda.Items.Sets.EquipmentSetProperties setProperties;

        #endregion
    }
}