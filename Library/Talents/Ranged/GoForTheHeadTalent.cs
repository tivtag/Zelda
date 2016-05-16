// <copyright file="GoForTheHeadTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.GoForTheHeadTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using System;
    using Zelda.Status;
    
    /// <summary>
    /// Provides a passive effect that increases chance to crit by (0,25% to 1,25%) for each ranged attack that hits.
    /// <para>
    /// This bonus chance is wiped when any attack crits.
    /// </para>
    /// </summary>
    internal sealed class GoForTheHeadTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The crit bonus that is added for each hit.
        /// </summary>
        private const float CritBonusPerHitPerLevel = 0.25f;

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
                TalentResources.TD_GoForTheHead,
                (level * CritBonusPerHitPerLevel).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="GoForTheHeadTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new GoForTheHeadTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal GoForTheHeadTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_GoForTheHead,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_GftH" ),
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this GoForTheHeadTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( AgilityTrainingTalent ) ), 1 )
            };

            Talent[] following = new Talent[1] {
                this.Tree.GetTalent( typeof( HeadshotTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this GoForTheHeadTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new ChanceToStatusEffect( 0.0f, StatusManipType.Fixed, ChanceToStatus.Crit );
            this.aura   = new PermanentAura( this.effect ) {
                Name = "GoForTheHead_Aura"
            };

            this.Statable.RangedHit += this.OnRangedHit;
            this.Statable.MeleeCrit += this.OnCrit;
            this.Statable.RangedCrit += this.OnCrit;
            this.Statable.MagicCrit  += this.OnCrit;
        }

        /// <summary>
        /// Uninitializes this GoForTheHeadTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.AuraList.Remove( this.aura );
            this.effect = null;
            this.aura = null;

            this.Statable.RangedHit -= this.OnRangedHit;
            this.Statable.MeleeCrit -= this.OnCrit;
            this.Statable.RangedCrit -= this.OnCrit;
            this.Statable.MagicCrit  -= this.OnCrit;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes this GoForTheHeadTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.critBonusPerHit = CritBonusPerHitPerLevel * this.Level;
        }

        /// <summary>
        /// Called when the player hits with any ranged ttack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnRangedHit( Statable sender )
        {
            this.AuraList.Remove( this.aura );

            // on a ranged hit we increase the bonus
            this.effect.Value = this.critBonusPerHit * ++this.hitCount;
            
            this.AuraList.Add( aura );
        }

        /// <summary>
        /// Called when the player crits with any attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCrit( Statable sender )
        {
            this.AuraList.Remove( this.aura );

            // on a crit we reset the bonus
            this.effect.Value = 0.0f;
            this.hitCount = 0;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The bonus crit the player gets for each ranged hit until he crits.
        /// </summary>
        private float critBonusPerHit;

        /// <summary>
        /// Stores the number of hits the player had without getting a crit.
        /// </summary>
        private int hitCount;

        /// <summary>
        /// The aura that contains the passive effect of the Talent.
        /// </summary>
        private PermanentAura aura;

        /// <summary>
        /// The effect of the Talent.
        /// </summary>
        private ChanceToStatusEffect effect;

        #endregion
    }
}
