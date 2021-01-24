// <copyright file="DamageSourceEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageSourceEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    /// <summary>
    /// Represents a <see cref="StatusValueEffect"/> that manipulates a status related to <see cref="DamageSource"/>s.
    /// </summary>
    public abstract class DamageSourceEffect : StatusValueEffect
    {
        /// <summary>
        /// Gets or sets the <see cref="DamageSource"/> this <see cref="DamageSourceEffect"/> manipulates.
        /// </summary>
        public DamageSource DamageSource
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the DamageSourceEffect class.
        /// </summary>
        protected DamageSourceEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DamageSourceEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new DamageSourceEffect.
        /// </param>
        /// <param name="manipulationType">
        /// The manipulation type.
        /// </param>
        /// <param name="damageSource">
        /// The property that is manipulated.
        /// </param>
        protected DamageSourceEffect( float value, StatusManipType manipulationType, DamageSource damageSource )
            : base( value, manipulationType )
        {
            this.DamageSource = damageSource;
        }

        /// <summary>
        /// Gets a localized string for the <see cref="DamageSource"/> of this DamageSourceEffect.
        /// </summary>
        /// <returns>
        /// A localized string.
        /// </returns>
        protected string GetLocalizedDamageSourceString()
        {
            return this.DamageSource != DamageSource.All ? LocalizedEnums.Get( this.DamageSource ) + " " : string.Empty;
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

            context.Write( (byte)this.DamageSource );
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

            this.DamageSource = (DamageSource)context.ReadByte();
        }

        /// <summary>
        /// Setups the given DamageSourceEffect to be a clone of this DamageSourceEffect.
        /// </summary>
        /// <param name="clone">
        /// The DamageSourceEffect to setup as a clone of this DamageSourceEffect.
        /// </param>
        protected void SetupClone( DamageSourceEffect clone )
        {
            clone.DamageSource = this.DamageSource;

            base.SetupClone( clone );
        }
    }
}
