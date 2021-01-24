// <copyright file="StatusHook.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Status.Hooks.StatusHook class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Status.Hooks
{
    using Atom;

    /// <summary>
    /// Represents an abstract base implementation of the <see cref="IStatusHook"/> interface.
    /// </summary>
    public abstract class StatusHook : IStatusHook
    {
        /// <summary>
        /// Fired when this IStatusHook has been invoked.
        /// </summary>
        public event RelaxedEventHandler<Statable> Invoked;

        /// <summary>
        /// Raises the <see cref="Invoked"/> event.
        /// </summary>
        /// <param name="invoker">
        /// The statable component of the ZeldaEntity that has invoked
        /// this IStatusHook.
        /// </param>
        protected void OnInvoked( Statable invoker )
        {
            if( this.Invoked != null )
            {
                this.Invoked( this, invoker );
            }
        }
        
        /// <summary>
        /// Hooks this StatusHook up with the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        public abstract void Hook( Statable statable );

        /// <summary>
        /// Unhooks this StatusHook from the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to unhook from.
        /// </param>
        public abstract void Unhook( Statable statable );

        /// <summary>
        /// Gets a short description of this IStatusHook.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that hooks up with this IStatusHook.
        /// </param>
        /// <returns>
        /// A short string that is descriping with what this IStatusHook is hooking up.
        /// </returns>
        public abstract string GetShortDescription( Statable statable );
        
        /// <summary>
        /// Returns a clone of this StatusHook.
        /// </summary>
        /// <returns>
        /// The cloned IStatusHook.
        /// </returns>
        public abstract IStatusHook Clone();
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void Serialize( Zelda.Saving.IZeldaSerializationContext context );

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public abstract void Deserialize( Zelda.Saving.IZeldaDeserializationContext context );
    }
}
