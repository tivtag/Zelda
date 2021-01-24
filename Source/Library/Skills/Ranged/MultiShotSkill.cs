// <copyright file="MultiShotSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.MultiShotSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Attacks.Ranged;
    using Zelda.Status;
    using Zelda.Talents.Ranged;

    /// <summary>
    /// The MultiShotSkill s a instant ranged attack that fires
    /// multiple arrows at the same time. A damage reduction is applied to those arrows.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class MultiShotSkill : PlayerAttackSkill<MultiShotTalent, MultiShotAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiShotSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new MultiShotSkill.
        /// </param>
        /// <param name="improvedTalent">
        /// The talent that improves the new MultiShotSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MultiShotSkill( MultiShotTalent talent, ImprovedMultiShotTalent improvedTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, MultiShotTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( MultiShotTalent.ManaNeededPoBM );
            this.improvedTalent = improvedTalent;

            this.Attack = new MultiShotAttack( talent.Owner, new MultiShotDamageMethod(), this.Cooldown );
            this.Attack.DamageMethod.Setup( serviceProvider );
            this.Attack.Setup( serviceProvider );
        }
        
        /// <summary>
        /// Refreshes the power of this MultiShotSkill based on the MultiShotTalent of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Attack.ProjectileCount = this.Talent.ArrowCount;
            this.Cooldown.TotalTime = MultiShotTalent.Cooldown - this.improvedTalent.CooldownReduction;
        }
                        
        /// <summary>
        /// Identifies the ImprovedMultiShotTalent that modifies the power of this MultiShotSkill.
        /// </summary>
        private readonly ImprovedMultiShotTalent improvedTalent;
    }
}
