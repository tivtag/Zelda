
namespace Zelda.Crafting
{
    using Zelda.Items;
    using Zelda.Saving;
    using Zelda.Status;

    public sealed class GemItemMatcher : IRecipeItemMatcher
    {
        public ElementalSchool GemColor
        {
            get { return gemColor; }
            set { gemColor = value; }
        }

        public bool Matches( Item inventoryItem, RequiredRecipeItem recipeItem )
        {
            var gem = inventoryItem as Gem;

            if( gem != null )
            {
                return gem.GemColor == gemColor;
            }

            return false;
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( (int)gemColor );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( typeof( GemItemMatcher ) );
            gemColor = (ElementalSchool)context.ReadInt32();
        }

        private ElementalSchool gemColor;
    }
}
