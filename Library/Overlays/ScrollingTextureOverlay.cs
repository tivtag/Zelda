// <copyright file="ScrollingTextureOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Overlays.ScrollingTextureOverlay class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Overlays
{
    using System;
    using Atom;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// A <see cref="ISceneOverlay"/> that is a texture
    /// that scrolls over the scene, repeating itself.
    /// This is a sealed class.
    /// </summary>
    public sealed class ScrollingTextureOverlay : ISceneOverlay, IReloadable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Xna.Color tinting applied to this <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <value>The default value is Xna.Color.White.</value>
        public Xna.Color Color
        {
            get 
            {
                return this.color;
            }

            set
            { 
                this.color = value; 
            }
        }

        /// <summary>
        /// Gets or sets the alpha value of the ScrollingTextureOverlay's <see cref="Xna.Color"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is greater than 1 or less than 0.
        /// </exception>
        public float Alpha
        {
            get 
            {
                return this.color.A / byte.MaxValue;
            }

            set
            {
                if( value < 0.0f || value > 1.0f )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "value" );

                this.color = new Xna.Color( color.R, color.G, color.B, (byte)(value * byte.MaxValue ) );
            }
        }

        /// <summary>
        /// Gets or sets the scroll speed of this <see cref="ScrollingTextureOverlay"/>.
        /// </summary>
        public Vector2 ScrollSpeed
        {
            get 
            { 
                return this.scrollSpeed;
            }

            set
            {
                this.scrollSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the Scroll value of the ScrollingTextureOverlay.
        /// </summary>
        public Vector2 Scroll
        {
            get
            {
                return this.scroll;
            }

            set
            { 
                this.scroll = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollingTextureOverlay"/> class.
        /// </summary>
        /// <param name="texture">
        /// The overlay's texture. Must be atleast as big as the window.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="texture"/> is null.
        /// </exception>
        public ScrollingTextureOverlay( Texture2D texture )
        {
            if( texture == null )
                throw new ArgumentNullException( "texture" );

            this.texture       = texture;
            this.scrollChanged = new RelaxedEventHandler<ChangedValue<Vector2>>( Camera_ScrollChanged );
        }

        public void Reload( IZeldaServiceProvider serviceProvider )
        {
            this.texture = serviceProvider.TextureLoader.Load( this.texture.Name );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="updateContext">The current <see cref="ZeldaUpdateContext"/>.</param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.scroll += this.scrollSpeed * updateContext.FrameTime;
        }
        
        /// <summary>
        /// Draws this <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current <see cref="ZeldaDrawContext"/>.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            drawContext.Begin( BlendState.NonPremultiplied, SamplerState.LinearWrap, SpriteSortMode.Immediate );           
            var viewSize = drawContext.Camera.ViewSize;

            drawContext.Batch.Draw( 
               this.texture,
               new Xna.Rectangle( 0, 0, viewSize.X, viewSize.Y ),
               new Xna.Rectangle( (int)this.scroll.X, (int)this.scroll.Y, viewSize.X, viewSize.Y ),
               this.color 
            );

            drawContext.End();
        }
        
        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been added to the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        public void AddedToScene( ZeldaScene scene )
        {
            System.Diagnostics.Debug.Assert( scene != null );
            scene.Camera.ScrollChanged += scrollChanged;
        }

        /// <summary>
        /// Gets called when this <see cref="ISceneOverlay"/> has been removed from the given <see cref="ISceneOverlay"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        public void RemovedFromScene( ZeldaScene scene )
        {
            System.Diagnostics.Debug.Assert( scene != null );
            scene.Camera.ScrollChanged -= scrollChanged;
        }

        /// <summary>
        /// Gets called when the scroll property of the Camera has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void Camera_ScrollChanged( object sender, ChangedValue<Vector2> e )
        {
            Vector2 change = e.NewValue - e.OldValue;

            this.scroll.X += change.X / 6.5f;
            this.scroll.Y += change.Y / 8.0f;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the Xna.Color tinting applied to this <see cref="ISceneOverlay"/>.
        /// </summary>
        private Xna.Color color = Xna.Color.White;
        
        /// <summary>
        /// Gets or sets the scroll speed applied each frame.
        /// </summary>
        private Vector2 scrollSpeed = new Vector2( -1.0f, 0.333f );
        
        /// <summary>
        /// Gets or sets the Scroll value of the ScrollingTextureOverlay.
        /// </summary>
        private Vector2 scroll;

        /// <summary>
        /// The fog texture.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Stores the delegate that gets called when the scroll value has changed.
        /// </summary>
        private readonly RelaxedEventHandler<ChangedValue<Vector2>> scrollChanged;

        #endregion
    }
}
