// <copyright file="AffixedItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.AffixedItem class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes
{
    using System;
    using System.Text;

    /// <summary>
    /// An <see cref="AffixedItem"/> is an item that is composed 
    /// by compining a base <see cref="Item"/>, an <see cref="IPrefix"/> and an <see cref="ISuffix"/>.
    /// </summary>
    /// <remarks>
    /// Affixed items allows the game to generate many-different 'randomized' items.
    /// <example>
    /// <para>
    /// 'Brutal Demonsword of the Kings'
    /// </para>
    /// <para>
    /// 'Brutal' is the prefix, adding +Strength (based on item-level).
    /// 'Demonsword' is the base item.
    /// 'of the Kings' is the suffix, adding +All Stats%.
    /// </para>
    /// </example>
    /// </remarks>
    public sealed class AffixedItem
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the Item that resulted by combining 
        /// the <see cref="Prefix"/>, <see cref="BaseItem"/> and <see cref="Suffix"/>.
        /// </summary>
        public Item ComposedItem
        {
            get { return this.item; }
        }

        /// <summary>
        /// Gets the <see cref="IAffix"/> the pre-fixes the <see cref="BaseItem"/>, if any.
        /// </summary>
        public IPrefix Prefix
        {
            get { return this.prefix; }
        }

        /// <summary>
        /// Gets the Item this AffixedItem is based on.
        /// </summary>
        public Item BaseItem
        {
            get { return this.baseItem; }
        }

        /// <summary>
        /// Gets the <see cref="IAffix"/> the post-fixes the <see cref="BaseItem"/>, if any.
        /// </summary>
        public ISuffix Suffix
        {
            get { return this.suffix; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the AffixedItem class.
        /// </summary>
        /// <param name="baseItem">
        /// The item the new AffixedItem is based on.
        /// </param>
        /// <param name="prefix">
        /// The IPrefix to apply to the given <paramref name="baseItem"/>.
        /// </param>
        /// <param name="suffix">
        /// The ISuffix to apply to the given <paramref name="baseItem"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseItem"/> is null.
        /// </exception>
        public AffixedItem( Item baseItem, IPrefix prefix, ISuffix suffix )
        {
            if( baseItem == null )
                throw new ArgumentNullException( "baseItem" );

            this.prefix = prefix;
            this.suffix = suffix;
            this.baseItem = baseItem;

            this.Compose();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates an instance of this AffixedItem.
        /// </summary>
        /// <param name="rand">
        /// The random number generator used to decide the power factor of the new instance.
        /// </param>
        /// <returns>
        /// A newly created instance of this AffixedItem.
        /// </returns>
        public ItemInstance CreateInstance( Atom.Math.RandMT rand )
        {
            float powerFactor = this.ComposedItem.GetPowerFactor( rand );
            return this.CreateInstance( powerFactor );
        }

        /// <summary>
        /// Creates an instance of this AffixedItem.
        /// </summary>
        /// <param name="powerFactor">
        /// The power facto of the new instance.
        /// </param>
        /// <returns>
        /// A newly created instance of this AffixedItem.
        /// </returns>
        public ItemInstance CreateInstance( float powerFactor )
        {
            switch( this.item.ItemType )
            {
                case ItemType.Equipment:
                    return new AffixedEquipmentInstance( this, powerFactor );

                case ItemType.Weapon:
                    return new AffixedWeaponInstance( this, powerFactor );

                default:
                    throw new NotSupportedException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Resources.Error_CreatingInstanceOfAffixedItemBaseTypeXNotSupportedOrImplemented,
                            this.item.ItemType.ToString()
                        )
                   );
            }
        }

        /// <summary>
        /// Composes the <see cref="Prefix"/>, <see cref="BaseItem"/> and <see cref="Suffix"/>
        /// to create the actual affixed <see cref="ComposedItem"/>.
        /// </summary>
        private void Compose()
        {
            this.PrepareCompose();

            var nameBuilder = new StringBuilder();
            var localizedNameBuilder = new StringBuilder();
            nameBuilder.Append( this.baseItem.Name );

            // Apply Prefix.
            if( this.prefix != null )
            {
                if( baseItem.AllowedAffixes.AllowsPrefix() && prefix.IsApplyable( baseItem ) )
                {
                    localizedNameBuilder.Append( this.prefix.LocalizedName );
                    localizedNameBuilder.Append( ' ' );

                    nameBuilder.Append( '_' );
                    nameBuilder.Append( this.prefix.GetType().Name );

                    this.prefix.Apply( this.item, this.baseItem );
                }
            }

            localizedNameBuilder.Append( this.baseItem.LocalizedName );

            // Apply Suffix.
            if( this.suffix != null )
            {
                if( baseItem.AllowedAffixes.AllowsSuffix() && suffix.IsApplyable( baseItem ) )
                {
                    localizedNameBuilder.Append( ' ' );
                    localizedNameBuilder.Append( this.suffix.LocalizedName );

                    nameBuilder.Append( '_' );
                    nameBuilder.Append( this.suffix.GetType().Name );

                    this.suffix.Apply( this.item, this.baseItem );
                }
            }

            // Apply Name.
            this.item.Name = nameBuilder.ToString();
            this.item.LocalizedName = localizedNameBuilder.ToString();

            this.CompleteCompose();
        }

        /// <summary>
        /// Prepares the <see cref="item"/> before composing it with the <see cref="prefix"/> and <see cref="suffix"/>.
        /// </summary>
        private void PrepareCompose()
        {
            this.item = this.baseItem.Clone();

            var equipment = this.item as Equipment;

            if( equipment != null )
            {
                if( equipment.AdditionalEffectsAura == null )
                {
                    equipment.AdditionalEffectsAura = new Zelda.Status.PermanentAura();
                }
            }
        }

        /// <summary>
        /// Completes the <see cref="item"/> after composing it with the <see cref="prefix"/> and <see cref="suffix"/>.
        /// </summary>
        private void CompleteCompose()
        {
            var equipment = this.item as Equipment;

            if( equipment != null )
            {
                equipment.AdditionalEffectsAura.SortEffects();
            }
        }

        #region > Storage <

        /// <summary>
        /// Serializes the given AffixedItem for storage in binary format.
        /// </summary>
        /// <param name="affixedItem">
        /// The AffixedItem to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal static void Serialize( AffixedItem affixedItem, Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( affixedItem.baseItem.Name );
            context.Write( AffixDatabase.GetAffixName( affixedItem.prefix ) );
            context.Write( AffixDatabase.GetAffixName( affixedItem.suffix ) );
        }

        /// <summary>
        /// Creates a new AffixedItem by deserializing the required informating.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The newly deserialized AffixedItem.
        /// </returns>
        internal static AffixedItem Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, "AffixedItem" );

            string itemName = context.ReadString();
            string prefixName = context.ReadString();
            string suffixName = context.ReadString();

            var itemManager = context.ServiceProvider.ItemManager;
            Item baseItem = itemManager.Get( itemName );

            IPrefix prefix = null;
            if( prefixName.Length > 0 )
            {
                prefix = AffixDatabase.Instance.GetPrefix( prefixName );
            }

            ISuffix suffix = null;
            if( suffixName.Length > 0 )
            {
                suffix = AffixDatabase.Instance.GetSuffix( suffixName );
            }

            return new AffixedItem( baseItem, prefix, suffix );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The <see cref="IPrefix"/> that is attached to the BaseItem.
        /// </summary>
        private readonly IPrefix prefix;

        /// <summary>
        /// The item this AffixedItem adds to.
        /// </summary>
        private readonly Item baseItem;

        /// <summary>
        /// The <see cref="ISuffix"/> that is attached to the BaseItem.
        /// </summary>
        private readonly ISuffix suffix;

        /// <summary>
        /// The final Item that is created by this AffixedItem,
        /// by adding the prefix and the suffix to the baseItem.
        /// </summary>
        private Item item;

        #endregion
    }
}
