// <copyright file="ArmorTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.ArmorTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents
{
    using System.Globalization;
    using Zelda.Status;
    
    /// <summary>
    /// A <see cref="Talent"/> which increases the armor of the player by a specific value.
    /// This is an abstract 'helper' class.
    /// </summary>
    internal abstract class ArmorTalent : Talent
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
                    Resources.Armor,
                    (level * amountPerLevel).ToString( CultureInfo.CurrentCulture )
                );
            }
            else
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.IncreasesXByYPercent,
                    Resources.Armor,
                    (level * amountPerLevel).ToString( CultureInfo.CurrentCulture )
                );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorTalent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="maximumLevel">
        /// The maximum number of TalentPoints the Player can invest into the talent.
        /// </param>
        /// <param name="manipulationType">The manip type of the armor increase effect.</param>
        /// <param name="amountPerLevel">The amount of armor increase per talent level.</param>
        /// <param name="tree">
        /// The TalentTree that 'owns' the new Talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected internal ArmorTalent( 
            string                localizedName,
            Atom.Xna.Sprite       symbol, 
            int                   maximumLevel,
            StatusManipType       manipulationType, 
            int                   amountPerLevel,
            TalentTree            tree, 
            IZeldaServiceProvider serviceProvider
        )
            : base( localizedName, symbol, maximumLevel, tree, serviceProvider )
        {
            this.manipulationType = manipulationType;
            this.amountPerLevel   = amountPerLevel;
        }

        /// <summary>
        /// Initializes this ArmorTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new ArmorEffect( 0.0f, this.manipulationType );

            this.aura = new PermanentAura( this.effect ) {
                Name = this.GetType().Name + "_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this ArmorTalent.
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
        /// Refreshes the strength of this ArmorTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = this.amountPerLevel * this.Level;
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the amount of armor this ArmorTalent provides per talent-level.
        /// </summary>
        private readonly int amountPerLevel;

        /// <summary>
        /// Defines how the value of ArmorEffect is interpreted.
        /// </summary>
        private readonly StatusManipType manipulationType;

        /// <summary>
        /// Identifies the status effect appplied by this ArmorTalent.
        /// </summary>
        private ArmorEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the status effect this ArmorTalent applies.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
