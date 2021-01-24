// <copyright file="IsUseableInSpecificScene.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Core.Useability.IsUseableInSpecificScene class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Useability
{
    using System;
    using System.ComponentModel;
    using Zelda.Saving;
    using Zelda.Core.Requirements;

    /// <summary>
    /// Defines an <see cref="IRequirement"/> that returns true if
    /// the player is currently in a specific scene.
    /// </summary>
    public sealed class IsUseableInSpecificScene : IRequirement
    {
        /// <summary>
        /// Gets or sets the name of the Scene the user must be
        /// part of for the IsUseable method to return true. 
        /// </summary>
        [Editor( typeof( Zelda.Design.SceneNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SceneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether something is currently useable.
        /// </summary>
        /// <param name="user">
        /// The object that wants to know whether it can use something.
        /// </param>
        /// <returns>
        /// true if it is useable;
        /// otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity user )
        {
            var scene = user.Scene;
            if( scene == null )
                return false;

            return scene.Name.Equals( this.SceneName, StringComparison.OrdinalIgnoreCase );
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
            context.WriteDefaultHeader();
            context.Write( this.SceneName ?? string.Empty );
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
            this.SceneName = context.ReadString();
        }
    }
}
