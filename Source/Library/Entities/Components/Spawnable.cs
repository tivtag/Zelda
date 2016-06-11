
namespace Zelda.Entities.Components
{
    using Atom;
    using Zelda.Entities.Spawning;

    public sealed class Spawnable : ZeldaComponent
    {
        public event RelaxedEventHandler<Spawnable, ISpawnPoint> Spawned;

        public void NotifySpawnedAt( ISpawnPoint spawnPoint )
        {
            this.Spawned.Raise( this, spawnPoint );
        }
    }
}
