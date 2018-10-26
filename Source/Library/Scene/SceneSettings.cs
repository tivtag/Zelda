// <copyright file="SceneSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.SceneSettings class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
using System.Collections.Generic;
using System.ComponentModel;
using Atom.Diagnostics.Contracts;
using Atom.Math;
using Atom.Scene;
using Atom.Xna;
using Microsoft.Xna.Framework;
using Zelda.Audio;
using Zelda.Core.Properties;
using Zelda.Saving;

    /// <summary>
    /// Encapsulates the settings of a <see cref="ZeldaScene"/>.
    /// </summary>
    public sealed class SceneSettings : ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value that inidicates what general kind of scene
        /// the ZeldaScene is.
        /// </summary>
        public SceneType SceneType
        {
            get 
            { 
                return this.sceneType; 
            }

            set
            {
                if( value == this.sceneType )
                    return;

                this.sceneType = value;
                this.ApplyNewSceneType();
            }
        }

        /// <summary>
        /// Gets or sets the ambient color of the scene.
        /// </summary>
        public Color AmbientColor
        {
            get
            {
                return this.ambientColor;
            }

            set
            { 
                this.ambientColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// Lightning is currently enabled for this Scene.
        /// </summary>
        public bool IsLightingEnabled
        {
            get
            {
                return this.isLightingEnabled;
            }

            set 
            {
                this.isLightingEnabled = value;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="BackgroundMusic"/> from which songs are randomly choosen
        /// to be play in the background.
        /// </summary>
        public List<BackgroundMusic> MusicList
        {
            get
            { 
                return this.musicList;
            }
        }
        
        /// <summary>
        /// Gets the list of additional arbitrary scene properties.
        /// </summary>
        [Editor( "Zelda.Core.Properties.Design.PropertyListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public IPropertyList Properties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>
        /// Gets the settings that control the IWeather in the scene.
        /// </summary>
        public Zelda.Weather.IWeatherMachineSettings WeatherSettings
        {
            get
            {
                return this.scene.WeatherMachine.Settings;
            }
        }

        /// <summary>
        /// Gets or sets the number of subdivision that are done in the quad tree.
        /// </summary>
        public int SubdivisionDepth
        {
            get
            {
                return this.subdivisionDepth;
            }

            set
            {
                Contract.Requires<ArgumentException>( value >= 1 );
                Contract.Requires<ArgumentException>( value <= 5 );

                if( value == this.SubdivisionDepth )
                    return;

                this.subdivisionDepth = value;
                this.RecreateQuadTree();
            }
        }

        /// <summary>
        /// Gets the size of the scene in tilespace.
        /// </summary>
        public Point2 MapSize
        {
            get
            {
                return this.scene.Map.Size;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SceneSettings class.
        /// </summary>
        /// <param name="scene">
        /// The scene whose settings are managed by the new SceneSettings instance.
        /// </param>
        internal SceneSettings( ZeldaScene scene )
        {
            this.scene = scene;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Recreates the quad-tree.
        /// </summary>
        private void RecreateQuadTree()
        {
            var quadTree = this.scene.QuadTree;
            var items = new List<IQuadTreeItem2>( quadTree );

            quadTree.Create(
                Atom.Math.Vector2.Zero,
                scene.WidthInPixels,
                scene.HeightInPixels,
                64.0f,
                64.0f,
                this.subdivisionDepth,
                items.Count 
            );

            for( int i = 0; i< items.Count; ++i )
            {
                quadTree.Insert( items[i] );
            }
        }

        /// <summary>
        /// Applies the settings the currently set <see cref="SceneType"/> imply to the underlying Scene.
        /// </summary>
        private void ApplyNewSceneType()
        {
            switch( this.sceneType )
            {
                case SceneType.IndoorAmbient:
                    this.scene.DayNightCycle.IsEnabled = false;
                    this.scene.WeatherMachine.IsActivated = false;
                    break;

                case SceneType.Outdoor:
                    this.scene.DayNightCycle.IsEnabled = true;
                    this.scene.WeatherMachine.IsActivated = true;
                    break;

                case SceneType.OutdoorAmbient:
                    this.scene.DayNightCycle.IsEnabled = false;
                    this.scene.WeatherMachine.IsActivated = true;
                    break;

                case SceneType.Indoor:
                    this.scene.DayNightCycle.IsEnabled = true;
                    this.scene.WeatherMachine.IsActivated = false;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Clears this SceneSettings instance.
        /// </summary>
        internal void Clear()
        {
            this.musicList.Clear();
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            // Header.
            const int CurrentVersion = 4;
            context.Write( CurrentVersion );

            // Data.
            context.Write( (int)this.sceneType );
            context.Write( this.ambientColor );
            context.Write( this.subdivisionDepth );
           
            // Background Music:
            context.Write( this.musicList.Count );

            for( int i = 0; i < this.musicList.Count; ++i )
            {
                context.WriteObject( this.musicList[i] );
            }

            this.properties.Serialize( context );
            this.WeatherSettings.Serialize( context );
        }
        
        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            // Header.
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 0, 4, this.GetType() );

            // Data.
            this.SceneType = (SceneType)context.ReadInt32();
            this.ambientColor = context.ReadColor();

            if( version >= 3 )
            {
                this.subdivisionDepth = context.ReadInt32();
            }
            else
            {
                this.subdivisionDepth = 2;
            }

            // Background Music:
            int trackCount = context.ReadInt32();
            this.musicList.Capacity = trackCount;

            if( version >= 4 )
            {
                for( int i = 0; i < trackCount; ++i )
                {
                    this.musicList.Add( context.ReadObject<BackgroundMusic>() );
                }
            }
            else
            {
                for( int i = 0; i < trackCount; ++i )
                {
                    this.musicList.Add( new BackgroundMusic() { FileName = context.ReadString() } );
                }
            }

            if( version >= 1 )
            {
                this.properties.Deserialize( context );
            }

            if( version >= 2 )
            {
                this.WeatherSettings.Deserialize( context );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the <see cref="SubdivisionDepth"/> property.
        /// </summary>
        private int subdivisionDepth = 2;

        /// <summary>
        /// The ambient color applied to this ZeldaScene.
        /// </summary>
        private Color ambientColor = Color.White;

        /// <summary>
        /// States whether lighting is enabled for this ZeldaScene.
        /// </summary>
        private bool isLightingEnabled = true;

        /// <summary>
        /// Indicates what kind of scene this ZeldaScene is.
        /// </summary>
        private SceneType sceneType;

        /// <summary>
        /// Stores the arbitrary scene properties.
        /// </summary>
        private readonly IPropertyList properties = new PropertyList();

        /// <summary>
        /// Stores the names of the music tracks played in the background in this ZeldaScene.
        /// </summary>
        private readonly List<BackgroundMusic> musicList = new List<BackgroundMusic>();

        /// <summary>
        /// Identifies the ZeldaScene whose settings this SceneSettings instance encapsulates.
        /// </summary>
        private readonly ZeldaScene scene;

        #endregion
    }
}