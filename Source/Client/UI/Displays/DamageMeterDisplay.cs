// <copyright file="DamageMeterDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.DamageMeterDisplay class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System.Globalization;
    using Atom.Xna.Fonts;
    using Zelda.Attacks;
    using Zelda.Entities;

    /// <summary>
    /// Represents an UIElement that displays the current Damage Done per seconds
    /// of the player.
    /// </summary>
    internal sealed class DamageMeterDisplay : ZeldaUIElement
    {
        /// <summary>
        /// Gets or sets the <see cref="PlayerEntity"/> whose Damage Per Second
        /// is displayed by this DamageMeterDisplay.
        /// </summary>
        public PlayerEntity Player
        {
            get
            {
                return this.player;
            }

            set
            {
                this.player = value;
                this.OnIsEnabledChanged();
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageMeterDisplay"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal DamageMeterDisplay( IZeldaServiceProvider serviceProvider )
        {
            var viewSize = serviceProvider.ViewSize;
            this.Position = new Atom.Math.Vector2( 5.0f, 40.0f );

            this.HideAndDisable();
        }

        /// <summary>
        /// Toggles this DamageMeterDisplay on and off.
        /// </summary>
        internal void Toggle()
        {
            if( this.IsEnabled )
            {
                this.HideAndDisable();
            }
            else
            {
                this.ShowAndEnable();
            }
        }

        /// <summary>
        /// Called when this DamageMeterDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            this.font.Draw(
                this.cachedDpsString,
                this.Position,
                Microsoft.Xna.Framework.Color.White,
                drawContext
            );
        }

        /// <summary>
        /// Called when this DamageMeterDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            this.damageMeter.Update( (ZeldaUpdateContext)updateContext );
            this.UpdateCache( updateContext );
        }

        /// <summary>
        /// Updates the string caching related logic.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        private void UpdateCache( Atom.IUpdateContext updateContext )
        {
            this.timeLeftUntilCacheUpdate -= updateContext.FrameTime;

            if( this.timeLeftUntilCacheUpdate <= 0.0f )
            {
                this.UpdateCachedDps();
                this.timeLeftUntilCacheUpdate = TimeBetweenCacheUpdates;
            }
        }

        /// <summary>
        /// Updates the cachedDpsString and cachedDpsValue if required.
        /// </summary>
        private void UpdateCachedDps()
        {
            float dps = this.GetCurrentDamagePerSecond();
            this.SetCachedDps( dps );
        }

        /// <summary>
        /// Caches the specified dps value.
        /// </summary>
        /// <param name="dps">
        /// The dps value to set.
        /// </param>
        private void SetCachedDps( float dps )
        {
            if( dps != this.cachedDpsValue )
            {
                this.cachedDpsString = GetDpsString( dps );
                this.cachedDpsValue = dps;
            }
        }

        /// <summary>
        /// Gets the string drawn for the given dps value.
        /// </summary>
        /// <param name="dps">
        /// The damage per seconds value.
        /// </param>
        /// <returns>
        /// The damage per seconds string.
        /// </returns>
        private static string GetDpsString( float dps )
        {
            return dps.ToString( CultureInfo.CurrentCulture ) + " dps";
        }

        /// <summary>
        /// Gets the current damage per seconds; as displayed to the player.
        /// </summary>
        /// <returns></returns>
        private float GetCurrentDamagePerSecond()
        {
            return (float)System.Math.Round( this.damageMeter.DamagePerSecond, 1 );
        }

        /// <summary>
        /// Called when this DamageMeterDisplay gets enabled or disabled.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            if( this.IsEnabled )
            {
                this.damageMeter.AttackerToFollow = this.Player != null ? this.Player.Attackable : null;
            }
            else
            {
                this.damageMeter.AttackerToFollow = null;
            }

            this.damageMeter.Reset();
            this.SetCachedDps( 0.0f );
        }

        /// <summary>
        /// Represents the storage property of the Player property.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// The DamageMeter that is used by this DamageMeterDisplay.
        /// </summary>
        private readonly DamageMeter damageMeter = new DamageMeter();

        /// <summary>
        /// States the time between updates of the Damage Per Second string.
        /// </summary>
        private const float TimeBetweenCacheUpdates = 1.0f;

        /// <summary>
        /// The time left until the cached data is updated.
        /// </summary>
        private float timeLeftUntilCacheUpdate;

        /// <summary>
        /// The last cached dps value.
        /// </summary>
        private float cachedDpsValue;

        /// <summary>
        /// The last cached dps value; converted into the string that is displayed to the player.
        /// </summary>
        private string cachedDpsString = GetDpsString( 0.0f );

        /// <summary>
        /// The IFont used to draw the dps string.
        /// </summary>
        private readonly IFont font = UIFonts.TahomaBold10;
    }
}
