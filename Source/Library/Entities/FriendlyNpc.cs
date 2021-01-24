// <copyright file="FriendlyNpc.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.FriendlyNpc class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Diagnostics;
    using Zelda.Entities.Components;
    using Zelda.Factions;
    using Zelda.Quests;

    /// <summary>
    /// Represents a Non Player Character that is friendly towards the player.
    /// <para>
    /// As such the player may talk to the NPC and receive quests from the NPC.
    /// </para>
    /// </summary>
    public class FriendlyNpc : ZeldaEntity, IUseable, IMoveableEntity, IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Faction this <see cref="FriendlyNpc"/> is part of.
        /// </summary>
        public Faction Faction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Talkable"/> component of this FriendlyNpc.
        /// </summary>
        public Talkable Talkable
        {
            get
            {
                return this.talkable; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Moveable"/> component of this FriendlyNpc.
        /// </summary>
        public Moveable Moveable
        {
            get
            {
                return this.moveable; 
            }
        }        

        /// <summary>
        /// Gets the <see cref="QuestsGiveable"/> component of this FriendlyNpc.
        /// </summary>
        public QuestsGiveable QuestsGiveable
        {
            get 
            {
                return this.questsGiveable;
            }
        }        

        /// <summary>
        /// Gets the <see cref="Visionable"/> component of this FriendlyNpc.
        /// </summary>
        public Visionable Visionable
        {
            get
            {
                return this.visionable; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Behaveable"/> component of this FriendlyNpc.
        /// </summary>
        public Behaveable Behaveable
        {
            get
            {
                return this.behaveable;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the FriendlyNpc class.
        /// </summary>
        public FriendlyNpc()
            : base( 8 )
        {
            this.talkable = new Talkable();
            this.visionable = new Visionable();
            this.behaveable = new Behaveable();
            this.moveable = new Moveable() { CanBePushed = false };
            this.questsGiveable = new QuestsGiveable();

            this.Components.BeginSetup();
            {
                this.Components.Add( talkable );
                this.Components.Add( moveable );
                this.Components.Add( visionable );
                this.Components.Add( behaveable );
                this.Components.Add( questsGiveable );
            }
            this.Components.EndSetup();
        }

        /// <summary>
        /// Setups this FriendlyNpc entity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.questsGiveable.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Talks to this FriendlyNpc.
        /// </summary>
        /// <param name="user">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the PlayerEntity has used this FriendlyNpc;
        /// otherwise false.
        /// </returns>
        public virtual bool Use( PlayerEntity user )
        {
            if( !this.Collision.Intersects( user.Collision ) )
                return false;
            if( this.Scene.UserInterface.Dialog.IsEnabled )
                return false;

            if( this.AnalyzeAndLoadQuests( user ) )
                return true;
            return this.ShowUncompletedQuestsOrDefaultText( user );
        }

        /// <summary>
        /// Loads and analyzes the quests that are relevant to this FriendlyNpc.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that is talking to this FriendlyNpc.
        /// </param>
        /// <returns>
        /// true if the player is talking to the npc;
        /// otherwise false.
        /// </returns>
        protected bool AnalyzeAndLoadQuests( PlayerEntity user )
        {
            this.questsGiveable.LoadRelevantQuests( user.QuestLog );

            if( this.AnalyzeCompletedQuests( user.QuestLog ) )
                return true;

            if( this.AnalyzeNpcQuests( user ) )
                return true;

            return false;
        }

        /// <summary>
        /// Shows either the quest related to any uncompleted quests that
        /// have its origin at this FriendlyNpc or shows the default text of
        /// this FriendlyNpc.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that is talking to this FriendlyNpc.
        /// </param>
        /// <returns>
        /// true if the player is talking to the npc;
        /// otherwise false.
        /// </returns>
        protected bool ShowUncompletedQuestsOrDefaultText( PlayerEntity user )
        {
            if( this.AnalyzeUncompletedQuests( user.QuestLog ) )
                return true;

            return this.ShowDefaultText( user );
        }

        /// <summary>
        /// Analyzes the quests of the player regarding
        /// whether he has any completed quests that can be turned
        /// in at this FriendlyNpc.
        /// </summary>
        /// <param name="questLog">
        /// The related quest-log.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the Player has talked to this Npc;
        /// otherwise <see langword="false"/>.
        /// </returns>
        private bool AnalyzeCompletedQuests( QuestLog questLog )
        {
            for( int i = 0; i < questLog.ActiveQuestCount; ++i )
            {
                Quest quest = questLog.GetActiveQuest( i );
                if( quest.DeliverType != QuestDeliverType.Npc )
                    continue;

                string deliverLoc = quest.DeliverLocation;

                if( deliverLoc.Equals( this.Name, StringComparison.Ordinal ) )
                {
                    // The quest may be turned in at this Npc.
                    if( quest.TurnIn() )
                    {
                        // The player is done with that quest!
                        this.ShowText( quest.LocalizedTextCompleted );
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// Analyzes the quests of the player regarding
        /// whether he has any quests that can be turned
        /// in at this FriendlyNpc but aren't completed yet.
        /// </summary>
        /// <param name="questLog">
        /// The related quest-log.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the Player has talked to this Npc;
        /// otherwise <see langword="false"/>.
        /// </returns>
        private bool AnalyzeUncompletedQuests( QuestLog questLog )
        {
            for( int i = 0; i < questLog.ActiveQuestCount; ++i )
            {
                Quest quest = questLog.GetActiveQuest( i );
                if( quest.DeliverType != QuestDeliverType.Npc )
                    continue;

                string deliverLoc = quest.DeliverLocation;

                if( deliverLoc.Equals( this.Name, StringComparison.Ordinal ) &&
                    !string.IsNullOrEmpty( quest.LocalizedTextNotCompleted ) )
                {
                    // The player is on that quest, but not done with it.
                    this.ShowText( quest.LocalizedTextNotCompleted );
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Analyzes the quests of this Friendly Npc.
        /// </summary>
        /// <param name="user">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the Player has talked to this Npc;
        /// otherwise false.
        /// </returns>
        private bool AnalyzeNpcQuests( PlayerEntity user )
        {
            var questLog = user.QuestLog;

            foreach( var quest in this.questsGiveable.Quests )
            {
                if( quest.FulfillsRequirements( user ) )
                {
                    if( !questLog.HasCompletedQuest( quest.Name ) &&
                        !questLog.HasActiveQuest( quest.Name ) )
                    {
                        var dialog = this.Scene.UserInterface.Dialog;

                        this.questAccept = quest;
                        dialog.Ended += this.Dialog_AcceptQuestEnded;
                        dialog.Show( quest.LocalizedTextStart );

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Shows the default text of this friendly npc.
        /// </summary>
        /// <param name="user">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the Player has talked to this Npc;
        /// otherwise <see langword="false"/>.
        /// </returns>
        protected bool ShowDefaultText( PlayerEntity user )
        {
            var factionState = user.FactionStates.GetState( this.Faction.Name );
            var defaultText  = talkable.GetText( factionState.ReputationLevel, this.Scene.Rand );

            if( defaultText != null )
            {
                this.ShowText( defaultText );
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Called when the player ends the <see cref="Zelda.UI.Dialog"/>
        /// which presents the user the quest.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void Dialog_AcceptQuestEnded( object sender, EventArgs e )
        {
            if( this.questAccept == null )
                return;

            this.questAccept.Accept( this.Scene.Player );
            this.questAccept = null;

            // Unregister dialog event.
            var dialog = this.Scene.UserInterface.Dialog;
            dialog.Ended -= this.Dialog_AcceptQuestEnded;
        }

        /// <summary>
        /// Helper method that shows
        /// some text using a Dialog.
        /// </summary>
        /// <param name="text">
        /// The text to show.
        /// </param>
        protected void ShowText( string text )
        {
            this.Scene.UserInterface.Dialog.Show( text );
        }

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this FriendlyNpc entity.
        /// </summary>
        /// <returns>
        /// The cloned ZeldaEntity.
        /// </returns>
        public override ZeldaEntity Clone()
        {
            FriendlyNpc clone = new FriendlyNpc();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given FriendlyNpc entity to be a clone of this FriendlyNpc.
        /// </summary>
        /// <param name="clone">
        /// The FriendlyNpc to setup as a clone of this FriendlyNpc.
        /// </param>
        public void SetupClone( FriendlyNpc clone )
        {
            base.SetupClone( clone );

            clone.Faction = this.Faction;

            this.behaveable.SetupClone( clone.behaveable );
            this.visionable.SetupClone( clone.visionable );
            this.talkable.SetupClone( clone.talkable );
            this.moveable.SetupClone( clone.moveable );
            this.questsGiveable.SetupClone( clone.questsGiveable );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the quest the player is currently accepting.
        /// </summary>
        private Quest questAccept;

        /// <summary>
        /// Identifies the <see cref="Talkable"/> component of this <see cref="FriendlyNpc"/> entity.
        /// </summary>
        private readonly Talkable talkable;

        /// <summary>
        /// Identifies the <see cref="QuestsGiveable"/> component of this <see cref="FriendlyNpc"/> entity.
        /// </summary>
        private readonly QuestsGiveable questsGiveable;

        /// <summary>
        /// Identifies the <see cref="Behaveable"/> component of this <see cref="FriendlyNpc"/> entity.
        /// </summary>
        private readonly Behaveable behaveable;

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of this <see cref="FriendlyNpc"/> entity.
        /// </summary>
        private readonly Moveable moveable;

        /// <summary>
        /// Identifies the <see cref="Visionable"/> component of this <see cref="FriendlyNpc"/> entity.
        /// </summary>
        private readonly Visionable visionable;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="FriendlyNpc"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<FriendlyNpc>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services. 
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( FriendlyNpc entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                // Write Name:
                context.Write( entity.Name );

                // Write Collision component
                entity.Collision.Serialize( context );

                // Write Moveable component
                entity.moveable.Serialize( context );

                // Write Behaveable component
                entity.behaveable.Serialize( context );

                // Write Visionable component
                entity.visionable.Serialize( context );

                // Read Talkable component
                entity.talkable.Serialize( context );

                // Read QuestsGiveable component
                entity.questsGiveable.Serialize( context );

                // Write DDaS
                if( entity.DrawDataAndStrategy != null )
                {
                    context.Write( Drawing.DrawStrategyManager.GetName( entity.DrawDataAndStrategy ) );
                    entity.DrawDataAndStrategy.Serialize( context );
                }
                else
                {
                    context.Write( string.Empty );
                }

                if( entity.Faction != null )
                {
                    context.Write( entity.Faction.Name );
                }
                else
                {
                    context.Write( string.Empty );
                }
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( FriendlyNpc entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

                // Read Name
                entity.Name = context.ReadString();

                // Read Collision component
                entity.Collision.Deserialize( context );

                // Read Moveable component
                entity.moveable.Deserialize( context );
                entity.moveable.CanBePushed = false;

                // Read Behaveable component
                entity.behaveable.Deserialize( context );

                // Read Visionable component
                entity.visionable.Deserialize( context );

                // Read Talkable component
                entity.talkable.Deserialize( context );

                // Read QuestsGiveable component
                entity.questsGiveable.Deserialize( context );

                // Read draw data and strategy:
                string ddsName = context.ReadString();

                // -- need to refactor this --
                if( ddsName.Length != 0 )
                {
                    var dds = serviceProvider.DrawStrategyManager.GetStrategyClone( ddsName, entity );
                    dds.Deserialize( context );

                    try
                    {
                        dds.Load( serviceProvider );
                    }
                    catch( System.IO.FileNotFoundException exc )
                    {
                        serviceProvider.Log.WriteLine( exc.ToString() );
                    }

                    entity.DrawDataAndStrategy = dds;
                }

                string factionName = context.ReadString();

                if( factionName.Length != 0 )
                {
                    entity.Faction = Factions.FactionList.Get( factionName );
                }
            }
        }

        #endregion
    }
}
