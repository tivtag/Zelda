// <copyright file="ElementalSchoolContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.ElementalSchoolContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    using System;
    using System.Diagnostics;
    using Zelda.Saving;

    // TODO: change SetFinalToBase to private
    // once migration to new system has been
    // completed.

    /// <summary>
    /// Defines a container that contain modifier values for all <see cref="ElementalSchool"/>s.
    /// </summary>
    public abstract class ElementalSchoolContainer : ISaveable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ElementalSchoolContainer class.
        /// </summary>
        protected ElementalSchoolContainer()
        {
        }

        /// <summary>
        /// Initializes this ElementalSchoolContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this ElementalSchoolContainer.
        /// </param>
        internal void Initialize( Statable statable )
        {
            this.auraList = statable.AuraList;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Applies this multiplier for the specified <see cref="ElementalSchool"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="element">
        /// The element of the damage.
        /// </param>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int Apply( ElementalSchool element, int damage )
        {
            float multiplier = this.Get( element );
            return (int)(damage * multiplier);
        }

        /// <summary>
        /// Overriden to return a string representation of this ElementalSchoolContainer.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            sb.AppendFormat(
                culture,
                "Fire={0} Water={1} Nature={2} Light={3} Shadow={4}", 
                this.fire.ToString( culture ),
                this.water.ToString( culture ),
                this.nature.ToString( culture ),
                this.light.ToString( culture ),
                this.shadow.ToString( culture )
            );

            return sb.ToString();
        }

        #region > Refresh <

        /// <summary>
        /// Refreshes the value of the specified ElementalSchool modifier value.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        internal void Refresh( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.All:
                    this.RefreshActual( ElementalSchool.Fire );
                    this.RefreshActual( ElementalSchool.Water );
                    this.RefreshActual( ElementalSchool.Light );
                    this.RefreshActual( ElementalSchool.Shadow );
                    this.RefreshActual( ElementalSchool.Nature );
                    break;

                case ElementalSchool.None:
                    break;

                default:
                    this.RefreshActual( school );
                    break;
            }
        }

        /// <summary>
        /// Refreshes the value of the specified ElementalSchool modifier value.
        /// </summary>
        /// <param name="school">
        /// The school of damage. Must be an actual ElementalSchool, no generalization.
        /// </param>
        private void RefreshActual( ElementalSchool school )
        {
            Debug.Assert( school != ElementalSchool.All );
            Debug.Assert( school != ElementalSchool.None );

            float effectValue = this.auraList.GetPercentalEffectValue(
                this.GetEffectIdentifier( school ),
                this.GetEffectIdentifier( ElementalSchool.All )
            );

            float baseValue = this.GetBase( school );
            float finalValue = baseValue + effectValue - 1.0f;

            this.Set( school, finalValue );
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected abstract string GetEffectIdentifier( ElementalSchool school );

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the final damage multiplier value for the given ElementalSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given ElementalSchool should be multiplied by.
        /// </returns>
        public float Get( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return this.fire;

                case ElementalSchool.Water:
                    return this.water;

                case ElementalSchool.Light:
                    return this.light;

                case ElementalSchool.Shadow:
                    return this.shadow;

                case ElementalSchool.Nature:
                    return this.nature;

                case ElementalSchool.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Gets the base damage multiplier value for the given ElementalSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given ElementalSchool should be multiplied by.
        /// </returns>
        public float GetBase( ElementalSchool school )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    return this.baseFire;

                case ElementalSchool.Water:
                    return this.baseWater;

                case ElementalSchool.Light:
                    return this.baseLight;

                case ElementalSchool.Shadow:
                    return this.baseShadow;

                case ElementalSchool.Nature:
                    return this.baseNature;

                case ElementalSchool.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the final damage multiplier value for the given ElementalSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <param name="value">
        /// The value a damage value of the given ElementalSchool should be multiplied by.
        /// </param>
        private void Set( ElementalSchool school, float value )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    this.fire = value;
                    break;

                case ElementalSchool.Water:
                    this.water = value;
                    break;

                case ElementalSchool.Light:
                    this.light = value;
                    break;

                case ElementalSchool.Shadow:
                    this.shadow = value;
                    break;

                case ElementalSchool.Nature:
                    this.nature = value;
                    break;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Sets the base damage multiplier value for the given ElementalSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <param name="value">
        /// The value a damage value of the given ElementalSchool should be multiplied by.
        /// </param>
        public void SetBase( ElementalSchool school, float value )
        {
            switch( school )
            {
                case ElementalSchool.Fire:
                    this.baseFire = value;
                    break;

                case ElementalSchool.Water:
                    this.baseWater = value;
                    break;

                case ElementalSchool.Light:
                    this.baseLight = value;
                    break;

                case ElementalSchool.Shadow:
                    this.baseShadow = value;
                    break;

                case ElementalSchool.Nature:
                    this.baseNature = value;
                    break;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        #endregion

        #region > Serialization <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.baseFire );
            context.Write( this.baseWater );
            context.Write( this.baseShadow );
            context.Write( this.baseLight );
            context.Write( this.baseNature );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.baseFire = context.ReadSingle();
            this.baseWater = context.ReadSingle();
            this.baseShadow = context.ReadSingle();
            this.baseLight = context.ReadSingle();
            this.baseNature = context.ReadSingle();

            this.SetFinalToBase();
        }

        /// <summary>
        /// Sets the final values to the base values.
        /// </summary>
        internal void SetFinalToBase()
        {
            this.fire = this.baseFire;
            this.water = this.baseWater;
            this.light = this.baseLight;
            this.shadow = this.baseShadow;
            this.nature = this.baseNature;
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given ElementalSchoolContainer to be a clone of this ElementalSchoolContainer.
        /// </summary>
        /// <param name="clone">
        /// The ElementalSchoolContainer to setup as a clone of this ElementalSchoolContainer.
        /// </param>
        public void SetupClone( ElementalSchoolContainer clone )
        {
            clone.fire = this.fire;
            clone.water = this.water;
            clone.light = this.light;
            clone.shadow = this.shadow;
            clone.nature = this.nature;

            clone.baseFire = this.baseFire;
            clone.baseWater = this.baseWater;
            clone.baseLight = this.baseLight;
            clone.baseShadow = this.baseShadow;
            clone.baseNature = this.baseNature;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the base multiplier values for elemental damage.
        /// </summary>
        private float baseFire = 1.0f, baseWater = 1.0f, baseLight = 1.0f, baseShadow = 1.0f, baseNature = 1.0f; 

        /// <summary>
        /// Stores the final multiplier values for elemental damage.
        /// </summary>
        private float fire = 1.0f, water = 1.0f, light = 1.0f, shadow = 1.0f, nature = 1.0f;

        /// <summary>
        /// Identifies the AuraList of the statable entity that owns this ElementalSchoolContainer.
        /// </summary>
        private AuraList auraList;

        #endregion
    }
}