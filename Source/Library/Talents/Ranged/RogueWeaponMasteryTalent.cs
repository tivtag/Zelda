// <copyright file="RogueWeaponMasteryTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.RogueWeaponMasteryTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Status;

    /// <summary>
    /// Passively increases damage done with bows by 1/2/3% and damage done with daggery by 2/4/6%.
    /// </summary>
    internal sealed class RogueWeaponMasteryTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Gets the damage increase to bows this talent gives for the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The damage increase in %.
        /// </returns>
        private static float GetDamageIncreaseWithBows( int level )
        {
            return level;
        }

        /// <summary>
        /// Gets the damage increase to daggers this talent gives for the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The damage increase in %.
        /// </returns>
        private static float GetDamageIncreaseWithDaggers( int level )
        {
            return level * 2.0f;
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
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                TalentResources.TD_RogueWeaponMastery,
                GetDamageIncreaseWithBows( level ).ToString( culture ),
                GetDamageIncreaseWithDaggers( level ).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RogueWeaponMasteryTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new RogueWeaponMasteryTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RogueWeaponMasteryTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_RogueWeaponMastery,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_QuickHands" ),
                3,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this RogueWeaponMasteryTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( AgilityTrainingTalent ) ), 2 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this RogueWeaponMasteryTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effectBow = new WeaponDamageTypeBasedEffect( 
                0.0f,
                StatusManipType.Percental,
                Zelda.Items.WeaponType.Bow
            );

            this.effectDagger = new WeaponDamageTypeBasedEffect( 
                0.0f, 
                StatusManipType.Percental,
                Zelda.Items.WeaponType.Dagger
            );

            this.aura = new PermanentAura( new StatusEffect[2] { this.effectBow, this.effectDagger } );
        }

        /// <summary>
        /// Uninitializes this RogueWeaponMasteryTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );

            this.effectBow = null;
            this.effectDagger = null;
            this.aura = null;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this RogueWeaponMasteryTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );

            // Apply new value.
            this.effectBow.Value    = GetDamageIncreaseWithBows( this.Level );
            this.effectDagger.Value = GetDamageIncreaseWithDaggers( this.Level );

            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The aura that contains the passive effect of the Talent.
        /// </summary>
        private PermanentAura aura;

        /// <summary>
        /// The effect of the Talent.
        /// </summary>
        private WeaponDamageTypeBasedEffect effectBow, effectDagger;

        #endregion
    }
}
