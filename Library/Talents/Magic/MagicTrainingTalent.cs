// <copyright file="MagicTrainingTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Magic.MagicTrainingTalent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Talents.Magic
{
    /// <summary>
    /// Defines the root talent of the Magic sub-TalentTree.
    /// Increases Intelligence by 1 per level.
    /// </summary>
    internal sealed class MagicTrainingTalent : StatRootTalent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicTrainingTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the MagicTrainingTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MagicTrainingTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_MagicTraining,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_MagicTraining" ),
                Zelda.Status.Stat.Intelligence,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this MagicTrainingTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            Talent[] followingTalents = new Talent[2] {
                this.Tree.GetTalent( typeof( FirewhirlTalent ) ),
                this.Tree.GetTalent( typeof( MagicalBalanceTalent ) ),
            };

            this.SetTreeStructure( null, followingTalents );
        }
    }
}
