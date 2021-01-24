// <copyright file="INpcPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.INpcPropertyWrapper interface.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.NpcCreator
{
    using Atom.Design;

    /// <summary>
    /// Represents the IObjectPropertyWrapper interface used in the Npc Creator tool.
    /// </summary>
    public interface INpcPropertyWrapper : IObjectPropertyWrapper, Atom.IReadOnlyNameable
    {
        /// <summary>
        /// Gets a value indicating whether this NpcPropertyWrapper
        /// requires the Loot tab of the NPC Creator to be accessable.
        /// </summary>
        bool HasLoot
        {
            get;
        }

        /// <summary>
        /// Applies any additional data for this INpcPropertyWrapper.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        void ApplyData( MainWindow window );
        
        /// <summary>
        /// Applies any additional data of this INpcPropertyWrapper to the View.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        void SetupView( MainWindow window );

        /// <summary>
        /// Ensures the correctness of the current state of the NPC.
        /// </summary>    
        /// <returns>
        /// true if the data is in a correct state and can be saved;
        /// otherwise false.
        /// </returns>
        bool Ensure();
    }
}
