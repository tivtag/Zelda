// <copyright file="BashSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.BashSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks.Melee;
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary> 
    /// Bash is a powerful instant melee attack to provides additional damage
    /// and if specced crit chance compared to a normal attack.
    /// </summary>
    internal sealed class BashSkill : PlayerAttackSkill<BashTalent, PlayerMeleeAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BashSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="improvedBashTalent">
        /// The talent that 'learns' the player an improved to the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal BashSkill( BashTalent talent, ImprovedBashTalent improvedBashTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, talent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( BashTalent.ManaNeededPoBM );

            this.improvedBashTalent = improvedBashTalent;            
            this.method = new BashDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new PlayerMeleeAttack( this.Player, method, this.Cooldown ) {
                IsPushing           = true,
                PushingPowerMinimum = 50.0f,
                PushingPowerMaximum = 60.0f
            };

            this.Attack.Setup( serviceProvider );
        }
        
        /// <summary>
        /// Refreshes the power of this BashSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.method.SetValues( 
                this.Talent.DamageMultiplier,
                this.Talent.FixedDamageIncrease,
                this.improvedBashTalent.BashCritIncrease
            );

            this.Cooldown.TotalTime = this.Talent.Cooldown;
        }
                
        /// <summary>
        /// Identifies the supporting talent related to this BashSkill.
        /// </summary>
        private readonly ImprovedBashTalent improvedBashTalent;
        
        /// <summary>
        /// The damage method that calculates how much damage this BashSkill does.
        /// </summary>
        private readonly BashDamageMethod method;
    }
}