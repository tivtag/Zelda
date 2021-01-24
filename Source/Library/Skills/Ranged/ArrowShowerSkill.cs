// <copyright file="ArrowShowerSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.ArrowShowerSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Ranged
{
    using System;
    using Zelda.Attacks.Ranged;
    using Zelda.Status;
    using Zelda.Talents.Ranged;
    
    /// <summary>
    /// The ArrowShowerTalent learns the player the ArrowShowerSkill.
    /// 'You fire one Multi Shot every X seconds for Y seconds.'
    /// </summary>
    internal sealed class ArrowShowerSkill : PlayerAttackSkill<ArrowShowerTalent, ArrowShowerAttack>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowShowerSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new ArrowShowerSkill.
        /// </param>
        /// <param name="multiShotTalent">
        /// The talent that modifies the power of each multi shot fired by the ArrowShowerSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ArrowShowerSkill( ArrowShowerTalent talent, MultiShotTalent multiShotTalent, IZeldaServiceProvider serviceProvider )
            : base( talent, ArrowShowerTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( ArrowShowerTalent.ManaNeededPoBM );

            this.multiShotTalent = multiShotTalent;
            
            this.Attack = new ArrowShowerAttack( talent.Owner, new MultiShotDamageMethod(), this.Cooldown );
            this.Attack.Setup( serviceProvider );
            this.Attack.DamageMethod.Setup( serviceProvider );
        }

        /// <summary>v
        /// Initializes this HeadshotSkill.
        /// </summary>
        public override void Initialize()
        {
            this.multiShotTalent.LevelChanged += this.OnMultiShotTalent_LevelChanged;
        }

        /// <summary>
        /// Uninitializes this HeadshotSkill.
        /// </summary>
        public override void Uninitialize()
        {
            this.multiShotTalent.LevelChanged -= this.OnMultiShotTalent_LevelChanged;
        }

        /// <summary>
        /// Refreshes the power of this ArrowShowerSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Attack.AttackCount     = this.Talent.AttackCount;
            this.Attack.ProjectileCount = this.multiShotTalent.ArrowCount;
        }        

        /// <summary>
        /// Gets called when the level of the Multi Shot talent has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnMultiShotTalent_LevelChanged( object sender, EventArgs e )
        {
            this.RefreshDataFromTalents();
        }

        /// <summary>
        /// Identifies the MultiShotTalent that modifies the power of each Multi Shot.
        /// </summary>
        private readonly MultiShotTalent multiShotTalent;
    }
}
