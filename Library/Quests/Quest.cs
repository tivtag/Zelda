// <copyright file="Quest.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Quests.Quest class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Quests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom;
    using Atom.Diagnostics;
    using Zelda.Entities;
    using Zelda.Core.Requirements;

    /// <summary>
    /// Represents a quest of the player.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// Quests drive the story and npc interaction in the game.
    /// </remarks>
    public sealed class Quest
    {
        #region [ Events ]

        /// <summary>
        /// Called when the player has accomplished all goals of this Quest.
        /// </summary>
        public event EventHandler Accomplished;

        #endregion

        #region [ Constants ]

        /// <summary>
        /// The extension of the quest definition file. ".zq"
        /// </summary>
        public const string Extension = ".zq";

        #endregion

        #region [ Properties ]

        #region > Settings <

        /// <summary>
        /// Gets or sets the type of this Quest.
        /// </summary>
        public QuestType QuestType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the object/location this Quest must be delivered at.
        /// </summary>
        /// <remarks>
        /// The value of this property is interpeted depending of the set <see cref="DeliverType"/>.
        /// </remarks>
        public string DeliverLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating how or where
        /// the player gets this Quest.
        /// </summary>
        public QuestDeliverType DeliverType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Quest is repeatable.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool IsRepeatable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the level of this Quest.
        /// </summary>
        /// <remarks>
        /// The level of the Quest identifies the difficulty of the Quest.
        /// </remarks>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state of the goals
        /// of this Quest is hidden from the user.
        /// </summary>
        public bool IsStateHidden
        {
            get;
            set;
        }

        #endregion

        #region > State <

        /// <summary>
        /// Gets a value indicating whether the player has turned this Quest in.
        /// </summary>
        public bool IsTurnedIn
        {
            get
            {
                return this.delivered;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this quest has been accepted by the player
        /// and is currently ongoing.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.player != null;
            }
        }

        /// <summary>
        /// Gets the state of this Quest in percentage.
        /// </summary>
        public float State
        {
            get
            {
                if( goals.Count == 0 )
                    return 1.0f;

                float state = 0.0f;

                foreach( var goal in this.goals )
                    state += goal.State;

                state /= goals.Count;

                return state;
            }
        }

        #endregion

        #region > Enumerations <

        /// <summary>
        /// Gets a read-only enumeration over the <see cref="IRequirement"/>s of this Quest.
        /// </summary>
        public IEnumerable<IRequirement> Requirements
        {
            get
            {
                return this.requirements.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a read-only enumeration over the <see cref="IQuestGoal"/>s of this Quest.
        /// </summary>
        public IEnumerable<IQuestGoal> Goals
        {
            get
            {
                return this.goals.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a read-only enumeration over the <see cref="IQuestReward"/>s of this Quest.
        /// </summary>
        public IEnumerable<IQuestReward> Rewards
        {
            get
            {
                return this.rewards.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a read-only enumeration over the <see cref="IQuestEvent"/>s
        /// that are executed when the player has accepted this Quest.
        /// </summary>
        public IEnumerable<IQuestEvent> StartEvents
        {
            get
            {
                return this.startEvents.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a read-only enumeration over the <see cref="IQuestEvent"/>s
        /// that are executed after the player has completed this Quest.
        /// </summary>
        public IEnumerable<IQuestEvent> CompletionEvents
        {
            get
            {
                return this.completionEvents.AsReadOnly();
            }
        }

        #endregion

        #region > Strings <

        /// <summary>
        /// Gets or sets the name that uniquely identifies this Quest.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value ?? string.Empty;

                try
                {
                    this.LocalizedName = QuestResources.ResourceManager.GetString( "QN_" + this.Name );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedName = this.Name;
                }
            }
        }

        /// <summary>
        /// Gets the localized name of this Quest.
        /// </summary>
        public string LocalizedName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Resource Id used to receive 
        /// the text that is shown as an introduction to this Quest.
        /// </summary>
        public string TextStart
        {
            get
            {
                return this.textStart;
            }

            set
            {
                this.textStart = value ?? string.Empty;

                try
                {
                    this.LocalizedTextStart = QuestResources.ResourceManager.GetString( this.TextStart );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedTextStart = this.TextStart;
                }
            }
        }

        /// <summary>
        /// Gets the localized text that shown at the start of this Quest.
        /// </summary>
        public string LocalizedTextStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Resource Id used to receive 
        /// the localized description of this Quest.
        /// </summary>
        public string TextDescription
        {
            get
            {
                return this.textDescription;
            }

            set
            {
                this.textDescription = value ?? string.Empty;

                try
                {
                    this.LocalizedTextDescription = QuestResources.ResourceManager.GetString( this.TextDescription );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedTextDescription = this.TextDescription;
                }
            }
        }

        /// <summary>
        /// Gets the localized description of this Quest.
        /// </summary>
        public string LocalizedTextDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Resource Id used to receive 
        /// the text that is shown when this Quest 
        /// has not been completed yet.
        /// </summary>
        public string TextNotCompleted
        {
            get
            {
                return this.textNotCompleted;
            }

            set
            {
                this.textNotCompleted = value ?? string.Empty;

                try
                {
                    this.LocalizedTextNotCompleted = QuestResources.ResourceManager.GetString( this.TextNotCompleted );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedTextNotCompleted = this.TextNotCompleted;
                }
            }
        }

        /// <summary>
        /// Gets the localized text that shown when this Quest
        /// has not been completed yet .
        /// </summary>
        public string LocalizedTextNotCompleted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Resource Id used to receive 
        /// the text that is shown when this Quest ends.
        /// </summary>
        public string TextCompleted
        {
            get
            {
                return this.textCompleted;
            }

            set
            {
                this.textCompleted = value ?? string.Empty;

                try
                {
                    this.LocalizedTextCompleted = QuestResources.ResourceManager.GetString( this.TextCompleted );
                }
                catch( System.InvalidOperationException )
                {
                    this.LocalizedTextCompleted = this.TextCompleted;
                }
            }
        }

        /// <summary>
        /// Gets the localized text that shown when this Quest ends.
        /// </summary>
        public string LocalizedTextCompleted
        {
            get;
            private set;
        }

        #endregion

        #region > Misc <

        /// <summary>
        /// Gets a value indicating how many different goals this Quest has.
        /// </summary>
        public int GoalCount
        {
            get
            {
                return this.goals.Count;
            }
        }

        /// <summary>
        /// Gets the PlayerEntity that currently does this Quest.
        /// </summary>
        internal PlayerEntity Player
        {
            get
            {
                return this.player;
            }
        }

        #endregion

        #endregion

        #region [ Initialisation ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class.
        /// </summary>
        public Quest()
        {
            this.onGoalStateChanged = new EventHandler( OnGoalStateChanged );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to accept this Quest for the given player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the player has started the quest;
        /// otherwise false.
        /// </returns>
        public bool Accept( PlayerEntity player )
        {
            if( player == null )
                throw new ArgumentNullException( "player" );

            if( !CanAccept( player ) )
                return false;

            player.QuestLog.AddActiveQuest( this );
            player.QuestLog.OnQuestAccepted( this );

            this.OnQuestAccepted( player );
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this quest can currently
        /// be accepted by the given player.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the player can currently start the quest; -or otherwise false.
        /// </returns>
        public bool CanAccept( PlayerEntity player )
        {
            if( this.IsActive || player.QuestLog.HasActiveQuest( this.name ) )
                return false;

            if( !CanEverAccept( player ) )
                return false;

            if( !this.FulfillsRequirements( player ) )
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the given player is expected to ever be able accept this quest.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if it will ever again be able to be accepted.
        /// </returns>
        public bool CanEverAccept( PlayerEntity player )
        {
            if( IsRepeatable )
            {
                return true;
            }
            else
            {
                bool ongoingOrCompleted = this.IsActive || player.QuestLog.HasCompletedQuest( this.Name );
                return !ongoingOrCompleted;
            }
        }

        /// <summary>
        /// Called when this quest has been accepted.
        /// </summary>
        /// <param name="player">
        /// The related player entity.
        /// </param>
        internal void SetupOnQuestAccepted( PlayerEntity player )
        {
            foreach( var goal in this.goals )
                goal.OnAccepted( player );

            this.player = player;
            this.oldAccomplishedGoalsState = this.HasAccomplishedGoals();
        }

        /// <summary>
        /// Called when this quest has been accepted.
        /// </summary>
        /// <param name="player">
        /// The related player entity.
        /// </param>
        private void OnQuestAccepted( PlayerEntity player )
        {
            this.SetupOnQuestAccepted( player );

            foreach( var e in this.startEvents )
            {
                e.Execute( this );
            }
        }

        /// <summary>
        /// Tries to turn in this Quest.
        /// </summary>
        /// <returns>
        /// true if this Quest has been turned-in;
        /// otherwise false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If the player has not accepted this quest.
        /// </exception>
        public bool TurnIn()
        {
            #region - Verify -

            if( player == null )
                throw new InvalidOperationException( Zelda.Resources.Error_QuestNotAccepted );

            if( this.IsTurnedIn )
                return false;

            #endregion

            if( !this.HasAccomplishedGoals() )
                return false;

            foreach( var goal in this.goals )
                goal.OnTurnIn( player );

            foreach( var reward in this.rewards )
                reward.Reward( player );

            this.delivered = true;

            // Notify
            foreach( var e in this.completionEvents )
                e.Execute( this );

            var log = this.player.QuestLog;
            log.OnQuestTurnedIn( this );

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the player fulfills 
        /// all <see cref="IRequirement"/>s to start to this Quest.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the player fulfills alls requirements;
        /// otherwise false.
        /// </returns>
        public bool FulfillsRequirements( PlayerEntity player )
        {
            foreach( var requirement in this.requirements )
            {
                if( !requirement.IsFulfilledBy( player ) )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the player has accomplished 
        /// all <see cref="IQuestGoal"/>s to finish to this Quest.
        /// </summary>
        /// <returns>
        /// true if the player has accomplished all goals;
        /// otherwise false.
        /// </returns>
        private bool HasAccomplishedGoals()
        {
            foreach( var goal in this.goals )
            {
                if( !goal.IsAccomplished( player ) )
                    return false;
            }

            return true;
        }

        #region - Add -

        /// <summary>
        /// Adds the given <see cref="IRequirement"/> to this Quest.
        /// </summary>
        /// <param name="requirement">
        /// The IQuestRequirement to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requirement"/> is null.
        /// </exception>
        public void AddRequirement( IRequirement requirement )
        {
            if( requirement == null )
                throw new ArgumentNullException( "requirement" );

            this.requirements.Add( requirement );
        }

        /// <summary>
        /// Adds the given <see cref="IQuestGoal"/> to this Quest.
        /// </summary>
        /// <param name="goal">
        /// The IQuestGoal to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="goal"/> is null.
        /// </exception>
        public void AddGoal( IQuestGoal goal )
        {
            if( goal == null )
                throw new ArgumentNullException( "goal" );

            // Hook events
            goal.StateChanged += this.onGoalStateChanged;

            this.goals.Add( goal );
        }

        /// <summary>
        /// Adds the given <see cref="IQuestReward"/> to this Quest.
        /// </summary>
        /// <param name="reward">
        /// The IQuestReward to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="reward"/> is null.
        /// </exception>
        public void AddReward( IQuestReward reward )
        {
            if( reward == null )
                throw new ArgumentNullException( "reward" );

            this.rewards.Add( reward );
        }

        /// <summary>
        /// Adds the given <see cref="IQuestEvent"/> to the list of events that 
        /// get called when this Quest has been accepted.
        /// </summary>
        /// <param name="e">
        /// The IQuestEvent to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="e"/> is null.
        /// </exception>
        public void AddStartEvent( IQuestEvent e )
        {
            if( e == null )
                throw new ArgumentNullException( "e" );

            this.startEvents.Add( e );
        }

        /// <summary>
        /// Adds the given <see cref="IQuestEvent"/> to the list of events that 
        /// get called when this Quest has been completed.
        /// </summary>
        /// <param name="e">
        /// The IQuestEvent to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="e"/> is null.
        /// </exception>
        public void AddCompletionEvent( IQuestEvent e )
        {
            if( e == null )
                throw new ArgumentNullException( "e" );

            this.completionEvents.Add( e );
        }

        #endregion

        #region - Remove -

        /// <summary> 
        /// Tries to remove the given <see cref="IRequirement"/> from this Quest.
        /// </summary>
        /// <param name="requirement">
        /// The IQuestRequirement to remove.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="requirement"/> has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveRequirement( IRequirement requirement )
        {
            return this.requirements.Remove( requirement );
        }

        /// <summary> 
        /// Tries to remove the given <see cref="IQuestGoal"/> from this Quest.
        /// </summary>
        /// <param name="goal">
        /// The IQuestGoal to remove.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="goal"/> has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveGoal( IQuestGoal goal )
        {
            return this.goals.Remove( goal );
        }

        /// <summary> 
        /// Tries to remove the given <see cref="IQuestReward"/> from this Quest.
        /// </summary>
        /// <param name="reward">
        /// The IQuestReward to remove.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="reward"/> has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveReward( IQuestReward reward )
        {
            return this.rewards.Remove( reward );
        }

        /// <summary> 
        /// Tries to remove the given <see cref="IQuestEvent"/> from this Quest.
        /// </summary>
        /// <param name="e">
        /// The IQuestCompletedEvent to remove.
        /// </param>
        /// <returns>
        /// true if the given <see cref="IQuestEvent"/> has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveStartEvent( IQuestEvent e )
        {
            return this.startEvents.Remove( e );
        }

        /// <summary> 
        /// Tries to remove the given <see cref="IQuestEvent"/> from this Quest.
        /// </summary>
        /// <param name="e">
        /// The IQuestCompletedEvent to remove.
        /// </param>
        /// <returns>
        /// true if the given <see cref="IQuestEvent"/> has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveCompletionEvent( IQuestEvent e )
        {
            return this.completionEvents.Remove( e );
        }

        #endregion

        #region > Utility <

        /// <summary>
        /// Utility method that returns the color associated
        /// with the difficulty of the quest.
        /// </summary>
        /// <param name="quest">
        /// The quest.
        /// </param>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <returns>
        /// The associated Color.
        /// </returns>
        public static Microsoft.Xna.Framework.Color GetQuestColor( Quest quest, PlayerEntity player )
        {
            int questLevel = quest.Level;
            int playerLevel = player.Statable.Level;
            int delta = questLevel - playerLevel;

            // <5      Gray
            // -5 to 0 Green
            //  0 to 2 Yellow
            //  2 to 5 Orange
            //  >5     Red
            if( delta >= 0 )
            {
                if( delta <= 2 )
                    return Microsoft.Xna.Framework.Color.Yellow;// new Microsoft.Xna.Framework.Color( 255, 255, 100, 255 ); // Yellow;
                else if( delta <= 5 )
                    return Microsoft.Xna.Framework.Color.Orange;
                else
                    return Microsoft.Xna.Framework.Color.Red;
            }
            else
            {
                if( delta >= -5 )
                    return Microsoft.Xna.Framework.Color.LightGreen;
                else
                    return Microsoft.Xna.Framework.Color.Gray;
            }
        }

        #endregion

        #region > Events <

        /// <summary>
        /// Gets called when the state of one of the <see cref="IQuestGoal"/>s of this Quest has changed. 
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnGoalStateChanged( object sender, EventArgs e )
        {
            if( this.HasAccomplishedGoals() )
            {
                if( this.DeliverType == QuestDeliverType.Instant )
                {
                    this.TurnIn();
                }

                if( !this.oldAccomplishedGoalsState )
                {
                    this.OnAccomplishedGoals();
                }

                this.oldAccomplishedGoalsState = true;
            }
            else
            {
                this.oldAccomplishedGoalsState = false;
            }
        }

        /// <summary>
        /// Fires the Accomplished and QuestLog.QuestAccomplished events.
        /// </summary>
        private void OnAccomplishedGoals()
        {
            // Don't notify the quest has no actual goals.
            // Travel Quests for example.
            if( this.goals.Count == 0 )
                return;

            this.Accomplished.Raise( this );

            if( this.player != null )
                this.player.QuestLog.OnQuestGoalsAccomplished( this );
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes/Writes the data to descripe this Quest.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 6;
            context.Write( Version );

            context.Write( this.Name ?? string.Empty );
            context.Write( (int)this.QuestType );
            context.Write( this.Level );
            context.Write( this.IsRepeatable );
            context.Write( this.IsStateHidden );

            context.Write( (int)this.DeliverType );
            context.Write( this.DeliverLocation ?? string.Empty );

            context.Write( this.TextStart ?? string.Empty );
            context.Write( this.TextDescription ?? string.Empty );
            context.Write( this.TextNotCompleted ?? string.Empty );
            context.Write( this.TextCompleted ?? string.Empty );

            // Write Requirements
            context.Write( this.requirements.Count );
            foreach( var requirement in this.requirements )
            {
                context.Write( requirement.GetType().GetTypeName() );
                requirement.Serialize( context );
            }

            // Write Goals
            context.Write( this.goals.Count );
            foreach( var goal in this.goals )
            {
                context.Write( goal.GetType().GetTypeName() );
                goal.Serialize( context );
            }

            // Write Rewards
            context.Write( this.rewards.Count );
            foreach( var reward in this.rewards )
            {
                context.Write( reward.GetType().GetTypeName() );
                reward.Serialize( context );
            }

            // Write Start Events
            context.Write( this.startEvents.Count );
            foreach( var e in this.startEvents )
            {
                context.Write( e.GetType().GetTypeName() );
                e.Serialize( context );
            }

            // Write Completion Events
            context.Write( this.completionEvents.Count );
            foreach( var e in this.completionEvents )
            {
                context.Write( e.GetType().GetTypeName() );
                e.Serialize( context );
            }
        }

        /// <summary>
        /// Deserializes/Reads the data to descripe this Quest.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 6;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 4, Version, this.GetType() );

            var serviceProvider = context.ServiceProvider;
            this.Name = context.ReadString();
            this.QuestType = (QuestType)context.ReadInt32();
            this.Level = context.ReadInt32();
            this.IsRepeatable = context.ReadBoolean();

            if( version >= 6 )
            {
                this.IsStateHidden = context.ReadBoolean();
            }

            this.DeliverType = (QuestDeliverType)context.ReadInt32();
            this.DeliverLocation = context.ReadString();

            this.TextStart = context.ReadString();
            this.TextDescription = context.ReadString();
            this.TextNotCompleted = context.ReadString();
            this.TextCompleted = context.ReadString();

            // Read Requirements
            int requirementCount = context.ReadInt32();

            requirements.Clear();
            requirements.Capacity = requirementCount;

            for( int i = 0; i < requirementCount; ++i )
            {
                string typeName = context.ReadString();
                Type type = Type.GetType( typeName );

                var requirement = (IRequirement)Activator.CreateInstance( type );

                var setupable = requirement as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );

                requirement.Deserialize( context );
                this.AddRequirement( requirement );
            }

            // Read Goals
            int goalCount = context.ReadInt32();

            goals.Clear();
            goals.Capacity = goalCount;

            for( int i = 0; i < goalCount; ++i )
            {
                string typeName = context.ReadString();
                Type type = Type.GetType( typeName );

                var goal = (IQuestGoal)Activator.CreateInstance( type );

                var setupable = goal as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );

                goal.Deserialize( context );

                this.AddGoal( goal );
            }

            // Read Rewards
            int rewardCount = context.ReadInt32();

            rewards.Clear();
            rewards.Capacity = rewardCount;

            for( int i = 0; i < rewardCount; ++i )
            {
                string typeName = context.ReadString();
                Type type = Type.GetType( typeName );

                var reward = (IQuestReward)Activator.CreateInstance( type );

                var setupable = reward as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );

                reward.Deserialize( context );
                this.AddReward( reward );
            }

            // Read Start Events
            if( version >= 5 )
            {
                int startEventCount = context.ReadInt32();

                startEvents.Clear();
                startEvents.Capacity = startEventCount;

                for( int i = 0; i < startEventCount; ++i )
                {
                    string typeName = context.ReadString();
                    Type type = Type.GetType( typeName );
                    var startEvent = (IQuestEvent)Activator.CreateInstance( type );

                    var setupable = startEvent as IZeldaSetupable;
                    if( setupable != null )
                        setupable.Setup( serviceProvider );

                    startEvent.Deserialize( context );
                    this.AddStartEvent( startEvent );
                }
            }

            // Read Completion Events
            int completionEventCount = context.ReadInt32();

            completionEvents.Clear();
            completionEvents.Capacity = completionEventCount;

            for( int i = 0; i < completionEventCount; ++i )
            {
                string typeName = context.ReadString();
                Type type = Type.GetType( typeName );

                var completionEvent = (IQuestEvent)Activator.CreateInstance( type );

                var setupable = completionEvent as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );

                completionEvent.Deserialize( context );
                this.AddCompletionEvent( completionEvent );
            }
        }

        /// <summary>
        /// Serializes/Writes the current state of this Quest.
        /// </summary>
        /// <param name="context">
        /// The context under which the derialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int StateVersion = 1;
            context.Write( StateVersion );

            context.Write( goals.Count );
            foreach( var goal in goals )
            {
                // Write the typename of the goal to allow an easier implementation
                // of loading error recoveration at some point later.
                // This process would allow to change the goals of a quest
                // even after the game has been released to the public.
                context.Write( goal.GetType().GetTypeName() );

                goal.SerializeState( context );
            }
        }

        /// <summary>
        /// Deserializes/Reads the current state of this Quest.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int StateVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, StateVersion, this.GetType() );

            // Read the state of the goals of this quest:
            int goalCount = context.ReadInt32();
            Debug.Assert( goalCount == this.goals.Count, "Goal count mismatch." );

            for( int i = 0; i < goalCount; ++i )
            {
                string typeName = context.ReadString();
                var goal = goals[i];

                // Implement loading error recoveration here ...
                Debug.Assert( typeName == goal.GetType().GetTypeName() );

                goal.DeserializeState( context );
            }
        }

        /// <summary>
        /// Saves the given <see cref="Quest"/>.
        /// </summary>
        /// <param name="quest">
        /// The quest to save.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="quest"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the name of the given <paramref name="quest"/> has not been set.
        /// </exception>
        public static void Save( Quest quest, IZeldaServiceProvider serviceProvider )
        {
            if( quest == null )
                throw new ArgumentNullException( "quest" );
            if( string.IsNullOrEmpty( quest.Name ) )
                throw new ArgumentException( Zelda.Resources.Error_QuestNameNotSet, "quest" );

            const string Directory = "Content/Quests/";
            string path = Directory + quest.Name;

            using( var stream = new System.IO.FileStream(
                path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None ) )
            {
                using( var writer = new System.IO.BinaryWriter( stream ) )
                {
                    var context = new Zelda.Saving.SerializationContext( writer, serviceProvider );
                    quest.Serialize( context );
                }
            }
        }

        /// <summary>
        /// Loads the <see cref="Quest"/> with the given name.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the quest to load.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The loaded Quest.
        /// </returns>
        public static Quest Load( string name, IZeldaServiceProvider serviceProvider )
        {
            string path = "Content/Quests/" + name;
            if( !path.EndsWith( Extension, StringComparison.Ordinal ) )
                path += Extension;

            using( var reader = new System.IO.BinaryReader( System.IO.File.OpenRead( path ) ) )
            {
                Quest quest = new Quest();

                var context = new Zelda.Saving.DeserializationContext( reader, serviceProvider );
                quest.Deserialize( context );

                return quest;
            }
        }

        /// <summary>
        /// Tries to load the <see cref="Quest"/> with the given name.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the quest to load.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The loaded Quest or null if an error occured while loading.
        /// </returns>
        public static Quest TryLoad( string name, IZeldaServiceProvider serviceProvider )
        {
            try
            {
                return Quest.Load( name, serviceProvider );
            }
            catch( Exception exc )
            {
                if( serviceProvider != null )
                {
                    string headerMessage = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_ErrorLoadingQuestX,
                        name ?? string.Empty
                    );

                    var log = serviceProvider.Log;
                    log.WriteLine( headerMessage );
                    log.WriteLine( exc.ToString() );
                    log.WriteLine();
                }

                return null;
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The name that uniquely identifies this Quest.
        /// </summary>
        private string name;

        /// <summary>
        /// Stores the Resource Ids that are used to identify the text related to this Quest.
        /// </summary>
        private string textStart, textDescription, textNotCompleted, textCompleted;

        /// <summary>
        /// States whether the player has finished this Quest.
        /// </summary>
        private bool delivered;

        /// <summary>
        /// Records the old quest accomplishment state.
        /// </summary>
        /// <remarks>
        /// This is used to reduce the 'Quest X Completed' spam.
        /// </remarks>
        private bool oldAccomplishedGoalsState;

        /// <summary>
        /// Identifies the PlayerEntity that owns this Quest.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The list of requirements that must be fulfilled before this Quest is offered to the player.
        /// </summary>
        private readonly List<IRequirement> requirements = new List<IRequirement>();

        /// <summary>
        /// The list of <see cref="IQuestGoal"/>s.
        /// </summary>
        private readonly List<IQuestGoal> goals = new List<IQuestGoal>();

        /// <summary>
        /// The list of <see cref="IQuestReward"/>s.
        /// </summary>
        private readonly List<IQuestReward> rewards = new List<IQuestReward>();

        /// <summary>
        /// The <see cref="IQuestEvent"/>s that get executed when this Quest has been accepted.
        /// </summary>
        private readonly List<IQuestEvent> startEvents = new List<IQuestEvent>();

        /// <summary>
        /// The <see cref="IQuestEvent"/>s that get executed when this Quest has been completed.
        /// </summary>
        private readonly List<IQuestEvent> completionEvents = new List<IQuestEvent>();

        #region > Event Handler <

        /// <summary>
        /// Identifies the EventHandler that gets invoked when 
        /// the state of one of the <see cref="IQuestGoal"/>s of this Quest has changed.
        /// </summary>
        private readonly EventHandler onGoalStateChanged;

        #endregion

        #endregion
    }
}
