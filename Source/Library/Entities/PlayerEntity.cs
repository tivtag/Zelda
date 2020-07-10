// <copyright file="PlayerEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.PlayerEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Items;
    using Zelda.Profiles;
    using Zelda.Skills;
    using Zelda.Status;

    /// <summary>
    /// Defines the ZeldaEntity the player is controlling; Link.
    /// This class can't be inherited.
    /// </summary>
    public sealed class PlayerEntity : ZeldaEntity, INotifyKilledEntity, IAttackableEntity, ILaternOwner, IMoveableEntity, ISpawnableEntity
    {
        #region [ Events ]

        /// <summary>
        /// Fired when this PlayerEntity has respawned after dieing.
        /// </summary>
        public event EventHandler Respawned;

        /// <summary>
        /// Fired when this PlayerEntity has killed an <see cref="ZeldaEntity"/>.
        /// </summary>
        public event Atom.RelaxedEventHandler<ZeldaEntity> EntityKilled;

        /// <summary>
        /// Fired when this PlayerEntity has collected an <see cref="Item"/>.
        /// </summary>
        public event Atom.RelaxedEventHandler<Item> ItemCollected;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Inventory"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Inventory Inventory
        {
            get
            {
                return this.inventory;
            }
        }

        /// <summary>
        /// Gets the magic <see cref="Crafting.CraftingBottle"/> of this PlayerEntity.
        /// </summary>
        public Crafting.CraftingBottle CraftingBottle
        {
            get
            {
                return this.craftingBottle;
            }
        }

        /// <summary>
        /// Gets the <see cref="EquipmentStatus"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public EquipmentStatus Equipment
        {
            get
            {
                return this.statable.Equipment;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Talents.TalentTree"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Zelda.Talents.TalentTree TalentTree
        {
            get
            {
                return this.talentTree;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Ocarina.OcarinaBox"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Ocarina.OcarinaBox OcarinaBox
        {
            get
            {
                return this.ocarinaBox;
            }
        }

        /// <summary>
        /// Gets a component that manages the <see cref="Skills"/> this <see cref="PlayerEntity"/> has aquired.
        /// </summary>
        public Skillable Skills
        {
            get
            {
                return this.skillable;
            }
        }

        /// <summary>
        /// Gets the list of QuickActionSlots of this PlayerEntity.
        /// </summary>
        public QuickActions.QuickActionSlotList QuickActionSlots
        {
            get
            {
                return this.quickActionSlots;
            }
        }

        /// <summary>
        /// Gets the <see cref="Quests.QuestLog"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Quests.QuestLog QuestLog
        {
            get
            {
                return this.questLog;
            }
        }

        /// <summary>
        /// Gets the <see cref="Factions.FactionStates"/> of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Factions.FactionStates FactionStates
        {
            get
            {
                return this.factionStates;
            }
        }

        /// <summary>
        /// Gets the IDrawDataAndStrategy that is responsible for drawing this PlayerEntity.
        /// </summary>
        public new Drawing.PlayerDrawDataAndStrategy DrawDataAndStrategy
        {
            get
            {
                return this.playerDrawDaS;
            }
        }

        /// <summary>
        /// Gets or sets the object that controls how the Player interacts with the PlayerEntity.
        /// </summary>
        private Zelda.Core.Controls.PlayerControl Control
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the container object which stores what <see cref="Item"/>
        /// this <see cref="PlayerEntity"/> currently has picked up.
        /// </summary>
        public PickedupItemContainer PickedupItemContainer
        {
            get
            {
                return this.pickedupItemContainer;
            }
        }

        /// <summary>
        /// Gets the localized class name of this PlayerEntity.
        /// </summary>
        public Zelda.Talents.Classes.CharacterClass Class
        {
            get
            {
                return this.talentTree.Class;
            }
        }

        public string ClassName
        {
            get
            {
                var c = this.talentTree.Class;
                return c != null ? (profile.Hardcore ? c.LocalizedHardcoreName : c.LocalizedName) : string.Empty;
            }
        }

        /// <summary>
        /// Gets the <see cref="Latern"/> of this PlayerEntity.
        /// </summary>
        public Latern Latern
        {
            get
            {
                return this.lantern;
            }
        }

        /// <summary>
        /// Gets the <see cref="GameProfile"/> that stores information about the PlayerEntity and his adventure.
        /// </summary>
        public GameProfile Profile
        {
            get
            {
                return this.profile;
            }
        }

        /// <summary>
        /// Gets the status of this PlayerEntity's world.
        /// </summary>
        public Saving.WorldStatus WorldStatus
        {
            get
            {
                return this.profile.WorldStatus;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IIngameState"/> object
        /// that provides access to ingame-related functionality.
        /// </summary>
        public IIngameState IngameState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Items.SharedChest"/> of this PlayerEntity.
        /// </summary>
        public SharedChest SharedChest
        {
            get
            {
                return this.sharedChest;
            }

            internal set
            {
                this.sharedChest = value;
            }
        }

        public Fairy Fairy
        {
            get
            {
                return this.fairy;
            }
        }

        public PlayerStatistics Statistics
        {
            get
            {
                return this.statistics;
            }
        }

        #region > State <

        /// <summary>
        /// Gets a value indicating whether this PlayerEntity 
        /// is currently casting a Spell.
        /// </summary>
        public bool IsCasting
        {
            get
            {
                return this.castable.CastBar.IsCasting;
            }
        }

        public bool IsDead
        {
            get
            {
                return Statable.IsDead;
            }
        }

        #endregion

        #region > Components <

        /// <summary>
        /// Gets the <see cref="Moveable"/> component of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Moveable Moveable
        {
            get
            {
                return this.moveable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Statable"/> component of this <see cref="PlayerEntity"/>.
        /// </summary>
        public ExtendedStatable Statable
        {
            get
            {
                return this.statable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Attackable"/> component of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Attackable Attackable
        {
            get
            {
                return this.attackable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Casting.Castable"/> component of this <see cref="PlayerEntity"/>.
        /// </summary>
        public Zelda.Casting.Castable Castable
        {
            get
            {
                return this.castable;
            }
        }

        public Spawnable Spawnable
        {
            get
            {
                return this.spawnable;
            }
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEntity"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the new PlayerEntity.
        /// </param>
        /// <param name="profile">
        /// The profile that stores information about the PlayerEntity and his adventure.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private PlayerEntity( string name, GameProfile profile, IZeldaServiceProvider serviceProvider )
            : base( 8 )
        {
            this.profile = profile;

            const float BaseMovementSpeed = 62.0f;

            // Create additional components:
            this.moveable = new Moveable() {
                CanSwim = true,
                BaseSpeed = BaseMovementSpeed,
                Speed = BaseMovementSpeed,
                CanCurrentlySwimStateFunction = this.CanSwimStateFunction
            };
            this.statistics = new PlayerStatistics( this );

            this.SetupComponents( serviceProvider );

            // Add components:
            this.Components.BeginSetup();
            {
                this.Components.Add( this.moveable );
                this.Components.Add( this.statable );
                this.Components.Add( this.castable );
                this.Components.Add( this.attackable );
                this.Components.Add( this.skillable );
                this.Components.Add( new PlayerMultiFrameAttackTracker() );
                this.Components.Add( this.statistics );
            }
            this.Components.EndSetup();

            // DrawDataAndStrategy:
            this.playerDrawDaS = new Drawing.PlayerDrawDataAndStrategy( this, serviceProvider );
            base.DrawDataAndStrategy = this.playerDrawDaS;

            // Create other objects.
            this.pickedupItemContainer = new PickedupItemContainer( this );
            this.inventory = new Inventory( this );
            this.craftingBottle = new Zelda.Crafting.CraftingBottle( this );
            this.talentTree = new Zelda.Talents.TalentTree( this, serviceProvider );
            this.ocarinaBox = new Zelda.Ocarina.OcarinaBox( this, serviceProvider );
            this.factionStates = new Zelda.Factions.FactionStates();
            this.questLog = new Zelda.Quests.QuestLog( this, serviceProvider );

            // Skills:
            var meleeSkill = new Zelda.Skills.Melee.NormalMeleeAttackSkill( this, serviceProvider );
            var rangedSkill = new Zelda.Skills.Ranged.NormalRangedAttackSkill( this, serviceProvider );

            this.skillable.Learn( meleeSkill );
            this.skillable.Learn( rangedSkill );

            this.quickActionSlots = new Zelda.QuickActions.QuickActionSlotList( this, profile.KeySettings );
            this.quickActionSlots.AddSkill( meleeSkill );
            this.quickActionSlots.AddSkill( rangedSkill );

            this.lantern = new Latern( this );
            this.fairy = new Fairy( this );

            this.Name = name;
            this.HookEvents();
        }

        /// <summary>
        /// Setups the components of this PlayerEntity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void SetupComponents( IZeldaServiceProvider serviceProvider )
        {
            this.statable.Setup( serviceProvider );
            this.statable.IsFriendly = true;
            this.statable.AuraList.CaptureVisibleAuras = true;

            this.Collision.Set( new Vector2( 0.0f, 11.0f ), new Vector2( 16.0f, 11.0f ) );
        }

        /// <summary>
        /// Hooks up this PlayerEntity with the events of various components.
        /// </summary>
        private void HookEvents()
        {
            this.statable.Died += this.OnDied;
            this.statable.Damaged += this.OnDamaged;
            this.statable.Restored += this.OnLifeRestored;
            this.statable.LevelUped += this.OnLevelUp;
            this.statable.RestoredMana += this.OnManaRestored;
            this.statable.ExperienceGained += this.OnExperienceGained;

            this.questLog.QuestAccepted += this.OnQuestAccepted;
            this.questLog.QuestAccomplished += this.OnQuestAccomplished;
            this.Transform.PositionChanged += this.OnPositionChanged;
            this.factionStates.ReputationLevelChanged += this.OnReputationLevelChanged;
        }
        
        /// <summary>
        /// Loads the content used by this PlayerEntity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void LoadContent( IZeldaServiceProvider serviceProvider )
        {
            var content = serviceProvider.Content;

            this.lantern.LoadContent( serviceProvider );
            this.playerDrawDaS.Load( serviceProvider );
            this.fairy.Setup( serviceProvider );
        }

        /// <summary>
        /// Creates a new <see cref="PlayerEntity"/> which is set-up to start a new adventure.
        /// </summary>
        /// <param name="playerName">
        /// The name of the new PlayerEntity.
        /// </param>
        /// <param name="profile">
        /// The profile that owns the new PlayerEntity.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly created PlayerEntity.
        /// </returns>
        public static PlayerEntity StartNewAdventure(
            string playerName,
            GameProfile profile,
            IZeldaServiceProvider serviceProvider )
        {
            PlayerEntity player = new PlayerEntity( playerName, profile, serviceProvider );

            player.LoadContent( serviceProvider );
            player.Control = new Zelda.Core.Controls.PlayerControl( player, profile.KeySettings );

            player.statable.SetupInitialStatus();
            player.talentTree.Statistics.UpdateClass();

            // Equip starting equipment:
            var itemManager = serviceProvider.ItemManager;
            EquipmentInstance dummy;

            player.Equipment.Equip(
                (EquipmentInstance)itemManager.Get( "Staff_Ward_KokirianNovice" ).CreateInstance(),
                EquipmentStatusSlot.Staff,
                out dummy
            );

            player.RefreshStatus();
            player.statable.Life = player.statable.MaximumLife / 3;
            player.statable.Mana = 0;
            player.LoadSharedChest( serviceProvider );

            return player;
        }
        
        /// <summary>
        /// Creates a new PlayerEntity that already has started an adventure.
        /// </summary>
        /// <param name="playerName">
        /// The name of the player.
        /// </param>
        /// <param name="profile">
        /// The profile that owns the new PlayerEntity.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The newly loaden PlayerEntity.
        /// </returns>
        internal static PlayerEntity CreateExisting(
            string playerName,
            GameProfile profile,
            IZeldaServiceProvider serviceProvider )
        {
            PlayerEntity player = new PlayerEntity( playerName, profile, serviceProvider );

            player.LoadContent( serviceProvider );
            player.Control = new Zelda.Core.Controls.PlayerControl( player, profile.KeySettings );

            return player;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="PlayerEntity"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( !IsDead )
            {
                this.Control.Update( updateContext );
            }

            base.Update( updateContext );
        }

        #region > Events <

        /// <summary>
        /// Gets called when this PlayerEntity has 'died'.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnDied( Statable sender )
        {
            this.moveable.CanMove = false;
            this.statable.IsInvincible = true;

            this.playerDrawDaS.ResetAnimationDead();
        }

        /// <summary>
        /// Gets called when this PlayerEntity has restored some of its life.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The Zelda.Attacks.AttackDamageResult that contains the event data.</param>
        private void OnLifeRestored( object sender, Zelda.Attacks.AttackDamageResult e )
        {
            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 20;

            if( e.AttackReceiveType == AttackReceiveType.Crit )
                flyingTextManager.FireRestoredLifeCrit( position, e.Damage );
            else
                flyingTextManager.FireRestoredLife( position, e.Damage );
        }

        /// <summary>
        /// Gets called when this PlayerEntity has restored some of its mana.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The Zelda.Attacks.AttackDamageResult that contains the event data.</param>
        private void OnManaRestored( object sender, Zelda.Attacks.AttackDamageResult e )
        {
            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireRestoredMana( position, e.Damage );
        }

        /// <summary>
        /// Gets called when this PlayerEntity has got a level-up.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnLevelUp( object sender, EventArgs e )
        {
            this.talentTree.GainTalentPointsOnLevelUp();

            // Show ingame text.
            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireLevelUp( position );

            // Play sound sample.
            /*
            var sample = this.audioSystem.GetSample( "LevelUpFanfare.ogg" );

            if( sample != null )
            {
                sample.LoadAsSample( false );

                var channel = sample.Play( true );
                channel.Volume = 0.75f;
                channel.IsPaused = false;
            }
            */
        }

        /// <summary>
        /// Gets called when this <see cref="PlayerEntity"/> managed to kill a <see cref="Killable"/> ZeldaEntity.
        /// </summary>
        /// <param name="killable">
        /// The component that indentifies the ZeldaEntity that has been killed.
        /// </param>
        void INotifyKilledEntity.NotifyKilled( Killable killable )
        {
            if( this.EntityKilled != null )
                this.EntityKilled( this, killable.Owner );

            this.statable.AddExperienceModified( killable.Experience );
        }

        /// <summary>
        /// Called when the player has gained experience.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="experience">The amount of experience gained by the player.</param>
        private void OnExperienceGained( object sender, long experience )
        {
            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireGainedExperience( position, experience );
        }

        /// <summary>
        /// Called when the player has been damaged.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The Zelda.Attacks.AttackDamageResult that contains the event data.</param>
        private void OnDamaged( object sender, Zelda.Attacks.AttackDamageResult e )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            switch( e.AttackReceiveType )
            {
                case Zelda.Attacks.AttackReceiveType.Hit:
                    flyingTextManager.FireAttackOnPlayerHit( position, e.Damage, e.WasBlocked );
                    break;

                case Zelda.Attacks.AttackReceiveType.Crit:
                    flyingTextManager.FireAttackOnPlayerCrit( position, e.Damage, e.WasBlocked );
                    break;

                case Zelda.Attacks.AttackReceiveType.PartialResisted:
                    flyingTextManager.FireAttackOnPlayerPartiallyResisted( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.Miss:
                    flyingTextManager.FireAttackOnPlayerMissed( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Dodge:
                    flyingTextManager.FireAttackOnPlayerDodged( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Parry:
                    flyingTextManager.FireAttackOnPlayerParried( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Resisted:
                    flyingTextManager.FireAttackOnPlayerResisted( position );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called when the player has accepted a new Quest.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="quest">The related Quest.</param>
        private void OnQuestAccepted( object sender, Zelda.Quests.Quest quest )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireQuestAccepted( quest.LocalizedName, position );
        }

        /// <summary>
        /// Called when the player has accomplished all quals of a Quest.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="quest">The related Quest.</param>
        private void OnQuestAccomplished( object sender, Zelda.Quests.Quest quest )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireQuestAccomplished( quest.LocalizedName, position );
        }

        /// <summary>
        /// Called when the player has changed position.
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The Atom.ChangedValue{Vector2} that contains the event data.</param>
        private void OnPositionChanged( object sender, Atom.ChangedValue<Vector2> e )
        {
            var scene = this.Scene;
            if( scene == null )
                return;

            var fow = scene.Status.FogOfWar;
            var position = e.NewValue;

            // Uncover Player Position.
            fow.Uncover( position, scene );

            const float Size = 45.0f;

            // Uncover the four corners.
            fow.Uncover( position + new Vector2( Size, 0.0f ), scene );
            fow.Uncover( position - new Vector2( Size, 0.0f ), scene );
            fow.Uncover( position + new Vector2( 0.0f, Size ), scene );
            fow.Uncover( position - new Vector2( 0.0f, Size ), scene );
        }

        /// <summary>
        /// Called when the player has gained in reputation level. 
        /// </summary>
        /// <param name="sender">The sender of the event </param>
        /// <param name="e">The Zelda.Factions.ReputationLevelChangedEventArgs that contains the event data.</param>
        private void OnReputationLevelChanged( object sender, Zelda.Factions.ReputationLevelChangedEventArgs e )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            flyingTextManager.FireReputationLevelChanged( e.Faction.LocalizedName, e.NewLevel, position );
        }

        #endregion

        #region > Other <

        private void RefreshStatus()
        {
            statable.RefreshStatus();
            lantern.Refresh();
        }

        /// <summary>
        /// Collects the give ItemInstance, adding it to the Inventory of the player.
        /// </summary>
        /// <remarks>
        /// The <see cref="ItemCollected"/> event is fired upon collection.
        /// </remarks>
        /// <param name="itemInstance">
        /// The ItemInstance to collect.
        /// </param>
        /// <returns>
        /// True if the ItemInstance could successfully collected;
        /// otherwise false.
        /// </returns>
        internal bool Collect( ItemInstance itemInstance )
        {
            Item item = itemInstance.Item;

            if( item.StackSize <= 0 )
            {
                if( item.UseEffect != null )
                {
                    item.UseEffect.Use( this );
                }

                this.Statable.Rubies += item.RubiesWorth;
            }
            else
            {
                if( !this.Inventory.Insert( itemInstance ) )
                {
                    return false;
                }
            }

            this.ItemCollected.Raise( this, item );
            return true;
        }

        /// <summary>
        /// Respawns this PlayerEntity after the player has died.
        /// </summary>
        /// <exception cref="Atom.NotFoundException">
        /// If the spawn point of the last save point couldn't be found and 
        /// no alternative <see cref="Zelda.Entities.Spawning.PlayerSpawnPoint"/> could be found either.
        /// </exception>
        public void RespawnWhenDead()
        {
            if( !statable.IsDead )
                return;

            // Apply dead penality
            statable.Rubies -= statable.Rubies / 15;

            // Heal up
            statable.Life = statable.MaximumLife / 3;
            statable.Mana = statable.MaximumMana / 3;

            RespawnAtLastSavePoint();
            moveable.CanMove = true;
            moveable.ResetPush();
            statable.MakeTempInvincible( 2.5f );

            this.Respawned.Raise( this );
        }

        /// <summary>
        /// Respawns the player at the last used save point.
        /// </summary>
        private void RespawnAtLastSavePoint()
        {
            var scene = this.Scene;
            var savePoint = this.profile.LastSavePoint;

            if( scene == null || scene.Name != savePoint.Scene )
            {
                scene = this.IngameState.RequestSceneChange( savePoint.Scene, true );
            }

            var spawnPoint = GetRespawnSpawnPoint( scene, savePoint );
            spawnPoint.Spawn( this );
        }

        /// <summary>
        /// Gets the spawn point at which the player should respawn.
        /// </summary>
        /// <param name="scene">
        /// The scene that is supposed to contain the ISpawnPoint.
        /// </param>
        /// <param name="savePoint">
        /// The last used save point.
        /// </param>
        /// <returns>
        /// The spawnpoint to use.
        /// </returns>
        private static Zelda.Entities.Spawning.ISpawnPoint GetRespawnSpawnPoint( ZeldaScene scene, Zelda.Saving.SavePoint savePoint )
        {
            var spawnPoint = scene.GetSpawnPoint( savePoint.SpawnPoint );

            if( spawnPoint == null )
            {
                spawnPoint = scene.GetEntity<Zelda.Entities.Spawning.PlayerSpawnPoint>();

                if( spawnPoint == null )
                {
                    spawnPoint = scene.GetEntity<Zelda.Entities.Spawning.ISpawnPoint>();

                    if( spawnPoint == null )
                    {
                        throw new Atom.NotFoundException(
                            string.Format(
                                System.Globalization.CultureInfo.CurrentCulture,
                                Resources.Error_SpawnPointXOrAlternativeNotFound,
                                savePoint.SpawnPoint ?? string.Empty
                            )
                        );
                    }
                }
            }
            return spawnPoint;
        }

        /// <summary>
        /// Defines the function that is used by the Moveable component to determine
        /// whether this PlayerEntity can currently swim.
        /// </summary>
        /// <returns>
        /// Whether the PlayerEntity can currently swim.
        /// </returns>
        private bool CanSwimStateFunction()
        {
            return this.playerDrawDaS.SpecialAnimation == PlayerSpecialAnimation.None;
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This operation is not supported.
        /// </exception>
        /// <returns>
        /// This operation doesn't return, but always throws NotSupportedException.
        /// </returns>
        public override ZeldaEntity Clone()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes/Writes the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 9;
            context.Write( CurrentVersion );

            context.Write( this.Name );
            context.Write( this.lantern.IsToggled );
            context.Write( this.fairy.IsEnabled ); // New in Version 8

            this.statable.SerializeExtended( context );
            this.talentTree.Serialize( context );
            this.factionStates.SerializeState( context );
            this.inventory.Serialize( context );
            this.Equipment.Serialize( context );
            this.quickActionSlots.Serialize( context );
            this.craftingBottle.Serialize( context ); // New in Version 3!
            this.ocarinaBox.Serialize( context );     // New in Version 4!
            this.questLog.Serialize( context );       // moved down in Version 6.
            SharedChest.Save( this.sharedChest, context.ServiceProvider );

            this.statable.SerializePowerStatus( context );
            this.statistics.Serialize( context ); // New in Version 9!
        }

        /// <summary>
        /// Deserializes/Reads the current state of this IStateSaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 9;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 2, CurrentVersion, this.GetType() );

            this.Name = context.ReadString();
            this.lantern.IsToggled = context.ReadBoolean();

            if( version >= 8 )
            {
                this.fairy.IsEnabled = context.ReadBoolean(); // New in Version 8
            }

            this.statable.DeserializeExtended( context );
            this.talentTree.Deserialize( context );
            this.factionStates.DeserializeState( context );

            if( version == 5 )
                this.questLog.Deserialize( context );

            this.inventory.Deserialize( context );
            this.Equipment.Deserialize( context );
            this.quickActionSlots.Deserialize( context );
            this.craftingBottle.Deserialize( context );

            if( version >= 4 )
            {
                this.ocarinaBox.Deserialize( context );
            }

            if( version >= 6 )
            {
                this.questLog.Deserialize( context );
            }

            this.RefreshStatus();
            this.LoadSharedChest( context.ServiceProvider );

            if( version >= 7 )
            {
                this.statable.DeserializePowerStatus( context );
            }

            if( version >= 9 )
            {
                this.statistics.Deserialize( context ); // New in Version 9
            }
        }

        /// <summary>
        /// Loads the content of the SharedChest from the HD and assigns it
        /// it the SharedChest of this PlayerEntity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void LoadSharedChest( IZeldaServiceProvider serviceProvider )
        {
            this.sharedChest = SharedChest.Load( this, serviceProvider );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The SharedChest represents a storage place for items that are shared over -all- characters.
        /// </summary>
        private SharedChest sharedChest;

        /// <summary>
        /// The companion of the player.
        /// </summary>
        private readonly Fairy fairy;

        /// <summary>
        /// The inventory of the PlayerEntity.
        /// </summary>
        private readonly Inventory inventory;

        /// <summary>
        /// The magic crafting bottle of the PlayerEntity.
        /// </summary>
        private readonly Crafting.CraftingBottle craftingBottle;

        /// <summary>
        /// The TalentTree of the PlayerEntity.
        /// </summary>
        private readonly Talents.TalentTree talentTree;

        /// <summary>
        /// The QuestLog of the PlayerEntity.
        /// </summary>
        private readonly Quests.QuestLog questLog;

        /// <summary>
        /// The OcarinaBox of the PlayerEntity.
        /// </summary>
        private readonly Ocarina.OcarinaBox ocarinaBox;

        /// <summary>
        /// The latern that emits light for this PlayerEntity.
        /// </summary>
        private readonly Latern lantern;

        /// <summary>
        /// The lists that contains the QuickActionSlots of this PlayerEntity.
        /// </summary>
        private readonly QuickActions.QuickActionSlotList quickActionSlots;

        /// <summary>
        /// Stores the state of the player towards the <see cref="Factions.Faction"/>s in the game.
        /// </summary>
        private readonly Factions.FactionStates factionStates;

        /// <summary>
        /// Stores to the <see cref="Drawing.PlayerDrawDataAndStrategy"/> object.
        /// </summary>
        private readonly Drawing.PlayerDrawDataAndStrategy playerDrawDaS;

        /// <summary>
        /// Stores the Item which the player currently has picked up.
        /// </summary>
        private readonly PickedupItemContainer pickedupItemContainer;

        /// <summary>
        /// The profile that stores information about the PlayerEntity and his adventure.
        /// </summary>
        private readonly GameProfile profile;

        /// <summary>
        /// Encapsulates various playing statistics.
        /// </summary>
        private readonly PlayerStatistics statistics;

        #region > Components <

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of this PlayerEntity.
        /// </summary>
        private readonly Moveable moveable;

        /// <summary>
        /// Identifies the <see cref="ExtendedStatable"/> component of this PlayerEntity.
        /// </summary>
        private readonly ExtendedStatable statable = new ExtendedStatable();

        /// <summary>
        /// Identifies the <see cref="Skillable"/> component of this PlayerEntity.
        /// </summary>
        private readonly Skillable skillable = new Skillable();

        /// <summary>
        /// Identifies the <see cref="Attackable"/> component of this PlayerEntity.
        /// </summary>
        private readonly Attackable attackable = new Attackable();

        /// <summary>
        /// Identifies the <see cref="Zelda.Casting.Castable"/> component of this PlayerEntity.
        /// </summary>
        private readonly Zelda.Casting.Castable castable = new Zelda.Casting.Castable();

        private readonly Spawnable spawnable = new Spawnable();

        #endregion

        #endregion
    }
}