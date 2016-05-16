
namespace Zelda
{
    /// <summary>
    /// Allows setup logic to be executed after all normal setup logic (even from other modules)
    /// has been executed.
    /// </summary>
    public interface IPostLoad
    {
        void PostLoad( IZeldaServiceProvider serviceProvider );
    }
}
