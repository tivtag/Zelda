
namespace Zelda.Entities.Modifiers
{
    using Atom.Components;
    using Zelda.Saving;

    /// <summary>
    /// Represents an object that modifies an entity while being directly attached to it.
    /// As such the modification of the entity can change over time.
    /// </summary>
    public interface IAttachedEntityModifier : IComponent, ISaveable
    {
    }
}
