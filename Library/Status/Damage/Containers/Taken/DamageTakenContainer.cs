// <copyright file="DamageTakenContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageTakenContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage.Containers
{
    using Zelda.Saving;
    
    /// <summary>
    /// Encapsulates the damage taken properties of <see cref="Statable"/> entity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageTakenContainer : ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="DamageTakenFromSourceContainer"/> that encapsulates
        /// what the damage taken properties towards specific <see cref="DamageSource"/>s.
        /// </summary>
        public DamageTakenFromSourceContainer FromSource
        {
            get
            {
                return this.fromSource;
            }
        }

        /// <summary>
        /// Gets the <see cref="DamageTakenFromSchoolContainer"/> that encapsulates
        /// what the damage taken properties towards specifics <see cref="DamageSchool"/>.
        /// </summary>
        public DamageTakenFromSchoolContainer FromSchool
        {
            get
            {
                return this.fromSchool;
            }
        }

        /// <summary>
        /// Gets the <see cref="ElementalDamageTakenContainer"/> that encapsulates
        /// what the damage taken properties towards specifics <see cref="ElementalSchool"/>.
        /// </summary>
        public ElementalDamageTakenContainer FromElement
        {
            get
            {
                return this.fromElement;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DamageTakenContainer class.
        /// </summary>
        internal DamageTakenContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageTakenContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this DamageTakenContainer.
        /// </param>
        internal void Initialize( Statable statable )
        {
            this.fromSource.Initialize( statable );
            this.fromSchool.Initialize( statable );
            this.fromElement.Initialize( statable );
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Applies the fixed damage modifiers contained in this DamageTakenContainer
        /// to the specified damage that is descriped by the specified DamageTypeInfo.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="damageTypeInfo">
        /// The type of the damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyFixed( int damage, DamageTypeInfo damageTypeInfo )
        {
            damage = this.fromSchool.ApplyFixed( damageTypeInfo.School, damage );
            damage = this.fromSource.ApplyFixed( damageTypeInfo.Source, damage );

            return damage;
        }

        /// <summary>
        /// Applies the damage multpliers contained in this DamageTakenContainer
        /// to the specified damage that is descriped by the specified DamageTypeInfo.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="damageTypeInfo">
        /// The type of the damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int Apply( int damage, DamageTypeInfo damageTypeInfo )
        {
            damage = this.fromSchool.Apply( damageTypeInfo.School, damage );
            damage = this.fromSource.Apply( damageTypeInfo.Source, damage );
            damage = this.fromElement.Apply( damageTypeInfo.Element, damage );

            return damage;
        }

        /// <summary>
        /// Applies the damage multipliers for Physical Melee damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        public int ApplyPhysicalMelee( int damage )
        {
            damage = this.FromSchool.ApplyPhysical( damage );
            damage = this.FromSource.ApplyMelee( damage );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers for Physical Melee damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        public int ApplyFixedPhysicalMelee( int damage )
        {
            damage = this.FromSchool.ApplyFixedPhysical( damage );
            damage = this.FromSource.ApplyFixedMelee( damage );

            return damage;
        }

        /// <summary>
        /// Applies the damage multipliers for Physical Ranged damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyPhysicalRanged( int damage )
        {
            damage = this.FromSchool.ApplyPhysical( damage );
            damage = this.FromSource.ApplyRanged( damage );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers for Physical Ranged damage.
        /// </summary>
        /// <param name="damage">
        /// The initial input damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyFixedPhysicalRanged( int damage )
        {
            damage = this.FromSchool.ApplyFixedPhysical( damage );
            damage = this.FromSource.ApplyFixedRanged( damage );

            return damage;
        }

        /// <summary>
        /// Applies the damage multiplier for a magical spell of
        /// the specified <see cref="ElementalSchool"/>.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="elementalSchool">
        /// The school of the spell.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyMagicalSpell( int damage, ElementalSchool elementalSchool )
        {
            damage = this.FromSchool.ApplyMagical( damage );
            damage = this.FromSource.ApplySpell( damage );
            damage = this.FromElement.Apply( elementalSchool, damage );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers for a magical spell of
        /// the specified <see cref="ElementalSchool"/>.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="elementalSchool">
        /// The school of the spell.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyFixedMagicalSpell( int damage, ElementalSchool elementalSchool )
        {
            damage = this.FromSchool.ApplyFixedMagical( damage );
            damage = this.FromSource.ApplyFixedSpell( damage );

            return damage;
        }

        #region > Storage <

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

            this.fromSource.Serialize( context );
            this.fromSchool.Serialize( context );
            this.fromElement.Serialize( context );
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

            this.fromSource.Deserialize( context );
            this.fromSchool.Deserialize( context );
            this.fromElement.Deserialize( context );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given DamageTakenContainer to be a clone of this DamageTakenContainer.
        /// </summary>
        /// <param name="clone">
        /// The DamageTakenContainer to setup as a clone of this DamageTakenContainer.
        /// </param>
        public void SetupClone( DamageTakenContainer clone )
        {
            this.fromSource.SetupClone( clone.fromSource );
            this.fromSchool.SetupClone( clone.fromSchool );
            this.fromElement.SetupClone( clone.fromElement );
        }

        #endregion

        #endregion
        
        #region [ Fields ]

        /// <summary>
        /// Encapsulates the damage taken properties that relate to specific <see cref="DamageSource"/>s.
        /// </summary>
        private readonly DamageTakenFromSourceContainer fromSource = new DamageTakenFromSourceContainer();

        /// <summary>
        /// Encapsulates the damage taken properties that relate to specific <see cref="DamageSchool"/>s.
        /// </summary>
        private readonly DamageTakenFromSchoolContainer fromSchool = new DamageTakenFromSchoolContainer();

        /// <summary>
        /// Encapsulates the damage taken properties that relate to specific <see cref="ElementalSchool"/>s.
        /// </summary>
        private readonly ElementalDamageTakenContainer fromElement = new ElementalDamageTakenContainer();

        #endregion
    }
}
