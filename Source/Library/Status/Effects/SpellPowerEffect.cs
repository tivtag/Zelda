// <copyright file="SpellPowerEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.SpellPowerEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System.Globalization;

    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates the power of a spell school
    /// of a <see cref="ExtendedStatable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class SpellPowerEffect : StatusValueEffect
    {       
        #region [ Identification ]

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of all spells.
        /// </summary>
        private const string IdentifierAll = "SP_All";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of Fire spells.
        /// </summary>
        private const string IdentifierFire = "SP_Fire";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of Water spells.
        /// </summary>
        private const string IdentifierWater = "SP_Water";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of Nature spells.
        /// </summary>
        private const string IdentifierNature = "SP_Nature";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of Shadow spells.
        /// </summary>
        private const string IdentifierShadow = "SP_Shadow";

        /// <summary>
        /// The manipulation name that uniquely identifies the StatusEffects that manipulate the power of Light spells.
        /// </summary>
        private const string IdentifierLight = "SP_Light";
        
        /// <summary>
        /// Gets the manipulation string used by the SpellPowerEffect for the given ElementSchool.
        /// </summary>
        /// <param name="school">
        /// The element school.
        /// </param>
        /// <returns>
        /// An unique string.
        /// </returns>
        public static string GetManipulationString( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.All:
                    return IdentifierAll;

                case ElementalSchool.Fire:
                    return IdentifierFire;

                case ElementalSchool.Water:
                    return IdentifierWater;

                case ElementalSchool.Nature:
                    return IdentifierNature;

                case ElementalSchool.Light:
                    return IdentifierLight;

                case ElementalSchool.Shadow:
                    return IdentifierShadow;

                default:
                    return string.Empty;
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetManipulationString( this.SpellSchool );
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what kind of spell power is manipulated by this <see cref="SpellPowerEffect"/>.
        /// </summary>
        public ElementalSchool SpellSchool
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
            string formatString = this.ManipulationType == StatusManipType.Fixed ?
                Resources.IncXByY : Resources.IncXByYPercent;
            
            return string.Format(
                CultureInfo.CurrentCulture,
                formatString,
                this.GetDefineDescription(),
                this.Value.ToString( CultureInfo.CurrentCulture )
            );
        }

        /// <summary>
        /// Gets a localized string the defines what this Spell Power Effect increases.
        /// </summary>
        /// <returns>
        /// A localiued string for the current SpellSchool.
        /// </returns>
        private string GetDefineDescription()
        {
            if( this.SpellSchool == ElementalSchool.All )
            {
                return Resources.SpellPower;
            }
            else
            {
                return LocalizedEnums.Get( this.SpellSchool ) + ' ' + Resources.SpellPower;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellPowerEffect"/> class.
        /// </summary>
        public SpellPowerEffect()
            : base()
        {
            this.SpellSchool      = ElementalSchool.All;
            this.ManipulationType = StatusManipType.Fixed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellPowerEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the new SpellPowerEffect.
        /// </param>
        /// <param name="manipulationType">
        /// The manipulation type of value of the new SpellPowerEffect.
        /// </param>
        /// <param name="spellSchool">
        /// The ElementalSchool the new SpellPowerEffect should affect.
        /// </param>
        public SpellPowerEffect( float value, StatusManipType manipulationType, ElementalSchool spellSchool )
            : base( value, manipulationType )
        {
            this.SpellSchool = spellSchool;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="SpellPowerEffect"/> gets enabled for the given extended-sstatable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnStateChange( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="SpellPowerEffect"/> gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnStateChange( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when the Enabled/Disabled state of this SpellPowerEffect has changed for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The extended-statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        private void OnStateChange( ExtendedStatable user )
        {
            user.SpellPower.RefreshExtra( this.SpellSchool );
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
            context.Write( (byte)this.SpellSchool );
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
            this.SpellSchool = (ElementalSchool)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this SpellPowerEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new SpellPowerEffect() {
                SpellSchool = this.SpellSchool
            };

            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
