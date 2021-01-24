// <copyright file="StatTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.StatTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents
{
    using System.Globalization;
    using Zelda.Status;

    /// <summary>
    /// Represents a <see cref="Talent"/> which increases a <see cref="Stat"/> of the Player by a specific value.
    /// This is an abstract 'helper' class.
    /// </summary>
    internal abstract class StatTalent : Talent
    {
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
            if( manipulationType == StatusManipType.Fixed )
            {
                return string.Format( 
                    CultureInfo.CurrentCulture, 
                    Resources.IncreasesXByY,
                    LocalizedEnums.Get( stat ),
                    (level * amountPerLevel).ToString( CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return string.Format( 
                    CultureInfo.CurrentCulture, 
                    Resources.IncreasesXByYPercent,
                    LocalizedEnums.Get( stat ),
                    (level * amountPerLevel).ToString( CultureInfo.CurrentCulture )
                );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatTalent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="stat">
        /// The stat the new StatTalent manipulates.
        /// </param>
        /// <param name="amountPerLevel">
        /// The amount of the Stat provided for each Talent-Level.
        /// </param>
        /// <param name="manipType">
        /// Defines how the talent increases the Stat.
        /// </param>
        /// <param name="maximumLevel">
        /// The maximum number of TalentPoints the Player can invest into the talent.
        /// </param>
        /// <param name="tree">
        /// The TalentTree that 'owns' the new Talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected internal StatTalent( 
                string                localizedName,
                Atom.Xna.Sprite       symbol,
                Stat                  stat,
                float                 amountPerLevel,
                StatusManipType       manipType, 
                int                   maximumLevel,
                TalentTree            tree,
                IZeldaServiceProvider serviceProvider 
            )
            : base( localizedName, symbol, maximumLevel, tree, serviceProvider )
        {
            this.stat           = stat;
            this.manipulationType      = manipType;
            this.amountPerLevel = amountPerLevel;
        }

        /// <summary>
        /// Initializes this StatTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new StatEffect( 0.0f, this.manipulationType, this.stat );

            this.aura = new PermanentAura( this.effect ) {
                Name = this.GetType().ToString() + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this StatTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.effect = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this StatTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.Statable.AuraList.Remove( this.aura );
            this.effect.Value = amountPerLevel * this.Level;
            this.Statable.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States what kind of stat this StatTalent modifies.
        /// </summary>
        private readonly Stat stat;

        /// <summary>
        /// States how the status value is interpreted.
        /// </summary>
        private readonly StatusManipType manipulationType;

        /// <summary>
        /// States the amount this StatTalent increases the specified Stat per talent level.
        /// </summary>
        private readonly float amountPerLevel;

        /// <summary>
        /// Identifies the StatusValueEffect that gets applied by this StatTalent.
        /// </summary>
        private StatEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that stores the StatusValueEffect applied by this StatTalent.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
