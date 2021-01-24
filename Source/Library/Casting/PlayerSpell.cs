// <copyright file="PlayerSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.PlayerSpell class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Casting
{
    using Zelda.Attacks;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;
    
    /// <summary>
    /// Represents a Spell that can only be used by the Player.
    /// </summary>
    public abstract class PlayerSpell : Spell
    {        
        /// <summary>
        /// Gets a value indicating whether this <see cref="PlayerSpell"/> is useable depending on the state of its owner.
        /// </summary>
        /// <remarks>
        /// E.g. one usually can't use an attack while swimming,
        /// or if there is not enough mana to use it.
        /// </remarks>
        public override bool IsUseable
        {
            get
            {
                return this.player.Equipment.CanCast && !this.player.Moveable.IsSwimming;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Spell can currently be cast.
        /// </summary>
        /// <remarks>
        /// This property is the ultimate castable check.
        /// </remarks>
        protected override bool IsCastable
        {
            get
            {
                if( base.IsCastable )
                {
                    return this.player.DrawDataAndStrategy.SpecialAnimation == PlayerSpecialAnimation.None;
                }
                
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSpell"/> class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new PlayerSpell.
        /// </param>
        /// <param name="castTime">
        /// The time it takes for the new PlayerSpell to cast.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new PlayerSpell does. 
        /// </param>
        protected PlayerSpell( PlayerEntity owner, float castTime, AttackDamageMethod method )
            : base( owner, castTime, method )
        {
            this.player = owner;
        }

        /// <summary>
        /// The PlayerEntity that owns this <see cref="PlayerSpell"/>.
        /// </summary>
        protected readonly PlayerEntity player;
    }
}
