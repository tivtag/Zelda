// <copyright file="FirewhirlSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Magic.FirewhirlSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Magic
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Casting.Spells;
    using Zelda.Entities;
    using Zelda.Entities.Projectiles;
    using Zelda.Status;
    using Zelda.Status.Auras;
    using Zelda.Status.Pooling;
    using Zelda.Talents.Magic;
    
    /// <summary>
    /// Firewhirl deals 103/106/109/112/115% to 105/110/115/120/125% fire damage.
    /// 2.4/2.3/2.2/2.1/2.0 seconds cast time.
    /// 5 seconds cooldown.
    /// </summary>
    /// <remarks>
    /// Firewhirl can be enhanced using the <see cref="CorrosiveFireTalent"/>.
    /// </remarks>
    internal sealed class FirewhirlSkill : PlayerSpellSkill<FirewhirlTalent, ProjectilePlayerSpell>, ICooldownDependant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirewhirlSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new FirewhirlSkill.
        /// </param>
        /// <param name="impactTheoryTalent">
        /// The talent that gives Firewhirl a chance to split into more Firewhirls upon impact.
        /// </param>
        /// <param name="corrosiveFireTalent">
        /// The talent that adds a damage-over-time effect to Firewhirl.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FirewhirlSkill( 
            FirewhirlTalent talent,
            ImpactTheoryTalent impactTheoryTalent,
            CorrosiveFireTalent corrosiveFireTalent, 
            IZeldaServiceProvider serviceProvider )
            : base( talent, talent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBaseAndTotal( FirewhirlTalent.ManaCostOfBaseMana, FirewhirlTalent.ManaCostOfTotalMana );

            this.method = new FirewhirlDamageMethod();
            this.method.Setup( serviceProvider );

            this.Spell = new ProjectilePlayerSpell( this.Player, this.Talent.CastTime, this.method ) {
                HitSettings = new Zelda.Entities.Projectiles.ProjectileHitSettings(),
                Limiter = new Zelda.Attacks.Limiter.TimedAttackLimiter( this.Cooldown )
            };

            this.hitEffect = new FirewhirlHitEffect( this.Spell, impactTheoryTalent, corrosiveFireTalent, serviceProvider );            
            this.SetupSpell( serviceProvider );
        }

        /// <summary>
        /// Setups the Firewhirl spell.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void SetupSpell( IZeldaServiceProvider serviceProvider )
        {
            this.Spell.Settings.Speed = new Atom.Math.IntegerRange( 58, 75 );
            this.Spell.Settings.PiercingMode = Zelda.Entities.Projectiles.ProjectilePiercingChanceMode.None;
            this.Spell.Settings.TravellingSound = new Zelda.Audio.SoundSampleSettings() {
                SampleName = "FireRing.wav",
                Distance = new Atom.Math.FloatRange( 16.0f * 2, 16.0f * 10 ),
                IsLooping = true,
                Volumne = 0.8f
            };

            this.Spell.Settings.SetSprites(
                serviceProvider.SpriteLoader.LoadAnimatedSprite( "Firewhirl" )
            );

            this.Spell.HitSettings.AttackHitEffect = this.hitEffect;
            this.Spell.HitSettings.SoundSample.Volumne = 0.75f;
            this.Spell.HitSettings.SoundSample.Distance = new FloatRange( 16.0f * 1.4f, 16.0f * 9 );
            this.Spell.HitSettings.SoundSample.SampleName = "FireHit_Wizzrobe.wav";

            this.Spell.Setup( serviceProvider );
        }

        /// <summary>
        /// Refreshes the cooldown of this FirewhirlSkill.
        /// </summary>
        public void RefreshCooldown()
        {
            float fixedValue, multiplierValue;
            this.AuraList.GetEffectValues(
                SkillCooldownEffect.GetIdentifier<FirevortexSkill>(),
                out fixedValue,
                out multiplierValue
            );

            this.Cooldown.TotalTime = (this.Talent.Cooldown + fixedValue) * multiplierValue;
        }

        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Spell.CastTime = this.Talent.CastTime;

            this.method.SetValues( this.Talent.MinumumDamageModifier, this.Talent.MaximumDamageModifier );
            this.hitEffect.RefreshDataFromTalents();

            this.RefreshCooldown();
        }

        /// <summary>
        /// The FirewhirlDamageMethod that calculates how much damage an arrow launched by the FirewhirlSkill does.
        /// </summary>
        private readonly FirewhirlDamageMethod method;

        /// <summary>
        /// The FirewhirlHitEffect that is applied when Firewhirl hits a target.
        /// </summary>
        private readonly FirewhirlHitEffect hitEffect;

        #region [ class ProjectileData ]

        /// <summary>
        /// Represents the data that is additonally attached
        /// to the Projectiles of the Firewhirl spell.
        /// </summary>
        private sealed class ProjectileData
        {
            /// <summary>
            /// Gets or sets the number of times the Projectile has
            /// been split so far.
            /// </summary>
            public int NumberOfSplits
            {
                get;
                set;
            }
        }

        #endregion

        #region [ class FirewhirlHitEffect ]

        /// <summary>
        /// Defines the IAttackHitEffect that gets applies when a
        /// Firewhirl has hit a target.
        /// </summary>
        private class FirewhirlHitEffect : Zelda.Attacks.IAttackHitEffect
        {
            /// <summary>
            /// Initializes a new instance of the FirewhirlHitEffect class.
            /// </summary>
            /// <param name="firewhirlSpell">
            /// The Firewhirl spell object.
            /// </param>
            /// <param name="impactTheoryTalent">
            /// Identifies the ImpactTheoryTalent that gives Firewhirl a chance to split into more Firewhirls upon impact.
            /// </param>
            /// <param name="corrosiveFireTalent">
            /// Identifies the CorrosiveFireTalent that modifies the strength of the corrosive fire damage-over-time effect.
            /// </param>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public FirewhirlHitEffect(
                ProjectilePlayerSpell firewhirlSpell,
                ImpactTheoryTalent impactTheoryTalent,
                CorrosiveFireTalent corrosiveFireTalent,
                IZeldaServiceProvider serviceProvider )
            {
                this.corrosiveFireHitEffect = new CorrosiveFireHitEffect( corrosiveFireTalent, serviceProvider );
                this.impactTheoryProjectileDestroyedEffect = new ImpactTheoryProjectileDestroyedEffect(
                    firewhirlSpell,
                    impactTheoryTalent, 
                    serviceProvider
                );
            }

            /// <summary>
            /// Refreshes the strength of this FirewhirlHitEffect.
            /// </summary>
            public void RefreshDataFromTalents()
            {
                this.corrosiveFireHitEffect.RefreshDataFromTalents();
                this.impactTheoryProjectileDestroyedEffect.RefreshDataFromTalents();
            }

            /// <summary>
            /// Called when the FirewhirlHitEffect is to be applied.
            /// </summary>
            /// <param name="user">
            /// The user of the attack.
            /// </param>
            /// <param name="target">
            /// The target of the attack.
            /// </param>
            public void OnHit( Zelda.Status.Statable user, Zelda.Status.Statable target )
            {
                this.corrosiveFireHitEffect.OnHit( user, target );
            }

            /// <summary>
            /// The effect that applies the Corrosive Fire effect.
            /// </summary>
            private readonly CorrosiveFireHitEffect corrosiveFireHitEffect;

            /// <summary>
            /// The effect that implements the Impact Theory effect.
            /// </summary>
            private readonly ImpactTheoryProjectileDestroyedEffect impactTheoryProjectileDestroyedEffect;

            #region [ class ImpactTheoryProjectileDestroyedEffect ]

            /// <summary>
            /// Defines the effect that gets invoked when a Firewhirl projectile has been destroyed.
            /// </summary>
            /// <seealso cref="ImpactTheoryTalent"/>
            private sealed class ImpactTheoryProjectileDestroyedEffect
            {
                #region [ Initialization ]

                /// <summary>
                /// Initializes a new instance of the ImpactTheoryProjectileDestroyedEffect class.
                /// </summary>
                /// <param name="fireWhirlSpell">
                /// The Firewhirl spell object.
                /// </param>
                /// <param name="impactTheoryTalent">
                /// Identifies the ImpactTheoryTalent that gives Firewhirl a chance to 
                /// split into more Firewhirls upon impact.
                /// </param>
                /// <param name="serviceProvider">
                /// Provides fast access to game-related services.
                /// </param>
                public ImpactTheoryProjectileDestroyedEffect(
                    ProjectilePlayerSpell fireWhirlSpell,
                    ImpactTheoryTalent impactTheoryTalent,
                    IZeldaServiceProvider serviceProvider )
                {
                    this.rand = serviceProvider.Rand;
                    this.fireWhirlSpell = fireWhirlSpell;
                    this.impactTheoryTalent = impactTheoryTalent;

                    this.HookEvents();
                }

                /// <summary>
                /// Hooks up the EventHandlers for this ImpactTheoryProjectileDestroyedEffect.
                /// </summary>
                private void HookEvents()
                {
                    this.fireWhirlSpell.ProjectileDestroyed += this.OnFirewhirlDestroyed;
                }

                /// <summary>
                /// Refreshes the strength of this ImpactTheoryProjectileDestroyedEffect.
                /// </summary>
                public void RefreshDataFromTalents()
                {
                    this.chanceToProc = impactTheoryTalent.ProcChance;
                }

                #endregion

                #region [ Methods ]

                /// <summary>
                /// Gets called when a Firewhirl projectile gets destroyed.
                /// </summary>
                /// <param name="sender">
                /// The sender of the event.
                /// </param>
                /// <param name="firewhirl">
                /// The firewhirl projectile that has been destroyed.
                /// </param>
                private void OnFirewhirlDestroyed( object sender, Projectile firewhirl )
                {
                    if( this.ShouldSplitIntoMoreFirewhirls( firewhirl ) )
                    {
                        this.Split( firewhirl );
                    }
                }              

                /// <summary>
                /// Gets a value indicating whether a destroyed Firewhirl should
                /// split into more Firewhirls.
                /// </summary>
                /// <param name="projectile">
                /// The firewhirl Projectile that might split into multiple Firewhirls.
                /// </param>
                /// <returns>
                /// True if the splitting logic should be executed;
                /// otherwise false.
                /// </returns>
                private bool ShouldSplitIntoMoreFirewhirls( Zelda.Entities.Projectiles.Projectile projectile )
                {
                    if( this.impactTheoryTalent.Level == 0 )
                        return false;

                    if( projectile.DistanceTravelledSquared < ImpactTheoryTalent.RequiredTravelDistanceSquared )
                        return false;

                    if( GetNumberOfSplits( projectile ) >= ImpactTheoryTalent.MaximumNumberOfContinuousSplits )
                        return false;

                    float roll = this.rand.UncheckedRandomRange( 0.0f, 100.0f );
                    return roll <= this.chanceToProc;
                }

                /// <summary>
                /// Gets the number of times the given Firewhirl projectile has been split.
                /// </summary>
                /// <param name="firewhirl">
                /// The Firewhirl projectile.
                /// </param>
                /// <returns>
                /// The split count.
                /// </returns>
                private static int GetNumberOfSplits( Projectile firewhirl )
                {
                    var data = firewhirl.OptionalData as ProjectileData;

                    if( data != null )
                    {
                        return data.NumberOfSplits;
                    }
                    else
                    {
                        return 0;
                    }
                }

                /// <summary>
                /// Sets the number of times the given Firewhirl projectile has been split.
                /// </summary>
                /// <param name="splitCount">
                /// The number of splits to set.
                /// </param>
                /// <param name="firewhirl">
                /// The Firewhirl projectile.
                /// </param>
                private static void SetNumberOfSplits( int splitCount, Projectile firewhirl )
                {
                    if( splitCount > 0 )
                    {
                        firewhirl.OptionalData = new ProjectileData()
                        {
                            NumberOfSplits = splitCount
                        };
                    }
                }

                /// <summary>
                /// Splits the given firewhirl into more firewhirls;
                /// which in-turn can split too.
                /// </summary>
                /// <param name="parentFirewhirl">
                /// The firewhirl projectile that should be split.
                /// </param>
                private void Split( Zelda.Entities.Projectiles.Projectile parentFirewhirl )
                {
                    Vector2 parentPosition = parentFirewhirl.Collision.Center;
                    int floorNumber = parentFirewhirl.FloorNumber;

                    int splitCount = this.impactTheoryTalent.GetNumberOfSplits( this.rand );
                    int newNumberOfContinousSplits = GetNumberOfSplits( parentFirewhirl ) + 1;

                    float angle = rand.RandomRange( 0.0f, Constants.TwoPi );
                    float angleBetweenTwoWhirls = Constants.TwoPi / splitCount;

                    for( int i = 0; i < splitCount; ++i, angle += angleBetweenTwoWhirls )
                    {
                        Vector2 direction = new Vector2( 
                            (float)System.Math.Cos( angle ),
                           -(float)System.Math.Sin( angle )
                        );

                        Vector2 spawnPosition = this.GetProjectileSpawnPosition( parentPosition, direction );

                        if( this.CanSpawnProjectileAt( spawnPosition, floorNumber ) )
                        {
                            this.FireProjectile( spawnPosition, direction, floorNumber, newNumberOfContinousSplits );
                        }
                    }
                }

                /// <summary>
                /// Gets the projectile spawning position for an offspring Firewhirl.
                /// </summary>
                /// <param name="position">
                /// The position of the parent Firewhirl.
                /// </param>
                /// <param name="direction">
                /// The direction the projectile is heading.
                /// </param>
                /// <returns>
                /// The spawning position.
                /// </returns>
                private Vector2 GetProjectileSpawnPosition( Vector2 position, Vector2 direction )
                {
                    position += GetOffset( direction );
                    return this.fireWhirlSpell.GetCenteredSpawnPosition( position, Direction4.Left );
                }

                /// <summary>
                /// Gets the total offset applied to the parent position to get the offspring spawning
                /// position.
                /// </summary>
                /// <param name="direction">
                /// The direction the offspring is heading.
                /// </param>
                /// <returns>
                /// The offset to apply.
                /// </returns>
                private static Vector2 GetOffset( Vector2 direction )
                {
                    return direction * ImpactTheoryTalent.Offset;
                }
                
                /// <summary>
                /// Gets a value indicating whether a Firewhirl offspring can spawn at the given position.
                /// </summary>
                /// <param name="position">
                /// The position´at which the projectile is supposed to spawn.
                /// </param>
                /// <param name="floorNumber">
                /// The number of the floor to spawn at.
                /// </param>
                /// <returns>
                /// Returns true if the offspring can spawn;
                /// otherwise false.
                /// </returns>
                private bool CanSpawnProjectileAt( Vector2 position, int floorNumber )
                {
                    var actionLayer = this.GetActionLayer( floorNumber );

                    int tileX = (int)position.X / 16;
                    int tileY = (int)position.Y / 16;
                    int tileId = actionLayer.GetTileAtSafe( tileX, tileY );

                    return FlyingTileHandler.IsWalkableByDefault( (ActionTileId)tileId );
                }

                /// <summary>
                /// Gets the tile map action layer associated with the given floorNumber.
                /// </summary>
                /// <param name="floorNumber">
                /// The number of the floor.
                /// </param>
                /// <returns>
                /// The requested action layer.
                /// </returns>
                private Atom.Scene.Tiles.TileMapDataLayer GetActionLayer( int floorNumber )
                {
                    ZeldaScene scene = this.fireWhirlSpell.Owner.Scene;
                    var tileMap = scene.Map;
                    var tileFloor = tileMap.GetFloor( floorNumber );
                    var actionLayer = tileFloor.ActionLayer;
                    return actionLayer;
                }

                /// <summary>
                /// Fires an offspring Firewhirl projectile.
                /// </summary>
                /// <param name="position">
                /// The position the offspring should be spawned at.
                /// </param>
                /// <param name="direction">
                /// The direction the offspring should be heading.
                /// </param>
                /// <param name="floorNumber">
                /// The number of the floor the offspring should be spawned on.
                /// </param>
                /// <param name="numberOfContinousSplits">
                /// The number of times the projectile to fire has been split by now.
                /// </param>
                private void FireProjectile( Vector2 position, Vector2 direction, int floorNumber, int numberOfContinousSplits )
                {
                    var projectile = this.fireWhirlSpell.FireFromInto( 
                        position, direction, Direction4.Left, floorNumber
                    );
                    
                    SetNumberOfSplits( numberOfContinousSplits, projectile );
                }

                #endregion

                #region [ Fields ]

                /// <summary>
                /// The (cached) chance for a Firewhirl to split into more Firewhirls.
                /// </summary>
                private float chanceToProc;

                /// <summary>
                /// Identifies the Firewhirl spell object.
                /// </summary>
                private readonly ProjectilePlayerSpell fireWhirlSpell;

                /// <summary>
                /// Identifies the ImpactTheoryTalent that gives Firewhirl a chance to
                /// split into more Firewhirls upon impact.
                /// </summary>
                private readonly ImpactTheoryTalent impactTheoryTalent;

                /// <summary>
                /// A random number generator.
                /// </summary>
                private readonly RandMT rand;

                #endregion
            }

            #endregion

            #region [ class CorrossiveFireHitEffect ]

            /// <summary>
            /// Defines the IAttackHitEffect that applies a Damage Over Time effect
            /// when Firewhirl hits an target.
            /// </summary>
            /// <seealso cref="CorrosiveFireTalent"/>
            private sealed class CorrosiveFireHitEffect : Zelda.Attacks.IAttackHitEffect
            {
                /// <summary>
                /// Initializes a new instance of the CorrosiveFireHitEffect class.
                /// </summary>
                /// <param name="corrosiveFireTalent">
                /// Identifies the CorrosiveFireTalent that modifies the strength of 
                /// the corrosive fire damage-over-time effect.
                /// </param>
                /// <param name="serviceProvider">
                /// Provides fast access to game-related services.
                /// </param>
                public CorrosiveFireHitEffect(
                    CorrosiveFireTalent corrosiveFireTalent,
                    IZeldaServiceProvider serviceProvider )
                {
                    this.corrosiveFireTalent = corrosiveFireTalent;
                    this.corrosiveFireAuraPool = AuraPool<DamageOverTimeAura>.Create(
                        5, 
                        CreatePooledAura
                    );

                    this.corrosiveFireMethod.Setup( serviceProvider );
                }

                /// <summary>
                /// Refreshes the strength of this FirewhirlHitEffect.
                /// </summary>
                public void RefreshDataFromTalents()
                {
                    this.corrosiveFireMethod.SetValues(
                        this.corrosiveFireTalent.MinimumDamageModifier,
                        this.corrosiveFireTalent.MaximumDamageModifier
                    );
                }

                /// <summary>
                /// Called when the FirewhirlHitEffect is to be applied.
                /// </summary>
                /// <param name="user">
                /// The user of the attack.
                /// </param>
                /// <param name="target">
                /// The target of the attack.
                /// </param>
                public void OnHit( Zelda.Status.Statable user, Zelda.Status.Statable target )
                {
                    if( ShouldApplyCorrosiveFire() )
                    {
                        this.ApplyCorrosiveFire( user, target );
                    }
                }

                /// <summary>
                /// Gets a value indicating whether the Corrosive Fire damage-over-time
                /// effect should be applies.
                /// </summary>
                /// <returns>
                /// true if it should be applied;
                /// otherwise false.
                /// </returns>
                private bool ShouldApplyCorrosiveFire()
                {
                    return this.corrosiveFireTalent.Level >= 1;
                }

                /// <summary>
                /// Applies the corrosive fire damage-over-time effect to the given target.
                /// </summary>
                /// <param name="user">
                /// The user of the corrosive fire.
                /// </param>
                /// <param name="target">
                /// The target of the corrosive fire.
                /// </param>
                private void ApplyCorrosiveFire( Statable user, Statable target )
                {
                    var aura = this.GetCorrosiveFireAura();

                    aura.ResetDuration();
                    aura.ResetTick();
                    aura.Attacker = user.Owner as IAttackableEntity;

                    AttackDamageResult totalDamage = this.corrosiveFireMethod.GetDamageDone( user, target );
                    if( totalDamage.AttackReceiveType == AttackReceiveType.Resisted )
                        return;

                    aura.DamageEachTick = totalDamage.Damage / aura.TickCount;
                    if( aura.DamageEachTick == 0 )
                        aura.DamageEachTick = 1;

                    target.AuraList.Add( aura );
                }

                /// <summary>
                /// Gets a DamageOverTimeAura that can be used for the Corrosive Fire effect.
                /// </summary>
                /// <returns>
                /// An DamageOverTimeAura that is ready to be setup and applied.
                /// </returns>
                private DamageOverTimeAura GetCorrosiveFireAura()
                {
                    var auraNode = corrosiveFireAuraPool.Get();
                    var auraWrapper = auraNode.Item;
                    var aura = auraWrapper.PooledObject;
                    return aura;
                }

                /// <summary>
                /// Creates a new PooledDamageOverTimeAura; pooled in an AuraPool{DamageOverTimeAura}.
                /// </summary>
                /// <returns>
                /// A newly created PooledDamageOverTimeAura.
                /// </returns>
                private static PooledDamageOverTimeAura CreatePooledAura()
                {
                    var aura = new DamageOverTimeAura(
                        CorrosiveFireTalent.Duration,
                        CorrosiveFireTalent.Duration / CorrosiveFireTalent.TickCount
                    );

                    return new PooledDamageOverTimeAura( aura );
                }

                /// <summary>
                /// Identifies the CorrosiveFireTalent that modifies the strength of
                /// the corrosive fire damage-over-time effect.
                /// </summary>
                private readonly CorrosiveFireTalent corrosiveFireTalent;

                /// <summary>
                /// The AuraPool that stores the DamageOverTimeAuras used for the corrosive fire damage-over-time effect.
                /// </summary>
                private readonly AuraPool<DamageOverTimeAura> corrosiveFireAuraPool;

                /// <summary>
                /// The DamageMethod used to calculate the damage done by the corrosive fire damage-over-time effect.
                /// </summary>
                private readonly CorrosiveFireDamageMethod corrosiveFireMethod = new CorrosiveFireDamageMethod();
            }

            #endregion
        }

        #endregion
    }
}