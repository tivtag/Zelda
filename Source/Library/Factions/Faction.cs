// <copyright file="Faction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Factions.Faction class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Factions
{
    using System.ComponentModel;

    /// <summary>
    /// Represents an ingame faction, similiar to the ones in World of Warcraft.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The player has a specified reputation towards each Faction.
    /// This reputation is represented by a simple integer, called the reputation value.
    /// </para>
    /// <para>
    /// The following list shows what reputation value represents
    /// what <see cref="ReputationLevel"/>.
    /// <code>
    ///    Exalted      42000 to     ~
    ///    Revered      21000 to 42000
    ///    Honored       9000 to 21000
    ///    Friendly      3000 to  9000
    ///    Neutral          0 to  3000
    ///    Unfriendly   -3000 to     0
    ///    Hostile      -6000 to -3000
    ///    Hated           -~ to -6000
    /// </code>
    /// </para>
    /// <para>
    /// The player may gain reputation by completing quests,
    /// using/turning-in specific items or killing monsters.
    /// </para>
    /// </remarks>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class Faction : Atom.IReadOnlyNameable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the name that uniquely identifies this Faction.
        /// </summary>
        public string Name
        {
            get 
            {
                return this._name;
            }
        }

        /// <summary>
        /// Gets the localized name of this Faction.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return this._localizedName; 
            }
        }

        /// <summary>
        /// Gets the localized description of this Faction.
        /// </summary>
        public string LocalizedDescription
        {
            get
            {
                return this._localizedDescription;
            }
        }

        /// <summary>
        /// Gets the initial reputation the player gets towards
        /// this Faction when he first discovers it.
        /// </summary>
        public int InitialReputation
        {
            get
            {
                return this._initialReputation; 
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the Faction class.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the new Faction.
        /// </param>
        /// <param name="localizedDescription">
        /// The localized description of the new Faction.
        /// </param>
        /// <param name="initialReputation">
        /// The initial reputation the player gets towards the new Faction
        /// when he first discovers it.
        /// </param>
        internal Faction( string name, string localizedDescription, int initialReputation )
        {
            this._name = name;
            this._initialReputation = initialReputation;
            this._localizedDescription = localizedDescription;

            try
            {
                this._localizedName = Resources.ResourceManager.GetString( "FN_" + name );
            }
            catch( System.InvalidOperationException )
            {
                this._localizedName = name;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the <see cref="ReputationLevel"/> towards a Faction
        /// given a <paramref name="reputation"/> value.
        /// </summary>
        /// <param name="reputation">
        /// The reputation value.
        /// </param>
        /// <returns>
        /// The <see cref="ReputationLevel"/> that corresponds to
        /// the given <paramref name="reputation"/> value.
        /// </returns>
        public static ReputationLevel GetReputationLevel( int reputation )
        {
            /*  -Level-   -Points needed-
              
                Exalted              42000 to     ~
                Revered     21,000   21000 to 42000
                Honored     12,000    9000 to 21000
                Friendly     6,000    3000 to  9000
                Neutral      3,000       0 to  3000
                Unfriendly   3,000   -3000 to     0
                Hostile      3,000   -6000 to -3000
                Hated                   -~ to -6000
            */

            if( reputation < 0 )
            {
                if( reputation >= -3000 )
                    return ReputationLevel.Unfriendly;
                else if( reputation >= -6000 )
                    return ReputationLevel.Hostile;
                else
                    return ReputationLevel.Hated;
            }
            else
            {
                if( reputation < 3000 )
                    return ReputationLevel.Neutral;
                else if( reputation < 9000 )
                    return ReputationLevel.Friendly;
                else if( reputation < 21000 )
                    return ReputationLevel.Honored;
                else if( reputation < 42000 )
                    return ReputationLevel.Revered;
                else
                    return ReputationLevel.Exalted;
            }
        }

        /// <summary>
        /// Gets the reputation start and end values of the given <see cref="ReputationLevel"/>.
        /// </summary>
        /// <param name="level">The input ReputationLevel.</param>
        /// <param name="startValue">
        /// Will contain the reputation value at which the given ReputationLevel starts.
        /// </param>
        /// <param name="endValue">
        /// Will contain the reputation value at which the given ReputationLevel ends.
        /// </param>
        public static void GetReputationValues( ReputationLevel level, out int startValue, out int endValue )
        {            
            /*  -Level-   -Points needed-
              
                Exalted              42000 to     ~
                Revered     21,000   21000 to 42000
                Honored     12,000    9000 to 21000
                Friendly    6,000     3000 to  9000
                Neutral     3,000        0 to  3000
                Unfriendly  3,000    -3000 to     0
                Hostile     3,000    -6000 to -3000
                Hated                   -~ to -6000
            */

            switch( level )
            {
                case ReputationLevel.Hated:
                    startValue = int.MinValue;
                    endValue   = -6000;
                    break;

                case ReputationLevel.Hostile:
                    startValue = -6000;
                    endValue   = -3000;
                    break;

                case ReputationLevel.Unfriendly:
                    startValue = -3000;
                    endValue   = 0;
                    break;

                case ReputationLevel.Neutral:
                    startValue = 0;
                    endValue   = 3000;
                    break;

                case ReputationLevel.Friendly:
                    startValue = 6000;
                    endValue   = 9000;
                    break;

                case ReputationLevel.Honored:
                    startValue = 9000;
                    endValue   = 21000;
                    break;

                case ReputationLevel.Revered:
                    startValue = 21000;
                    endValue   = 42000;
                    break;

                case ReputationLevel.Exalted:
                    startValue = 42000;
                    endValue   = int.MaxValue;
                    break;

                default:
                    startValue = endValue = int.MaxValue;
                    break;
            }
        }
        
        /// <summary>
        /// Returns a human-readable string representation
        /// of this <see cref="Faction"/>.
        /// </summary>
        /// <returns>
        /// A human-readable string representation of this <see cref="Faction"/>.
        /// </returns>
        public override string ToString()
        {
            return this.LocalizedName;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field of the <see cref="Name"/> property.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The storage field of the <see cref="LocalizedName"/> property.
        /// </summary>
        private readonly string _localizedName;

        /// <summary>
        /// The storage field of the <see cref="LocalizedDescription"/> property.
        /// </summary>
        private readonly string _localizedDescription;

        /// <summary>
        /// The initial reputation the player gets towards this Faction
        /// when he first discovers it.
        /// </summary>
        private readonly int _initialReputation;

        #endregion
    }
}
