// <copyright file="RespawnTextDisplay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.RespawnTextDisplay class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using System;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Graphics;
    using Atom.Xna.Fonts;

    /// <summary>
    /// Defines an UIElement that informs the player
    /// that he has died and might respawn now by pressing the Space key.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class RespawnTextDisplay : UIElement
    {
        /// <summary>
        /// Gets or sets the Player entity
        /// registered to this RespawnTextDisplay.
        /// </summary>
        public Zelda.Entities.PlayerEntity Player
        {
            get
            {
                return this.player; 
            }

            set
            {
                // Unhook events.
                if( this.Player != null )
                {
                    this.Player.Respawned     -= this.OnPlayerRespawned;
                    this.Player.Statable.Died -= this.OnPlayerDied;
                }

                this.player = value;

                // Hook events.
                if( this.Player != null )
                {
                    this.Player.Respawned     += this.OnPlayerRespawned;
                    this.Player.Statable.Died += this.OnPlayerDied;
                    RefreshText();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the RespawnTextDisplay class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RespawnTextDisplay( IZeldaServiceProvider serviceProvider )
        {
            this.IsEnabled = false;
            this.IsVisible = false;

            // Create and setup TextField
            var font     = UIFonts.TahomaBold11;     
            var viewSize = serviceProvider.ViewSize;

            this.textField = new TextField() {
                Text = new Text( font, TextAlign.Center, Microsoft.Xna.Framework.Color.White ),
                Position = new Atom.Math.Vector2( viewSize.X / 2.0f, viewSize.Y / 3.0f )
            };
        }

        private void RefreshText()
        {
            this.textField.Text.TextString = player.Profile.Hardcore ? "YOU HAVE DIED\nYour deeds of valor will be remembered" : Resources.PressSpaceToRespawn;
        }

        /// <summary>
        /// Called when the player has died.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnPlayerDied( Zelda.Status.Statable sender )
        {
            ShowText();
        }

        public void ShowText()
        {
            this.ShowAndEnable();
            this.textField.ShowAndEnable();
        }

        /// <summary>
        /// Called when the player has respawned.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contains the event data.
        /// </param>
        private void OnPlayerRespawned( object sender, EventArgs e )
        {
            this.HideAndDisable();
            this.textField.HideAndDisable();
        }

        /// <summary>
        /// Called when this RespawnTextDisplay is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            this.textField.Draw( drawContext );
        }

        /// <summary>
        /// Called when this RespawnTextDisplay is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            this.textField.Update( updateContext );
        }

        /// <summary>
        /// The TextField that is internally used.
        /// </summary>
        private readonly TextField textField;

        /// <summary>
        /// The storage field for the <see cref="Player"/> property.
        /// </summary>
        private Zelda.Entities.PlayerEntity player;
    }
}
