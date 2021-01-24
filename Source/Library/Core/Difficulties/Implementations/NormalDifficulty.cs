// <copyright file="NormalDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.NormalDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Difficulties
{
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Represents the IDifficulty after the <see cref="EasyDifficulty"/>.
    /// </summary>
    public sealed class NormalDifficulty : BaseDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public override DifficultyId Id
        {
            get
            {
                return DifficultyId.Normal;
            }
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public override string Name
        {
            get
            {
                return Resources.Normal;
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
                new LifeManaEffect( 20.0f, StatusManipType.Percental, LifeMana.Life ),
                new DamageDoneWithSourceEffect( 15.0f, StatusManipType.Percental, DamageSource.All ),
                new ChanceToBeStatusEffect( 1.0f, StatusManipType.Fixed, ChanceToStatus.Miss ),
                new ChanceToResistEffect( 1.0f, StatusManipType.Fixed, ElementalSchool.All ),
                new MovementSpeedEffect( 26.0f, StatusManipType.Percental ),
                new ArmorEffect( 5.0f, StatusManipType.Percental )
            };
        }
    }
}
