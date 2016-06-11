// <copyright file="FirevortexSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Magic.FirevortexSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Magic
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Attacks.Ranged;
    using Zelda.Casting.Spells;
    using Zelda.Entities;
    using Zelda.Entities.Projectiles;
    using Zelda.Status;
    using Zelda.Status.Damage;
    using Zelda.Talents.Magic;
    
    /// <summary>
    /// Casts a vortex of fire that gains 5/7/10% in size and 
    /// strength every 0.5 seconds.
    /// Sucks nearby enemies into it.
    /// </summary>
    internal sealed class FirevortexSkill : PlayerSpellSkill<FirevortexTalent, ProjectilePlayerSpell>, ICooldownDependant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirevortexSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new FirevortexSkill.
        /// </param>
        /// <param name="razorWindsTalent">
        /// The RazorWindsTalent that increases the piercing chance of the new FirevortexSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirevortexSkill(
            FirevortexTalent talent,
            RazorWindsTalent razorWindsTalent,
            IZeldaServiceProvider serviceProvider )
            : base( talent, FirevortexTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBaseAndTotal( FirevortexTalent.ManaCostOfBaseMana, FirevortexTalent.ManaCostOfTotalMana );
            this.razorWindsTalent = razorWindsTalent;

            this.method = new FirevortexDamageMethod();
            this.method.Setup( serviceProvider );

            this.Spell = new ProjectilePlayerSpell( this.Player, FirevortexTalent.CastTime, this.method ) {
                HitSettings = new ProjectileHitSettings(),
                Limiter = new Zelda.Attacks.Limiter.TimedAttackLimiter( this.Cooldown )
            };
            
            this.SetupSpell( serviceProvider );
        }

        /// <summary>
        /// Setups the Firevortex spell.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void SetupSpell( IZeldaServiceProvider serviceProvider )
        {
            var settings = this.Spell.Settings;
            settings.StatefulUpdateLogicTemplate = new FirevortexUpdateLogic( this.Talent );

            settings.Speed = new Atom.Math.IntegerRange( 55, 65 );
            settings.TravellingSound = new Zelda.Audio.SoundSampleSettings() {
                SampleName = "FireRing.wav",
                Distance = new Atom.Math.FloatRange( 16.0f * 2, 16.0f * 10 ),
                IsLooping = true,
                Volumne = 0.9f
            };

            settings.SetSprites(
                serviceProvider.SpriteLoader.LoadAnimatedSprite( "Firewhirl" )
            );

            this.Spell.HitSettings.SoundSample.Volumne = 0.8f;
            this.Spell.HitSettings.SoundSample.Distance = new FloatRange( 16.0f * 1.4f, 16.0f * 9 );
            this.Spell.HitSettings.SoundSample.SampleName = "FireHit_Wizzrobe.wav";

            this.Spell.Setup( serviceProvider );
        }

        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.RefreshPiercingMode();
        }

        /// <summary>
        /// Refreshes the cooldown of this FirevortexSkill.
        /// </summary>
        public void RefreshCooldown()
        {
            float fixedValue, multiplierValue;
            this.AuraList.GetEffectValues(
                SkillCooldownEffect.GetIdentifier<FirevortexSkill>(),
                out fixedValue,
                out multiplierValue
            );

            this.Cooldown.TotalTime = (FirevortexTalent.Cooldown + fixedValue) * multiplierValue;
        }

        /// <summary>
        /// Refreshes the piercing settings of the Firevortex spell.
        /// </summary>
        private void RefreshPiercingMode()
        {
            var settings = this.Spell.Settings;

            if( this.razorWindsTalent.Level >= 1 )
            {
                settings.PiercingMode = ProjectilePiercingChanceMode.OnlyAdditional;
                settings.AdditionalPiercingChance = this.razorWindsTalent.FirevortexPiercingChance;
            }
            else
            {
                settings.PiercingMode = ProjectilePiercingChanceMode.None;
            }
        }

        /// <summary>
        /// Identifies the RazorWindsTalent that modifies the power of this FirevortexSkill.
        /// </summary>
        private readonly RazorWindsTalent razorWindsTalent;
        
        /// <summary>
        /// The FirevortexDamageMethod that calculates how much damage an arrow launched by the FirevortexSkill does.
        /// </summary>
        private readonly FirevortexDamageMethod method;
        
        /// <summary>
        /// Contains the additional update logic and data of a Firevortex projectile.
        /// </summary>
        private sealed class FirevortexUpdateLogic : IStatefulProjectileUpdateLogic
        {
            /// <summary>
            /// Gets the damage modifier value of the Firevortex.
            /// </summary>
            public float DamageModifier
            {
                get
                {
                    return this.damageModifier;
                }
            }

            /// <summary>
            /// Initializes a new instance of the FirevortexUpdateLogic class.
            /// </summary>
            /// <param name="firevortexTalent">
            /// Identifies the FirevortexTalent that controls the strength of the vortex.
            /// </param>
            public FirevortexUpdateLogic( FirevortexTalent firevortexTalent )
            {
                this.firevortexTalent = firevortexTalent;
            }

            /// <summary>
            /// Updates the given Firevortex.
            /// </summary>
            /// <param name="entity">
            /// The entity to update.
            /// </param>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            public void Update( Projectile entity, ZeldaUpdateContext updateContext )
            {
                this.timeLeft -= updateContext.FrameTime;

                if( this.timeLeft <= 0.0f )
                {
                    this.PowerUpVortex( entity );
                    this.timeLeft = FirevortexTalent.TimeBetweenVortexPowerups;
                }

                SuckNearbyEnemiesIntoVortex( entity, updateContext );
            }

            /// <summary>
            /// Powers-up the given Firevortex.
            /// </summary>
            /// <param name="vortex">
            /// The vortex to modify.
            /// </param>
            private void PowerUpVortex( Projectile vortex )
            {
                this.IncreaseSize( vortex );
                this.IncreasePower();
            }

            /// <summary>
            /// Increases the size of the given Firevortex.
            /// </summary>
            /// <param name="vortex">
            /// The vortex to modify.
            /// </param>
            private void IncreaseSize( Projectile vortex )
            {
                vortex.Transform.Scale *= this.firevortexTalent.ScalingFactorOnPowerUp;
            }

            /// <summary>
            /// Increases the power of the Firevortex.
            /// </summary>
            private void IncreasePower()
            {
                this.damageModifier += this.firevortexTalent.DamageIncreaseOnPowerUp;
            }

            /// <summary>
            /// Sucks nearby enemies into the given FireVortex.
            /// </summary>
            /// <param name="vortex">
            /// The vortex that is supposed to suck enemies into it.
            /// </param>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            private static void SuckNearbyEnemiesIntoVortex( Projectile vortex, ZeldaUpdateContext updateContext )
            {
                var scene = vortex.Scene;
                if( scene == null )
                    return;

                float vortexRadius = vortex.Collision.Circle.Radius;
                float influenceRadius = vortexRadius * 4.0f;
                float powerFactor = updateContext.FrameTime * 16.5f;

                foreach( var entity in scene.VisibleEntities )
                {
                    var enemy = entity as Enemy;

                    if( enemy != null )
                    {
                        var statable = enemy.Statable;
                        if( statable.IsFriendly )
                            continue;

                        var moveable = enemy.Moveable;
                        if( !moveable.CanBePushed )
                            continue;

                        Vector2 fromVortexToEnemy = vortex.Collision.Center - enemy.Collision.Center;
                        float range = fromVortexToEnemy.Length;

                        if( range <= vortexRadius || range >= influenceRadius )
                            continue;

                        float factor = (range / influenceRadius) * powerFactor;
                        Vector2 force = fromVortexToEnemy * factor;

                        moveable.Push( force );
                    }
                }
            }

            /// <summary>
            /// Returns an instance of this FirevortexUpdateLogic.
            /// </summary>
            /// <returns>A new FirevortexUpdateLogic instance.</returns>
            public object Clone()
            {
                return new FirevortexUpdateLogic( this.firevortexTalent );
            }

            /// <summary>
            /// The damage modifier of the FireVortex;
            /// </summary>
            private float damageModifier = 1.0f;

            /// <summary>
            /// The time left (in seconds) before a Firevortex increases in strength.
            /// </summary>
            private float timeLeft = FirevortexTalent.TimeBetweenVortexPowerups;

            /// <summary>
            /// Identifies the FirevortexTalent that controls the strength of the vortex.
            /// </summary>
            private readonly FirevortexTalent firevortexTalent;
        }

        /// <summary>
        /// Calculates the damage done by Firevortex.
        /// </summary>
        private sealed class FirevortexDamageMethod : ProjectileDamageMethod
        {
            /// <summary>
            /// Stores type information about the damage inflicted by the FirevortexDamageMethod.
            /// </summary>
            private static readonly DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
                DamageSource.Spell,
                ElementalSchool.Fire
            );

            /// <summary>
            /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
            /// using this <see cref="AttackDamageMethod"/>.
            /// </summary>
            /// <param name="user">The user of the attack.</param>
            /// <param name="target">The target of the attack.</param>
            /// <returns>The calculated result.</returns>
            public override AttackDamageResult GetDamageDone( Statable user, Statable target )
            {
                var userEx = (ExtendedStatable)user;

                // Did it resist?
                if( target.Resistances.TryResist( userEx, ElementalSchool.Fire ) )
                    return AttackDamageResult.CreateResisted( DamageTypeInfo );

                int damage = userEx.SpellPower.GetDamage( ElementalSchool.Fire );

                // Apply fixed modifiers:
                damage = userEx.DamageDone.ApplyFixed( damage, target, DamageTypeInfo );
                damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

                // Apply multipliers:
                damage = (int)(damage * this.GetDamageModifier()); // 1. Increased damage.
                damage = userEx.DamageDone.Apply( damage, target, DamageTypeInfo );
                damage = target.DamageTaken.Apply( damage, DamageTypeInfo );

                // Critical:
                bool isCrit = user.TryCrit( target );
                if( isCrit )
                    damage = (int)(damage * user.CritModifierSpell);

                return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
            }

            /// <summary>
            /// Gets the damage multiplier of the Firevortex.
            /// </summary>
            /// <returns>
            /// The current damage modification value of the FireVortex.
            /// </returns>
            private float GetDamageModifier()
            {
                var updateLogic = (FirevortexUpdateLogic)this.CurrentProjectile.StatefulUpdateLogic;
                return updateLogic.DamageModifier;
            }
        }
    }
}