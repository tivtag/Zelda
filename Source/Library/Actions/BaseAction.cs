// <copyright file="BaseAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.BaseAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Actions
{
    using Atom;
    using Zelda.Saving;

    /// <summary>
    /// Represents an abstract base implementation of the IAction interface.
    /// </summary>
    public abstract class BaseAction : IAction
    {
        /// <summary>
        /// Executes this IAction.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Deferredly undoes this IAction.
        /// </summary>
        public abstract void Dexecute();

        /// <summary>
        /// Gets a value indicating whether this IAction can be executed.
        /// </summary>
        /// <returns>
        /// true if this IAction can be executed;
        /// otherwise false.
        /// </returns>
        public virtual bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Gets a localized description text of this IAction.
        /// </summary>
        /// <returns>
        /// The localized description of this IAction.
        /// </returns>
        public virtual string GetDescription()
        {
            return string.Empty;
        }
        
        /// <summary>
        /// Serializes this IStoreable object using the given ISerializationContext.
        /// </summary>
        /// <param name="context">
        /// Provides access to the mechanisms required to serialize this IStoreable object.
        /// </param>
        public virtual void Serialize( Atom.Storage.ISerializationContext context )
        {
            context.WriteDefaultHeader();
        }

        /// <summary>
        /// Deserializes this IStoreable object using the given IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// Provides access to the mechanisms required to deserialize this IStoreable object.
        /// </param>
        public virtual void Deserialize( Atom.Storage.IDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
        }
    }
}
