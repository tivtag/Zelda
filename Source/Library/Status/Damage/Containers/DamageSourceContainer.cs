// <copyright file="DamageSourceContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageSourceContainer class.
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
    /// Defines a container that contain modifier values for all <see cref="DamageSource"/>s.
    /// </summary>
    public abstract class DamageSourceContainer : ISaveable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DamageSourceContainer class.
        /// </summary>
        protected DamageSourceContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageSourceContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this DamageSourceContainer.
        /// </param>
        internal void Initialize( Statable statable )
        {
            this.auraList = statable.AuraList;
        }

        #endregion

        #region [ Methods ]

        #region > Apply <

        /// <summary>
        /// Applies the multiplier for the specified <see cref="DamageSource"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="source">
        /// The source of the damage.
        /// </param>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        internal int Apply( DamageSource source, int damage )
        {
            return (int)(damage * this.Get( source ));
        }

        /// <summary>
        /// Applies the fixed modifier for the specified <see cref="DamageSource"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <param name="source">
        /// The source of the damage.
        /// </param>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        internal int ApplyFixed( DamageSource source, int damage )
        {
            return damage + this.GetFixed( source );
        }

        /// <summary>
        /// Applies the multiplier for <see cref="DamageSource.Melee"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyMelee( int damage )
        {
            return (int)(damage * this.melee);
        }

        /// <summary>
        /// Applies the multiplier for <see cref="DamageSource.Ranged"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyRanged( int damage )
        {
            return (int)(damage * this.ranged);
        }

        /// <summary>
        /// Applies the multiplier for <see cref="DamageSource.Spell"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplySpell( int damage )
        {
            return (int)(damage * this.spell);
        }

        /// <summary>
        /// Applies the fixed modifier value for <see cref="DamageSource.Melee"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyFixedMelee( int damage )
        {
            return damage + this.fixedMelee;
        }

        /// <summary>
        /// Applies the fixed modifier value for <see cref="DamageSource.Ranged"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyFixedRanged( int damage )
        {
            return damage + this.fixedRanged;
        }

        /// <summary>
        /// Applies the fixed modifier value for <see cref="DamageSource.Spell"/>
        /// to the specified <paramref name="damage"/> value.
        /// </summary>
        /// <remarks>
        /// This shortcut method for Apply( Int32, DamageSource ) has been
        /// defined for perfomance reasons.
        /// </remarks>
        /// <param name="damage">
        /// The input damage value.
        /// </param>
        /// <returns>
        /// The output damage value.
        /// </returns>
        public int ApplyFixedSpell( int damage )
        {
            return damage + this.fixedSpell;
        }

        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the value of the specified DamageSource modifier value.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        internal void Refresh( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.All:
                    this.RefreshActual( DamageSource.Melee );
                    this.RefreshActual( DamageSource.Ranged );
                    this.RefreshActual( DamageSource.Spell );
                    break;

                case DamageSource.None:
                    break;

                default:
                    this.RefreshActual( source );
                    break;
            }
        }

        /// <summary>
        /// Refreshes the value of the specified DamageSource modifier value.
        /// </summary>
        /// <param name="source">
        /// The source of damage. Must be an actual DamageSource, no generalization.
        /// </param>
        private void RefreshActual( DamageSource source )
        {
            Debug.Assert( source != DamageSource.All );
            Debug.Assert( source != DamageSource.None );

            float percentValue;
            float fixedValue;

            this.auraList.GetEffectValues(
                this.GetEffectIdentifier( source ),
                this.GetEffectIdentifier( DamageSource.All ),
                out fixedValue,
                out percentValue
            );

            float baseValue = this.GetBase( source );
            float finalMultiplier = baseValue + percentValue - 1.0f;

            this.Set( source, (int)fixedValue, finalMultiplier );
        }

        /// <summary>
        /// Gets the StatusEffect identifier string that is associated with
        /// the given DamageSource.
        /// </summary>
        /// <param name="source">
        /// The source of damage.
        /// </param>
        /// <returns>
        /// An unique string identifier.
        /// </returns>
        protected abstract string GetEffectIdentifier( DamageSource source );

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the final damage multiplier value for the given DamageSource.
        /// </summary>
        /// <param name="source">
        /// The Source of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given DamageSource should be multiplied by.
        /// </returns>
        public float Get( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return this.melee;

                case DamageSource.Ranged:
                    return this.ranged;

                case DamageSource.Spell:
                    return this.spell;

                case DamageSource.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "source" );
            }
        }

        /// <summary>
        /// Gets the base damage multiplier value for the given DamageSource.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="source">
        /// The Source of damage.
        /// </param>
        /// <returns>
        /// The value a damage value of the given DamageSource should be multiplied by.
        /// </returns>
        public float GetBase( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return this.baseMelee;

                case DamageSource.Ranged:
                    return this.baseRanged;

                case DamageSource.Spell:
                    return this.baseSpell;

                case DamageSource.None:
                    return 1.0f;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "source" );
            }
        }

        /// <summary>
        /// Gets the fixed damage modifier value for the given DamageSource.
        /// </summary>
        /// <param name="source">
        /// The Source of damage.
        /// </param>
        /// <returns>
        /// The value that should be added to the  damage value of the specified DamageSource.
        /// </returns>
        public int GetFixed( DamageSource source )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    return this.fixedMelee;

                case DamageSource.Ranged:
                    return this.fixedRanged;

                case DamageSource.Spell:
                    return this.fixedSpell;

                case DamageSource.None:
                    return 0;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "source" );
            }
        }

        #endregion

        #region > Set <

        /// <summary>
        /// Sets the final damage multiplier value for the given DamageSource.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="source">
        /// The Source of damage.
        /// </param>
        /// <param name="fixedValue">
        /// The value that should be added to a damage value of the given DamageSource.
        /// </param>
        /// <param name="multiplierValue">
        /// The value a damage value of the given DamageSource should be multiplied by.
        /// </param>
        private void Set( DamageSource source, int fixedValue, float multiplierValue )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    this.fixedMelee = fixedValue;
                    this.melee = multiplierValue;
                    break;

                case DamageSource.Ranged:
                    this.fixedRanged = fixedValue;
                    this.ranged = multiplierValue;
                    break;

                case DamageSource.Spell:
                    this.fixedSpell = fixedValue;
                    this.spell = multiplierValue;
                    break;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "source" );
            }
        }

        /// <summary>
        /// Sets the base damage multiplier value for the given DamageSource.
        /// </summary>
        /// <remarks>
        /// The base value before any StatusEffects are applied.
        /// </remarks>
        /// <param name="source">
        /// The Source of damage.
        /// </param>
        /// <param name="value">
        /// The value a damage value of the given DamageSource should be multiplied by.
        /// </param>
        public void SetBase( DamageSource source, float value )
        {
            switch( source )
            {
                case DamageSource.Melee:
                    this.baseMelee = value;
                    break;

                case DamageSource.Ranged:
                    this.baseRanged = value;
                    break;

                case DamageSource.Spell:
                    this.baseSpell = value;
                    break;

                default:
                    throw new ArgumentException( Resources.EnumValueIsNotSupported, "source" );
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

            context.Write( this.baseMelee );
            context.Write( this.baseRanged );
            context.Write( this.baseSpell );
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

            this.baseMelee = context.ReadSingle();
            this.baseRanged = context.ReadSingle();
            this.baseSpell = context.ReadSingle();

            this.SetFinalToBase();
        }

        /// <summary>
        /// Sets the chance to resist to be the base chance to resist.
        /// </summary>
        internal void SetFinalToBase()
        {
            this.melee = this.baseMelee;
            this.ranged = this.baseRanged;
            this.spell = this.baseSpell;
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given DamageSourceContainer to be a clone of this DamageSourceContainer.
        /// </summary>
        /// <param name="clone">
        /// The DamageSourceContainer to setup as a clone of this DamageSourceContainer.
        /// </param>
        public void SetupClone( DamageSourceContainer clone )
        {
            clone.melee = this.melee;
            clone.ranged = this.ranged;
            clone.spell = this.spell;

            clone.baseMelee = this.baseMelee;
            clone.baseRanged = this.baseRanged;
            clone.baseSpell = this.baseSpell;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the base multipliers for the various DamageSources.
        /// </summary>
        private float baseMelee = 1.0f, baseRanged = 1.0f, baseSpell = 1.0f;

        /// <summary>
        /// Stores the final multipliers for the various DamageSources.
        /// </summary>
        private float melee = 1.0f, ranged = 1.0f, spell = 1.0f;

        /// <summary>
        /// Stores the final multipliers for the various DamageSources.
        /// </summary>
        private int fixedMelee, fixedRanged, fixedSpell;

        /// <summary>
        /// Identifies the AuraList of the statable entity that owns this DamageSourceContainer.
        /// </summary>
        private AuraList auraList;

        #endregion
    }
}