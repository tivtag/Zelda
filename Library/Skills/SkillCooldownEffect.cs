// <copyright file="SkillCooldownEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Skills.SkillCooldownEffect class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Skills
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// Defines an effect that modifies the cooldown of an <see cref="ICooldownDependant"/>
    /// object.
    /// </summary>
    internal sealed class SkillCooldownEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// Gets the identifier for this SkillCooldownEffect.
        /// </summary>
        /// <typeparam name="TSkill">
        /// The type of the skill.
        /// </typeparam>
        /// <returns>
        /// The string that uniquely identifies this CooldownEffect.
        /// </returns>
        public static string GetIdentifier<TSkill>()
            where TSkill : Skill, ICooldownDependant
        {
            return GetIdentifier( typeof( TSkill ) );
        }

        /// <summary>
        /// Gets the identifier for this SkillCooldownEffect.
        /// </summary>
        /// <param name="skillType">
        /// The type of the Skill.
        /// </param>
        /// <returns>
        /// The string that uniquely identifies this CooldownEffect.
        /// </returns>
        private static string GetIdentifier( Type skillType )
        {
            return skillType.Name + "_Cooldown";
        }

        /// <summary> 
        /// Gets an unique string that represents what this SkillCooldownEffect manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <param name="statable">
        /// This paramater is unused.
        /// </param>
        /// <returns>
        /// This method will never return.
        /// </returns>
        /// <exception cref="NotSupportedException"/>
        public override string GetDescription( Statable statable )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SkillCooldownEffect class.
        /// </summary>
        /// <param name="manipulationType">
        /// States how the value of the new SkillCooldownEffect should be interpreted.
        /// </param>
        /// <param name="skillTalent">
        /// The SkillTalent whose skill's cooldown is modified by this CooldownEffect.
        /// </param>
        public SkillCooldownEffect( StatusManipType manipulationType, Zelda.Talents.SkillTalent skillTalent )
            : base( 0.0f, manipulationType )
        {
            var skillType = skillTalent.SkillType;

            if( !typeof( ICooldownDependant ).IsAssignableFrom( skillType ) )
            {
                throw new ArgumentException(
                    "The skill created by the specified SkillTalent is not ICooldownDependant.",
                    "skillTalent"
                );
            }

            this.identifier = GetIdentifier( skillType );
            this.skillProvider = skillTalent;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged();
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged();
        }

        /// <summary>
        /// Called when this StatusEffect has bene enabled or disabled.
        /// </summary>
        private void OnChanged()
        {
            var skill = this.skillProvider.Skill;

            if( skill != null )
            {
                ((ICooldownDependant)skill).RefreshCooldown();
            }
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <returns>
        /// This method will never return.
        /// </returns>
        /// <exception cref="NotSupportedException"/>
        public override StatusEffect Clone()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <param name="context">This paramater is unused.</param>
        /// <exception cref="NotSupportedException"/>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <param name="context">This paramater is unused.</param>
        /// <exception cref="NotSupportedException"/>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The object that provides the Skill whose cooldown is affected
        /// by this SkillCooldownEffect.
        /// </summary>
        private readonly ISkillProvider skillProvider;

        /// <summary>
        /// The cached identifier for this SkillCooldownEffect.
        /// </summary>
        private readonly string identifier;

        #endregion
    }
}
