// <copyright file="EntityBehaviourSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.Dialogs.EntityBehaviourSelectionDialog class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator.Dialogs
{
    /// <summary>
    /// Defines a dialog that allows the user to select the IEntityBehaviour use.
    /// </summary>
    internal sealed class EntityBehaviourSelectionDialog : SelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBehaviourSelectionDialog"/> class.
        /// </summary>
        /// <param name="behaviourManager">
        /// The DrawStrategyManager that is used to populate the list of IEntityBehaviours.
        /// </param>
        public EntityBehaviourSelectionDialog( Zelda.Entities.Behaviours.BehaviourManager behaviourManager )
            : base( behaviourManager.KnownBehaviours )
        {
        }
    }
}
