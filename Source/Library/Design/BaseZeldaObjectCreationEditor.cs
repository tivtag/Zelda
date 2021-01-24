// <copyright file="ObjectCreationEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Design.ObjectCreationEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Design
{
    /// <summary>
    /// Represents an <see cref="System.Drawing.Design.UITypeEditor"/> that allows the user to create an object.
    /// </summary>
    internal abstract class BaseZeldaObjectCreationEditor : Atom.Design.BaseItemCreationEditor
    {
        /// <summary>
        /// Called after the given object has been created.
        /// </summary>
        /// <param name="obj">
        /// The object which has been created.
        /// </param>
        protected override void SetupCreatedObject( object obj )
        {
            var editNodifier = obj as IManualEditNotifier;
            if( editNodifier != null )
                editNodifier.StartManualEdit();

            var setupable = obj as IZeldaSetupable;
            if( setupable != null )
                setupable.Setup( DesignTime.Services );
        }
    }
}
