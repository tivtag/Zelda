
namespace Zelda.Status
{
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Saving;

    /// <summary>
    /// Represents an effect that changes the color of the fairy.
    /// </summary>
    public sealed class ColorEffect : StatusValueEffect
    {
        public enum ColorTarget
        {
            Fairy,
            Player
        }

        /// <summary>
        /// The string that uniquely identifies this ColorEffect.
        /// </summary>
        public const string IdentifierString = "Color";

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="LightRadiusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return IdentifierString;
            }
        }

        public ColorTarget Target
        {
            get;
            set;
        }

        public Vector4 Color
        {
            get;
            set;
        }

        public override string GetDescription( Statable statable )
        {
            return string.Format( "Modifies {0} color.", Target.ToString() );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            // Write Data:
            context.WriteDefaultHeader();
            context.Write( (byte)this.Target );
            context.Write( this.Color );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            // Read Data:
            context.ReadDefaultHeader( this.GetType() );
            this.Target = (ColorTarget)context.ReadByte();
            this.Color = context.ReadVector4();
        }

        /// <summary>
        /// Returns a clone of this ChanceToStatusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new ColorEffect() { Target = this.Target, Color = this.Color };
            this.SetupClone( clone );
            return clone;
        }

        public override void OnEnable( Statable user )
        {
            Refresh( user );
        }

        public override void OnDisable( Statable user )
        {
            Refresh( user );
        }

        public override void ModifyPowerBy( float factor )
        {
            Color *= factor;
            base.ModifyPowerBy( factor );
        }

        private void Refresh( Statable user )
        {
            PlayerEntity player = (PlayerEntity)user.Owner;
            Fairy fairy = player.Fairy;

            fairy.RefreshColor();
        }

        public override bool Equals( StatusEffect effect )
        {
            var colorEffect = effect as ColorEffect;
            if( colorEffect == null )
                return false;

            return colorEffect.Target == this.Target && colorEffect.Color == this.Color;
        }
    }
}
