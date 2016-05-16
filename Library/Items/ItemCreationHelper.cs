// <copyright file="ItemCreationHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Items.ItemCreationHelper class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using Atom.Math;
    using Zelda.Items.Affixes;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides a mechanism to create instances of an <see cref="Item"/>,
    /// that based on a random number generator and the Item's properties 
    /// may be affixed or not.
    /// </summary>
    public static class ItemCreationHelper
    {
        /// <summary>
        /// Creates an instance of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item for which an instance should be created from.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>The newly created ItemInstance.</returns>
        public static ItemInstance Create( Item item, RandMT rand )
        {
            if( ShouldCreateAffixedItemInstance( item, rand ) )
            {
                return CreateAffixedItemInstance( item, rand, AffixDatabase.Instance );
            }
            else
            {
                float powerFactor = item.GetPowerFactor( rand );
                return item.CreateInstance( powerFactor );
            }
        }

        /// <summary>
        /// Gets a value indicating whether an AffixedItemInstance should be created for the given Item.
        /// </summary>
        /// <param name="item">
        /// The item for which an instance should be created from.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// True if an AffixedItemInstance should be created;
        /// otherwise false.
        /// </returns>
        private static bool ShouldCreateAffixedItemInstance( Item item, RandMT rand )
        {
            if( item.AllowedAffixes == AffixType.None )
                return false;

            if( item.AllowedAffixes == AffixType.AlwaysBoth ||
                item.AllowedAffixes == AffixType.AlwaysOneOrBoth )
            {
                return true;
            }

            return rand.RandomRange( 0, 100 ) > GetChanceToBeAffixed( item.Quality );                
        }

        /// <summary>
        /// Gets the chance an item of the given ItemQuality has atleast one affix.
        /// </summary>
        /// <param name="itemQuality">
        /// The quality of the item.
        /// </param>
        /// <returns>
        /// A value between 0 and 100, representing the chance for the item to have atleast one affix.
        /// </returns>
        private static int GetChanceToBeAffixed( ItemQuality itemQuality )
        {
            switch( itemQuality )
            {
                case ItemQuality.Common:
                case ItemQuality.Magic:
                    return 50;

                case ItemQuality.Rare:
                    return 35;

                case ItemQuality.Epic:
                    return 25;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Creates an instance of the given Item, that has been extended with atleast one affix.
        /// </summary>
        /// <param name="item">
        /// The base item.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <param name="affixDatabase">
        /// Provices access to all known ISuffixes and IPrefixes.
        /// </param>
        /// <returns>
        /// The newly created ItemInstance.
        /// </returns>
        private static ItemInstance CreateAffixedItemInstance( Item item, RandMT rand, AffixDatabase affixDatabase )
        {
            // Randomly select what affixes to create if both are supported; otherwise use the default:
            AffixType affixes = GetAffixesToCreate( item, rand );

            IPrefix prefix = null;
            ISuffix suffix = null;

            switch( affixes )
            {
                case AffixType.Prefix:
                    prefix = affixDatabase.GetRandomFittingPrefix( item, rand );
                    break;

                case AffixType.Suffix:
                    suffix = affixDatabase.GetRandomFittingSuffix( item, rand );
                    break;

                // Prefix & Sufix.
                case AffixType.Both:
                case AffixType.AlwaysBoth:
                    prefix = affixDatabase.GetRandomFittingPrefix( item, rand );
                    suffix = affixDatabase.GetRandomFittingSuffix( item, rand );
                    break;

                default:
                    throw new NotImplementedException();
            }

            if( prefix == null && suffix == null )
            {
                // Finding affixes for the item has failed; fall back.
                return item.CreateInstance( rand );
            }
            else
            {
                var affixedItem = new AffixedItem( item, prefix, suffix );
                return affixedItem.CreateInstance( rand );
            }
        }

        /// <summary>
        /// Gets the Affixes that should be created for the given Item.
        /// </summary>
        /// <param name="item">The base item.</param>
        /// <param name="rand">A random number generator.</param>
        /// <returns>
        /// The affixes to create.</returns>
        private static AffixType GetAffixesToCreate( Item item, RandMT rand )
        {
            switch( item.AllowedAffixes )
            {
                case AffixType.Both:
                    return (AffixType)rand.RandomRange( 1, 3 );

                case AffixType.AlwaysBoth:
                    return AffixType.Both;

                case AffixType.AlwaysOneOrBoth:
                    if( rand.RandomBoolean )
                        return (AffixType)rand.RandomRange( 1, 3 );
                    else
                        return AffixType.Both;

                default:
                    return item.AllowedAffixes;
            }
        }
    }
}
