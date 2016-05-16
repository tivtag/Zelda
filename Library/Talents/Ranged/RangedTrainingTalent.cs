// <copyright file="RangedTrainingTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.RangedTrainingTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    /// <summary>
    /// Defines the root talent of the Ranged sub-TalenTree.
    /// Increases Dexterty by 1 per level.
    /// </summary>
    internal sealed class RangedTrainingTalent : StatRootTalent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangedTrainingTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The talent tree that owns the talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RangedTrainingTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_RangedTraining,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_AttackRanged" ),
                Zelda.Status.Stat.Dexterity,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this RangedTrainingTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            Talent[] following = new Talent[3] { 
                Tree.GetTalent( typeof( LightArrowTalent     ) ),
                Tree.GetTalent( typeof( PiercingArrowsTalent ) ),
                Tree.GetTalent( typeof( DodgeTrainingTalent  ) )
            };

            this.SetTreeStructure( null, following );
        }
    }
}
