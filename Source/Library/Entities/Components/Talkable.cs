// <copyright file="Talkable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Talkable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom.Math;
    using Zelda.Factions;

    /// <summary>
    /// Defines a <see cref="ZeldaComponent"/> that allows
    /// a <see cref="ZeldaEntity"/> to talk to the player.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// -- Developer Remarks --
    /// The internal implementation of this class is a bit messy.
    /// It depends on the exact format of the <see cref="ReputationLevel"/> enumeration.
    /// </remarks>
    public sealed class Talkable : ZeldaComponent, Saving.ISaveable
    {
        #region [ Constants ]

        /// <summary>
        /// States the number of different reputation levels.
        /// </summary>
        /// <remarks>
        /// The talkable ZeldaEntity may say different things depending 
        /// on the current Reputation Level of the player towards the ZeldaEntity.
        /// </remarks>
        private const int ReputationLevelCount = 9;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to get the text the ZeldaEntity says.
        /// </summary>
        /// <param name="level">
        /// The current <see cref="ReputationLevel"/> of the player
        /// towards the talkable ZeldaEntity.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// The text the talkable ZeldaEntity says;
        /// or null if the talkable ZeldaEntity doesn't want to talk.
        /// </returns>
        public string GetText( ReputationLevel level, RandMT rand )
        {
            // Get the list for the current level.
            var list = GetTextList( level );

            // If there is no list at the current
            // level then take a look at lower levels.
            while( list == null || list.Count == 0 )
            {
                int currentLevel = (int)level;
                if( currentLevel == 0 )
                    return null; // There is no lower level.

                // Move one level down:
                level = (ReputationLevel)(currentLevel - 1);

                // And get the list (:
                list  = GetTextList( level );
            }

            Debug.Assert( list != null );
            return list[rand.RandomRange( 0, list.Count - 1 )].LocalizedText;
        }

        /// <summary>
        /// Gets the list of text for the given <see cref="ReputationLevel"/>.
        /// </summary>
        /// <param name="level">
        /// The <see cref="ReputationLevel"/> to get the list of text for.
        /// </param>
        /// <returns>
        /// The list of text for the given <paramref name="level"/>. (may be null)
        /// </returns>
        public List<LocalizableText> GetTextList( ReputationLevel level )
        {
            if( texts == null )
                return null;
            return this.texts[(int)level];
        }

        /// <summary>
        /// Gets the list of text for the given <see cref="ReputationLevel"/>.
        /// </summary>
        /// <param name="level">
        /// The <see cref="ReputationLevel"/> to set the list of text for.
        /// </param>
        /// <param name="list">
        /// The list of text the talkable ZeldaEntity uses to chose 
        /// a line of text to say on the given <see cref="ReputationLevel"/>.
        /// </param>
        public void SetTextList( ReputationLevel level, List<LocalizableText> list )
        {
            if( this.texts == null )
                CreateTextsArray();

            this.texts[(int)level] = list;
        }

        /// <summary>
        /// Helpers method that creates the texts array.
        /// </summary>
        private void CreateTextsArray()
        {
            this.texts = new List<LocalizableText>[ReputationLevelCount];
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given Talkable component to be a clone of this Talkable component.
        /// </summary>
        /// <param name="clone">
        /// The Talkable component to setup as a clone of this Talkable component.
        /// </param>
        public void SetupClone( Talkable clone )
        {
            clone.texts = this.texts;
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

            for( int i = 0; i < ReputationLevelCount; ++i )
            {
                var list = texts != null ? texts[i] : null;

                if( list != null )
                {
                    context.Write( true );

                    context.Write( list.Count );
                    foreach( var entry in list )
                        context.Write( entry.Id );
                }
                else
                {
                    context.Write( false );
                }
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

            for( int i = 0; i < ReputationLevelCount; ++i )
            {
                bool hasText = context.ReadBoolean();

                if( hasText )
                {
                    if( texts == null )
                        this.CreateTextsArray();

                    int count = context.ReadInt32();
                    var list  = new List<LocalizableText>( count );

                    for( int j = 0; j < count; ++j )
                    {
                        list.Add( new LocalizableText() { Id = context.ReadString() } );
                    }

                    texts[i] = list;
                }
                else
                {
                    if( texts != null )
                        texts[i] = null;
                }
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the lists of text the talkable ZeldaEntity may speak, sorted by <see cref="ReputationLevel"/>.
        /// </summary>
        private List<LocalizableText>[] texts;

        #endregion
    }
}
