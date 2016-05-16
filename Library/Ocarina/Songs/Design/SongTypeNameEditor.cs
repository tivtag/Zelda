// <copyright file="SongTypeNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Ocarina.Songs.Design.SongTypeNameEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Ocarina.Songs.Design
{
    using System;
    using System.Collections.Generic;
    using Atom;

    /// <summary>
    /// Defines an UITypeEditor that lets the user select the type-name of an ocarina song.
    /// </summary>
    public sealed class SongTypeNameEditor : Atom.Design.BaseTypeSelectionEditor
    {
        /// <summary>
        /// Gets the final value that is returned by this SongTypeNameEditor.
        /// </summary>
        /// <param name="selectedItem">
        /// The object the used has selected.
        /// </param>
        /// <returns>
        /// The object that is returned from the SongTypeNameEditor.
        /// </returns>
        protected override object GetFinalValue( Atom.Design.NameableObjectWrapper<Type> selectedItem )
        {
            return selectedItem.Object.GetTypeName();
        }

        /// <summary>
        /// Gets the types that the user can select in this SongTypeNameEditor.
        /// </summary>
        /// <returns>
        /// The types the user can select.
        /// </returns>
        protected override IEnumerable<Type> GetTypes()
        {
            return SongTypeNameEditor.types;
        }
        
        /// <summary>
        /// The types the user can select in this SongTypeNameEditor.
        /// </summary>
        private static readonly Type[] types = new Type[] {
            typeof( Teleportation.RouteOfDinTeleportationSong ),
            typeof( Teleportation.FamilyTombsTeleportationSong ),
            typeof( Teleportation.HomeTeleportationSong ),
            typeof( TimeWarpSong ),
            typeof( ZoneResetSong ),
            typeof( ExecuteTriggerSong )
        };
    }
}
