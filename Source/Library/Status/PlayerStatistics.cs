
namespace Zelda.Status
{
    using System;
using Zelda.Entities;
using Zelda.Entities.Components;
using Zelda.Saving;

    /// <summary>
    /// Encapsulates the play statistics of a player.
    /// </summary>
    public sealed class PlayerStatistics : ZeldaComponent, ISaveable
    {
        /// <summary>
        /// Gets the number of times that the player has died.
        /// </summary>
        public int DeathCount
        {
            get
            {
                return this.deathCount;
            }
        }

        /// <summary>
        /// Gets the number of targets that the player has killed.
        /// </summary>
        public uint KillCount
        {
            get
            {
                return this.killCount;
            }
        }

        public TimeSpan GameTime
        {
            get
            {
                return gameTime;
            }
        }

        public PlayerStatistics( PlayerEntity player )
        {
            player.Statable.Died += OnPlayerDied;
            player.Attackable.AttackHit += this.OnAttackHit;
        }

        private void OnAttackHit( object sender, AttackEventArgs e )
        {
            if( e.TargetStatable != null && e.TargetStatable.IsDead )
            {
                ++this.killCount;
            }
        }

        private void OnPlayerDied( Statable sender )
        {
            ++this.deathCount;
        }

        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteHeader( 3 );
            context.Write( this.DeathCount );
            context.WriteUnsigned( this.KillCount );
            context.Write( gameTime.Ticks );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            int version = context.ReadHeader( 3, typeof( PlayerStatistics ) );

            this.deathCount = context.ReadInt32();

            if( version >= 2 )
            {
                this.killCount = context.ReadUInt32();
            }

            if( version >= 3 )
            {
                this.gameTime = new TimeSpan( context.ReadInt64() );
            }
        }

        public override void Update( Atom.IUpdateContext updateContext )
        {
            gameTime = gameTime.Add( TimeSpan.FromSeconds( updateContext.FrameTime ) );
        }

        private int deathCount;
        private uint killCount;
        private TimeSpan gameTime;
    }
}
