// <copyright file="EmitterSettingsListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.Design.EmitterSettingsListEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles.Settings.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="List&lt;IEmitterSettings&gt;"/> objects.
    /// This class can't be inherited.
    /// </summary>
    public sealed class EmitterSettingsListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitterSettingsListEditor"/> class.
        /// </summary>
        public EmitterSettingsListEditor()
            : base( typeof( List<IEmitterSettings> ) )
        {
        }

        /// <summary>
        /// Receives the list of types this CollectionEditor can create.
        /// </summary>
        /// <returns>
        /// The list of types the CollectionEditor can create.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return KnownEmitterSetting.Types;
        }
    }   
}