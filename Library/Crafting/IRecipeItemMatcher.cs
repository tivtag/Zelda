
namespace Zelda.Crafting
{
    public interface IRecipeItemMatcher : Zelda.Saving.ISaveable
    {
        bool Matches( Items.Item inventoryItem, RequiredRecipeItem recipeItem );
    }
}
