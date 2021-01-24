// <copyright file="DamageSchoolContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageSchoolContainer class.
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

    // TODO: Add fixed support.

    /// <summary>
    /// Defines a container that contain modifier values for all <see cref="DamageSchool"/>s.
    /// </summary>
    public abstract class DamageSchoolContainer : ISaveable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DamageSchoolContainer class.
        /// </summary>
        protected DamageSchoolContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageSchoolContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this DamageSchoolContainer.
        /// </param>
        internal void Initialize( Statable statable )
        {
            this.auraList = statable.AuraList;
        }

        #endregion

        #region [ Methods ]

        #region > Apply <

        /// <summary>
        /// Applies the damage multiplier for the specified <see cref="DamageSchool"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="school">
        /// The school of damage the damage value is part of.
        /// </param>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int Apply( DamageSchool school, int damage )
        {
            float multiplier = this.Get( school );
            return (int)(damage * multiplier);
        }

        /// <summary>
        /// Applies the fixed damage modifier for the specified <see cref="DamageSchool"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="school">
        /// The school of damage the damage value is part of.
        /// </param>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        internal int ApplyFixed( DamageSchool school, int damage )
        {
            return damage + this.GetFixed( school );
        }

        /// <summary>
        /// Applies the multiplier for <see cref="DamageSchool.Physical"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSchool ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyPhysical( int damage )
        {
            return (int)(damage * this.physical);
        }

        /// <summary>
        /// Applies the multiplier for <see cref="DamageSchool.Magical"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSchool ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyMagical( int damage )
        {
            return (int)(damage * this.magical);
        }

        /// <summary>
        /// Applies the fixed damage modifier for Physical damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        public int ApplyFixedPhysical( int damage )
        {
            // return damage + this.fixedPhysical;
            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifier for Magical damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        public int ApplyFixedMagical( int damage )
        {
            // return damage + this.fixedMagical;
            return damage;
        }
        
        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the value of the specified DamageSchool modifier value.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        internal void Refresh( DamageSchool school )
        {
            switch( school )
            {
                case DamageSchool.All:
                    this.RefreshActual( DamageSchool.Physical );
                    this.RefreshActual( DamageSchool.Magical );
                    break;

                case DamageSchool.None:
                    break;

                default:
                    this.RefreshActual( school );
                    break;
            }
        }

        /// <summary>
        /// Refreshes the value of the specified DamageSchool modifier value.
        /// </summary>
        /// <param name="school">
        /// The school of damage. Must be an actual DamageSchool, no generalization.
        /// </param>
        private void RefreshActual( DamageSchool school )
        {
            Debug.Assert( school != DamageSchool.All );
            Debug.Assert( school != DamageSchool.None );

            float effectValue = this.auraList.GetPercentalEffectValue(
                this.GetEffectIdentifier( school ),
                this.GetEffectIdentifier( DamageSchool.All )
            );

            float baseValue = this.GetBase( school );
            float finalValue = baseValue + effectValue - 1.0f;

            this.Set( school, finalValue );
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given DamageSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected abstract string GetEffectIdentifier( DamageSchool school );

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the final damage multiplier value for the given DamageSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given DamageSchool should be multiplied by.
        /// </returns>
        public float Get( DamageSchool school )
        {
            switch( school )
            {
                case DamageSchool.Physical:
                    return this.physical;

                case DamageSchool.Magical:
                    return this.magical;

                case DamageSchool.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Gets the base damage multiplier value for the given DamageSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given DamageSchool should be multiplied by.
        /// </returns>
        public float GetBase( DamageSchool school )
        {
            switch( school )
            {
                case DamageSchool.Physical:
                    return this.basePhysical;

                case DamageSchool.Magical:
                    return this.baseMagical;

                case DamageSchool.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Gets the fixed damage modifier value for the given DamageSchool.
        /// </summary>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <returns>
        /// The value that should be added to a damage value of the specified DamageSchool.
        /// </returns>
        public int GetFixed( DamageSchool school )
        {
            return 0;
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the final damage multiplier value for the given DamageSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <param name="value">
        /// The value a damage value of the given DamageSchool should be multiplied by.
        /// </param>
        private void Set( DamageSchool school, float value )
        {
            switch( school )
            {
                case DamageSchool.Physical:
                    this.physical = value;
                    break;

                case DamageSchool.Magical:
                    this.magical = value;
                    break;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "school" );
            }
        }

        /// <summary>
        /// Sets the base damage multiplier value for the given DamageSchool.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="school">
        /// The school of damage.
        /// </param>
        /// <param name="value">
        /// The value a damage value of the given DamageSchool should be multiplied by.
        /// </param>
        public void SetBase( DamageSchool school, float value )
        {
            switch( school )
            {
                case DamageSchool.Physical:
                    this.basePhysical = value;
                    break;

                case DamageSchool.Magical:
                    this.baseMagical = value;
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

            context.Write( this.basePhysical );
            context.Write( this.baseMagical );
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

            this.basePhysical = context.ReadSingle();
            this.baseMagical = context.ReadSingle();

            this.SetFinalToBase();
        }

        /// <summary>
        /// Sets the chance to resist to be the base chance to resist.
        /// </summary>
        internal void SetFinalToBase()
        {
            this.physical = this.basePhysical;
            this.magical = this.baseMagical;
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given DamageSchoolContainer to be a clone of this DamageSchoolContainer.
        /// </summary>
        /// <param name="clone">
        /// The DamageSchoolContainer to setup as a clone of this DamageSchoolContainer.
        /// </param>
        public void SetupClone( DamageSchoolContainer clone )
        {
            clone.physical = this.physical;
            clone.magical = this.magical;

            clone.basePhysical = this.basePhysical;
            clone.baseMagical = this.baseMagical;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the base multiplier for physical and magical damage.
        /// </summary>
        private float basePhysical = 1.0f, baseMagical = 1.0f;

        /// <summary>
        /// Stores the final multiplier for physical and magical damage.
        /// </summary>
        private float physical = 1.0f, magical = 1.0f;

        /// <summary>
        /// Identifies the AuraList of the statable entity that owns this DamageSchoolContainer.
        /// </summary>
        private AuraList auraList;

        #endregion
    }
}