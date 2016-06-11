// <copyright file="RedirectZoneResetProperty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Properties.Scene.RedirectZoneResetProperty class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Core.Properties.Scene
{
    /// <summary>
    /// Defines an <see cref="IProperty"/> that states that when a Zone Reset
    /// request comes in on a scene that has this property will redirect the request
    /// to another zone/scene.
    /// </summary>
    [UniqueProperty]
    public sealed class RedirectZoneResetProperty : IProperty
    {
        /// <summary>
        /// Gets or sets the name of the scene the zone reset should be redirected to.
        /// </summary>
        public string ToScene
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current scene
        /// should be reset before redirecting the zone reset.
        /// </summary>
        public bool ResetCurrentBeforeRedirecting
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

            context.Write( this.ToScene ?? string.Empty );
            context.Write( this.ResetCurrentBeforeRedirecting );
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

            this.ToScene = context.ReadString();
            this.ResetCurrentBeforeRedirecting = context.ReadBoolean();
        }
    }
}
