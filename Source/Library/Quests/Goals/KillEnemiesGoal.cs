// <copyright file="KillEnemiesGoal.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.Goals.KillEnemiesGoal class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests.Goals
{
    using System;
    using System.Globalization;
    using Atom;
    using Zelda.Entities;
    
    /// <summary>
    /// Defines an <see cref="IQuestGoal"/> that requires
    /// the player to kill N-amount enemies of type X.
    /// This class can't be inherited.
    /// </summary>
    public sealed class KillEnemiesGoal : IQuestGoal, IZeldaSetupable
    {
        #region [ Events ]

        /// <summary>
        /// Invoked when the state of this KillEnemiesGoal has changed.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the enemy that is required to be killed for this GetItemsGoal.
        /// </summary>
        public string EnemyName
        {
            get 
            {
                return this.enemyName;
            }

            set
            {
                this.enemyName = value;

                if( this.templateManager != null )
                {
                    this.EnemyTemplate = this.templateManager.GetTemplate( value );
                }
            }
        }

        /// <summary>
        /// Gets the template of the Enemy that needs to be killed for this KillEnemiesGoal.
        /// </summary>
        public IEntityTemplate EnemyTemplate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of enemies (of the same type) are required to be killed for this GetItemsGoal.
        /// </summary>
        public uint KillAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the state of this KillEnemiesGoal in percentage.
        /// </summary>
        public float State
        {
            get
            {
                return this.killedCount / (float)this.KillAmount;
            }
        }

        /// <summary>
        /// Gets a short (localized) string that descripes
        /// the current of this GetItemsGoal.
        /// </summary>
        public string StateDescription
        {
            get
            {
                // Query the number of items :)
                var killed = this.killedCount;
                if( killed > this.KillAmount )
                    killed = this.KillAmount;

                string name = this.EnemyTemplate != null ? this.EnemyTemplate.LocalizedName : this.EnemyName;

                if( KillAmount == 1 )
                {
                    return string.Format( 
                        CultureInfo.CurrentCulture,
                        QuestResources.QuestGoalDesc_KillEnemy,
                        name,
                        killed.ToString( CultureInfo.CurrentCulture )
                    );
                }
                else
                {
                    return string.Format( 
                        CultureInfo.CurrentCulture,
                        QuestResources.QuestGoalDesc_KillEnemies,
                        name,
                        killed.ToString( CultureInfo.CurrentCulture ),
                        this.KillAmount.ToString( CultureInfo.CurrentCulture )
                    );
                }
            }
        }   

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the KillEnemiesGoal class.
        /// </summary>
        public KillEnemiesGoal()
        {
            onEntityKilled = new Atom.RelaxedEventHandler<ZeldaEntity>( OnEntityKilled );
        }

        /// <summary>
        /// Setups this KillEnemiesGoal.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.templateManager = serviceProvider.EntityTemplateManager;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a value indicating whether the player has accomplished this KillEnemiesGoal.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        /// <returns>
        /// true if the given PlayerEntity has accomplished this IQuestGoal; 
        /// otherwise false.
        /// </returns>
        public bool IsAccomplished( PlayerEntity player )
        {
            return this.killedCount >= this.KillAmount;
        }

        /// <summary>
        /// Fired when the player has accepeted the quest this KillEnemiesGoal is related to.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void OnAccepted( PlayerEntity player )
        {
            player.EntityKilled += onEntityKilled;
        }

        /// <summary>
        /// Fired when the player has accomplished all goals (including this!) of a <see cref="Quest"/>.
        /// </summary>
        /// <param name="player">
        /// The related PlayerEntity.
        /// </param>
        public void OnTurnIn( PlayerEntity player )
        {
            player.EntityKilled -= onEntityKilled;
        }

        /// <summary>
        /// Gets called when the player has killed an enemy.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="entity">The ZeldaEntity that was killed.</param>
        private void OnEntityKilled( object sender, ZeldaEntity entity )
        {
            if( this.IsAccomplished( null ) )
                return;

            if( entity.Name.Equals( this.EnemyName, StringComparison.Ordinal ) )
            {
                ++this.killedCount;
                this.StateChanged.Raise( this );
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
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.EnemyName ?? string.Empty );
            context.WriteUnsigned( this.KillAmount );
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.EnemyName  = context.ReadString();
            this.KillAmount = context.ReadUInt32();
        }

        /// <summary>
        /// Serializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Saving.IStateSaveable.SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteUnsigned( this.killedCount );
        }

        /// <summary>
        /// Deserializes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        void Saving.IStateSaveable.DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            this.killedCount = context.ReadUInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The amount of enemies the player has killed so far.
        /// </summary>
        private uint killedCount;
         
        /// <summary>
        /// The storage field of the <see cref="EnemyName"/> property.
        /// </summary>
        private string enemyName;

        /// <summary>
        /// Provides a mechanism for loading the <see cref="EnemyTemplate"/>.
        /// </summary>
        private EntityTemplateManager templateManager;

        /// <summary>
        /// Stores the event handler that gets invoked when the player has killed an enemy.
        /// </summary>
        private readonly Atom.RelaxedEventHandler<ZeldaEntity> onEntityKilled;

        #endregion
    }    
}
