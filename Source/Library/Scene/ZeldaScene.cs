// <copyright file="ZeldaScene.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ZeldaScene class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using Atom;
    using Atom.Components;
    using Atom.Events;
    using Atom.Math;
    using Atom.Scene;
    using Atom.Scene.Tiles;
    using Atom.Xna;
    using Zelda.Entities;
    using Zelda.Events;
    using Zelda.Saving;
    using Zelda.Story;
    using Zelda.Waypoints;

    /// <summary>
    /// Represents an actual ingame scene which contains entities, events, 
    /// particle systems, the tilemap and more.
    /// This is a sealed class.
    /// </summary>
    public sealed partial class ZeldaScene : IScene, ISceneChangeListener, Atom.AI.IMultiFloorPathSearcherProvider, IReloadable
    {
        #region [ Constants ]

        /// <summary>
        /// The extension of the file-format ZeldaScene's are saved in. ".zs"
        /// </summary>
        public const string Extension = ".zs";

        /// <summary>
        /// Gets the localized name of the scene with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the scene. Can be null.
        /// </param>
        /// <returns>
        /// The localized name.
        /// </returns>
        internal static string GetLocalizedName( string name )
        {
            if( name == null )
                return string.Empty;

            try
            {
                return Resources.ResourceManager.GetString( "SN_" + name );
            }
            catch( System.InvalidOperationException )
            {
                return name;
            }
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Fired when a <see cref="ZeldaEntity"/> has been added to this ZeldaScene.
        /// </summary>
        public event Atom.RelaxedEventHandler<ZeldaEntity> EntityAdded;

        /// <summary>
        /// Fired when a <see cref="ZeldaEntity"/> has been removed from this ZeldaScene.
        /// </summary>
        public event Atom.RelaxedEventHandler<ZeldaEntity> EntityRemoved;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies this <see cref="ZeldaScene"/>.
        /// </summary>
        public string Name
        {
            get
            {
                if( this.map == null )
                    return null;

                return this.map.Name;
            }

            set
            {
                this.map.Name = value;
                this.RefreshLocalizedName();
            }
        }

        /// <summary>
        /// Gets the localized name of this ZeldaScene, used ingame.
        /// </summary>
        public string LocalizedName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZeldaScene"/>
        /// is currently paused.
        /// </summary>
        /// <value>
        /// The default value is false.
        /// </value>
        public bool IsPaused
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the object that descripes the status of this <see cref="ZeldaScene"/>.
        /// </summary>
        public Saving.SceneStatus Status
        {
            get
            {
                return this.status;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaEventManager"/> of this <see cref="ZeldaScene"/>.
        /// </summary>
        public ZeldaEventManager EventManager
        {
            get
            {
                return this.eventManager;
            }
        }

        /// <summary>
        /// Gets or sets a reference of the <see cref="IngameDateTime"/> object
        /// which manages the date and time settings of the <see cref="ZeldaScene"/>.
        /// </summary>
        /// <value>
        /// This value is null by default and must be set after the Scene has been created.
        /// </value>
        public IngameDateTime IngameDateTime
        {
            get
            {
                return this.ingameDateTime;
            }

            set
            {
                this.ingameDateTime = value;
                this.dayNightCycle.IngameDateTime = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DayNightCycle"/> used by this ZeldaScene.
        /// </summary>
        public DayNightCycle DayNightCycle
        {
            get
            {
                return this.dayNightCycle;
            }
        }

        /// <summary>
        /// Gets the <see cref="IIngameState"/> this ZeldaScene is associated with.
        /// </summary>
        /// <value>The IIngameState; or null.</value>
        public IIngameState IngameState
        {
            get
            {
                if( this.Player == null )
                    return null;

                return this.Player.IngameState;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Weather.WeatherMachine"/>
        /// that manages the IWeather of this ZeldaScene.
        /// </summary>
        public Weather.WeatherMachine WeatherMachine
        {
            get
            {
                return this.weatherMachine;
            }
        }

        /// <summary>
        /// Gets the <see cref="FlyingTextManager"/> object.
        /// </summary>
        public FlyingTextManager FlyingTextManager
        {
            get
            {
                return this.flyingTextManager;
            }
        }

        /// <summary>
        /// Gets the object that manages "speak" bubbles.
        /// </summary>
        public BubbleTextManager BubbleTextManager
        {
            get
            {
                return this.bubbleTextManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaWaypointMap"/> that contains the Waypoints and Paths of this ZeldaScene.
        /// </summary>
        public ZeldaWaypointMap WaypointMap
        {
            get
            {
                return this.waypointMap;
            }
        }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Atom.Math.RandMT Rand
        {
            get
            {
                return this.serviceProvider.Rand;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.UI.ZeldaUserInterface"/>
        /// that is currently associated with this ZeldaScene.
        /// </summary>
        public Zelda.UI.ZeldaUserInterface UserInterface
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ZeldaStoryboard that manages the various Timelines that this ZeldaScene has.
        /// </summary>
        public ZeldaStoryboard Storyboard
        {
            get
            {
                return this.storyboard;
            }
        }

        /// <summary>
        /// Gets the settings of this ZeldaScene.
        /// </summary>
        public SceneSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// Gets the <see cref="QuadTree2"/> that contains the objects this ZeldaScene contains.
        /// </summary>
        /// <remarks>
        /// Should not be modified manually.
        /// </remarks>
        public QuadTree2 QuadTree
        {
            get
            {
                return this.quadTree;
            }
        }

        #region - Map Related -

        /// <summary>
        /// Gets the <see cref="TileMap"/> of this <see cref="ZeldaScene"/>.
        /// </summary>
        public TileMap Map
        {
            get
            {
                return this.map;
            }
        }

        /// <summary>
        /// Gets the width of this ZeldaScene (in pixels).
        /// </summary>
        public int WidthInPixels
        {
            get
            {
                return this.map.Width * 16;
            }
        }

        /// <summary>
        /// Gets the height of this ZeldaScene (in pixels).
        /// </summary>
        public int HeightInPixels
        {
            get
            {
                return this.map.Height * 16;
            }
        }

        /// <summary>
        /// Gets a value that indicates how many TileMapFloors this ZeldaScene has.
        /// </summary>
        public int FloorCount
        {
            get
            {
                return this.map.FloorCount;
            }
        }

        #endregion

        #region - Entities -

        /// <summary>
        /// Gets or sets the <see cref="PlayerEntity"/> object.
        /// </summary>
        public PlayerEntity Player
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the camera of this <see cref="ZeldaScene"/>.
        /// </summary>
        public ZeldaCamera Camera
        {
            get
            {
                return this.camera;
            }
        }

        #endregion

        #region - Lists -

        /// <summary>
        /// Gets the list of entities that are part of this Scene.
        /// </summary>
        public IEnumerable<ZeldaEntity> Entities
        {
            get
            {
                return this.entities;
            }
        }

        /// <summary>
        /// Gets the list of entities that are currently visible to the Player.
        /// </summary>
        public List<ZeldaEntity> VisibleEntities
        {
            get
            {
                return this.visibleEntities;
            }
        }

        /// <summary>
        /// Gets the list that contains the Lights that are currently visible.
        /// </summary>
        /// <remarks>
        /// Don't modify the list.
        /// </remarks>
        internal List<ILight> VisibleLights
        {
            get
            {
                return this.visibleLights;
            }
        }

        /// <summary>
        /// Gets the list that contains the ISceneOverlay that should
        /// be drawn ontop of the scene.
        /// </summary>
        /// <remarks>
        /// Don't modify the list.
        /// </remarks>
        internal List<Zelda.Overlays.ISceneOverlay> Overlays
        {
            get
            {
                return this.overlays;
            }
        }

        #endregion

        #region - Static Settings -

        /// <summary>
        /// Gets or sets a value indicating whether the application is in editor-mode.
        /// </summary>
        /// <value>The default value is false.</value>
        public static bool EditorMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is in entity edit-mode.
        /// </summary>
        /// <value>The default value is false.</value>
        public static bool EntityEditMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether EventTriggers are drawn in all Scenes.
        /// </summary>
        /// <value>The default value is false.</value>
        public static bool EventTriggersAreDrawn
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region [ Initialization ]

        #region - Constructors -

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaScene"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private ZeldaScene( IZeldaServiceProvider serviceProvider )
            : this( 0, 0, 0, serviceProvider )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaScene"/> class.
        /// </summary>
        /// <param name="width">The width of the new TileMap (in tile-space).</param>
        /// <param name="height">The height of the new TileMap (in tile-space).</param>
        /// <param name="initialFloorCount">
        /// The initial number of floors the new Scene has.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private ZeldaScene( int width, int height, int initialFloorCount, IZeldaServiceProvider serviceProvider )
        {
#if DEBUG
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );
            
            if( width < 0 || width > 1000 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "width" );

            if( height < 0 || height > 1000 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "height" );
#endif
            this.serviceProvider = serviceProvider;
            this.settings = new SceneSettings( this );

            this.map = new TileMap( width, height, 16, initialFloorCount );
            this.map.FloorsChanged += sender => { this.CreateTileMapFloorTags(); };

            this.eventManager = new ZeldaEventManager( this );
            this.waypointMap = new ZeldaWaypointMap( this );
            this.weatherMachine = new Zelda.Weather.WeatherMachine( this, serviceProvider );

            this.flyingTextManager = serviceProvider.GetService<FlyingTextManager>();
            this.bubbleTextManager = serviceProvider.GetService<BubbleTextManager>();

            this.camera = new ZeldaCamera( this );
            this.camera.TransformChanged += this.OnCameraTransformChanged;
        }

        #endregion

        #region - CreateManual -

        /// <summary>
        /// Manually creates a new instance of the <see cref="ZeldaScene"/> class.
        /// </summary>
        /// <param name="width">The width of the new TileMap (in tile-space).</param>
        /// <param name="height">The height of the new TileMap (in tile-space).</param>
        /// <param name="initialFloorCount">
        /// The initial number of floors the new Scene has.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>The new ZeldaScene.</returns>
        public static ZeldaScene CreateManual( int width, int height, int initialFloorCount, IZeldaServiceProvider serviceProvider )
        {
            ZeldaScene scene = new ZeldaScene( width, height, initialFloorCount, serviceProvider );

            scene.SetupMap();
            scene.SetupStatus( null );

            return scene;
        }

        #endregion

        #region - LoadScene -

        /// <summary>
        /// Creates a new instance of the <see cref="ZeldaScene"/> class
        /// by loading an existing scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>The new ZeldaScene.</returns>
        public static ZeldaScene Load( string sceneName, IZeldaServiceProvider serviceProvider )
        {
            return Load( sceneName, null, serviceProvider );
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ZeldaScene"/> class
        /// by loading an existing scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="worldStatus">
        /// Stores the state of the world. can be null.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>The new ZeldaScene.</returns>
        public static ZeldaScene Load( string sceneName, WorldStatus worldStatus, IZeldaServiceProvider serviceProvider )
        {
            ZeldaScene scene = new ZeldaScene( serviceProvider );

            scene.Load( sceneName, worldStatus );

            return scene;
        }

        #endregion

        #region - Setup -

        /// <summary>
        /// Setups the status of the Scene.
        /// </summary>
        /// <param name="worldStatus">
        /// Stores the status of the world. Can be null.
        /// </param>
        private void SetupStatus( WorldStatus worldStatus )
        {
            if( worldStatus != null )
            {
                // Receive scene status object:
                this.status = worldStatus.GetSceneStatus( this.Name );

                // If no status is known, then create a new one:
                if( this.status == null )
                {
                    status = new SceneStatus( this.Name );
                    worldStatus.AddSceneStatus( status );
                }

                this.IngameDateTime = worldStatus.IngameDateTime;
            }
            else
            {
                this.IngameDateTime = new IngameDateTime( new DateTime( 1045, 1, 1, 9, 0, 0 ), 96.0f );
            }
        }

        /// <summary>
        /// Removes the persistant entities that have been removed
        /// from this ZeldaScene.
        /// </summary>
        private void RemoveRemovedPersistantEntities()
        {
            if( this.status == null )
                return;

            if( this.status.RemovedPersistantEntities != null )
            {
                foreach( string removedEntity in this.status.RemovedPersistantEntities )
                {
                    this.RemoveEntity( removedEntity );
                }
            }
        }

        /// <summary>
        /// Setups the map related data of this <see cref="ZeldaScene"/>.
        /// </summary>
        private void SetupMap()
        {
            if( this.map != null )
            {
                this.quadTree.Create(
                    Vector2.Zero,
                    this.WidthInPixels,
                    this.HeightInPixels,
                    64.0f,
                    64.0f,
                    this.settings.SubdivisionDepth,
                    50
                );

                this.CreateTileMapFloorTags();
            }
            else
            {
                this.quadTree.Create( Vector2.Zero, 0, 0, 0, 0, 0, 0 );
            }

            this.waypointMap.Initialize();

            this.RefreshLocalizedName();
            this.RefreshSpriteSheetsToUpdate();
        }

        /// <summary>
        /// Refreshes the localized name of this ZeldaScene.
        /// </summary>
        private void RefreshLocalizedName()
        {
            this.LocalizedName = GetLocalizedName( this.Name );
        }

        /// <summary>
        /// Creates the ZeldaTileMapFloorTags of the TileMap.
        /// </summary>
        private void CreateTileMapFloorTags()
        {
            foreach( var floor in this.map.Floors )
            {
                if( floor.Tag == null )
                {
                    floor.Tag = ZeldaTileMapFloorTag.Create( floor );
                }
            }
        }

        #endregion

        #endregion
        
        #region [ Methods ]

        #region > Update <

        /// <summary>
        /// Updates this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {            
            if( this.IsPaused )
                return;

            float frameTime = updateContext.FrameTime;

            // Update all entities:
            for( int i = 0; i < this.entities.Count; ++i )
            {
                var entity = this.entities[i];

                entity.PreUpdate( updateContext );
                entity.Update( updateContext );
            }

            for( int i = 0; i < this.additionalObjectsToUpdate.Count; ++i )
            {
                this.additionalObjectsToUpdate[i].Update( updateContext );
            }

            // Update ingame weather
            this.weatherMachine.Update( updateContext );

            // We can stop updating this scene if this
            // is a silent behind-the-scenes update.
            if( !updateContext.IsMainUpdate )
                return;

            // Update the SpriteSheets:
            for( int i = 0; i < this.spriteSheetsToUpdate.Count; ++i )
            {
                this.spriteSheetsToUpdate[i].Update( updateContext );
            }

            // Update gametime & daynigth-cycle
            if( this.ingameDateTime != null )
            {
                this.ingameDateTime.Update( updateContext );
                this.dayNightCycle.Update();
            }

            // Update overlays   
            for( int i = 0; i < this.overlays.Count; ++i )
                this.overlays[i].Update( updateContext );

            // Update camera
            this.camera.Update();

            // Update visible
            this.timeSinceLastVisabilityUpdate += frameTime;
            if( this.visabilityUpdateNeeded || this.timeSinceLastVisabilityUpdate > MaximumTimeBetweenVisabilityUpdates )
                this.UpdateVisible();

            this.timeSinceLastSceneSort += frameTime;
            if( this.timeSinceLastSceneSort >= MaximumTimeBetweenSceneSorts )
                this.SortVisible();

            if( this.flyingTextManager != null )
                this.flyingTextManager.Update( updateContext );
            this.bubbleTextManager.Update( updateContext );
        }

        /// <summary>
        /// Updates the list of visible entities/drawables.
        /// </summary>
        private void UpdateVisible()
        {
            ClearVisible();

            #region Find Visible drawables

            quadTree.FindVisible( visibleTreeItems, camera.ViewArea );

            for( int i = 0; i < visibleTreeItems.Count; ++i )
            {
                IQuadTreeItem2 item = visibleTreeItems[i];

                // The item is a component, and as such is part of an Entity.
                var entity = (ZeldaEntity)item.Owner;
                visibleEntities.Add( entity );

                // Lights and normal Entities are drawn in seperate passes.
                var light = entity as ILight;
                if( light != null )
                {
                    visibleLights.Add( light );

                    if( !light.IsLightOnly )
                    {
                        visibleDrawables.Add( entity );
                    }
                }
                else
                {
                    visibleDrawables.Add( entity );
                }
            }
            
            if( EventTriggersAreDrawn )
                FindDrawableEventTriggers();
            
            #endregion

            this.ClearTileMapFloorTags();
            this.AddVisibleDrawablesToTileMapFloorTags();
            this.SortVisible();

            this.visabilityUpdateNeeded = false;
            this.timeSinceLastVisabilityUpdate = 0.0f;
        }

        private void ClearVisible()
        {
            visibleEntities.Clear();
            visibleTreeItems.Clear();
            visibleDrawables.Clear();
            visibleLights.Clear();
        }

        /// <summary>
        /// Clears the <see cref="ZeldaTileMapFloorTag"/>s of the TileMap.
        /// </summary>
        private void ClearTileMapFloorTags()
        {
            for( int i = 0; i < map.FloorCount; ++i )
            {
                var floor = map.GetFloor( i );

                var tag = (ZeldaTileMapFloorTag)floor.Tag;
                tag.ClearVisibleDrawables();
            }
        }

        /// <summary>
        /// Adds the currently visibleDrawables to the <see cref="ZeldaTileMapFloorTag"/>s of the TileMap.
        /// </summary>
        private void AddVisibleDrawablesToTileMapFloorTags()
        {
            for( int i = 0; i < visibleDrawables.Count; ++i )
            {
                var drawable = visibleDrawables[i];
                var floor = this.map.GetFloor( drawable.FloorNumber );

                if( floor != null )
                {
                    // The tag is never null when we reach this line.
                    var tag = (ZeldaTileMapFloorTag)floor.Tag;
                    tag.VisibleDrawables.Add( drawable );
                }
                else
                {
                    #region Error Logging

                    serviceProvider.Log.WriteLine(
                        Atom.Diagnostics.LogSeverities.Error,
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Error_DrawableHasInvalidFloorNrXThereAreYFloorsInSceneZ,
                            drawable.FloorNumber.ToString( CultureInfo.CurrentCulture ),
                            map.Floors.Count.ToString( CultureInfo.CurrentCulture ),
                            this.Name
                        )
                    );

                    #endregion
                }
            }
        }

        /// <summary>
        /// Sorts the IFloorDrawables that are currently visible.
        /// </summary>
        private void SortVisible()
        {
            for( int i = 0; i < this.map.FloorCount; ++i )
            {
                var floor = this.map.GetFloor( i );

                // The tag is never null when we reach this line.
                var tag = (ZeldaTileMapFloorTag)floor.Tag;
                tag.SortVisibleDrawables();
            }

            this.timeSinceLastSceneSort = 0.0f;
        }

        /// <summary>
        /// Helper-method of UpdateVisible that finds 
        /// the currently visible drawable EventTriggers.
        /// </summary>
        /// <remarks>
        /// This method needs refactoring.
        /// </remarks>
        private void FindDrawableEventTriggers()
        {
            foreach( var trigger in this.eventManager.Triggers )
            {
                // TileAreaEventTriggers can be drawn:
                var tileAreaTrigger = trigger as TileAreaEventTrigger;
                if( tileAreaTrigger == null )
                    continue;

                // The tag of an EventTrigger is used to hold an TileAreaTriggerDrawObject:
                var tag = tileAreaTrigger.Tag;
                if( tag != null )
                {
                    var drawObject = tag as TileAreaTriggerDrawObject;
                    if( drawObject != null )
                    {
                        drawObject.Scene = this;
                        visibleDrawables.Add( drawObject );
                    }
                }
                else
                {
                    var drawObject = new TileAreaTriggerDrawObject( tileAreaTrigger );

                    tileAreaTrigger.Tag = drawObject;
                    drawObject.Scene = this;
                    visibleDrawables.Add( drawObject );
                }
            }
        }

        public void UpdateSortVisible()
        {
            this.UpdateVisible();
            this.SortVisible();
        }

        #endregion

        #region > Organization <

        #region - Add -
        
        /// <summary>
        /// Adds the given <see cref="ZeldaEntity"/> to this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to add.
        /// </param>
        public void Add( ZeldaEntity entity )
        {
            Debug.Assert( entity != null );
            Debug.Assert( !entities.Contains( entity ), Resources.Error_EntityAlreadyExistsInScene );
            entity.Transform.UpdateTransform();

            this.quadTree.Insert( entity.QuadTreeItem );
            this.entities.Add( entity );

            entity.Scene = this;
            visabilityUpdateNeeded = true;

            // Notify.
            if( this.EntityAdded != null )
                this.EntityAdded( this, entity );
        }

        /// <summary>
        /// Adds the given <see cref="ZeldaEntity"/> to this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to add.
        /// </param>
        void IScene.Add( IEntity entity )
        {
            Add( (ZeldaEntity)entity );
        }
        
        /// <summary>
        /// Adds the given <see cref="Zelda.Overlays.ISceneOverlay"/> to this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="overlay">
        /// The overlay to add.
        /// </param>
        public void Add( Zelda.Overlays.ISceneOverlay overlay )
        {
            Debug.Assert( overlay != null );
            Debug.Assert( !overlays.Contains( overlay ), Resources.Error_OverlayAlreadyExistsInScene );

            this.overlays.Add( overlay );
            overlay.AddedToScene( this );
        }

        public void Add( Timing.Timer timer )
        {
            additionalObjectsToUpdate.Add( timer );
        }

        #endregion

        #region - Remove -
        
        /// <summary>
        /// Tries to remoe the given <see cref="ZeldaEntity"/> from this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to remove.
        /// </param>
        /// <returns>
        /// true if the ZeldaEntity has been removed;
        /// otherwise false.
        /// </returns>
        internal bool RemoveEntity( ZeldaEntity entity )
        {
            Debug.Assert( entity != null );

            if( this.entities.Remove( entity ) )
            {
                this.quadTree.Remove( entity.QuadTreeItem );
                this.visabilityUpdateNeeded = true;

                entity.Scene = null;

                if( this.EntityRemoved != null )
                    this.EntityRemoved( this, entity );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to remove the ZeldaEntity with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the entity to remove.</param>
        /// <returns>
        /// true if the ZeldaEntity has been removed;
        /// otherwise false.
        /// </returns>
        public bool RemoveEntity( string name )
        {
            if( name == null )
                return false;

            for( int i = 0; i < entities.Count; ++i )
            {
                var entity = entities[i];

                if( name.Equals( entity.Name, StringComparison.Ordinal ) )
                {
                    entity.RemoveFromScene();
                    this.NotifyVisabilityUpdateNeeded();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to remove the given <see cref="ZeldaEntity"/> from this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to remove.
        /// </param>
        /// <returns>
        /// true if the ZeldaEntity has been removed;
        /// otherwise false.
        /// </returns>
        bool IScene.Remove( IEntity entity )
        {
            return this.RemoveEntity( (ZeldaEntity)entity );
        }
        
        /// <summary>
        /// Adds the given <see cref="Zelda.Overlays.ISceneOverlay"/> to this <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="overlay">
        /// The overlay to add.
        /// </param>
        /// <returns>
        /// true if the overlay has been removed;
        /// otherwise false.
        /// </returns>
        public bool Remove( Zelda.Overlays.ISceneOverlay overlay )
        {
            if( this.overlays.Remove( overlay ) )
            {
                overlay.RemovedFromScene( this );
                return true;
            }

            return false;
        }

        public bool Remove( Timing.ITimer timer )
        {
            return additionalObjectsToUpdate.Remove( timer );
        }

        #endregion

        #region - Has -

        /// <summary>
        /// Returns whether this <see cref="ZeldaScene"/>
        /// contains a <see cref="ZeldaEntity"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the Entity to look for.</param>
        /// <returns>
        /// true if such an Entity exists;
        /// otherwise false.
        /// </returns>
        public bool HasEntity( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            for( int i = 0; i < this.entities.Count; ++i )
            {
                if( name.Equals( this.entities[i].Name, StringComparison.OrdinalIgnoreCase ) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether this ZeldaScene contains a <see cref="ZeldaEntity"/>
        /// within the given <paramref name="area"/> on the floor of the given <paramref name="floorNumber"/>.
        /// </summary>
        /// <param name="area">
        /// The area to check.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to check.
        /// </param>        
        /// <returns>
        /// true if an Entity exists at the given area of the given floor;
        /// otherwise false.
        /// </returns>
        public bool HasEntityAt( Rectangle area, int floorNumber )
        {
            // Could be optimized to use the QuadTree!
            for( int i = 0; i < this.entities.Count; ++i )
            {
                ZeldaEntity entity = this.entities[i];

                if( entity.FloorNumber == floorNumber )
                {
                    if( entity.Collision.Intersects( ref area ) )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether this ZeldaScene contains a solid <see cref="ZeldaEntity"/>
        /// within the given <paramref name="area"/> on the floor of the given <paramref name="floorNumber"/>.
        /// </summary>
        /// <param name="area">
        /// The area to check.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to check.
        /// </param>        
        /// <returns>
        /// true if an Entity exists at the given area of the given floor;
        /// otherwise false.
        /// </returns>
        public bool HasSolidEntityAt( Rectangle area, int floorNumber )
        {
            // Could be optimized to use the QuadTree!
            for( int i = 0; i < this.entities.Count; ++i )
            {
                ZeldaEntity entity = this.entities[i];

                if( entity.FloorNumber == floorNumber )
                {
                    if( entity.Collision.IsSolid &&
                        entity.Collision.Intersects( ref area ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether this ZeldaScene contains a <see cref="ZeldaEntity"/>
        /// within the given <paramref name="area"/> on the floor of the given <paramref name="floorNumber"/>,
        /// ignoring the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="area">
        /// The area to check.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to check.
        /// </param>
        /// <param name="butEntity">
        /// The ZeldaEntity to ignore.
        /// </param>
        /// <returns>
        /// true if an Entity exists at the given area of the given floor;
        /// otherwise false.
        /// </returns>
        public bool HasEntityAtBut( Rectangle area, int floorNumber, ZeldaEntity butEntity )
        {
            // Could be optimized to use the QuadTree!
            for( int i = 0; i < this.entities.Count; ++i )
            {
                ZeldaEntity entity = this.entities[i];

                if( entity.FloorNumber == floorNumber )
                {
                    if( entity.Collision.Intersects( ref area ) )
                    {
                        if( entity != butEntity )
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region - Get -

        /// <summary>
        /// Returns the first <see cref="ZeldaEntity"/> of the given <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of the <see cref="ZeldaEntity"/> to receive.
        /// </typeparam>
        /// <returns>
        /// The first <see cref="ZeldaEntity"/> of the given <typeparamref name="TEntity"/>;
        /// or null.
        /// </returns>
        internal TEntity GetEntity<TEntity>()
            where TEntity : class
        {
            for( int i = 0; i < this.entities.Count; ++i )
            {
                TEntity castEntity = this.entities[i] as TEntity;
                if( castEntity != null )
                    return castEntity;
            }

            return null;
        }

        /// <summary>
        /// Tries to receive the <see cref="Zelda.Entities.ZeldaEntity"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <remarks>This operation has a complexity of O(N).</remarks>
        /// <param name="name">
        /// The name that uniquely identifies the entity to receive.
        /// </param>
        /// <returns>
        /// The Zelda.Entities.ZeldaEntity or null if there exists no ZeldaEntity with the given name.
        /// </returns>
        public ZeldaEntity GetEntity( string name )
        {
            Debug.Assert( name != null );

            for( int i = 0; i < this.entities.Count; ++i )
            {
                ZeldaEntity entity = this.entities[i];
                if( name.Equals( entity.Name, StringComparison.Ordinal ) )
                {
                    return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to receive the <see cref="Zelda.Entities.Spawning.ISpawnPoint"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <remarks>This operation has a complexity of O(N).</remarks>
        /// <param name="name">
        /// The name that uniquely identifies the ISpawnPoint to receive.
        /// </param>
        /// <returns>
        /// The Zelda.Entities.Spawning.ISpawnPoint or null if there exists no ISpawnPoint with the given name.
        /// </returns>
        public Zelda.Entities.Spawning.ISpawnPoint GetSpawnPoint( string name )
        {
            Debug.Assert( name != null );

            for( int i = 0; i < this.entities.Count; ++i )
            {
                ZeldaEntity entity = this.entities[i];
                if( name.Equals( entity.Name, StringComparison.Ordinal ) )
                {
                    return entity as Zelda.Entities.Spawning.ISpawnPoint;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the action tilemap layer for the given floor.
        /// </summary>
        /// <param name="floorNumber">
        /// The number of the floor of the action layer to get.
        /// </param>
        /// <returns>
        /// The action layer of the map on the given floor.
        /// </returns>
        public TileMapDataLayer GetActionLayer( int floorNumber )
        {
            TileMapFloor floor = this.Map.GetFloor( floorNumber );
            return floor != null ? floor.ActionLayer : null;
        }

        #endregion

        #endregion

        #region > Visability <

        /// <summary>
        /// Finds all IQuadTreeItem2s that are in the given <see cref="Rectangle"/>.
        /// </summary>
        /// <remarks>
        /// The returned enumeration gets destroyed when FindVisible is called again.
        /// </remarks>
        /// <param name="rectangle">
        /// The area to look for entities.
        /// </param>
        /// <returns>
        /// The enumeration of visible IQuadTreeItem2s.
        /// WARNING: Don't midify this list.
        /// </returns>
        public List<IQuadTreeItem2> FindVisible( Rectangle rectangle )
        {
            tempVisibleTreeItems.Clear();
            this.quadTree.FindVisible( tempVisibleTreeItems, rectangle );

            return tempVisibleTreeItems;
        }

        /// <summary>
        /// Finds all ZeldaEntities that are in the given <see cref="Rectangle"/>.
        /// </summary>
        /// <remarks>
        /// The returned enumeration gets destroyed when FindVisibleEntities is called again.
        /// </remarks>
        /// <param name="floorNumber">
        /// The number of the floor to look for entities.
        /// </param>
        /// <param name="rectangle">
        /// The area to look for entities.
        /// </param>
        /// <returns>
        /// The enumeration of visible IQuadTreeItem2s.
        /// WARNING: Don't modify this list.
        /// </returns>
        public List<ZeldaEntity> FindVisibleEntities( int floorNumber, Rectangle rectangle )
        {
            var visibleItems = FindVisible( rectangle );
            tempVisibleEntities.Clear();

            for( int i = 0; i < visibleItems.Count; ++i )
            {
                var entity = (ZeldaEntity)visibleItems[i].Owner;
                
                if( floorNumber == entity.FloorNumber )
                    tempVisibleEntities.Add( entity );
            }

            return tempVisibleEntities;
        }

        /// <summary>
        /// Finds all ZeldaEntities that touch the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <remarks>
        /// The returned enumeration gets destroyed when FindVisibleEntities is called again.
        /// </remarks>
        /// <param name="entity">
        /// The related ZeldaEntity.
        /// </param>
        /// <returns>
        /// The enumeration of visible IQuadTreeItem2s.
        /// WARNING: Don't modify this list.
        /// </returns>
        public List<ZeldaEntity> FindVisibleEntities( ZeldaEntity entity )
        {
            return FindVisibleEntities( entity.FloorNumber, (Rectangle)entity.Collision.Rectangle );
        }

        #endregion

        #region > Containment <

        /// <summary>
        /// Gets whether this <see cref="ZeldaScene"/> contains an <see cref="IQuadTreeItem2"/>
        /// in the given <paramref name="area"/> while the given additional <paramref name="predicate"/> holds true.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If <paramref name="predicate"/> is null.
        /// </exception>
        /// <param name="area">
        /// The area to look for items in.
        /// </param>
        /// <param name="predicate">
        /// The additional predicate that must hold true.
        /// </param>
        /// <returns>
        /// true if this ZeldaScene contains an IQuadTreeItem2 in the given <paramref name="area"/>
        /// while the given <paramref name="predicate"/> still holds true;
        /// otherwise false.
        /// </returns>
        public bool Contains( Rectangle area, Predicate<IQuadTreeItem2> predicate )
        {
            return this.quadTree.Contains( area, predicate );
        }

        #endregion

        #region > Utility <

        /// <summary>
        /// Receives the <see cref="Atom.AI.AStarTilePathSearcher"/> for the given floor.
        /// </summary>
        /// <param name="floorNumber">
        /// The number of the floor.
        /// </param>
        /// <returns>
        /// The Atom.AI.AStarTilePathSearcher.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="floorNumber"/> is invalid.
        /// </exception>
        public Atom.AI.AStarTilePathSearcher GetTilePathSearcher( int floorNumber )
        {
            if( floorNumber < 0 || floorNumber >= this.map.FloorCount )
                throw new ArgumentOutOfRangeException( "floorNumber" );

            var floor = this.map.GetFloor( floorNumber );
            var tag = (ZeldaTileMapFloorTag)floor.Tag;

            var pathSearcher = tag.PathSearcher;

            if( pathSearcher == null )
            {
                pathSearcher = new Atom.AI.AStarTilePathSearcher();
                pathSearcher.Setup( floor.ActionLayer );

                tag.PathSearcher = pathSearcher;
            }

            return pathSearcher;
        }

        /// <summary>
        /// Gets the <see cref="Atom.AI.ITilePathSearcher"/> for the floor with the specified <paramref name="floorNumber"/>.
        /// </summary>
        /// <param name="floorNumber">
        /// The number of the floor.
        /// </param>
        /// <returns>
        /// The <see cref="Atom.AI.ITilePathSearcher"/> for the floor with the specified floorNumber.
        /// </returns>
        Atom.AI.ITilePathSearcher Atom.AI.IMultiFloorPathSearcherProvider.GetTilePathSearcher( int floorNumber )
        {
            return this.GetTilePathSearcher( floorNumber );
        }

        /// <summary>
        /// Notifies this ZeldaScene that a visability update is needed.
        /// </summary>
        public void NotifyVisabilityUpdateNeeded()
        {
            visabilityUpdateNeeded = true;
        }

        /// <summary>
        /// Notifies this ZeldaScene that the next visability update
        /// should happen sooner.
        /// </summary>
        public void NotifyVisabilityUpdateNeededSoon()
        {
            this.timeSinceLastVisabilityUpdate *= 3.0f;
        }

        /// <summary>
        /// Notifies this ZeldaScene that a scene change has occured.
        /// </summary>
        /// <param name="changeType">
        /// States whether the current scene has changed away or to this scene.
        /// </param>
        public void NotifySceneChange( ChangeType changeType )
        {
            for( int i = 0; i < this.entities.Count; ++i )
            {
                var listener = this.entities[i] as ISceneChangeListener;

                if( listener != null )
                {
                    listener.NotifySceneChange( changeType );
                }
            }

            if( changeType == ChangeType.Away )
            {
                if( this.flyingTextManager != null )
                {
                    this.flyingTextManager.Clear();
                }

                this.ClearVisible();
            }
            else
            {
                this.visabilityUpdateNeeded = true;
            }

            this.weatherMachine.NotifySceneChange( changeType );
        }

        /// <summary>
        /// Utility method that sets the visability of all ActionLayers of the TileMap of the Scene
        /// to the given boolean <paramref name="state"/>.
        /// </summary>
        /// <param name="state">
        /// true to show all action layers;
        /// false to hide all action layers.
        /// </param>
        public void SetVisibilityStateActionLayer( bool state )
        {
            foreach( var floor in this.map.Floors )
                floor.ActionLayer.IsVisible = state;
        }

        /// <summary>
        /// Toggles the visibility state of the action layers of the map On/Off.
        /// </summary>
        public void ToggleVisibilityStateActionLayer()
        {
            bool isVisible = false;

            foreach( var floor in map.Floors )
            {
                if( floor.ActionLayer.IsVisible )
                {
                    isVisible = true;
                    break;
                }
            }

            SetVisibilityStateActionLayer( !isVisible );
        }

        /// <summary>
        /// Refreshes the list of SpriteSheets that need to updated in the Scene.
        /// </summary>
        public void RefreshSpriteSheetsToUpdate()
        {
            // Reminder: The ActionLayer is never updateable in this Game.
            spriteSheetsToUpdate.Clear();

            foreach( var floor in map.Floors )
            {
                foreach( var layer in floor.Layers )
                {
                    var sheet = ((TileMapSpriteDataLayer)layer).Sheet;

                    if( sheet != null && sheet.UpdatableCount > 0 )
                    {
                        if( !spriteSheetsToUpdate.Contains( sheet ) )
                            spriteSheetsToUpdate.Add( sheet );
                    }
                }
            }
        }

        /// <summary>
        /// Removes every single object, entity, etc from this Scene.
        /// </summary>
        private void ClearAll()
        {
            this.quadTree.RemoveAll();
            this.entities.Clear();
            this.eventManager.Clear();
            this.overlays.Clear();

            this.visibleTreeItems.Clear();
            this.visibleLights.Clear();
            this.visibleEntities.Clear();
            this.visibleDrawables.Clear();

            this.settings.Clear();
            this.bubbleTextManager.Clear();
        }

        #endregion

        #region > Events <

        /// <summary>
        /// Gets called when the Transformation of the Camera has changed;
        /// such as when the players moves around the Scene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCameraTransformChanged( ZeldaCamera sender )
        {
            // By doing the following we reduce the time
            // until the next manual visability update.
            //
            // We need to to this instead of simply
            // calling NotifyVisabilityUpdateNeeded
            // because that would lag the game too much. (profiled)
            this.timeSinceLastVisabilityUpdate *= 2.0f;
        }

        #endregion

        /// <summary>
        /// Relodas this scene; including all its reloadable entities, overlays and weather effects.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            for( int i = 0; i < this.entities.Count; ++i )
            {
                var reloadable = this.entities[i] as IReloadable;

                if( reloadable != null )
                {
                    reloadable.Reload( serviceProvider );
                }
            }

            for( int i = 0; i < this.overlays.Count; ++i )
            {
                var reloadable = this.overlays[i] as IReloadable;

                if( reloadable != null )
                {
                    reloadable.Reload( serviceProvider );
                }
            }

            this.weatherMachine.Reload( serviceProvider );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Descripes the game status of this ZeldaScene. Is saved in the SaveFile.
        /// </summary>
        private SceneStatus status;

        /// <summary>
        /// The tilemap of this <see cref="ZeldaScene"/>.
        /// </summary>
        private readonly TileMap map;

        /// <summary>
        /// Combines and stores the various Timelines of this ZeldaScene.
        /// </summary>
        private readonly ZeldaStoryboard storyboard = new ZeldaStoryboard();

        /// <summary>
        /// Manages the Events and EventTriggers of this ZeldaScene.
        /// </summary>
        private readonly ZeldaEventManager eventManager;

        /// <summary>
        /// Manages and stores Waypoints and Paths in this ZeldaScene.
        /// </summary>
        private readonly ZeldaWaypointMap waypointMap;

        /// <summary>
        /// Manages the creation, drawing and updating of <see cref="FlyingText"/> instances.
        /// </summary>
        private readonly FlyingTextManager flyingTextManager;

        /// <summary>
        /// Manages the creation, drawing and updating of <see cref="BubbleText"/> instances.
        /// </summary>
        private readonly BubbleTextManager bubbleTextManager;

        /// <summary>
        /// Manages the IWeather shown in this ZeldaScene.
        /// </summary>
        private readonly Weather.WeatherMachine weatherMachine;

        /// <summary>
        /// The settings of this ZeldaScene.
        /// </summary>
        private readonly SceneSettings settings;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #region > Visability <

        /// <summary>
        /// The tree that stores the Entities of this <see cref="ZeldaScene"/>
        /// in a hirachical scheme for fast lock-up.
        /// </summary>
        private readonly QuadTree2 quadTree = new QuadTree2();

        /// <summary>
        /// The camera is responsible for showing us
        /// what can be seen by the player.
        /// </summary>
        private readonly ZeldaCamera camera;

        /// <summary>
        /// States whether a visability update is needed.
        /// </summary>
        private bool visabilityUpdateNeeded;

        /// <summary>
        /// Stores the time since the last visability update.
        /// </summary>
        private float timeSinceLastVisabilityUpdate;

        /// <summary>
        /// The maximum time that is allowed to pass before a visability update must happen.
        /// </summary>
        private const float MaximumTimeBetweenVisabilityUpdates = 10.0f;

        #endregion

        #region > Sorting <

        /// <summary>
        /// Stores the time since the last scene sort.
        /// </summary>
        private float timeSinceLastSceneSort;

        /// <summary>
        /// The maximum time that is allowed to pass before a scene sort must happen.
        /// </summary>
        private const float MaximumTimeBetweenSceneSorts = 2.0f;

        #endregion

        #region > Day Night <

        /// <summary>
        /// The <see cref="IngameDateTime"/> which stores the current date and time in the <see cref="ZeldaScene"/>.
        /// </summary>
        private IngameDateTime ingameDateTime;

        /// <summary>
        /// The <see cref="DayNightCycle"/> calculates the alpha value needed
        /// to make the day-night cycle go smooth.
        /// </summary>
        private readonly DayNightCycle dayNightCycle = new DayNightCycle();

        #endregion

        #region > Lists <

        /// <summary>
        /// Lists additional non-entity objects that are updated each frame; such as simple timers.
        /// </summary>
        private readonly List<IZeldaUpdateable> additionalObjectsToUpdate = new List<IZeldaUpdateable>();

        /// <summary>
        /// The list of entities that are part of this <see cref="ZeldaScene"/>.
        /// </summary>
        private readonly List<ZeldaEntity> entities = new List<ZeldaEntity>();

        /// <summary>
        /// The list of ISceneOverlay active in this ZeldaScene.
        /// </summary>
        private readonly List<Zelda.Overlays.ISceneOverlay> overlays = new List<Zelda.Overlays.ISceneOverlay>();

        /// <summary>
        /// Stores the currently visible items that have been stored in the quad tree.
        /// </summary>
        private readonly List<IQuadTreeItem2> visibleTreeItems = new List<IQuadTreeItem2>( 50 );

        /// <summary>
        /// The list of entities that are currently in the ViewArea of the player.
        /// </summary>
        private readonly List<ZeldaEntity> visibleEntities = new List<ZeldaEntity>( 50 );

        /// <summary>
        /// The list of currently visible Lights.
        /// </summary>
        private readonly List<ILight> visibleLights = new List<ILight>( 10 );

        /// <summary>
        /// Stores the currently visible <see cref="IZeldaFloorDrawable"/>s.
        /// </summary>
        private readonly List<IZeldaFloorDrawable> visibleDrawables = new List<IZeldaFloorDrawable>( 50 );

        /// <summary>
        /// Enumerates the SpriteSheets of the TileMap which need to updated each frame.
        /// </summary>
        private readonly List<SpriteSheet> spriteSheetsToUpdate = new List<SpriteSheet>( 2 );

        /// <summary>
        /// Temporary storage list that stores visible quad tree items.
        /// </summary>
        private readonly List<IQuadTreeItem2> tempVisibleTreeItems = new List<IQuadTreeItem2>( 25 );

        /// <summary>
        /// Temporary storage list that stores visible ZeldaEntities.
        /// </summary>
        private readonly List<ZeldaEntity> tempVisibleEntities = new List<ZeldaEntity>( 25 );

        #endregion

        #endregion
    }
}