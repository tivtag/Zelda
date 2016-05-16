// <copyright file="StatRootTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.StatRootTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents
{
    using System.Globalization;
    using Zelda.Status;

    /// <summary>
    /// Represents a <see cref="Talent"/> which increases a <see cref="Stat"/> of the Player by a specific value.
    /// This is an abstract 'helper' class.
    /// </summary>
    internal abstract class StatRootTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the fixed stat increase given by this StatRootTalent for the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The fixed stat increase.
        /// </returns>
        private static int GetFixedIncrease( int level )
        {
            return level;
        }

        /// <summary>
        /// Gets the percentual stat increase given by this StatRootTalent for the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The percentual stat increase.
        /// </returns>
        private static float GetPercentualIncrease( int level )
        {
            return level * 1.0f;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the description of this Talent for
        /// the specified talent level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent to get the description for.
        /// </param>
        /// <returns>
        /// The localized description of this Talent for the specified talent level.
        /// </returns>
        protected override string GetDescription( int level )
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                TalentResources.TD_StatRoot,
                LocalizedEnums.Get( stat ),
                GetFixedIncrease(level).ToString( CultureInfo.CurrentCulture ),
                GetPercentualIncrease(level).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatRootTalent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="stat">
        /// The stat the new StatRootTalent manipulates.
        /// </param>
        /// <param name="tree">
        /// The TalentTree that 'owns' the new Talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected internal StatRootTalent(
                string localizedName,
                Atom.Xna.Sprite symbol,
                Stat stat,
                TalentTree tree,
                IZeldaServiceProvider serviceProvider
            )
            : base( localizedName, symbol, 5, tree, serviceProvider )
        {
            this.stat = stat;
        }

        /// <summary>
        /// Initializes this StatRootTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.fixedFffect = new StatEffect( 0.0f, StatusManipType.Fixed, this.stat );
            this.percentalEffect = new StatEffect( 0.0f, StatusManipType.Percental, this.stat );

            this.aura = new PermanentAura( new StatusEffect[] { this.fixedFffect, this.percentalEffect } ) {
                Name = this.GetType().ToString() + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this StatRootTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.fixedFffect = null;
            this.percentalEffect = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this StatRootTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.Statable.AuraList.Remove( this.aura );

            this.fixedFffect.Value = GetFixedIncrease( this.Level );
            this.percentalEffect.Value = GetPercentualIncrease( this.Level );

            this.Statable.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States what kind of stat this StatTalent modifies.
        /// </summary>
        private readonly Stat stat;

        /// <summary>
        /// Identifies the StatusValueEffect that gets applied by this StatTalent.
        /// </summary>
        private StatEffect fixedFffect;

        /// <summary>
        /// Identifies the StatusValueEffect that gets applied by this StatTalent.
        /// </summary>
        private StatEffect percentalEffect;

        /// <summary>
        /// Identifies the PermanentAura that stores the StatusValueEffect applied by this StatTalent.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}