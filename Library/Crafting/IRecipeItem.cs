
namespace Zelda.Crafting
{
    using Zelda.Items;

    public interface IRecipeItem
    {
        int Amount { get; }
        Item Item { get; }
    }
}
