// <copyright file="PoisonedShotSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.PoisonedShotSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Attacks.Ranged;
    using Zelda.Status;
    using Zelda.Status.Auras;
    using Zelda.Talents.Ranged;
    
    /// <summary>  
    /// PoisonedShot is a ranged attack that does 
    /// (RangedDamage * InstantDamagePenality) plus (RangedDamage*DamageOverTimeMultiplier) over time.
    /// <para>
    /// The effect also slows down the enemy by X% per TalentLevel.
    /// </para>
    /// </summary>
    internal sealed class PoisonedShotSkill : PlayerAttackSkill<PoisonedShotTalent, RangedPlayerAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PoisonedShotSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PoisonedShotSkill( PoisonedShotTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, PoisonedShotTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( PoisonedShotTalent.ManaNeededPoBM );

            // Setup effect:
            this.slowingEffect = new MovementSpeedEffect( 0.0f, StatusManipType.Percental );

            this.dotAura = new DamageOverTimeAura( PoisonedShotTalent.Duration, PoisonedShotTalent.TickTime, slowingEffect, talent.Owner ) {
                Name             = "PoisonedShot_Dot",
                PowerType        = LifeMana.Life,
                ManipulationType = StatusManipType.Fixed,
                DebuffFlags      = DebuffFlags.Poisoned | DebuffFlags.Slow
            };

            // Setup attack:
            var method = new PlayerRangedDamageMethod();
            method.SetValues( PoisonedShotTalent.InstantDamagePenality );
            method.Setup( serviceProvider );

            this.Attack = new RangedPlayerAttack( this.Player, method ) {
                Limiter = new Zelda.Attacks.Limiter.TimedAttackLimiter( this.Cooldown )
            };

            this.Attack.Setup( serviceProvider );

            var spriteLoader = serviceProvider.SpriteLoader;
            this.Attack.Settings.SetSprites(
                spriteLoader.LoadSprite( "Arrow_Steel_Up" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Down" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Left" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Right" )
            );

            // Setup dot effect:
            this.dotDamageMethod = new RangedNatureDotDamageMethod();
            this.dotDamageMethod.Setup( serviceProvider );

            this.Attack.HitSettings = new Zelda.Entities.Projectiles.ProjectileHitSettings() {
                AttackHitEffect = new Zelda.Attacks.HitEffects.DamageOverTimeAttackHitEffect( dotAura, dotDamageMethod )
            };
        }
        
        /// <summary>
        /// Refreshes the power of this PoisonedShotSkill based on the PoisonedShotTalent.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.dotDamageMethod.SetValues( this.Talent.DamageOverTimeMultiplier );
            this.slowingEffect.Value = this.Talent.MovementSlowingEffect;
        }

        /// <summary>
        /// The movement slowing affect applied to a poisoned target.
        /// </summary>
        private readonly MovementSpeedEffect slowingEffect;

        /// <summary>
        /// The DamageOverTimeAura.
        /// </summary>
        private readonly DamageOverTimeAura dotAura;

        /// <summary>
        /// The AttackDamageMethod that is used to calculate the damage done by the DOT effect.
        /// </summary>
        private readonly RangedNatureDotDamageMethod dotDamageMethod;
    }
}
