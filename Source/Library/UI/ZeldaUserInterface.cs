// <copyright file="ZeldaUserInterface.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//    Defines the Zelda.UI.ZeldaUserInterface class.
// </summary>
// <author>
//    Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System;
    using Atom.Xna.UI;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents the <see cref="UserInterface"/> used by the Zelda game.
    /// This class can't be inherited.
    /// </summary>
    public class ZeldaUserInterface : UserInterface, IZeldaSetupable, IServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="Dialog"/> object used by this <see cref="ZeldaUserInterface"/>.
        /// </summary>
        /// <remarks>
        /// Usually there is only one Dialog active at a time.
        /// </remarks>
        public Dialog Dialog
        {
            get
            {
                return this.dialog;
            }
        }

        /// <summary>
        /// Gets the <see cref="FullBlendInOutUIElement"/> object used by this <see cref="ZeldaUserInterface"/>.
        /// </summary>
        public FullBlendInOutUIElement BlendElement
        {
            get
            {
                return this.blendElement;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ZeldaScene"/> this ZeldaUserInterface is currently related to.
        /// </summary>
        public ZeldaScene Scene
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scaling factor applied to the
        /// mouse position.
        /// </summary>
        public static Atom.Math.Vector2 ScalingFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of the pixels on the x and y-axis
        /// for which the output image is moved before rendering
        /// it to the screen.
        /// </summary>
        public static Atom.Math.Point2 ViewOffset
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes static members of the ZeldaUserInterface class.
        /// </summary>
        static ZeldaUserInterface()
        {
            ScalingFactor = Atom.Math.Vector2.One;
        }

        /// <summary>
        /// Initializes a new instance of the ZeldaUserInterface class.
        /// </summary>
        /// <param name="providesDialog">
        /// States whether the new ZeldaUserInterface provides access to a <see cref="Dialog"/>.
        /// </param>
        public ZeldaUserInterface( bool providesDialog = true )
            : base( 30 )
        {
            if( providesDialog )
            {
                this.dialog = new Dialog();
            }

            this.blendElement = new FullBlendInOutUIElement();
        }

        /// <summary>
        /// Setups this <see cref="ZeldaUserInterface"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.game = serviceProvider.Game;

            if( this.dialog != null )
            {
                this.dialog.Setup( serviceProvider );
                this.AddElement( dialog );
            }

            this.blendElement.Setup( serviceProvider );
            this.AddElement( blendElement );
        }
        
        /// <summary>
        /// Tries to get the service of the specified type.
        /// </summary>
        /// <param name="serviceType">
        /// The type of service object to get.
        /// </param>
        /// <returns>
        /// The requested service object; or null.
        /// </returns>
        public virtual object GetService( Type serviceType )
        {
            return null;
        }

        /// <summary>
        /// Overwritten to take into account to current ScalingFactor applied
        /// when drawing the actual image of the game.
        /// </summary>
        /// <returns>
        /// The current state of the Mouse that will get cached.
        /// </returns>
        protected override MouseState GetCurrentMouseState()
        {
            var mouseState = Mouse.GetState();

            int scaledX = (int)((mouseState.X - ViewOffset.X) / ScalingFactor.X);
            int scaledY = (int)((mouseState.Y - ViewOffset.Y) / ScalingFactor.Y);

            return new MouseState(
                scaledX,
                scaledY,
                mouseState.ScrollWheelValue,
                mouseState.LeftButton,
                mouseState.MiddleButton,
                mouseState.RightButton,
                mouseState.XButton1,
                mouseState.XButton2
            );
        }

        /// <summary>
        /// Gets a value indicating whether this UserInterface should
        /// be currentl handle input from the user. Can be overwritten to customize.
        /// </summary>
        /// <returns>
        /// true if the game has focus;
        /// otherwise false.
        /// </returns>
        protected override bool ShouldHandleInput()
        {
            return this.game.IsActive;
        }
        
        /// <summary>
        /// Identifies the xna game object that holds information about whether
        /// the game is currently focused.
        /// </summary>
        private Microsoft.Xna.Framework.Game game;

        /// <summary>
        /// The <see cref="Dialog"/> object used by this <see cref="ZeldaUserInterface"/>.
        /// </summary>
        private readonly Dialog dialog;

        /// <summary>
        /// The <see cref="FullBlendInOutUIElement"/> object used by this <see cref="ZeldaUserInterface"/>.
        /// </summary>
        private readonly FullBlendInOutUIElement blendElement;
    }
}
