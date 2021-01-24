// <copyright file="KnownStatusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Design.KnownStatusEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Design
{
    using System;

    /// <summary>
    /// Enumerates all StatusEffects that are available at design-time.
    /// </summary>
    public static class KnownStatusEffect
    {
        /// <summary>
        /// Gets the types of all known StatusEffects.
        /// </summary>
        /// <remarks>
        /// The returned array should not be modified.
        /// </remarks>
        public static Type[] Types
        {
            get
            {
                return KnownStatusEffect.types;
            }
        }

        /// <summary>
        /// The list of effect types supported by the design-time editors.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( StatEffect ),              
            typeof( ChanceToStatusEffect ),              
            typeof( ChanceToBeStatusEffect ),             
            typeof( ChanceToResistEffect ),
          
            typeof( Zelda.Status.Damage.DamageDoneWithSchoolEffect ),
            typeof( Zelda.Status.Damage.DamageDoneWithSourceEffect ),
            typeof( Zelda.Status.Damage.DamageDoneAgainstRaceEffect ),
            typeof( Zelda.Status.Damage.SpecialDamageDoneEffect ),  
            typeof( Zelda.Status.Damage.ElementalDamageDoneEffect ),
            typeof( Zelda.Status.Damage.CriticalDamageBonusEffect ),
            typeof( Zelda.Status.Damage.DamageTakenFromSchoolEffect ),
            typeof( Zelda.Status.Damage.DamageTakenFromSourceEffect ),
            typeof( Zelda.Status.Damage.ElementalDamageTakenEffect ),
                 
            typeof( SpellPowerEffect ),
            typeof( SpellPenetrationEffect ),
            typeof( WeaponDamageTypeBasedEffect ),
            typeof( SpellHasteEffect ),
            typeof( AttackSpeedEffect ),

            typeof( ArmorEffect ),
            typeof( ArmorIgnoreEffect ),    
            typeof( LifeManaEffect ),  
            typeof( LifeManaRegenEffect ),  
            typeof( MovementSpeedEffect ),
            typeof( PushingForceEffect ),
            typeof( BlockValueEffect ),
            typeof( MagicFindEffect ),
            typeof( ExperienceGainedEffect ),
            typeof( LifeManaPotionEffectivenessEffect ),
            typeof( LightRadiusEffect ),
            typeof( ColorEffect ),
            typeof( Procs.TimedStatusProcEffect )
        };
    }
}
