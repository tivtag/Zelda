// <copyright file="IQuickAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuickActions.IQuickAction interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuickActions
{
    /// <summary>
    /// Represents an action that can be quickly executed.
    /// </summary>
    /// <remarks>
    /// IsActive, IsExecuteable and CooldownLeft all expose the similiar 
    /// concept of whether the IQuickAction is currently 'useable'.
    /// </remarks>
    public interface IQuickAction : IZeldaUpdateable
    {
        /// <summary>
        /// Gets a value indicating whether this IQuickAction is active;
        /// and as such executeable.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction is executeable.
        /// </summary>
        bool IsExecuteable { get; }

        /// <summary>
        /// Gets the time left (in seconds) until this IQuickAction can be executed
        /// again.
        /// </summary>
        float CooldownLeft { get; }

        /// <summary>
        /// Gets the time (in seconds) this IQuickAction can't be executed again after executing it.
        /// </summary>
        float CooldownTotal { get; }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction executeability is only
        /// limited by the cooldown.
        /// </summary>
        bool IsOnlyLimitedByCooldown { get; }

        /// <summary>
        /// Gets the symbol associated with this IQuickAction.
        /// </summary>
        Atom.Xna.ISprite Symbol { get; }

        /// <summary>
        /// Gets the Color the <see cref="Symbol"/> of this IQuickAction is tinted in.
        /// </summary>
        Microsoft.Xna.Framework.Color SymbolColor { get; }
        
        /// <summary>
        /// Executes this IQuickAction.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wants to use this IQuickAction.
        /// </param>
        /// <returns>
        /// Whether this IQuickAction was succesfully used.
        /// </returns>
        bool Execute( Zelda.Entities.PlayerEntity user );

        /// <summary>
        /// Serializes this IQuickAction using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Serialize( Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes this IQuickAction using the given BinaryReader.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity whose action is executed by this IQuickAction.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Deserialize( Zelda.Entities.PlayerEntity player, Zelda.Saving.IZeldaDeserializationContext context );
    }
}
