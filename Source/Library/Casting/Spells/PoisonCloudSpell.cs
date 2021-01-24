// <copyright file="PoisonCloudSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.PoisonCloudSpell class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting.Spells
{
    using Zelda.Attacks;
    using Zelda.Attacks.HitEffects;
    using Zelda.Attacks.Limiter;
    using Zelda.Entities;
    using Zelda.Entities.Behaviours;
    using Zelda.Status;
    using Zelda.Status.Auras;

    /// <summary>
    /// The PoisonCloudSpell creates cloud of poison that apply a damage over time and slowing effect
    /// to their enemy in an aoe radius.
    /// </summary>
    public sealed class PoisonCloudSpell : Spell
    {        
        /// <summary>
        /// The name of the sprite animation used by the Poison Clouds.
        /// </summary>
        private const string PoisonCloudAnimation = "PoisonCloud_A";

        /// <summary>
        /// Initializes a new instance of the PoisonCloudSpell class.
        /// </summary>
        /// <param name="damageMethod">
        /// The AttackDamageMethod the poison clouds use to calculate their damage output.
        /// </param>
        /// <param name="statable">
        /// The ZeldaEntity that owns the new PoisonCloudSpell.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PoisonCloudSpell( AttackDamageMethod damageMethod, Statable statable, IZeldaServiceProvider serviceProvider )
            : base( statable.Owner, 0.0f, null )
        {
            this.statable = statable;
            this.damageMethod = damageMethod;

            const float DefaultSlowingEffect = -20.0f;

            this.slowingEffect = new MovementSpeedEffect( DefaultSlowingEffect, StatusManipType.Percental );
            this.attackSpeedSlowingEffect = new AttackSpeedEffect( AttackType.All, -DefaultSlowingEffect, StatusManipType.Percental );

            this.dotAura = CreateAura( this.slowingEffect, this.attackSpeedSlowingEffect, this.Owner, serviceProvider );
            this.hitEffect = new DamageOverTimeAttackHitEffect( this.dotAura, this.damageMethod );

            var wrappedHitEffect = new UniqueTimedAuraAttackHitEffectWrapper( this.dotAura, this.hitEffect );
            this.cloudTemplate = CreateCloudTemplate( wrappedHitEffect, serviceProvider );
        }

        /// <summary>
        /// Creates the template object from which further Poison Clouds are created / initialized.
        /// </summary>
        /// <param name="hitEffect">
        /// The effect that is executed when a PoisonCloud created by this PoisonCloudSpell
        /// has hit an enemy target.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The Poison Cloud template.
        /// </returns>
        private static DamageEffectEntity CreateCloudTemplate( IAttackHitEffect hitEffect, IZeldaServiceProvider serviceProvider )
        {
            var cloudTemplate = new DamageEffectEntity() {
                 FloorRelativity = EntityFloorRelativity.IsAbove
            };

            cloudTemplate.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );

            // Drawing.
            cloudTemplate.DrawDataAndStrategy = new Zelda.Entities.Drawing.TintedOneDirAnimDrawDataAndStrategy( cloudTemplate ) {
                SpriteGroup = PoisonCloudAnimation
            };

            cloudTemplate.DrawDataAndStrategy.Load( serviceProvider );
            
            // Behaviour:
            var multiBehaviour = new MultiBehaviour();
            multiBehaviour.Add( new ResizeToCurrentSpriteBehaviour( cloudTemplate ) );
            multiBehaviour.Add( new DespawnAfterAnimationEndedBehaviour( cloudTemplate ) { FadeOutTime = 0.25f } );

            cloudTemplate.Behaveable.Behaviour = multiBehaviour;

            // Attack
            var limiter = new InitiallyLimitThenRedirectToOtherAttackLimiter( 1.5f, new TimedAttackLimiter( 2.5f ) );

            cloudTemplate.MeleeAttack.Limiter = limiter;
            cloudTemplate.MeleeAttack.HitEffect = hitEffect; 
            
            return cloudTemplate;
        }

        /// <summary>
        /// Creates the DamageOverTimeAura that gets applied to targets that got hit
        /// by a Poison Cloud.
        /// </summary>
        /// <param name="slowingEffect">
        /// The slowing effect that gets applied.
        /// </param>
        /// <param name="attackSpeedSlowingEffect">
        /// The attackspeed slowing effect that gets applied.
        /// </param>
        /// <param name="owner">
        /// The owner of the PoisonCloudSpell.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The DamageOverTimeAura to apply when a Poison Cloud hits a target.
        /// </returns>
        private static DamageOverTimeAura CreateAura( 
            MovementSpeedEffect slowingEffect, 
            AttackSpeedEffect attackSpeedSlowingEffect,
            Zelda.Entities.ZeldaEntity owner,
            IZeldaServiceProvider serviceProvider )
        {
            const float DefaultEffectDuration = 8.0f;
            const float DefaultTimeBetweenTicks = 2.0f;

            return new DamageOverTimeAura( 
                DefaultEffectDuration,
                DefaultTimeBetweenTicks,
                new StatusEffect[2] { slowingEffect, attackSpeedSlowingEffect },
                owner as IAttackableEntity ) {
                Name             = "PoisonCloud",
                PowerType        = LifeMana.Life,
                ManipulationType = StatusManipType.Fixed,
                DebuffFlags      = DebuffFlags.Poisoned | DebuffFlags.Slow,
                IsVisible        = true,
                Symbol = LoadSymbol( serviceProvider ),
                SymbolColor = new Microsoft.Xna.Framework.Color( 255, 125, 125, 255 )
            };
        }

        /// <summary>
        /// Loads the sprite used to visualize the Poison Cloud aura.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <returns>
        /// The loaded symbol sprite.
        /// </returns>
        private static Atom.Xna.Sprite LoadSymbol( IZeldaServiceProvider serviceProvider )
        {
            return serviceProvider.SpriteLoader.LoadSprite( "Symbol_PoisonCloud" );
        }

        /// <summary>
        /// Fires this PoisonCloudSpell.
        /// </summary>
        /// <param name="target">
        /// The target of the spell.
        /// </param>
        /// <returns>
        /// true if it has fired;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            if( this.ShouldFire() )
            {
                var cloud = this.CreatePoisonCloud( this.Owner.Collision.Center, this.Owner.FloorNumber );
                cloud.AddToScene( this.Owner.Scene );

                this.OnFired();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new Poison Cloud at the specified position.
        /// </summary>
        /// <param name="position">
        /// The position at which the Poison Cloud should be spawned.
        /// </param>
        /// <param name="floorNumber">
        /// The floor number at which the Poison Cloud should be spawned.
        /// </param>
        /// <returns>
        /// The newly created DamageEffectEntity.
        /// </returns>
        public DamageEffectEntity CreatePoisonCloud( Atom.Math.Vector2 position, int floorNumber )
        {
            var cloud = (DamageEffectEntity)cloudTemplate.Clone();

            // Setup.
            cloud.FloorNumber = floorNumber;
            cloud.Transform.Position = position;
            cloud.Creator = this.statable;

            return cloud;
        }

        /// <summary>
        /// Identifies the Statable component that controls the strength of the Poison Clouds.
        /// </summary>
        private readonly Statable statable;

        /// <summary>
        /// The template entity that is used to create new Poison Clouds.
        /// </summary>
        private readonly DamageEffectEntity cloudTemplate;
        
        /// <summary>
        /// The damage method the PoisonClouds created by this PoisonCloudSpell use
        /// to calculate how much damage they have done.
        /// </summary>
        private readonly AttackDamageMethod damageMethod;

        /// <summary>
        /// The effect that is executed when a PoisonCloud created by this PoisonCloudSpell
        /// has hit an enemy target.
        /// </summary>
        private readonly IAttackHitEffect hitEffect;

        /// <summary>
        /// The slowing effect applied to enemy entities that run thru the poison cloud.
        /// </summary>
        private readonly MovementSpeedEffect slowingEffect;

        /// <summary>
        /// The slowing effect applied to enemy entities that run thru the poison cloud.
        /// </summary>
        private readonly AttackSpeedEffect attackSpeedSlowingEffect;

        /// <summary>
        /// The damage over time aura that is applied to enemy entities that run thru the poison cloud.
        /// </summary>
        private readonly DamageOverTimeAura dotAura;
    }
}
