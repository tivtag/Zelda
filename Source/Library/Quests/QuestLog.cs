// <copyright file="QuestLog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.QuestLog class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Quests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    /// <summary>
    /// The QuestLog is responsible for holding the
    /// list of current and finished <see cref="Quest"/>s of the player.
    /// </summary>
    public sealed class QuestLog : Saving.ISaveable
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the player that owns this QuestLog
        /// has accepted a new <see cref="Quest"/>.
        /// </summary>
        public event Atom.RelaxedEventHandler<Quest> QuestAccepted;
        
        /// <summary>
        /// Fired when the player that owns this QuestLog
        /// has accomplished all goal of a <see cref="Quest"/>.
        /// </summary>
        public event Atom.RelaxedEventHandler<Quest> QuestAccomplished;        

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the amount of currently active <see cref="Quest"/>s.
        /// </summary>
        public int ActiveQuestCount
        {
            get { return this.activeQuests.Count; }
        }

        /// <summary>
        /// Gets the PlayerEntity that owns this QuestLog.
        /// </summary>
        public Entities.PlayerEntity Owner
        {
            get { return this.player; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the QuestLog class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new QuestLog.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal QuestLog( Zelda.Entities.PlayerEntity player, IZeldaServiceProvider serviceProvider )
        {
            Debug.Assert( player != null );
            Debug.Assert( serviceProvider != null );

            this.player          = player;
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the quest at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the quest to get.
        /// </param>
        /// <returns>
        /// The Quest at the given <paramref name="index"/>.
        /// </returns>
        public Quest GetActiveQuest( int index )
        {
            return this.activeQuests[index];
        }

        /// <summary>
        /// Gets a value indicating whether the player
        /// that owns this QuestLog has completed the quest
        /// with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the quest to look for.
        /// </param>
        /// <returns>
        /// true if the player that owns this QuestLog has completed 
        /// the quest with the given <paramref name="name"/>;
        /// otherwise false.
        /// </returns>
        public bool HasCompletedQuest( string name )
        {
            return this.completedQuests.Contains( name );
        }
        
        /// <summary>
        /// Gets a value indicating whether this QuestLog
        /// contains an active <see cref="Quest"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the Quest.
        /// </param>
        /// <returns>
        /// true if this QuestLog contains an active <see cref="Quest"/> with the given <paramref name="name"/>;
        /// otherwise false.
        /// </returns>
        public bool HasActiveQuest( string name )
        {
            return this.GetActiveQuest( name ) != null;
        }

        /// <summary>
        /// Tries to get the active <see cref="Quest"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the Quest to get.
        /// </param>
        /// <returns>
        /// The requested Quest;
        /// or null if this QuestLog doesn't contain any active Quest that has the given <paramref name="name"/>.
        /// </returns>
        public Quest GetActiveQuest( string name )
        {
            return this.activeQuests.Find(
                ( Quest quest ) => {
                    return quest.Name.Equals( name, StringComparison.Ordinal );
                }
            );
        }

        /// <summary>
        /// Tries to add the given <see cref="Quest"/>
        /// to the list of active quests to this <see cref="QuestLog"/>.
        /// </summary>
        /// <param name="quest">
        /// The <see cref="Quest"/> to add.
        /// </param>
        internal void AddActiveQuest( Quest quest )
        {
            if( quest == null )
                throw new ArgumentNullException( "quest" );
            if( quest.IsTurnedIn )
                throw new ArgumentException( Resources.Error_QuestIsCompleted, "quest" );
            if( activeQuests.Contains( quest ) )
                throw new InvalidOperationException( Resources.Error_QuestLogAlreadyContainsQuest );

            // Add to list
            this.activeQuests.Add( quest );
        }

        /// <summary>
        /// Fires the <see cref="QuestAccepted"/> events.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        internal void OnQuestAccepted( Quest quest )
        {
            if( this.QuestAccepted != null )
                this.QuestAccepted( this, quest );
        }

        /// <summary>
        /// Fires the <see cref="QuestAccomplished"/> events.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        internal void OnQuestGoalsAccomplished( Quest quest )
        {
            if( this.QuestAccomplished != null )
                this.QuestAccomplished( this, quest );
        }

        /// <summary>
        /// Called when a Quest has been turned in.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        internal void OnQuestTurnedIn( Quest quest )
        {
            Debug.Assert( quest.IsTurnedIn );
            this.AddCompletedQuest( quest );
            this.RemoveActiveQuest( quest );
        }

        /// <summary>
        /// Removes the given Quest from the list of currently active Quests.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        private void RemoveActiveQuest( Quest quest )
        {
            Debug.Assert( quest != null );
            this.activeQuests.Remove( quest );
        }

        /// <summary>
        /// Adds the given Quest to the list of completed Quests.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        private void AddCompletedQuest( Quest quest )
        {
            Debug.Assert( quest != null );
            this.AddCompletedQuest( quest.Name );
        }

        /// <summary>
        /// Adds the given quest-name to the list of completed Quests.
        /// </summary>
        /// <param name="questName">
        /// The name of the related Quest.
        /// </param>
        private void AddCompletedQuest( string questName )
        {
            Debug.Assert( questName != null );

            if( !this.completedQuests.Contains( questName ) )
            {
                this.completedQuests.Add( questName );
            }
        }

        #region > Storage <
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );

            // Write Completed Quests
            context.Write( completedQuests.Count );
            foreach( var questName in this.completedQuests )
                context.Write( questName );

            // Write Active Quests
            context.Write( activeQuests.Count );
            foreach( var quest in this.activeQuests )
            {
                context.Write( quest.Name );
                quest.SerializeState( context );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, Version, this.GetType() );

            // Read Completed Quests:
            int completedQuestCount = context.ReadInt32();

            completedQuests.Clear();
            completedQuests.Capacity = completedQuestCount;

            for( int i = 0; i < completedQuestCount; ++i )
                completedQuests.Add( context.ReadString() );

            // Read Active Quests:
            int activeQuestCount = context.ReadInt32();

            activeQuests.Clear();
            activeQuests.Capacity = activeQuestCount;

            for( int i = 0; i < activeQuestCount; ++i )
            {
                string questName = context.ReadString();

                Quest quest = Quest.Load( questName, serviceProvider );
                quest.DeserializeState( context );

                AddActiveQuest( quest );
                quest.SetupOnQuestAccepted( this.player );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the <see cref="Quest"/>s the player is currently doing.
        /// </summary>
        private readonly List<Quest> activeQuests = new List<Quest>();

        /// <summary>
        /// Stores the names of the <see cref="Quest"/>s the player has finished.
        /// </summary>
        private readonly List<string> completedQuests = new List<string>();

        /// <summary>
        /// Identifies the PlayerEntity that owns this QuestLog.
        /// </summary>
        private readonly Zelda.Entities.PlayerEntity player;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}