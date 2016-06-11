// <copyright file="OfEquilibriumSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfEquilibriumSuffix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Suffixes
{
    using Zelda.Status;

    /// <summary>
    /// The 'of Equilibrium' suffix equally distributes the base stats of an item, 
    /// adding 15% additonal item budget to the item.
    /// </summary>
    internal sealed class OfEquilibriumSuffix : ISuffix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            { 
                return AffixResources.OfEquilibrium; 
            }
        }

        /// <summary>
        /// Applies this IAffix to an Item.
        /// </summary>
        /// <param name="item">
        /// The Item that gets directly modified by this IAffix.
        /// </param>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        public void Apply( Item item, Item baseItem )
        {
            var equipment = (Equipment)item;
            double budget = GetBudget( equipment );

            // Increase item budget by 15%.
            budget = 1.15 * budget;

            // Reset equipment.
            equipment.Strength = 0;
            equipment.Dexterity = 0;
            equipment.Agility = 0;
            equipment.Vitality = 0;
            equipment.Intelligence = 0;
            equipment.Luck = 0;

            // Improve item until out of budget.
            double usedBudget = 0.0;

            do
            {
                equipment.Strength += 1;
                equipment.Dexterity += 1;
                equipment.Agility += 1;
                equipment.Vitality += 1;
                equipment.Intelligence += 1;
                equipment.Luck += 1;

                usedBudget = GetBudget( equipment );
            }
            while( usedBudget < budget );

            // Make the item more blue:
            equipment.MultiplyColor( 0.75f, 0.55f, 1.0f );
        }

        /// <summary>
        /// Gets the item budget used by the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment instance.
        /// </param>
        /// <returns>
        /// The stat budget used by the given Equipment.
        /// </returns>
        private static double GetBudget( Equipment equipment )
        { 
            return StatusCalc.GetItemBudgetUsed(
                equipment.Strength,
                equipment.Dexterity,
                equipment.Agility,
                equipment.Vitality,
                equipment.Intelligence,
                equipment.Luck
            );
        }

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
        public bool IsApplyable( Item baseItem )
        {
            var equipment = baseItem as Equipment;
            return equipment != null && GetBudget( equipment ) >= 10;
        }
    }
}
