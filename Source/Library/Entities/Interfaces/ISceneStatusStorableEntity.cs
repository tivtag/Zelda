namespace Zelda.Entities
{
    using Zelda.Entities.Components;
    
    public interface ISceneStatusStorableEntity : IZeldaEntity
    {
        SceneStatusStoreable Storeable
        {
            get;
        }
    }
}
