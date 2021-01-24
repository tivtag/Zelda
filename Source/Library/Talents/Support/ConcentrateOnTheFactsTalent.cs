// <copyright file="ConcentrateOnTheFactsTalent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Talents.Support.ConcentrateOnTheFactsTalent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Talents.Support
{
    using Zelda.Status;

    /// <summary>
    /// The ConcentrateOnTheFactsTalent provides the Player
    /// with an increased crit rate of 1% per talent level.
    /// </summary>
    internal sealed class ConcentrateOnTheFactsTalent : ChanceToStatusTalent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentrateOnTheFactsTalent"/> class.
        /// </summary>
        /// <param name="tree">The TalentTree that owns the new ConcentrateOnTheFactsTalent.</param>
        /// <param name="serviceProvider">Provides fast access to game-related services.</param>
        internal ConcentrateOnTheFactsTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base( 
                TalentResources.TN_CotF,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_CotF" ),
                ChanceToStatus.Crit,
                1,
                StatusManipType.Fixed,
                5,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this Talent with the other Talents in the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            Talent[] following = new Talent[3] {
                this.Tree.GetTalent( typeof( MeditationTalent ) ),
                this.Tree.GetTalent( typeof( LuckyBastardTalent ) ),
                this.Tree.GetTalent( typeof( CriticalBalanceTalent ) )
            };            

            this.SetTreeStructure( null, following );
        }
    }
}
