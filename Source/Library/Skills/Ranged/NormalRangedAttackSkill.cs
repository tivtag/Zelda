// <copyright file="NormalRangedAttackSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.NormalRangedAttackSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Ranged
{
    using System;
    using Atom.Xna;
    using Zelda.Attacks.Ranged;
    using Zelda.Entities;
    
    /// <summary>
    /// Defines the <see cref="Skill"/> that enables the
    /// Player to attack with his bow.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class NormalRangedAttackSkill : Skill
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the localized description of this <see cref="Skill"/>.
        /// </summary>
        public override string Description
        {
            get
            { 
                throw new NotImplementedException(); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is useable 
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
        /// Initializes a new instance of the <see cref="NormalRangedAttackSkill"/> class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public NormalRangedAttackSkill( PlayerEntity player, IZeldaServiceProvider serviceProvider )
            : base( Resources.AttackRanged, null, serviceProvider.SpriteLoader.LoadSprite( "Symbol_AttackRanged" ), player.Statable )
        {
            this.attack = new RangedPlayerAttack( player, new NormalPlayerRangedDamageMethod() ) {
                Limiter = new Zelda.Attacks.Limiter.RangedAttackSpeedBasedAttackLimiter( player.Statable )
            };

            this.attack.Setup( serviceProvider );
            this.attack.DamageMethod.Setup( serviceProvider );

            var spriteLoader = serviceProvider.SpriteLoader;
            this.attack.Settings.SetSprites(
                spriteLoader.LoadSprite( "Arrow_Wood_Up" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Down" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Left" ),
                spriteLoader.LoadSprite( "Arrow_Wood_Right" )
            );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this NormalRangedAttackSkill, firing an arrow.
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
        /// The attack related to this skill.
        /// </summary>
        private readonly RangedPlayerAttack attack;

        #endregion
    }
}
