// <copyright file="UseSkillAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuickActions.UseSkillAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.QuickActions
{
    using System;
    using Atom;
    using Zelda.Skills;

    /// <summary>
    /// Represents an <see cref="IQuickAction"/> that when executed
    /// invokes a <see cref="Skill"/>.
    /// </summary>
    public sealed class UseSkillAction : IQuickAction
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Skill"/> that is invoked by this UseSkillAction.
        /// </summary>
        public Skill Skill
        {
            get 
            {
                return this.skill;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction is active;
        /// and as such executeable.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !this.skill.IsInactive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction is executeable.
        /// </summary>
        public bool IsExecuteable
        {
            get
            {
                return this.skill.IsUseable;
            }
        }

        /// <summary>
        /// Gets the time left (in seconds) until this IQuickAction can be executed
        /// again.
        /// </summary>
        public float CooldownLeft
        {
            get 
            {
                var cooldown = this.skill.Cooldown;
                if( cooldown == null )
                    return 0.0f;

                return cooldown.TimeLeft;
            }
        }

        /// <summary>
        /// Gets the time (in seconds) this IQuickAction can't be executed again after executing it.
        /// </summary>
        public float CooldownTotal
        {
            get
            {
                var cooldown = this.skill.Cooldown;
                if( cooldown == null )
                    return 0.0f;

                return cooldown.TotalTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IQuickAction executeability is only
        /// limited by the cooldown.
        /// </summary>
        public bool IsOnlyLimitedByCooldown
        {
            get 
            {
                return this.skill.IsOnlyLimitedByCooldown;
            }
        }

        /// <summary>
        /// Gets the symbol associated with this IQuickAction.
        /// </summary>
        public Atom.Xna.ISprite Symbol
        {
            get 
            {
                return this.skill.Symbol;
            }
        }

        /// <summary>
        /// Gets the Color the <see cref="Symbol"/> of this IQuickAction is tinted in.
        /// </summary>
        public Microsoft.Xna.Framework.Color SymbolColor
        {
            get
            {
                return Microsoft.Xna.Framework.Color.White;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the UseSkillAction class.
        /// </summary>
        /// <param name="skill">
        /// The Skill that should be invoked by the new UseSkillAction.
        /// </param>
        public UseSkillAction( Skill skill )
        {
            if( skill == null )
                throw new ArgumentNullException( "skill" );

            this.skill = skill;
        }

        /// <summary>
        /// Initializes a new instance of the UseSkillAction class;
        /// used for deserialization.
        /// </summary>
        public UseSkillAction()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Executes this UseSkillQuickAction.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wants to use this IQuickAction.
        /// </param>
        /// <returns>
        /// Whether this IQuickAction has been executed.
        /// </returns>
        public bool Execute( Zelda.Entities.PlayerEntity user )
        {
            return this.skill.Use();
        }

        /// <summary>
        /// Updates this UseSkillAction.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            // no op.
        }

        /// <summary>
        /// Serializes this IQuickAction using the given BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            string skillTypeName = this.skill.GetType().GetTypeName();
            context.Write( skillTypeName );
        }

        /// <summary>
        /// Deserializes this IQuickAction using the given BinaryReader.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity whose action is executed by this IQuickAction.
        /// </param>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Entities.PlayerEntity player, Zelda.Saving.IZeldaDeserializationContext context )
        {
            string skillTypeName = context.ReadString();
            Type type = Type.GetType( skillTypeName );

            this.skill = player.Skills.Get( type );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The Skill invoked by this UseSkillAction.
        /// </summary>
        private Skill skill;

        #endregion
    }
}