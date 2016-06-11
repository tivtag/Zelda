// <copyright file="WantedSignPost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.WantedSignPost class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Zelda.Saving;
    using Zelda.Quests;
    
    /// <summary>
    /// Represents a <see cref="SignPost"/> that gives the Player a
    /// (most of the times, repeatable) Quest.
    /// </summary>
    public sealed class WantedSignPost : SignPost, IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies the Quest given by this WantedSignPost.
        /// </summary>
        public string QuestName
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Setups this WantedSignPost.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when the Player has read this WantedSignPost.
        /// </summary>
        /// <param name="user">
        /// The related PlayerEntity.
        /// </param>
        protected override void OnRead( PlayerEntity user )
        {
            if( user.QuestLog.HasActiveQuest( this.QuestName ) )
            {
                this.ShowText();
            }
            else
            {
                this.AcceptQuest( user );
            }
        }

        /// <summary>
        /// Tries to accept the quest given by this WantedSignPost.
        /// </summary>
        /// <param name="user">
        /// The related PlayerEntity.
        /// </param>
        private void AcceptQuest( PlayerEntity user )
        {
            Quest quest = Quest.TryLoad( this.QuestName, this.serviceProvider );

            if( quest == null )
            {
                this.ShowText();
            }
            else if( quest.Accept( user ) )
            {
                this.ShowQuestText( quest );
            }
            else
            {
                this.ShowText();
            }
        }

        /// <summary>
        /// Shows the text of this WantedSignPost that is related to the Quest.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        private void ShowQuestText( Quest quest )
        {
            var userInterface = this.Scene.UserInterface;
            var dialog        = userInterface.Dialog;

            dialog.Show( quest.LocalizedTextStart );
        }
        
        /// <summary>
        /// Creates a clone of this WantedSignPost.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new WantedSignPost();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given WantedSignPost to be a clone of this WantedSignPost.
        /// </summary>
        /// <param name="clone">
        /// The WantedSignPost to setup as a clone of this WantedSignPost.
        /// </param>
        private void SetupClone( WantedSignPost clone )
        {
            base.SetupClone( clone );

            clone.QuestName = this.QuestName;
            clone.serviceProvider = this.serviceProvider;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="WantedSignPost"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<WantedSignPost>
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
            public override void Serialize( WantedSignPost entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Main Data
                context.Write( entity.Name );
                context.Write( entity.FloorNumber );
                context.Write( entity.Text.Id ?? string.Empty );
                context.Write( entity.QuestName ?? string.Empty );

                // Transform
                context.Write( entity.Transform.Position.X );
                context.Write( entity.Transform.Position.Y );
                context.Write( (int)entity.Transform.Direction );

                entity.Collision.Serialize( context );

                // Drawing
                entity.DrawDataAndStrategy.Serialize( context );
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
            public override void Deserialize( WantedSignPost entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                context.ReadDefaultHeader( this.GetType() );

                // Main Data
                entity.Name    = context.ReadString();
                entity.FloorNumber = context.ReadInt32();
                entity.Text.Id = context.ReadString();
                entity.QuestName = context.ReadString();

                // Transform
                float x = context.ReadSingle();
                float y = context.ReadSingle();
                entity.Transform.Position = new Atom.Math.Vector2( x, y );
                entity.Transform.Direction = (Atom.Math.Direction4)context.ReadInt32();
                
                entity.Collision.Deserialize( context );

                // Drawing
                entity.DrawDataAndStrategy.Deserialize( context );
                entity.DrawDataAndStrategy.Load( serviceProvider );
            }
        }

        #endregion
    }
}
