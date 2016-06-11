
namespace Zelda.Actions.Scene
{
    using System.ComponentModel;
    using Atom.Events;

    /// <summary>
    /// Implements an action that triggers an event. The event must support null objects.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class ExecuteEventAction : BaseAction
    {
        /// <summary>
        /// Gets or sets the event that will be triggered.
        /// </summary>
        [System.ComponentModel.Editor( typeof( Atom.Events.Design.EventCreationEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Event Event
        {
            get;
            set;
        }

        public override void Execute()
        {
            if( this.Event != null && this.Event.CanBeTriggeredBy( null ) )
            {
                this.Event.Trigger( null );
            }
        }

        public override void Dexecute()
        {
        }

        public override void Serialize( Atom.Storage.ISerializationContext context )
        {
            base.Serialize( context );
            context.Write( this.Event != null ? this.Event.Name : string.Empty );
        }

        public override void Deserialize( Atom.Storage.IDeserializationContext context )
        {
            base.Deserialize( context );
            
            string notUnlockedEventName = context.ReadString();
            if( notUnlockedEventName.Length > 0 )
            {
                var sceneContext = context as Zelda.Saving.ISceneDeserializationContext;
                this.Event = sceneContext.Scene.EventManager.GetEvent( notUnlockedEventName );
            }
        }
    }
}
