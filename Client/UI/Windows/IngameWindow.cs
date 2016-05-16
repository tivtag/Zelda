// <copyright file="IngameWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.IngameWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Xna.UI;

    /// <summary>
    /// Represents the base class of all Ingame Windows.
    /// </summary>
    internal abstract class IngameWindow : UIElement
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this IngameWindow has been opened.
        /// </summary>
        public event EventHandler Opened;

        #endregion

        #region [ Properties ]

        /// <summary> 
        /// Gets or sets the PlayerEntity whose status is visualized by this IngameWindow.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player
        {
            get
            {
                return this.player;
            }

            set
            {
                this.player = value;
                this.OnPlayerChanged();
            }
        }

        /// <summary>
        /// Gets or sets the next <see cref="IngameWindow"/> in the closed ring of IngameWindows.
        /// </summary>
        public IngameWindow Next
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the previous <see cref="IngameWindow"/> in the closed ring of IngameWindows.
        /// </summary>
        public IngameWindow Previous
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IngameWindow"/> is currently open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.IsEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this IngameWindow can currently be opened.
        /// </summary>
        /// <value>The default value is true.</value>
        public virtual bool CanBeOpened
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region [ Initializiation ]

        /// <summary>
        /// Initializes a new instance of the <see cref="IngameWindow"/> class.
        /// </summary>
        protected IngameWindow()
        {
            this.HideAndDisableNoEvent();
            this.PassInputToSubElements = false;

            // The ingame windows are above the basic UI,
            // such as the HeartBar, the ManaBar, the RubyDisplay or the TimeDisplay.
            this.FloorNumber = 2;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens this <see cref="IngameWindow"/>.
        /// </summary>
        public void Open()
        {
            if( this.IsOpen )
                return;

            this.OpeningCore();
            this.ShowAndEnable();
            this.Owner.FocusedElement = this;
            this.Opened.Raise( this );
        }

        /// <summary>
        /// Closes this <see cref="IngameWindow"/>.
        /// </summary>
        public void Close()
        {
            if( !this.IsOpen )
                return;

            if( this.Owner != null )
            {
                this.Owner.FocusedElement = null;
            }

            this.ClosingCore();
            this.HideAndDisable();
        }

        /// <summary>
        /// Called when this IngameWindow is about to be opened.
        /// </summary>
        private void OpeningCore()
        {
            this.AddChildElementsTo( this.Owner );
            this.Opening();
        }

        /// <summary>
        /// Called when this IngameWindow is about to be closed.
        /// </summary>
        private void ClosingCore()
        {
            this.RemoveChildElementsFrom( this.Owner );
            this.Closing();
        }

        /// <summary>
        /// Gets called when this <see cref="IngameWindow"/> is opening.
        /// </summary>
        protected virtual void Opening()
        {
        }

        /// <summary>
        /// Gets called when this <see cref="IngameWindow"/> is closing.
        /// </summary>
        protected virtual void Closing()
        {
        }

        /// <summary>
        /// Called when the PlayerEntity that owns this IngameWindow has changed.
        /// </summary>
        protected virtual void OnPlayerChanged()
        {
        }

        /// <summary>
        /// Adds the child elements of this IngameWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected virtual void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected virtual void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
        }

        /// <summary>
        /// Gets called when this IngameWindow gets removed from an UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void OnRemoved( Atom.Xna.UI.UserInterface userInterface )
        {
            this.RemoveChildElementsFrom( userInterface );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the Player property.
        /// </summary>
        private Zelda.Entities.PlayerEntity player;

        #endregion
    }
}
