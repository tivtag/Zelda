// <copyright file="DoubleAttackTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.DoubleAttackTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    using Atom.Math;
    using Zelda.Attacks.Melee;
    using Zelda.Status;
    
    /// <summary>
    /// The Double Attack Talent gives the player a 5/10/15% chance 
    /// to attack twice on a normal melee attack.
    /// The effect can proc off of itself.
    /// </summary>
    internal sealed class DoubleAttackTalent : Talent
    {
        #region [ Constants ]

        /// <summary>
        /// The chance to proc the Double Attack effect per talent point.
        /// </summary>
        private const float ProcChancePerLevel = 5.0f;

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
                TalentResources.TD_DoubleAttack,
                (level * ProcChancePerLevel).ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleAttackTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the DoubleAttackTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal DoubleAttackTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_DoubleAttack,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_DoubleAttack" ),
                3,
                tree, 
                serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
        }
        
        /// <summary>
        /// Setups the connections of this ImprovedBashTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( ToughnessTalent ) ), 2 )
            };

            Talent[] following = new Talent[2] {
                this.Tree.GetTalent( typeof( QuickStrikeTalent ) ),                
                this.Tree.GetTalent( typeof( FrustrationTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this DoubleAttackTalent.
        /// </summary>
        protected override void Initialize()
        {
            this.damageMethod = new NormalPlayerMeleeDamageMethod();
            this.damageMethod.Setup( this.ServiceProvider );

            this.Statable.DefaultMeleeHit += this.OnDefaultMeleeStrike;
            this.Statable.DefaultMeleeCrit += this.OnDefaultMeleeStrike;
        }

        /// <summary>
        /// Uninitializes this DoubleAttackTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            this.damageMethod = null;

            this.Statable.DefaultMeleeHit -= this.OnDefaultMeleeStrike;
            this.Statable.DefaultMeleeCrit -= this.OnDefaultMeleeStrike;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this DoubleAttackTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.procChance = ProcChancePerLevel * this.Level;
        }

        /// <summary>
        /// Called when the player has managed to hit an enemy with his default attack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The CombatEventArgs that contains the event data.</param>
        private void OnDefaultMeleeStrike( object sender, CombatEventArgs e )
        {
            if( this.rand.RandomRange( 0.0f, 100.0f ) <= this.procChance )
            {
                var attackable = e.Target.Owner.Components.Get<Entities.Components.Attackable>();

                if( attackable != null )
                {
                    // Attack again.
                    attackable.Attack( e.User.Owner, this.damageMethod.GetDamageDone( e.User, e.Target ) );
                }
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the chance the Double Attack effect can proc.
        /// </summary>
        private float procChance;

        /// <summary>
        /// The damage method used to calculate the damage inflicted by the Double Attack.
        /// </summary>
        private NormalPlayerMeleeDamageMethod damageMethod;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        #endregion
    }
}
