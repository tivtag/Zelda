// <copyright file="QuickStrikeSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.QuickStrikeSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks.Melee;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.Talents.Melee;
    
    /// <summary> 
    /// <para>
    /// Swiftly strikes at the enemy dealing MeleeDamage. 
    /// Can proc Double Attack. Cooldown of {0} seconds.
    /// </para>
    /// <para>
    /// Requires a Dagger to be used!
    /// </para>
    /// </summary>
    internal sealed class QuickStrikeSkill : PlayerAttackSkill<QuickStrikeTalent, PlayerMeleeAttack>
    {
        /// <summary>
        /// Gets a value indicating whether this QuickStrikeSkill is currently in-active and
        /// as such unuseable.
        /// </summary>
        public override bool IsInactive
        {
            get
            {
                var weapon = this.equipment.WeaponHand;
                return weapon == null || weapon.Weapon.WeaponType != WeaponType.Dagger;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickStrikeSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal QuickStrikeSkill( QuickStrikeTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, talent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( QuickStrikeTalent.ManaNeededPoBM );
            this.equipment = this.Player.Equipment;

            this.Attack = new PlayerMeleeAttack( this.Player, new NormalPlayerMeleeDamageMethod(), this.Cooldown ) {
                IsPushing           = true,
                PushingPowerMinimum = 10.0f,
                PushingPowerMaximum = 25.0f
            };

            this.Attack.Setup( serviceProvider );
            this.Attack.DamageMethod.Setup( serviceProvider );
        }
        
        /// <summary>
        /// Refreshes the power of this QuickStrikeSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Cooldown.TotalTime = this.Talent.Cooldown;
        }

        /// <summary>
        /// Identifies the current EquiptmentStatus of the Player that owns this Skill.
        /// </summary>
        private readonly EquipmentStatus equipment;
    }
}
