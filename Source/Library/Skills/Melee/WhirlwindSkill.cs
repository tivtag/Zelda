// <copyright file="WhirlwindSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.WhirlwindSkill class.
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
    /// Whirlwind is a powerful attack that needs to be charged up.
    /// After the charge is complete the player turns like a Whirlwind,
    /// hitting and pushing all enemies very hard.
    /// As a bonus the Whirlwind attack can't be dodged.
    /// </summary>
    internal sealed class WhirlwindSkill : PlayerAttackSkill<WhirlwindTalent, WhirlwindAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhirlwindSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The WhirlwindTalent that 'learns' the player the new WhirlwindSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal WhirlwindSkill( WhirlwindTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, 0.0f )
        {
            this.Cost = ManaCost.PercentageOfBase( WhirlwindTalent.ManaNeededPoBM );

            this.method = new WhirlwindDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new WhirlwindAttack( talent.Owner, method, this.Cooldown );
            this.Attack.Setup( serviceProvider );
        }
        
        /// <summary>
        /// Refreshes the power of this WhirlwindSkill based on the Talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Cooldown.TotalTime = this.Talent.Cooldown;
            this.method.SetValues( this.Talent.DamageMultiplier );
        }
                
        /// <summary>
        /// The AttackDamageMethod that is used to calculate damage done by the WhirlwindAttack.
        /// </summary>
        private readonly WhirlwindDamageMethod method;
    }
}
