// <copyright file="ISceneChangeListener.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ISceneChangeListener interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda
{
    /// <summary>
    /// Represents an object that wants to listen
    /// to scene chance messages.
    /// </summary>
    /// <remarks>
    /// This should be confused with the changing of the current Scene of an entity.
    /// A scene change is a higher level concept as in that it represents the complete
    /// change of the current scene to another scene.
    /// </remarks>
    public interface ISceneChangeListener
    {
        /// <summary>
        /// Notifies this ISceneChangeListener that a scene change has occured.
        /// </summary>
        /// <param name="changeType">
        /// States whether the current scene has changed away or to its current scene.
        /// </param>
        void NotifySceneChange( ChangeType changeType );
    }
}
