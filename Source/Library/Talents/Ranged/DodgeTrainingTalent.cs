// <copyright file="DodgeTrainingTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.DodgeTrainingTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Status;

    /// <summary>
    /// The DodgeTrainingTalent increases the Chance To Dodge by 1% per talent level.
    /// </summary>
    internal sealed class DodgeTrainingTalent : ChanceToStatusTalent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DodgeTrainingTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new DodgeTrainingTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fastaccess to game-related service.
        /// </param>
        internal DodgeTrainingTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_DodgeTraining,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Dodge" ),
                ChanceToStatus.Dodge,
                1,
                StatusManipType.Fixed,
                5,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this DodgeTrainingTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( RangedTrainingTalent ) ), 1 )
            };

            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( AgilityTrainingTalent ) ),                
                this.Tree.GetTalent( typeof( HandEyeCoordinationTalent ) ),                
                this.Tree.GetTalent( typeof( QuickHandsTalent ) )
            };

            this.SetTreeStructure( requirements, following );
        }
    }
}
