// <copyright file="FullBlendInOutUIElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.FullBlendInOutUIElement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System;
    using Atom;
    using Atom.Xna;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an UIElement that can be used to blend the game in/out
    /// using a black overlay.
    /// This class can't be inherited.
    /// </summary>
    public sealed class FullBlendInOutUIElement : ZeldaUIElement
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the FullBlendInOutUIElement class.
        /// </summary>
        public FullBlendInOutUIElement()
        {
            this.IsEnabled = false;
            this.IsVisible = false;

            this.FloorNumber = int.MaxValue;
            this.PassInputToSubElements = true;
        }

        /// <summary>
        /// Setups this Dialog.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.Size = serviceProvider.ViewSize;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tells this FullBlendInOutUIElement to start a new fullscreen blending operation.
        /// </summary>
        /// <param name="time">
        /// The time it should take to blend the screen in/out.
        /// </param>
        /// <param name="isBlendingIn">
        /// States whether this FullBlendInOutUIElement is blending in or out.
        /// </param>
        /// <param name="disableOnEnd">
        /// States whether this FullBlendInOutUIElement automatically disables itself
        /// when the blending effect has ended.
        /// </param>
        public void StartBlending( float time, bool isBlendingIn, bool disableOnEnd )
        {
            this.StartBlending( time, isBlendingIn, disableOnEnd, null );
        }

        /// <summary>
        /// Tells this FullBlendInOutUIElement to start a new fullscreen blending operation.
        /// </summary>
        /// <param name="time">
        /// The time it should take to blend the screen in/out.
        /// </param>
        /// <param name="isBlendingIn">
        /// States whether this FullBlendInOutUIElement is blending in or out.
        /// </param>
        /// <param name="disableOnEnd">
        /// States whether this FullBlendInOutUIElement automatically disables itself
        /// when the blending effect has ended.
        /// </param>
        /// <param name="endedOrReplaced">
        /// The event handler that gets invoked when the blending operation has been completed;
        /// or replaced by another blending operation.
        /// </param>
        public void StartBlending( float time, bool isBlendingIn, bool disableOnEnd, EventHandler endedOrReplaced )
        {
            if( this.isActive )
            {
                this.endedOrReplaced.Raise( this );
            }

            this.time = time;
            this.timeEnd = time;
            this.isBlendingIn = isBlendingIn;
            this.disableOnEnd = disableOnEnd;
            this.endedOrReplaced = endedOrReplaced;
            this.SetEnabledState( true );
        }

        private void SetEnabledState( bool value )
        {
            this.isActive = value;
            this.IsEnabled = value;
            this.IsVisible = value;
        }
        
        public void Reset()
        {
            this.SetEnabledState( false );
        }

        /// <summary>
        /// Called when this FullBlendInOutUIElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            drawContext.Batch.DrawRect(
                this.ClientArea,
                new Color( 0, 0, 0, GetAlpha() )
            );
        }

        /// <summary>
        /// Gets the alpha value used when drawing this FullBlendInOutUIElement.
        /// </summary>
        /// <returns>
        /// A value between 0 and 255.
        /// </returns>
        private byte GetAlpha()
        {
            return (byte)(255 * GetRatio());
        }

        /// <summary>
        /// Gets the ratio between that specifies the
        /// current position in the blending operation.
        /// </summary>
        /// <returns>
        /// A value between 0 and 1.
        /// </returns>
        private float GetRatio()
        {
            float ratio = this.time / this.timeEnd;

            if( this.isBlendingIn )
            {
                return ratio;
            }
            else
            {
                return 1.0f - ratio;
            }
        }

        /// <summary>
        /// Called when this FullBlendInOutUIElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            this.time -= updateContext.FrameTime;

            if( this.time <= 0.0f )
            {
                this.time = 0.0f;

                if( this.isActive )
                {
                    // Must be set before OnEnded is called
                    // or we might get a stack-overflow.
                    this.isActive = false;

                    this.OnEnded();
                }
            }
        }

        /// <summary>
        /// Gets called when the blending operation has completed.
        /// </summary>
        private void OnEnded()
        {
            if( this.disableOnEnd )
            {
                this.IsEnabled = false;
                this.IsVisible = false;
            }

            if( this.endedOrReplaced != null )
            {
                this.endedOrReplaced( this, EventArgs.Empty );
                this.endedOrReplaced = null;
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States whether this FullBlendInOutUIElement is blending in or out.
        /// </summary>
        private bool isBlendingIn;

        /// <summary>
        /// States whether this FullBlendInOutUIElement automatically disables itself
        /// when the blending effect has ended.
        /// </summary>
        private bool disableOnEnd;

        /// <summary>
        /// The time (in seconds) left until the blending effect ends.
        /// </summary>
        private float time;

        /// <summary>
        /// The time (in seconds) the total blending effect takes to execute.
        /// </summary>
        private float timeEnd;

        /// <summary>
        /// States whether a blending operation is currently active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Raised when the blending operation has ended or been replaced.
        /// </summary>
        private EventHandler endedOrReplaced;

        #endregion
    }
}
