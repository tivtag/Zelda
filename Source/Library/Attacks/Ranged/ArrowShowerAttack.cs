// <copyright file="ArrowShowerAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.ArrowShowerAttack class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Ranged
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom.Math;
    using Zelda.Entities.Drawing;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// The ArrowShowerAttack is an instant ranged attack
    /// that releases X arrows at the same time.
    /// <para>
    /// The arrows travel into the same direction
    /// and slowly start to spread out.
    /// </para>
    /// </summary>
    internal sealed class ArrowShowerAttack : RangedPlayerAttack
    {
        #region [ Constants ]

        /// <summary>
        /// The speed correction that is applied to each arrow.
        /// This is needed to let the arrows 'spread' out.
        /// </summary>
        private const int SpeedCorrection = 5;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Sets the number of projectiles this ArrowShowerAttack releases at the same time. 
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is less than zero.
        /// </exception>
        public int ProjectileCount
        {  
            // Unused.
            // get { return projectileCount; }
            set
            {
                Contract.Requires<ArgumentException>( value >= 0 );

                this.projectileCount = value;
            }
        }

        /// <summary>
        /// Sets the number of attacks this ArrowShowerAttack this ArrowShowerAttack unleashes. 
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is less than zero.
        /// </exception>
        public int AttackCount
        {
            // Unused.
            // get { return attackCount; }
            set
            {
                Contract.Requires<ArgumentException>( value >= 0 );

                this.attackCount = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowShowerAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity which owns the new ArrowShowerAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod to use for calculating the damage
        /// that is done by the new ArrowShowerAttack. 
        /// </param>
        /// <param name="cooldown">
        /// The cooldown which defines the delay between two RangedAttacks.
        /// </param>
        public ArrowShowerAttack( Zelda.Entities.PlayerEntity player, MultiShotDamageMethod method, Cooldown cooldown )
            : base( player, method, cooldown )
        {
        }

        /// <summary>
        /// Setups this ArrowShowerAttack.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            var spriteLoader = serviceProvider.SpriteLoader;

            this.Settings.SetSprites(
                spriteLoader.LoadSprite( "Arrow_Wood_Up" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Down" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Left" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Right" )
            );

            base.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary> 
        /// Fires this <see cref="ArrowShowerAttack"/>. 
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// Returns true if the ArrowShower was launched;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.StartArrowShower();
                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts the arrow shower attack.
        /// </summary>
        private void StartArrowShower()
        {
            this.FireMultiShot();

            this.isAttacking = true;
            this.multiShotsFired = 0;
            this.timeToNextMultiShot = ArrowShowerTalent.TimeBetweenMultiShots;
            this.Player.Statable.Died += this.OnPlayerDied;
        }

        /// <summary>
        /// Stops the arrow shower attack.
        /// </summary>
        private void StopArrowShower()
        {
            this.isAttacking = false;
            this.Player.Statable.Died -= this.OnPlayerDied;
        }

        /// <summary>
        /// Fires one Multi Shot.
        /// </summary>
        private void FireMultiShot()
        {
            var playerDrawDas = this.Player.DrawDataAndStrategy;

            Direction4 direction = this.Transform.Direction;
            Vector2    center    = this.Player.Collision.Center;

            for( int i = 0; i < this.projectileCount; ++i )
            {
                int currentSpeedCorrection = SpeedCorrection * i * ( (i % 2 == 0) ? -1 : 1);

                switch( direction )
                {
                    case Direction4.Up:
                        SpawnProjectile(
                            new Vector2( center.X - 7, this.Transform.Y + 2 ),
                            new Vector2( currentSpeedCorrection, -GetProjectileMovementSpeed() )
                        );
                        break;

                    case Direction4.Down:
                        SpawnProjectile(
                            new Vector2( center.X + 1, center.Y ),
                            new Vector2( currentSpeedCorrection, GetProjectileMovementSpeed() )
                        );
                        break;

                    case Direction4.Left:
                        SpawnProjectile(
                            new Vector2( this.Transform.X, center.Y - 6 ),
                            new Vector2( -GetProjectileMovementSpeed(), currentSpeedCorrection )
                        );
                        break;

                    case Direction4.Right:
                        SpawnProjectile(
                            new Vector2( center.X, center.Y - 6 ),
                            new Vector2( GetProjectileMovementSpeed(), currentSpeedCorrection )
                        );
                        break;

                    default:
                        break;
                }
            }

            playerDrawDas.SpecialAnimation = PlayerSpecialAnimation.AttackRanged;
            playerDrawDas.ResetSpecialAnimation( PlayerSpecialAnimation.AttackRanged, direction );
        }

        /// <summary>
        /// Updates this ArrowShowerAttack.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.isAttacking )
            {
                this.timeToNextMultiShot -= updateContext.FrameTime;

                if( this.timeToNextMultiShot <= 0.0f )
                {
                    this.FireMultiShot();

                    ++this.multiShotsFired;
                    this.timeToNextMultiShot = ArrowShowerTalent.TimeBetweenMultiShots;

                    if( this.multiShotsFired >= this.attackCount )
                    {
                        this.StopArrowShower();
                    }
                }
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Called when the player that owns this ArrowShowerAttack has died 
        /// turing firing the Arrow Shower.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnPlayerDied( Zelda.Status.Statable sender )
        {
            this.StopArrowShower();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of multi shots the arrow shower has fired.
        /// </summary>
        private int multiShotsFired;

        /// <summary>
        /// The time left in seconds until the next multi shot is fired.
        /// </summary>
        private float timeToNextMultiShot;

        /// <summary>
        /// States whether arrow shower is currently firing.
        /// </summary>
        private bool isAttacking;

        /// <summary>
        /// The number of projectiles released per multi shot.
        /// </summary>
        private int projectileCount = 5;
        
        /// <summary>
        /// The number of multi shots the Arrow Shower unleashese.
        /// </summary>
        private int attackCount = 3;        

        #endregion
    }
}
