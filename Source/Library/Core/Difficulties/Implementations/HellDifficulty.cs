// <copyright file="HellDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.HellDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Represents the IDifficulty after the <see cref="NightmareDifficulty"/>.
    /// </summary>
    public sealed class HellDifficulty : BaseDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public override DifficultyId Id
        {
            get
            {
                return DifficultyId.Hell;
            }
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public override string Name
        {
            get
            {
                return Resources.Hell;
            }
        }

        /// <summary>
        /// Creates the StatusValueEffects that are applied to enemies.
        /// </summary>
        /// <returns>
        /// The newly created StatusValueEffects.
        /// </returns>
        protected override StatusValueEffect[] CreateStatusEffects()
        {
            return new StatusValueEffect[] {
                new LifeManaEffect( 40.0f, StatusManipType.Percental, LifeMana.Life ),
                new DamageDoneWithSourceEffect( 40.0f, StatusManipType.Percental, DamageSource.All ),
                new ChanceToBeStatusEffect( 4.0f, StatusManipType.Fixed, ChanceToStatus.Miss ),
                new ChanceToResistEffect( 4.0f, StatusManipType.Fixed, ElementalSchool.All ),
                new MovementSpeedEffect( 32.25f, StatusManipType.Percental ),
                new ArmorEffect( 15.0f, StatusManipType.Percental )
            };
        }
    }
}
