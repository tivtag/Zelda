// <copyright file="BehaviourEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Design.BehaviourEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours.Design
{
    using System;
    using Atom.Design;
    using Zelda.Design;

    /// <summary>
    /// Implements an UITypeEditor that allows the creation of
    /// </summary>
    public sealed class BehaviourEditor : BaseItemCreationEditor
    {
        /// <summary>
        /// Creates an instance of the object of the given selected <see cref="Type"/>.
        /// </summary>
        /// <param name="type">
        /// The type that has been selected by the user.
        /// </param>
        /// <returns>
        /// The newly created object.
        /// </returns>
        protected override object CreateObject( Type type )
        {
            return DesignTime.Services.BehaviourManager.GetBehaviourClone( type, null );
        }

        /// <summary>
        /// Gets the types that the user can select in this BehaviourEditor.
        /// </summary>
        /// <returns>
        /// The types the user can select.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return DesignTime.Services.BehaviourManager.KnownBehaviours;
        }
    }
}
