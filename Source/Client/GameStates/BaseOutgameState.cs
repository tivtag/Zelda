// <copyright file="BaseOutgameState.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.BaseOutgameState class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
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
            this.UnloadParticleEffect();
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

        #region > Particle Effects <

        /// <summary>
        /// Copies the references of the ParticleEffect and other objects from
        /// the specified BaseCharacterState into this BaseCharacterState.
        /// </summary>
        /// <param name="otherState">
        /// The other BaseCharacterState.
        /// </param>
        protected void StealParticleEffect( BaseOutgameState otherState )
        {
            this.particleEffect = otherState.particleEffect;
            this.particleVortex = otherState.particleVortex;
            this.gravityModifier = otherState.gravityModifier;
            this.randomizeGravity = !otherState.randomizeGravity;
        }

        /// <summary>
        /// Loads and setups the ParticleEffect shown in the Character Selection Screen.
        /// </summary>
        protected void LoadParticleEffect()
        {
            this.CreateParticleVortex();

            var emitter = this.CreateEmitter();

            //var rand = this.rand;
            //// if( rand.RandomBoolean )
            //{                
            //    float initial = rand.RandomRange( 3.5f, 6.5f );
            //    float mid = initial + rand.RandomRange( -3.0f, 5.0f );
            //    float final = mid + rand.RandomRange( -3.0f, 10.0f );

            //    emitter.Modifiers.Add(
            //        new ScaleModifier( initial, mid, 0.5f, final )
            //    );
            //}

            this.particleEffect = new ParticleEffect( this.game.Graphics.ParticleRenderer );
            this.particleEffect.Emitters.Add( emitter );

            if( this.rand.RandomSingle <= 0.3f )
            {
                this.RandomizeParticleColor();
            }

            var controler = new TriggerController( this.particleEffect ) {
                Position = new Microsoft.Xna.Framework.Vector2( this.game.ViewSize.X / 2.0f, this.game.ViewSize.Y / 2.0f )
            };
            this.particleEffect.Controllers.Add( controler );
        }

        /// <summary>
        /// Creates the vortex effect that follows the mouse of the player.
        /// </summary>
        private void CreateParticleVortex()
        {
            this.particleVortex = new RadialGravityModifier() {
                Radius = this.rand.RandomRange( 35.0f, 80.0f ),
                Strength = this.rand.RandomRange( 60.0f, 100.0f )
            };
        }

        /// <summary>
        /// Creates a random Particle Emitter.
        /// </summary>
        /// <returns>
        /// A newly created Emitter.
        /// </returns>
        private Emitter CreateEmitter()
        {
            switch( this.rand.RandomRange( 0, 3 ) )
            {
                case 1:
                    return this.CreateShapeEmitter();

                case 2:
                    return this.CreateTriforceEmitter();

                case 3:
                    return this.CreateConnectedShapeEmitter();

                case 0:
                default:
                    return this.CreateSpiralEmitter();
            }
        }

        /// <summary>
        /// Creates a Shape Particle Emitter.
        /// </summary>
        /// <returns>
        /// A newly created Emitter.
        /// </returns>
        private Emitter CreateTriforceEmitter()
        {
            float edgeLength = this.rand.RandomRange( 65.0f, 85.0f );

            var points = new List<Vector2>();

            points.Add( new Vector2( 0.0f, 0.0f ) );
            points.Add( new Vector2( edgeLength * 1, -edgeLength * 2 ) );
            points.Add( new Vector2( edgeLength * 2, 0 ) );
            points.Add( new Vector2( edgeLength * 1, 0 ) );
            points.Add( new Vector2( edgeLength * 1.5f, -edgeLength * 1.0f ) );
            points.Add( new Vector2( edgeLength * 0.5f, -edgeLength * 1.0f ) );
            points.Add( new Vector2( edgeLength * 1, 0 ) );
            points.Add( new Vector2( 20.0f, 0.0f ) );

            Vector2 offset = new Vector2( -edgeLength, edgeLength * 0.9f );

            for( int i = 0; i < points.Count; ++i )
            {
                points[i] += offset;
            }

            return this.CreateShapeEmitter( points );
        }

        /// <summary>
        /// Creates a ParticleEmitter that represents a connected shape.
        /// </summary>
        /// <returns>
        /// A newly created Emitter.
        /// </returns>
        private Emitter CreateConnectedShapeEmitter()
        {
            var rand = this.rand;
            float innerRadius = rand.RandomRange( 0.0f, 50.0f );
            float outerRadius = rand.RandomRange( 10.0f, 125.0f );
            int spikeCount = rand.RandomRange( 2, 10 );

            Polygon2 polygon = Polygon2.CreateStar( Vector2.Zero, innerRadius, outerRadius, spikeCount );

            var triangles = Atom.Math.DelaunyTriangulation.Triangulate( polygon.Vertices );
            var points = new List<Vector2>( triangles.Count * 3 );

            foreach( IndexedTriangle triangle in triangles )
            {
                points.Add( polygon.Vertices[triangle.IndexA] );
                points.Add( polygon.Vertices[triangle.IndexB] );
                points.Add( polygon.Vertices[triangle.IndexC] );
            }

            return this.CreateShapeEmitter( points );
        }

        /// <summary>
        /// Creates a ParticleEmitter that represents a shape.
        /// </summary>
        /// <returns>
        /// A newly created Emitter.
        /// </returns>
        private Emitter CreateShapeEmitter()
        {
            float innerRadius = rand.RandomRange( 0.0f, 50.0f );
            float outerRadius = rand.RandomRange( 10.0f, 125.0f );
            int spikeCount = rand.RandomRange( 3, 10 );

            Polygon2 polygon = Polygon2.CreateStar( Vector2.Zero, innerRadius, outerRadius, spikeCount );
            return CreateShapeEmitter( polygon );
        }

        /// <summary>
        /// Creates a new ShapeEmitter that visualizes the given points.
        /// </summary>
        /// <param name="points">
        /// The points that represents the shape when connected.
        /// </param>
        /// <returns>
        /// A newly created ShapeEmitter.
        /// </returns>
        private ShapeEmitter CreateShapeEmitter( IEnumerable<Vector2> points )
        {
            var emitter = new ShapeEmitter( points ) {
                ParticleTexture = GetParticleTexture(),
                ReleaseQuantity = rand.RandomRange( 10, 55 ),
                ReleaseScale = (FloatRange)new VariableFloat( rand.RandomRange( 1.5f, 6.5f ), 0.25f ),
                Term = rand.RandomRange( 2.5f, 15.0f ),
                Budget = game.Graphics.SupportsHighDef ? 8000 : 4000
            };

            this.randomizeGravity = rand.RandomBoolean;
            this.gravityModifier = new LinearGravityModifier() {
                Gravity = new Microsoft.Xna.Framework.Vector2( 0.0f, rand.RandomRange( 0.25f, 3.0f ) )
            };

            emitter.Modifiers.Add( this.gravityModifier );

            // Modifiers
            emitter.Modifiers.Add( new ColorModifier( Xna.Color.Red, new Xna.Color( 0, 225, 242, 255 ), 0.65f, Xna.Color.DarkBlue ) );
            emitter.Modifiers.Add( new OpacityModifier( 0.0f, 0.5f, 0.25f, 0.0f ) );
            emitter.Modifiers.Add( (Modifier)this.particleVortex );

            emitter.Initialize();
            return emitter;
        }

        /// <summary>
        /// Creates a Spirtal Particle Emitter.
        /// </summary>
        /// <returns>
        /// A newly created Emitter.
        /// </returns>
        private Emitter CreateSpiralEmitter()
        {
            int rate = rand.RandomRange( 1, 5 );
            float term = rand.RandomRange( 6.0f, 15.0f );

            var emitter = new SpiralEmitter( game.Graphics.SupportsHighDef ? 5000 : 2500, term, rate ) {
                ParticleTexture = GetParticleTexture(),
                ReleaseQuantity = rand.RandomRange( 20, 80 ),
                ReleaseScale = (FloatRange)new VariableFloat( rand.RandomRange( 3.0f, 10.0f ), 0.25f ),
                ReleaseSpeed = (FloatRange)new VariableFloat( rand.RandomRange( -35.0f, -10.0f ), 0.5f ),

                Radius = rand.RandomRange( 10.0f, 80.0f ),
                Direction = rand.RandomBoolean ? TurnDirection.AntiClockwise : TurnDirection.Clockwise
            };

            // Modifiers
            emitter.Modifiers.Add( new OpacityModifier( 0.0f, 0.5f, 0.15f, 0.0f ) );
            emitter.Modifiers.Add( new ColorModifier( Xna.Color.Red, Xna.Color.YellowGreen, 0.5f, Xna.Color.Green ) );
            emitter.Modifiers.Add( (Modifier)this.particleVortex );

            emitter.Initialize();
            return emitter;
        }

        /// <summary>
        /// Gets the Texture2D used by the Particle Effect. 
        /// </summary>
        /// <returns>
        /// A ready-to-be-used Texture2D object.
        /// </returns>
        private Texture2D GetParticleTexture()
        {
            /*
            string[] files = new string[] {
                @"Content/Textures/DefaultParticle2d"
                //@"Content/Textures/Particles/RainDrop2",
                //@"Content/Textures/DefaultParticle2d"
            };

            string file = files[this.rand.RandomRange( 0, files.Length - 1 )];
            */
            return game.TextureLoader.Load( "DefaultParticle2d" );
        }

        /// <summary>
        /// Releases the reference associated with the ParticleEffect of this BaseCharacterState.
        /// </summary>
        private void UnloadParticleEffect()
        {
            this.particleEffect = null;
            this.gravityModifier = null;
        }

        #endregion

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
                return;

            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var pipeline = this.game.Graphics.Pipeline;

            pipeline.InitializeFrame( this.scene, this.UserInterface, zeldaDrawContext );
            {
                this.DrawPreScene( zeldaDrawContext );

                pipeline.BeginScene();
                if( this.scene == null && this.particleEffect != null )
                {
                    this.particleEffect.Render();
                }
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

            if( game.Fps < 24 )
            {
                particleEffect = null;
                this.game.Graphics.ChangePipeline( DrawingPipeline.Normal );
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
                return;

            var zeldaUpdateContext = (ZeldaUpdateContext)updateContext;

            this.UpdateParticleEffect( zeldaUpdateContext );
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

        /// <summary>
        /// Updates the ParticleEffect.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        private void UpdateParticleEffect( IXnaUpdateContext updateContext )
        {
            if( !this.isActive )
                return;

            if( this.particleEffect != null )
            {
                if( this.randomizeGravity && this.gravityModifier != null )
                {
                    this.timeLeftUntilGravityRandomization -= updateContext.FrameTime;

                    if( this.timeLeftUntilGravityRandomization <= 0.0f )
                    {
                        this.gravityModifier.Gravity = new Microsoft.Xna.Framework.Vector2(
                            0.0f,
                            game.Rand.RandomRange( -1.0f, 5.0f )
                        );

                        this.timeLeftUntilGravityRandomization = game.Rand.RandomRange( 0.5f, 4.0f );
                    }
                }

                this.particleEffect.Update( updateContext );
            }
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
            this.particleVortex.Position = new Microsoft.Xna.Framework.Vector2( mouseState.X, mouseState.Y );

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
                return;

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
            if( keyState.IsKeyDown( Keys.Tab ) &&
                oldKeyState.IsKeyUp( Keys.Tab ) )
            {
                var vortex = this.particleVortex as RadialGravityModifier;
                if( vortex != null )
                {
                    if( keyState.IsKeyDown( Keys.LeftShift ) )
                    {
                        vortex.Strength *= 0.5f;
                    }
                    else
                    {
                        vortex.Strength *= 1.5f;
                    }
                }
                return true;
            }

            if( keyState.IsKeyDown( Keys.F1 ) && oldKeyState.IsKeyUp( Keys.F1 ) )
            {
                this.RandomizeBloom();
                return true;
            }

            if( keyState.IsKeyDown( Keys.F2 ) && oldKeyState.IsKeyUp( Keys.F2 ) )
            {
                this.RandomizeBloomExtreme();
                return true;
            }

            if( keyState.IsKeyDown( Keys.F3 ) && oldKeyState.IsKeyUp( Keys.F3 ) )
            {
                this.RandomizeBloomVeryExtreme();
                return true;
            }

            if( keyState.IsKeyDown( Keys.F4 ) && oldKeyState.IsKeyUp( Keys.F4 ) )
            {
                this.RandomizeParticleColor();
                return true;
            }

            if( keyState.IsKeyDown( Keys.F9 ) )
            {
                this.ChangeParticleReleaseSpeed( 0.1f );
                return true;
            }

            if( keyState.IsKeyDown( Keys.F10 ) )
            {
                this.ChangeParticleReleaseSpeed( -0.1f );
                return true;
            }

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

        #region [ Particle ]

        /// <summary>
        /// Randomizes the color of the paticle effect.
        /// </summary>
        private void RandomizeParticleColor()
        {
            if( particleEffect != null )
            {
                foreach( var emitter in this.particleEffect.Emitters )
                {
                    var colorModifier = emitter.Modifiers.Find( m => m is ColorModifier );

                    if( colorModifier != null )
                    {
                        emitter.Modifiers.Remove( colorModifier );

                        int keyCount = this.rand.RandomRange( 2, 15 );
                        var keys = new Microsoft.Xna.Framework.Vector4[keyCount];

                        for( int i = 0; i < keyCount; ++i )
                        {
                            keys[i] = new Microsoft.Xna.Framework.Vector4(
                                rand.RandomSingle,
                                rand.RandomSingle,
                                rand.RandomSingle,
                                rand.RandomSingle
                            );
                        }

                        colorModifier = new ColorModifier( keys );
                        emitter.Modifiers.Add( colorModifier );
                    }
                }
            }
        }

        /// <summary>
        /// Changes the speed at which particles are released at by the given amount.
        /// </summary>
        /// <param name="amount">
        /// The amount to change the minimum and maximum release speed by.
        /// </param>
        private void ChangeParticleReleaseSpeed( float amount )
        {
            if( particleEffect != null )
            {
                foreach( var emitter in particleEffect.Emitters )
                {
                    if( emitter.ReleaseQuantity > 0 )
                    {
                        emitter.ReleaseSpeed = new FloatRange( emitter.ReleaseSpeed.Minimum + amount, emitter.ReleaseSpeed.Maximum + amount );
                        ;
                    }
                }
            }
        }

        #endregion

        #region [ Bloom ]

        /// <summary>
        /// Ranomizes the bloom setting sof the BloomDrawingPipeline.
        /// </summary>
        protected void RandomizeBloom()
        {
            var pipeline = this.game.Graphics.BloomPipeline;

            if( pipeline.IsLoaded )
            {
                var rand = this.game.Rand;
                pipeline.Settings.BloomIntensity = rand.RandomRange( 0.0f, 2.0f );
                pipeline.Settings.BaseSaturation = rand.RandomRange( 0.8f, 2.0f );
                pipeline.Settings.BloomSaturation = rand.RandomRange( 0.25f, 2.0f );
                pipeline.Settings.BlurAmount = rand.RandomRange( 0.0f, 25.0f );
            }
        }

        /// <summary>
        /// Ranomizes the bloom setting sof the BloomDrawingPipeline.
        /// </summary>
        private void RandomizeBloomExtreme()
        {
            var pipeline = this.game.Graphics.BloomPipeline;

            if( pipeline.IsLoaded )
            {
                var rand = this.game.Rand;
                pipeline.Settings.BloomIntensity = rand.RandomRange( 0.0f, 20.0f );
                pipeline.Settings.BaseSaturation = rand.RandomRange( 0.0f, 5.0f );
                pipeline.Settings.BloomSaturation = rand.RandomRange( 0.0f, 5.0f );
                pipeline.Settings.BlurAmount = rand.RandomRange( 0.0f, 25.0f );
            }
        }

        /// <summary>
        /// Ranomizes the bloom setting sof the BloomDrawingPipeline.
        /// </summary>
        private void RandomizeBloomVeryExtreme()
        {
            var pipeline = this.game.Graphics.BloomPipeline;

            if( pipeline.IsLoaded )
            {
                var rand = this.game.Rand;
                pipeline.Settings.BloomIntensity = rand.RandomRange( 0.0f, 50.0f );
                pipeline.Settings.BaseSaturation = rand.RandomRange( 0.0f, 50.0f );
                pipeline.Settings.BloomSaturation = rand.RandomRange( 0.0f, 50.0f );
                pipeline.Settings.BlurAmount = rand.RandomRange( 0.0f, 50.0f );
            }
        }

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
                this.BackgroundColor = Xna.Color.Black.WithAlpha( 200 );
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
        /// States whether the gravity of the ParticleEffect should be randomized.
        /// </summary>
        private bool randomizeGravity;

        /// <summary>
        /// The time in seconds until the gravity modifier of the ParticleEffects gets changed.
        /// </summary>
        private float timeLeftUntilGravityRandomization;

        /// <summary>
        /// The ParticleEffect shown in the background.
        /// </summary>
        private ParticleEffect particleEffect;

        /// <summary>
        /// Vortex that applies force to the particles of the ParticleEffect.
        /// </summary>
        private IPositionalModifier particleVortex;

        /// <summary>
        /// The gravity source of the ParticleEffect; if any.
        /// </summary>
        private LinearGravityModifier gravityModifier;

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