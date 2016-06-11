
namespace Zelda.Items.Affixes
{
    /// <summary>
    /// Item affixes are used to randomly 'customize' items.
    /// </summary>
    /// <seealso cref="AffixedItem"/>
    /// <seealso cref="IPrefix"/>
    /// <seealso cref="ISuffix"/>
    /// <remarks>
    /// Most affixes scale with the level of the item they are applied to.
    /// Affixes are supposed to be immutable objects, that always behave the same.
    /// IAffixes can be pulled from the <see cref="AffixDatabase"/>.
    /// </remarks>
    public interface IAffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        /// Applies this IAffix to an Item.
        /// </summary>
        /// <param name="item">
        /// The Item that gets directly modified by this IAffix.
        /// </param>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        void Apply( Item item, Item baseItem );

        /// <summary>
        /// Gets a value indicating whether this IAffix could
        /// possibly applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The item this IAffix is supposed to be applied to.
        /// </param>
        /// <returns>
        /// True if this IAffix could possible applied to the given <paramref name="baseItem"/>;
        /// otherwise false.
        /// </returns>
        bool IsApplyable( Item baseItem );
    }
}
