// <copyright file="ExtendedDamageDoneContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.Containers.ExtendedDamageDoneContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Damage.Containers
{
    /// <summary>
    /// Encapsulates the damage done properties of <see cref="ExtendedStatable"/> entity.
    /// </summary>
    public sealed class ExtendedDamageDoneContainer : DamageDoneContainer
    {
        /// <summary>
        /// Gets the <see cref="DamageDoneAgainstRaceContainer"/> that encapsulates the
        /// damage done properties that relate to specific <see cref="RaceType"/>s.
        /// </summary>
        public DamageDoneAgainstRaceContainer AgainstRace
        {
            get
            {
                return this.againstRace;
            }
        }

        /// <summary>
        /// Gets the <see cref="SpecialDamageDoneContainer"/> that encapsulates the
        /// damage done properties of damage with a <see cref="SpecialDamageType"/>.
        /// </summary>
        public SpecialDamageDoneContainer WithSpecial
        {
            get
            {
                return this.withSpecial;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedDamageDoneContainer class.
        /// </summary>
        internal ExtendedDamageDoneContainer()
        {
        }

        /// <summary>
        /// Initializes this DamageDoneContainer instance.
        /// </summary>
        /// <param name="statable">
        /// The statable component that should own this DamageDoneContainer.
        /// </param>
        internal override void Initialize( Statable statable )
        {
            var extendedStatable = (ExtendedStatable)statable;

            this.againstRace.Initialize( extendedStatable );
            this.withSpecial.Initialize( extendedStatable );

            base.Initialize( statable );
        }

        /// <summary>
        /// Applies the damage multpliers contained in this ExtendedDamageDoneContainer
        /// to the specified damage that is descriped by the specified DamageTypeInfo.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="target">
        /// The target of the damage.
        /// </param>
        /// <param name="damageTypeInfo">
        /// The type of the damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int Apply( int damage, Statable target, DamageTypeInfo damageTypeInfo )
        {
            damage = this.againstRace.Apply( damage, target.Race );
            damage = base.Apply( damage, damageTypeInfo );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers contained in this ExtendedDamageDoneContainer
        /// to the specified damage that is descriped by the specified DamageTypeInfo.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="target">
        /// The target of the damage.
        /// </param>
        /// <param name="damageTypeInfo">
        /// The type of the damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyFixed( int damage, Statable target, DamageTypeInfo damageTypeInfo )
        {
            damage = this.againstRace.ApplyFixed( damage, target.Race );
            damage = base.ApplyFixed( damage, damageTypeInfo );

            return damage;
        }

        /// <summary>
        /// Applies the damage multipliers for a magical spell of the given <see cref="ElementalSchool"/>
        /// targeted against an enemy of the specified RaceType.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="elementalSchool">
        /// The school of spell.
        /// </param>
        /// <param name="targetRace">
        /// The race of the entity that is the target of this damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyMagicalSpell( int damage, ElementalSchool elementalSchool, RaceType targetRace )
        {
            damage = this.WithSchool.ApplyMagical( damage );
            damage = this.WithSource.ApplySpell( damage );

            damage = this.WithElement.Apply( elementalSchool, damage );
            damage = this.againstRace.Apply( damage, targetRace );

            return damage;
        }

        /// <summary>
        /// Applies the fixed damage modifiers for a magical spell of the given <see cref="ElementalSchool"/>
        /// targeted against an enemy of the specified RaceType.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="elementalSchool">
        /// The school of spell.
        /// </param>
        /// <param name="targetRace">
        /// The race of the entity that is the target of this damage.
        /// </param>
        /// <returns>
        /// The output damage.
        /// </returns>
        internal int ApplyFixedMagicalSpell( int damage, ElementalSchool elementalSchool, RaceType targetRace )
        {
            damage = this.WithSchool.ApplyFixedMagical( damage );
            damage = this.WithSource.ApplyFixedSpell( damage );
            damage = this.againstRace.ApplyFixed( damage, targetRace );

            return damage;
        }
        
        /// <summary>
        /// Encapsulates the damage done properties that relate to specific <see cref="RaceType"/>s.
        /// </summary>
        private readonly DamageDoneAgainstRaceContainer againstRace = new DamageDoneAgainstRaceContainer();
        
        /// <summary>
        /// Encapsulates the damage done properties for damage of a <see cref="SpecialDamageType"/>.
        /// </summary>
        private readonly SpecialDamageDoneContainer withSpecial = new SpecialDamageDoneContainer();
    }
}
