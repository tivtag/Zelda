
namespace Zelda.Talents
{
    /// <summary>
    /// Enumerates the types of talents.
    /// </summary>
    public enum TalentType
    {
        /// <summary>
        /// If the type has not been set or is unknown.
        /// </summary>
        None,

        /// <summary>
        /// An active talent requires the player to do something; e.g. trigger a skill.
        /// </summary>
        Active,

        /// <summary>
        /// A passive talent is active without the player having to do anything actively.
        /// </summary>
        Passive
    }
}
