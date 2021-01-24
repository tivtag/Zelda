// <copyright file="DamageTypeInfo.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Damage.DamageTypeInfo class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Damage
{
    using System.Diagnostics;
    
    /// <summary>
    /// Encapsulates information that descripes the type of some damage.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageTypeInfo
    {
        /// <summary>
        /// The school of the damage.
        /// </summary>
        public readonly DamageSchool School;

        /// <summary>
        /// The source of the damage.
        /// </summary>
        public readonly DamageSource Source;
        
        /// <summary>
        /// The element of the damage;
        /// only applied with <see cref="DamageSchool.Magical"/>.
        /// </summary>
        public readonly ElementalSchool Element;

        /// <summary>
        /// The special type of the damage.
        /// </summary>
        /// <remarks>
        /// Might consist of multiple combined flags.
        /// </remarks>
        public readonly SpecialDamageType SpecialType;

        /// <summary>
        /// Initializes a new instance of the DamageTypeInfo class.
        /// </summary>
        /// <param name="school">
        /// The school of the damage descriped by the new DamageTypeInfo.
        /// </param>
        /// <param name="source">
        /// The source of the damage descriped by the new DamageTypeInfo.
        /// </param>
        /// <param name="element">
        /// The element of the damage descriped by the new DamageTypeInfo.
        /// </param>
        /// <param name="specialType">
        /// The special type of the damage descriped by the new DamageTypeInfo.
        /// </param>
        private DamageTypeInfo( DamageSchool school, DamageSource source, ElementalSchool element, SpecialDamageType specialType )
        {
            this.School = school;
            this.Source = source;
            this.Element = element;
            this.SpecialType = specialType;
        }

        /// <summary>
        /// Creates a new instance of the DamageTypeInfo class that
        /// represents damage of the <see cref="DamageSchool.Physical"/>. 
        /// </summary>
        /// <param name="source">
        /// The source of the physical damage.
        /// </param>
        /// <returns>
        /// A newly created DamageTypeInfo instance.
        /// </returns>
        public static DamageTypeInfo CreatePhysical( DamageSource source )
        {
            Debug.Assert( source != DamageSource.All && source != DamageSource.None );

            return new DamageTypeInfo( DamageSchool.Physical, source, ElementalSchool.None, SpecialDamageType.None );
        }

        /// <summary>
        /// Creates a new instance of the DamageTypeInfo class that
        /// represents damage of the <see cref="DamageSchool.Magical"/>. 
        /// </summary>
        /// <param name="source">
        /// The source of the magical damage.
        /// </param>
        /// <param name="element">
        /// The element of the damage.
        /// </param>
        /// <returns>
        /// A newly created DamageTypeInfo instance.
        /// </returns>
        public static DamageTypeInfo CreateMagical( DamageSource source, ElementalSchool element )
        {
            Debug.Assert( source != DamageSource.All && source != DamageSource.None );
            Debug.Assert( element != ElementalSchool.All && element != ElementalSchool.None );

            return new DamageTypeInfo( DamageSchool.Magical, source, element, SpecialDamageType.None );
        }

        /// <summary>
        /// Creates a new instance of the DamageTypeInfo class.
        /// </summary>
        /// <param name="school">
        /// The school of the damage.
        /// </param>
        /// <param name="source">
        /// The source of the magical damage.
        /// </param>
        /// <param name="element">
        /// The element of the damage.
        /// </param>
        /// <param name="specialType">
        /// The special type of the damage.
        /// </param>
        /// <returns>
        /// A newly created DamageTypeInfo instance.
        /// </returns>
        public static DamageTypeInfo Create( DamageSchool school, DamageSource source, ElementalSchool element, SpecialDamageType specialType )
        {
            Debug.Assert( school != DamageSchool.All && school != DamageSchool.None );
            Debug.Assert( source != DamageSource.All && source != DamageSource.None );
            Debug.Assert( element != ElementalSchool.All && element != ElementalSchool.None );

            return new DamageTypeInfo( school, source, element, specialType );
        }

        /// <summary>
        /// Creates a new DamageTypeInfo instance based on this DamageTypeInfo instance; but with the given ElementalSchool.
        /// </summary>
        /// <param name="element">
        /// The element of the damage.
        /// </param>
        /// <returns>
        /// A newly created DamageTypeInfo instance.
        /// </returns>
        public DamageTypeInfo WithElement( ElementalSchool element )
        {
            return new DamageTypeInfo( this.School, this.Source, element, this.SpecialType );
        }

        /// <summary>
        /// Stores type information about the damage inflicted by a physical melee attack.
        /// </summary>
        public static readonly DamageTypeInfo PhysicalMelee = DamageTypeInfo.CreatePhysical( DamageSource.Melee );

        /// <summary>
        /// Stores type information about the damage inflicted by a physical ranged attack.
        /// </summary>
        public static readonly DamageTypeInfo PhysicalRanged = DamageTypeInfo.CreatePhysical( DamageSource.Ranged );
    }
}
