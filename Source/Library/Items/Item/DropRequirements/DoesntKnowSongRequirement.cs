// <copyright file="DoesntKnowSongRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.DropRequirements.DoesntKnowSongRequirement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.DropRequirements
{
    using System;
    using System.ComponentModel;
    using Atom;
    using Zelda.Core.Requirements;
    using Zelda.Ocarina;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="IRequirement"/> that requires the player
    /// to not have learned a specified <see cref="Song"/>.
    /// </summary>
    public sealed class DoesntKnowSongRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the name of the song the player isn't allowed
        /// to have learned for the item to drop.
        /// </summary>
        [Editor( typeof( Zelda.Ocarina.Songs.Design.SongTypeNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SongTypeName
        {
            get
            {
                return this.songType == null ? string.Empty : this.songType.GetTypeName();            
            }

            set
            {
                var songType = Type.GetType( value );

                if( songType == null || !typeof( Song ).IsAssignableFrom( songType ) )
                {
                    throw new ArgumentException( Zelda.Resources.Error_TheGivenSongNameIsInvalid );
                }

                this.songType = songType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given PlayerEntity
        /// fulfills the requirements as specified by this IItemDropRequirement.
        /// </summary>
        /// <param name="player">
        /// The realted PlayerEntity.
        /// </param>
        /// <returns>
        /// Returns true if the given PlayerEntity fulfills the specified requirement;
        /// or otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity player )
        {
            if( player.OcarinaBox.HasSong( this.songType ) )
            {
                return false;
            }

            return true;
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
            context.Write( this.SongTypeName );
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
        }

        /// <summary>
        /// The type of the song.
        /// </summary>
        private Type songType;
    }
}
