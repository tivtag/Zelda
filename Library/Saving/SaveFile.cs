// <copyright file="SaveFile.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Saving.SaveFile class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Saving
{
    using System;
    using System.Globalization;
    using System.IO;
    using Atom.Storage;
    using Atom.Xna;
    using Zelda.Difficulties;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    using Zelda.Profiles;

    /// <summary>
    /// Provides type-safe access to the information stored in a SaveFile.
    /// </summary>
    /// <remarks>
    /// Keep in mind that the data stored in the <see cref="SaveFile"/>
    /// object may not be completly up-to-date until it is actually saved/loaden.
    /// </remarks>
    public sealed class SaveFile
    {
        #region [ Constants ]

        /// <summary>
        /// The current version number of the SaveFile.
        /// </summary>
        /// <remarks>
        /// V3/4 : Added versioning and power factors to Gem and Item instances.
        /// </remarks>
        private const int Version = 4;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of this <see cref="SaveFile"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="WorldStatus"/> object that descripes the status of the whole world.
        /// </summary>
        public WorldStatus WorldStatus
        {
            get
            {
                return this.worldStatus;
            }
        }

        /// <summary>
        /// Gets or sets the last safe <see cref="SavePoint"/> the player has used.
        /// </summary>
        public SavePoint LastSavePoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID that uniquely identifies the difficulty of the game.
        /// </summary>
        public DifficultyId Difficulty
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player choose to play on hardcore;
        /// e.g. once dead always dead.
        /// </summary>
        public bool Hardcore
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the colors in which the player's character is tinted.
        /// </summary>
        public LinkSpriteColorTint CharacterColorTint
        {
            get;
            internal set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFile"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="SaveFile"/>.
        /// </param>
        /// <param name="profile">
        /// Represents the GameProfile that is saved in this SaveFile.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides access to game-related services.
        /// </param>
        internal SaveFile( string name, GameProfile profile, IZeldaServiceProvider serviceProvider )
        {
            this.name = name;
            this.player = profile.Player;
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        #region > Saving <

        /// <summary>
        /// Saves this <see cref="SaveFile"/>.
        /// </summary>
        /// <returns>
        /// true if the SaveFile has been succesfully saved;
        /// otherwise false.
        /// </returns>
        public bool Save()
        {
            try
            {
                this.SaveUnchecked();
                return true;
            }
            catch( Exception exc )
            {
                this.serviceProvider.Log.WriteLine( "\nAn error has occurred while saving .." );
                this.serviceProvider.Log.WriteLine( exc.ToString() );
                return false;
            }
        }

        /// <summary>
        /// Saves this <see cref="SaveFile"/>.
        /// </summary>
        private void SaveUnchecked()
        {
            using( var memoryStream = new MemoryStream() )
            {
                var writer = new BinaryWriter( memoryStream );
                var context = new SerializationContext( writer, this.serviceProvider );
                this.Write( context );

                this.WriteToFile( memoryStream );
            }
        }

        /// <summary>
        /// Copies the content of the given memory stream onto
        /// the harddisc.
        /// </summary>
        /// <param name="memoryStream">
        /// The stream of data that contains all data that has been written.
        /// </param>
        private void WriteToFile( Stream memoryStream )
        {
            string folderPath = Path.Combine( GameFolders.Profiles, this.name );
            string filePath = Path.Combine( folderPath, this.name );
            Directory.CreateDirectory( folderPath );

            MoveToBackup( filePath );
            StorageUtilities.CopyToFile( memoryStream, filePath );
        }

        private static void MoveToBackup( string filePath )
        {
            if( File.Exists( filePath ) )
            {
                string backupFilePath = filePath + ".bak";
                File.Delete( backupFilePath );
                File.Move( filePath, backupFilePath );
            }
        }

        /// <summary>
        /// Writes the save file using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void Write( Zelda.Saving.IZeldaSerializationContext context )
        {
            WriteHeader( context );
            WriteShortGameInfo( context );

            // Save Point
            context.Write( this.LastSavePoint.Scene );
            context.Write( this.LastSavePoint.SpawnPoint );

            player.Serialize( context );
            worldStatus.SerializeState( context );
        }

        /// <summary>
        /// Writes the header information about this SaveFile.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private static void WriteHeader( Zelda.Saving.IZeldaSerializationContext context )
        {
            string fileInfo = string.Format(
                CultureInfo.InvariantCulture,
                Resources.SaveGameDescriptionVersionX,
                Version.ToString( CultureInfo.InvariantCulture )
            );

            context.Write( Version );
            context.Write( fileInfo );
        }

        /// <summary>
        /// Writes short information about the player.
        /// This is loaded when opening the game
        /// and shown om the character screen.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void WriteShortGameInfo( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentShortVersion = 4;
            context.Write( CurrentShortVersion );

            context.Write( player.Name );
            context.Write( player.ClassName );
            context.Write( player.Statable.Level );
            context.Write( player.Scene.LocalizedName ?? string.Empty );
            this.CharacterColorTint.Serialize( context ); // New in V4
            context.Write( (byte)this.Difficulty );
            context.Write( this.Hardcore ); // New in V3
        }

        #endregion

        #region > Loading <

        /// <summary>
        /// Loads this SaveFile.
        /// </summary>
        public void Load()
        {
            string folderPath = Path.Combine( GameFolders.Profiles, this.name );
            string filePath = Path.Combine( folderPath, this.name );

            using( var reader = new BinaryReader( File.OpenRead( filePath ) ) )
            {
                var context = new DeserializationContext( reader, this.serviceProvider );
                Read( context );
            }
        }

        /// <summary>
        /// Loads the <see cref="ShortSaveGameInfo"/> of the profile with the given name.
        /// </summary>
        /// <param name="profileName">
        /// The name of the profile.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The ShortSaveGameInfo that contains the data that has been loaden.
        /// </returns>
        public static ShortSaveGameInfo LoadShortGameInfo( string profileName, IZeldaServiceProvider serviceProvider )
        {
            string folderPath = Path.Combine( GameFolders.Profiles, profileName );
            string filePath = Path.Combine( folderPath, profileName );

            using( var reader = new BinaryReader( File.OpenRead( filePath ) ) )
            {
                // Read Header
                int version = reader.ReadInt32();
                reader.ReadString(); // Info String
                Atom.ThrowHelper.InvalidVersion( version, 0, SaveFile.Version, "SaveFile - ShortGameInfo" );

                var context = new DeserializationContext( reader, serviceProvider );
                return ReadShortGameInfo( version, context );
            }
        }

        /// <summary>
        /// Reads this SaveFile using the given BinaryReader.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void Read( DeserializationContext context )
        {
            // Read Header
            context.Version = context.ReadInt32();
            context.ReadString(); // Info String
            Atom.ThrowHelper.InvalidVersion( context.Version, 0, SaveFile.Version, this.GetType() );

            var shortInfo = ReadShortGameInfo( context.Version, context );
            this.Difficulty = shortInfo.Difficulty;
            this.Hardcore = shortInfo.Hardcore;
            this.CharacterColorTint = shortInfo.CharacterColorTint;
            GameDifficulty.Current = this.Difficulty; // Ugly hack. Don't use static classes ever again. ;<    

            // Read Save Point
            string sceneName = context.ReadString();
            string spawnPointName = context.ReadString();
            this.LastSavePoint = new SavePoint( sceneName, spawnPointName );

            player.Deserialize( context );
            worldStatus.DeserializeState( context );
        }

        /// <summary>
        /// Reads the <see cref="ShortSaveGameInfo"/> using the given BinaryReader.
        /// </summary>
        /// <param name="version">
        /// The version of the SaveFile.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The ShortSaveGameInfo that has been readen.
        /// </returns>
        private static ShortSaveGameInfo ReadShortGameInfo( int version, Zelda.Saving.IZeldaDeserializationContext context )
        {
            if( version >= 1 )
            {
                const int CurrentShortVersion = 4;
                int shortVersion = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( shortVersion, 1, CurrentShortVersion, "SaveFile.ShortGameInfo" );

                string name = context.ReadString();
                string className = context.ReadString();
                int level = context.ReadInt32();
                string region = context.ReadString();

                LinkSpriteColorTint colorTint;
                if( shortVersion >= 4 )
                {
                    colorTint = new LinkSpriteColorTint();
                    colorTint.Deserialize( context );
                }
                else
                {
                    context.ReadString(); // frontSpriteName
                    colorTint = LinkSpriteColorTint.Default;
                }

                DifficultyId difficulty = DifficultyId.Easy;
                bool hardcore = false;

                if( shortVersion >= 2 )
                {
                    difficulty = (DifficultyId)context.ReadByte();
                }

                if( shortVersion >= 3 )
                {
                    hardcore = context.ReadBoolean();
                }

                return new ShortSaveGameInfo( name, className, level, region, colorTint, difficulty, hardcore );
            }
            else
            {
                string name = context.ReadString();
                string className = context.ReadString();
                int level = context.ReadInt32();
                context.ReadString(); // frontSpriteName
                DifficultyId difficulty = DifficultyId.Easy;

                if( version >= 2 )
                {
                    difficulty = (DifficultyId)context.ReadByte();
                }

                return new ShortSaveGameInfo( name, className, level, "???", LinkSpriteColorTint.Default, difficulty, false );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The name of the <see cref="SaveFile"/>.
        /// </summary>
        private string name;

        /// <summary>
        /// Descripes the status of the whole world.
        /// </summary>
        private readonly WorldStatus worldStatus = new WorldStatus();

        /// <summary>
        /// The character whose status is descripted.
        /// </summary>
        private readonly PlayerEntity player;

        /// <summary>
        /// Provides type-safe access to game services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
