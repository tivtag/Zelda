// <copyright file="DamageSchoolEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageSchoolEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Represents a <see cref="StatusValueEffect"/> that manipulates a status related to <see cref="DamageSchool"/>s.
    /// </summary>
    public abstract class DamageSchoolEffect : StatusValueEffect
    {
        /// <summary>
        /// Gets or sets the <see cref="DamageSchool"/> this <see cref="DamageSchoolEffect"/> manipulates.
        /// </summary>
        public DamageSchool DamageSchool
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a localized string for the <see cref="DamageSchool"/> of this DamageTakenFromSchoolEffect.
        /// </summary>
        /// <returns>
        /// The localized DamageSchool string.
        /// </returns>
        protected string GetLocalizedModifyingString()
        {
            return this.DamageSchool != DamageSchool.All ? LocalizedEnums.Get( this.DamageSchool ) + " " : string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the DamageSchoolEffect class.
        /// </summary>
        protected DamageSchoolEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DamageSchoolEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new DamageSchoolEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> of the new DamageSchoolEffect should be interpreted.
        /// </param>
        /// <param name="damageSchool">
        /// The DamageSchool that is manipulated by the new DamageSchoolEffect.
        /// </param>
        protected DamageSchoolEffect( float value, StatusManipType manipulationType, DamageSchool damageSchool )
            : base( value, manipulationType )
        {
            this.DamageSchool = damageSchool;
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

            context.Write( (byte)this.DamageSchool );
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

            this.DamageSchool = (DamageSchool)context.ReadByte();
        }

        /// <summary>
        /// Setups the given DamageSchoolEffect to be a clone of this DamageSchoolEffect.
        /// </summary>
        /// <param name="clone">
        /// The DamageSchoolEffect to setup as a clone of this DamageSchoolEffect.
        /// </param>
        protected void SetupClone( DamageSchoolEffect clone )
        {
            clone.DamageSchool = this.DamageSchool;

            base.SetupClone( clone );
        }
    }
}
