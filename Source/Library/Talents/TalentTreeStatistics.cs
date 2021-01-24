// <copyright file="TalentTreeStatistics.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.TalentTreeStatistics class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents
{
    using System.Collections.Generic;
    using System.Linq;
    using Zelda.Talents.Classes;

    /// <summary>
    /// Captures statistics about a <see cref="TalentTree"/>.
    /// </summary>
    public sealed class TalentTreeStatistics
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the CharacterClass that is associated with the current
        /// configuration of the TalenTree.
        /// </summary>
        /// <value>
        /// Might be null.
        /// </value>
        public CharacterClass Class
        {
            get;
            private set;
        }

        #region - Level -

        /// <summary>
        /// Gets the 'level' of the melee talent tree.
        /// </summary>
        /// <value>
        /// A value between 0 and 5 representing how many points have
        /// been invested into the tree relative to the total amount of points.
        /// </value>
        public int MeleeLevel
        {
            get
            {
                float meleeRatio = this.InvestedPointsMelee  / (float)this.TotalsPointsMelee;
                return GetLevel( meleeRatio );
            }
        }

        /// <summary>
        /// Gets the 'level' of the ranged talent tree.
        /// </summary>
        /// <value>
        /// A value between 0 and 5 representing how many points have
        /// been invested into the tree relative to the total amount of points.
        /// </value>
        public int RangedLevel
        {
            get
            {
                float rangedRatio = this.InvestedPointsRanged  / (float)this.TotalsPointsRanged;
                return GetLevel( rangedRatio );
            }
        }

        /// <summary>
        /// Gets the 'level' of the magic talent tree.
        /// </summary>
        /// <value>
        /// A value between 0 and 5 representing how many points have
        /// been invested into the tree relative to the total amount of points.
        /// </value>
        public int MagicLevel
        {
            get
            {
                float magicRatio = this.InvestedPointsMagic  / (float)this.TotalsPointsMagic;
                return GetLevel( magicRatio );
            }
        }

        /// <summary>
        /// Gets the 'level' of the support talent tree.
        /// </summary>
        /// <value>
        /// A value between 0 and 5 representing how many points have
        /// been invested into the tree relative to the total amount of points.
        /// </value>
        public int SupportLevel
        {
            get
            {
                float supportRatio = this.InvestedPointsSupport  / (float)this.TotalsPointsSupport;
                return GetLevel( supportRatio );
            }
        }

        #endregion

        #region - Invested -

        /// <summary>
        /// Gets the number of talent points the player has invested into the melee sub TalentTree.
        /// </summary>
        public int InvestedPointsMelee
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of talent points the player has invested into the ranged sub TalentTree.
        /// </summary>
        public int InvestedPointsRanged
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of talent points the player has invested into the magic sub TalentTree.
        /// </summary>
        public int InvestedPointsMagic
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of talent points the player has invested into the support sub TalentTree.
        /// </summary>
        public int InvestedPointsSupport
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total number of talent points the player has invested so far.
        /// </summary>
        public int TotalInvestedPoints
        {
            get
            {
                return this.InvestedPointsMelee + this.InvestedPointsRanged + this.InvestedPointsMagic + this.InvestedPointsSupport;
            }
        }

        #endregion

        #region - Total -

        /// <summary>
        /// Gets the number of talent points the player may invest into the melee sub TalentTree.
        /// </summary>
        public int TotalsPointsMelee
        {
            get
            {
                return this.totalsPointsMelee;
            }
        }

        /// <summary>
        /// Gets the number of talent points the player may invest into the ranged sub TalentTree.
        /// </summary>
        public int TotalsPointsRanged
        {
            get
            {
                return this.totalsPointsRanged;
            }
        }

        /// <summary>
        /// Gets the number of talent points the player may invest into the magic sub TalentTree.
        /// </summary>
        public int TotalsPointsMagic
        {
            get
            {
                return this.totalsPointsMagic;
            }
        }

        /// <summary>
        /// Gets the number of talent points the player may invest into the support sub TalentTree.
        /// </summary>
        public int TotalsPointsSupport
        {
            get
            {
                return this.totalsPointsSupport;
            }
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the TalentTreeStatistics class.
        /// </summary>
        /// <param name="talentTree">
        /// The <see cref="TalentTree"/> whose statistics are captured by the new TalentTreeStatistics.
        /// </param>
        internal TalentTreeStatistics( TalentTree talentTree )
            : this( talentTree, new CharacterClassTalentMap( new CharacterClassList() ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the TalentTreeStatistics class.
        /// </summary>
        /// <param name="talentTree">
        /// The <see cref="TalentTree"/> whose statistics are captured by the new TalentTreeStatistics.
        /// </param>
        /// <param name="classMap">
        /// The map that maps talent point investment onto CharacterClasses.
        /// </param>
        private TalentTreeStatistics( TalentTree talentTree, CharacterClassTalentMap classMap )
        {
            this.talentTree = talentTree;
            this.classMap = classMap;

            // Hook events.
            this.talentTree.TalentsChanged += this.OnTalentsChanged;

            // Anaylze the tree.
            this.totalsPointsMelee   = GetTotalPoints( talentTree.MeleeRoot );
            this.totalsPointsRanged  = GetTotalPoints( talentTree.RangedRoot );
            this.totalsPointsMagic   = GetTotalPoints( talentTree.MagicRoot );
            this.totalsPointsSupport = GetTotalPoints( talentTree.SupportRoot );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when any of the Talent in the TalentTree has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The System.EventArgs that contain the event data.
        /// </param>
        private void OnTalentsChanged( object sender, System.EventArgs e )
        {
            this.AnalyzeInvestedPoints();
            this.UpdateClass();
        }

        /// <summary>
        /// Updates the CharacterClass based on the current configuration of the TalentTree.
        /// </summary>
        public void UpdateClass()
        {
            var key = new CharacterClassTalentKey(
                this.MeleeLevel,
                this.RangedLevel,
                this.MagicLevel,
                this.SupportLevel
            );

            this.Class = this.classMap.TryGet( key );
        }

        /// <summary>
        /// Gets the number of points the player can invest
        /// into the given talent and all talents that follow it.
        /// </summary>
        /// <param name="talent">
        /// The talent to traverse.
        /// </param>
        /// <returns>
        /// The number of points the player can invest
        /// into the given talent and all talents that follow it.
        /// </returns>
        private static int GetTotalPoints( Talent talent )
        {
            return GetTalentChain( talent )
                .Distinct()
                .Sum( t => t.MaximumLevel );
        }

        /// <summary>
        /// Gets the number of points the player has invested
        /// into the given talent and all talents that follow it.
        /// </summary>
        /// <param name="talent">
        /// The talent to traverse.
        /// </param>
        /// <returns>
        /// The number of points the player has invested
        /// into the given talent and all talents that follow it.
        /// </returns>  
        private static int GetInvestedPoints( Talent talent )
        {
            return GetTalentChain( talent )
                .Distinct()
                .Sum( t => t.Level );
        }
        
        private static IEnumerable<Talent> GetTalentChain( Talent talent )
        {
            yield return talent;

            foreach( Talent following in talent.Following )
            {
                foreach( Talent result in GetTalentChain( following ) )
                {
                    yield return result;                    
                }
            }
        }
        
        /// <summary>
        /// Reanylzes the number of points the player has invested
        /// into the individual sub TalentTrees.
        /// </summary>
        private void AnalyzeInvestedPoints()
        {
            this.InvestedPointsMelee   = GetInvestedPoints( this.talentTree.MeleeRoot );
            this.InvestedPointsRanged  = GetInvestedPoints( this.talentTree.RangedRoot );
            this.InvestedPointsMagic   = GetInvestedPoints( this.talentTree.MagicRoot );
            this.InvestedPointsSupport = GetInvestedPoints( this.talentTree.SupportRoot );
        }

        /// <summary>
        /// Gets the level of talent sub tree given the ratio of invested points to total points.
        /// </summary>
        /// <param name="talentRatio">
        /// The ratio of invested points to total points.
        /// </param>
        /// <returns>
        /// The level of the talent tree; a value from 0 to 5.
        /// </returns>
        private static int GetLevel( float talentRatio )
        {
            return talentRatio > 0.15 ? (talentRatio > 0.3 ? (talentRatio  > 0.6 ? (talentRatio > 0.9 ? 4 : 3) : 2) : 1) : 0;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Maps talent point investment onto CharacterClasses.
        /// </summary>
        private readonly CharacterClassTalentMap classMap;

        /// <summary>
        /// The total number of points in the various talent sub-trees.
        /// </summary>
        private readonly int totalsPointsMelee, totalsPointsRanged, totalsPointsMagic, totalsPointsSupport;

        /// <summary>
        /// Identifies the <see cref="TalentTree"/> whose statistics are captured by this TalentTreeStatistics.
        /// </summary>
        private readonly TalentTree talentTree;

        #endregion
    }
}