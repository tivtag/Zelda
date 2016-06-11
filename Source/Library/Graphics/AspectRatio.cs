
namespace Zelda.Graphics
{
    /// <summary>
    /// Enumerates the various aspect ratios that the game supports.
    /// </summary>
    public enum AspectRatio
    {
        /// <summary>
        /// The default aspect ratio of the game; best suited for window-play.
        /// </summary>
        /// <value>
        /// 1.5
        /// </value>
        Normal,

        /// <summary>
        /// 16:9 aspect ratio; best used for fullscreen play on a 16:9 monitor.
        /// </summary>
        /// <value>
        /// 1.7*
        /// </value>
        Wide16to9,
        
        /// <summary>
        /// 16:9 aspect ratio; best used for fullscreen play on a 16:9 monitor.
        /// </summary>
        /// <value>
        /// 1.6*
        /// </value>
        Wide16to10
    }
}
