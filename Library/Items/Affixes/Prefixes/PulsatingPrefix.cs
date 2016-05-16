// <copyright file="PulsatingPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Prefixes.PulsatingPrefix class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes.Prefixes
{
    using Atom.Math;
    using Zelda.Items.Affixes.Suffixes;

    /// <summary>
    /// The 'Pulsating' prefix randomly chooses another IPrefix
    /// every time the IPrefix is applied.
    /// </summary>
    public sealed class PulsatingPrefix : IPrefix
    {
        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.Pulsating;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PulsatingPrefix class.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        internal PulsatingPrefix( RandMT rand )
        {
            this.rand = rand;
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
            IAffix affix = GetAffix( baseItem );
            if( affix == null )
                return;

            affix.Apply( item, baseItem );
            item.MultiplyColor( 1.0f, 1.0f, 1.0f, 0.85f );
        }

        /// <summary>
        /// Gets the IAffix that should be used for the specified Item.
        /// </summary>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        /// <returns>
        /// An IAffix; or null.
        /// </returns>
        private IAffix GetAffix( Item baseItem )
        {
            var prefix = AffixDatabase.Instance.GetRandomFittingPrefix( baseItem, rand );

            if( IsPrefixAllowed( prefix ) )
            {
                return prefix;
            }
            else
            {
                var suffix = AffixDatabase.Instance.GetRandomFittingSuffix( baseItem, rand );

                if( IsSuffixAllowed( suffix ) )
                {
                    return suffix;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the specified IPrefix is allowed to be used by the PulsatingPrefix.
        /// </summary>
        /// <param name="prefix">
        /// The IPrefix to investigate.
        /// </param>
        /// <returns>
        /// true if the IPrefix can be used;
        /// otherwise false.
        /// </returns>
        private static bool IsPrefixAllowed( IPrefix prefix )
        {
            if( prefix is PulsatingPrefix || prefix is PrecisePrefix )
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the specified ISuffix is allowed to be used by the PulsatingPrefix.
        /// </summary>
        /// <param name="suffix">
        /// The ISuffix to investigate.
        /// </param>
        /// <returns>
        /// true if the ISuffix can be used;
        /// otherwise false.
        /// </returns>
        private static bool IsSuffixAllowed( ISuffix suffix )
        {
            // Stat modifying StatusEffects are exluded.
            if( suffix is OfKingsSuffix || suffix is OfPowerSuffix || suffix is OfTheHawkSuffix ||
                suffix is OfTheHolySpiritSuffix || suffix is OfRageSuffix || suffix is OfEquilibriumSuffix )
                return false;

            return true;
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
            if( baseItem.Level < 40 )
                return false;

            return baseItem is Equipment;
        }

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;
    }
}
