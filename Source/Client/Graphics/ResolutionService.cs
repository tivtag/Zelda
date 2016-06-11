
namespace Zelda.Graphics
{
    using System;
    using Atom;
    using Atom.Diagnostics;
    using Atom.Math;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ResolutionService : IResolutionService
    {
        /// <summary>
        /// Gets a value indicating whether the current AspectRatio is a wide-screen ratio.
        /// </summary>
        public bool IsWideAspectRatio
        {
            get
            {
                return this.aspectRatio == AspectRatio.Wide16to9 || this.aspectRatio == AspectRatio.Wide16to10;
            }             
        }

        /// <summary>
        /// Gets the size of the area the game is drawing to.
        /// </summary>
        /// <remarks>
        /// The view size differs from the client bounds of the game window
        /// as in that the game is drawn using the ViewSize,
        /// and then rescaled to fill up the game window.
        /// </remarks>
        /// <value>
        /// This value is constant and
        /// won't change during the game.
        /// </value>
        public Point2 ViewSize
        {
            get
            {
                return this.viewSize;
            }
        }

        /// <summary>
        /// Gets the aspect ratio that the game uses.
        /// </summary>
        /// <value>
        /// This value is constant and
        /// won't change during the game.
        /// </value>
        public AspectRatio AspectRatio
        {
            get
            {
                return this.aspectRatio;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the game is in fullscreen mode.
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                return Settings.Instance.IsFullscreen;
            }
        }

        /// <summary>
        /// Gets the size of the area the game is drawing to.
        /// </summary>
        /// <remarks>
        /// The view size differs from the client bounds of the game window
        /// as in that the game is drawn using the ViewSize,
        /// and then rescaled to fill up the game window.
        /// </remarks>
        public Point2 OutputSize
        {
            get
            {
                if( this.IsFullscreen )
                {
                    return new Point2(
                        this.desktopDisplayMode.Width,
                        this.desktopDisplayMode.Height
                    );
                }
                else
                {
                    return this.ViewSize;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ResolutionService class.
        /// </summary>
        /// <param name="graphics">
        /// The graphics device manager with which the graphics device will be created.
        /// </param>
        /// <param name="log">
        /// Provides a mechanism that allows logging of information.
        /// </param>
        public ResolutionService( Microsoft.Xna.Framework.GraphicsDeviceManager graphics, ILog log )
        {
            this.log = log;
            this.desktopDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            
            Settings settings = Settings.Instance;
            this.SetAspectRatio( settings.AspectRatio );

            int width = settings.Width;
            int height = settings.Height;
            bool iFullscreen = IsFullscreen;

            if( iFullscreen )
            {
                width = this.desktopDisplayMode.Width;
                height = this.desktopDisplayMode.Height;
            }

            graphics.PreferredBackBufferWidth  = width <= 0 ? ViewSize.X : width;
            graphics.PreferredBackBufferHeight = height <= 0 ? ViewSize.Y : height;
            graphics.IsFullScreen = iFullscreen;
        }

        private void SetAspectRatio( AspectRatio aspectRatio )
        {
            this.aspectRatio = aspectRatio;
            this.viewSize = GetViewSizeFor( this.aspectRatio );
        }
        
        /// <summary>
        /// Gets the size that the backbuffer should have.
        /// </summary>
        /// <param name="originalSize">
        /// The original size of the backbuffer.
        /// </param>
        /// <returns>
        /// The adjusted backbuffer size.
        /// </returns>
        public Point2 GetAdjustedBackBufferSize( Point2 originalSize )
        {
            if( originalSize.X < this.ViewSize.X )
                originalSize.X = this.ViewSize.X;

            if( originalSize.Y < this.ViewSize.Y )
                originalSize.Y = this.ViewSize.Y;

            if( IsFullscreen )
            {
                originalSize.X = desktopDisplayMode.Width;
                originalSize.Y = desktopDisplayMode.Height;
            }
            else
            {
                // Scale to keep the ratio.
                double ratio = ViewSize.X / (double)ViewSize.Y;
                int width = MathUtilities.GetNearestMul( originalSize.X, ViewSize.X );
                int height = (int)(width / ratio);

                originalSize.X = width;
                originalSize.Y = height;
            }

            return originalSize;
        }

        /// <summary>
        /// Gets the resolution at which the game is originally would be rendered
        /// at when using the given aspect-ratio.
        /// </summary>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        private static Point2 GetViewSizeFor( AspectRatio aspectRatio )
        {
            // Default : 360x240 reso; 22.5 x 15.00 tiles; 1.5  aspect ratio
            // 16:9    : 432x243 reso; 27.0 x 15.18 tiles; 1.7* aspect ratio
            // 16:10   : 432x270 reso; 27.0 x 16.87 tiles; 1.6  aspect ratio

            switch( aspectRatio )
            {
                case AspectRatio.Wide16to9:
                    return new Point2( 432, 243 );

                case AspectRatio.Wide16to10:
                    return new Point2( 432, 270 );

                default:
                case AspectRatio.Normal:
                    return new Point2( 360, 240 );
            }
        }

        /// <summary>
        /// Ensures that the current aspect ratio is supported.
        /// </summary>
        /// <param name="adapter">
        /// The graphics adapter that will be used.
        /// </param>
        public void EnsureAspectRatioSupport( GraphicsAdapter adapter )
        {
            if( this.IsFullscreen )
                return;
            
            if( this.aspectRatio == Graphics.AspectRatio.Normal )
            {                
                if( IsFlankyGraphicsDriver( adapter ) )
                {
                    this.log.WriteLine( "Detected buggy/flanky graphics driver! Uhuuhuh." );
                    // this.FallbackToNonNormalAspectRatio();
                }
            }
        }

        private void FallbackToNonNormalAspectRatio()
        {
            Zelda.Graphics.AspectRatio newRatio;

            if( this.desktopDisplayMode.AspectRatio == 1.6f )
            {
                newRatio = Graphics.AspectRatio.Wide16to10;
            }
            else
            {
                newRatio = Graphics.AspectRatio.Wide16to9;
            }

            this.SetAspectRatio( newRatio );
            Settings.Instance.AspectRatio = newRatio;
        }

        private static bool IsFlankyGraphicsDriver( GraphicsAdapter adapter )
        {
            string desc = adapter.Description;
            return
                desc.Contains( "Mobile Intel", StringComparison.OrdinalIgnoreCase ) ||
                desc.Contains( "Graphics Media Accelerator", StringComparison.OrdinalIgnoreCase );
        } 

        /// <summary>
        /// The size at which the game is originally rendered at.
        /// </summary>
        private Point2 viewSize;
        
        /// <summary>
        /// The aspect ratio that the game is currently running under.
        /// </summary>
        private Graphics.AspectRatio aspectRatio;

        /// <summary>
        /// Stores the display-mode that the user used just when he entered the game.
        /// </summary>
        private readonly DisplayMode desktopDisplayMode;

        /// <summary>
        /// Provides a mechanism that allows logging of information.
        /// </summary>
        private readonly ILog log;
    }
}
