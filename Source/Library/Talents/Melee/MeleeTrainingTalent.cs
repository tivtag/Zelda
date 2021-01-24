// <copyright file="MeleeTrainingTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Melee.MeleeTrainingTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Melee
{
    /// <summary>
    /// Defines the root talent of the Melee sub-TalentTree.
    /// Increases Strength by 1 per level.
    /// </summary>
    internal sealed class MeleeTrainingTalent : StatRootTalent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeleeTrainingTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the MeleeTrainingTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal MeleeTrainingTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_MeleeTraining,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_AttackMelee" ),
                Zelda.Status.Stat.Strength,
                tree,
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this MeleeTrainingTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            Talent[] followingTalents = new Talent[3] {
                this.Tree.GetTalent( typeof( BashTalent        ) ),
                this.Tree.GetTalent( typeof( BattleShoutTalent ) ),            
                this.Tree.GetTalent( typeof( ToughnessTalent   ) )
            };

            this.SetTreeStructure( null, followingTalents );
        }
    }
}
