// <copyright file="AffixedWeaponInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.AffixedWeaponInstance class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes
{
    /// <summary>
    /// Represents an instance of an Weapon that has been 'enhanced' using affixes.
    /// </summary>
    internal class AffixedWeaponInstance : WeaponInstance, IAffixedItemInstance
    {
        /// <summary>
        /// Gets the <see cref="AffixedItem"/> this AffixedWeaponInstance is based on.
        /// </summary>
        public AffixedItem AffixedItem
        {
            get
            { 
                return this.affixedItem;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AffixedWeaponInstance class.
        /// </summary>
        /// <param name="affixedItem">
        /// The underlying AffixedItem.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of this AffixedWeaponInstance varies compared to the base Item.
        /// </param>
        internal AffixedWeaponInstance( AffixedItem affixedItem, float powerFactor )
            : base( (Weapon)affixedItem.ComposedItem, powerFactor )
        {
            this.affixedItem = affixedItem;
        }

        /// <summary>
        /// Serializes/Writes the data descriping this AffixedWeaponInstance using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Header
            context.Write( (int)ItemType.AffixedWeapon );

            const int Version = 2;
            context.Write( Version );

            // Write Affixed data.
            AffixedItem.Serialize( affixedItem, context );

            // Write normal Equipment data.
            context.Write( this.PowerFactor ); // New in V2
            context.Write( this.Count );

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
        /// Deserializes the data required to create an affixed <see cref="WeaponInstance"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new WeaponInstance object.
        /// </returns>
        internal static WeaponInstance ReadAffixedWeapon( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.Affixes.AffixedWeaponInstance" );

            // Read affixed data.
            var weapon = AffixedItem.Deserialize( context );
            
            float powerFactor;

            if( version >= 2 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            // Read normal weapon data.
            var instance = (WeaponInstance)weapon.CreateInstance( powerFactor );
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
