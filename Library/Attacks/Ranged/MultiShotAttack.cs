// <copyright file="MultiShotAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.MultiShotAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Ranged
{
    using System;
    using System.Diagnostics.Contracts;
    using Atom.Math;
    using Zelda.Entities.Drawing;

    /// <summary>
    /// The MultiShotAttack is an instant ranged attack
    /// that releases X arrows at the same time.
    /// <para>
    /// The arrows travel into the same direction
    /// and slowly start to spread out.
    /// </para>
    /// </summary>
    internal sealed class MultiShotAttack : RangedPlayerAttack
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
        /// Sets the number of projectiles this MultiShotAttack releases at the same time. 
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is less than zero.
        /// </exception>
        public int ProjectileCount
        {
            // Unused.
            // get { return this.projectileCount; }
            set
            {
                Contract.Requires<ArgumentException>( value >= 0 );

                this.projectileCount = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiShotAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity which owns the new MultiShotAttack.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod to use for calculating the damage
        /// that is done by the new MultiShotAttack. 
        /// </param>
        /// <param name="cooldown">
        /// The Cooldown of the new MultiShotAttack.
        /// </param>
        public MultiShotAttack( Zelda.Entities.PlayerEntity player, MultiShotDamageMethod method, Cooldown cooldown )
            : base( player, method, cooldown )
        {
        }

        /// <summary>
        /// Setups this MultiShotAttack.
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
        /// Fires this <see cref="MultiShotAttack"/>. 
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// Returns true if the Multi Shot has been launched;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                this.FireMultiShot();
                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fires a Multi Shot into the direction the player is currently facing.
        /// </summary>
        public void FireMultiShot()
        {
            Direction4 direction = this.Transform.Direction;
            Vector2 center = this.Player.Collision.Center;

            for( int i = 0; i < this.projectileCount; ++i )
            {
                int currentSpeedCorrection = SpeedCorrection * i * ((i % 2 == 0) ? -1 : 1);

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

            this.DrawStrategy.ShowSpecialAnimation( PlayerSpecialAnimation.AttackRanged );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of projectiles released at a time.
        /// </summary>
        private int projectileCount = 5;

        #endregion
    }
}