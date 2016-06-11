// <copyright file="AlsoResetAnotherZoneOnZoneResetProperty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.Scene.AlsoResetAnotherZoneOnZoneResetProperty class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties.Scene
{
    using System.ComponentModel;

    /// <summary>
    /// Defines an <see cref="IProperty"/> that states that another zone
    /// should also be reset when a zone reset request comes in.
    /// </summary>
    public sealed class AlsoResetAnotherZoneOnZoneResetProperty : IProperty
    {
        /// <summary>
        /// Gets or sets the name of the scene the zone reset also should reset.
        /// </summary>
        [Editor( typeof( Zelda.Design.SceneNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Scene
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
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Scene ?? string.Empty );
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Scene = context.ReadString();
        }
    }
}
