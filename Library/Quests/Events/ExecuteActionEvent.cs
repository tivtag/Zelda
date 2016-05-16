
namespace Zelda.Quests.Events
{
    using Zelda.Actions;
    using Zelda.Saving;
    using Atom;

    /// <summary>
    /// Implements an <see cref="IQuestEvent"/> that when executed excutes an <see cref="IAction"/>.
    /// </summary>
    public sealed class ExecuteActionEvent : IQuestEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="IAction"/> that gets executed when this ExecuteActionEvent gets executed.
        /// </summary>
        [System.ComponentModel.Editor( typeof( Zelda.Actions.Design.ActionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IAction Action
        {
            get;
            set;
        }
        
        /// <summary>
        /// Executes this ExecuteActionEvent.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        public void Execute( Quest quest )
        {
            if( this.Action != null && this.Action.CanExecute() )
            {
                this.Action.Execute();
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.WriteStoreObject( this.Action );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Saving.IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.Action = context.ReadStoreObject<IAction>();
        }
    }
}
