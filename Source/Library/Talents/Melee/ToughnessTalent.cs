// <copyright file="ToughnessTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Melee.ToughnessTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Melee
{
    /// <summary>
    /// Toughness increases armor by 6% per level for a total of 30%. 
    /// </summary>
    /// <remarks>
    /// Armor decreases damage taken by physical attacks.
    /// </remarks>
    internal sealed class ToughnessTalent : ArmorTalent
    {
        /// <summary>
        /// The armor increase in % provided by the talent.
        /// </summary>
        private const int ArmorIncrease = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToughnessTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new ToughnessTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ToughnessTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_Toughness,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_ShieldBlock" ),
                5,
                Zelda.Status.StatusManipType.Percental,
                ArmorIncrease,
                tree,
                serviceProvider 
            )
        {
        }

        /// <summary>
        /// Setups this ToughnessTalent.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[] {
                new TalentRequirement( this.Tree.GetTalent( typeof( MeleeTrainingTalent ) ), 1 )
            };

            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( VitalityTalent ) ),
                this.Tree.GetTalent( typeof( DoubleAttackTalent ) ),
                this.Tree.GetTalent( typeof( BattleAwarenessTalent  ) )
            };

            this.SetTreeStructure( requirements, following );
        }
    }
}
