
namespace Zelda.Scripting
{
    using System;
    using System.Collections.Generic;
    using Zelda.Entities;
    using Zelda.Entities.Spawning;
    using Zelda.Profiles;
    using Atom.Math;

    public sealed class DSLHelper
    {
        public GameProfile Profile { get; set; }
        public ZeldaScene Scene { get; set; }
        public Zelda.Audio.ZeldaAudioSystem AudioSystem { get { return serviceProvider.AudioSystem; } }

        public DSLHelper( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        // deco = spawn_deco 'PoisonCloud_A', 'BFS_3',
        //     type: 'anim_1D',
        //     anim_speed: 500, 
        //     on_anim_ended: 'despawn'        
        public Decoration SpawnDeco( string spriteName, string spawnPointName, string type )
        {
            ISpawnPoint sp = this.Scene.GetSpawnPoint( spawnPointName );

            if( sp == null )
            {
                throw new ArgumentException( "Could not find SpawnPoint named " + spawnPointName );
            }

            var deco = new Decoration();

            var dds = new Zelda.Entities.Drawing.TintedOneDirAnimDrawDataAndStrategy( deco );
            dds.SpriteGroup = spriteName;
            dds.Load( this.serviceProvider );

            deco.DrawDataAndStrategy = dds;

            SpawnHelper.AddBlendInColorTint( deco );

            sp.Spawn( deco );
            return deco;
        }

        public ZeldaEntity DespawnOnAnimationEnd( ZeldaEntity entity )
        {
            var adds = entity.DrawDataAndStrategy as Zelda.Entities.Drawing.IAnimatedDrawDataAndStrategy;

            adds.CurrentAnimation.ReachedEnd += sender => {
                SpawnHelper.Despawn( entity );
            };

            return entity;
        }

        public Microsoft.Xna.Framework.Color GetRandomColorRGB()
        {
            var rand = serviceProvider.Rand;
            return new Microsoft.Xna.Framework.Color( rand.RandomSingle, rand.RandomSingle, rand.RandomSingle );
        }

        public void OnEvent( string id, Func<object> action )
        {
            EventHandler handler = null;

            switch( id )
            {
                case "level_up":
                    handler = ( s, e ) => {
                        var result = action();

                        if( IsNullOrTrue( result ) )
                        {
                            this.Profile.Player.Statable.LevelUped -= handler;
                        }
                    };

                    this.Profile.Player.Statable.LevelUped += handler;
                    break;

                default:
                    throw new ArgumentException( "event with id " + id + " unknown" );
            }
        }

        public void AfterSeconds( float time, Func<object> action )
        {
            Zelda.Timing.Timer timer = new Timing.Timer() {
                Time = time
            };

            ZeldaScene scene = this.Scene;

            timer.Ended += sender => {
                action();
                scene.Remove( sender );
            };

            scene.Add( timer );
        }

        public void PlaySample( string soundName, float volume )
        {
            Atom.Fmod.Sound sample = this.AudioSystem.GetSample( soundName );

            sample.LoadAsSample();
            sample.Play( volume );
        }

        public void PlayMusic( string musicName )
        {
           this.Scene.IngameState.BackgroundMusic
               .ChangeTo( musicName );
        }

        public void Despawn( ZeldaEntity entity )
        {
            if( entity != null )
            {
                SpawnHelper.Despawn( entity );
            }
        }

        public void SpawnAt( string spawnPointName, ZeldaEntity entity )
        {
            ISpawnPoint spawnPoint = Scene.GetSpawnPoint( spawnPointName );
            if( spawnPoint == null || entity == null )
                return;

            spawnPoint.Spawn( entity );
        }

        public void SpawnEnemy( string templateName, ISpawnPoint spawnPoint, int count )
        {
            SpawnHelper.Spawn( templateName, count, spawnPoint, this.serviceProvider.EntityTemplateManager );
        }

        public void SetActionTile( int floorNumber, int tileX, int tileY, int targetActionTile )
        {
            var floor = this.Scene.Map.GetFloor( floorNumber );
            if( floor == null )
                return;

            floor.ActionLayer.TrySetTile( tileX, tileY, targetActionTile );
        }

        public void SetAmbientColor( byte red, byte green, byte blue, byte alpha )
        {
            this.Scene.Settings.AmbientColor = new Microsoft.Xna.Framework.Color( red, green, blue, alpha );
        }

        public byte GetRandomByte()
        {
            return (byte)serviceProvider.Rand.UncheckedRandomRange( 0, 255 );
        }

        private static bool IsNullOrTrue( object obj )
        {
            return obj == null || (bool)obj;
        }

        private readonly IZeldaServiceProvider serviceProvider;
    }
}
