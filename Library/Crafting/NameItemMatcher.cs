
namespace Zelda.Crafting
{
    public sealed class NameItemMatcher : IRecipeItemMatcher
    {
        public static readonly NameItemMatcher Instance = new NameItemMatcher();

        public bool Matches( Items.Item inventoryItem, RequiredRecipeItem recipeItem )
        {
            return inventoryItem.Name.Equals( recipeItem.Name, System.StringComparison.Ordinal );
        }

        public void Serialize( Saving.IZeldaSerializationContext context )
        {
        }

        public void Deserialize( Saving.IZeldaDeserializationContext context )
        {
        }
    }
}
