// <copyright file="LearnSongEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Quests.Events.LearnSongEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Quests.Events
{
    using System;
    using System.ComponentModel;
    using Zelda.Ocarina;
    using Zelda.Saving;
    
    /// <summary>
    /// Defines an IQuestEvent that learns the player an ocarina Song.
    /// </summary>
    public sealed class LearnSongEvent : IQuestEvent
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies the ocarina Song learned by this LearnSongEvent.
        /// </summary>
        /// <value>The default value is null.</value>
        [Editor( typeof( Zelda.Ocarina.Songs.Design.SongTypeNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SongTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the song music
        /// should be played once the song has been learned.
        /// </summary>
        public bool ShouldPlaySongMusic
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
            OcarinaBox ocarinaBox = quest.Player.OcarinaBox;

            Type songType = Type.GetType( this.SongTypeName );
            if( ocarinaBox.HasSong( songType ) )
                return;

            Song song = (Song)Activator.CreateInstance( songType );
            song.Setup( this.serviceProvider );
            ocarinaBox.AddSong( song );

            if( this.ShouldPlaySongMusic )
            {
                song.Music.Play( quest.Player );
            }
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

            context.Write( this.SongTypeName ?? string.Empty );
            context.Write( this.ShouldPlaySongMusic );
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

            this.SongTypeName = context.ReadString();
            this.ShouldPlaySongMusic = context.ReadBoolean();
            this.serviceProvider = context.ServiceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }
}
