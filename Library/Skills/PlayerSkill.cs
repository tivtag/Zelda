// <copyright file="PlayerSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.PlayerSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills
{
    using Zelda.Entities;

    /// <summary>
    /// Represents a <see cref="Skill"/> that can only be used by the player.
    /// </summary>
    /// <remarks>
    /// Most skills are player skills that require an investment into a talent.
    /// </remarks>
    public abstract class PlayerSkill : Skill
    {
        /// <summary>
        /// Gets the <see cref="Zelda.Status.ExtendedStatable"/> component
        /// of the <see cref="PlayerEntity"/> that owns this <see cref="PlayerSkill"/>.
        /// </summary>
        protected Zelda.Status.ExtendedStatable Statable
        {
            get 
            { 
                return this.Player.Statable; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Status.AuraList"/> of the <see cref="PlayerEntity"/>
        /// that owns this <see cref="PlayerSkill"/>.
        /// </summary>
        protected Zelda.Status.AuraList AuraList
        {
            get
            { 
                return this.Player.Statable.AuraList;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSkill"/> class.
        /// </summary>
        /// <param name="localizedName">The localized name of the new PlayerSkill.</param>
        /// <param name="cooldown">The cooldown on the PlayerSkill.</param>
        /// <param name="symbol">The symbol displayed for the new PlayerSkill.</param>
        /// <param name="player">The PlayerEntity that owns the new PlayerSkill.</param>
        protected PlayerSkill( string localizedName, Cooldown cooldown, Atom.Xna.Sprite symbol, PlayerEntity player )
            : base( localizedName, cooldown, symbol, player.Statable )
        {
            this.Player = player;
        }

        /// <summary>
        /// Identifies the <see cref="PlayerEntity"/> that owns this <see cref="PlayerSkill"/>.
        /// </summary>
        protected readonly PlayerEntity Player;
    }
}
