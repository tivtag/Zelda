// <copyright file="ArrowRushTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.ArrowRushTalent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents.Ranged
{
    using Atom.Math;
    using Zelda.Skills;

    /// <summary>
    /// You gain a 6/9/12/15/18% chance to unleash a Multi Shot
    /// when you fire a normal arrow.
    /// </summary>
    internal sealed class ArrowRushTalent : Talent
    {
        /// <summary>
        /// Gets the chance for the Arrow Rush to proc at the given level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// The chance to proc as a value from 0..100.
        /// </returns>
        private static float GetChanceToProc( int level )
        {
            // 6 9 12 15 18
            return 3 + (3 * level);
        }

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
                TalentResources.TD_ArrowRush,
                GetChanceToProc(level).ToString( culture )
            );
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrowRushTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new GoForTheHeadTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ArrowRushTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_ArrowRush,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ArrowRush" ),
                5,
                tree,
                serviceProvider
            )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Setups the connections of this ArrowRushTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            this.multiShotTalent = this.Tree.GetTalent<MultiShotTalent>();
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.multiShotTalent, 3 )
            };
            
            this.SetTreeStructure( requirements, followingTalents : null );
        }
        
        /// <summary>
        /// Initializes this ArrowRushTalent.
        /// </summary>
        protected override void Initialize()
        {
            Skill rangedSkill = this.GetNormalRangedSkill();
            rangedSkill.Fired += this.OnNormalRangedSkillFired;
        }

        /// <summary>
        /// Uninitializes this ArrowRushTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            Skill rangedSkill = this.GetNormalRangedSkill();
            rangedSkill.Fired -= this.OnNormalRangedSkillFired;
        }

        /// <summary>
        /// Gets the NormalRangedAttackSkill of the player.
        /// </summary>
        /// <returns>
        /// The Skill that triggers the Arrow Rush effect.
        /// </returns>
        private Skill GetNormalRangedSkill()
        {
            return this.Owner.Skills.Get( typeof( Skills.Ranged.NormalRangedAttackSkill ) );
        }

        /// <summary>
        /// Raised when the player has sucessfully fired the NormalRangedAttackSkill.
        /// </summary>
        /// <param name="skill">
        /// The skill that has been fired.
        /// </param>
        private void OnNormalRangedSkillFired( Skill skill )
        {
            if( this.Procs() )
            {
                this.FireMultiShot();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Arrow Rush Talent should proc
        /// now.
        /// </summary>
        /// <returns>
        /// true if it should fire the Multi Shot;
        /// or otherwise false.
        /// </returns>
        private bool Procs()
        {
            return this.rand.UncheckedRandomRange( 0.0f, 100.0f ) <= this.chance;
        }

        /// <summary>
        /// Fires a MultiShot.
        /// </summary>
        private void FireMultiShot()
        {
            var attack = this.multiShotTalent.Skill.Attack;
            attack.FireMultiShot();
        }

        /// <summary>
        /// Refreshes the strength of this ArrowRushTalent.
        /// </summary>
        protected override void Refresh()
        {
            this.chance = GetChanceToProc( this.Level );
        }

        /// <summary>
        /// Captures the current chance that the Arrow Rush effect 
        /// will proc when the player fires a normal arrow.
        /// </summary>
        private float chance;

        /// <summary>
        /// Represents the talent that is used to fiire the Multi Shots. 
        /// </summary>
        private MultiShotTalent multiShotTalent;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;
    }
}
