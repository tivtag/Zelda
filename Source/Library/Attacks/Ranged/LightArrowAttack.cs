// <copyright file="LightArrowAttack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.LightArrowAttack class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Ranged
{
    using Atom.Collections.Pooling;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Projectiles;

    /// <summary>
    /// The LightArrow attack is a modified PlayerRangedAttack that
    /// also applies a Light effect to the Projectile it fires.
    /// This is a sealed class.
    /// </summary>
    internal sealed class LightArrowAttack : RangedPlayerAttack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightArrowAttack"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new LightArrowAttack.
        /// </param>
        /// <param name="attackDamageMethod">
        /// The <see cref="LightArrowDamageMethod"/> used by the new LightArrowAttack.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal LightArrowAttack( PlayerEntity player, LightArrowDamageMethod attackDamageMethod, IZeldaServiceProvider serviceProvider )
            : base( player, attackDamageMethod )
        {            
            this.lightSprite = serviceProvider.SpriteLoader.LoadSprite( "Light_Circle2_80px" );
            this.lightPool = Pool<Light>.Create( 2, this.CreateArrowLight );
        }

        /// <summary>
        /// Setups this LightArrowAttack.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            var spriteLoader = serviceProvider.SpriteLoader;

            this.Settings.Speed = new IntegerRange( 97, 115 );
            this.Settings.SetSprites(
                spriteLoader.LoadSprite( "Arrow_Steel_Up" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Down" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Left" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Right" )
            );

            base.Setup( serviceProvider );
        }

        /// <summary>
        /// Creates a new Light, used by the Light Arrow.
        /// </summary>
        /// <returns>
        /// The newly created Light.
        /// </returns>
        private Light CreateArrowLight()
        {
            Light light = new Light() {
                Name   = "LArrowL",
                Sprite = lightSprite,
                Color  = new Microsoft.Xna.Framework.Color( 255, 255, 255, 100 )
            };

            light.Transform.InheritsScale = false;
            
            var scaleModifier = new Zelda.Entities.Modifiers.ScaleCurveEntityModifier();
            scaleModifier.AddScaleKey( 0.0f, 1.0f );
            scaleModifier.AddScaleKey( 1.0f, 1.2f );
            scaleModifier.AddScaleKey( 2.0f, 1.0f );
            scaleModifier.AddScaleKey( 3.0f, 0.9f );
            scaleModifier.AddScaleKey( 4.0f, 1.0f );
            light.Components.Add( scaleModifier );

            return light;
        }

        private static Vector2 GetRelativeLightPosition( Direction4 direction )
        {
            switch( direction )
            {
                case Direction4.Left:
                    return new Vector2( 5.0f, 2.0f );

                case Direction4.Right:
                    return new Vector2( 6.0f, 2.0f );

                case Direction4.Up:
                    return new Vector2( 2.0f, 6.0f );
                    
                default:
                case Direction4.Down:
                    return new Vector2( 2.0f, 5.0f );
            }
        }

        /// <summary>
        /// Creates a new projectile object with the given settings
        /// and spawns it in the scene.
        /// </summary>
        /// <param name="creator">
        /// The object that fires the projectile.
        /// </param>
        /// <param name="position">
        /// The starting position of the projectile.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor the projectile should spawn at.
        /// </param>
        /// <param name="direction">
        /// The direction the projectile is travelling in.
        /// </param>
        /// <param name="speed">
        /// The traveling speed of the projectile to spawn.
        /// </param>
        /// <returns>
        /// The ProjectileObject; taken from a pool of inactive projectiles.
        /// </returns>
        protected override Projectile SpawnProjectile(
            Zelda.Status.Statable creator,
            Vector2 position,
            int floorNumber,
            Direction4 direction,
            Vector2 speed )
        {
            // Create a regular projectile:
            Projectile projectile = base.SpawnProjectile( 
                creator,
                position,
                floorNumber, 
                direction,
                speed
            );

            // Setup projectile 
            projectile.Destroyed += this.OnLightArrowDestroyed;

            // Receive Light from the pool
            PoolNode<Light> lightNode = lightPool.Get();

            // Setup and connect the Light
            if( lightNode != null )
            {
                Light light = lightNode.Item;
                light.Transform.RelativePosition = GetRelativeLightPosition( direction );

                projectile.OptionalData = lightNode;
                projectile.Transform.AddChild( light.Transform );

                light.AddToScene( creator.Scene );
            }

            return projectile;
        }

        /// <summary>
        /// Gets called when a LightArrow Projectile has been destroyed.
        /// </summary>
        /// <param name="projectile">
        /// The sender of the event.
        /// </param>
        private void OnLightArrowDestroyed( Projectile projectile )
        {
            // Remove items that have been attached
            // to the Projectile so that it can be reused.
            projectile.Destroyed -= this.OnLightArrowDestroyed;

            var lightNode = projectile.OptionalData as PoolNode<Light>;

            // Also remove the Light attached to the
            // LightArrow.
            if( lightNode != null )
            {
                Light light = lightNode.Item;

                light.Transform.Parent = null;
                light.RemoveFromScene();

                lightPool.Return( lightNode );
            }
        }
        
        /// <summary>
        /// The pool of Light resources. There can be only 5 Light Arrows around at the same time.
        /// </summary>
        private readonly Pool<Light> lightPool;

        /// <summary>
        /// The sprite that is used to visualize the light.
        /// </summary>
        private readonly Atom.Xna.Sprite lightSprite;
    }
}
