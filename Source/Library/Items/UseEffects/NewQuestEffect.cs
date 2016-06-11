using Zelda.Quests;

namespace Zelda.Items.UseEffects
{
    /// <summary>
    /// Represents an <see cref="ItemUseEffect"/> that
    /// gives the player a new <see cref="Quest"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class NewQuestEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely
        /// identifies the quest the item gives to the player.
        /// </summary>
        public string QuestName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="LocalizableText"/> object that stores
        /// the description of this NewQuestEffect.
        /// </summary>
        public LocalizableText DescriptionText
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Zelda.Status.Statable statable )
        {
            return this.description.LocalizedText;
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this NewQuestEffect uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get
            {
                return 0.0;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the NewQuestEffect class.
        /// </summary>
        public NewQuestEffect()
        {
            this.DestroyItemOnUse = true;
        }

        /// <summary>
        /// Setups this NewQuestEffect.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;

            base.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this NewQuestEffect.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this NewQuestEffect.
        /// </param>
        /// <returns>
        /// true if this NewQuestEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Zelda.Entities.PlayerEntity user )
        {
            var questLog = user.QuestLog;
            if( questLog.HasActiveQuest( this.QuestName ) )
                return false;

            if( quest == null )
            {
                quest = Quest.TryLoad( this.QuestName, serviceProvider );
                if( quest == null )
                    return false;
            }

            var dialog = user.Scene.UserInterface.Dialog;
            if( dialog.IsEnabled )
                return false;

            if( !quest.CanEverAccept( user ) )
            {
                dialog.Show( LocalizableTextResources.NewQuest_CantEverAccept );
                return true;
            }

            if( quest.Accept( user ) )
            {
                if( !string.IsNullOrEmpty( quest.LocalizedTextStart ) )
                {
                    dialog.Show( quest.LocalizedTextStart );
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.QuestName ?? string.Empty );
            context.Write( this.description.Id ?? string.Empty );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.QuestName      = context.ReadString();
            this.description.Id = context.ReadString();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The cached quest.
        /// </summary>
        private Quest quest;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        /// <summary>
        /// The text that is shown as a description for this NewQuestEffect.
        /// </summary>
        private readonly LocalizableText description = new LocalizableText();

        #endregion
    }
}
