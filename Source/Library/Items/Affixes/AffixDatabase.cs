// <copyright file="AffixDatabase.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.AffixDatabase class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.Affixes
{
    // TODO: Re-factoring AffixDatabase to not use a singleton.

    using System;
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Zelda.Items.Affixes.Prefixes;
    using Zelda.Items.Affixes.Suffixes;
    
    /// <summary>
    /// Allows access to all known <see cref="IPrefix"/>es and <see cref="ISuffix"/>es.
    /// </summary>
    public class AffixDatabase
    {
        #region [ Singleton ]

        /// <summary>
        /// Gets the singleton instance of the AffixDatabase class.
        /// </summary>
        public static AffixDatabase Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// The singleton instance of the AffixDatabase class.
        /// </summary>
        private static readonly AffixDatabase instance = new AffixDatabase();

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes this AffixDatabase by adding all known IPrefixes and IAffixes.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Initialize( IZeldaServiceProvider serviceProvider )
        {
            this.Add( new AcutePrefix() );
            this.Add( new RuggedPrefix() );
            this.Add( new SturdyPrefix() );
            this.Add( new EdgedPrefix() );
            this.Add( new DeadlyPrefix() );
            this.Add( new FlexiblePrefix() );
            this.Add( new OrnatePrefix() );
            this.Add( new RustyPrefix() );
            this.Add( new StrondPrefix() );
            this.Add( new RubyPrefix() );
            this.Add( new BlackenedPrefix() );
            this.Add( new RainbowPrefix() );
            this.Add( new RiskyPrefix() );
            this.Add( new InflamedPrefix() );
            this.Add( new PoisonedPrefix() );
            this.Add( new CemeteryPrefix() );
            this.Add( new PiercingPrefix() );
            this.Add( new EnragingPrefix() );
            this.Add( new PrecisePrefix() );
            this.Add( new SlipperyPrefix() );
            this.Add( new HeavyPrefix() );
            this.Add( new ForcefulPrefix() );
            this.Add( new RetardingPrefix() );
            this.Add( new PenetratingPrefix() );
            this.Add( new EmeraldPrefix() );
            this.Add( new PerforatedPrefix() );
            this.Add( new HauntingPrefix() );
            this.Add( new ScintillantPrefix() );
            this.Add( new EnrichedPrefix() );
            this.Add( new PulsatingPrefix( serviceProvider.Rand ) );
            this.Add( new UselessPrefix() );
            this.Add( new GoldenPrefix() );
            this.Add( new OiledPrefix() );
            this.Add( new ImpetuousPrefix() );

            this.Add( new OfFeathersSuffix() );
            this.Add( new OfCourageSuffix() );
            this.Add( new OfPowerSuffix() );
            this.Add( new OfTheSagesSuffix() );
            this.Add( new OfFortuneSuffix() );
            this.Add( new OfSorcerySuffix() );
            this.Add( new OfDoomSuffix() );
            this.Add( new OfEquilibriumSuffix() );
            this.Add( new OfEaseSuffix() );
            this.Add( new OfRecklesnessSuffix() );
            this.Add( new OfDefensivenesSuffix() );
            this.Add( new OfRecoverySuffix() );
            this.Add( new OfTheSeasonsSuffix() );
            this.Add( new OfTheTitansSuffix() );
            this.Add( new OfTheBlockadeSuffix() );
            this.Add( new OfCorruptionSuffix() );
            this.Add( new OfKingsSuffix() );
            this.Add( new OfTheHolySpiritSuffix() );
            this.Add( new OfAssassinationSuffix() );
            this.Add( new OfWitchCraftSuffix() );
            this.Add( new OfBerserkerRageSuffix() );
            this.Add( new OfHasteSuffix() );
            this.Add( new OfTheHawkSuffix() );
            this.Add( new OfRuptureSuffix() );
            this.Add( new OfImmediacySuffix() );
            this.Add( new OfRageSuffix() );
            this.Add( new OfProtectionSuffix() );
            this.Add( new OfFluencySuffix() );
            this.Add( new OfLightSuffix() );
            this.Add( new OfTheeHastyAvatarSuffix( serviceProvider.SpriteLoader ) );            
        }

        /// <summary>
        /// Adds the given IPrefix to this AffixDatabase.
        /// </summary>
        /// <param name="prefix">
        /// The IPrefix to add.
        /// </param>
        private void Add( IPrefix prefix )
        {
            this.prefixes.Add( GetAffixName( prefix ), prefix );
        }

        /// <summary>
        /// Adds the given ISuffix to this AffixDatabase.
        /// </summary>
        /// <param name="suffix">
        /// The ISuffix to add.
        /// </param>
        private void Add( ISuffix suffix )
        {
            this.suffixes.Add( GetAffixName( suffix ), suffix );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to get the IPrefix with the given name.
        /// </summary>
        /// <seealso cref="GetAffixName"/>
        /// <param name="prefixName">
        /// The name of the prefix to get.
        /// </param>
        /// <returns>
        /// The requested IPrefix.
        /// </returns>
        internal IPrefix GetPrefix( string prefixName )
        {
            return this.prefixes[prefixName];
        }

        /// <summary>
        /// Tries to get the ISuffix with the given name.
        /// </summary>
        /// <seealso cref="GetAffixName"/>
        /// <param name="suffixName">
        /// The name of the suffix to get.
        /// </param>
        /// <returns>
        /// The requested ISuffix.
        /// </returns>
        internal ISuffix GetSuffix( string suffixName )
        {
            return this.suffixes[suffixName];
        }

        /// <summary>
        /// Randomly gets one of the <see cref="IPrefix"/>es in this AffixDatabase. 
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A random IPrefix.
        /// </returns>
        public IPrefix GetRandomPrefix( RandMT rand )
        {
            int index = rand.RandomRange( 0, prefixes.Count - 1 );
            return this.prefixes.Values[index];
        }

        /// <summary>
        /// Randomly gets one of the <see cref="ISuffix"/>es in this AffixDatabase. 
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A random ISuffix.
        /// </returns>
        public ISuffix GetRandomSuffix( RandMT rand )
        {
            int index = rand.RandomRange( 0, suffixes.Count - 1 );
            return this.suffixes.Values[index];
        }

        /// <summary>
        /// Tries to randomly get an <see cref="IPrefix"/> that can be applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The base item the affix is supposed to be applied to.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A fitting randomly-selected affix; or null if none could be found in time.
        /// </returns>
        public IPrefix GetRandomFittingPrefix( Item baseItem, RandMT rand )
        {
            int trials = 0;
            const int MaxTrials = 30;

            IPrefix prefix = null;

            while( prefix == null && trials <= MaxTrials )
            {
                prefix = this.GetRandomPrefix( rand );

                if( !prefix.IsApplyable( baseItem ) )
                {
                    // If the affix is not applyable,
                    // then try to find another one.
                    prefix = null;
                    ++trials;
                }
            }

            return prefix;
        }
        
        /// <summary>
        /// Tries to randomly get an <see cref="ISuffix"/> that can be applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The base item the affix is supposed to be applied to.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A fitting randomly-selected affix; or null if none could be found in time.
        /// </returns>
        public ISuffix GetRandomFittingSuffix( Item baseItem, RandMT rand )
        {
            int trials = 0;
            const int MaxTrials = 25;

            ISuffix suffix = null;

            while( suffix == null && trials <= MaxTrials )
            {
                suffix = this.GetRandomSuffix( rand );

                if( !suffix.IsApplyable( baseItem ) )
                {
                    // If the affix is not applyable,
                    // then try to find another one.
                    suffix = null;
                    ++trials;
                }
            }

            return suffix;
        }

        /// <summary>
        /// Helper method that returns the name that uniquely idenfities the given Affix.
        /// </summary>
        /// <param name="affix">
        /// The affix. Can be null.
        /// </param>
        /// <returns>
        /// The name of the affix; or <see cref="String.Empty"/>.
        /// </returns>
        public static string GetAffixName( IAffix affix )
        {
            return affix != null ? affix.GetType().GetTypeName() : string.Empty;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores all known <see cref="IPrefix"/>es, sorted by their type-name.
        /// </summary>
        private readonly SortedList<string, IPrefix> prefixes = new SortedList<string, IPrefix>( 10 );

        /// <summary>
        /// Stores all known <see cref="ISuffix"/>es, sorted by their type-name.
        /// </summary>
        private readonly SortedList<string, ISuffix> suffixes = new SortedList<string, ISuffix>( 10 );

        #endregion
    }
}
