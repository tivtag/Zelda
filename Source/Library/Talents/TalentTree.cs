// <copyright file="TalentTree.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.TalentTree class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents
{ 
    using System;
    using System.Collections.Generic;
    using Atom;

    /// <summary>
    /// The TalentTree stores the talents of the player
    /// in a tree like structure.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are four roots that each spawn individual 'trees':
    /// </para>
    /// <para>
    /// Melee    - Contains melee talents.
    /// Ranged   - Contains ranged/rogue talents.
    /// Magic    - Contains magic talents.
    /// Support  - Contains talents that support the other three main trees.
    /// </para>
    /// This class can't be inherited.
    /// </remarks>
    public sealed class TalentTree
    {
        /// <summary>
        /// Raised when anything about the Talents in this TalentTree has changed.
        /// </summary>
        public event EventHandler TalentsChanged;
        
        #region [ Properties ]

        /// <summary>
        /// Gets the amount of free talent points the owner of the TalentTree has.
        /// </summary>
        public int FreeTalentPoints
        {
            get 
            {
                return this.freeTalentPoints;
            }
        }

        /// <summary>
        /// Gets the <see cref="Entities.PlayerEntity"/> that owns the TalentTree.
        /// </summary>
        public Entities.PlayerEntity Owner
        {
            get 
            { 
                return this.player; 
            }
        }

        /// <summary>
        /// Gets the root of the sub-tree that contains melee oriented talents.
        /// </summary>
        public Talent MeleeRoot
        {
            get
            {
                return this.meleeRoot;
            }
        }

        /// <summary>
        /// Gets the root of the sub-tree that contains ranged oriented talents.
        /// </summary>
        public Talent RangedRoot
        {
            get
            { 
                return this.rangedRoot;
            }
        }
        
        /// <summary>
        /// Gets the root of the sub-tree that contains magic oriented talents.
        /// </summary>
        public Talent MagicRoot
        {
            get
            {
                return this.magicRoot; 
            }
        }

        /// <summary>
        /// Gets the root of the sub-tree that contains talents that support the other tree trees.
        /// </summary>
        public Talent SupportRoot
        {
            get
            {
                return this.supportRoot;
            }
        }

        /// <summary>
        /// Gets the CharacterClass the current configuration of this TalentTree is associated with.
        /// </summary>
        public Zelda.Talents.Classes.CharacterClass Class
        {
            get
            {
                return this.statistics.Class;
            }
        }

        /// <summary>
        /// Gets the <see cref="TalentTreeStatistics"/> of this TalentTree.
        /// </summary>
        public TalentTreeStatistics Statistics
        {
            get
            {
                return this.statistics;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TalentTree"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new TalentTree.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal TalentTree( Entities.PlayerEntity player, IZeldaServiceProvider serviceProvider )
        {
            this.player = player;
            this.log    = serviceProvider.Log;

            this.Setup( serviceProvider );
            this.statistics = new TalentTreeStatistics( this );
        }

        /// <summary>
        /// Setups this TalentTree.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void Setup( IZeldaServiceProvider serviceProvider )
        {            
            // Melee Talents:
            this.meleeRoot = new Melee.MeleeTrainingTalent( this, serviceProvider );

            AddTalent( meleeRoot );
            AddTalent( new Melee.ToughnessTalent( this, serviceProvider ) );
            AddTalent( new Melee.VitalityTalent( this, serviceProvider ) );
            AddTalent( new Melee.BashTalent( this, serviceProvider ) );
            AddTalent( new Melee.ImprovedBashTalent( this, serviceProvider ) );
            AddTalent( new Melee.BattleShoutTalent( this, serviceProvider ) );
            AddTalent( new Melee.EnrageTalent( this, serviceProvider ) );
            AddTalent( new Melee.FrenzyTalent( this, serviceProvider ) );
            AddTalent( new Melee.FurorTalent( this, serviceProvider ) );
            AddTalent( new Melee.RecoverWoundsTalent( this, serviceProvider ) );
            AddTalent( new Melee.ShieldWallTalent( this, serviceProvider ) );
            AddTalent( new Melee.PushingAttackTalent( this, serviceProvider ) );
            AddTalent( new Melee.WhirlwindTalent( this, serviceProvider ) );
            AddTalent( new Melee.DoubleAttackTalent( this, serviceProvider ) );
            AddTalent( new Melee.FrustrationTalent( this, serviceProvider ) );
            AddTalent( new Melee.BattleAwarenessTalent( this, serviceProvider ) );
            AddTalent( new Melee.CounterAttackTalent( this, serviceProvider ) );
            AddTalent( new Melee.BladestormTalent( this, serviceProvider ) );
            AddTalent( new Melee.QuickStrikeTalent( this, serviceProvider ) );
            AddTalent( new Melee.ShieldBreakerTalent( this, serviceProvider ) );
            AddTalent( new Melee.ShieldMasteryTalent( this, serviceProvider ) );
            AddTalent( new Melee.ShieldBlockTalent( this, serviceProvider ) );
            AddTalent( new Melee.RevitalizingStrikesTalent( this, serviceProvider ) );           

            // Ranged Talents:
            this.rangedRoot = new Ranged.RangedTrainingTalent( this, serviceProvider );

            AddTalent( rangedRoot );
            AddTalent( new Ranged.PiercingArrowsTalent( this, serviceProvider ) );
            AddTalent( new Ranged.DodgeTrainingTalent( this, serviceProvider ) );
            AddTalent( new Ranged.AgilityTrainingTalent( this, serviceProvider ) );
            AddTalent( new Ranged.GoForTheHeadTalent( this, serviceProvider ) );
            AddTalent( new Ranged.SprintTalent( this, serviceProvider ) );
            AddTalent( new Ranged.LightArrowTalent( this, serviceProvider ) );
            AddTalent( new Ranged.MultiShotTalent( this, serviceProvider ) );
            AddTalent( new Ranged.PoisonedShotTalent( this, serviceProvider ) );
            AddTalent( new Ranged.RapidFireTalent( this, serviceProvider ) );
            AddTalent( new Ranged.ImprovedMultiShotTalent( this, serviceProvider ) );
            AddTalent( new Ranged.ArrowShowerTalent( this, serviceProvider ) );
            AddTalent( new Ranged.HandEyeCoordinationTalent( this, serviceProvider ) );
            AddTalent( new Ranged.QuickHandsTalent( this, serviceProvider ) );
            AddTalent( new Ranged.FireBombTalent( this, serviceProvider ) );
            AddTalent( new Ranged.FireBombChainTalent( this, serviceProvider ) );
            AddTalent( new Ranged.HeadshotTalent( this, serviceProvider ) );
            AddTalent( new Ranged.RogueWeaponMasteryTalent( this, serviceProvider ) );
            AddTalent( new Ranged.ArrowRushTalent( this, serviceProvider ) );
            
            // Magic Talents
            this.magicRoot = new Magic.MagicTrainingTalent( this, serviceProvider );
            AddTalent( magicRoot );
            AddTalent( new Magic.FirewhirlTalent( this, serviceProvider ) );
            AddTalent( new Magic.CorrosiveFireTalent( this, serviceProvider ) );
            AddTalent( new Magic.ImpactTheoryTalent( this, serviceProvider ) );
            AddTalent( new Magic.AppliedImpactResearchTalent( this, serviceProvider ) );
            AddTalent( new Magic.FirevortexTalent( this, serviceProvider ) );
            AddTalent( new Magic.RazorWindsTalent( this, serviceProvider ) );
            AddTalent( new Magic.MagicalBalanceTalent( this, serviceProvider ) );
            AddTalent( new Magic.PyromaniaTalent( this, serviceProvider ) );
            AddTalent( new Magic.FirewallTalent( this, serviceProvider ) );
            AddTalent( new Magic.PiercingFireTalent( this, serviceProvider ) );
            AddTalent( new Magic.FlamesOfPhlegethonTalent( this, serviceProvider ) );                       
                       
            // Support Talents:
            this.supportRoot = new Support.ConcentrateOnTheFactsTalent( this, serviceProvider );

            AddTalent( supportRoot );
            AddTalent( new Support.MeditationTalent( this, serviceProvider ) );
            AddTalent( new Support.ManaHullTalent( this, serviceProvider ) );   
            AddTalent( new Support.LuckyBastardTalent( this, serviceProvider ) );
            AddTalent( new Support.AngelEmbracementTalent( this, serviceProvider ) );
            AddTalent( new Support.PotionMasteryTalent( this, serviceProvider ) );
            AddTalent( new Support.PoisonMasteryTalent( this, serviceProvider ) );
            AddTalent( new Support.CriticalBalanceTalent( this, serviceProvider ) );
            AddTalent( new Support.SwiftFightStyleTalent( this, serviceProvider ) );
            AddTalent( new Support.SmoothedEmblazonmentTalent( this, serviceProvider ) );
            AddTalent( new Support.RackingPainsTalent( this, serviceProvider ) );              

            // Setup the graph connections between the talents:
            foreach( Talent talent in talents.Values )
            {
                talent.SetupNetwork();
            }
        }

        /// <summary>
        /// Adds the given Talent to the dictionary of all known talents.
        /// </summary>
        /// <param name="talent">
        /// The Talent to add.
        /// </param>
        private void AddTalent( Talent talent )
        {
            this.talents.Add( talent.GetType(), talent );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to get the Talent of the given Type.
        /// </summary>
        /// <param name="type">
        /// The type of the talent to get.
        /// </param>
        /// <returns>
        /// The Talent of the requested Type.
        /// </returns>
        public Talent GetTalent( Type type )
        {
            return this.talents[type];
        }

        /// <summary>
        /// Tries to get the Talent of the given Type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the talent to get.
        /// </typeparam>
        /// <returns>
        /// The Talent of the requested Type.
        /// </returns>
        internal T GetTalent<T>()
            where T : Talent
        {
            return this.GetTalent( typeof( T ) ) as T;
        }

        /// <summary>
        /// Tries to invest one talent point into the Talent of the given Type.
        /// </summary>
        /// <param name="type">
        /// The type of the talent to invest in.
        /// </param>
        /// <returns>Whether any point was invested into the talent.</returns>
        public bool InvestInto( Type type )
        {
            if( this.freeTalentPoints <= 0 )
                return false;

            Talent talent = this.talents[type];

            if( talent.FulfillsRequirements() )
            {
                if( talent.Level < talent.MaximumLevel )
                {
                    talent.GainLevel();
                    --freeTalentPoints;

                    this.OnTalentChanged();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Increases the amount of free talent points based on the new level of the Player.
        /// This is called on a levelup.
        /// </summary>
        internal void GainTalentPointsOnLevelUp()
        {
            int level = this.player.Statable.Level;

            if( level % 10 == 0 )
            {
                this.freeTalentPoints += 3;
            }
            else if( level % 5 == 0 )
            {
                this.freeTalentPoints += 2;
            }
            else
            {
                this.freeTalentPoints += 1;
            }
        }

        /// <summary>
        /// Resets the talents that the player has learned so far.
        /// </summary>
        internal void Reset()
        {
            int investedPoints = this.statistics.TotalInvestedPoints;

            foreach( var talent in this.talents.Values )
            {
                talent.Reset();
            }

            this.freeTalentPoints += investedPoints;
            OnTalentChanged();
        }

        /// <summary>
        /// Raises the TalentsChanged event.
        /// </summary>
        private void OnTalentChanged()
        {
            this.TalentsChanged.Raise( this );
        }
      
        #region > Storage <

        /// <summary>
        /// Serializes/Writes the data of this <see cref="TalentTree"/> 
        /// to the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );
            context.Write( freeTalentPoints );

            List<Talent> learnedTalents = new List<Talent>();
            foreach( Talent talent in this.talents.Values )
            {
                if( talent.Level > 0 )
                    learnedTalents.Add( talent );
            }

            context.Write( learnedTalents.Count );
            foreach( Talent learnedTalent in learnedTalents )
            {
                context.Write( learnedTalent.GetType().GetTypeName() );
                context.Write( learnedTalent.Level );
            }
        }

        /// <summary>
        /// Deserializes/Reads the data of this <see cref="TalentTree"/> 
        /// to the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.freeTalentPoints = context.ReadInt32();

            // Reset all talents:
            foreach( Talent talent in talents.Values )
            {
                if( talent.Level != 0 )
                    talent.SetLevel( 0 );
            }

            int learnedTalentCount = context.ReadInt32();

            // Set learned talents:
            for( int i = 0; i < learnedTalentCount; ++i )
            {
                // Receive talent type:
                string talentTypeName = context.ReadString();
                int    talentLevel    = context.ReadInt32();

                Type talentType = Type.GetType( talentTypeName );

                if( talentType != null )
                {
                    Talent talent = this.GetTalent( talentType );
                    talent.SetLevel( talentLevel );
                }
                else
                {
                    // This naturally happens when a talent
                    // gets removed from the game.
                    // As a counter-act refund the talent points.
                    this.freeTalentPoints += talentLevel;

                    // Log error.
                    log.WriteLine(
                        Atom.Diagnostics.LogSeverities.Error,
                        string.Format(
                            System.Globalization.CultureInfo.InvariantCulture,
                            "TALENT ERROR: The talent '{0}' couldn't be found. Refunded '{1}' talent points.",
                            talentTypeName,
                            talentLevel
                        )
                    );
                }
            }

            this.AnalyzeInvestmentCorrectness();
            this.OnTalentChanged();
        }

        /// <summary>
        /// Analyzes the correctness of the talent tree investments of the player.
        /// This is required to be run when loading the TalentTree.
        /// </summary>
        private void AnalyzeInvestmentCorrectness()
        {
            foreach( var talent in this.talents.Values )
            {
                int talentLevel = talent.Level;

                if( talentLevel > 0 )
                {
                    if( !talent.FulfillsRequirements() )
                    {
                        // This happens when a pre-requirement of a talent 
                        // has been removed or changed.
                        this.freeTalentPoints += talentLevel;
                        talent.SetLevel( 0 );

                        log.WriteLine(
                            Atom.Diagnostics.LogSeverities.Error,
                            string.Format(
                                System.Globalization.CultureInfo.InvariantCulture,
                                "TALENT ERROR: '{0}' talent points have been invested into the talent '{1}', " +
                                "but the talent requirements were not fulfilled. Refunded the talent points.",
                                talentLevel,
                                talent.GetType().GetTypeName()
                            )
                        );

                        // Restart analysis.
                        this.AnalyzeInvestmentCorrectness();
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of free talent points.
        /// </summary>
        private int freeTalentPoints = 0;

        /// <summary>
        /// The Player that owns the <see cref="TalentTree"/>.
        /// </summary>
        private readonly Entities.PlayerEntity player;

        /// <summary>
        /// The root of the sub-tree that contains melee oriented talents.
        /// </summary>
        private Talent meleeRoot;

        /// <summary>
        /// The root of the sub-tree that contains ranged oriented talents.
        /// </summary>
        private Talent rangedRoot;

        /// <summary>
        /// The root of the sub-tree that contains magic oriented talents.
        /// </summary>
        private Talent magicRoot;

        /// <summary>
        /// The root of the sub-tree that contains talents that support the other tree trees.
        /// </summary>
        private Talent supportRoot;

        /// <summary>
        /// All available talents sorted by their Type.
        /// </summary>
        private readonly Dictionary<Type, Talent> talents = new Dictionary<Type, Talent>( 25 );

        /// <summary>
        /// Provides a mechanism that allows logging of information.
        /// </summary>
        private readonly Atom.Diagnostics.ILog log;

        /// <summary>
        /// Captures statistics about this TalentTree.
        /// </summary>
        private readonly TalentTreeStatistics statistics;

        #endregion
    }
}
