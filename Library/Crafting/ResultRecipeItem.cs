// <copyright file="ResultRecipeItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.ResultRecipeItem class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Crafting
{
    /// <summary>
    /// Represents the item that results from a <see cref="Recipe"/>.
    /// </summary>
    public sealed class ResultRecipeItem : RecipeItem
    {
        /// <summary>
        /// Gets the name that uniquely identifies the base item of the AffixedItem
        /// whose affixes should be applied to the resulting item.
        /// </summary>
        public string AppliesAffixesOf
        {
            get
            {
                return this.appliesAffixesOf;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultRecipeItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the Item.
        /// </param>
        /// <param name="amount">
        /// The number of items needed.
        /// </param>
        /// <param name="appliesAffixesOf">
        /// The name that uniquely identifies the base item of the AffixedItem
        /// whose affixes should be applied to the resulting item.
        /// </param>
        public ResultRecipeItem( string name, int amount, string appliesAffixesOf )
            : base( name, amount )
        {
            this.appliesAffixesOf = appliesAffixesOf;
        }

        /// <summary>
        /// Serializes an instance of the <see cref="ResultRecipeItem"/> class by reading from the specified IDeserializationContext.
        /// </summary>
        /// <param name="item">
        /// The item to serialize.
        /// </param>
        /// <param name="context">
        /// The ISerializationContext to write to.
        /// </param>
        internal static void Serialize( ResultRecipeItem item, Atom.Storage.ISerializationContext context )
        {
            context.Write( item.Name );
            context.Write( item.Amount );
            context.Write( item.AppliesAffixesOf ?? string.Empty );
        }

        /// <summary>
        /// Deserializes an instance of the <see cref="ResultRecipeItem"/> class by reading from the specified IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The IDeserializationContext to read from.
        /// </param>
        /// <returns>
        /// The newly created ResultRecipeItem.
        /// </returns>
        internal static ResultRecipeItem Deserialize( Atom.Storage.IDeserializationContext context )
        {
            string itemName         = context.ReadString();
            int    itemAmount       = context.ReadInt32();
            string appliesAffixesOf = context.ReadString();

            if( appliesAffixesOf.Length == 0 )
                appliesAffixesOf = null;

            return new ResultRecipeItem( itemName, itemAmount, appliesAffixesOf );
        }

        /// <summary>
        /// The name that uniquely identifies the base item of the AffixedItem
        /// whose affixes should be applied to the resulting item.
        /// </summary>
        private readonly string appliesAffixesOf;
    }
}
