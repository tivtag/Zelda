// <copyright file="DamageDoneContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.DamageDoneContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage.Containers
{
    using Zelda.Saving;

    /// <summary>
    /// Encapsulates the damage done properties of <see cref="Statable"/> entity.
    /// </summary>
    public class DamageDoneContainer : ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="DamageDoneWithSourceContainer"/> that encapsulates
        /// what the damage done properties towards specific <see cref="DamageSource"/>s.
        /// </summary>
        public DamageDoneWithSourceContainer WithSource
        {
            get
            {
                return this.withSource;
            }
        }

        /// <summary>
        /// Gets the <see cref="DamageDoneWithSchoolContainer"/> that encapsulates
        /// what the damage done properties towards specifics <see cref="DamageSchool"/>.
        /// </summary>
        public DamageDoneWithSchoolContainer WithSchool
        {
            get
            {
                return this.withSchool;
            }
        }

        /// <summary>
        /// Gets the <see cref="ElementalDamageDoneContainer"/> that encapsulates
        /// what the damage done properties towards specifics <see cref="ElementalSchool"/>.
        /// </summary>
        public ElementalDamageDoneContainer WithElement
        {
            get
            {
                return this.withElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="CriticalDamageBonusContainer"/> that encapsulates
        /// what the damage done properties that are applied when damage has been critical.
        /// </summary>
        public CriticalDamageBonusContainer WithCritical
        {
            get
            {
                return this.withCritical;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the DamageDoneContainer class.
        /// </summary>
        internal DamageDoneContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageDoneContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this DamageDoneContainer.
        /// </param>
        internal virtual void Initialize( Statable statable )
        {
            this.withSource.Initialize( statable );
            this.withSchool.Initialize( statable );
            this.withElement.Initialize( statable );
            this.withCritical.Initialize( statable );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Applies the damage multipliers contained in this DamageDoneContainer
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
        protected int Apply( int damage, DamageTypeInfo damageTypeInfo )
        {
            damage = this.withSchool.Apply( damageTypeInfo.School, damage );
            damage = this.withSource.Apply( damageTypeInfo.Source, damage );
            damage = this.withElement.Apply( damageTypeInfo.Element, damage );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers contained in this DamageDoneContainer
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
        protected int ApplyFixed( int damage, DamageTypeInfo damageTypeInfo )
        {
            damage = this.withSchool.ApplyFixed( damageTypeInfo.School, damage );
            damage = this.withSource.ApplyFixed( damageTypeInfo.Source, damage );

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
        internal int ApplyPhysicalMelee( int damage )
        {
            damage = this.WithSchool.ApplyPhysical( damage );
            damage = this.WithSource.ApplyMelee( damage );

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
        internal int ApplyFixedPhysicalMelee( int damage )
        {
            damage = this.WithSchool.ApplyFixedPhysical( damage );
            damage = this.WithSource.ApplyFixedMelee( damage );

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
            damage = this.WithSchool.ApplyPhysical( damage );
            damage = this.WithSource.ApplyRanged( damage );

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
            damage = this.WithSchool.ApplyFixedPhysical( damage );
            damage = this.WithSource.ApplyFixedRanged( damage );

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
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            this.withSource.Serialize( context );
            this.withSchool.Serialize( context );
            this.withElement.Serialize( context );
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
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.withSource.Deserialize( context );
            this.withSchool.Deserialize( context );
            this.withElement.Deserialize( context );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given DamageDoneContainer to be a clone of this DamageDoneContainer.
        /// </summary>
        /// <param name="clone">
        /// The DamageDoneContainer to setup as a clone of this DamageDoneContainer.
        /// </param>
        public void SetupClone( DamageDoneContainer clone )
        {
            this.withSource.SetupClone( clone.withSource );
            this.withSchool.SetupClone( clone.withSchool );
            this.withElement.SetupClone( clone.withElement );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Encapsulates the damage done properties that relate to specific <see cref="DamageSource"/>s.
        /// </summary>
        private readonly DamageDoneWithSourceContainer withSource = new DamageDoneWithSourceContainer();

        /// <summary>
        /// Encapsulates the damage done properties that relate to specific <see cref="DamageSchool"/>s.
        /// </summary>
        private readonly DamageDoneWithSchoolContainer withSchool = new DamageDoneWithSchoolContainer();

        /// <summary>
        /// Encapsulates the damage done properties that relate to specific <see cref="ElementalSchool"/>s.
        /// </summary>
        private readonly ElementalDamageDoneContainer withElement = new ElementalDamageDoneContainer();

        /// <summary>
        /// Encapsulates the damage multipliers that are applied when damage has been critical.
        /// </summary>
        private readonly CriticalDamageBonusContainer withCritical = new CriticalDamageBonusContainer();

        #endregion
    }
}
