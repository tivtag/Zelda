
namespace Zelda.Crafting
{
    using System.Collections.Generic;
    using Zelda.Entities;
    using Zelda.Items;
    using Zelda.Saving;

    public interface IRecipeHandler : ISaveable
    {
        Recipe Recipe
        {
            get;
            set;
        }

        bool HasRequired( Inventory inventory );
        IEnumerable<ItemInstance> RemoveRequired( Inventory inventory );
        ItemInstance CreateResult( IEnumerable<ItemInstance> usedItems, PlayerEntity player );
    }
}
