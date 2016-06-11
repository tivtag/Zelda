
namespace Zelda.Items.Sets
{
    /// <summary>
    /// Provides a mechanism to receive <see cref="ISet"/>s.
    /// </summary>
    public interface ISetDatabase
    {
        /// <summary>
        /// Tries to get the <see cref="ISet"/> with the specified <paramref name="setName"/>.
        /// </summary>
        /// <param name="setName">
        /// The name of the set.
        /// </param>
        /// <returns>
        /// The requested ISet; or null.
        /// </returns>
        ISet Get( string setName );
    }
}
