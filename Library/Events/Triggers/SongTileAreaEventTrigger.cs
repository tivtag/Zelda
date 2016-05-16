
namespace Zelda.Events
{
    using Zelda.Ocarina.Songs;
    using Zelda.Saving;

    /// <summary>
    /// Represents an EventTrigger that gets triggered when the player
    /// plays a specific song.
    /// </summary>
    public sealed class SongTileAreaEventTrigger : ZeldaTileAreaEventTrigger
    {
        public override bool CanBeTriggeredBy( Atom.Events.TriggerContext context )
        {
            if( context.Source is ExecuteTriggerSong )
            {
                return base.CanBeTriggeredBy( context );
            }
            else
            {
                return false;
            }
        }

        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );
            context.WriteDefaultHeader();
        }

        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );
            context.ReadDefaultHeader( typeof( SongTileAreaEventTrigger ) );
        }
    }
}
