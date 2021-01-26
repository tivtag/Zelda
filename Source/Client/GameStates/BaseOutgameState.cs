// <copyright file="BaseOutgameState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.BaseOutgameState class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.Particles;
    using Atom.Xna.Particles.Controllers;
    using Atom.Xna.Particles.Emitters;
    using Atom.Xna.Particles.Modifiers;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Graphics;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents the base class that both the <see cref="CharacterCreationState"/> and <see cref="CharacterSelectionState"/> share.
    /// </summary>
    internal abstract class BaseOutgameState : IGameState
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the large IFont used in the UserInterface.
        /// </summary>
        protected IFont FontLarge
        {
            get
            {
                return this.fontLarge;
            }
        }

        /// <summary>
        /// Gets the normal IFont used in the UserInterface.
        /// </summary>
        protected IFont Font
        {
            get
            {
                return this.font;
            }
        }

        /// <summary>
        /// Gets the UserInterface of this BaseCharacterState.
        /// </summary>
        protected ZeldaUserInterface UserInterface
        {
            get
            {
                return this.userInterface;
            }
        }

        public ZeldaScene Scene => this.scene;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the BaseCharacterState class.
        /// </summary>
        /// <param name="game">
        /// The game that owns the new BaseCharacterState.
        /// </param>
        protected BaseOutgameState( ZeldaGame game )
        {
            this.game = game;
            this.rand = game.Rand;
        }

        /// <summary>
        /// Loads the resources used by this BaseCharacterState.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unloads the resources used by this BaseCharacterState.
        /// </summary>
        public virtual void Unload()
        {
            this.userInterface.RemoveAllElements();
        }

        /// <summary>
        /// Loads and setups the user-interface of this BaseCharacterState.
        /// </summary>
        protected void LoadUserInterface()
        {
            this.userInterface = new ZeldaUserInterface( false );
            this.userInterface.Setup( this.game );
            this.userInterface.KeyboardInput += this.OnKeyboardInputCore;
            this.userInterface.MouseInput += this.OnMouseInputCore;

            this.SetupUserInterface();
        }

        /// <summary>
        /// Allws sub-classes to register controls at the user-interface.
        /// </summary>
        protected abstract void SetupUserInterface();

        #endregion

        #region [ Methods ]

        #region [ Drawing ]

        /// <summary>
        /// Draws this BaseCharacterState.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( IDrawContext drawContext )
        {
            if( !this.isActive )
            {
                return;
            }

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            IDrawingPipeline pipeline = this.game.Graphics.Pipeline;

            pipeline.InitializeFrame( this.scene, this.UserInterface, zeldaDrawContext );
            {
                this.DrawPreScene( zeldaDrawContext );

                pipeline.BeginScene();
                pipeline.EndScene();

                if( this.scene != null )
                {
                    this.DrawBackground( zeldaDrawContext );
                }

                pipeline.BeginUserInterface();
                {
                    this.DrawUserInterface( zeldaDrawContext );
                }
                pipeline.EndUserInterface();
            }
        }

        /// <summary>
        /// Called before the scene is drawn.
        /// </summary>
        /// <param name="zeldaDrawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        protected virtual void DrawPreScene( ZeldaDrawContext zeldaDrawContext )
        {
        }

        /// <summary>
        /// Draws the background and ParticleEffect of the BaseCharacterState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected virtual void DrawBackground( ISpriteDrawContext drawContext )
        {
            drawContext.Begin();
            drawContext.Batch.DrawRect(
                new Rectangle( 0, 0, this.game.ViewSize.X, this.game.ViewSize.Y ),
                BackgroundColor
            );
            drawContext.End();
        }

        /// <summary>
        /// Draws the Normal State of the CharacterSelectionState.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected abstract void DrawUserInterface( ISpriteDrawContext drawContext );

        #endregion

        #region [ Updating ]

        /// <summary>
        /// Updates this BaseCharacterState.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public void Update( IUpdateContext updateContext )
        {
            if( !this.isActive )
            {
                return;
            }

            var zeldaUpdateContext = (ZeldaUpdateContext)updateContext;

            this.UserInterface.Update( zeldaUpdateContext );
            this.Update( zeldaUpdateContext );
        }

        /// <summary>
        /// Updates this BaseCharacterState.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        protected virtual void Update( ZeldaUpdateContext updateContext )
        {
        }

        #endregion

        #region [ Input ]

        /// <summary>
        /// Handles mouse input; called once every frame.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnMouseInputCore( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            this.OnMouseInput( ref mouseState, ref oldMouseState );
        }

        /// <summary>
        /// Handles mouse input; called once every frame.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        protected virtual void OnMouseInput( ref MouseState mouseState, ref MouseState oldMouseState )
        {
        }

        /// <summary>
        /// Handles keyboard input; called once every frame.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        private void OnKeyboardInputCore( object sender, ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            if( this.HandleKeyboardInput( ref keyState, ref oldKeyState ) )
            {
                return;
            }

            this.OnKeyboardInput( ref keyState, ref oldKeyState );
        }

        /// <summary>
        /// Handles keyboard input; called once every frame.
        /// </summary>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        protected abstract void OnKeyboardInput( ref KeyboardState keyState, ref KeyboardState oldKeyState );

        /// <summary>
        /// Handles keyboard input; called once every frame.
        /// </summary>
        /// <param name="keyState">
        /// The current state of the keyboard.
        /// </param>
        /// <param name="oldKeyState">
        /// The state of the keyboard one frame ago.
        /// </param>
        /// <returns>
        /// true if input has been handled;
        /// otherwise false.
        /// </returns>
        private bool HandleKeyboardInput(
            ref Microsoft.Xna.Framework.Input.KeyboardState keyState,
            ref Microsoft.Xna.Framework.Input.KeyboardState oldKeyState )
        {
            if( keyState.IsKeyDown( Keys.Escape ) && oldKeyState.IsKeyUp( Keys.Escape ) )
            {
                this.LeaveToPreviousState();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the user has pressed the Escape key.
        /// </summary>
        protected abstract void LeaveToPreviousState();

        #endregion

        #region [ State ]

        /// <summary>
        /// Gets called when the focus has changed from the given IGameState to this IGameState.
        /// </summary>
        /// <param name="oldState">
        /// The old IGameState.
        /// </param>
        public virtual void ChangedFrom( IGameState oldState )
        {
            var sceneProvider = oldState as ISceneProvider;

            if( sceneProvider != null )
            {
                this.scene = sceneProvider.Scene;
                this.BackgroundColor = Xna.Color.Black.WithAlpha( 20 );
            }
            else
            {
                this.scene = null;
                this.BackgroundColor = Xna.Color.Black;
            }

            this.game.GetService<FlyingTextManager>()
                .IsVisible = false;
            this.isActive = true;
        }

        /// <summary>
        /// Gets called when the focus has changed away from this IGameState to the given IGameState.
        /// </summary>
        /// <param name="newState">
        /// The new IGameState.
        /// </param>
        public virtual void ChangedTo( IGameState newState )
        {
            this.game.GetService<FlyingTextManager>()
                .IsVisible = true;

            this.scene = null;
            this.isActive = false;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The fonts used in the UserInterface.
        /// </summary>
        private readonly IFont fontLarge = UIFonts.Tahoma14, font = UIFonts.TahomaBold10;

        /// <summary>
        /// The user interface of this BaseCharacterState.
        /// </summary>
        private ZeldaUserInterface userInterface;

        /// <summary>
        /// Represents the game that owns this BaseCharacterState.
        /// </summary>
        private readonly ZeldaGame game;

        /// <summary>
        /// Represents a random number generator.
        /// </summary>
        private readonly IRand rand;

        /// <summary>
        /// States whether this BaseCharacterState is currently active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The scene that is drawn in the background.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// The color of the background rectangle.
        /// </summary>
        private Xna.Color BackgroundColor;

        #endregion
    }
}