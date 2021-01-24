// <copyright file="ChanceToStatusTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.ChanceToStatusTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents
{
    using System.Globalization;
    using Zelda.Status;

    /// <summary>
    /// A <see cref="Talent"/> which increases any of the chance to statuses of the player by a specific value.
    /// This is an abstract 'helper' class.
    /// </summary>
    internal abstract class ChanceToStatusTalent : Talent
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
            return string.Format(
                CultureInfo.CurrentCulture,
                manipulationType == StatusManipType.Fixed ? Resources.IncreasesChanceToXByY : Resources.IncreasesChanceToXByYPercent,
                LocalizedEnums.Get( statusType ),
                (level * amountPerLevel).ToString( CultureInfo.CurrentCulture ) 
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceToStatusTalent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="statusType">
        /// The <see cref="ChanceToStatus"/> the new ChanceToStatusTalent manipulates.
        /// </param>
        /// <param name="amountPerLevel">
        /// The status-value the new ChanceToStatusTalent provides per level.
        /// </param>
        /// <param name="manipulationType">
        /// States how the status-value should be interpreted.
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
        protected internal ChanceToStatusTalent( 
            string                localizedName, 
            Atom.Xna.Sprite       symbol,
            ChanceToStatus        statusType, 
            int                   amountPerLevel,
            StatusManipType       manipulationType,
            int                   maximumLevel,                                               
            TalentTree            tree,
            IZeldaServiceProvider serviceProvider 
        )
            : base( localizedName, symbol, maximumLevel, tree, serviceProvider )
        {
            this.statusType       = statusType;
            this.amountPerLevel   = amountPerLevel;
            this.manipulationType = manipulationType;
        }

        /// <summary>
        /// Initializes this ChanceToStatusTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new ChanceToStatusEffect( 0.0f, this.manipulationType, this.statusType );

            this.aura = new PermanentAura( this.effect ) {
                Name = this.GetType().ToString() + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this ChanceToStatusTalent.
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
        /// Refreshes the strength of this ChanceToStatusTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.Statable.AuraList.Remove( this.aura );
            this.effect.Value = this.amountPerLevel * this.Level;
            this.Statable.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States what <see cref="ChanceToStatus"/> this ChanceToStatusTalent manipulates.
        /// </summary>
        private readonly ChanceToStatus statusType;

        /// <summary>
        /// States how much of the ChanceToStatus this ChanceToStatusTalent provides per talent level.
        /// </summary>
        private readonly float amountPerLevel;

        /// <summary>
        /// States how the status value of this ChanceToStatusTalent is interpreted.
        /// </summary>
        private readonly StatusManipType manipulationType;

        /// <summary>
        /// Identifies the ChanceToStatusEffect this ChanceToStatusTalent provides.
        /// </summary>
        private ChanceToStatusEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the StatusValueEffect this ChanceToStatusTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
