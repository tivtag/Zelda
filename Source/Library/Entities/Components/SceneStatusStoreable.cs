
namespace Zelda.Entities.Components
{
    using System.ComponentModel;

    /// <summary>
    /// Adds the functionallity of storing the entity within the dynamic scene status storage;
    /// instead of the static map file.
    /// </summary>
    public sealed class SceneStatusStoreable : ZeldaComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is currently stored by the scene status.
        /// </summary>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
        public bool Stored
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );

            context.Write( this.Stored );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, Version, this.GetType() );

            this.Stored = context.ReadBoolean();
        }
    }
}
