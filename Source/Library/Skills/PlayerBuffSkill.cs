// <copyright file="PlayerBuffSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.PlayerBuffSkill{TTalent} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills
{
    using Zelda.Status;

    /// <summary>
    /// Represents a PlayerSkill that temp. applies an Aura to the Player
    /// when the uses the skill.
    /// </summary>
    /// <typeparam name="TTalent">
    /// The exact type of the <see cref="Zelda.Talents.Talent"/> that descripes the buff.
    /// </typeparam>
    internal abstract class PlayerBuffSkill<TTalent> : PlayerTalentSkill<TTalent>
        where TTalent : Zelda.Talents.Talent
    {
        /// <summary>
        /// Gets a value indicating whether this PlayerBuffSkill is currently useable.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                return this.Cooldown.IsReady && this.HasRequiredMana;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this PlayerBuffSkill is only limited by its own cooldown
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
        /// Gets or sets the TimedAura which contains the buff 
        /// effects of this PlayerBuffSkill{TTalent}
        /// </summary>
        protected TimedAura Aura
        {
            get
            {
                return this.aura;
            }

            set
            {
                this.aura = value;
            }
        }
          
        /// <summary>
        /// Initializes a new instance of the PlayerBuffSkill{TTalent} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Zelda.Talents.Talent"/> that descripes the power of the new PlayerBuffSkill{TTalent}.
        /// </param>
        /// <param name="cooldownTime">
        /// The number of second the new PlayerBuffSkill{TTalent} has to cooldown after it has been used.
        /// </param>
        protected PlayerBuffSkill( TTalent talent, float cooldownTime )
            : base( talent, cooldownTime )
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlayerBuffSkill{TTalent} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Zelda.Talents.Talent"/> that descripes the power of the new PlayerBuffSkill{TTalent,}.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown of the new PlayerBuffSkill{TTalent}.
        /// </param>
        protected PlayerBuffSkill( TTalent talent, Cooldown cooldown )
            : base( talent, cooldown )
        {
        }

        /// <summary>
        /// Uses this PlayerBuffSkill{TTalent}.
        /// </summary>
        /// <returns>
        /// true if this PlayerBuffSkill{TTalent} has been used;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            this.aura.ResetDuration();
            this.AuraList.Add( this.aura );
            this.Cooldown.Reset();
            return true;
        }

        /// <summary>
        /// Refreshes the strength of this PlayerBuffSkill{TTalent} based on the talents the player has choosen.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            bool removed = this.AuraList.Remove( this.aura );

            this.RefreshAuraEffect();

            if( removed )
            {
                this.AuraList.Add( this.aura );
            }
        }

        /// <summary>
        /// Refreshes the strength of the individual buff effect of this PlayerBuffSkill{TTalent}.
        /// </summary>
        protected virtual void RefreshAuraEffect()
        {
        }

        /// <summary>
        /// The TimedAura that holds the StatusEffects that gets applied by this PlayerBuffSkill{TTalent}.
        /// </summary>
        private TimedAura aura;
    }
}
