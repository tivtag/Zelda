// <copyright file="IngameState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.IngameState class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Atom;
    using Atom.Math;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Zelda.Entities.Spawning;
    using Zelda.Profiles;
    using Zelda.Saving;
    using Zelda.UI;

    /// <summary>
    /// Defines the the IGameState in which the actual game runs.
    /// </summary>
    internal sealed class IngameState : Atom.IGameState, IIngameState, IDisposable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether the game
        /// itself has been paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.isPaused;
            }

            set
            {
                this.isPaused = value;
            }
        }

        /// <summary>
        /// Gets the currently active PlayerEntity.
        /// </summary>
        public PlayerEntity Player
        {
            get
            {
                return this.player;
            }
        }

        /// <summary>
        /// Gets the <see cref="GameProfile"/> of player that is currently playing the game.
        /// </summary>
        public GameProfile Profile
        {
            get
            {
                return this.profile;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaGame"/> object.
        /// </summary>
        public ZeldaGame Game
        {
            get
            {
                return this.game;
            }
        }

        /// <summary>
        /// Gets the <see cref="IngameUserInterface"/> that contains all the UIElements shown ingame.
        /// </summary>
        public IngameUserInterface UserInterface
        {
            get
            {
                return this.userInterface;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Audio.BackgroundMusicComponent"/> that manages 
        /// the music that is playing in the background of the game.
        /// </summary>
        public Zelda.Audio.BackgroundMusicComponent BackgroundMusic
        {
            get
            {
                return this.backgroundMusic;
            }
        }

        /// <summary>
        /// Gets the current <see cref="ZeldaScene"/>.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        private Zelda.Scripting.ZeldaScriptEnvironment Scripts
        {
            get
            {
                return this.game.Scripts;
            }
        }

        /// <summary>
        /// Gets the UserInterface shown ingame.
        /// </summary>
        ZeldaUserInterface IIngameState.UserInterface
        {
            get
            {
                return this.userInterface;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="IngameState"/> class.
        /// </summary>
        /// <param name="game">
        /// The ZeldaGame object.
        /// </param>
        public IngameState( ZeldaGame game )
        {
            this.game = game;
            this.graphics = game.Graphics;
            this.audioSystem = this.game.AudioSystem;

            this.userInterface = new IngameUserInterface( this );
            this.backgroundMusic = new Zelda.Audio.BackgroundMusicComponent( game );

            this.userInterface.KeyboardInput += this.OnKeyboardInput;
            this.userInterface.MouseInput += this.OnMouseInput;
        }

        /// <summary>
        /// Loads the IngameState.
        /// </summary>
        public void Load()
        {
            this.backgroundMusic.Initialize();
            this.userInterface.Setup( game );
            this.LoadProfile();
        }

        /// <summary>
        /// Unloads the IngameState.
        /// </summary>
        public void Unload()
        {
        }

        public void Reload()
        {
            this.ReloadScenes();
            this.ReloadUserInterface();
        }

        private void ReloadScenes()
        {
            if( this.player != null )
            {
                var cache = this.player.WorldStatus.ScenesCache;
                cache.Reload( this.game );

                if( !cache.Contains( this.scene ) )
                {
                    this.scene.Reload( game );
                }
            }
        }

        private void ReloadUserInterface()
        {
            this.userInterface.Unload();
            this.userInterface.Setup( game );
            this.userInterface.SetupForProfile( this.profile );
            this.userInterface.SetupForScene( scene );
        }

        /// <summary>
        /// Setups the IngameState for the currently selected profile.
        /// </summary>
        private void LoadProfile()
        {
            if( profile == null )
                throw new InvalidOperationException( Resources.Error_TheProfileIsNull );

            this.player = profile.Player;
            this.worldStatus = profile.WorldStatus;
            this.keySettings = profile.KeySettings;

            this.player.IngameState = this;
            this.game.ItemManager.Statable = this.player.Statable;

            // Setup UI:
            userInterface.SetupForProfile( profile );

            // Receive & Load the last SavePoint
            SavePoint lastSavePoint = profile.LastSavePoint;

            // Load scene:
            LoadScene( lastSavePoint.Scene );

            // Spawn player:
            ISpawnPoint spawnPoint = this.scene.GetSpawnPoint( lastSavePoint.SpawnPoint );

            if( spawnPoint == null )
            {
                throw new Atom.NotFoundException(
                   string.Format(
                       System.Globalization.CultureInfo.CurrentCulture,
                       Resources.Error_CouldntFindSpawnPointXInSceneY,
                       lastSavePoint.SpawnPoint,
                       scene.Name
                   )
                );
            }

            spawnPoint.Spawn( player );

            // Setup scene:
            scene.Camera.EntityToFollow = player;
            scene.UpdateSortVisible();

            // Setup hardcore:
            player.Statable.Died += OnPlayerDied;

            // Setup autosave
            player.Spawnable.Spawned += OnPlayerSpawned;
            player.Statable.LevelUped += OnPlayerLevelUped;
        }

        /// <summary>
        /// Loads and changes to the ZeldaScene with the given name.
        /// </summary>
        /// <param name="name">
        /// The name of the scene to load.
        /// </param>
        /// <returns>
        /// The loaded ZeldaScene.
        /// </returns>
        private ZeldaScene LoadScene( string name )
        {
            #region Verify State

            Debug.Assert( profile != null );
            Debug.Assert( this.player == profile.Player );

            #endregion

            // Load scene:
            ZeldaScene newScene = ZeldaScene.Load( name, profile.SaveFile.WorldStatus, game );

            ChangeScene( newScene );

            return newScene;
        }

        /// <summary>
        /// Helper method that changes the current ZeldaScene to the given ZeldaScene.
        /// </summary>
        /// <param name="scene">
        /// The new ZeldaScene.
        /// </param>
        private void ChangeScene( ZeldaScene scene )
        {
            if( this.player.Scene != null )
                this.player.RemoveFromScene();

            this.scene = scene;

            // Setup scene:
            scene.Camera.ViewSize = game.ViewSize;
            scene.Camera.EntityToFollow = this.player;
            scene.Player = this.player;
            scene.UserInterface = this.userInterface;
            scene.SetVisibilityStateActionLayer( false );

            this.userInterface.SetupForScene( scene );
            this.Scripts.OnSceneChanged( scene );

            // Notify:
            this.scene.NotifySceneChange( ChangeType.To );
            ShowEnteringRegion( scene );

            // Music:
            this.backgroundMusic.MusicList = scene.Settings.MusicList.ToArray();
            this.backgroundMusic.ChangeToRandom();

            // Simulate one time-step to be ready to draw the scene
            var context = new ZeldaUpdateContext() { GameTime = new Microsoft.Xna.Framework.GameTime(), IsPaused = true };
            userInterface.Update( context );
            scene.Update( context );
        }

        private void ShowEnteringRegion( ZeldaScene scene )
        {
            if( !player.IsDead )
            {
                this.userInterface.GetElement<EnteringRegionDisplay>().Show( scene.LocalizedName );
            }
        }

        #endregion

        #region [ Methods ]

        #region > Updating <

        /// <summary>
        /// Updates the IngameState.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( Atom.IUpdateContext updateContext )
        {
            var zeldaUpdateContext = (ZeldaUpdateContext)updateContext;

            if( !this.isPaused && !this.scene.IsPaused )
            {
                zeldaUpdateContext.IsPaused = false;

                this.scene.Update( zeldaUpdateContext );
                this.scene.EventManager.Update( zeldaUpdateContext, scene.Player );
                this.worldStatus.Update( zeldaUpdateContext );

                this.TestCollisionPlayerEnemies();
                this.UpdateAudioListenerPosition();
                this.inactiveSceneUpdater.Update( zeldaUpdateContext );
            }
            else
            {
                zeldaUpdateContext.IsPaused = true;
            }

            this.userInterface.Update( zeldaUpdateContext );
            this.backgroundMusic.Update( zeldaUpdateContext );
        }

        /// <summary>
        /// Tests collision of all nearby enemies with the player.
        /// </summary>
        private void TestCollisionPlayerEnemies()
        {
            int playerFloor = player.FloorNumber;
            var entities = this.scene.VisibleEntities;

            for( int i = 0; i < entities.Count; ++i )
            {
                var entity = entities[i];
                var notifier = entity as ICollisionWithPlayerNotifier;

                if( notifier != null )
                {
                    if( playerFloor == entity.FloorNumber )
                    {
                        if( entity.Collision.IntersectsUnstrict2px( player.Collision ) )
                        {
                            notifier.NotifyCollisionWithPlayer( player );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the position of the audio listener for 2D sound.
        /// </summary>
        private void UpdateAudioListenerPosition()
        {
            this.audioSystem.ListenerPosition2D = this.player.Collision.Center;
        }

        #endregion

        #region > Drawing <

        /// <summary>
        /// Draws the IngameState.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( Atom.IDrawContext drawContext )
        {
            var pipeline = this.graphics.Pipeline;

            pipeline.InitializeFrame( this.scene, this.userInterface, (ZeldaDrawContext)drawContext );
            {
                pipeline.BeginScene();
                pipeline.EndScene();
                pipeline.BeginUserInterface();
                pipeline.EndUserInterface();
            }
        }

        #endregion

        #region > Game Helpers <

        /// <summary>
        /// Tells this IngameState to reload the current <see cref="ZeldaScene"/>
        /// by discarding and the loading it again.
        /// </summary>
        /// <returns>
        /// The new ZeldaScene instance.
        /// </returns>
        public ZeldaScene RequestSceneReload()
        {
            if( this.scene == null )
            {
                throw new InvalidOperationException( Resources.Error_SceneIsNull );
            }

            string sceneName = this.scene.Name;

            // Discard current scene
            this.scene.NotifySceneChange( ChangeType.Away );
            this.scene = null;

            // Reload old scene
            return this.RequestSceneChange( sceneName, cachePrevious: false );
        }

        /// <summary>
        /// Tells this IngameState to change the current <see cref="ZeldaScene"/>.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the ZeldaScene to change to.
        /// </param>
        /// <param name="cachePrevious">
        /// States whether the previous ZeldaScene should be cached using the <see cref="ZeldaScenesCache"/>.
        /// </param>
        /// <returns>
        /// The new current ZeldaScene.
        /// </returns>
        public ZeldaScene RequestSceneChange( string name, bool cachePrevious )
        {
            if( this.player == null )
                return null;

            // If no change is required:
            if( this.scene != null && this.scene.Name.Equals( name, StringComparison.Ordinal ) )
            {
                return this.scene;
            }

            // Aquire and fill cache:
            var scenesCache = this.player.WorldStatus.ScenesCache;

            if( this.scene != null )
            {
                if( cachePrevious )
                {
                    scenesCache.Add( this.scene );
                    this.inactiveSceneUpdater.NotifyInacitify( this.scene );
                }

                this.scene.NotifySceneChange( ChangeType.Away );
                this.scene.Player = null;
            }

            // Load New
            ZeldaScene cachedScene = scenesCache.Get( name );

            if( cachedScene != null )
            {
                this.ChangeScene( cachedScene );
                return cachedScene;
            }
            else
            {
                var newScene = this.LoadScene( name );
                this.ChangeScene( newScene );
                return newScene;
            }
        }

        /// <summary>
        /// Tries to pickup or use any item the Player is interacting with.
        /// </summary>
        private void TryPickupOrUse()
        {
            var entities = this.scene.VisibleEntities;
            if( entities == null )
                return;

            // first check whether we can pickup an item
            for( int i = 0; i < entities.Count; ++i )
            {
                var pickupable = entities[i] as IPickupableEntity;
                if( pickupable == null )
                    continue;

                if( pickupable.PickUp( player ) )
                    return;
            }

            // didn't find an item to pick up; now the useable objects
            for( int i = 0; i < entities.Count; ++i )
            {
                var useable = entities[i] as IUseable;
                if( useable == null )
                    continue;

                if( useable.Use( player ) )
                    return;
            }
        }

        /// <summary>
        /// Saves the current progress of the game.
        /// </summary>
        /// <param name="informPlayer">
        /// States whether the player should be informed about the save process.
        /// </param>
        public void Save( bool informPlayer = true )
        {
            this.userInterface.RestoreForSaveOrExit();

            if( this.profile.Save() )
            {
                if( informPlayer )
                {
                    this.scene.FlyingTextManager.FireGameSaved( player.Transform.Position );
                }

                Settings.Instance.LastSavedProfile = this.profile.Name;
                Settings.Instance.Save();
            }
        }

        /// <summary>
        /// Attempts to change to the specified drawing pipeline.
        /// </summary>
        /// <param name="newDrawingPipeline">
        /// The DrawingPipeline to change to.
        /// </param>
        /// <returns>
        /// true if the change has been successful;
        /// otherwise false.
        /// </returns>
        public bool ChangeDrawingPipeline( Zelda.Graphics.DrawingPipeline newDrawingPipeline )
        {
            return this.game.Graphics.ChangePipeline( newDrawingPipeline );
        }

        private void OnPlayerDied( Status.Statable sender )
        {
            this.player.Latern.IsToggled = false;
            this.player.Fairy.IsEnabled = true;

            if( profile.Hardcore )
            {
                Save();
            }
        }

        #endregion

        #region > Input <

        #region - Keyboard -

        /// <summary>
        /// Called every frame to check for keyboard input.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="keyState">The current state of the keyboard.</param>
        /// <param name="oldKeyState">The state of the keyboard one frame ago.</param>
        private void OnKeyboardInput( object sender, ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            if( !this.userInterface.HasFocus )
                return;

            Keys[] pressedKeys = keyState.GetPressedKeys();
            if( this.HandleKeyboardInput_WindowsAndSettings( ref oldKeyState, pressedKeys ) )
            {
                return;
            }

            this.HandleKeyboardInput_Game( ref keyState, ref oldKeyState, pressedKeys );
        }

        /// <summary>
        /// Handles the keyboard input of the user;
        /// taking into account the keys that open/manipulate game windows and settings.
        /// </summary>
        /// <param name="oldKeyState">The state of the keyboard one frame ago.</param>
        /// <param name="pressedKeys">The keys the user currently has pressed.</param>
        /// <returns>Whether the game should stop handling input request for this frame.</returns>
        private bool HandleKeyboardInput_WindowsAndSettings(
            ref KeyboardState oldKeyState,
            Keys[] pressedKeys )
        {
            for( int i = 0; i < pressedKeys.Length; ++i )
            {
                Keys key = pressedKeys[i];
                if( this.userInterface.HandleKeyDown( key, ref oldKeyState ) )
                {
                    break;
                }

                if( key == keySettings.QuickSave && oldKeyState.IsKeyUp( keySettings.QuickSave ) )
                {
                    this.Save();
                }
                else if( key == keySettings.ToggleUserInterface && oldKeyState.IsKeyUp( keySettings.ToggleUserInterface ) )
                {
                    this.userInterface.IsVisible = !this.userInterface.IsVisible;
                }
                else if( key == keySettings.ToggleDpsMeter && oldKeyState.IsKeyUp( keySettings.ToggleDpsMeter ) )
                {
                    this.userInterface.GetElement<DamageMeterDisplay>().Toggle();
                }
                else if( key == keySettings.TakeScreenshot && oldKeyState.IsKeyUp( keySettings.TakeScreenshot ) )
                {
                    TakeScreenshot();
                }

                ////if( key == Keys.F3 && oldKeyState.IsKeyUp( Keys.F3 ) )
                ////{
                ////    // Shield_Quiver_Chicken
                ////    // Trinket_OfExploitivePower
                ////    // Weapon_Dagger_Crystal
                ////    // Relic_MysticalShell
                ////    // Shield_Quiver_Chicken" + game.Rand.RandomRange( 1, 7 ) Sword_Claymore
                ////    {
                ////        string[] items = new string[] {
                ////            "Misc_Herb_Sticky",
                ////        };

                ////        foreach( var itemName in items )
                ////        {
                ////            var item = game.ItemManager.Get( itemName );
                ////            if( item.DropRequirement != null && !item.DropRequirement.IsFulfilledBy( player ) )
                ////            {
                ////                // throw new InvalidOperationException();
                ////            }

                ////            var mapItem = new MapItem( Zelda.Items.ItemCreationHelper.Create( item, game.Rand ) );
                ////            mapItem.Transform.Position = player.Transform.Position;
                ////            mapItem.ItemInstance.Count = item.StackSize > 0 ? 1 : 0;
                ////            scene.Add( mapItem );
                ////        }
                ////    }
                ////}

                ////if( key == Keys.F4 && oldKeyState.IsKeyUp( Keys.F4 ) )
                ////{
                ////    player.Statable.AddExperience( 50000 );
                ////}
            }

            //switch( key )
            //{
            //case Keys.F4:
            //    if( oldKeyState.IsKeyUp( Keys.F4 ) )
            //    {
            //        var method = new Attacks.Methods.FixedShadowSpellDamageMethod() {
            //            DamageRange = new IntegerRange( 1, 100 )
            //        };

            //        var spell = new Zelda.Casting.FireTail( player, player, method, method, game );
            //        spell.AddToScene( scene );
            //    }
            //    break;

            //case Keys.F3:

            //    break;

            //case Keys.F4:
            //    if (oldKeyState.IsKeyUp(Keys.F4))
            //    {
            //        player.Statable.AuraList.Add( new Status.PermanentAura( new Zelda.Status.SpellHasteEffect(20.0f, Status.StatusManipType.Percental )));
            //    }
            //    break;


            //    break;

            //                    case Keys.F7:
            //                        if( oldKeyState.IsKeyUp( Keys.F7 ) )
            //                        {
            //                            string ruby =
            //                                @"bubble_text %Q/What is this?! Where is the road.. ?/, 10, fairy
            //                                  ";

            //                            Atom.Scripting.IScript script = Scripts.CreateScript( ruby, false );
            //                            script.Execute();
            //                        }
            //                        break;
            //        default:
            //            break;
            //    }
            //}

            return false;
        }

        /// <summary>
        /// Handles the keyboard input of the user;
        /// taking into account game related actions.
        /// </summary>
        /// <param name="keyState">The current state of the keyboard.</param>
        /// <param name="oldKeyState">The state of the keyboard one frame ago.</param>
        /// <param name="pressedKeys">The keys the player currently has pressed.</param>
        private void HandleKeyboardInput_Game(
            ref KeyboardState keyState,
            ref KeyboardState oldKeyState,
            Keys[] pressedKeys )
        {
            // The following input is only allowed to be handled when ..
            if( this.isPaused || !this.userInterface.HasFocus )
                return;

            // special case if the player has died
            // then he can respawn by pressing Space
            if( player.Statable.IsDead )
            {
                if( keyState.IsKeyDown( Keys.Space ) )
                {
                    if( player.Profile.Hardcore )
                    {
                        userInterface.GetElement<RespawnTextDisplay>().HideAndDisable();
                    }
                    else
                    {
                        player.RespawnWhenDead();
                    }
                }

                return;
            }

            for( int i = 0; i < pressedKeys.Length; ++i )
            {
                Keys key = pressedKeys[i];

                if( !this.HandleQuickActionKey( key, ref keyState ) )
                {
                    if( key == this.keySettings.UsePickup && oldKeyState.IsKeyUp( this.keySettings.UsePickup ) )
                    {
                        this.TryPickupOrUse();
                    }
                    else if( key == this.keySettings.UseHealingPotion && oldKeyState.IsKeyUp( this.keySettings.UseHealingPotion ) )
                    {
                        this.player.Inventory.UseLifeRestoringItem();
                    }
                    else if( key == this.keySettings.UseManaPotion && oldKeyState.IsKeyUp( this.keySettings.UseManaPotion ) )
                    {
                        this.player.Inventory.UseManaRestoringItem();
                    }
                    else if( key == this.keySettings.ToggleLatern && oldKeyState.IsKeyUp( this.keySettings.ToggleLatern ) )
                    {
                        this.player.Latern.Toggle();
                    }
                    else if( key == this.keySettings.ToggleFairy && oldKeyState.IsKeyUp( this.keySettings.ToggleFairy ) )
                    {
                        this.player.Fairy.Toggle();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the case of the user clicking one of the keys related to the Quick Action Buttons.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        /// <param name="keyState">The current state of the keyboard.</param>
        /// <returns>Whether the keys has been handled as a Quick Action key.</returns>
        private bool HandleQuickActionKey( Keys key, ref KeyboardState keyState )
        {
            bool secondRow = keyState.IsKeyDown( Keys.LeftShift ) || keyState.IsKeyDown( Keys.LeftAlt ) || keyState.IsKeyDown( Keys.LeftControl );
            return player.QuickActionSlots.Execute( key, secondRow );
        }

        #endregion

        #region - Mouse -

        /// <summary>
        /// Called once a frame to handle general mouse input.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">the state of the mouse one frame ago.</param>
        private void OnMouseInput(
            object sender,
            ref MouseState mouseState,
            ref MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                if( !this.userInterface.HasFocus || this.IsPaused )
                    return;
                if( this.userInterface.SideBar.HasButtonAt( new Point2( mouseState.X, mouseState.Y ) ) )
                    return;

                this.player.PickedupItemContainer.Drop();
            }
        }

        #endregion

        #endregion

        #region > State <

        /// <summary>
        /// Gets called the the user enters the IngameState.
        /// </summary>
        /// <param name="oldState">
        /// The old state of the game.
        /// </param>
        public void ChangedFrom( Atom.IGameState oldState )
        {
            Debug.Assert( oldState != null );

            var charScreen = oldState as CharacterSelectionState;
            if( charScreen != null )
            {
                this.ChangeProfile( charScreen.SelectedProfile );
                this.Load();
            }

            this.userInterface.HasFocus = true;
        }

        private void ChangeProfile( GameProfile profile )
        {
            this.profile = profile;
            this.Scripts.OnProfileChanged( profile );
            GameProfile.Current = profile;
        }

        /// <summary>
        /// Gets called the the user leaves the IngameState.
        /// </summary>
        /// <param name="newState">
        /// The new state of the game.
        /// </param>
        public void ChangedTo( Atom.IGameState newState )
        {
            this.userInterface.HasFocus = false;
        }

        #endregion

        #region > Misc <

        private void OnPlayerLevelUped( object sender, EventArgs e )
        {
            AutoSave();
        }

        private void OnPlayerSpawned( Entities.Components.Spawnable sender, ISpawnPoint e )
        {
            AutoSave();
        }

        private void AutoSave()
        {
            if( Settings.Instance.AutoSaveEnabled )
            {
                this.Save( informPlayer: false );
            }
        }

        private void TakeScreenshot()
        {
            try
            {
                string fileName = string.Format( "{0}-{1}.png", profile.Name, DateTime.Now.ToString( "yyMMddHHmmss" ) );
                string folderPath = GameFolders.Screenshots;
                string filePath = Path.Combine( folderPath, fileName );
                Directory.CreateDirectory( folderPath );

                using( FileStream fs = new FileStream( filePath, FileMode.OpenOrCreate ) )
                {
                    Microsoft.Xna.Framework.Graphics.RenderTarget2D target = game.GetService<Zelda.Graphics.IViewToWindowRescaler>().Target;
                    target.SaveAsPng( fs, target.Width, target.Height ); // save render target to disk
                }

                scene.FlyingTextManager.FireScreenshotTaken( player.Collision.Center, string.Format( "{0} Saved", fileName ) );
            }
            catch( Exception ex )
            {
                game.ReportError( ex, isFatal: false );
            }
        }

        /// <summary>
        /// Immediatly releases the unmanaged resources used by this IngameState.
        /// </summary>
        public void Dispose()
        {
            this.userInterface.Dispose();
        }

        /// <summary>
        /// Exists this IngameState to the CharacterSelectionState. 
        /// </summary>
        public void ExitToCharacterSelectionState()
        {
            if( this.scene != null )
            {
                this.scene.NotifySceneChange( ChangeType.Away );
                this.scene = null;
            }

            this.player = null;
            this.worldStatus = null;
            this.profile = null;
            GameProfile.Current = null;

            this.inactiveSceneUpdater.Clear();

            this.BackgroundMusic.Stop();
            this.game.GetService<FlyingTextManager>()
                .Clear();

            this.game.States.PopAll();
            this.game.States.Push<CharacterSelectionState>();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whether the game is currently paused.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// The current player.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The profile of the player.
        /// </summary>
        private GameProfile profile;

        /// <summary>
        /// The current scene of the game.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The current state of the game world.
        /// </summary>
        private WorldStatus worldStatus;

        /// <summary>
        /// Identifies the currently active <see cref="KeySettings"/>.
        /// </summary>
        private KeySettings keySettings;

        /// <summary>
        /// The GameGraphics object that holds all graphics-related objects.
        /// </summary>
        private readonly Zelda.Graphics.GameGraphics graphics;

        /// <summary>
        /// Reponsible for updating scenes after they have gone inactive; for example
        /// when the player switches the current zone.
        /// </summary>
        private readonly InactiveSceneUpdater inactiveSceneUpdater = new InactiveSceneUpdater();

        /// <summary>
        /// The UI shown in the Ingame State.
        /// </summary>
        private readonly IngameUserInterface userInterface;

        /// <summary>
        /// Stores a reference to the ZeldaGame object.
        /// </summary>
        private readonly ZeldaGame game;

        /// <summary>
        /// Manages the background music of the game.
        /// </summary>
        private readonly Zelda.Audio.BackgroundMusicComponent backgroundMusic;

        /// <summary>
        /// The Atom.Fmod.AudioSystem object.
        /// </summary>
        private readonly Atom.Fmod.AudioSystem audioSystem;

        #endregion
    }
}