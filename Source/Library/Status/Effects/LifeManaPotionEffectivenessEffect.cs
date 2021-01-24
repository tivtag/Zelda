// <copyright file="LifeManaPotionEffectivenessEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.LifeManaPotionEffectivenessEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{   
    /// <summary>
    /// Defines a <see cref="StatusEffect"/> which manipulates how
    /// well a <see cref="ExtendedStatable"/> ZeldaEntity regenerates Life/Mana.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LifeManaPotionEffectivenessEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this LifeManaPotionEffectivenessEffect.
        /// </summary>
        public const string IdentifierLife = "LifePotEffect",
                            IdentifierMana = "ManaPotEffect";
        
        /// <summary>
        /// Gets an unique string that represents what this LifeManaPotionEffectivenessEffect manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                if( this.PowerType == LifeMana.Life )
                    return IdentifierLife;
                else
                    return IdentifierMana;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what kind of Power the LifeManaPotionEffectivenessEffect manipulates.
        /// </summary>
        public LifeMana PowerType
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
            string powerString = this.PowerType == LifeMana.Life ? 
                Resources.HealingPotionEffectiveness :
                Resources.ManaPotionEffectiveness;

            if( this.Value >= 0.0f )
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.IncXByYPercent,
                    powerString,
                    this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
            else
            {                    
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.DecXByYPercent,
                    powerString,
                    (-this.Value).ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the LifeManaPotionEffectivenessEffect class.
        /// </summary>
        public LifeManaPotionEffectivenessEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LifeManaPotionEffectivenessEffect class.
        /// </summary>
        /// <param name="value">
        /// The value of the new LifeManaPotionEffectivenessEffect.
        /// </param>
        /// <param name="powerType">
        /// The power type the new <see cref="LifeManaRegenEffect"/> modifies.
        /// </param>
        public LifeManaPotionEffectivenessEffect( float value, LifeMana powerType )
           : base( value, StatusManipType.Percental )
        {
            this.PowerType = powerType;
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
            var statable = (ExtendedStatable)user;

            switch( PowerType )
            {
                case LifeMana.Life:
                    statable.Refresh_LifePotionEffectiveness();
                    break;
                case LifeMana.Mana:
                    statable.Refresh_ManaPotionEffectiveness();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            var statable = (ExtendedStatable)user;

            switch( PowerType )
            {
                case LifeMana.Life:
                    statable.Refresh_LifePotionEffectiveness();
                    break;
                case LifeMana.Mana:
                    statable.Refresh_ManaPotionEffectiveness();
                    break;
                default:
                    break;
            }
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

            context.Write( (byte)this.PowerType );
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

            this.PowerType = (LifeMana)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this LifeManaPotionEffectivenessEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new LifeManaPotionEffectivenessEffect();

            this.SetupClone( clone );
            clone.PowerType = this.PowerType;

            return clone;
        }

        #endregion
    }
}