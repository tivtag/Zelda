// <copyright file="IWorkspace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.IWorkspace interface.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor
{
    using Atom.Xna;

    /// <summary>
    /// Defines the interface a workspace of the Editor
    /// has to implement. Workspaces are run on the XNA-side of the Editor.
    /// </summary>
    public interface IWorkspace : IContentLoadable
    {
        /// <summary>
        /// Gets the <see cref="WorkspaceType"/> of this <see cref="IWorkspace"/>.
        /// </summary>
        WorkspaceType Type { get; }

        /// <summary>
        /// Updates this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="updateContext">
        /// Tje current IUpdateContext.
        /// </param>
        void Update( ZeldaUpdateContext updateContext );

        /// <summary>
        /// Draws this <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        void Draw( ZeldaDrawContext drawContext );

        /// <summary>
        /// Gets called when the application has entered this <see cref="IWorkspace"/>.
        /// </summary>
        void Enter();

        /// <summary>
        /// Gets called when the application has left this <see cref="IWorkspace"/>.
        /// </summary>
        void Leave();

        /// <summary>
        /// Gets called when the user clicks on the Xna-PictureBox.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        void HandleMouseClick( System.Windows.Forms.MouseEventArgs e );
                
        /// <summary>
        /// Gets called when the user presses any key while the window has focus but no other control.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        void HandleKeyDown( System.Windows.Input.KeyEventArgs e );

        /// <summary>
        /// Gets called when the user releases any key while the window has focus but no other control.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        void HandleKeyUp( System.Windows.Input.KeyEventArgs e );

        /// <summary>
        /// Gets called when the user presses the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        void HandleMouseDown( System.Windows.Forms.MouseEventArgs e );

        /// <summary>
        /// Gets called when the user releases the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        void HandleMouseUp( System.Windows.Forms.MouseEventArgs e );
        
        /// <summary>
        /// Gets called when the user moves the mouse on the window.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        void HandleMouseMove( System.Windows.Forms.MouseEventArgs e );
    }
}
