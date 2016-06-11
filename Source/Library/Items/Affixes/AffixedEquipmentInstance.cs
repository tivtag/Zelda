
namespace Zelda.Items.Affixes
{
    /// <summary>
    /// Represents an instance of an Equipment that has been 'enhanced' using affixes.
    /// </summary>
    internal class AffixedEquipmentInstance : EquipmentInstance, IAffixedItemInstance
    {
        /// <summary>
        /// Gets the <see cref="AffixedItem"/> this AffixedEquipmentInstance is based on.
        /// </summary>
        public AffixedItem AffixedItem
        {
            get { return this.affixedItem; }
        }

        /// <summary>
        /// Initializes a new instance of the AffixedEquipmentInstance class.
        /// </summary>
        /// <param name="affixedItem">
        /// The underlying AffixedItem.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of this AffixedEquipmentInstance varies compared to the base Item.
        /// </param>
        internal AffixedEquipmentInstance( AffixedItem affixedItem, float powerFactor )
            : base( (Equipment)affixedItem.ComposedItem, powerFactor )
        {
            this.affixedItem = affixedItem;
        }

        /// <summary>
        /// Serializes/Writes the data descriping this AffixedEquipmentInstance using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Header
            context.Write( (int)ItemType.AffixedEquipment );

            const int Version = 2;
            context.Write( Version );

            // Write affixed data.
            AffixedItem.Serialize( affixedItem, context );

            // Write normal equipment data.
            context.Write( this.PowerFactor ); // New in V2
            context.Write( this.Count );

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
        /// Deserializes the data required to create an <see cref="EquipmentInstance"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new EquipmentInstance object.
        /// </returns>
        internal static EquipmentInstance ReadAffixedEquipment( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.Affixes.AffixedEquipmentInstance" );

            // Read affixed data.
            var affixedItem = AffixedItem.Deserialize( context );

            float powerFactor;

            if( version >= 2 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            // Read normal equipment data.
            var instance = (EquipmentInstance)affixedItem.CreateInstance( powerFactor );
            instance.Count = context.ReadInt32();

            bool isGemmed = context.ReadBoolean();
            if( isGemmed )
            {
                ReadGemInfo( instance, context );
            }

            bool isEnchanted = context.ReadBoolean();
            if( isEnchanted )
            {
                throw new System.NotImplementedException( "Enchanting is not implemented." );
            }

            return instance;
        }

        /// <summary>
        /// The underlying AffixedItem.
        /// </summary>
        private readonly AffixedItem affixedItem;
    }
}
