// <copyright file="StatResetAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.StatResetAction class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Actions.Player
{
    using System;
    using Zelda.Saving;
    using Zelda.Status;

    /// <summary>
    /// Represents the base class for both the StatReset and TalentReset actions.
    /// </summary>
    public abstract class ResetAction : BasePlayerAction
    {
        /// <summary>
        /// The number of rubies resetting costs per character level.
        /// </summary>
        public int LevelToRubyFactor
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the ResetAction class.
        /// </summary>
        protected ResetAction()
        {
            this.LevelToRubyFactor = 10;
        }

        /// <summary>
        /// This method is not supported by this ResetAction.
        /// </summary>
        public override void Dexecute()
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Gets the number of rubies resetting would cost.
        /// </summary>
        /// <param name="statble">
        /// The statble component of the player.
        /// </param>
        /// <returns>
        /// The number of rubies resetting the stat points would cost.
        /// </returns>
        protected long GetRubyCost( Statable statble )
        {
            return statble.Level * this.LevelToRubyFactor;
        }

        /// <summary>
        /// Executes this StatResetAction.
        /// </summary>
        public override void Execute()
        {
            if( this.CanExecute() )
            {
                this.ActuallyReset();
                this.Statable.Rubies -= this.GetRubyCost( this.Statable );

                var service = IoC.Resolve<Zelda.UI.IItemInfoVisualizer>();
                service.ResetCache();
            }
        }

        /// <summary>
        /// Executes the actual resetting logic.
        /// </summary>
        protected abstract void ActuallyReset();

        /// <summary>
        /// Gets a value indicating whether this IAction can be executed.
        /// </summary>
        /// <returns>
        /// true if this IAction can be executed;
        /// otherwise false.
        /// </returns>
        public override bool CanExecute()
        {
            var statable = this.Statable;
            return statable.Rubies >= this.GetRubyCost( statable );
        }

        /// <summary>
        /// Serializes this IStoreable object using the given ISerializationContext.
        /// </summary>
        /// <param name="context">
        /// Provides access to the mechanisms required to serialize this IStoreable object.
        /// </param>
        public override void Serialize( Atom.Storage.ISerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.LevelToRubyFactor );
        }

        /// <summary>
        /// Deserializes this IStoreable object using the given IDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// Provides access to the mechanisms required to deserialize this IStoreable object.
        /// </param>
        public override void Deserialize( Atom.Storage.IDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.LevelToRubyFactor = context.ReadInt32();
        }
    }
}
