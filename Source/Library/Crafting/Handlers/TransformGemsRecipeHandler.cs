
namespace Zelda.Crafting
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom.Collections;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Items;
    using Zelda.Saving;
    using Zelda.Status;

    /// <summary>
    /// Given 3x gems of the same color produces a gem of the same color within +- 5 item-level of the average
    /// input gem level.
    /// </summary>
    public sealed class TransformGemsRecipeHandler : IRecipeHandler, IZeldaSetupable
    {
        private const int RequiredGemCount = 3;
        private static readonly IntegerRange GemLevelRange = new IntegerRange( 9, 12 );

        public ElementalSchool GemColor
        {
            get;
            set;
        }

        public Recipe Recipe
        {
            get;
            set;
        }

        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
            this.itemManager = serviceProvider.ItemManager;
        }

        public bool HasRequired( Inventory inventory )
        {
            return inventory.ContainedItems.AtLeast( RequiredGemCount, this.Predicate );
        }

        private bool Predicate( Item item )
        {
            var gem = item as Gem;
            return gem != null && gem.GemColor == this.GemColor;
        }

        public IEnumerable<ItemInstance> RemoveRequired( Inventory inventory )
        {
            var items = inventory.ContainedItemInstances
                .Where( itemInstance => this.Predicate( itemInstance.Item ) )
                .Take( RequiredGemCount )
                .ToArray();

            inventory.RemoveAll( items );
            return items;
        }

        public ItemInstance CreateResult( IEnumerable<ItemInstance> usedItems, PlayerEntity player )
        {
            int averageItemLevel = (int)usedItems.Select( x => x.Item.Level )
                .Average();

            IntegerRange itemLevelRange = new IntegerRange( 
                averageItemLevel - GemLevelRange.Minimum,
                averageItemLevel + GemLevelRange.Maximum 
            );

            IList<Gem> gems = this.itemManager.GetGems()
                .Where( g => IsValidGem( g, itemLevelRange, player ) )
                .ToArray();

            Gem gem = gems.RandomOrDefault( rand );

            if( gem != null )
            {
                return gem.CreateInstance( rand );
            }
            else
            {
                return null;
            }
        }

        private bool IsValidGem( Gem gem, IntegerRange itemLevelRange, PlayerEntity player )
        {
            if( gem.DropRequirement != null )
            {
                if( !gem.DropRequirement.IsFulfilledBy( player ) )
                {
                    return false;
                }
            }

            return gem.GemColor == this.GemColor && IsWithinAverageItemLevel( gem, itemLevelRange );
        }

        /// <summary>
        /// Gets a value indicating whether the gem is within the required item level range.
        /// </summary>
        /// <example>
        /// avg = 13
        /// gem = 40
        /// 4 .. 25
        /// -> False
        /// 
        /// avg = 13
        /// gem = 20
        /// 4 .. 25
        /// -> True
        /// </example>
        /// <param name="gem">
        /// The gem to check.
        /// </param>
        /// <returns>
        /// true when the gem is within the required range;
        /// -or- otherwise false.
        /// </returns>
        private static bool IsWithinAverageItemLevel( Gem gem, IntegerRange itemLevelRange )
        {
            bool result = itemLevelRange.Minimum <= gem.Level && gem.Level <= itemLevelRange.Maximum;
            return result;
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( (byte)this.GemColor );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.GemColor = (ElementalSchool)context.ReadByte();
        }

        private ItemManager itemManager;
        private IRand rand;
    }
}
