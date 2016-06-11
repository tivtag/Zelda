// <copyright file="DungeonProperty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.Scene.DungeonProperty class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties.Scene
{
    using Zelda.Saving;

    /// <summary>
    /// Defines an <see cref="IProperty"/> that states that the
    /// current scene is considered to be a dungeon area.
    /// </summary>
    public sealed class DungeonProperty : IProperty
    {
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
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
            context.ReadDefaultHeader( this.GetType() );
        }
    }
}
