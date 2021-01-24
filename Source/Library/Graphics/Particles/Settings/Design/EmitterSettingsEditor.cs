// <copyright file="EmitterSettingsEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.Design.EmitterSettingsEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles.Settings.Design
{
    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism
    /// that allows the user to create <see cref="IEmitterSettings"/> instances.
    /// </summary>
    internal sealed class EmitterSettingsEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this EmitterSettingsEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<System.Type> GetTypes()
        {
            return KnownEmitterSetting.Types;
        }
    }
}
