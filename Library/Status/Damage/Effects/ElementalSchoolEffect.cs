// <copyright file="ElementalSchoolEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.ElementalSchoolEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Represents a <see cref="StatusValueEffect"/> that manipulates a status related to the <see cref="ElementalSchool"/>s.
    /// </summary>
    public abstract class ElementalSchoolEffect : StatusValueEffect
    {
        /// <summary>
        /// Gets or sets the <see cref="ElementalSchool"/> this <see cref="ElementalSchoolEffect"/> manipulates.
        /// </summary>
        public ElementalSchool ElementalSchool
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a localized string for the <see cref="ElementalSchool"/> of this ElementalDamageDoneEffect.
        /// </summary>
        /// <returns>
        /// A localized string that is displayed to the player.
        /// </returns>
        protected string GetLocalizedModifyingString()
        {
            var school = this.ElementalSchool != ElementalSchool.All ? 
                LocalizedEnums.Get( this.ElementalSchool ) : 
                Resources.Magical;

            return school + " ";
        }
                
        /// <summary>
        /// Initializes a new instance of the ElementalSchoolEffect class.
        /// </summary>
        protected ElementalSchoolEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementalSchoolEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new ElementalSchoolEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> of the new ElementalSchoolEffect should be interpreted.
        /// </param>
        /// <param name="elementalSchool">
        /// The ElementalSchool that is manipulated by the new ElementalSchoolEffect.
        /// </param>
        protected ElementalSchoolEffect( float value, StatusManipType manipulationType, ElementalSchool elementalSchool )
            : base( value, manipulationType )
        {
            this.ElementalSchool = elementalSchool;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            context.Write( (byte)this.ElementalSchool );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            this.ElementalSchool = (ElementalSchool)context.ReadByte();
        }

        /// <summary>
        /// Setups the given ElementalSchoolEffect to be a clone of this ElementalSchoolEffect.
        /// </summary>
        /// <param name="clone">
        /// The ElementalSchoolEffect to setup as a clone of this ElementalSchoolEffect.
        /// </param>
        protected void SetupClone( ElementalSchoolEffect clone )
        {
            clone.ElementalSchool = this.ElementalSchool;

            base.SetupClone( clone );
        }
    }
}