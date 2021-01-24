// <copyright file="CriticalBalanceTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.CriticalBalanceTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Support
{
    using System.Globalization;
    using Zelda.Status;

    /// <summary>
    /// The <see cref="CriticalBalanceTalent"/> increases the bonus a critical attack
    /// has over a normal attack by 3/7/10%.
    /// </summary>
    internal sealed class CriticalBalanceTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// Returns the critical damage bonus provided by the Critical Balance Talent.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The critical damage bonus for the given talent <paramref name="level"/>.
        /// </returns>
        private static float GetCriticalDamageBonus( int level )
        {
            switch( level )
            {
                case 1:
                    return 3.0f;

                case 2:
                    return 7.0f;

                case 3:
                    return 10.0f;

                case 0:
                default:
                    return 0.0f;
            }
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
                TalentResources.TD_CriticalBalance,
                GetCriticalDamageBonus( level ).ToString( CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalBalanceTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new CriticalBalanceTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal CriticalBalanceTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_CriticalBalance,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_CriticalBalance" ),
                3,
                tree,
                serviceProvider
            )
        { 
        }

        /// <summary>
        /// Setups the talent and its connections with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ConcentrateOnTheFactsTalent ) ), 4 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( SmoothedEmblazonmentTalent ) ),
                this.Tree.GetTalent( typeof( SwiftFightStyleTalent ) )
            };

            SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this CriticalBalanceTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.effect = new Zelda.Status.Damage.CriticalDamageBonusEffect(
                0.0f,
                StatusManipType.Percental,
                Zelda.Status.Damage.DamageSource.All
            );

            this.aura = new PermanentAura( this.effect ) {
                Name = "CriticalBalance_Aura"
            };
        }

        /// <summary>
        /// Uninitializes this CriticalBalanceTalent.
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
        /// Refreshes the strength of this CriticalBalanceTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.AuraList.Remove( this.aura );
            this.effect.Value = GetCriticalDamageBonus( this.Level );
            this.AuraList.Add( this.aura );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the passive effect this CriticalBalanceTalent provides.
        /// </summary>
        private Zelda.Status.Damage.CriticalDamageBonusEffect effect;

        /// <summary>
        /// Identifies the PermanentAura that holds the passive StatusEffects this CriticalBalanceTalent provides.
        /// </summary>
        private PermanentAura aura;

        #endregion
    }
}
