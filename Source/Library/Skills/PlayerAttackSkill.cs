// <copyright file="PlayerAttackSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.PlayerAttackSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills
{
    using Zelda.Attacks;
    using Zelda.Talents;

    /// <summary>
    /// Represents a <see cref="PlayerSkill"/> that internally uses an <see cref="Attack"/>
    /// when the Skill is fired.
    /// </summary>
    /// <typeparam name="TTalent">
    /// The exact type of the <see cref="SkillTalent"/>.
    /// </typeparam>
    /// <typeparam name="TAttack">
    /// The exact type of the <see cref="Attack"/>.
    /// </typeparam>
    internal abstract class PlayerAttackSkill<TTalent, TAttack> : PlayerTalentSkill<TTalent>
        where TTalent : SkillTalent
        where TAttack : Attack
    {
        /// <summary>
        /// Gets or sets the attack this PlayerAttackSkill{TTalent, TAttack} is based on.
        /// </summary>
        public TAttack Attack
        {
            get
            {
                return this.attack;
            }

            protected set
            {
                this.attack = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this PlayerAttackSkill{TTalent, TAttack} is currently useable.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                if( !this.HasRequiredMana || this.IsInactive )
                    return false;

                return this.Attack.IsUseable;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PlayerAttackSkill{TTalent, TAttack} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Talent"/> that descripes the power of the new PlayerAttackSkill{TTalent, TAttack}.
        /// </param>
        /// <param name="cooldownTime">
        /// The number of second the new PlayerAttackSkill{TTalent, TAttack} has to cooldown after it has been used.
        /// </param>
        protected PlayerAttackSkill( TTalent talent, float cooldownTime )
            : base( talent, cooldownTime )
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlayerAttackSkill{TTalent, TAttack} class.
        /// </summary>
        /// <param name="talent"> 
        /// The <see cref="Talent"/> that descripes the power of the new PlayerAttackSkill{TTalent, TAttack}.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown of the new PlayerAttackSkill{TTalent, TAttack}.
        /// </param>
        protected PlayerAttackSkill( TTalent talent, Cooldown cooldown )
            : base( talent, cooldown )
        {
        }

        /// <summary>
        /// Fires this PlayerAttackSkill{TTalent, TAttack}.
        /// </summary>
        /// <returns>
        /// true if this PlayerAttackSkill{TTalent, TAttack} has been used;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            return this.attack.Fire( null );
        }

        /// <summary>
        /// Updates this PlayerAttackSkill{TTalent, TAttack}.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.attack.Update( updateContext );
        }

        /// <summary>
        /// Represents the storage field of the Attack property.
        /// </summary>
        private TAttack attack;
    }
}
