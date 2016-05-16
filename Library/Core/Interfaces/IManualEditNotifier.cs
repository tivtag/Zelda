
namespace Zelda
{
    /// <summary>
    /// Provides a mechanism of informing an object
    /// that manual editing of the object has started.
    /// </summary>
    /// <remarks>
    /// This interface is useful for objects that need to be initialized
    /// before a tool starts manually editing the object.
    /// </remarks>
    public interface IManualEditNotifier
    {
        /// <summary>
        /// Notifies this IManualEditNotifier that editing has begun.
        /// </summary>
        void StartManualEdit();
    }
}
