// <copyright file="BossBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.BossPolarity class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Zelda.Events;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines the IEntityBehaviour of the hell chicken boss.
    /// </summary>
    /// <remarks>
    /// The main mechanic of this boss is that he changes 'polarity' every
    /// ~10 seconds. Adds are spawned on the change.
    /// <seealso cref="BossPolarity"/>
    /// </remarks>
    public sealed partial class HellChickenBossBehaviour : MeleeEnemyBehaviour
    {
        #region [ Constants ]

        /// <summary>
        /// The name of the background music resource
        /// that is playing when the players fights the boss.
        /// </summary>
        private const string BossMusicName = "Caves.mp3";

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HellChickenBossBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal HellChickenBossBehaviour( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HellChickenBossBehaviour"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The entity that is controlled by the new HellChickenBossBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="enemy"/> is null.
        /// </exception>
        public HellChickenBossBehaviour( Enemy enemy, IZeldaServiceProvider serviceProvider )
            : base( enemy, serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( enemy != null );

            this.boss = enemy;
            this.serviceProvider = serviceProvider;
            this.ingameState = (IIngameState)serviceProvider.GetService( typeof( IIngameState ) );

            this.polarityBehaviour = new PolarityBehaviour( this.boss, serviceProvider );
            this.increaseSizeBehaviour = new IncreaseSizeAndStrengthOnPolarityChangeBehaviour( this.polarityBehaviour, this.boss );
            this.spawnAddsBehaviour = new SpawnAddsOnPolarityChangeBehaviour( this.polarityBehaviour, this.boss, serviceProvider );

            this.HookBossEvents();
        }

        /// <summary>
        /// Hooks up the events of this HellChickenBossBehaviour.
        /// </summary>
        private void HookBossEvents()
        {
            this.boss.Statable.Died += this.OnBossDied;
            this.boss.Attackable.Attacked += this.OnBossAttacked;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this HellChickenBossBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( this.State == BehaviourState.ChasingPlayer )
            {
                this.polarityBehaviour.Update( updateContext );
            }
        }

        /// <summary>
        /// Plays the background music of the boss fight.
        /// </summary>
        private void PlayBossMusic()
        {
            // Receive.
            var music = this.serviceProvider.AudioSystem.GetMusic( BossMusicName );
            if( music == null )
                return;

            // Load.
            music.LoadAsMusic( false );

            // Play.
            var backgroundMusic = ingameState.BackgroundMusic;

            backgroundMusic.Mode = Zelda.Audio.BackgroundMusicMode.Loop;
            backgroundMusic.ChangeTo( music );
        }

        /// <summary>
        /// Tells the game to start playing normal music again.
        /// </summary>
        private void PlayNormalMusicAgain()
        {
            var backgroundMusic = this.ingameState.BackgroundMusic;
            backgroundMusic.Mode = Zelda.Audio.BackgroundMusicMode.Random;
            backgroundMusic.ChangeToRandom();
        }

        #region > Events <

        /// <summary>
        /// Gets called when the BehaviourState of this StoneTombsBossBehaviour
        /// has changed.
        /// </summary>
        /// <param name="oldState">
        /// The old BehaviourState of the boss.
        /// </param>
        /// <param name="newState">
        /// The new BehaviourState of the boss.
        /// </param>
        protected override void OnStateChanged( BehaviourState oldState, BehaviourState newState )
        {
            if( this.ingameState == null )
                return;

            switch( newState )
            {
                case BehaviourState.ChasingPlayer:
                    this.ChangeToDarkLight();
                    this.HookEvents();
                    this.PlayBossMusic();
                    break;

                default:
                    this.PlayNormalMusicAgain();
                    this.UnhookEvents();
                    this.ChangeToBrightLight();
                    break;
            }
        }

        /// <summary>
        /// Darkens the ambient light of the scene.
        /// </summary>
        private void ChangeToDarkLight()
        {
            var e = new InterpolateSceneAmbientEvent() {
                Name = "DarkenLight",
                AmbientColor = new Color( 153, 46, 46, 255 ),
                Time = 5.0f
            };

            if( this.boss.Scene.EventManager.TryAdd( e ) )
            {
                e.Trigger( this.boss );
            }
        }

        /// <summary>
        /// Lightens the ambient light of the scene.
        /// </summary>
        private void ChangeToBrightLight()
        {
            if( this.boss == null || this.boss.Scene == null )
            {
                return;
            }

            var e = new InterpolateSceneAmbientEvent() {
                Name = "LightenScene",
                AmbientColor = new Color( 75, 73, 73, 255 ),
                Time = 5.0f
            };

            if( this.boss.Scene.EventManager.TryAdd( e ) )
            {
                e.Trigger( this.boss );
            }
        }

        /// <summary>
        /// Hooks up extra events when the boss is starting to chase the player.
        /// </summary>
        private void HookEvents()
        {
            var player = this.ingameState.Player;
            if( player == null )
                return;

            player.Statable.Died += this.OnPlayerDied;
        }

        /// <summary>
        /// Unhooks the extra events when the boss is starting to wander again.
        /// </summary>
        private void UnhookEvents()
        {
            var player = this.ingameState.Player;
            if( player == null )
                return;

            player.Statable.Died -= this.OnPlayerDied;
        }

        /// <summary>
        /// Gets called when the player has died.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnPlayerDied( Zelda.Status.Statable sender )
        {
            this.FullyHealUpBoss();

            this.polarityBehaviour.ResetPolarity();
            this.spawnAddsBehaviour.DespawnAll();
            this.increaseSizeBehaviour.Reset();
        }

        /// <summary>
        /// Fully heals up the boss. Happens when the player dies.
        /// </summary>
        private void FullyHealUpBoss()
        {
            this.boss.Statable.AuraList.Clear();
            this.boss.Statable.Life = this.boss.Statable.MaximumLife;
        }

        /// <summary>
        /// Gets called when the player has attacked the boss.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The AttackedEventArgs that contains the event data.
        /// </param>
        private void OnBossAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            if( this.boss == null )
                return;
        }

        /// <summary>
        /// Gets called when the player managed to defeat the boss.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnBossDied( Zelda.Status.Statable sender )
        {
            if( this.boss == null )
                return;

            var scene = this.boss.Scene;
            if( scene == null )
                return;

            this.polarityBehaviour.ResetPolarity();

            // Make path free.
            var bossKilledEvent = scene.EventManager.GetEvent( "BossKilled" );
            if( bossKilledEvent != null )
                bossKilledEvent.Trigger( this.boss );

            this.PlayNormalMusicAgain();
            this.UnhookEvents();
            this.ChangeToBrightLight();
        }

        #endregion

        /// <summary>
        /// Returns a clone of this HellChickenBossBehaviour.
        /// </summary>
        /// <param name="newOwner">
        /// The owner of the cloned HellChickenBossBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public override IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new HellChickenBossBehaviour( (Enemy)newOwner, this.serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Controls the polarity behaviour of the hell chicken boss.
        /// </summary>
        private readonly PolarityBehaviour polarityBehaviour;

        /// <summary>
        /// Controls how additional adds spawn when the polarity of the boss changes.
        /// </summary>
        private readonly SpawnAddsOnPolarityChangeBehaviour spawnAddsBehaviour;

        /// <summary>
        /// Controls how the boss increases in size when the polarity of the boss changes.
        /// </summary>
        private readonly IncreaseSizeAndStrengthOnPolarityChangeBehaviour increaseSizeBehaviour;

        /// <summary>
        /// Identifies the Enemy entity that goes after the player.
        /// </summary>
        private readonly Enemy boss;

        /// <summary>
        /// Identifies the ingame state.
        /// </summary>
        private readonly IIngameState ingameState;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
