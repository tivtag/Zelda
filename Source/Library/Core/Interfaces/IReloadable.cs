
namespace Zelda
{
    /// <summary>
    /// Provides a mechanism for reloading an object.
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// Reloads this <see cref="IReloadable"/> object.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        void Reload( IZeldaServiceProvider serviceProvider );
    }
}
