// <copyright file="ShieldBlockSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Skills.Melee.ShieldBlockSkill class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Skills.Melee
{
    using Zelda.Status;
    using Zelda.Talents.Melee;
    
    /// <summary>
    /// Shield Block increases Chance to Block by 10/20/30 for 12 seconds.
    /// 60 seconds cooldown.
    /// </summary>
    internal sealed class ShieldBlockSkill : PlayerBuffSkill<ShieldBlockTalent>
    {
        /// <summary>
        /// Gets a value indicating whether this ShieldBlockSkill is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        public override bool IsOnlyLimitedByCooldown
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ShieldBlockSkill is currently useable.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                return base.IsUseable && this.Player.Statable.CanBlock;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldBlockSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the new ShieldBlockSkill.
        /// </param>
        internal ShieldBlockSkill( ShieldBlockTalent talent )
            : base( talent, ShieldBlockTalent.Cooldown )
        {
            this.effect = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Block );
            this.Aura = new TimedAura( ShieldBlockTalent.Duration, effect ) {
                Name                = this.LocalizedName,
                IsVisible           = true,
                Symbol              = talent.Symbol,
                DescriptionProvider = this
            };
        }
        
        /// <summary>
        /// Refreshes the strength of the individual buff effect of this ShieldBlockSkill.
        /// </summary>
        protected override void  RefreshAuraEffect()
        {
            this.effect.Value = this.Talent.BlockChanceIncrease;
        }

        /// <summary>
        /// The effect that is applied by this ShieldBlockSkill.
        /// </summary>
        private readonly ChanceToStatusEffect effect;
    }
}
