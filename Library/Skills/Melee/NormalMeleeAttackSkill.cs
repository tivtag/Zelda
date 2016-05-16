// <copyright file="NormalMeleeAttackSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.NormalMeleeAttackSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using System;
    using Zelda.Attacks.Melee;
    using Zelda.Entities;
    
    /// <summary>
    /// Defines the <see cref="Skill"/> that enables the
    /// Player to attack with his sword.
    /// </summary>
    internal sealed class NormalMeleeAttackSkill : Skill
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the localized description of this <see cref="NormalMeleeAttackSkill"/>.
        /// </summary>
        public override string Description
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="NormalMeleeAttackSkill"/> is useable 
        /// depending on the current state of the user.
        /// </summary>
        /// <remarks>
        /// The cooldown is not taken into account.
        /// </remarks>
        public override bool IsUseable
        {
            get
            {
                return this.attack.IsUseable;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalMeleeAttackSkill"/> class.
        /// </summary>
        /// <param name="player">
        /// The <see cref="PlayerEntity"/> that owns the new NormalMeleeAttackSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public NormalMeleeAttackSkill( PlayerEntity player, IZeldaServiceProvider serviceProvider )
            : base( Resources.AttackMelee, null, serviceProvider.SpriteLoader.LoadSprite( "Symbol_AttackMelee" ), player.Statable )
        {
            var method = new NormalPlayerMeleeDamageMethod();
            method.Setup( serviceProvider );

            this.attack = new PlayerMeleeAttack( player, method ) {
                IsPushing           = true,
                PushingPowerMinimum = 34.0f,
                PushingPowerMaximum = 55.0f,
                Limiter = new Zelda.Attacks.Limiter.MeleeAttackSpeedBasedAttackLimiter( player.Statable )
            };

            this.attack.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Fires this <see cref="NormalMeleeAttackSkill"/>.
        /// </summary>
        /// <returns>
        /// true if this Skill has been used;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            return this.attack.Fire( null );
        }       
        
        /// <summary>
        /// Updates this <see cref="Skill"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.attack.Update( updateContext );
        }

        /// <summary>
        /// There are no talents that modify the NormalMeleeAttackSkill.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the <see cref="PlayerMeleeAttack"/> object related to this Skill.
        /// </summary>
        private readonly PlayerMeleeAttack attack;

        #endregion
    }
}
