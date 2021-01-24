// <copyright file="NeededRecipeItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.NeededRecipeItem class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Crafting
{
    using Zelda.Saving;

    /// <summary>
    /// Represents an item that is required for an <see cref="Recipe"/> to work.
    /// </summary>
    public sealed class RequiredRecipeItem : RecipeItem
    {
        /// <summary>
        /// Gets a value indicating whether AffixedItems are allowed.
        /// </summary>
        public bool AllowsAffixed
        {
            get
            {
                return this.allowsAffixed;
            }
        }

        /// <summary>
        /// Gets the predicate object that decides which actual Item matches this RequiredRecipeItem.
        /// </summary>
        public IRecipeItemMatcher Matcher
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRecipeItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the Item.
        /// </param>
        /// <param name="amount">
        /// The number of items needed.
        /// </param>
        /// <param name="allowsAffixed">
        ///  States whether affixed items are allowed.
        /// </param>
        public RequiredRecipeItem( string name, int amount, bool allowsAffixed, IRecipeItemMatcher matcher )
            : base( name, amount )
        {
            this.allowsAffixed = allowsAffixed;
            this.Matcher = matcher ?? NameItemMatcher.Instance;
        }

        /// <summary>
        /// Serializes an instance of the <see cref="RequiredRecipeItem"/> class by reading from the specified IDeserializationContext.
        /// </summary>
        /// <param name="item">
        /// The item to serialize.
        /// </param>
        /// <param name="context">
        /// The ISerializationContext to write to.
        /// </param>
        internal static void Serialize( RequiredRecipeItem item, IZeldaSerializationContext context )
        {
            context.Write( item.Name );
            context.Write( item.Amount );
            context.Write( item.AllowsAffixed );
            context.WriteObject( item.Matcher );
        }
        
        /// <summary>
        /// Deserializes an instance of the <see cref="RequiredRecipeItem"/> class by reading from the specified IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The IDeserializationContext to read from.
        /// </param>
        /// <returns>
        /// The newly created NeededRecipeItem.
        /// </returns>
        internal static RequiredRecipeItem Deserialize( IZeldaDeserializationContext context )
        {
            string itemName      = context.ReadString();
            int    itemAmount    = context.ReadInt32();
            bool   allowsAffixes = context.ReadBoolean();
            var matcher          = context.ReadObject<IRecipeItemMatcher>();

            return new RequiredRecipeItem( itemName, itemAmount, allowsAffixes, matcher );
        }

        /// <summary>
        /// States whether affixed items are allowed.
        /// </summary>
        private readonly bool allowsAffixed;
    }
}
