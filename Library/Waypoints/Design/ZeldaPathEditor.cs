// <copyright file="ZeldaPathEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Waypoints.Design.ZeldaPathEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Waypoints.Design
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Design;
    using Zelda.Design;

    /// <summary>
    /// Implements an <see cref="BaseItemSelectionEditor{ZeldaPath}"/> that allows the user
    /// to selects a ZeldaPath of the current ZeldaScene.
    /// </summary>
    public sealed class ZeldaPathEditor : BaseItemSelectionEditor<ZeldaPath>
    {
        /// <summary>
        /// Gets the ZeldaPaths of the current ZeldaScene.
        /// </summary>
        /// <returns>
        /// The ZeldaPaths the user is allowed to select.
        /// </returns>
        protected override IEnumerable<ZeldaPath> GetSelectableItems()
        {
            var scene = DesignTime.Services.Resolve<ZeldaScene>();
            return scene.WaypointMap.Paths;
        }
    }
}
