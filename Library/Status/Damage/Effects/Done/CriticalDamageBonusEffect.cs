// <copyright file="CriticalDamageBonusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.CriticalDamageBonusEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates how much damage
    /// the <see cref="Statable"/> ZeldaEntity does with a specific type of attack.
    /// This class can't be inherited.
    /// </summary>
    public sealed class CriticalDamageBonusEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation identifiers of the CriticalDamageBonusEffect.
        /// </summary>
        private const string 
            IdentifierMelee  = "CritDmg_M",
            IdentifierRanged = "CritDmg_R",
            IdentifierSpell  = "CritDmg_S",
            IdentifierAll    = "CritDmg_All";

        /// <summary>
        /// Gets the (unique) identifier string that is associated with
        /// the CriticalDamageBonusEffect of the specified <see cref="DamageSource"/>.
        /// </summary>
        /// <param name="source">
        /// The DamageSource whose effect identifier should be returned.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        public static string GetIdentifier( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return IdentifierMelee;

                case DamageSource.Ranged:
                    return IdentifierRanged;

                case DamageSource.Spell:
                    return IdentifierSpell;

                case DamageSource.All:
                    return IdentifierAll;

                case DamageSource.None:
                    return string.Empty;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary> 
        /// Gets a string that uniquely identifies this <see cref="DamageDoneAgainstRaceEffect"/>.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.DamageSource );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what kind of Critical Damage Bonus this <see cref="CriticalDamageBonusEffect"/> modifies.
        /// </summary>
        public DamageSource DamageSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            var sb = new System.Text.StringBuilder( 25 );

            if( this.Value >= 0.0f )
            {
                sb.Append( '+' );
            }

            sb.Append( this.Value.ToString( CultureInfo.CurrentCulture ) );
            sb.Append( ' ' );
            sb.Append( LocalizedEnums.GetCriticalDamageBonus( this.DamageSource ) );

            if( this.ManipulationType == StatusManipType.Rating )
            {
                var value = System.Math.Round( StatusCalc.ConvertRating( this.Value, statable.Level ), 2 );

                sb.Append( ' ' );
                sb.Append( Resources.Rating );
                sb.Append( " (" );
                sb.Append( value );
                sb.Append( "%)" );
            }
            else
            {
                throw new NotSupportedException();
            }

            return sb.ToString();
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalDamageBonusEffect"/> class.
        /// </summary>
        public CriticalDamageBonusEffect()
            : this( 0.0f, StatusManipType.Percental, DamageSource.All )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalDamageBonusEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new CriticalDamageBonusEffect.
        /// </param>
        /// <param name="manipulationType">
        /// States how the <paramref name="value"/> of the new CriticalDamageBonusEffect should be interpreted.
        /// </param>
        /// <param name="source">
        /// States what kind of Critical Damage Bonus Modifier(s) the new CriticalDamageBonusEffect affects.
        /// </param>
        public CriticalDamageBonusEffect( float value, StatusManipType manipulationType, DamageSource source )
            : base( value, manipulationType )
        {
            this.DamageSource = source;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="CriticalDamageBonusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="CriticalDamageBonusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="CriticalDamageBonusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="CriticalDamageBonusEffect"/> gets enabled or disabled for the given Statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( Statable user )
        {
            user.DamageDone.WithCritical.Refresh( this.DamageSource );
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

            // Write Data:
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

            // Read Data:
            this.DamageSource = (DamageSource)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this CriticalDamageBonusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new CriticalDamageBonusEffect() { DamageSource = this.DamageSource };

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
