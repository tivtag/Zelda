// <copyright file="CombineColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.CombineColorTint class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting
{
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines an <see cref="IColorTint"/> that combines the existing
    /// color with a specified color.
    /// </summary>
    public class CombineColorTint : IColorTint
    {
        /// <summary>
        /// Gets or sets the <see cref="Color"/> associated with this ColorTint instance.
        /// </summary>
        public Vector4 Color
        {
            get;
            set;
        }

        /// <summary>
        /// Applies this IColorTint to the given color.
        /// </summary>
        /// <param name="color">
        /// The input color.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        public Vector4 Apply( Vector4 color )
        {
            return (this.Color + color) * 0.5f;
        }

        /// <summary>
        /// Updates this IZeldaUpdateable.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void IZeldaUpdateable.Update( ZeldaUpdateContext updateContext )
        {
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

            context.Write( this.Color );
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

            this.Color = context.ReadVector4();
        }
    }
}
