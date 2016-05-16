// <copyright file="DamageDoneAgainstRaceEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageDoneAgainstRaceEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage
{
    using System;

    /// <summary>
    /// Defines a StatusEffect that manipulates the damage done against a specified Race.
    /// </summary>
    public sealed class DamageDoneAgainstRaceEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary>
        /// Identifies the unique manipulation identifiers of the DamageDoneAgainstRaceEffect.
        /// </summary>
        private const string 
            IdentifierUndead    = "DmgDone_Undead",
            IdentifierSlime     = "DmgDone_Slime",
            IdentifierPlant     = "DmgDone_Plant",
            IdentifierMachine   = "DmgDone_Machine",
            IdentifierHuman     = "DmgDone_Human",
            IdentifierFairy     = "DmgDone_Fairy",
            IdentifierDemiHuman = "DmgDone_DemiHuman",
            IdentifierDemiBeast = "DmgDone_DemiBeast",
            IdentifierBeast     = "DmgDone_Beast",
            IdentifierDemon     = "DmgDone_Demon";
                
        /// <summary>
        /// Gets the (unique) identifier string that is associated with
        /// the DamageDoneAgainstRaceEffect of the specified <see cref="RaceType"/>.
        /// </summary>
        /// <param name="race">
        /// The race whose effect identifier should be returned.
        /// </param>
        /// <returns>
        /// An (unique) identifier string.
        /// </returns>
        internal static string GetIdentifier( RaceType race )
        {
            switch( race )
            {
                case RaceType.Beast:
                    return IdentifierBeast;

                case RaceType.DemiBeast:
                    return IdentifierDemiBeast;
                    
                case RaceType.DemiHuman:
                    return IdentifierDemiHuman;

                case RaceType.Demon:
                    return IdentifierDemon;

                case RaceType.Fairy:
                    return IdentifierFairy;

                case RaceType.Human:
                    return IdentifierHuman;

                case RaceType.Machine:
                    return IdentifierMachine;

                case RaceType.Plant:
                    return IdentifierPlant;

                case RaceType.Slime:
                    return IdentifierSlime;

                case RaceType.Undead:
                    return IdentifierUndead;

                default:
                    throw new NotImplementedException( race.ToString() );
            }
        }

        /// <summary> 
        /// Gets a string that uniquely identifies this <see cref="DamageDoneAgainstRaceEffect"/>.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return GetIdentifier( this.Race );
            }
        }

        /// <summary>
        /// Cached description strings.
        /// </summary>
        private static readonly string
            DescStringPositive = Resources.ED_DmgDoneRaceMod_Positive,
            DescStringNegative = Resources.ED_DmgDoneRaceMod_Negative;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value that indicates what kind of race this <see cref="DamageDoneAgainstRaceEffect"/>
        /// modifies the damage done for.
        /// </summary>
        public RaceType Race
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
            if( this.Value >= 0.0f )
            {
                string valueString = this.Value.ToString( System.Globalization.CultureInfo.CurrentCulture );                                        
                if( this.ManipulationType == StatusManipType.Percental )
                    valueString += "%";

                return string.Format( 
                    System.Globalization.CultureInfo.CurrentCulture,
                    DescStringPositive,
                    LocalizedEnums.GetPlural( this.Race ),
                    valueString
                );
            }
            else
            {
                string valueString = (-this.Value).ToString( System.Globalization.CultureInfo.CurrentCulture );                                        
                if( this.ManipulationType == StatusManipType.Percental )
                    valueString += "%";

                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    DescStringNegative,
                    LocalizedEnums.GetPlural( this.Race ),
                    valueString
                );
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageDoneAgainstRaceEffect"/> class.
        /// </summary>
        public DamageDoneAgainstRaceEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageDoneAgainstRaceEffect"/> class.
        /// </summary>
        /// <param name="race">
        /// States what kind of race the new DamageDoneRaceModEffect modifies damage done for.
        /// </param>
        /// <param name="value">
        /// The manipulation value of the new DamageDoneRaceModEffect.
        /// </param>
        public DamageDoneAgainstRaceEffect( RaceType race, float value )
            : base( value, StatusManipType.Fixed )
        {
            this.Race = race;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="DamageDoneAgainstRaceEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="DamageDoneAgainstRaceEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneAgainstRaceEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="DamageDoneAgainstRaceEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            this.OnChanged( (ExtendedStatable)user );
        }

        /// <summary>
        /// Called when this <see cref="DamageDoneAgainstRaceEffect"/> gets enabled or disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( ExtendedStatable user )
        {
            user.DamageDone.AgainstRace.Refresh( this.Race );
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
            context.Write( (byte)this.Race );
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
            this.Race = (RaceType)context.ReadByte();
        }

        /// <summary>
        /// Returns a clone of this DamageDoneAgainstRaceEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new DamageDoneAgainstRaceEffect() { Race = this.Race };

            this.SetupClone( clone );

            return clone;
        }

        #endregion
    }
}
