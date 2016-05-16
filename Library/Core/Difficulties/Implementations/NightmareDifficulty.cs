// <copyright file="NightmareDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.NightmareDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Represents the IDifficulty after the <see cref="NormalDifficulty"/>.
    /// </summary>
    public sealed class NightmareDifficulty : BaseDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public override DifficultyId Id
        {
            get
            {
                return DifficultyId.Nightmare;
            }
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public override string Name
        {
            get
            {
                return Resources.Nightmare;
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
                new LifeManaEffect( 25.0f, StatusManipType.Percental, LifeMana.Life ),
                new DamageDoneWithSourceEffect( 25.0f, StatusManipType.Percental, DamageSource.All ),
                new ChanceToBeStatusEffect( 3.0f, StatusManipType.Fixed, ChanceToStatus.Miss ),
                new ChanceToResistEffect( 3.0f, StatusManipType.Fixed, ElementalSchool.All ),
                new MovementSpeedEffect( 28.5f, StatusManipType.Percental ),
                new ArmorEffect( 10.0f, StatusManipType.Percental )
            };
        }
    }
}
