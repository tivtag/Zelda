// <copyright file="MagicalPolarity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.MagicalPolarity class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Represent a BossPolarity that increases damage taken of the boss
    /// towards a specific element.
    /// </summary>
    internal class MagicalPolarity : BossPolarity
    {
        /// <summary>
        /// The damage taken increase the boss takes against the elemental school
        /// of the MagicalPolarity while the MagicalPolarity is active, in percent.
        /// </summary>
        private const float ElementalDamageTakenIncrease = 40.0f;

        /// <summary>
        /// Initializes a new instance of the MagicalPolarity class.
        /// </summary>
        /// <param name="color">
        /// The color of the new MagicalPolarity.
        /// </param>
        /// <param name="school">
        /// The elemental school of the new MagicalPolarity.
        /// </param>
        /// <param name="boss">
        /// Identifies the hell-chicken boss object that owns the new MagicalPolarity.
        /// </param>
        public MagicalPolarity( Atom.Math.Vector4 color, ElementalSchool school, Enemy boss )
            :base( color, CreateAura( school ), boss )
        {
        }

        /// <summary>
        /// Creates the effect that is applied when the boss changes
        /// into a magical polarity.
        /// </summary>
        /// <param name="elementalSchool">
        /// The school of the magical polarity.
        /// </param>
        /// <returns>
        /// A new PermanentAura that encapsulates the effect.
        /// </returns>
        private static PermanentAura CreateAura( ElementalSchool elementalSchool )
        {
            var effect = new ElementalDamageTakenEffect( 
                ElementalDamageTakenIncrease, 
                StatusManipType.Percental,
                elementalSchool 
            );

            return new PermanentAura( effect ) {
                Name = elementalSchool.ToString() + "_Polarity"
            };
        }
    }
}
