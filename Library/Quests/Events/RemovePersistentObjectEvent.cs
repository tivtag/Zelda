// <copyright file="RemovePersistentObjectEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.Events.RemovePersistentObjectEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests.Events
{
    using System;
    using System.Diagnostics;
    using Zelda.Saving;

    /// <summary>
    /// Defines an <see cref="IQuestEvent"/> that removes
    /// an <see cref="Zelda.Entities.IPersistentEntity"/> from the world
    /// when executed.
    /// </summary>
    public sealed class RemovePersistentObjectEvent : IQuestEvent
    {
        /// <summary>
        /// Gets or sets the name of the persistent
        /// object that gets removed by this IQuestEvent.
        /// </summary>
        /// <value>The default value is null.</value>
        public string ObjectName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the Scene that contains
        /// the object to remove.
        /// </summary>
        /// <remarks>
        /// If this string property is null or empty 
        /// then the current scene of the player is assumed.
        /// </remarks>
        /// <value>The default value is null.</value>
        public string SceneName
        {
            get;
            set;
        }
        
        /// <summary>
        /// Executes this <see cref="IQuestEvent"/>.
        /// </summary>
        /// <param name="quest">
        /// The related Quest.
        /// </param>
        public void Execute( Quest quest )
        {
            var player = quest.Player;
            Debug.Assert( player != null, "The quest has not been accepted or already completed by the player." );

            var sceneStatus = string.IsNullOrEmpty( this.SceneName ) ?
                player.Scene.Status : player.WorldStatus.GetSceneStatus( this.SceneName );

            if( sceneStatus == null )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_CouldntFindXNameY,
                        "SceneStatus",
                        this.SceneName
                    )
                );
            }

            sceneStatus.RemovePersistantEntity( this.ObjectName );
        }
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.ObjectName ?? string.Empty );
            context.Write( this.SceneName ?? string.Empty );
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
            context.ReadDefaultHeader( this.GetType() );

            this.ObjectName = context.ReadString();
            this.SceneName  = context.ReadString();
        }
    }
}
