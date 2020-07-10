// <copyright file="FireBombSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.FireBombSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Ranged
{
    using Atom.Fmod;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Attacks.Ranged;
    using Zelda.Entities;
    using Zelda.Status;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// The FireBombSkill increases the movement speed 
    /// of the Player for a fixed amount of time.
    /// </summary>
    internal sealed class FireBombSkill : PlayerTalentSkill<FireBombTalent>
    {
        /// <summary>
        /// Gets a value indicating whether the Skill is currently useable by the Player.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                if( this.Cooldown.IsReady && this.HasRequiredMana )
                {
                    return !this.Player.Moveable.IsSwimming;
                }

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireBombSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FireBombSkill( FireBombTalent talent, FireBombChainTalent chainTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, FireBombTalent.Cooldown )
        {
            this.chainTalent = chainTalent;

            this.bombSprite    = serviceProvider.SpriteLoader.LoadAnimatedSprite( "BombA" );
            this.bombExplosion = serviceProvider.SpriteLoader.LoadAnimatedSprite( "BombExplosion_A" );

            this.damageMethod = new FireBombDamageMethod();
            this.damageMethod.Setup( serviceProvider );

            // Audio.
            this.bombAudio = new BombAudio( serviceProvider.AudioSystem, serviceProvider.Rand );
        }
        
        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.damageMethod.SetValues( this.Talent.RangedMagicDamageContribution, this.Talent.MagicDamageContribution );

            this.maximumBombsUp = 1 + chainTalent.ExtraBombs;
            this.Cooldown.TotalTime = FireBombTalent.Cooldown + chainTalent.ExtraCooldown;
            this.Cost = ManaCost.PercentageOfBase( FireBombTalent.ManaNeededPoBM - chainTalent.ManaCostReductionPoBM );
        }

        /// <summary>
        /// Fires this FireBombSkill.
        /// </summary>
        /// <returns>
        /// true if this Skill has been fired;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            this.CreateBomb( this.GetBombPosition() );
            
            ++activeBombs;

            if( activeBombs >= maximumBombsUp )
            {
                this.Cooldown.Reset();
                activeBombs = 0;
            }
            else
            {
                this.Cooldown.Reset( FireBombChainTalent.GlobalCooldown );
            }

            return true;
        }

        /// <summary>
        /// Gets the position the next bomb should be spawned at.
        /// </summary>
        /// <returns>
        /// The position.
        /// </returns>
        private Atom.Math.Vector2 GetBombPosition()
        {
            var collision = this.Player.Collision;
            var center    = collision.Center;

            switch( this.Player.Transform.Direction )
            {
                case Direction4.Left:
                    return new Vector2( center.X - collision.Width - 2.0f, center.Y - 6.0f );

                case Direction4.Right:
                    return new Vector2( center.X + (collision.Width / 2.0f), center.Y - 6.0f );

                case Direction4.Up:
                    return new Vector2( center.X - 7.0f, center.Y - collision.Height );

                default:
                case Direction4.Down:
                    return new Vector2( center.X - 7.0f, center.Y + (collision.Height / 2.0f) );
            }
        }

        /// <summary>
        /// Creates and spawns a new Bomb at the given explosion.
        /// </summary>
        /// <param name="position">
        /// The position at which the Bomb should be spawned at.
        /// </param>
        private void CreateBomb( Atom.Math.Vector2 position )
        {
            Bomb bomb = Bomb.Spawn( 
                this.Statable,
                position,
                this.Player.FloorNumber,
                this.damageMethod, 
                null,
                FireBombTalent.ExplosionRadius,
                FireBombTalent.ExplosionPushingPower,
                this.bombSprite.CreateInstance(),
                this.bombExplosion.CreateInstance(),
                new Vector2( -2.0f, -2.0f ),
                this.bombAudio.GetRandomSound()
            );

            bomb.AddToScene( this.Player.Scene );
        }
        
        /// <summary>
        /// The maximum number of bombs that can be placed before the cooldown is triggered.
        /// </summary>
        private int maximumBombsUp;

        /// <summary>
        /// The number of bombs that have been placed before reaching maximumBombsUp.
        /// </summary>
        private int activeBombs;

        private FireBombChainTalent chainTalent;

        /// <summary>
        /// The damage method used to calculate the damage of each bomb.
        /// </summary>
        private readonly FireBombDamageMethod damageMethod;

        /// <summary>
        /// The sprites used to visualize the bombs.
        /// </summary>
        private readonly AnimatedSprite bombSprite, bombExplosion;

        /// <summary>
        /// The sound to play when a bomb explodes.
        /// </summary>
        private readonly BombAudio bombAudio;
    }
}