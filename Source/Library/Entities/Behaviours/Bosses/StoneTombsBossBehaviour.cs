// <copyright file="StoneTombsBossBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.StoneTombsBossBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses
{
    using System;
    using Atom.Diagnostics.Contracts;
    using System.Globalization;
    using Atom.Math;
    using Zelda.Entities.Spawning;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Represents the behaviour script of the end boss of the Family Stone Tombs.
    /// </summary>
    /// <remarks>
    /// As the player fights the boss enemies randomly spawn in the boss room.
    /// </remarks>
    public sealed class StoneTombsBossBehaviour : MeleeEnemyBehaviour
    {
        #region [ Constants ]

        /// <summary>
        /// The time until the first enemy wave.
        /// The time between the other waves is also based on the WaveTime.
        /// </summary>
        private const float WaveTime = 8.0f;

        /// <summary>
        /// The name of the background music resource
        /// that is playing when the players fights the boss.
        /// </summary>
        private const string BossMusicName = "MajorasWrath.mid";

        /// <summary>
        /// The variable shadow damage of the fire tail attack.
        /// </summary>
        private static readonly IntegerRange ShadowTailDamageRange = new IntegerRange( 50, 150 );
        
        /// <summary>
        /// The variable shadow damage of the explosion part of the fire tail attack.
        /// </summary>
        private static readonly IntegerRange ShadowExplosionDamageRange = new IntegerRange( 1, 15 );        

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StoneTombsBossBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal StoneTombsBossBehaviour( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoneTombsBossBehaviour"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The entity that is controlled by the new StoneTombsBossBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="enemy"/> is null.
        /// </exception>
        public StoneTombsBossBehaviour( Enemy enemy, IZeldaServiceProvider serviceProvider )
            : base( enemy, serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( enemy != null );

            this.boss = enemy;
            this.serviceProvider = serviceProvider;
            this.ingameState = (IIngameState)serviceProvider.GetService( typeof( IIngameState ) );

            // Create Status Effect.
            this.enrageAura = new PermanentAura(
                new StatusEffect[2] {
                    new MovementSpeedEffect( 30.0f, StatusManipType.Percental ),
                    new DamageDoneWithSourceEffect( 25.0f, StatusManipType.Percental, DamageSource.Melee )
                }
            );

            this.secondaryEnrageAura = new TimedAura(
                10.0f,
                new MovementSpeedEffect( 50.0f, StatusManipType.Percental )
            );

            // Hook Events
            this.StateChanged += OnStateChanged;
            this.boss.Statable.Died += OnBossDied;
            this.boss.Attackable.Attacked += OnBossAttacked;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this StoneTombsBossBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
            {
                return;
            }

            base.Update( updateContext );

            // Spawn Waves if the Boss is chasing the Player.
            if( !this.ChasePlayerBehaviour.IsActive || boss.Statable.IsDead )
            {
                return;
            }

            if( !this.isEnraged )
            {
                this.timeLeftNextWave -= updateContext.FrameTime;

                if( timeLeftNextWave <= 0.0f )
                {
                    this.SpawnWave();
                    this.timeLeftNextWave = WaveTime + ((WaveTime / 5.0f) * this.waveCount);
                }
            }
        }

        /// <summary>
        /// Randomly gets one of the ISpawnPoint used in this boss fight
        /// to spawn enemy waves on.
        /// </summary>
        /// <returns>
        /// A ISpawnPoint.
        /// </returns>
        private ISpawnPoint GetRandomSpawnPoint()
        {
            // There are 4 spawn points
            // for enemy waves in this boss fight.
            int value = this.serviceProvider.Rand.RandomRange( 0, 3 );

            return boss.Scene.GetSpawnPoint( "BossEnemySpawnPoint_" + value.ToString( CultureInfo.InvariantCulture ) );
        }

        /// <summary>
        /// Spawns the next wave on enemies.
        /// </summary>
        private void SpawnWave()
        {
            ISpawnPoint spawnPoint = GetRandomSpawnPoint();

            switch( waveCount )
            {
                case 0:
                    SpawnEntity( 3, 6, "Skeleton", spawnPoint );
                    break;

                case 1:
                    SpawnEntity( 4, 12, "Skeleton", spawnPoint );
                    SpawnEntity( 2, 7, "SkeletonHead", spawnPoint );
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                    SpawnEntity( 2, waveCount + 1, "Skeleton_Red", spawnPoint );
                    SpawnEntity( 0, 7, "Skeleton", spawnPoint );
                    SpawnEntity( 0, 4, "SkeletonHead", spawnPoint );
                    break;

                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    SpawnEntity( 1, 3, "Skeleton_Green", spawnPoint );
                    SpawnEntity( 1, waveCount / 2, "Skeleton_Red", spawnPoint );
                    SpawnEntity( 0, 6, "Skeleton", spawnPoint );
                    break;

                default:
                    SpawnEntity( 1, 5, "Skeleton_Green", spawnPoint );
                    SpawnEntity( 1, 1 + (waveCount / 25), "DemonGirl", spawnPoint );
                    SpawnEntity( 0, 3, "Skeleton", spawnPoint );
                    break;
            }

            ++this.waveCount;

            if( waveCount % 3 == 0 )
            {
                SpawnFireTail();
            }
        }

        private void SpawnEntity( int fromCount, int toCount, string entityName, ISpawnPoint spawnPoint )
        {
            RandMT rand = this.serviceProvider.Rand;
            EntityTemplateManager entityManager = this.serviceProvider.EntityTemplateManager;

            IEntityTemplate template = entityManager.GetTemplate( entityName );
            int count = rand.RandomRange( fromCount, toCount );

            for( int i = 0; i < count; ++i )
            {
                ZeldaEntity entity = template.CreateInstance();
                SpawnHelper.AddBlendInColorTint( entity );

                spawnPoint.Spawn( entity );
            }
        }

        private void SpawnFireTail()
        {
            var method = new Attacks.Methods.FixedShadowSpellDamageMethod() {
                DamageRange = ShadowTailDamageRange
            };

            var explosionMethod = new Attacks.Methods.FixedShadowSpellDamageMethod() {
                DamageRange = ShadowExplosionDamageRange
            };

            var spell = new Zelda.Casting.FireTail( this.boss.Scene.Player, boss, method, explosionMethod, serviceProvider );
            spell.AddToScene( this.boss.Scene );
        }

        /// <summary>
        /// Plays the background music of the boss fight.
        /// </summary>
        private void PlayBossMusic()
        {
            // Receive.
            Atom.Fmod.Sound music = this.serviceProvider.AudioSystem.GetMusic( BossMusicName );
            if( music == null )
            {
                return;
            }

            // Load.
            music.LoadAsMusic( false );

            // Play.
            Audio.BackgroundMusicComponent backgroundMusic = ingameState.BackgroundMusic;

            backgroundMusic.Mode = Zelda.Audio.BackgroundMusicMode.Loop;
            backgroundMusic.ChangeTo( music );
        }

        /// <summary>
        /// Applies the enrage effect to the boss.
        /// </summary>
        private void EnableEnrage()
        {
            if( enrageAura.AuraList != null )
            {
                return;
            }

            // Apply status StatusEffects.
            this.boss.Statable.AuraList.Add( enrageAura );

            this.secondaryEnrageAura.ResetDuration();
            this.boss.Statable.AuraList.Add( secondaryEnrageAura );

            // Tint the boss redish.
            Drawing.ITintedDrawDataAndStrategy tintedDds = boss.DrawDataAndStrategy as Zelda.Entities.Drawing.ITintedDrawDataAndStrategy;
            if( tintedDds != null )
            {
                tintedDds.BaseColor = new Microsoft.Xna.Framework.Color( 255, 100, 100, 255 );
            }

            this.isEnraged = true;
        }

        #region > Events <

        /// <summary>
        /// Gets called when the BehaviourState of this StoneTombsBossBehaviour
        /// has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contains the event data.
        /// </param>
        private void OnStateChanged( object sender, EventArgs e )
        {
            if( this.State == BehaviourState.ChasingPlayer )
            {
                this.PlayBossMusic();
            }
            else
            {
                if( ingameState == null )
                {
                    return;
                }

                Audio.BackgroundMusicComponent backgroundMusic = this.ingameState.BackgroundMusic;

                if( backgroundMusic.Mode != Audio.BackgroundMusicMode.Random )
                {
                    PlayNormalMusic();
                }
            }
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
            {
                return;
            }

            Statable statable = this.boss.Statable;
            if( statable.Life <= (statable.MaximumLife * 0.25) )
            {
                this.EnableEnrage();
            }
        }

        /// <summary>
        /// Gets called when the player managed to defeat the boss.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnBossDied( Statable sender )
        {
            if( this.boss == null )
            {
                return;
            }

            ZeldaScene scene = this.boss.Scene;
            if( scene == null )
            {
                return;
            }

            // 1. Move all red/blue blocks down.
            Atom.Events.Event blockDownEvent = scene.EventManager.GetEvent( "Event_RedBlueBlocks_AllDown" );
            blockDownEvent.Trigger( scene.Player );

            // 2. Disable all triggers.
            foreach( ZeldaEntity entity in scene.Entities )
            {
                var trigger = entity as RedBlueBlockTrigger;
                if( trigger != null )
                {
                    trigger.IsEnabled = false;
                }
            }

            // 3. Play Normal Music again.            
            PlayNormalMusic();
        }

        private void PlayNormalMusic()
        {
            Audio.BackgroundMusicComponent backgroundMusic = ingameState.BackgroundMusic;
            backgroundMusic.Mode = Zelda.Audio.BackgroundMusicMode.Random;
            backgroundMusic.ChangeToRandom();
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="StoneTombsBossBehaviour"/>
        /// for the given <see cref="Enemy"/> entity.
        /// </summary>
        /// <param name="newOwner">
        /// The entity that wants to be controlled by the newly cloned IEntityBehaviour.
        /// </param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public override IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new StoneTombsBossBehaviour( (Enemy)newOwner, this.serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whether the boss is currently enraged; %lt; 25% Hp.
        /// </summary>
        private bool isEnraged;

        /// <summary>
        /// The number of enemy waves that have been spawned.
        /// </summary>
        private int waveCount;

        /// <summary>
        /// The time left until the next enemy wave spawns.
        /// </summary>
        private float timeLeftNextWave = WaveTime;

        /// <summary>
        /// Identifies the Enemy entity that goes after the player.
        /// </summary>
        private readonly Enemy boss;

        /// <summary>
        /// Identifies the IIngameState object.
        /// </summary>
        private readonly IIngameState ingameState;

        /// <summary>
        /// The aura that gets activated when the boss reaches 25% or less HP.
        /// </summary>
        private readonly Zelda.Status.PermanentAura enrageAura;

        /// <summary>
        /// The aura that gets activated when the boss reaches 25% or less HP.
        /// </summary>
        private readonly Zelda.Status.TimedAura secondaryEnrageAura;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}