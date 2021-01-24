// <copyright file="QuestsGiveable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.QuestsGiveable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Zelda.Quests;

    /// <summary>
    /// Defines a <see cref="ZeldaComponent"/> that allows
    /// a <see cref="ZeldaEntity"/> to give out <see cref="Quest"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class QuestsGiveable : ZeldaComponent, Saving.ISaveable, IZeldaSetupable
    {
        #region [ Initialization ]

        /// <summary>
        /// Setups this QuestsGiveable component.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the list of names that uniquely identifiy the Quests this QuestsGiveable provides.
        /// </summary>
        public List<string> QuestNames
        {
            get 
            { 
                return questNames;
            }
        }

        /// <summary>
        /// Gets the (loaden) Quests this QuestsGiveable provides.
        /// </summary>
        public IEnumerable<Quest> Quests
        {
            get
            {
                return this.quests;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Loads all quests which are needed. 
        /// </summary>
        /// <remarks>
        /// Quests which already have been finished aren't loaded.
        /// </remarks>
        /// <param name="questLog">
        /// The <see cref="QuestLog"/> that stores all quest information.
        /// </param>
        public void LoadRelevantQuests( QuestLog questLog )
        {
            Debug.Assert( questLog != null );
            if( this.relevantQuestsLoaden )
                return;

            for( int i = 0; i < questNames.Count; ++i )
            {
                string questName = questNames[i];

                if( questLog.HasCompletedQuest( questName ) )
                    continue;

                Quest quest = questLog.GetActiveQuest( questName );
                if( quest == null )
                    quest = Quest.Load( questName, this.serviceProvider );

                quests.Add( quest );
            }

            this.relevantQuestsLoaden = true;
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given <see cref="QuestsGiveable"/> component
        /// to be a clone of this QuestsGiveable.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="QuestsGiveable"/> component
        /// to be a clone of this QuestsGiveable.
        /// </param>
        public void SetupClone( QuestsGiveable clone )
        {
            clone.Setup( this.serviceProvider );

            clone.questNames.Clear();
            clone.questNames.AddRange( this.questNames );
            clone.quests.Clear();
            clone.quests.AddRange( this.quests );

            clone.relevantQuestsLoaden = this.relevantQuestsLoaden;
        }

        #endregion

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

            context.Write( this.questNames.Count );
            foreach( var quest in this.questNames )
            {
                context.Write( quest );
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.questNames.Clear();
            this.quests.Clear();

            int questCount = context.ReadInt32();
            this.questNames.Capacity = questCount;

            for( int i = 0; i < questCount; ++i )
            {
                questNames.Add( context.ReadString() );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The names that uniquely identify the <see cref="Quest"/>s this QuestsGiveable provides.
        /// </summary>
        private readonly List<string> questNames = new List<string>();

        /// <summary>
        /// The <see cref="Quest"/>s which have been loaden for this QuestsGiveable.
        /// </summary>
        private readonly List<Quest> quests = new List<Quest>();

        /// <summary>
        /// States whether all relevant <see cref="Quest"/>s have been loaden.
        /// </summary>
        private bool relevantQuestsLoaden;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
