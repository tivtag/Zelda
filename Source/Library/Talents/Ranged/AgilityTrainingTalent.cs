// <copyright file="AgilityTrainingTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Ranged.AgilityTrainingTalent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents.Ranged
{
    using Zelda.Status;
    
    /// <summary>
    /// The AgilityTrainingTalent provides the player
    /// 5% Agility increase per level for a total increase of +30% Vitality.
    /// </summary>
    internal sealed class AgilityTrainingTalent : StatTalent
    {
        /// <summary>
        /// The agility increase in % provided by the <see cref="AgilityTrainingTalent"/>.
        /// </summary>
        private const float AgilityIncreasePerLevel = 6.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgilityTrainingTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new AgilityTrainingTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fastaccess to game-related service.
        /// </param>
        internal AgilityTrainingTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_AgilityTraining,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_Dodge" ),
                Stat.Agility,
                AgilityIncreasePerLevel,
                StatusManipType.Percental,
                5,
                tree, 
                serviceProvider
            )
        {
        }
        
        /// <summary>
        /// Setups the connections of this AgilityTrainingTalent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( DodgeTrainingTalent ) ), 2 )
            };

            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( RogueWeaponMasteryTalent ) ),
                this.Tree.GetTalent( typeof( GoForTheHeadTalent ) ),
                this.Tree.GetTalent( typeof( SprintTalent       ) )
            };

            this.SetTreeStructure( requirements, following );
        }
    }
}
