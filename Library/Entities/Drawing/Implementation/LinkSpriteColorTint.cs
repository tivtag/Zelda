
namespace Zelda.Entities.Drawing
{
    using Atom.Xna;
    using Zelda.Saving;
    using Xna = Microsoft.Xna.Framework;

    public struct LinkSpriteColorTint : ISaveable
    {
        public Xna.Color ClothMain;
        public Xna.Color ClothHighlight;
        public Xna.Color HairMain;
        public Xna.Color HairHighlight;

        public static LinkSpriteColorTint Default
        {
            get
            {
                return new LinkSpriteColorTint( LinkSprites.ColorDefaults.ClothMain, LinkSprites.ColorDefaults.ClothHighlight, LinkSprites.ColorDefaults.HairMain, LinkSprites.ColorDefaults.HairHighlight );
            }
        }

        public LinkSpriteColorTint( Xna.Color clothMain, Xna.Color clothHighlight, Xna.Color hairMain, Xna.Color hairHighlight )
        {
            this.ClothMain = clothMain;
            this.ClothHighlight = clothHighlight;
            this.HairMain = hairMain;
            this.HairHighlight = hairHighlight;
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.ClothMain );
            context.Write( this.ClothHighlight );
            context.Write( this.HairMain );
            context.Write( this.HairHighlight );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader<LinkSpriteColorTint>();

            this.ClothMain = context.ReadColor();
            this.ClothHighlight = context.ReadColor();
            this.HairMain = context.ReadColor();
            this.HairHighlight = context.ReadColor();
        }
    }

}
