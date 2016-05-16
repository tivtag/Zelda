
namespace Zelda.Entities
{
    using Zelda.Entities.Components;

    public interface ISpawnableEntity : IZeldaEntity
    {
        Spawnable Spawnable
        {
            get;
        }
    }
}
