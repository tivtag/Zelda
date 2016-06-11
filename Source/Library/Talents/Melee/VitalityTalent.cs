// <copyright file="VitalityTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.VitalityTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Melee
{
    using Zelda.Status;

    /// <summary>
    /// The VitalityTalent provides a 7% Vitality increase per level;
    /// for a total increase of +35% Vitality.
    /// </summary>
    internal sealed class VitalityTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// States the vitality increase in % provided by the <see cref="VitalityTalent"/>.
        /// </summary>
        private const float VitalityIncreasePerLevel = 7.0f;

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
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format( 
                culture,
                TalentResources.TD_Vitality,
                (level * VitalityIncreasePerLevel).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="VitalityTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new VitalityTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal VitalityTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_Vitality,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Vitality" ),
                5, 
                tree,
                serviceProvider 
            )
        {
        }

        /// <summary>
        /// Setups the connections of this VitalityTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ToughnessTalent ) ), 1 )
            };

            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( RecoverWoundsTalent ) ),
                this.Tree.GetTalent( typeof( ShieldWallTalent    ) ),
                this.Tree.GetTalent( typeof( ShieldBreakerTalent  ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this VitalityTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new StatEffect( 0.0f, StatusManipType.Percental, Stat.Vitality );
            this.aura = new PermanentAura( this.effect ) {
                Name = "Aura_VitalityTalent"
            };
        }

        /// <summary>
        /// Uninitializes this VitalityTalent.
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
        /// Refreshes the strength of this VitalityTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = this.Level * VitalityIncreasePerLevel;
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the StatEffect this VitalityTalent provides.
        /// </summary>
        private StatEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the Status Effects this VitalityTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
