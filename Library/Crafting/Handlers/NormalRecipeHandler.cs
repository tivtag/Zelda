
namespace Zelda.Crafting
{
    using System.Collections.Generic;
    using Zelda.Items;
    using System.Linq;
    using Zelda.Items.Affixes;
    using Zelda.Saving;

    public sealed class NormalRecipeHandler : IRecipeHandler, IZeldaSetupable
    {
        public Recipe Recipe
        {
            get;
            set;
        }

        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        public bool HasRequired( Inventory inventory )
        {
            int actualFound;
            return this.Recipe.Required.All( recipeItem => inventory.Contains( inventoryItem => recipeItem.Matcher.Matches( inventoryItem, recipeItem ), recipeItem.Amount, recipeItem.AllowsAffixed, out actualFound ) );
        }

        public IEnumerable<ItemInstance> RemoveRequired( Inventory inventory )
        {
            var removedItems = new List<ItemInstance>( this.Recipe.RequiredCount );

            foreach( var requiredItem in this.Recipe.Required )
            {
                inventory.Remove( inventoryItem => requiredItem.Matcher.Matches( inventoryItem, requiredItem ), requiredItem.Amount, requiredItem.AllowsAffixed, removedItems );
            }

            return removedItems;
        }

        public ItemInstance CreateResult( IEnumerable<ItemInstance> usedItems, Zelda.Entities.PlayerEntity player )
        {
            var recipeItem = this.Recipe.Result;
            Item item = recipeItem.Item;
            System.Diagnostics.Debug.Assert( item != null );

            ItemInstance instance = null;

            if( !string.IsNullOrEmpty( recipeItem.AppliesAffixesOf ) )
            {
                var affixedItem = GetAffixedItem( recipeItem.AppliesAffixesOf, usedItems );

                if( affixedItem != null )
                {
                    var newAffixedItem = new AffixedItem( item, affixedItem.Prefix, affixedItem.Suffix );
                    instance = newAffixedItem.CreateInstance( this.rand );
                }
            }

            if( instance == null )
            {
                instance = ItemCreationHelper.Create( item, rand );
            }

            instance.Count = recipeItem.Amount;
            return instance;
        }

        /// <summary>
        /// Helper method that gets the AffixedItem whose BaseItem has the given name.
        /// </summary>
        /// <param name="baseName">
        /// The name of the base item of the AffixedItem to get.
        /// </param>
        /// <param name="items">
        /// The list of item to search.
        /// </param>
        /// <returns>
        /// The requested AffixedItem; or null.
        /// </returns>
        private static AffixedItem GetAffixedItem( string baseName, IEnumerable<ItemInstance> items )
        {
            foreach( var itemInstance in items )
            {
                var affixedInstance = itemInstance as IAffixedItemInstance;

                if( affixedInstance != null )
                {
                    var affixedItem = affixedInstance.AffixedItem;

                    if( affixedItem.BaseItem.Name.Equals( baseName, System.StringComparison.Ordinal ) )
                    {
                        return affixedItem;
                    }
                }
            }

            return null;
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
        }

        private Atom.Math.RandMT rand;
    }
}
