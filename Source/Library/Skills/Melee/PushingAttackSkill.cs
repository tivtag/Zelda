// <copyright file="PushingAttackSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.PushingAttackSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks.Melee;
    using Zelda.Talents.Melee;
    
    /// <summary> 
    /// Pushing Attack pushes the enemy away with full power dealing MeleeDamage,
    /// increasing pushing power by X to Y.
    /// </summary>
    internal sealed class PushingAttackSkill : PlayerAttackSkill<PushingAttackTalent, PlayerMeleeAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PushingAttackSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The PushingAttackTalent that 'learns' the player the new PushingAttackSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal PushingAttackSkill( PushingAttackTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, PushingAttackTalent.Cooldown )
        {
            this.Attack = new PlayerMeleeAttack( this.Player, new PlayerMeleeDamageMethod(), this.Cooldown ) {
                IsPushing = true
            };

            this.Attack.DamageMethod.Setup( serviceProvider );
            this.Attack.Setup( serviceProvider );
        }

        /// <summary>
        /// Refreshes the power of this PushingAttackSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Attack.PushingPowerMinimum = this.Talent.PushingPowerMinimum;
            this.Attack.PushingPowerMaximum = this.Talent.PushingPowerMaximum;
        }
    }
}
