// <copyright file="ViewToWindowRescaler.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ViewToWindowRescaler class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Graphics
{
    using System;
    using Atom;
    using Atom.Xna;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.UI;

    /// <summary>
    /// The ViewToWindowRescaler is used to rescale the
    /// image drawn each frame to completly fill the game window.
    /// This allows the user to change the size of the window.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class ViewToWindowRescaler : System.IDisposable, IViewToWindowRescaler
    {
        #region [ Events ]
        
        /// <summary>
        /// Raised when the scaling factor of this ViewToWindowRescaler has changed.
        /// </summary>
        public event EventHandler ScaleChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the target to which this IViewToWindowRescaler draws to.
        /// </summary>
        public RenderTarget2D Target
        {
            get
            {
                return this.scalingTarget;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ViewToWindowRescaler class.
        /// </summary>
        /// <param name="renderTargetFactory">
        /// Provides a mechanism that allows the creation of RenderTargets.
        /// </param>
        /// <param name="game">
        /// The ZeldaGame object.
        /// </param>
        internal ViewToWindowRescaler( IRenderTarget2DFactory renderTargetFactory, ZeldaGame game )
        {
            this.game = game;
            this.renderTargetFactory = renderTargetFactory;
        }

        /// <summary>
        /// Initializes this ViewToWindowRescaler.
        /// </summary>
        public void Initialize()
        {
            this.game.Window.ClientSizeChanged += this.OnWindowClientSizeChanged;
            this.RefreshScalingFactor();
        }
        
        /// <summary>
        /// Loads the content used by this ViewToWindowRescaler.
        /// </summary>
        public void LoadContent()
        {
            this.device = game.GraphicsDevice;

            if( this.scalingTarget != null )
            {
                this.scalingTarget.Dispose();
            }

            this.scalingTarget = this.renderTargetFactory.Create();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Begins drawing to the RenderTarget that is used
        /// to scale the drawn game to fit into the complete game window.
        /// </summary>
        public void Begin()
        {
            this.device.SetRenderTarget( this.scalingTarget );
        }

        /// <summary>
        /// Ends drawing to the RenderTarget and then outputs the result 
        /// to the Back Buffer.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void End( ZeldaDrawContext drawContext )
        {            
            this.device.SetRenderTarget( null );
            this.device.Clear( ClearOptions.Target, Color.Black, 0.0f, 0 );
            Texture2D texture = this.scalingTarget;

            var batch = drawContext.Batch;
            drawContext.Begin( BlendState.Opaque, SamplerState.PointClamp, SpriteSortMode.Immediate );

            batch.Draw( 
                texture,
                this.viewOffset, 
                null, 
                Color.White,
                0.0f, 
                Vector2.Zero, 
                scalingFactor, 
                SpriteEffects.None,
                0.0f
            );

            drawContext.End();
        }

        /// <summary>
        /// Immediatly releases the unmanaged resources used by this ViewToWindowRescaler.
        /// </summary>
        public void Dispose()
        {
            if( this.scalingTarget != null )
            {
                this.scalingTarget.Dispose();
                this.scalingTarget = null;
            }
        }

        /// <summary>
        /// Refreshes the Scaling Factor value.
        /// </summary>
        private void RefreshScalingFactor()
        {
            var viewSize = game.ViewSize;
            var targetBounds = game.Window.ClientBounds;
            var targetSize = new Atom.Math.Point2( targetBounds.Width, targetBounds.Height );
         
            Settings settings = Settings.Instance;

            if( settings.IsFullscreen && !settings.IsFullscreenStretched )
            {
                Atom.Math.Point2 originalTargetSize = targetSize;

                targetSize = Atom.Math.MathUtilities.GetNearestSmallerMul( new Atom.Math.Point2( targetSize.X, targetSize.Y ), viewSize );
                this.viewOffset = (originalTargetSize - targetSize) / 2;
            }
            else
            {
                this.viewOffset = Atom.Math.Point2.Zero;
            }

            if( viewSize.X != targetSize.X || viewSize.Y != targetSize.Y )
            {
                this.scalingFactor.X = targetSize.X / (float)viewSize.X;
                this.scalingFactor.Y = targetSize.Y / (float)viewSize.Y;
            }
            else
            {
                this.scalingFactor = Vector2.One;
            }

            ZeldaUserInterface.ScalingFactor = scalingFactor.ToAtom();
            ZeldaUserInterface.ViewOffset = this.viewOffset;

            // In fullscreen mode the desktop
            // resolution is used; and as such
            // those changes should not be saved.
            if( !settings.IsFullscreen )
            {
                settings.Width = targetSize.X;
                settings.Height = targetSize.Y;
                settings.Save();
            }
        }

        /// <summary>
        /// Called when the window has been resized.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains all event data.</param>
        private void OnWindowClientSizeChanged( object sender, EventArgs e )
        {
            this.RefreshScalingFactor();
            this.ScaleChanged.Raise( this );
        }

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// The currently cached scaling factor.
        /// </summary>
        private Vector2 scalingFactor;

        /// <summary>
        /// The offset from the upper left corner of the screen to the upper left corner
        /// of the actual game view. Used to create black borders.
        /// </summary>
        private Atom.Math.Point2 viewOffset;

        /// <summary>
        /// The RenderTarget that is drawn to, instead of the Back Buffer, if isScaling is true.
        /// The output of the RenderTarget is then drawn into the Back Buffer, rescaled to fit the complete window.
        /// </summary>
        private RenderTarget2D scalingTarget;

        /// <summary>
        /// The Xna graphics device required for rendering.
        /// </summary>
        private GraphicsDevice device;

        /// <summary>
        /// Identifies the ZeldaGame that owns this ViewToWindowRescaler.
        /// </summary>
        private readonly ZeldaGame game;

        /// <summary>
        /// Provides a mechanism that allows the creation of RenderTargets.
        /// </summary>
        private readonly IRenderTarget2DFactory renderTargetFactory;

        #endregion
    }
}